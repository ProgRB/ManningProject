using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManningTable.Helpers;
using System.Data;
using Oracle.DataAccess.Client;
using System.Linq.Expressions;
using System.ComponentModel;
using ManningTable.ViewModel;
using ManningTable.Model;
using Helpers;

namespace ManningTable.View
{
    /// <summary>
    /// Interaction logic for StaffSectionEditor.xaml
    /// </summary>
    public partial class StaffSectionViewer : UserControl, INotifyPropertyChanged
    {
        
        
        public StaffSectionViewer()
        {
            InitializeComponent();

        }
        
        StaffSectionList list_section;
        public StaffSectionList StaffSections
        {
            get
            {
                decimal? crSec = (list_section != null && list_section.CurrentItem != null ? list_section.CurrentItem.StaffSectionID : null);
                StaffSectionList.UpdateStaffSection();
                list_section = StaffSectionList.BuildTree;
                list_section.CurrentItem = list_section[crSec];
                return list_section;
            }
        }

        private void Add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command);
        }

        private void Add_executed(object sender, ExecutedRoutedEventArgs e)
        {
            StaffSectionEditor f = new StaffSectionEditor(null, (StaffSectionTree.SelectedItem == null ? null : (StaffSectionTree.SelectedItem as StaffSection).StaffSectionID));
            f.Owner = Window.GetWindow(this);
            f.Tag = StaffSections;
            if (f.ShowDialog() == true)
            {
                OnPropertyChanged(r => r.StaffSections);
            }
        }
        private void OnPropertyChanged<T>(Expression<Func<StaffSectionViewer, T>> expr)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs((expr.Body as MemberExpression).Member.Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Selected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && StaffSectionTree != null && StaffSectionTree.SelectedItem != null;
        }

        private void Edit_executed(object sender, ExecutedRoutedEventArgs e)
        {
            StaffSectionEditor f = new StaffSectionEditor((StaffSectionTree.SelectedItem as StaffSection).StaffSectionID, null);
            f.Owner = Window.GetWindow(this);
            f.Tag = StaffSections;
            if (f.ShowDialog() == true)
            {
                OnPropertyChanged(r => r.StaffSections);
            }
        }

        OracleCommand cmdDelete;
        private void Delete_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (cmdDelete == null)
            {
                cmdDelete = new OracleCommand(string.Format("begin {0}.STAFF_SECTION_DELETE(:p_staff_section_id);end;", Connect.Schema), Connect.CurConnect);
                cmdDelete.Parameters.Add("p_staff_section_id", OracleDbType.Decimal, 0, ParameterDirection.Input);
            }
            cmdDelete.Parameters[0].Value = (StaffSectionTree.SelectedItem as StaffSection).StaffSectionID;
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                cmdDelete.ExecuteNonQuery();
                tr.Commit();
                OnPropertyChanged(r => r.StaffSections);
            }
            catch (Exception ex)
            {
                tr.Rollback();
                MessageBox.Show(ex.GetFormattedException(), "Ошибка удаления");
            }
        }

        
    }

    
}
