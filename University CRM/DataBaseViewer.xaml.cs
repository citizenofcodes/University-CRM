using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using University_CRM.Models;
using University_CRM.Services;
using University_CRM.ViewModels;

namespace University_CRM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DataBaseViewer : Window
    {
        public DataBaseViewer()
        {

            InitializeComponent();
            DataContext = App.AppHost.Services.GetRequiredService<DataBaseViewerViewModel>();
        }


        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addStudent = new AddStudentWindow();
            addStudent.DataContext = App.AppHost.Services.GetRequiredService<AddStudentWindowViewModel>();
            addStudent.ShowDialog();

        }


        private void ChangeTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var style = ((ComboBoxItem)e.AddedItems[0]).Content;

            if (style != null)
            {
                var uri = new Uri("Themes\\" + style + "Theme.xaml", UriKind.Relative);

                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

                Application.Current.Resources.Clear();

                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }
        }
    }


}
