using EntityGenerator;
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
using Helpers;

namespace ManningTable.View
{
    /// <summary>
    /// Interaction logic for SubdivPartitionView.xaml
    /// </summary>
    public partial class SubdivPartitionView : UserControl
    {
        private SubdivPartViewModel _model;
        public SubdivPartitionView()
        {
            _model = new SubdivPartViewModel();
            InitializeComponent();
            DataContext = Model;
        }
        public SubdivPartViewModel Model
        {
            get
            {
                return _model;
            }
        }

        private void Add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command);
        }

        private void Add_executed(object sender, ExecutedRoutedEventArgs e)
        {
            decimal? parentPartition=null, subdivid= null;
            if (e.Parameter != null && e.Parameter is SubdivPartition)
            {
                parentPartition = (e.Parameter as SubdivPartition).SubdivPartitionID;
                subdivid = (e.Parameter as SubdivPartition).SubdivID;
            }
            SubdivPartEditor f = new SubdivPartEditor(null, subdivid, parentPartition);
            f.Owner = Window.GetWindow(this);
            if (f.ShowDialog() == true)
            {
                var returnedValue = f.Model.SubdivPartitionID;
                Model.UpdateSubdivSource();
                Model.SetSelectedID(returnedValue);
            }
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && Model.SelectedNode!=null && Model.SelectedNode is SubdivPartition;
        }

        private void Edit_executed(object sender, ExecutedRoutedEventArgs e)
        {
            SubdivPartEditor f = new SubdivPartEditor((Model.SelectedNode as SubdivPartition).SubdivPartitionID, null, null);
            f.Owner = Window.GetWindow(this);
            if (f.ShowDialog() == true)
            {
                var returnedValue = f.Model.SubdivPartitionID;
                Model.UpdateSubdivSource();
                Model.SetSelectedID(returnedValue);
            }
        }

        private void Delete_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(Window.GetWindow(this), "Вы действительно хотите удалить выбранное структурное подразделение?", "Штатное расписание", MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {
                Exception ex = Model.DeleteSelectedNode();
                if (ex!=null)
                    MessageBox.Show(Window.GetWindow(this), ex.GetFormattedException(), "Ошибка удаления данных");
            }
        }
    }

    public partial class SubdivPartViewModel : NotificationObject
    {
        DataSet ds;
        private OracleDataAdapter odaSubdiv;
        private List<SubdivWithChild> _listSubdivWithChild;

        public SubdivPartViewModel()
        {
            Init();
        }

        public void Init()
        {
            try
            {
                ds = new DataSet();
                odaSubdiv = new OracleDataAdapter(Queries.GetQueryWithSchema(@"MT/SelectSubdivPartitionView.sql"), Connect.CurConnect);
                odaSubdiv.DeleteCommand = new OracleCommand("begin APSTAFF.SUBDIV_PARTITION_DELETE(:p_subdiv_partition_id);end;", Connect.CurConnect);
                odaSubdiv.DeleteCommand.BindByName = true;
                odaSubdiv.DeleteCommand.Parameters.Add("p_subdiv_partition_id", OracleDbType.Decimal, ParameterDirection.Input);

                odaSubdiv.SelectCommand.Parameters.Add("c1", OracleDbType.RefCursor, ParameterDirection.Output);
                odaSubdiv.SelectCommand.Parameters.Add("c2", OracleDbType.RefCursor, ParameterDirection.Output);
                odaSubdiv.TableMappings.Add("Table", "Subdiv");
                odaSubdiv.TableMappings.Add("Table1", "SUBDIV_PARTITION");
                UpdateSubdivSource();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.GetFormattedException(), "Ошибка получения значений");
            }
        }

        /// <summary>
        /// Процедура обновления списков подразделений и подструктур
        /// </summary>
        public void UpdateSubdivSource()
        {
            Exception ex = odaSubdiv.TryFillWithClear(ds, this);
            if (ex != null)
                MessageBox.Show(ex.GetFormattedException(), "Ошибка получения данных");
            else
            {
                _listSubdivWithChild = ds.Tables["SUBDIV"].ConvertToEntityList<SubdivWithChild>();
                RaisePropertyChanged(() => SubdivSource);
            }
        }

        /// <summary>
        /// Источник данных список подразделений
        /// </summary>
        public List<object> SubdivSource
        {
            get
            {
                List<SubdivWithChild> temp = _listSubdivWithChild.Where(r => r.CodeSubdiv.ToUpper().Contains(_filterSubdiv.Trim().ToUpper())).ToList();
                if (temp.Count > 0)
                    return temp.Select(r => (object)r).ToList();
                else
                    return null;
            }
        }

        /// <summary>
        /// Фильтр для установки подразделения
        /// </summary>
        public string FilterSubdiv
        {
            get
            {
                return _filterSubdiv;
            }
            set
            {
                _filterSubdiv = value;
                RaisePropertyChanged(() => FilterSubdiv);
                RaisePropertyChanged(() => SubdivSource);
            }
        }


        private string _filterSubdiv = string.Empty;
        private object _selectedNode;

        /// <summary>
        /// Выбранный узел в дереве
        /// </summary>
        public object SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                _selectedNode = value;
                RaisePropertyChanged(() => SelectedNode);
            }
        }

        /// <summary>
        /// Процедура для установки выбранного элемента по айдишнику
        /// </summary>
        /// <param name="subdivPartID"></param>
        public void SetSelectedID(decimal? subdivPartID)
        {
            try
            {
                foreach (var obj in SubdivSource)
                {
                    if (obj is SubdivPartition)
                    {
                        var find_obj = (obj as SubdivPartition).FindNode(subdivPartID);
                        if (find_obj != null)
                        {
                            SelectedNode = find_obj;
                            break;
                        }
                    }
                    else
                        if (obj is SubdivWithChild)
                        {
                            foreach (var inner_obj in (obj as SubdivWithChild).SubdivPartitionChildren)
                            {
                                var find2 = inner_obj.FindNode(subdivPartID);
                                if (find2 != null)
                                {
                                    SelectedNode = find2;
                                    return;
                                }
                            }
                        }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Удаление выбранного узла из дерева
        /// </summary>
        /// <returns></returns>
        public Exception DeleteSelectedNode()
        {
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                odaSubdiv.DeleteCommand.Parameters["p_subdiv_partition_id"].Value = (SelectedNode as SubdivPartition).SubdivPartitionID;
                odaSubdiv.DeleteCommand.ExecuteNonQuery();
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

    public enum SubdivPartViewType
    {
        PartTreeType =1,
        SubdivTreeType =2,
        TableType = 3
    }

    public partial class SubdivWithChild : Subdiv
    {
        private List<SubdivPartition> _subdivChildren;
        /// <summary>
        /// Дочерние структуры для подразделения.
        /// </summary>
        public List<SubdivPartition> SubdivPartitionChildren
        {
            get
            {
                if (_subdivChildren==null)
                 _subdivChildren= DataSet.Tables["SUBDIV_PARTITION"].Rows.OfType<DataRow>().Where(r => r.RowState != DataRowState.Deleted
                    && r.Field2<decimal?>("SUBDIV_ID") ==this.SubdivID && r.Field2<decimal?>("PARENT_SUBDIV_ID") == null)
                    .Select(r => new SubdivPartition() { DataRow = r }).OrderBy(r => r.SubdivNumber).ToList();
                return _subdivChildren;
            }
        }

        /// <summary>
        /// ОБновляем список дочерних элементов подразделения
        /// </summary>
        public void UpdateChildrenList()
        {
            _subdivChildren = null;
            RaisePropertyChanged(()=>SubdivPartitionChildren);
        }
    }
}
