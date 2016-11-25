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
using System.Windows.Shapes;
using Helpers;


namespace ManningTable.View
{
    /// <summary>
    /// Interaction logic for SubdivPartTypeEditor.xaml
    /// </summary>
    public partial class SubdivPartTypeEditor : UserControl
    {
        private SubdivPartTypeViewModel _model;
        public SubdivPartTypeEditor()
        {
            _model = new SubdivPartTypeViewModel();
            InitializeComponent();
        }

        public SubdivPartTypeViewModel Model
        {
            get
            {
                return _model;
            }
        }

        private void Save_canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && Model.HasChanges;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Exception ex = Model.Save();
            if (ex != null)
                MessageBox.Show(Window.GetWindow(this), ex.GetFormattedException(), "Ошибка сохранения");
        }
        
        
    }

    /// <summary>
    /// Модель представления для данных типов подструктур
    /// </summary>
    public class SubdivPartTypeViewModel : NotificationObject
    {
        DataSet ds;
        OracleDataAdapter odaSubdiv_Part_Type;
        DataView _subdivTypePartSource;
        private DataRowView _currentType;
        public SubdivPartTypeViewModel()
        {
            ds = new DataSet();

            odaSubdiv_Part_Type = new OracleDataAdapter(string.Format("select * from {0}.SUBDIV_PART_TYPE order by subdiv_part_type_code", Connect.Schema), Connect.CurConnect);
            odaSubdiv_Part_Type.TableMappings.Add("Table", "SUBDIV_PART_TYPE");

            odaSubdiv_Part_Type.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PART_TYPE_UPDATE(p_SUBDIV_PART_TYPE_ID=>:p_SUBDIV_PART_TYPE_ID,p_SUBDIV_PART_TYPE_CODE=>:p_SUBDIV_PART_TYPE_CODE,p_SUBDIV_PART_TYPE_NAME=>:p_SUBDIV_PART_TYPE_NAME);end;", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Part_Type.InsertCommand.BindByName = true;
            odaSubdiv_Part_Type.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaSubdiv_Part_Type.InsertCommand.Parameters.Add("p_SUBDIV_PART_TYPE_ID", OracleDbType.Decimal, 0, "SUBDIV_PART_TYPE_ID").Direction = ParameterDirection.InputOutput;
            odaSubdiv_Part_Type.InsertCommand.Parameters["p_SUBDIV_PART_TYPE_ID"].DbType = DbType.Decimal;
            odaSubdiv_Part_Type.InsertCommand.Parameters.Add("p_SUBDIV_PART_TYPE_CODE", OracleDbType.Varchar2, 0, "SUBDIV_PART_TYPE_CODE").Direction = ParameterDirection.Input;
            odaSubdiv_Part_Type.InsertCommand.Parameters.Add("p_SUBDIV_PART_TYPE_NAME", OracleDbType.Varchar2, 0, "SUBDIV_PART_TYPE_NAME").Direction = ParameterDirection.Input; 
            
            odaSubdiv_Part_Type.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PART_TYPE_UPDATE(p_SUBDIV_PART_TYPE_ID=>:p_SUBDIV_PART_TYPE_ID,p_SUBDIV_PART_TYPE_CODE=>:p_SUBDIV_PART_TYPE_CODE,p_SUBDIV_PART_TYPE_NAME=>:p_SUBDIV_PART_TYPE_NAME);end;", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Part_Type.UpdateCommand.BindByName = true;
            odaSubdiv_Part_Type.UpdateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaSubdiv_Part_Type.UpdateCommand.Parameters.Add("p_SUBDIV_PART_TYPE_ID", OracleDbType.Decimal, 0, "SUBDIV_PART_TYPE_ID").Direction = ParameterDirection.InputOutput;
            odaSubdiv_Part_Type.UpdateCommand.Parameters["p_SUBDIV_PART_TYPE_ID"].DbType = DbType.Decimal;
            odaSubdiv_Part_Type.UpdateCommand.Parameters.Add("p_SUBDIV_PART_TYPE_CODE", OracleDbType.Varchar2, 0, "SUBDIV_PART_TYPE_CODE").Direction = ParameterDirection.Input;
            odaSubdiv_Part_Type.UpdateCommand.Parameters.Add("p_SUBDIV_PART_TYPE_NAME", OracleDbType.Varchar2, 0, "SUBDIV_PART_TYPE_NAME").Direction = ParameterDirection.Input; 
            
            odaSubdiv_Part_Type.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PART_TYPE_DELETE(:p_SUBDIV_PART_TYPE_ID);end;", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Part_Type.DeleteCommand.BindByName = true;
            odaSubdiv_Part_Type.DeleteCommand.Parameters.Add("p_SUBDIV_PART_TYPE_ID", OracleDbType.Decimal, 0, "SUBDIV_PART_TYPE_ID").Direction = ParameterDirection.InputOutput;

            odaSubdiv_Part_Type.Fill(ds);
        }

        /// <summary>
        /// Источник данных для таблицы
        /// </summary>
        public DataView SubdivTypePartSource
        {
            get
            {
                if (_subdivTypePartSource == null && ds != null)
                {
                    _subdivTypePartSource = new DataView(ds.Tables["SUBDIV_PART_TYPE"], "", "", DataViewRowState.CurrentRows);
                }
                return _subdivTypePartSource;
            }
        }

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <returns></returns>
        public Exception Save()
        {
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                odaSubdiv_Part_Type.Update(SubdivTypePartSource.Table);
                tr.Commit();
                return null;
            }
            catch (Exception ex)
            {
                tr.Rollback();
                return ex;
            }
        }

        /// <summary>
        /// Имеются ли изменения в данных
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return ds.HasChanges();
            }
        }
    }

}
