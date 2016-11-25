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
using System.Windows.Shapes;
using Oracle.DataAccess.Client;
using System.Data;
using ManningTable.ViewModel;
using Helpers;

namespace ManningTable.View
{
    /// <summary>
    /// Interaction logic for StaffSectionEditor.xaml
    /// </summary>
    public partial class StaffSectionEditor : Window
    {
        OracleDataAdapter odaStaff_Section;
        DataSet ds;
        public StaffSectionEditor(object p_staffSectionID, object parentSectionID)
        {
            ds = new DataSet();
            odaStaff_Section = new OracleDataAdapter(string.Format("select * from {0}.staff_section where staff_section_id=:p_staff_section_id", Connect.Schema), Connect.CurConnect);
            odaStaff_Section.SelectCommand.BindByName = true;
            odaStaff_Section.SelectCommand.Parameters.Add("p_staff_section_id", OracleDbType.Decimal, p_staffSectionID, ParameterDirection.Input);
            odaStaff_Section.TableMappings.Add("Table", "STAFF_SECTION");

            odaStaff_Section.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_SECTION_UPDATE(:p_STAFF_SECTION_ID,:p_NAME_SECTION,:p_PARENT_SECTION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Section.InsertCommand.BindByName = true;
            odaStaff_Section.InsertCommand.Parameters.Add("p_STAFF_SECTION_ID", OracleDbType.Decimal, 0, "STAFF_SECTION_ID").Direction = ParameterDirection.InputOutput;
            odaStaff_Section.InsertCommand.Parameters["p_STAFF_SECTION_ID"].DbType = DbType.Decimal;
            odaStaff_Section.InsertCommand.Parameters.Add("p_NAME_SECTION", OracleDbType.Varchar2, 0, "NAME_SECTION").Direction = ParameterDirection.Input;
            odaStaff_Section.InsertCommand.Parameters.Add("p_PARENT_SECTION_ID", OracleDbType.Decimal, 0, "PARENT_SECTION_ID").Direction = ParameterDirection.Input;

            odaStaff_Section.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_SECTION_UPDATE(:p_STAFF_SECTION_ID,:p_NAME_SECTION,:p_PARENT_SECTION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Section.UpdateCommand.BindByName = true;
            odaStaff_Section.UpdateCommand.Parameters.Add("p_STAFF_SECTION_ID", OracleDbType.Decimal, 0, "STAFF_SECTION_ID").Direction = ParameterDirection.InputOutput;
            odaStaff_Section.UpdateCommand.Parameters["p_STAFF_SECTION_ID"].DbType = DbType.Decimal;
            odaStaff_Section.UpdateCommand.Parameters.Add("p_NAME_SECTION", OracleDbType.Varchar2, 0, "NAME_SECTION").Direction = ParameterDirection.Input;
            odaStaff_Section.UpdateCommand.Parameters.Add("p_PARENT_SECTION_ID", OracleDbType.Decimal, 0, "PARENT_SECTION_ID").Direction = ParameterDirection.Input;

            odaStaff_Section.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_SECTION_DELETE(:p_STAFF_SECTION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Section.DeleteCommand.BindByName = true;
            odaStaff_Section.DeleteCommand.Parameters.Add("p_STAFF_SECTION_ID", OracleDbType.Decimal, 0, "STAFF_SECTION_ID").Direction = ParameterDirection.InputOutput;
            try
            {
                odaStaff_Section.Fill(ds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetFormattedException(), "Ошибка получения данных");
            }
            if (StaffSection.Rows.Count == 0)
            {
                DataRow r = StaffSection.NewRow();
                if (parentSectionID != null)
                    r["PARENT_SECTION_ID"] = parentSectionID;
                StaffSection.Rows.Add(r);
            }
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public DataTable StaffSection
        {
            get
            {
                return ds.Tables["STAFF_SECTION"];
            }
        }

        private void Save_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command);
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                odaStaff_Section.Update(StaffSection);
                tr.Commit();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                MessageBox.Show(ex.GetFormattedException(), "Ошибка сохранения");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbStaffParent.ItemsSource = (this.Tag as StaffSectionList).RollSection;
            this.DataContext = StaffSection.DefaultView[0];
        }
    }
}
