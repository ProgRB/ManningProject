using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Helpers;
using System.ComponentModel;
using System.Data;
using ManningTable.ViewModel;
using ManningTable.View;
using System.Linq.Expressions;
using Oracle.DataAccess.Client;
using Helpers;

namespace ManningTable.Model
{
    public class Staff: NotificationObject, IDataErrorInfo
    {

        public Staff(DataRow d_row)
        {
            this.StaffTable = d_row.Table;
            data_row = d_row;
        }

        public Staff(DataTable staffTable)
        {
            this.StaffTable = staffTable;
            DataRow new_row = staffTable.NewRow();
            staffTable.Rows.Add(new_row);
            data_row = new_row;
        }

#region Main Propreties

        [DataMember(MemberProperty="STAFF_ID")]
        public decimal? StaffID
        {
            get
            {
                return GetDataRowField(()=> StaffID);
            }
            set
            {
                UpdateDataRow(t=>t.StaffID, value);
                RaisePropertyChanged(() => StaffID);
            }
        }

        [DataMember(MemberProperty="STAFF_GROUP_ID")]
        public decimal? GroupID
        {
            get { return GetDataRowField(() => GroupID); }
            set
            {
                UpdateDataRow(r => r.GroupID, value);
                RaisePropertyChanged(()=> GroupID);
            }
        }

        [DataMember(MemberProperty="CLASSIFIC")]
        public decimal? Classific
        {
            get { return GetDataRowField(()=>Classific); }
            set
            {
                UpdateDataRow(p=>p.Classific, value);
                if (value != null) TarBySchema = StaffGroup.GetTarSal(TariffGridID, value, DateTime.Now);
                else TarBySchema = null;
                RaisePropertyChanged(() => Classific);
            }
        }

        [DataMember(MemberProperty="SUBDIV_ID")]
        public decimal? SubdivID
        {
            get 
            { 
                return GetDataRowField(() => SubdivID); 
            }
            set
            {
                UpdateDataRow(p=>p.SubdivID, value);
                RaisePropertyChanged(() => SubdivID);
            }
        }

        [DataMember(MemberProperty="POS_ID")]
        public decimal? PosID
        {
            get 
            {
                return GetDataRowField(() => PosID); 
            }
            set
            {
                UpdateDataRow(p=>p.PosID, value);
                RaisePropertyChanged(() => PosID);

            }
        }

        [DataMember(MemberProperty = "TAR_BY_SCHEMA")]
        public decimal? TarBySchema
        {
            get 
            {
                return GetDataRowField(() => TarBySchema); 
            }
            set
            {
                UpdateDataRow(p=>p.TarBySchema, value);
                PersonalTar = value;
                RaisePropertyChanged(() => TarBySchema);
            }
        }

        public decimal? TariffSum
        {
            get
            {
                return Math.Round((PersonalTar??TarBySchema??0m) * BaseTariff, Classific.HasValue?2:0);
            }
        }

        [DataMember(MemberProperty = "ORDER_ID")]
        public decimal? OrderID
        {
            get 
            { 
                return GetDataRowField(()=>OrderID); 
            }
            set
            {
                UpdateDataRow(p=>p.OrderID, value);
                RaisePropertyChanged(()=> OrderID);
            }
        }

        [DataMember(MemberProperty = "TARIFF_GRID_ID")]
        public decimal? TariffGridID
        {
            get 
            { 
                return GetDataRowField(() => TariffGridID); 
            }
            set
            {
                UpdateDataRow(p=>p.TariffGridID, value);
                if (value != null) TarBySchema = StaffGroup.GetTarSal(value, Classific, DateTime.Now);
                else TarBySchema = null;
                RaisePropertyChanged(() => TariffGridID);
            }
        }

        [DataMember(MemberProperty = "PERSONAL_TAR")]
        public decimal? PersonalTar
        {
            get 
            { 
                return GetDataRowField(() => PersonalTar); 
            }
            set
            {
                UpdateDataRow(p=>p.PersonalTar, value);
                RaisePropertyChanged(() => PersonalTar);
                RaisePropertyChanged(() => TariffSum);
                RaisePropertyChanged(() => MonthSum);
            }
        }

        [DataMember(MemberProperty="COMMENTS")]
        public string Comments
        {
            get
            {
                return GetDataRowField(() => Comments);
            }
            set
            {
                UpdateDataRow(p => p.Comments, value);
                RaisePropertyChanged(() => Comments);
            }
        }

        [StaffAdditionAttribute("COMB_ADDITION", TypeAdditionID = 2)]
        public decimal? CombAddition
        {
            get
            {
                return GetDataRowField(() => CombAddition);
            }
            set
            {
                UpdateDataRow(r => r.CombAddition, value);
                RaisePropertyChanged(() => CombAddition);
                RaisePropertyChanged(() => CombSum);
                RaisePropertyChanged(() => MonthSum);
            }
        }

        [StaffAdditionAttribute("HARMFUL_ADDITION", TypeAdditionID = 1)]
        public decimal? HarmAddition
        {
            get
            {
                return GetDataRowField(() => HarmAddition);
            }
            set
            {
                UpdateDataRow(r => r.HarmAddition, value);
                RaisePropertyChanged(() => HarmAddition);
            }
        }

        [DataMember(MemberProperty="STAFF_SECTION_ID")]
        public decimal StaffSectionID
        {
            get
            {
                return GetDataRowField(() => StaffSectionID);
            }
            set
            {
                UpdateDataRow(r => r.StaffSectionID, value);
                RaisePropertyChanged(() => StaffSectionID);
            }
        }
#endregion

