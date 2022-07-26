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
    public partial class DataBaseViewer : Window, INotifyPropertyChanged

    {
        private readonly IDigitalOceanService _doService;
        private readonly IStudentRepository _studentRepository;
        public BindingList<StudentModel> StudentsList = new BindingList<StudentModel>();


        public string StudentsCounter => $"University Base[{StudentsList.Count.ToString()}] students";

        public DataBaseViewer(IDigitalOceanService doService, IStudentRepository studentRepository )
        {
            _doService = doService;
            _studentRepository = studentRepository;

            InitializeComponent();

            StudentsList.ListChanged += StudentsListListChanged;
            Pie.DataContext = LiveChartPainter.DrawDonut(); ;
            DataContext = App.AppHost.Services.GetRequiredService<DataBaseViewerViewModel>();
        }



       


        private void StudentsListListChanged(object sender, ListChangedEventArgs e)
        {


            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    break;
                case ListChangedType.ItemAdded:
                    OnPropertyChanged(nameof(StudentsCounter));
                    Pie.DataContext = LiveChartPainter.DrawDonut();
                    break;
                case ListChangedType.ItemDeleted:
                    Pie.DataContext = LiveChartPainter.DrawDonut();
                    OnPropertyChanged(nameof(StudentsCounter));

                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.ItemChanged:
                    _studentRepository.UpdateStudent(e, StudentsList);

                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
            }
        }



        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addStudent = new AddStudentWindow();
            addStudent.ShowDialog();




        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _studentRepository.DeleteStudentFromDb(sender, GridViews);

        }

        private async void Row_Selected(object sender, RoutedEventArgs e)
        {
            PopupImage.Source = null;


            var selectedRow = (FrameworkElement)e.Source;
            var rowDataContext = (StudentModel)selectedRow.DataContext;

            var firstName = rowDataContext.FirstName;
            var lastName = rowDataContext.LastName;
            var course = rowDataContext.Course;



            PopupFullName.Text = $"{firstName} {lastName}";
            PopupCourse.Text = $"{course}";

            Popup.IsOpen = true;


            PopupImage.Source = await GetBitmapImageFromDigitalOcean();

            async Task<BitmapImage> GetBitmapImageFromDigitalOcean()
            {
                
                var file = await _doService.GetFileInBytes($"{firstName} {lastName}");
                BitmapImage image = new BitmapImage();
                using MemoryStream imageStream = new MemoryStream(file);
                image.BeginInit();
                image.StreamSource = imageStream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }


        private void Row_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
