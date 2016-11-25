using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using Helpers;

namespace ManningTable
{
    class ManningLibrary
    {
    }
    public static class DataRowExtension
    {
        public static T Field2<T>(this DataRow sender, string FieldName)
        {
            if (sender[FieldName] == DBNull.Value) return default(T); 
            else return  sender.Field<T>(FieldName);
        }
    }
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double c = double.Parse(parameter.ToString(), CultureInfo.InvariantCulture );
            return ((double)value) * c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double c = double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
            return ((double)value) - c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToGridDetVisConveter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? DataGridRowDetailsVisibilityMode.VisibleWhenSelected : DataGridRowDetailsVisibilityMode.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListItemDetailsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EditableTemplate
        {
            get;
            set;
        }
        public DataTemplate ViewTemplate
        {
            get;
            set;
        }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (container != null)
            {
                return (container as ListViewItem).IsSelected ? EditableTemplate : ViewTemplate;
            }
            return ViewTemplate;
        }
    }

    public class MultiSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d= values.Sum(r => double.IsNaN((double)r)?0:(double)r) + double.Parse((parameter==null?"0":parameter.ToString()), CultureInfo.InvariantCulture);
            return new GridLength(d);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToDoubleConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                if (parameter == null)
                    return -1;
                else return double.Parse(parameter.ToString());
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class DataGridHelper
    {

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T TryFindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }


        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T TryFindFromPoint<T>(UIElement reference, System.Windows.Point point)
          where T : DependencyObject
        {
            DependencyObject element = reference.InputHitTest(point)
                                         as DependencyObject;
            if (element == null) return null;
            else if (element is T) return (T)element;
            else return TryFindParent<T>(element);
        }



    }

    public class DataGridAddition
    {
        public static readonly DependencyProperty DoubleClickCommandProperty =
                DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(RoutedUICommand), typeof(DataGridAddition),
                    new PropertyMetadata(OnDoubleClick_PropertyChanged))/*,
                    ColumnSavingProperty = DependencyProperty.RegisterAttached("ColumnSaving", typeof(bool), typeof(DataGridAddition),
                    new PropertyMetadata(OnColumnSaving_PropertyChanged))*/;
        public static RoutedUICommand GetDoubleClickCommand(DependencyObject e)
        {
            return (RoutedUICommand)e.GetValue(DoubleClickCommandProperty);
        }
        public static void SetDoubleClickCommand(DependencyObject obj, RoutedUICommand e)
        {
            obj.SetValue(DoubleClickCommandProperty, e);
        }

        /*public static bool GetColumnSaving(DependencyObject e)
        {
            return (bool)e.GetValue(ColumnSavingProperty);
        }
        public static void SetColumnSaving(DependencyObject obj, bool value)
        {
            obj.SetValue(ColumnSavingProperty, value);
        }*/

        public static void OnDoubleClick_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action<Control, DependencyPropertyChangedEventArgs>(SetDataGridDoubleClick), sender as Control, e);
        }

        public static void OnColumnSaving_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DataGrid)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        new Action<Control, DependencyPropertyChangedEventArgs>(SetDataGridDoubleClick), sender as Control, e);
        }
        
        private static void SetDataGridDoubleClick(Control sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                if ((bool)e.NewValue)
                    (sender as DataGrid).Columns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                else
                    (sender as DataGrid).Columns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
        }

        static void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (DataGridColumn d in e.NewItems)
            { 
            }
        }

        static void sender_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IInputElement elem = e.MouseDevice.DirectlyOver;
            if (e.ChangedButton == MouseButton.Left && elem != null && elem is FrameworkElement)
            {
                FrameworkElement f = elem as FrameworkElement;
                if (f.TryFindParent<DataGridRow>() != null || f.TryFindParent<Xceed.Wpf.DataGrid.Cell>() != null)
                {
                    Control dg = sender as Control;
                    RoutedUICommand r = dg.GetValue(DataGridAddition.DoubleClickCommandProperty) as RoutedUICommand;
                    if (r != null)
                        r.Execute(null, elem);
                }
            }
        }
    }

    public partial class ApplicationHelper
    {
        /// <summary>
        /// Возвращает папку где выполняется приложение. Корневой каталог
        /// </summary>
        public static string CurrentAppPath
        {
            get
            { return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName); }
        }
    }

    public partial class Queries
    {
        /// <summary>
        /// Метод получения тела запроса по его имени 
        /// </summary>
        /// <param name="queryName">Имя запроса(файла) с расширением (пример: employee.sql)</param>
        /// <param name="year">Год для использования запроса, если не задан то не использует подпапку с годом</param>
        /// <returns>Тело запроса</returns>
        public static string GetQuery(string queryName, decimal? current_year = null)
        {
            TextReader reader = new StreamReader(ApplicationHelper.CurrentAppPath + "/Queries/" + (current_year.HasValue ? current_year.Value.ToString() + "/" : string.Empty) + queryName, Encoding.GetEncoding(1251));
            string st = reader.ReadToEnd();
            reader.Close();
            return st;
        }

        /// <summary>
        /// Возвращает запрос с подставленными схемами {0} - apstaff,   {1} - salary
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static string GetQueryWithSchema(string queryName)
        {
            return string.Format(GetQuery(queryName), Connect.Schema, Connect.SchemaSalary);
        }
    }
}
