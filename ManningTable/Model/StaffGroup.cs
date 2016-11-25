using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using ManningTable.ViewModel;
using ManningTable.View;
using ManningTable.Helpers;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using Helpers;

namespace ManningTable.Model
{
    public class StaffGroup : NotificationObject, IDataErrorInfo
    {
    #region Var declaration
        decimal? posID, subdivID, classific, tarBySchema, personalTar, tariffGridID, orderID, groupID, comb_addition, harm_addition, staff_section_id;
        private static Dictionary<Tuple<decimal, decimal>, List<DataRow>> tariff_dictionary;
    #endregion

        public StaffGroup()
        {
        }

        

        public StaffFilterProvider StaffFilter
        {
            get;
            set;
        }        

        static StaffGroup()
        {
            tariff_dictionary = new Dictionary<Tuple<decimal, decimal>, List<DataRow>>();
            List<DataRow> p;
            foreach (DataRow r in AppDataSet.Tables["DESC_TAR_GRID"].Rows)
                if (tariff_dictionary.TryGetValue(Tuple.Create(r.Field2<Decimal>("TARIFF_GRID_ID"), r.Field2<Decimal>("TAR_CLASSIF")), out p))
                {
                    p.Add(r);
                }
                else
                    tariff_dictionary[Tuple.Create(r.Field2<Decimal>("TARIFF_GRID_ID"), r.Field2<Decimal>("TAR_CLASSIF"))] = new List<DataRow>(new DataRow[] { r });
        }

#region Main declaration 

        public decimal? GroupID
        {
            get { return groupID; }
            set
            {
                groupID = value;
                RaisePropertyChanged(()=> GroupID);
            }
        }

        public decimal? OrderID
        {
            get { return orderID; }
            set
            {
                orderID = value;
                UpdateGroupedRows(()=>OrderID, value);
                RaisePropertyChanged(()=> OrderID);
            }
        }

        public decimal? TariffGridID
        {
            get { return tariffGridID; }
            set
            {
                if (tariffGridID != value)
                {
                    tariffGridID = value;
                    UpdateGroupedRows(()=>TariffGridID, value);
                    if (value != null) TarBySchema = GetTarSal(value, Classific, DateTime.Now);
                    else TarBySchema = null;
                    RaisePropertyChanged(() => TariffGridID);
                }
            }
        }

        public decimal? PersonalTar
        {
            get { return personalTar; }
            set
            {
                if (personalTar != value)
                {
                    personalTar = value;
                    UpdateGroupedRows(()=>PersonalTar, value);
                    RaisePropertyChanged(() => PersonalTar);
                    RaisePropertyChanged(() => TariffSum);
                    RaisePropertyChanged(() => MonthSum);
                }
            }
        }

        public decimal? TarBySchema
        {
            get { return tarBySchema; }
            set
            {
                if (tarBySchema != value)
                {
                    tarBySchema = value;
                    UpdateGroupedRows(()=>TarBySchema, value);
                    PersonalTar = value;
                    RaisePropertyChanged(() => TarBySchema);
                }
            }
        }

        public decimal? TariffSum
        {
            get
            {
                Decimal? d= (PersonalTar??TarBySchema)*BaseTariff;
                
                return d.HasValue?Math.Round(d.Value, Classific.HasValue?2:0):d;
            }
        }

        public decimal BaseTariff
        {
            get
            {
                return StaffFilter.BaseTariff.CurrentTariff??0;
            }
        }

        [StaffAdditionAttribute("COMB_ADDITION", TypeAdditionID = 2)]
        public decimal? CombAddition
        {
            get
            {
                return comb_addition;
            }
            set
            {
                if (comb_addition != value)
                {
                    comb_addition = value;
                    UpdateGroupedRows(r => r.CombAddition, value);
                    RaisePropertyChanged(() => CombSum);
                    RaisePropertyChanged(() => MonthSum);
                }
            }
        }

