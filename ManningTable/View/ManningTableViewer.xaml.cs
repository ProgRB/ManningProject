using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Oracle.DataAccess.Client;
using LibraryKadr;
using System.ComponentModel;
using System.Linq.Expressions;
using ManningTable.ViewModel;
using ManningTable.View;
using System.Reflection;
using ManningTable.Model;
using ManningTable.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Helpers;

namespace ManningTable
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ManningTableViewer : UserControl
    {
        StaffFilterProvider staffFilter;

        public ManningTableViewer()
        {
            InitializeComponent();
            this.DataContext = ManningModel;
        }

        public StaffFilterProvider StaffFilter
        {
            get
            {
                if (staffFilter == null)
                {
                    staffFilter = new StaffFilterProvider() { BaseTariff = new BaseTariffGroup() };
                }
                return staffFilter;
            }
            set
            {
                staffFilter = value;
            }
        }

        ManningEditorViewModel _staffs;
        public ManningEditorViewModel ManningModel
        {
            get
            {
                if (_staffs == null)
                    _staffs = new ManningEditorViewModel(this.StaffFilter);
                return _staffs;
            }
        }

        private bool isManualEditCommit = false;
        private void dgStaffs_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEditCommit)
            {
                isManualEditCommit =true;
                DataGrid grid = sender as DataGrid;
                bool fl = false;
                try
                {
                    fl = grid.CommitEdit(DataGridEditingUnit.Row, true);
                }
                catch
                {}
                finally
                {
                    if (!fl)
                    grid.CancelEdit();
                }
                isManualEditCommit = false;
            }
        }

        /*bool IsInScrolling = true;
        private void dgStaffs_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (dgScrollviewer == null)
                    dgScrollviewer = GetScrollbar(dgStaffs);
            if (IsInScrolling && dgScrollviewer.HorizontalOffset!=svTotalStaffs.HorizontalOffset)
            {
                IsInScrolling = false;
                
                if (sender is ScrollViewer)
                {
                    dgScrollviewer.ScrollChanged -= dgStaffs_ScrollChanged;
                    dgScrollviewer.ScrollToHorizontalOffset(dgScrollviewer.HorizontalOffset + e.HorizontalChange);
                    dgScrollviewer.ScrollChanged += dgStaffs_ScrollChanged;
                }
                else
                {
                    svTotalStaffs.ScrollChanged -= dgStaffs_ScrollChanged;
                    svTotalStaffs.ScrollToHorizontalOffset(dgScrollviewer.HorizontalOffset + e.HorizontalChange);
                    svTotalStaffs.ScrollChanged += dgStaffs_ScrollChanged;
                }
                IsInScrolling = true;
            }
        }*/

        static ScrollViewer dgScrollviewer;

        private static ScrollViewer GetScrollbar(DependencyObject dep)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if (child != null && child is ScrollViewer)
                    return child as ScrollViewer;
                else
                {
                    ScrollViewer sub = GetScrollbar(child);
                    if (sub != null)
                        return sub;
                }
            }
            return null;
        }

        [DebuggerStepThrough]
        private void Save_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && ManningModel.HasChanges;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dgStaffs.CommitEdit();
            ManningModel.Save();            
        }

        [DebuggerStepThrough]
        private void AddGroupStaff_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command);
        }

        private void AddGroupStaff_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ManningModel.AddStaffGroup();
        }

        private void DeleteGroupStaff_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(e.Command) && ManningModel.CurrentGroup != null;
        }

        private void DeleteGroupStaff_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Удалить выбранную группу штатных единиц ({0} шт. без возможности восстановления)?", ManningModel.CurrentGroup.CountRows), "Удаление штатных единиц", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ManningModel.DeleteCurrentGroup();
            }
        }

        private void btApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rr = MessageBoxResult.Cancel;
            if (ManningModel.HasChanges)
            {
                if ((rr = MessageBox.Show("В таблицу были внесены изменения. Перед применением фильтра требуется сохранить изменения, иначе они будут утеряны.\n Сохранить изменения?", "Изменения в данных", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)) == MessageBoxResult.Yes)
                {
                    AppManningCommands.SaveStaff.Execute(null, null);
                    filterBindingGroup.CommitEdit();
                    ManningModel.RaiseStaffSourceChanged();
                }
                else
                    if (rr == MessageBoxResult.No)
                    {
                        filterBindingGroup.CommitEdit();
                        ManningModel.RaiseStaffSourceChanged();
                    }
            }
            else
            {
                filterBindingGroup.CommitEdit();
                ManningModel.RaiseStaffSourceChanged();
            }
        }

        private void Expander_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btApplyFilter_Click(this, null);
        }
    }

#region Конвертеры вспомогательные
    
    public class IDToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal || value is string || value is Boolean || value is double)
            {
                string[] st = parameter.ToString().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                DataRow r = AppDataSet.Tables[st[0].ToUpper()].Rows.Find(value);
                if (r != null)
                    return r[st[1]].ToString();
                else return "";
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StaffToTotalConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<StaffGroup> l = (ObservableCollection<StaffGroup>)value;
            if (l != null)
            {
                var t = from r in l
                        group r by 1 into g
                        select new
                            {
                                SumCountStaff = g.Sum(x => x.CountRows),
                                AvgClassific = Math.Round(g.Average(x => x.Classific) ?? 0, 2),
                                AvgTarBySchema = Math.Round(g.Average(x => x.TarBySchema) ?? 0, 2),
                                AvgPersonalTar = Math.Round(g.Average(x => x.PersonalTar) ?? 0, 2),
                                SumSumTariff = g.Sum(x => x.TariffSum),
                                AvgCombAddition = Math.Round(g.Average(x => x.CombAddition) ?? 0, 2),
                                SumSumComb = g.Sum(x => x.CombSum),
                                SumMonthSum = g.Sum(x => x.MonthSum),
                            };
                return t.FirstOrDefault();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

#endregion
    
    
}
