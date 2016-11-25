using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using Oracle.DataAccess.Client;
using LibraryKadr;

namespace Helpers
{
    /// <summary>
    /// Статичный класс для данных, одинаковых для всего приложения. Справочники приложения
    /// </summary>
    public class AppDataSet
    {
        static DataSet ds;
        static Dictionary<string, OracleDataAdapter> dic_set = new Dictionary<string, OracleDataAdapter>(StringComparer.CurrentCultureIgnoreCase);
        static AppDataSet()
        {
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                {
                    ds = new DataSet();
                    dic_set.Add("ORDER", new OracleDataAdapter(string.Format(@"select order_id, order_name from {0}.orders", Connect.Schema), Connect.CurConnect));
                    dic_set.Add("DEGREE", new OracleDataAdapter(string.Format(@"select * from {0}.DEGREE", Connect.Schema), Connect.CurConnect));
                    dic_set.Add("SUBDIV", new OracleDataAdapter(string.Format(@"select DECODE(subdiv_id, 0, '000', CODE_SUBDIV) code_subdiv, DECODE(SUBDIV_ID,0, 'У-УАЗ', SUBDIV_NAME) subdiv_name, subdiv_id, 
                                                                                  type_subdiv_id, sub_actual_sign, WORK_TYPE_ID, SERVICE_ID, SUB_DATE_START, SUB_DATE_END, PARENT_ID, GR_WORK_ID from {0}.SUBDIV", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    dic_set.Add("ACCESS_SUBDIV", new OracleDataAdapter(string.Format(@"select DECODE(subdiv_id, 0, '000', CODE_SUBDIV) code_subdiv, DECODE(SUBDIV_ID,0, 'У-УАЗ', SUBDIV_NAME) subdiv_name, subdiv_id, app_name from {0}.subdiv_roles_all where sub_level<3", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    dic_set.Add("TARIFF_GRID", new OracleDataAdapter(string.Format("select * from {0}.TARIFF_GRID", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    //dic_set.Add("DESC_TAR_GRID", new OracleDataAdapter(string.Format(Queries.GetQuery("Staff/SelectDescTarGrid.sql"), Connect.Schema), Connect.CurConnect));
                    dic_set.Add("POSITION", new OracleDataAdapter(string.Format("select pos_id, POS_NAME, CODE_POS from {0}.POSITION where pos_actual_sign=1", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    dic_set.Add("BASE_TARIFF", new OracleDataAdapter(string.Format("select * from {0}.base_tariff", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    dic_set.Add("TYPE_STAFF_ADDITION", new OracleDataAdapter(string.Format("select * from {0}.TYPE_STAFF_ADDITION", Connect.Schema, Connect.SchemaSalary), Connect.CurConnect));
                    UpdateAll();

                    /*EntityConnection conn = new EntityConnection(Library.GetEFConnectionMetadata());
                    conn.Open();
                    using (SalaryEntities context = new SalaryEntities(conn))
                    {
                        var cur_payment = context.ExecuteStoreQuery<PAYMENT_TYPE>(,
                        var res = from c in context.PAYMENT_TYPE
                                  where c.CODE_PAYMENT == "287"
                                  select c;
                        foreach (var t in res)
                        {
                            Console.WriteLine(t.CODE_PAYMENT);
                        }
                    }*/
                }
            }
            catch (Exception ex)
            { };
        }

        public AppDataSet()
        {
        }

        public DataTable this[string TableName]
        {
            get
            {
                return ds.Tables[TableName];
            }
        }

        public DataView this[string TableName, string Sort]
        {
            get
            {
                return new DataView(ds.Tables[TableName], "", Sort, DataViewRowState.CurrentRows) ;
            }
        }

        public DataView this[string TableName, string Sort, string Filter]
        {
            get
            {
                return new DataView(ds.Tables[TableName], Filter, Sort, DataViewRowState.CurrentRows);
            }
        }

        public DataTable DEGREE
        {
            get
            {
                return ds.Tables["DEGREE"];
            }
        }
        public static DataView GetSubdivView(string Filter = null)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                try
                {
                    return new DataView(ds.Tables["ACCESS_SUBDIV"], string.IsNullOrEmpty(Filter) ? "" : string.Format("APP_NAME='{0}'", Filter), "", DataViewRowState.CurrentRows);
                }
                catch (Exception ex)
                { return null; }
            else return null;
        }

        public static void UpdateSet(string TableName)
        {
            if (dic_set.ContainsKey(TableName))
            {
                dic_set[TableName].Fill(ds, TableName);
                if (ds.Tables[TableName].PrimaryKey.Length == 0)
                 if (ds.Tables[TableName].Columns.Contains(TableName + "_ID"))
                    ds.Tables[TableName].PrimaryKey = new DataColumn[] { ds.Tables[TableName].Columns[TableName + "_ID"] };
                 else
                     if (TableName.ToUpper()=="POSITION")
                         ds.Tables[TableName].PrimaryKey = new DataColumn[] { ds.Tables[TableName].Columns["POS_ID"] };
                     else
                        if (TableName.ToUpper() == "ORDERS")
                            ds.Tables[TableName].PrimaryKey = new DataColumn[] { ds.Tables[TableName].Columns["ORDER_ID"] };
            }
        }

        public static DataTableCollection Tables
        {
            get
            {
                return ds.Tables;
            }
        }

        public static void UpdateAll()
        {
            foreach (string s in dic_set.Keys)
            {
                UpdateSet(s);
            }
        }

    }
}