        public decimal? CombSum
        {
            get
            {
                decimal? d = comb_addition * BaseTariff;
                
                return d.HasValue? Math.Round(d.Value, 2):d;
            }
        }

        [StaffAdditionAttribute("HARMFUL_ADDITION", TypeAdditionID = 1)]
        public decimal? HarmAddition
        {
            get
            {
                return harm_addition;
            }
            set
            {
                if (harm_addition != value)
                {
                    harm_addition = value;
                    UpdateGroupedRows(r => r.HarmAddition, value);
                }
            }
        }

        
        public decimal? Classific
        {
            get { return classific; }
            set
            {
                classific = value;
                UpdateGroupedRows(()=>Classific, value);
                if (value != null) TarBySchema = GetTarSal(TariffGridID, value, DateTime.Now);
                else TarBySchema = null;
                RaisePropertyChanged(() => Classific);
            }
        }

        public decimal? SubdivID
        {
            get { return subdivID; }
            set
            {
                subdivID = value;
                UpdateGroupedRows(()=>SubdivID, value);
                RaisePropertyChanged(() => SubdivID);
            }
        }

        public decimal? PosID
        {
            get { return posID; }
            set
            {
                posID = value;
                UpdateGroupedRows(()=>PosID, value);
                RaisePropertyChanged(() => PosID);

            }
        }

        /*public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                _comments = value;
                RaisePropertyChanged(() => Comments);
            }
        }*/
        
        public decimal CountRows
        {
            get
            {
                return GroupedStaffs.Count;
            }
            set
            {
                RaisePropertyChanged(() => CountRows);
            }
        }

        public decimal? StaffSectionID
        {
            get
            {
                return staff_section_id;
            }
            set
            {
                staff_section_id = value;
                UpdateGroupedRows(()=>StaffSectionID, value);
                RaisePropertyChanged(()=>StaffSectionID);
            }
        }

#endregion

#region Ссылочные данные внешних таблиц

        public StaffSection StaffSection
        {
            get
            {
                return StaffFilter.StaffSections[staff_section_id];
            }
        }

#endregion

        public decimal? MonthSum
        {
            get
            {
                decimal? d = (TariffSum ?? 0) + (CombSum ?? 0);
                if (d > 0)
                    return Math.Round(d.Value, Classific.HasValue ? 2 : 0);
                else return null;
            }
        }

        ObservableCollection<Staff> grRows;

        public ObservableCollection<Staff> GroupedStaffs
        {
            get
            {
                if (grRows == null && TableView != null)
                {
                    DataRowView[] grouped_rows = TableView.FindRows(GroupID);
                    grRows = new ObservableCollection<Staff>(grouped_rows.Select(r => new Staff(r.Row) 
                    { 
                        StaffTable = this.StaffTable, 
                        StaffFilter = this.StaffFilter,
                        ParentGroup = this
                    }));
                }
                return grRows;
            }
        }

        private void UpdateGroupedRows<T>(Expression<Func<T>> exp, object val)
        {
            if (GroupedStaffs != null && GroupedStaffs.Count>0)
            {
                string PropName = (exp.Body as MemberExpression).Member.Name;
                PropertyInfo prop_info = GroupedStaffs[0].GetType().GetProperty(PropName);
                foreach (Staff r in GroupedStaffs)
                {
                    prop_info.SetValue(r, val, null);
                }
            }
            RaisePropertyChanged(() => GroupedStaffs);
        }
        
