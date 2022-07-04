using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using University_CRM.Models;

namespace University_CRM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DataBaseViewer : Window
    {
        private BindingList<StudentModel> StudentsList = new BindingList<StudentModel>();

        public DataBaseViewer()
        {
            
            //DataBaseFiller.FillDb();

            InitializeComponent();

            RefreshGrid();
            StudentsList.ListChanged += StudentsListListChanged;
            Pie.DataContext = LiveChartPainter.DrawDonut();

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

                    break;
                case ListChangedType.ItemDeleted:


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
            DigitalOceanSpacesController doController = new DigitalOceanSpacesController();

            var selectedRow = (FrameworkElement)e.Source;
            var rowDataContext = (StudentModel)selectedRow.DataContext;

            var firstName = rowDataContext.FirstName;
            var lastName = rowDataContext.LastName;
            var course = rowDataContext.Course;


            
            PopupFullName.Text = $"{firstName} {lastName}";
            PopupCourse.Text = $"{course}";

            Popup.IsOpen = true;

            var file = await doController.GetFileInBytes($"{firstName} {lastName}");
            BitmapImage image = new BitmapImage();
            using MemoryStream imagestream = new MemoryStream(file);
            image.BeginInit();
            image.StreamSource = imagestream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
               

            PopupImage.Source = image;

            
        }


        private void Row_MouseLeave(object sender, MouseEventArgs e)
        {
           Popup.IsOpen = false;
           
        }
    }


}
