using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Helpers;
using System.Collections.ObjectModel;
using ManningTable.Model;
using System.Data;
using Oracle.DataAccess.Client;
using LibraryKadr;
using System.Windows;
using System.ComponentModel;

namespace ManningTable.ViewModel
{
    public class ManningEditorViewModel: NotificationObject
    {
        OracleDataAdapter odaStaff, odaStaff_Addition;
        DataSet ds;
        BackgroundWorker asyncLoader;
        public ManningEditorViewModel(StaffFilterProvider _staffFilter)
        {
            asyncLoader = new BackgroundWorker();
            asyncLoader.WorkerSupportsCancellation = true;
            asyncLoader.DoWork += new DoWorkEventHandler(asyncLoader_DoWork);
            asyncLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(asyncLoader_RunWorkerCompleted);
            ds = new DataSet();
            odaStaff = new OracleDataAdapter(string.Format(Queries.GetQuery("staff/SelectStaffView.sql"), Connect.Schema), Connect.CurConnect);
            odaStaff.SelectCommand.BindByName = true;
            odaStaff.SelectCommand.Parameters.Add("p_subdiv_id", OracleDbType.Decimal, null, ParameterDirection.Input);
            odaStaff.SelectCommand.Parameters.Add("p_staff_section_id", OracleDbType.Decimal, null, ParameterDirection.Input);
            odaStaff.SelectCommand.Parameters.Add("p_date", OracleDbType.Date, null, ParameterDirection.Input);
            odaStaff.SelectCommand.Parameters.Add("c1", OracleDbType.RefCursor, ParameterDirection.Output);
            odaStaff.SelectCommand.Parameters.Add("c2", OracleDbType.RefCursor, ParameterDirection.Output);
            odaStaff.TableMappings.Add("Table", "Staff");
            odaStaff.TableMappings.Add("Table1", "STAFF_ADDITION");

#region Адаптер сохранение штатных единиц

            odaStaff.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_INSERT(:p_STAFF_ID,:p_POS_ID,:p_SUBDIV_ID,:p_DEGREE_ID,:p_MAX_TARIF,:p_TYPE_PERSON_ID,:p_COMMENTS,:p_ORDER_ID,:p_TYPE_STAFF_ID,:p_TARIFF_GRID_ID,:p_TAR_BY_SCHEMA,:p_CLASSIFIC,:p_PERSONAL_TAR,:p_STAFF_SECTION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff.InsertCommand.BindByName = true;
            odaStaff.InsertCommand.Parameters.Add("p_STAFF_ID", OracleDbType.Decimal, 0, "STAFF_ID").Direction = ParameterDirection.InputOutput;
            odaStaff.InsertCommand.Parameters["p_STAFF_ID"].DbType = DbType.Decimal;
            odaStaff.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaStaff.InsertCommand.Parameters.Add("p_POS_ID", OracleDbType.Decimal, 0, "POS_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_SUBDIV_ID", OracleDbType.Decimal, 0, "SUBDIV_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_DEGREE_ID", OracleDbType.Decimal, 0, "DEGREE_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_MAX_TARIF", OracleDbType.Decimal, 0, "MAX_TARIF").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_TYPE_PERSON_ID", OracleDbType.Decimal, 0, "TYPE_PERSON_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_COMMENTS", OracleDbType.Varchar2, 0, "COMMENTS").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_ORDER_ID", OracleDbType.Decimal, 0, "ORDER_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_TYPE_STAFF_ID", OracleDbType.Decimal, 0, "TYPE_STAFF_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_TARIFF_GRID_ID", OracleDbType.Decimal, 0, "TARIFF_GRID_ID").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_TAR_BY_SCHEMA", OracleDbType.Decimal, 0, "TAR_BY_SCHEMA").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_CLASSIFIC", OracleDbType.Decimal, 0, "CLASSIFIC").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_PERSONAL_TAR", OracleDbType.Decimal, 0, "PERSONAL_TAR").Direction = ParameterDirection.Input;
            odaStaff.InsertCommand.Parameters.Add("p_STAFF_SECTION_ID", OracleDbType.Decimal, 0, "STAFF_SECTION_ID").Direction = ParameterDirection.Input;

