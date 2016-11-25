using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Model;
using System.Data;
using Oracle.DataAccess.Client;
using System.ComponentModel;
using System.Linq.Expressions;
using Helpers;

namespace ManningTable.ViewModel
{
    public class StaffSectionList : List<StaffSection>, INotifyPropertyChanged
    {
        static DataTable staffSectionTable;
        static OracleDataAdapter odaStaff_Section;
        static StaffSectionList()
        {
            odaStaff_Section = new OracleDataAdapter(string.Format(Queries.GetQuery(@"staff/SelectStaffSection.sql"), Connect.Schema), Connect.CurConnect);
            staffSectionTable = new DataTable();
        }
        
        public StaffSectionList RollSection
        {
            get
            {
                StaffSectionList l = new StaffSectionList();
                foreach (StaffSection s in this)
                    l.AddRange(RollInnerNodes(s));
                return l;
            }
        }
        private StaffSectionList RollInnerNodes(StaffSection ss)
        {
            StaffSectionList  ll = new StaffSectionList();
            ll.Add(ss);
            foreach (StaffSection s in ss.ChildSection)
            {
                ll.AddRange(RollInnerNodes(s));
            }
            return ll;
        }
        public static StaffSectionList BuildTree
        {
            get
            {
                var lst = (from DataRow r in StaffSectionDataTable.Rows
                           where r["PARENT_SECTION_ID"] == DBNull.Value
                           select new StaffSection()
                           {
                               NameSection = r["NAME_SECTION"].ToString(),
                               SectionTable = StaffSectionDataTable,
                               StaffSectionID = r.Field2<Decimal?>("STAFF_SECTION_ID")
                           }).OrderBy(t => t.NameSection).ToList();
                StaffSectionList l = new StaffSectionList();
                l.AddRange(lst);
                return l;
            }
        }

        StaffSection current_section;
        public StaffSection CurrentItem
        {
            get
            {
                return current_section;
            }
            set
            {
                current_section = value;
                OnPropertyChanged(t => t.CurrentItem);
            }
        }
        public StaffSection this[decimal? staffSectionID]
        {
            get
            {
                return this.FirstOrDefault(r => r.StaffSectionID == staffSectionID);
            }
        }
        public static DataTable StaffSectionDataTable
        {
            get
            {
                return staffSectionTable;
            }
            set
            {
                staffSectionTable = value;
            }
        }
        public static void UpdateStaffSection()
        {
            staffSectionTable.Clear();
            if (staffSectionTable.PrimaryKey.Length == 0)
                staffSectionTable.PrimaryKey = new DataColumn[] { staffSectionTable.Columns["STAFF_SECTION_ID"] };
            odaStaff_Section.Fill(staffSectionTable);
        }
#region INotifyRegion
        public event PropertyChangedEventHandler  PropertyChanged;
        private void OnPropertyChanged<T>(Expression<Func<StaffSectionList, T>> exp)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs((exp.Body as MemberExpression).Member.Name));
            }
        }

#endregion
    }
}
