using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace ManningTable.View
{
    public interface IEditable
    {
        void SetValue(Control sender, DependencyProperty property);
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute, IEditable
    {
        public EditableAttribute(bool isEditable)
        {
            this.ReadOnly = isEditable;
        }
        public virtual bool ReadOnly { get; set; }

        public void SetValue(Control sender, DependencyProperty property)
        {
            sender.SetValue(property, this.ReadOnly);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class StaffAdditionAttribute : Attribute
    {
        string additionName;
        public int addition_id;
        public StaffAdditionAttribute(string addName)
        {
            this.additionName = addName;
            addition_id = 0;
        }
        public string GetAdditionName()
        {
            return additionName;
        }
        public int TypeAdditionID
        {
            get
            {
                return addition_id;
            }
            set
            {
                addition_id = value;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataMember : Attribute
    {
        public string MemberProperty
        {
            get;
            set;
        }
    }
}
