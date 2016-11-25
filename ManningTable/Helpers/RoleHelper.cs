using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Input;
using Oracle.DataAccess.Client;
using System.Data;

namespace Helpers
{
    public class ControlRoles
    {
        private static HashSet<string> _controlRoles;
        static ControlRoles()
        {
            //_controlRoles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
        public static bool GetState(string ControlName)
        {
            if (_controlRoles == null)
                LoadControlRoles();
            return _controlRoles.Contains(ControlName.ToUpper());
        }
        public static bool GetState(ICommand cmd)
        {
            if (_controlRoles == null)
                LoadControlRoles();
            if (cmd is RoutedUICommand)
                return GetState((cmd as RoutedUICommand).Name);
            else return false;
        }

        /// <summary>
        /// Установка всех загруженных ролей для сотрудника
        /// </summary>
        /// <param name="listRoles"></param>
        public static void SetControlRoles(HashSet<string> listRoles)
        {
            _controlRoles = listRoles;
        }

        public static void LoadControlRoles()
        {
            if (Connect.CurConnect != null)
            {
                _controlRoles.Clear();
                OracleCommand cmd = new OracleCommand(string.Format("select * from {0}.user_roles where upper(role_name) in (select granted_role from user_role_privs) or upper(role_name) in (select granted_role from role_role_privs) or upper(trim(role_name)) = upper(trim(ora_login_user))", Connect.Schema), Connect.CurConnect);
                DataTable t = new DataTable();
                new OracleDataAdapter(cmd).Fill(t);
                for (int i = 0; i < t.Rows.Count; ++i)
                    if (!_controlRoles.Contains(t.Rows[i]["Component_Name"].ToString().ToUpper()))
                    {
                        _controlRoles.Add(t.Rows[i]["Component_Name"].ToString().ToUpper());
                    }
            }
        }
    }
}
