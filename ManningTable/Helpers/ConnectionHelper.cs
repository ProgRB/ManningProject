using ManningTable.Helpers;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class Connect:NotificationObject
    {
        static OracleConnection _connect = null;
        private static string _schema;

        public static OracleConnection CurConnect
        {
            get
            {
                if (_connect == null)
                    throw new Exception("Не установлено соединение для модуля штатное расписание");
                return _connect;
            }
            set
            {
                _connect = value;
            }
        }

        /// <summary>
        /// Схема 1 это APSTAFF для приложения
        /// </summary>
        public static string Schema
        {
            get
            {
                return "APSTAFF";
            }
        }

        /// <summary>
        /// Схема 2 это SALARY для приложения
        /// </summary>
        public static string SchemaSalary
        {
            get
            {
                return "SALARY";
            }
        }

        public static string SchemaApstaff
        {
            get
            {
                return Schema;
            }
        }

        public static string UserID
        {
            get;
            set;
        }
    }
}
