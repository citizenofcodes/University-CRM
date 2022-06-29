using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace University_CRM.Models
{
    internal class StudentModel : INotifyPropertyChanged

    {
        
        public int Id { get; set; }
        private string _firstName;





        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName == value)
                    return;
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName == value)
                    return;
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string _course;

        public string Course
        {
            get { return _course; }
            set
            {
                if (_course == value)
                    return;
                _course = value;
                OnPropertyChanged("Course");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }


        public static void AddStudent(string firstName, string lastName, string course, DataGrid gridViews)
        {
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "INSERT INTO `students`(`FirstName`,`LastName`,`Course`) VALUES (@FirstName, @LastName, @Course)";
            cmd.Parameters.AddWithValue("@FirstName", firstName);
            cmd.Parameters.AddWithValue("@LastName", lastName);
            cmd.Parameters.AddWithValue("@Course", course);
            cmd.ExecuteNonQuery();
            var source = gridViews.GetValue(ItemsControl.ItemsSourceProperty) as BindingList<StudentModel>;
            source.Add(new StudentModel() { FirstName = firstName, LastName = lastName, Course = course });




        }

        public static void DeleteStudentFromDb(object sender, DataGrid gridViews)
        {
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            var element = sender as FrameworkElement;
            var studentrow = element.DataContext as StudentModel;
            cmd.CommandText = "DELETE FROM `students` WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", studentrow.Id);
            cmd.ExecuteNonQuery();
            var source = gridViews.GetValue(ItemsControl.ItemsSourceProperty) as BindingList<StudentModel>;
            source.Remove(studentrow);




        }

        public static void StudentsFilter(string filter, BindingList<StudentModel> studentsList, ComboBox coursesBox, DataGrid gridViews)
        {

            IEnumerable<StudentModel> filteredList = studentsList;
            if (filter != "")
            {
                filteredList = filteredList.Where(x => x.FirstName.Contains(filter) || x.LastName.Contains(filter));
            }

            if (coursesBox.SelectedIndex != 0)
            {
                filteredList = filteredList.Where(x => x.Course.Contains(coursesBox.Text));

            }

             filteredList = new BindingList<StudentModel>(filteredList.ToList());
            gridViews.ItemsSource = filteredList;

        }

        public static void UpdateStudent(ListChangedEventArgs e , BindingList<StudentModel> studentsList)
        {
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = $"UPDATE `students` SET `FirstName`=@FirstName,`LastName`=@LastName,`Course`=@Course WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", studentsList[e.NewIndex].Id);
            cmd.Parameters.AddWithValue("@FirstName", studentsList[e.NewIndex].FirstName);
            cmd.Parameters.AddWithValue("@LastName", studentsList[e.NewIndex].LastName);
            cmd.Parameters.AddWithValue("@Course", studentsList[e.NewIndex].Course);
            cmd.ExecuteNonQuery();
        }
    }
}
