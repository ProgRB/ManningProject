using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Helpers
{
    public class MailBagHelper
    {
        public void InitHooks()
        {
            InterceptKeys.InitHooks();
            InterceptKeys.SetHookCombinations(new Tuple<VirtualKeys, VirtualKeys>[]{
                new Tuple<VirtualKeys, VirtualKeys>( VirtualKeys.LeftControl, VirtualKeys.Snapshot),
                new Tuple<VirtualKeys, VirtualKeys>( VirtualKeys.RightControl, VirtualKeys.Snapshot)});
            InterceptKeys.GlobalKeyPressed += InterceptKeys_GlobalKeyPressed;
        }

        void InterceptKeys_GlobalKeyPressed(object sender, KeyHookEventArgs e)
        {
            //Console.WriteLine(string.Format("{0}+{1}", e.PressedKey, e.Key));
            SendScreenReport();
        }
        public  void DestroyHook()
        {
            InterceptKeys.DestroyHook();
        }

        public void SetRecipients(IEnumerable<string> recipients)
        {
            Recipients = recipients.ToList();
        }

        private List<string> Recipients;

        private void SendScreenReport()
        {
            try
            {
                Thread thread = new Thread(() =>
                {
                    Outlook.Application app;
                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("OUTLOOK");
                    int collCount = processes.Length;
                    if (collCount != 0)
                    {
                        // Outlook already running, hook into the Outlook instance
                        app = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                    }
                    else
                    {
                        app = new Outlook.Application();
                        /*var oNS = app.GetNamespace("MAPI");
                        // Log on by using a dialog box to choose the profile. 
                        oNS.Logon(Type.Missing, Type.Missing, true, true);*/
                    }
                    Outlook.MailItem mail = app.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;
                    mail.Subject = string.Format("Снимок экрана пользователя ({0}, ПК: {1}, процесс: {2}, имя пользователя: {3})"
                            , Environment.UserName, Environment.MachineName, Process.GetCurrentProcess().ProcessName, Connect.UserID);
                    //mail.To = "bmw12714@uuap.com";
                    //mail.Body = "Снимок экрана пользователя";
                    // Add recipient using display name, alias, or smtp address
                    mail.To = string.Join(";", Recipients);

                    int left = Screen.AllScreens.Min(r => r.Bounds.Left);
                    int bottom = Screen.AllScreens.Min(r => r.Bounds.Bottom);

                    int width = Screen.AllScreens.Sum(r => r.Bounds.Width);
                    int height = Screen.AllScreens.Max(r => r.Bounds.Height);

                    var filename = string.Format("{0}{1}.png", System.IO.Path.GetTempPath(), DateTime.Now.Ticks);
                    using (var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (var bmpGraphics = Graphics.FromImage(screenBmp))
                        {
                            bmpGraphics.CopyFromScreen(left, 0, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
                            screenBmp.Save(filename);
                        }
                    }

                    Outlook.Attachment attachment = mail.Attachments.Add(filename, Outlook.OlAttachmentType.olEmbeddeditem, null, "Снимок экрана");
                    string imageCid = "1.png@123";
                    attachment.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x3712001E", imageCid);
                    mail.HTMLBody = String.Format("<body><img src=\"cid:{0}\"></body>", imageCid);
                    mail.Display(true);
                    //System.IO.File.Delete(filename);
                });
                thread.Start();
                thread.Join();             
                
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка формирования письма");
            }
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        }

    }



    public class InterceptKeys
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static bool IsCtrl=false, IsAlt=false, IsShift;
       

        public static void InitHooks()
        {
            _hookID = SetHook(_proc);
        }

        public static void DestroyHook()
        {
            UnhookWindowsHookEx(_hookID);
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static HashSet<VirtualKeys> pressedKeys = new HashSet<VirtualKeys>();

        private static Lookup<VirtualKeys, VirtualKeys> HookedKeyCombinations;
        public static void SetHookCombinations(IEnumerable<Tuple<VirtualKeys, VirtualKeys>> pressedKeysComb)
        {
            HookedKeyCombinations = (Lookup<VirtualKeys, VirtualKeys>)pressedKeysComb.ToLookup(r => r.Item2, r => r.Item1);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var khs = (KeyboardHookStruct)
                          Marshal.PtrToStructure(lParam,
                          typeof(KeyboardHookStruct));
                VirtualKeys v = (VirtualKeys)khs.VirtualKeyCode; 
                //int vkCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine((Key)vkCode);
                /*Console.WriteLine("Hook: Code: {0}, WParam: {1}, lParam={2}, VirtualKeyCode={3}, ScanCode={4}, Flags={5}, Time={6} ",
                            nCode, wParam, lParam,
                            v,
                            khs.ScanCode, khs.Flags, khs.Time);*/
                pressedKeys.Add(v);
                if (HookedKeyCombinations.Contains(v))
                {
                    foreach (var p in HookedKeyCombinations[v])
                    {
                        if (pressedKeys.Contains(p))
                        {
                            OnGlobalKeyPressed(p, v);
                            break;
                        }
                    }
                }
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                var khs = (KeyboardHookStruct)
                          Marshal.PtrToStructure(lParam,
                          typeof(KeyboardHookStruct));
                pressedKeys.Remove((VirtualKeys)khs.VirtualKeyCode);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
            public readonly int VirtualKeyCode;
            public readonly int ScanCode;
            public readonly int Flags;
            public readonly int Time;
            public readonly IntPtr ExtraInfo;
        }

        public static event KeyHookEventHandler GlobalKeyPressed;

        public delegate void KeyHookEventHandler(object sender, KeyHookEventArgs e);

        private static void OnGlobalKeyPressed(VirtualKeys p1, VirtualKeys p2)
        { 
            if (GlobalKeyPressed!=null)
                GlobalKeyPressed(null, new KeyHookEventArgs(p1, p2));
        }
    }

    public class KeyHookEventArgs
    {
        /// <summary>
        /// Зажатая клавиша на клавиатуре
        /// </summary>
        public VirtualKeys PressedKey
        {
            get;set;
        }
        /// <summary>
        /// Вторая нажатая клавиша клавиатуры
        /// </summary>
        public VirtualKeys Key
        {
            get;set;
        }

        /// <summary>
        /// Параметры отчета
        /// </summary>
        /// <param name="pressedKey">зажатая клавиша</param>
        /// <param name="currentKey"> Вторая клавиша</param>
        public KeyHookEventArgs(VirtualKeys pressedKey, VirtualKeys currentKey)
        {
            PressedKey = pressedKey;
            Key = currentKey;
        }
    }

    /// <summary>
    /// Enumeration for virtual keys.
    /// </summary>
    public enum VirtualKeys
        : ushort
    {
        /// <summary></summary>
        LeftButton = 0x01,
        /// <summary></summary>
        RightButton = 0x02,
        /// <summary></summary>
        Cancel = 0x03,
        /// <summary></summary>
        MiddleButton = 0x04,
        /// <summary></summary>
        ExtraButton1 = 0x05,
        /// <summary></summary>
        ExtraButton2 = 0x06,
        /// <summary></summary>
        Back = 0x08,
        /// <summary></summary>
        Tab = 0x09,
        /// <summary></summary>
        Clear = 0x0C,
        /// <summary></summary>
        Return = 0x0D,
        /// <summary></summary>
        Shift = 0x10,
        /// <summary></summary>
        Control = 0x11,
        /// <summary></summary>
        Menu = 0x12,
        /// <summary></summary>
        Pause = 0x13,
        /// <summary></summary>
        CapsLock = 0x14,
        /// <summary></summary>
        Kana = 0x15,
        /// <summary></summary>
        Hangeul = 0x15,
        /// <summary></summary>
        Hangul = 0x15,
        /// <summary></summary>
        Junja = 0x17,
        /// <summary></summary>
        Final = 0x18,
        /// <summary></summary>
        Hanja = 0x19,
        /// <summary></summary>
        Kanji = 0x19,
        /// <summary></summary>
        Escape = 0x1B,
        /// <summary></summary>
        Convert = 0x1C,
        /// <summary></summary>
        NonConvert = 0x1D,
        /// <summary></summary>
        Accept = 0x1E,
        /// <summary></summary>
        ModeChange = 0x1F,
        /// <summary></summary>
        Space = 0x20,
        /// <summary></summary>
        Prior = 0x21,
        /// <summary></summary>
        Next = 0x22,
        /// <summary></summary>
        End = 0x23,
        /// <summary></summary>
        Home = 0x24,
        /// <summary></summary>
        Left = 0x25,
        /// <summary></summary>
        Up = 0x26,
        /// <summary></summary>
        Right = 0x27,
        /// <summary></summary>
        Down = 0x28,
        /// <summary></summary>
        Select = 0x29,
        /// <summary></summary>
        Print = 0x2A,
        /// <summary></summary>
        Execute = 0x2B,
        /// <summary></summary>
        Snapshot = 0x2C,
        /// <summary></summary>
        Insert = 0x2D,
        /// <summary></summary>
        Delete = 0x2E,
        /// <summary></summary>
        Help = 0x2F,
        /// <summary></summary>
        N0 = 0x30,
        /// <summary></summary>
        N1 = 0x31,
        /// <summary></summary>
        N2 = 0x32,
        /// <summary></summary>
        N3 = 0x33,
        /// <summary></summary>
        N4 = 0x34,
        /// <summary></summary>
        N5 = 0x35,
        /// <summary></summary>
        N6 = 0x36,
        /// <summary></summary>
        N7 = 0x37,
        /// <summary></summary>
        N8 = 0x38,
        /// <summary></summary>
        N9 = 0x39,
        /// <summary></summary>
        A = 0x41,
        /// <summary></summary>
        B = 0x42,
        /// <summary></summary>
        C = 0x43,
        /// <summary></summary>
        D = 0x44,
        /// <summary></summary>
        E = 0x45,
        /// <summary></summary>
        F = 0x46,
        /// <summary></summary>
        G = 0x47,
        /// <summary></summary>
        H = 0x48,
        /// <summary></summary>
        I = 0x49,
        /// <summary></summary>
        J = 0x4A,
        /// <summary></summary>
        K = 0x4B,
        /// <summary></summary>
        L = 0x4C,
        /// <summary></summary>
        M = 0x4D,
        /// <summary></summary>
        N = 0x4E,
        /// <summary></summary>
        O = 0x4F,
        /// <summary></summary>
        P = 0x50,
        /// <summary></summary>
        Q = 0x51,
        /// <summary></summary>
        R = 0x52,
        /// <summary></summary>
        S = 0x53,
        /// <summary></summary>
        T = 0x54,
        /// <summary></summary>
        U = 0x55,
        /// <summary></summary>
        V = 0x56,
        /// <summary></summary>
        W = 0x57,
        /// <summary></summary>
        X = 0x58,
        /// <summary></summary>
        Y = 0x59,
        /// <summary></summary>
        Z = 0x5A,
        /// <summary></summary>
        LeftWindows = 0x5B,
        /// <summary></summary>
        RightWindows = 0x5C,
        /// <summary></summary>
        Application = 0x5D,
        /// <summary></summary>
        Sleep = 0x5F,
        /// <summary></summary>
        Numpad0 = 0x60,
        /// <summary></summary>
        Numpad1 = 0x61,
        /// <summary></summary>
        Numpad2 = 0x62,
        /// <summary></summary>
        Numpad3 = 0x63,
        /// <summary></summary>
        Numpad4 = 0x64,
        /// <summary></summary>
        Numpad5 = 0x65,
        /// <summary></summary>
        Numpad6 = 0x66,
        /// <summary></summary>
        Numpad7 = 0x67,
        /// <summary></summary>
        Numpad8 = 0x68,
        /// <summary></summary>
        Numpad9 = 0x69,
        /// <summary></summary>
        Multiply = 0x6A,
        /// <summary></summary>
        Add = 0x6B,
        /// <summary></summary>
        Separator = 0x6C,
        /// <summary></summary>
        Subtract = 0x6D,
        /// <summary></summary>
        Decimal = 0x6E,
        /// <summary></summary>
        Divide = 0x6F,
        /// <summary></summary>
        F1 = 0x70,
        /// <summary></summary>
        F2 = 0x71,
        /// <summary></summary>
        F3 = 0x72,
        /// <summary></summary>
        F4 = 0x73,
        /// <summary></summary>
        F5 = 0x74,
        /// <summary></summary>
        F6 = 0x75,
        /// <summary></summary>
        F7 = 0x76,
        /// <summary></summary>
        F8 = 0x77,
        /// <summary></summary>
        F9 = 0x78,
        /// <summary></summary>
        F10 = 0x79,
        /// <summary></summary>
        F11 = 0x7A,
        /// <summary></summary>
        F12 = 0x7B,
        /// <summary></summary>
        F13 = 0x7C,
        /// <summary></summary>
        F14 = 0x7D,
        /// <summary></summary>
        F15 = 0x7E,
        /// <summary></summary>
        F16 = 0x7F,
        /// <summary></summary>
        F17 = 0x80,
        /// <summary></summary>
        F18 = 0x81,
        /// <summary></summary>
        F19 = 0x82,
        /// <summary></summary>
        F20 = 0x83,
        /// <summary></summary>
        F21 = 0x84,
        /// <summary></summary>
        F22 = 0x85,
        /// <summary></summary>
        F23 = 0x86,
        /// <summary></summary>
        F24 = 0x87,
        /// <summary></summary>
        NumLock = 0x90,
        /// <summary></summary>
        ScrollLock = 0x91,
        /// <summary></summary>
        NEC_Equal = 0x92,
        /// <summary></summary>
        Fujitsu_Jisho = 0x92,
        /// <summary></summary>
        Fujitsu_Masshou = 0x93,
        /// <summary></summary>
        Fujitsu_Touroku = 0x94,
        /// <summary></summary>
        Fujitsu_Loya = 0x95,
        /// <summary></summary>
        Fujitsu_Roya = 0x96,
        /// <summary></summary>
        LeftShift = 0xA0,
        /// <summary></summary>
        RightShift = 0xA1,
        /// <summary></summary>
        LeftControl = 0xA2,
        /// <summary></summary>
        RightControl = 0xA3,
        /// <summary></summary>
        LeftMenu = 0xA4,
        /// <summary></summary>
        RightMenu = 0xA5,
        /// <summary></summary>
        BrowserBack = 0xA6,
        /// <summary></summary>
        BrowserForward = 0xA7,
        /// <summary></summary>
        BrowserRefresh = 0xA8,
        /// <summary></summary>
        BrowserStop = 0xA9,
        /// <summary></summary>
        BrowserSearch = 0xAA,
        /// <summary></summary>
        BrowserFavorites = 0xAB,
        /// <summary></summary>
        BrowserHome = 0xAC,
        /// <summary></summary>
        VolumeMute = 0xAD,
        /// <summary></summary>
        VolumeDown = 0xAE,
        /// <summary></summary>
        VolumeUp = 0xAF,
        /// <summary></summary>
        MediaNextTrack = 0xB0,
        /// <summary></summary>
        MediaPrevTrack = 0xB1,
        /// <summary></summary>
        MediaStop = 0xB2,
        /// <summary></summary>
        MediaPlayPause = 0xB3,
        /// <summary></summary>
        LaunchMail = 0xB4,
        /// <summary></summary>
        LaunchMediaSelect = 0xB5,
        /// <summary></summary>
        LaunchApplication1 = 0xB6,
        /// <summary></summary>
        LaunchApplication2 = 0xB7,
        /// <summary></summary>
        OEM1 = 0xBA,
        /// <summary></summary>
        OEMPlus = 0xBB,
        /// <summary></summary>
        OEMComma = 0xBC,
        /// <summary></summary>
        OEMMinus = 0xBD,
        /// <summary></summary>
        OEMPeriod = 0xBE,
        /// <summary></summary>
        OEM2 = 0xBF,
        /// <summary></summary>
        OEM3 = 0xC0,
        /// <summary></summary>
        OEM4 = 0xDB,
        /// <summary></summary>
        OEM5 = 0xDC,
        /// <summary></summary>
        OEM6 = 0xDD,
        /// <summary></summary>
        OEM7 = 0xDE,
        /// <summary></summary>
        OEM8 = 0xDF,
        /// <summary></summary>
        OEMAX = 0xE1,
        /// <summary></summary>
        OEM102 = 0xE2,
        /// <summary></summary>
        ICOHelp = 0xE3,
        /// <summary></summary>
        ICO00 = 0xE4,
        /// <summary></summary>
        ProcessKey = 0xE5,
        /// <summary></summary>
        ICOClear = 0xE6,
        /// <summary></summary>
        Packet = 0xE7,
        /// <summary></summary>
        OEMReset = 0xE9,
        /// <summary></summary>
        OEMJump = 0xEA,
        /// <summary></summary>
        OEMPA1 = 0xEB,
        /// <summary></summary>
        OEMPA2 = 0xEC,
        /// <summary></summary>
        OEMPA3 = 0xED,
        /// <summary></summary>
        OEMWSCtrl = 0xEE,
        /// <summary></summary>
        OEMCUSel = 0xEF,
        /// <summary></summary>
        OEMATTN = 0xF0,
        /// <summary></summary>
        OEMFinish = 0xF1,
        /// <summary></summary>
        OEMCopy = 0xF2,
        /// <summary></summary>
        OEMAuto = 0xF3,
        /// <summary></summary>
        OEMENLW = 0xF4,
        /// <summary></summary>
        OEMBackTab = 0xF5,
        /// <summary></summary>
        ATTN = 0xF6,
        /// <summary></summary>
        CRSel = 0xF7,
        /// <summary></summary>
        EXSel = 0xF8,
        /// <summary></summary>
        EREOF = 0xF9,
        /// <summary></summary>
        Play = 0xFA,
        /// <summary></summary>
        Zoom = 0xFB,
        /// <summary></summary>
        Noname = 0xFC,
        /// <summary></summary>
        PA1 = 0xFD,
        /// <summary></summary>
        OEMClear = 0xFE
    }
}
