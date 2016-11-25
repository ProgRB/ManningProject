using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Model;
using ManningTable.Helpers;

namespace ManningTable.ViewModel
{
    public class StaffFilterProvider : NotificationObject
    {
        public StaffFilterProvider()
        { 

        }
        decimal? _subdiv_id;
        public decimal? SubdivID
        {
            get
            {
                return _subdiv_id;
            }
            set
            {
                _subdiv_id = value;
                RaisePropertyChanged(() => SubdivID);
            }
        }
        public BaseTariffGroup BaseTariff
        {
            get;
            set;
        }

        StaffSectionList staffSectionList;
        public StaffSectionList StaffSections
        {
            get
            {
                if (staffSectionList == null)
                {
                    StaffSectionList.UpdateStaffSection();
                    staffSectionList = StaffSectionList.BuildTree.RollSection;
                    staffSectionList.CurrentItem = staffSectionList[1];

                    staffSectionList.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(staffSectionList_PropertyChanged);
                }
                return staffSectionList;
            }
        }

        public StaffSection CurrentStaffSection
        {
            get
            {
                return staffSectionList.CurrentItem;
            }
        }

        void staffSectionList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                RaisePropertyChanged(() => CurrentStaffSection);
            }
        }
    }
}
