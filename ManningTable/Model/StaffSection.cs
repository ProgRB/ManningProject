using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Helpers;
using System.Data;
using System.Windows;

namespace ManningTable.Model
{
    public class StaffSection : NotificationObject
    {
        decimal? _staff_section_id;
        public decimal? StaffSectionID
        {
            get
            {
                return _staff_section_id;
            }
            set
            {
                _staff_section_id = value;
                this.RaisePropertyChanged(() => StaffSectionID);
            }
        }

        string _name_section;
        public string NameSection
        {
            get
            {
                return _name_section;
            }
            set
            {
                _name_section = value;
                this.RaisePropertyChanged(() => NameSection);
            }
        }

        List<StaffSection> childSection = null;
        public List<StaffSection> ChildSection
        {
            get
            {
                if (childSection == null)
                    childSection = (from DataRow r in sectionTable.Rows
                                    where r.Field2<Decimal?>("PARENT_SECTION_ID") == this.StaffSectionID
                                    select new StaffSection()
                                    {
                                        SectionTable = this.SectionTable,
                                        NameSection = r["NAME_SECTION"].ToString(),
                                        StaffSectionID = r.Field2<Decimal?>("STAFF_SECTION_ID"),
                                        ParentSectionID = r.Field2<Decimal?>("PARENT_SECTION_ID"),
                                        ParentSection = this
                                    }).OrderBy(t => t.NameSection).ToList();
                return childSection;
            }
        }

        DataTable sectionTable;
        public DataTable SectionTable
        {
            get
            {
                return sectionTable;
            }
            set
            {
                sectionTable = value;
            }
        }

        public decimal? ParentSectionID
        {
            get;
            set;
        }

        public StaffSection ParentSection
        {
            get;
            set;
        }

        public int Level
        {
            get
            {
                int k = 0;
                StaffSection tempSec = this.ParentSection;
                while (k < 100 && tempSec != null)
                {
                    ++k;
                    tempSec = tempSec.ParentSection;
                }
                return k;
            }
        }
        public Thickness PaddingLevel
        {
            get
            {
                return new Thickness(Level * 15, 2, 0, 2);
            }
        }

        public override string ToString()
        {
            return this.NameSection;
        }
    }
}
