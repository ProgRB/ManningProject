using LibraryKadr;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Helpers.MailBagHelper bagHelper;
        public MainWindow()
        {
            Connect.NewConnection("KNV14534", "5162");
            Helpers.Connect.CurConnect = LibraryKadr.Connect.CurConnect;
            Helpers.Connect.UserID = LibraryKadr.Connect.UserId;
            HashSet<string> _controlRoles = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            if (Helpers.Connect.CurConnect != null)
            {
                OracleCommand cmd = new OracleCommand(string.Format("select * from {0}.user_roles where upper(role_name) in (select granted_role from user_role_privs) or upper(role_name) in (select granted_role from role_role_privs) or upper(trim(role_name)) = upper(trim(ora_login_user))", Connect.Schema), Connect.CurConnect);
                DataTable t = new DataTable();
                new OracleDataAdapter(cmd).Fill(t);
                for (int i = 0; i < t.Rows.Count; ++i)
                    if (!_controlRoles.Contains(t.Rows[i]["Component_Name"].ToString().ToUpper()))
                    {
                        _controlRoles.Add(t.Rows[i]["Component_Name"].ToString().ToUpper());
                    }
            }
            Helpers.ControlRoles.SetControlRoles(_controlRoles);
            InitializeComponent();
            this.Closing+=MainWindow_Closing;
            bagHelper = new Helpers.MailBagHelper();
            bagHelper.InitHooks();
            bagHelper.SetRecipients(new string[] { "BMV12714@uuap.com", "KNV14534@UUAP.COM" });
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bagHelper != null)
                bagHelper.DestroyHook();
        }
    }

    public static class ControlAccess
    {
        private static HashSet<string> componentName;
        /// <summary>
        /// Функция, которая делает элементы 
        /// управления доступными
        /// </summary>
        /// <param name="control"></param>
        /// 
        public static void EnableByRules()
        {
            //Если не заполнино то заполняем
            if (componentName == null)
            {
                componentName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
                OracleDataTable userRoles = new OracleDataTable(
                    string.Format(@"select * from apstaff.USER_ROLES where APP_NAME = 'KADR' 
                        and (role_name in (select granted_role from user_role_privs) 
                            or role_name in (select granted_role from role_role_privs) or role_name = upper(trim(ora_login_user)))
                    order by COMPONENT_NAME"), Connect.CurConnect);
                userRoles.Fill();
                foreach (var p  in userRoles.Rows.OfType<DataRow>().Select(r=>r.Field<string>("COMPONENT_NAME")))
                    componentName.Add(p);
            }
        }

        public static bool GetState(string ControlName)
        {
            return componentName.Contains(ControlName.ToUpper());
        }

        public static bool GetState(ICommand cmd)
        {
            if (cmd is RoutedUICommand)
                return GetState((cmd as RoutedUICommand).Name);
            else return false;
        }
    }
}
