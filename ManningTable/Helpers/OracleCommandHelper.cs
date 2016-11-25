using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Helpers
{
    public static class OracleCommandHelper
    {
        
        /// <summary>
        /// Данное расширение автоматически проставляет команде значения из класса согласно мэппингу атрибутов
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sourceValue">Источник значений, каждое значение для параметра должно быть отмечено атрибутом [OracleParameterMapping]</param>
        public static void SetParameters(this OracleCommand cmd, object sourceValue)
        {
            if (cmd == null)
                return;
            foreach (OracleParameter c in cmd.Parameters.OfType<OracleParameter>().Where(r => r.Direction != System.Data.ParameterDirection.Output))
            {
                PropertyInfo p = sourceValue.GetType().GetProperties()
                    .Where(r => r.GetCustomAttributes(typeof(OracleParameterMapping), true)
                                .Any(r1 => (r1 as OracleParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p != null)
                {
                    c.Value = p.GetValue(sourceValue, null);
                    continue;
                }
                FieldInfo p1 = sourceValue.GetType().GetFields()
                    .Where(r => r.GetCustomAttributes(typeof(OracleParameterMapping), true)
                                .Any(r1 => (r1 as OracleParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p1 != null)
                {
                    c.Value = p1.GetValue(sourceValue);
                    continue;
                }

                MethodInfo p2 = sourceValue.GetType().GetMethods()
                    .Where(r => r.GetCustomAttributes(typeof(OracleParameterMapping), true)
                                .Any(r1 => (r1 as OracleParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p2 != null)
                {
                    c.Value = p2.Invoke(sourceValue, null);
                    continue;
                }
            }
        }
    }
    /// <summary>
    /// Атрибут для класса, помогающий установить занчения для команды Оракл
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
    public class OracleParameterMapping : Attribute
    {
        /// <summary>
        /// Имя параметра, которому требуется присвоить искомое значение
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }
    }
}
