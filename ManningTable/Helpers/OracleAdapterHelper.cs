using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class OracleAdapterHelper
    {
        /// <summary>
        /// Попытка обновить данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ds">DataSet для обновления данных</param>
        /// <param name="classWithParameters">Класс содержащий информацию о параметрамх ([OracleParameterMapping] attribute)</param>
        /// <returns></returns>
        public static Exception TryFillWithClear(this OracleDataAdapter sender, DataSet ds, object classWithParameters)
        {
            if (ds == null) ds = new DataSet();
            foreach (DataTableMapping p in sender.TableMappings)
            {
                if (ds.Tables.Contains(p.DataSetTable))
                {
                    ds.Tables[p.DataSetTable].Rows.Clear();
                }
            }
            sender.SelectCommand.SetParameters(classWithParameters);
            try
            {
                sender.Fill(ds);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