            odaStaff.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_UPDATE(:p_STAFF_ID,:p_POS_ID,:p_SUBDIV_ID,:p_DEGREE_ID,:p_MAX_TARIF,:p_TYPE_PERSON_ID,:p_COMMENTS,:p_ORDER_ID,:p_TYPE_STAFF_ID,:p_TARIFF_GRID_ID,:p_TAR_BY_SCHEMA,:p_CLASSIFIC,:p_PERSONAL_TAR,:p_STAFF_SECTION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff.UpdateCommand.BindByName = true;
            odaStaff.UpdateCommand.Parameters.Add("p_STAFF_ID", OracleDbType.Decimal, 0, "STAFF_ID").Direction = ParameterDirection.InputOutput;
            odaStaff.UpdateCommand.Parameters["p_STAFF_ID"].DbType = DbType.Decimal;
            odaStaff.UpdateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            odaStaff.UpdateCommand.Parameters.Add("p_POS_ID", OracleDbType.Decimal, 0, "POS_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_SUBDIV_ID", OracleDbType.Decimal, 0, "SUBDIV_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_DEGREE_ID", OracleDbType.Decimal, 0, "DEGREE_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_MAX_TARIF", OracleDbType.Decimal, 0, "MAX_TARIF").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_TYPE_PERSON_ID", OracleDbType.Decimal, 0, "TYPE_PERSON_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_COMMENTS", OracleDbType.Varchar2, 0, "COMMENTS").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_ORDER_ID", OracleDbType.Decimal, 0, "ORDER_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_TYPE_STAFF_ID", OracleDbType.Decimal, 0, "TYPE_STAFF_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_TARIFF_GRID_ID", OracleDbType.Decimal, 0, "TARIFF_GRID_ID").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_TAR_BY_SCHEMA", OracleDbType.Decimal, 0, "TAR_BY_SCHEMA").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_CLASSIFIC", OracleDbType.Decimal, 0, "CLASSIFIC").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_PERSONAL_TAR", OracleDbType.Decimal, 0, "PERSONAL_TAR").Direction = ParameterDirection.Input;
            odaStaff.UpdateCommand.Parameters.Add("p_STAFF_SECTION_ID", OracleDbType.Decimal, 0, "STAFF_SECTION_ID").Direction = ParameterDirection.Input;

