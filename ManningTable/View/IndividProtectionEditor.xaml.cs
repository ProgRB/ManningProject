using EntityGenerator;
using Helpers;
using ManningTable.Helpers;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace ManningTable.View
{
    /// <summary>
    /// Interaction logic for IndividProtectionEditor.xaml
    /// </summary>
    public partial class IndividProtectionEditor : UserControl
    {
        private IndividProtectionViewModel _model;
        public IndividProtectionEditor()
        {
            InitializeComponent();
            _model = new IndividProtectionViewModel();
            DataContext = Model;
        }

        public IndividProtectionViewModel Model
        {
            get
            {
                return _model;
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && Model != null && Model.HasChanges;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Exception ex;
            if ((ex = Model.Save()) != null)
                MessageBox.Show(Window.GetWindow(this), ex.GetFormattedException(), "Ошибка сохранения данных");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!Model.HasChanges || MessageBox.Show("Имеются несохраненные изменения. Вы действительно хотите продолжить обновление? Несохраненные данные будут утеряны", "Возможная потеря данных",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                Model.RefreshIndividProtection();
            }
        }
    }

    public class IndividProtectionViewModel: NotificationObject
    {
        OracleDataAdapter odaIndivid_Protection;

        DataSet ds;

        public IndividProtectionViewModel()
        {
            ds = new DataSet();
            odaIndivid_Protection = new OracleDataAdapter(Queries.GetQuery(@"MT/SelectIndividProtectionData.sql"), Connect.CurConnect);
            odaIndivid_Protection.SelectCommand.BindByName = true;
            odaIndivid_Protection.SelectCommand.Parameters.Add("c1", OracleDbType.RefCursor, ParameterDirection.Output);
            odaIndivid_Protection.SelectCommand.Parameters.Add("c2", OracleDbType.RefCursor, ParameterDirection.Output);
            odaIndivid_Protection.TableMappings.Add("Table", "INDIVID_PROTECTION");
            odaIndivid_Protection.TableMappings.Add("Table1", "TYPE_INDIVID_PROTECTION");

            odaIndivid_Protection.Fill(ds);

            odaIndivid_Protection.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.INDIVID_PROTECTION_UPDATE(p_INDIVID_PROTECTION_ID=>:p_INDIVID_PROTECTION_ID,p_CODE_PROTECTION=>:p_CODE_PROTECTION,p_NAME_PROTECTION=>:p_NAME_PROTECTION,p_TYPE_INDIVID_PROTECTION_ID=>:p_TYPE_INDIVID_PROTECTION_ID);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaIndivid_Protection.InsertCommand.BindByName = true;
            odaIndivid_Protection.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaIndivid_Protection.InsertCommand.Parameters.Add("p_INDIVID_PROTECTION_ID", OracleDbType.Decimal, 0, "INDIVID_PROTECTION_ID").Direction = ParameterDirection.InputOutput;
            odaIndivid_Protection.InsertCommand.Parameters["p_INDIVID_PROTECTION_ID"].DbType = DbType.Decimal;
            odaIndivid_Protection.InsertCommand.Parameters.Add("p_CODE_PROTECTION", OracleDbType.Varchar2, 0, "CODE_PROTECTION").Direction = ParameterDirection.Input;
            odaIndivid_Protection.InsertCommand.Parameters.Add("p_NAME_PROTECTION", OracleDbType.Varchar2, 0, "NAME_PROTECTION").Direction = ParameterDirection.Input;
            odaIndivid_Protection.InsertCommand.Parameters.Add("p_TYPE_INDIVID_PROTECTION_ID", OracleDbType.Decimal, 0, "TYPE_INDIVID_PROTECTION_ID").Direction = ParameterDirection.Input; 
            
            odaIndivid_Protection.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.INDIVID_PROTECTION_UPDATE(p_INDIVID_PROTECTION_ID=>:p_INDIVID_PROTECTION_ID,p_CODE_PROTECTION=>:p_CODE_PROTECTION,p_NAME_PROTECTION=>:p_NAME_PROTECTION,p_TYPE_INDIVID_PROTECTION_ID=>:p_TYPE_INDIVID_PROTECTION_ID);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaIndivid_Protection.UpdateCommand.BindByName = true;
            odaIndivid_Protection.UpdateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaIndivid_Protection.UpdateCommand.Parameters.Add("p_INDIVID_PROTECTION_ID", OracleDbType.Decimal, 0, "INDIVID_PROTECTION_ID").Direction = ParameterDirection.InputOutput;
            odaIndivid_Protection.UpdateCommand.Parameters["p_INDIVID_PROTECTION_ID"].DbType = DbType.Decimal;
            odaIndivid_Protection.UpdateCommand.Parameters.Add("p_CODE_PROTECTION", OracleDbType.Varchar2, 0, "CODE_PROTECTION").Direction = ParameterDirection.Input;
            odaIndivid_Protection.UpdateCommand.Parameters.Add("p_NAME_PROTECTION", OracleDbType.Varchar2, 0, "NAME_PROTECTION").Direction = ParameterDirection.Input;
            odaIndivid_Protection.UpdateCommand.Parameters.Add("p_TYPE_INDIVID_PROTECTION_ID", OracleDbType.Decimal, 0, "TYPE_INDIVID_PROTECTION_ID").Direction = ParameterDirection.Input; 
            
            odaIndivid_Protection.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.INDIVID_PROTECTION_DELETE(:p_INDIVID_PROTECTION_ID);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaIndivid_Protection.DeleteCommand.BindByName = true;
            odaIndivid_Protection.DeleteCommand.Parameters.Add("p_INDIVID_PROTECTION_ID", OracleDbType.Decimal, 0, "INDIVID_PROTECTION_ID").Direction = ParameterDirection.InputOutput;

        }

        private DataView _individProtectionSource;
        private List<TypeIndividProtection> _typeProtectionSource;
        /// <summary>
        /// Источник данных для типов индивидуальной защиты
        /// </summary>
        public DataView IndividProtectionSource
        {
            get
            {
                return _individProtectionSource;
            }
        }

        /// <summary>
        /// Источник данных - Типы СИЗ
        /// </summary>
        public List<TypeIndividProtection> TypeProtectionSource
        {
            get
            {
                return _typeProtectionSource;
            }
        }

        /// <summary>
        /// Обновление списка 
        /// </summary>
        public void RefreshIndividProtection()
        {
            Exception ex = odaIndivid_Protection.TryFillWithClear(ds, this);
            if (ex != null)
            {
                MessageBox.Show(ex.GetFormattedException(), "Ошибка обновления данных");
            }
            else
            {
                _individProtectionSource = new DataView(ds.Tables["INDIVID_PROTECTION"], "", "CODE_PROTECTION", DataViewRowState.CurrentRows);
                _typeProtectionSource = ds.Tables["TYPE_INDIVID_PROTECTION"].ConvertToEntityList<TypeIndividProtection>();
                RaisePropertyChanged(() => IndividProtectionSource);
                RaisePropertyChanged(() => TypeProtectionSource);
            }
        }

        /// <summary>
        /// Есть ли изменения в наборе данных
        /// </summary>
        public bool HasChanges 
        {
            get
            {
                return ds != null && ds.HasChanges();
            }
        }

        /// <summary>
        /// Сохраняем данные
        /// </summary>
        /// <returns></returns>
        public Exception Save()
        {
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                odaIndivid_Protection.Update(ds.Tables["INDIVID_PROTECTION"]);
                tr.Commit();
                return null;
            }
            catch (Exception ex)
            {
                tr.Rollback();
                return ex;
            }
        }
    }
}
