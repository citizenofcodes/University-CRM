using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.IO;
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

        private BindingList<StudentModel> _studentsList = new BindingList<StudentModel>();

        public DataBaseViewer()
        {

            //DataBaseFiller.FillDb();
          
            InitializeComponent();

            RefreshGrid();
            _studentsList.ListChanged += StudentsListListChanged;
            Pie.DataContext = LiveChartPainter.DrawDonut();

        }


        private void RefreshGrid()
        {

            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "SELECT  id,FirstName,LastName,Course FROM students";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                _studentsList.Add(new StudentModel()
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



                GridViews.ItemsSource = _studentsList;
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
                    StudentModel.UpdateStudent(e, _studentsList);

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
            StudentModel.StudentsFilter(NameFilter.Text, _studentsList, CoursesBox, GridViews);
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

            var rowindex = (e.Source as DataGridRow).GetIndex();
            var FirstName = _studentsList[rowindex].FirstName;
            var LastName = _studentsList[rowindex].LastName;
            var Course = _studentsList[rowindex].Course;


            Popup.IsOpen = true;
            PopupFullName.Text = $"{FirstName} {LastName}";
            PopupCourse.Text = $"{Course}";




            var file = await doController.GetFileInBytes($"{FirstName} {LastName}");
            BitmapImage image = new BitmapImage();
            using (MemoryStream imagestream = new MemoryStream(file))
            {
                image.BeginInit();
                image.StreamSource = imagestream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
               

                PopupImage.Source = image;
            }

        }


        private void Row_MouseLeave(object sender, MouseEventArgs e)
        {
           Popup.IsOpen = false;
           
        }
    }


}
