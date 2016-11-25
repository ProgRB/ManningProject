using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class ExceptionHelper
    {
        public static string GetFormattedException(this Exception ex)
        {
            if (ex is OracleException)
                if ((ex as OracleException).Number > 19999 && (ex as OracleException).Number < 25000 && (ex as OracleException).Message.Length > 0)
                {
                    OracleException e = (ex as OracleException);
                    return e.Message.Substring(0, e.Message.IndexOf("ORA-", 10, StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    OracleException orcl_ex = ex as OracleException;
                    switch (orcl_ex.Number)
                    {
                        case 1013: return "Пользователь прервал операцию";
                        case 910: return "Строковый параметр превышает установленную длину";
                        case 1031: return "Не достаточно привилегий";
                        case 1033: return "Сервер базы данных находится в состоянии запуска или отключения";
                        case 1034: return "Сервер базы данных недоступен";
                        case 1062: return "Недостаточно памяти на сервере для создания буфера указанно размера";
                    }
                    return ex.Message;
                }
            else return ex.Message;
        }
    }
}