        public StaffSection StaffSection
        {
            get
            {
                return StaffFilter.StaffSections[GetDataRowField(()=>StaffSectionID)];
            }
        }

        public StaffFilterProvider StaffFilter
        {
            get;
            set;
        }

        public DataView TableView
        {
            get;
            set;
        }

        public StaffGroup ParentGroup
        {
            get;
            set;
        }

        public void Delete()
        {
            if (ParentGroup.CountRows == 0)
                return;
            else
            {
                ParentGroup.GroupedStaffs.Remove(this);
                data_row.Delete();
            }
        }

        public decimal BaseTariff
        {
            get
            {
                return StaffFilter.BaseTariff.CurrentTariff??0;
            }
        }

        public decimal? CombSum
        {
            get
            {
                return Math.Round((CombAddition??0m) * BaseTariff, 2);
            }
        }

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

        static OracleCommand cmdGetId = new OracleCommand(String.Format("select {0}.staff_id_seq.nextval from dual", Connect.Schema), Connect.CurConnect);
        public static decimal? GetStaffID()
        {
            return (decimal)cmdGetId.ExecuteScalar();
        }
        DataRow data_row;

        public DataRow DataRow
        {
            get
            {
                return data_row;
            }
            set
            {
                data_row = value;
            }
        }

        /// <summary>
        /// Родительская таблица, в которой хранятся штатные единицы
        /// </summary>
        public DataTable StaffTable
        {
            get;
            set;
        }

        private void UpdateDataRow<T>(Expression<Func<Staff, T>> expr, object val)
        {
            if (data_row != null && data_row.RowState != DataRowState.Detached)
            {
                object[] attrib_info = (expr.Body as MemberExpression).Member.GetCustomAttributes(true);

                StaffAdditionAttribute sa = attrib_info.OfType<StaffAdditionAttribute>().FirstOrDefault();
                if (sa != null)
                {
                    if (val == null)
                    {
                        List<DataRow> list = data_row.GetChildRows("staff_addition_fk", DataRowVersion.Current).OfType<DataRow>().Where(t => t.Field2<Decimal?>("TYPE_STAFF_ADDITION_ID") == sa.TypeAdditionID).ToList();
                        for (int i = list.Count - 1; i > -1; --i)
                        {
                            list[i].Delete();
                        }
                    }
                    else
                    {
                        List<DataRow> list = data_row.GetChildRows("staff_addition_fk", DataRowVersion.Current).OfType<DataRow>().Where(t => t.Field2<Decimal?>("TYPE_STAFF_ADDITION_ID") == sa.TypeAdditionID).ToList();
                        if (list.Count > 0 && list[0]["ADDITION_VALUE"] != val)
                            list[0]["ADDITION_VALUE"] = val;
                        else
                        {
                            DataTable tt = data_row.Table.DataSet.Tables["STAFF_ADDITION"];
                            DataRow new_row = tt.NewRow();
                            new_row["STAFF_ID"] = this.StaffID;
                            new_row["TYPE_STAFF_ADDITION_ID"] = sa.TypeAdditionID;
                            new_row["ADDITION_VALUE"] = val;
                            tt.Rows.Add(new_row);
                        }
                    }
                }
                else
                {
                    DataMember dm = attrib_info.OfType<DataMember>().FirstOrDefault();
                    if (dm != null)
                    {
                        data_row[dm.MemberProperty] = val == null ? DBNull.Value : val;
                    }
                    else
                        throw new Exception("Для свойства не установлен поле источник данных DataRow (DataMember)");
                }
            }
        }

        private T GetDataRowField<T>(Expression<Func<T>> expr)
        {
            if (data_row!=null && data_row.RowState!= DataRowState.Detached)
            {
                object[] attrib_info = (expr.Body as MemberExpression).Member.GetCustomAttributes(true);

                StaffAdditionAttribute sa = attrib_info.OfType<StaffAdditionAttribute>().FirstOrDefault();
                if (sa != null)
                {
                    DataRow r = data_row.GetChildRows("staff_addition_fk", DataRowVersion.Current).OfType<DataRow>().Where(t => t.Field2<Decimal?>("TYPE_STAFF_ADDITION_ID") == sa.TypeAdditionID).FirstOrDefault();
                    if (r != null)
                        return r.Field2<T>("ADDITION_VALUE");
                    else
                        return default(T);
                }
                else
                {
                    DataMember dm = (expr.Body as MemberExpression).Member.GetCustomAttributes(true).OfType<DataMember>().FirstOrDefault();
                    if (dm != null)
                    {
                        return data_row.Field2<T>(dm.MemberProperty);
                    }
                    else
                        throw new Exception("Для свойства не установлен поле источник данных DataRow (DataMember)");
                }
            }
            return default(T);
        }

        #region IDataError info
        public string Error
        {
            get
            {
                if (Classific.HasValue && !TariffGridID.HasValue)
                    return "Не установлена тарифная сетка для разряда";
                if (PersonalTar > TarBySchema && string.IsNullOrWhiteSpace(Comments))
                    return "Для персонального тарифа не по схеме обязательно для заполнения поле \"Комментарий\"";
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
                    case "COMMENTS": if (PersonalTar > TarBySchema && string.IsNullOrWhiteSpace(Comments)) return "Для персонального коэф-та требуется комментарий"; break;
                    default: return string.Empty;
                }
                return string.Empty;
            }
        }
        #endregion
    }
}
