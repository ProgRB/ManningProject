using EntityGenerator;
using Helpers;
using ManningTable.Helpers;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
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
    /// Interaction logic for SubdivPartEditor.xaml
    /// </summary>
    public partial class SubdivPartEditor : Window
    {
        private SubdivPartitionModel _model;
        public SubdivPartEditor(decimal? subdivPartitionID, decimal? subdivID = null, decimal? parentID = null)
        {
            _model = new SubdivPartitionModel(subdivPartitionID);
            if (subdivID!=null) _model.SubdivID = subdivID;
            if (parentID!=null) _model.ParentSubdivID = parentID;
            InitializeComponent();
            DataContext = Model;
        }

        /// <summary>
        /// Модель для формы
        /// </summary>
        public SubdivPartitionModel Model
        {
            get
            {
                return _model;
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && string.IsNullOrEmpty(Model.Error);
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Exception ex = Model.Save();
            if (ex != null)
                MessageBox.Show(ex.GetFormattedException(), "Ошибка сохранения данных");
            else
            {
                DialogResult = true;
                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public partial class SubdivPartitionModel:SubdivPartition, IDataErrorInfo
    {
        DataSet ds = new DataSet();
        OracleDataAdapter odaSubdiv_Partition;
        public SubdivPartitionModel(decimal? subdivPartitionID)
        {
            ds.Tables.Add(AppDataSet.Tables["SUBDIV"].Copy());

            odaSubdiv_Partition = new OracleDataAdapter(Queries.GetQueryWithSchema(@"MT\SelectSubdivPartitionData.sql"), Connect.CurConnect);
            odaSubdiv_Partition.SelectCommand.BindByName = true;
            odaSubdiv_Partition.SelectCommand.Parameters.Add("c1", OracleDbType.RefCursor, ParameterDirection.Output);
            odaSubdiv_Partition.SelectCommand.Parameters.Add("c2", OracleDbType.RefCursor, ParameterDirection.Output);
            odaSubdiv_Partition.TableMappings.Add("Table", "SUBDIV_PARTITION");
            odaSubdiv_Partition.TableMappings.Add("Table1", "SUBDIV_PART_TYPE");



            odaSubdiv_Partition.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PARTITION_UPDATE(p_SUBDIV_PARTITION_ID=>:p_SUBDIV_PARTITION_ID,p_SUBDIV_PART_TYPE_ID=>:p_SUBDIV_PART_TYPE_ID,p_SUBDIV_NUMBER=>:p_SUBDIV_NUMBER,p_PARENT_SUBDIV_ID=>:p_PARENT_SUBDIV_ID,p_SUBDIV_ID=>:p_SUBDIV_ID,p_DATE_START_SUBDIV_PART=>:p_DATE_START_SUBDIV_PART,p_DATE_END_SUBDIV_PART=>:p_DATE_END_SUBDIV_PART,p_SUBDIV_PART_NAME=>:p_SUBDIV_PART_NAME);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Partition.InsertCommand.BindByName = true;
            odaSubdiv_Partition.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_SUBDIV_PARTITION_ID", OracleDbType.Decimal, 0, "SUBDIV_PARTITION_ID").Direction = ParameterDirection.InputOutput;
            odaSubdiv_Partition.InsertCommand.Parameters["p_SUBDIV_PARTITION_ID"].DbType = DbType.Decimal;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_SUBDIV_PART_TYPE_ID", OracleDbType.Decimal, 0, "SUBDIV_PART_TYPE_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_SUBDIV_NUMBER", OracleDbType.Varchar2, 0, "SUBDIV_NUMBER").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_PARENT_SUBDIV_ID", OracleDbType.Decimal, 0, "PARENT_SUBDIV_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_SUBDIV_ID", OracleDbType.Decimal, 0, "SUBDIV_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_DATE_START_SUBDIV_PART", OracleDbType.Date, 0, "DATE_START_SUBDIV_PART").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_DATE_END_SUBDIV_PART", OracleDbType.Date, 0, "DATE_END_SUBDIV_PART").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.InsertCommand.Parameters.Add("p_SUBDIV_PART_NAME", OracleDbType.Varchar2, 0, "SUBDIV_PART_NAME").Direction = ParameterDirection.Input; 
            
            odaSubdiv_Partition.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PARTITION_UPDATE(p_SUBDIV_PARTITION_ID=>:p_SUBDIV_PARTITION_ID,p_SUBDIV_PART_TYPE_ID=>:p_SUBDIV_PART_TYPE_ID,p_SUBDIV_NUMBER=>:p_SUBDIV_NUMBER,p_PARENT_SUBDIV_ID=>:p_PARENT_SUBDIV_ID,p_SUBDIV_ID=>:p_SUBDIV_ID,p_DATE_START_SUBDIV_PART=>:p_DATE_START_SUBDIV_PART,p_DATE_END_SUBDIV_PART=>:p_DATE_END_SUBDIV_PART,p_SUBDIV_PART_NAME=>:p_SUBDIV_PART_NAME);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Partition.UpdateCommand.BindByName = true;
            odaSubdiv_Partition.UpdateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_SUBDIV_PARTITION_ID", OracleDbType.Decimal, 0, "SUBDIV_PARTITION_ID").Direction = ParameterDirection.InputOutput;
            odaSubdiv_Partition.UpdateCommand.Parameters["p_SUBDIV_PARTITION_ID"].DbType = DbType.Decimal;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_SUBDIV_PART_TYPE_ID", OracleDbType.Decimal, 0, "SUBDIV_PART_TYPE_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_SUBDIV_NUMBER", OracleDbType.Varchar2, 0, "SUBDIV_NUMBER").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_PARENT_SUBDIV_ID", OracleDbType.Decimal, 0, "PARENT_SUBDIV_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_SUBDIV_ID", OracleDbType.Decimal, 0, "SUBDIV_ID").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_DATE_START_SUBDIV_PART", OracleDbType.Date, 0, "DATE_START_SUBDIV_PART").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_DATE_END_SUBDIV_PART", OracleDbType.Date, 0, "DATE_END_SUBDIV_PART").Direction = ParameterDirection.Input;
            odaSubdiv_Partition.UpdateCommand.Parameters.Add("p_SUBDIV_PART_NAME", OracleDbType.Varchar2, 0, "SUBDIV_PART_NAME").Direction = ParameterDirection.Input; 
            
            odaSubdiv_Partition.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.SUBDIV_PARTITION_DELETE(:p_SUBDIV_PARTITION_ID);end;", Connect.SchemaApstaff, Connect.SchemaSalary), Connect.CurConnect);
            odaSubdiv_Partition.DeleteCommand.BindByName = true;
            odaSubdiv_Partition.DeleteCommand.Parameters.Add("p_SUBDIV_PARTITION_ID", OracleDbType.Decimal, 0, "SUBDIV_PARTITION_ID").Direction = ParameterDirection.InputOutput;

            this.DataAdapter = odaSubdiv_Partition;
            this.AdapterConnection = Connect.CurConnect;

            this.DataAdapter.Fill(ds);

            DataRow current =  ds.Tables["SUBDIV_PARTITION"].Select("SUBDIV_PARTITION_ID = "+ (subdivPartitionID??-1).ToString()).FirstOrDefault();
            if (current != null)
            {
                this.DataRow = current;
            }
            else
            {
                DataRow r = ds.Tables["SUBDIV_PARTITION"].NewRow();
                ds.Tables["SUBDIV_PARTITION"].Rows.Add(r);
                this.DataRow = r;
                this.SubdivPartitionID = -1m; // сделаем отрицательный айдшинк, чтобы не пусто было (как будто корень всех пустых деревьев)
                this.DateStartSubdivPart = new DateTime(2000, 1, 1);
                try
                {
                    this.SubdivNumber = string.Format("{0:00}", SubdivPartitionSource.Max(p => int.Parse(p.SubdivNumber))+1);
                }
                catch
                { }
            }

        }

        /// <summary>
        /// Источник данных для родительских структур
        /// </summary>
        public List<SubdivPartition> SubdivPartitionSource
        {
            get
            {
                if (dictSubPartition == null)
                {
                    dictSubPartition = ds.Tables["SUBDIV_PARTITION"].ConvertToEntityList<SubdivPartition>();
                }
                return dictSubPartition.Where(r=>r.SubdivPartitionID!=this.SubdivPartitionID).ToList();
            }
        }

        public List<SubdivPartition> dictSubPartition;

        List<SubdivPartitionChildDisabled> _subdivTreeList;
        /// <summary>
        /// Список с древовидным порядком подразделений
        /// </summary>
        public List<SubdivPartitionChildDisabled> SubdivPartitionTreeList
        {
            get
            {
                if (_subdivTreeList == null && SubdivPartitionSource!=null)
                {
                    _subdivTreeList = new List<SubdivPartitionChildDisabled>();
                    FillSubdivTreeList(dictSubPartition.Where(r=>r.ParentSubdivID == null), true);
                }
                return _subdivTreeList;
            }
        }

        private void FillSubdivTreeList(IEnumerable<SubdivPartition> subdivPartitionID, bool isEnabled)
        {
            foreach (var t in subdivPartitionID)
            {
                if (t.SubdivPartitionID == this.SubdivPartitionID)
                {
                    _subdivTreeList.Add(new SubdivPartitionChildDisabled() { DataRow = t.DataRow, IsSubEnabled = false });
                    FillSubdivTreeList(t.PartitionChildren, false);
                }
                else
                {
                    _subdivTreeList.Add(new SubdivPartitionChildDisabled() { DataRow = t.DataRow, IsSubEnabled = isEnabled });
                    FillSubdivTreeList(t.PartitionChildren, isEnabled);
                }
            }
        }

        /// <summary>
        /// Источник данных - список подразделений
        /// </summary>
        public List<Subdiv> SubdivSource
        {
            get
            {
                return ds.Tables["SUBDIV"].ConvertToEntityList<Subdiv>().Where(r=>r.TypeSubdivID!=6  && r.ParentID==0)
                    .OrderBy(r=>new Tuple<string, int?>(r.CodeSubdiv, (r.SubActualSign+1)%2)).ToList();
            }
        }

        /// <summary>
        /// Типы подструктур
        /// </summary>
        public List<SubdivPartType> SubdivPartTypeSource
        {
            get
            {
                return ds.Tables["SUBDIV_PART_TYPE"].ConvertToEntityList<SubdivPartType>();
            }
        }

        /// <summary>
        /// Ошибка для всего структурного подразделения
        /// </summary>
        public new string Error
        {
            get
            {
                string s = base.Error;
                if (!string.IsNullOrEmpty(s))
                    return s;
                return string.Empty;
            }
        }

        /// <summary>
        /// Переопределеим подразделение для обновления кода
        /// </summary>
        [Column(CanBeNull = false)]
        public new decimal? SubdivID
        {
            get
            {
                return base.SubdivID;
            }
            set
            {
                base.SubdivID = value;
                RaisePropertyChanged(() => CodeSubdivPartition);
            }
        }

        /// <summary>
        /// Переопределеим тип структуры для обновления кода
        /// </summary>
        [Column(CanBeNull = false)]
        public new decimal? SubdivPartTypeID
        {
            get
            {
                return base.SubdivPartTypeID;
            }
            set
            {
                base.SubdivPartTypeID = value;
                RaisePropertyChanged(() => CodeSubdivPartition);
            }
        }

        /// <summary>
        /// Номер подразделения переопределенный чтобы менять код подразделения
        /// </summary>
        [Column(CanBeNull = false)]
        public new string SubdivNumber
        {
            get
            {
                return base.SubdivNumber;
            }
            set
            {
                base.SubdivNumber = value;
                RaisePropertyChanged(() => CodeSubdivPartition);
            }
        }

        /// <summary>
        ///Айдишник родительского узла
        /// </summary>
        public new decimal? ParentSubdivID
        {
            get
            {
                return base.ParentSubdivID;
            }
            set
            {
                base.ParentSubdivID = value;
                RaisePropertyChanged(() => SubLevel);
            }
        }


    }
    public class SubdivPartitionChildDisabled : SubdivPartition
    {
        bool _isEnabled = true;
        /// <summary>
        /// Используется для того чтобы построить дерево и не допустить циклические зависимости в интерфейсе
        /// </summary>
        public bool IsSubEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => IsSubEnabled);
            }
        }
    }
}
