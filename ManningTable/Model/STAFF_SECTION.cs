using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Helpers;

namespace ManningTable.Model
{
    class STAFF_SECTION: NotificationObject
    {
        decimal? _staff_section_id;
        public decimal? STAFF_SECTION_ID
        {
            get
            {
                return _staff_section_id;
            }
            set
            {
                _staff_section_id = value;
                this.RaisePropertyChanged(() => STAFF_SECTION_ID);
            }
        }

        string _name_section;
        public string NAME_SECTION
        {
            get
            {
                return _name_section;
            }
            set
            {
                _name_section = value;
                this.RaisePropertyChanged(() => NAME_SECTION);
            }
        }

        decimal? _parent_section_id;
        public decimal? PARENT_SECTION_ID
        {
            get
            {
                return _parent_section_id;
            }
            set
            {
                _parent_section_id = value;
                this.RaisePropertyChanged(() => PARENT_SECTION_ID);
            }
        }
    }
}