        private void UpdateGroupedRows<T>(Expression<Func<StaffGroup, T>> exp, object val)
        {
            if (GroupedStaffs != null)
            {
                StaffAdditionAttribute sa = (exp.Body as MemberExpression).Member.GetCustomAttributes(true).OfType<StaffAdditionAttribute>().FirstOrDefault();
                if (sa != null)
                {
                    if (val == null)
                    {
                        foreach (Staff r in GroupedStaffs)
                        {
                            List<DataRow> list = r.DataRow.GetChildRows("staff_addition_fk").OfType<DataRow>().Where(t => t.Field2<Decimal?>("TYPE_STAFF_ADDITION_ID") == sa.TypeAdditionID).ToList();
                            for (int i = list.Count - 1; i > -1; --i)
                            {
                                list[i].Delete();
                            }
                        }
                    }
                    else
                    {
                        foreach (Staff r in GroupedStaffs)
                        {
                            List<DataRow> list = r.DataRow.GetChildRows("staff_addition_fk").OfType<DataRow>().Where(t => t.Field2<Decimal?>("TYPE_STAFF_ADDITION_ID") == sa.TypeAdditionID).ToList();
                            if (list.Count > 0 && list[0]["ADDITION_VALUE"] != val)
                                list[0]["ADDITION_VALUE"] = val;
                            else
                            {
                                DataTable tt = r.DataRow.Table.DataSet.Tables["STAFF_ADDITION"];
                                DataRow new_row = tt.NewRow();
                                new_row["STAFF_ID"] = r.StaffID;
                                new_row["TYPE_STAFF_ADDITION_ID"] = sa.TypeAdditionID;
                                new_row["ADDITION_VALUE"] = val;
                                tt.Rows.Add(new_row);
                            }
                        }
                    }
                }
            }
            RaisePropertyChanged(() => GroupedStaffs);
        }

        public void Delete()
        {
            for (int i = grRows.Count - 1; i >= 0; --i)
                grRows[i].Delete();
        }

        /// <summary>
        /// Родительская таблица, в которой хранятся штатные единицы
        /// </summary>
        public DataTable StaffTable
        {
            get;
            set;
        }

        public static Decimal? GetTarSal(decimal? tariff_grid_id, decimal? classific, DateTime cur_date)
        {
            List<DataRow> p;
            if (tariff_grid_id.HasValue && classific.HasValue && tariff_dictionary.TryGetValue(Tuple.Create(tariff_grid_id.Value, classific.Value), out p))
                return p.Where(r => cur_date >= r.Field2<DateTime?>("TAR_DATE_BEGIN") && cur_date <= r.Field2<DateTime?>("TAR_DATE_END")).Select(r => r.Field2<Decimal?>("TAR_SAL")).FirstOrDefault();
            else return null;
        }

        public DataView TableView
        {
            get;
            set;
        }

        public string Error
        {
            get
            {
                if (Classific.HasValue && !TariffGridID.HasValue)
                    return "Не установлена тарифная сетка для разряда";
                /*if (PersonalTar > TarBySchema && string.IsNullOrWhiteSpace(Comments))
                    return "Для персонального тарифа не по схеме обязательно для заполнения поле \"Комментарий\"";*/
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName.ToUpper())
                {
                    case "SUBDIVID": if (!SubdivID.HasValue) return "Не выбрано подразделение"; break;
                    case "POSID": if (!PosID.HasValue) return "Не выбрана должность"; break;
                    case "TARBYSCHEMA": if (!TarBySchema.HasValue) return "Не установлен тариф по схеме"; break;
                    //case "COMMENTS": if (PersonalTar > TarBySchema && string.IsNullOrWhiteSpace(Comments)) return "Для персонального коэф-та требуется комментарий"; break;
                    default: return string.Empty;
                }
                return string.Empty;
            }
        }

        public Staff NewStaff()
        {
            DataRow new_row = StaffTable.NewRow();
            StaffTable.Rows.Add(new_row);
            Staff ss = new Staff(new_row) { 
                    StaffTable = this.StaffTable, 
                    ParentGroup =this, 
                    GroupID = this.GroupID, 
                    OrderID = this.OrderID,
                    PosID = this.PosID,
                    StaffFilter = this.StaffFilter,
                    SubdivID = this.subdivID,
                    TariffGridID = this.TariffGridID,
                    Classific = this.Classific, 
                    TarBySchema = this.tarBySchema,
                    PersonalTar = this.PersonalTar,
                    CombAddition = this.CombAddition,
                    HarmAddition = this.HarmAddition,
                    StaffID = Staff.GetStaffID()};
            return ss;
        }
    }
}
