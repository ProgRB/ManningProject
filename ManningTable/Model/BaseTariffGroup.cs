using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManningTable.Helpers;
using System.Data;
using Oracle.DataAccess.Client;
using Helpers;

namespace ManningTable.Model
{
    public class BaseTariffGroup : NotificationObject
    {
        DataTable baseTable = new DataTable();
        OracleDataAdapter odaBase = new OracleDataAdapter(string.Format("select * from {0}.base_tariff order by bdate", Connect.Schema), Connect.CurConnect);
        DataRow current_row = null;
        public BaseTariffGroup()
        {
            UpdateBaseTariff();
            CurrentDate = DateTime.Now.Date;
        }
        public void UpdateBaseTariff()
        {
            baseTable.Clear();
            odaBase.Fill(baseTable);
        }
        public DateTime CurrentDate
        {
            get
            {
                return curdate;
            }
            set
            {
                curdate = value;
                if (baseTable != null)
                    current_row = baseTable.Rows.OfType<DataRow>().OrderBy(t => t.Field2<DateTime>("BDATE")).LastOrDefault(t => t.Field2<DateTime>("BDATE") <= curdate);
                else
                    current_row = null;
                RaisePropertyChanged(() => CurrentDate);
                RaisePropertyChanged(() => CurrentTariff);
            }
        }
        public decimal? CurrentTariff
        {
            get
            {
                return current_row == null ? null : current_row.Field2<Decimal?>("TARIFF");
            }
        }
        private DateTime curdate = DateTime.Now;
    }
}
