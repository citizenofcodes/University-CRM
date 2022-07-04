using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using University_CRM.Models;

namespace University_CRM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DataBaseViewer : Window, INotifyPropertyChanged

    {
        public BindingList<StudentModel> StudentsList = new BindingList<StudentModel>();


        public string StudentsCounter => $"University Base[{StudentsList.Count.ToString()}] students";

        public DataBaseViewer()
        {

            //DataBaseFiller.FillDb();


            InitializeComponent();

            RefreshGrid();
            StudentsList.ListChanged += StudentsListListChanged;
            Pie.DataContext =  LiveChartPainter.DrawDonut(); ;
            DataContext = this;
        }



        private void RefreshGrid()
        {

            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "SELECT  id,FirstName,LastName,Course FROM students";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                StudentsList.Add(new StudentModel()
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Course = reader.GetString("Course")
                });

                if (!CoursesBox.Items.Contains(reader.GetString("Course")))
                {
                    CoursesBox.Items.Add(reader.GetString("Course"));
                }



                GridViews.ItemsSource = StudentsList;
            }

            reader.Close();


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
                    StudentModel.UpdateStudent(e, StudentsList);

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
            StudentModel.StudentsFilter(NameFilter.Text, StudentsList, CoursesBox, GridViews);
        }

        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addStudent = new AddStudentWindow(GridViews);
            addStudent.ShowDialog();




        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            StudentModel.DeleteStudentFromDb(sender, GridViews);

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
                DigitalOceanSpacesController doController = new DigitalOceanSpacesController();
                var file = await doController.GetFileInBytes($"{firstName} {lastName}");
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
    }


}
