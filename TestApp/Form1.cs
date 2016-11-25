using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibraryKadr;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Connect.NewConnection("KNVTEST", "3");
            InitializeComponent();
            elementHost1.Child = new ManningTable.ManningEditor();
        }
    }
}