            odaStaff.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_DELETE(:p_STAFF_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff.DeleteCommand.BindByName = true;
            odaStaff.DeleteCommand.Parameters.Add("p_STAFF_ID", OracleDbType.Decimal, 0, "STAFF_ID").Direction = ParameterDirection.InputOutput;

#endregion

#region Адаптер обновления надбавок

            odaStaff_Addition = new OracleDataAdapter();
            odaStaff_Addition.InsertCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_ADDITION_UPDATE(:p_STAFF_ADDITION_ID,:p_STAFF_ID,:p_TYPE_STAFF_ADDITION_ID,:p_ADDITION_VALUE);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Addition.InsertCommand.BindByName = true;
            odaStaff_Addition.InsertCommand.Parameters.Add("p_STAFF_ADDITION_ID", OracleDbType.Decimal, 0, "STAFF_ADDITION_ID").Direction = ParameterDirection.InputOutput;
            odaStaff_Addition.InsertCommand.Parameters["p_STAFF_ADDITION_ID"].DbType = DbType.Decimal;
            odaStaff_Addition.InsertCommand.Parameters.Add("p_STAFF_ID", OracleDbType.Decimal, 0, "STAFF_ID").Direction = ParameterDirection.Input;
            odaStaff_Addition.InsertCommand.Parameters.Add("p_TYPE_STAFF_ADDITION_ID", OracleDbType.Decimal, 0, "TYPE_STAFF_ADDITION_ID").Direction = ParameterDirection.Input;
            odaStaff_Addition.InsertCommand.Parameters.Add("p_ADDITION_VALUE", OracleDbType.Decimal, 0, "ADDITION_VALUE").Direction = ParameterDirection.Input;

            odaStaff_Addition.UpdateCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_ADDITION_UPDATE(:p_STAFF_ADDITION_ID,:p_STAFF_ID,:p_TYPE_STAFF_ADDITION_ID,:p_ADDITION_VALUE);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Addition.UpdateCommand.BindByName = true;
            odaStaff_Addition.UpdateCommand.Parameters.Add("p_STAFF_ADDITION_ID", OracleDbType.Decimal, 0, "STAFF_ADDITION_ID").Direction = ParameterDirection.InputOutput;
            odaStaff_Addition.UpdateCommand.Parameters["p_STAFF_ADDITION_ID"].DbType = DbType.Decimal;
            odaStaff_Addition.UpdateCommand.Parameters.Add("p_STAFF_ID", OracleDbType.Decimal, 0, "STAFF_ID").Direction = ParameterDirection.Input;
            odaStaff_Addition.UpdateCommand.Parameters.Add("p_TYPE_STAFF_ADDITION_ID", OracleDbType.Decimal, 0, "TYPE_STAFF_ADDITION_ID").Direction = ParameterDirection.Input;
            odaStaff_Addition.UpdateCommand.Parameters.Add("p_ADDITION_VALUE", OracleDbType.Decimal, 0, "ADDITION_VALUE").Direction = ParameterDirection.Input;

            odaStaff_Addition.DeleteCommand = new OracleCommand(string.Format(@"BEGIN {0}.STAFF_ADDITION_DELETE(:p_STAFF_ADDITION_ID);end;", Connect.Schema), Connect.CurConnect);
            odaStaff_Addition.DeleteCommand.BindByName = true;
            odaStaff_Addition.DeleteCommand.Parameters.Add("p_STAFF_ADDITION_ID", OracleDbType.Decimal, 0, "STAFF_ADDITION_ID").Direction = ParameterDirection.InputOutput;

#endregion
            StaffFilter = _staffFilter;
            StaffFilter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(StaffFilter_PropertyChanged);
        }

        

