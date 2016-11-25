using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ManningTable.ViewModel
{
    class AppManningCommands
    {
        static AppManningCommands()
        {
            AddEmpStaff = new RoutedUICommand("Добавить фактического работника", "EditEmpStaff", typeof(AppManningCommands));
            AddStaff = new RoutedUICommand("Добавить штатную единицу", "EditStaff", typeof(AppManningCommands));
            AddGroupStaff = new RoutedUICommand("Добавить группу штатных единиц", "EditStaff", typeof(AppManningCommands));
            DeleteGroupStaff = new RoutedUICommand("Удалить группу штатных единиц", "EditStaff", typeof(AppManningCommands));
            DeleteStaff = new RoutedUICommand("Удалить группу штатных единиц", "EditStaff", typeof(AppManningCommands));
            SaveStaff = new RoutedUICommand("Сохранить штатное расписание", "EditStaff", typeof(AppManningCommands));
            SaveStaffSection = new RoutedUICommand("Сохранить раздел", "EditManningCatalog", typeof(AppManningCommands));
            AddStaffSection = new RoutedUICommand("Добавить раздел", "EditManningCatalog", typeof(AppManningCommands));
            EditStaffSection = new RoutedUICommand("Редактировать раздел", "EditManningCatalog", typeof(AppManningCommands));
            DeleteStaffSection = new RoutedUICommand("Удалить раздел", "EditManningCatalog", typeof(AppManningCommands));

            SaveSubdivTypePart = new RoutedUICommand("Сохранить типы внутриструктурных подразделений", "EditManningCatalog", typeof(AppManningCommands));
            AddSubdivTypePart = new RoutedUICommand("Добавить тип подструктуры", "EditManningCatalog", typeof(AppManningCommands));
            DeleteSubdivTypePart = new RoutedUICommand("Удалить тип подструктуры", "EditManningCatalog", typeof(AppManningCommands));

            AddSubdivPartition = new RoutedUICommand("Добавить внутриструктурное подразделение", "EditManningCatalog", typeof(AppManningCommands));
            EditSubdivPartition = new RoutedUICommand("Редактировать внутриструктурное подразделение", "EditManningCatalog", typeof(AppManningCommands));
            DeleteSubdivPartition = new RoutedUICommand("Удалить внутриструктурное подразделение", "EditManningCatalog", typeof(AppManningCommands));
            SaveSubdivPartition = new RoutedUICommand("Сохранить внутриструктурное подразделение", "EditManningCatalog", typeof(AppManningCommands));

            SaveIndividProtection = new RoutedUICommand("Сохранить справочник СИЗ", "EditIndividProtectionType", typeof(AppManningCommands));
        }

        public static RoutedUICommand AddEmpStaff { get; set; }

        public static RoutedUICommand AddStaff { get; set; }

        public static RoutedUICommand DeleteStaff { get; set; }


        public static RoutedUICommand AddGroupStaff { get; set; }

        public static RoutedUICommand DeleteGroupStaff { get; set; }

        public static RoutedUICommand SaveStaff { get; set; }

        public static RoutedUICommand SaveStaffSection { get; set; }

        public static RoutedUICommand AddStaffSection { get; set; }

        public static RoutedUICommand EditStaffSection { get; set; }

        public static RoutedUICommand DeleteStaffSection { get; set; }

        public static RoutedUICommand SaveSubdivTypePart { get; set; }

        public static RoutedUICommand AddSubdivTypePart { get; set; }

        public static RoutedUICommand DeleteSubdivTypePart { get; set; }

        public static RoutedUICommand SaveSubdivPartition { get; set; }

        public static RoutedUICommand AddSubdivPartition { get; set; }

        public static RoutedUICommand EditSubdivPartition { get; set; }

        public static RoutedUICommand DeleteSubdivPartition { get; set; }

        public static RoutedUICommand SaveIndividProtection { get; set; }
    }
}