        void StaffFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubdivID" || e.PropertyName == "StaffSections" || e.PropertyName=="CurrentStaffSection")
                RaisePropertyChanged(() => StaffSource);
        }

        public void Save()
        {
            OracleTransaction tr = Connect.CurConnect.BeginTransaction();
            try
            {
                Array.ForEach(Staff.Rows.OfType<DataRow>().ToArray(), (DataRow r) => { if (r.RowState == DataRowState.Added) r["STAFF_ID"] = DBNull.Value; });
                odaStaff.Update(Staff);
                odaStaff_Addition.Update(StaffAddition);
                tr.Commit();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                MessageBox.Show(Library.GetMessageException(ex), "Ошибка сохранения");
            }
        }
        
        
        ObservableCollection<StaffGroup> _staffGroups;
        public ObservableCollection<StaffGroup> StaffSource
        {
            get
            {
                return _staffGroups;
            }
        }

        void asyncLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            StaffFilterProvider temp_filter = e.Argument as StaffFilterProvider;
            if (e.Cancel) return;
            UpdateStaffView(temp_filter);
            if (e.Cancel) return;
            if (dvStaff == null)
                dvStaff = new DataView(ds.Tables["Staff"], "", "STAFF_GROUP_ID", DataViewRowState.CurrentRows);
            if (e.Cancel) return;
            if (dvAdditionStaff == null)
                dvAdditionStaff = new DataView(ds.Tables["STAFF_ADDITION"], "", "STAFF_ID, TYPE_STAFF_ADDITION_ID", DataViewRowState.CurrentRows);
            if (e.Cancel) return;
            e.Result = new ObservableCollection<StaffGroup>((from r in dvStaff.OfType<DataRowView>()
                                                                 where r.Row.RowState != DataRowState.Detached
                                                                 group r by r.Row.Field<Decimal>("STAFF_GROUP_ID") into gr
                                                                 select new StaffGroup()
                                                                 {
                                                                     GroupID = gr.Key,
                                                                     Classific = gr.First().Row.Field2<Decimal?>("CLASSIFIC"),
                                                                     OrderID = gr.First().Row.Field2<Decimal?>("ORDER_ID"),
                                                                     PersonalTar = gr.First().Row.Field2<Decimal?>("PERSONAL_TAR"),
                                                                     PosID = gr.First().Row.Field2<Decimal?>("POS_ID"),
                                                                     SubdivID = gr.First().Row.Field2<Decimal?>("SUBDIV_ID"),
                                                                     TariffGridID = gr.First().Row.Field2<Decimal?>("TARIFF_GRID_ID"),
                                                                     TarBySchema = gr.First().Row.Field2<Decimal?>("TAR_BY_SCHEMA"),
                                                                     //Comments = gr.First().Row.Field<string>("COMMENTS"),
                                                                     CombAddition = gr.First().Row.Field2<Decimal?>("COMB_ADDITION"),
                                                                     HarmAddition = gr.First().Row.Field2<Decimal?>("HARM_ADDITION"),
                                                                     StaffSectionID = gr.First().Row.Field2<Decimal?>("STAFF_SECTION_ID"),
                                                                     StaffFilter = temp_filter,
                                                                     TableView = dvStaff,
                                                                     StaffTable = Staff
                                                                 }));
        }

        void asyncLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsLoading = false;
            if (e.Error != null)
            {
                _staffGroups = null;
                MessageBox.Show(Library.GetMessageException(e.Error), "Ошибка получения данных");
            }
            else
                _staffGroups = (ObservableCollection<StaffGroup>)e.Result;
            RaisePropertyChanged(() => StaffSource);
        }        

        bool _isLoading=false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

        public StaffFilterProvider StaffFilter
        {
            get;set;
        }

        public void AddStaffGroup()
        {
            _staffGroups.Add(this.GetNewGroup());
        }

        public DataTable Staff
        {
            get
            {
                return ds.Tables["STAFF"];
            }
        }
        public DataTable StaffAddition
        {
            get
            {
                return ds.Tables["STAFF_ADDITION"];
            }
        }

        public StaffGroup GetNewGroup()
        {
            StaffGroup s = new StaffGroup();
            s.GroupID = _staffGroups.Max(t=>t.GroupID+1)??1;
            s.StaffTable = this.Staff;
            s.TableView = dvStaff;
            s.SubdivID = this.StaffFilter.SubdivID;
            s.StaffFilter = this.StaffFilter;
            s.GroupedStaffs.Add(s.NewStaff());
            return s;
        }

        public void UpdateStaffView(StaffFilterProvider filter)
        {
            odaStaff.SelectCommand.Parameters["p_subdiv_id"].Value = filter.SubdivID;
            odaStaff.SelectCommand.Parameters["p_date"].Value = filter.BaseTariff.CurrentDate;
            odaStaff.SelectCommand.Parameters["p_staff_section_id"].Value = filter.StaffSections.CurrentItem.StaffSectionID;
            if (ds.Tables.Contains(odaStaff.TableMappings[1].DataSetTable))
                ds.Tables[odaStaff.TableMappings[1].DataSetTable].Clear();
            if (ds.Tables.Contains(odaStaff.TableMappings[0].DataSetTable))
                ds.Tables[odaStaff.TableMappings[0].DataSetTable].Clear();
            odaStaff.Fill(ds);
            if (!ds.Relations.Contains("staff_addition_fk"))
            { 
                ds.Relations.Add("staff_addition_fk", Staff.Columns["STAFF_ID"], StaffAddition.Columns["STAFF_ID"], false);
            }
        }

        public void RaiseStaffSourceChanged()
        {
            if (IsLoading)
                return;
            IsLoading = true;
            if (!asyncLoader.IsBusy)
                asyncLoader.RunWorkerAsync(StaffFilter);
        }

        private DataView dvStaff, dvAdditionStaff;

        public bool HasChanges
        {
            get
            {
                return ds != null && ds.HasChanges();
            }
        }

        StaffGroup _currentGroup;
        public StaffGroup CurrentGroup
        {
            get
            {
                return _currentGroup;
            }
            set
            {
                _currentGroup = value;
                RaisePropertyChanged(() => CurrentGroup);
            }
        }
        public void DeleteCurrentGroup()
        {
            StaffGroup temp_Group = CurrentGroup;
            _staffGroups.Remove(CurrentGroup);
            temp_Group.Delete();
            CurrentGroup = null;
        }
    }
}
