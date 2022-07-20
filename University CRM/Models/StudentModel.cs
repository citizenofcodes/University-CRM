using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using University_CRM.ViewModels;

namespace University_CRM.Models
{
    public class StudentModel : BaseVm

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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public static async Task<IEnumerable<StudentModel>> OnLoad(List<string> courses)
        {
            List<StudentModel> studentsList = new List<StudentModel>();
            using (var conn = await DB.GetAsyncConnection())
            {
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = "SELECT  id,FirstName,LastName,Course FROM students";
                var reader = (MySqlDataReader) await cmd.ExecuteReaderAsync();
                courses.Add("any");
                while (reader.Read())
                {

                    studentsList.Add(new StudentModel()
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Course = reader.GetString("Course")
                    });

                    if (!courses.Contains(reader.GetString("Course")))
                    {
                        courses.Add(reader.GetString("Course"));
                    }

                }

                reader.Close();

                return studentsList;

            }
        }

        public static void AddStudent(StudentModel student)
        {
            var st = student;
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "INSERT INTO `students`(`FirstName`,`LastName`,`Course`) VALUES (@FirstName, @LastName, @Course)";
            cmd.Parameters.AddWithValue("@FirstName", st.FirstName);
            cmd.Parameters.AddWithValue("@LastName", st.LastName);
            cmd.Parameters.AddWithValue("@Course", st.Course);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteStudentFromDb(object sender, DataGrid gridViews)
        {
            DigitalOceanSpacesController doController = new DigitalOceanSpacesController();
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            var element = sender as FrameworkElement;
            var studentrow = element.DataContext as StudentModel;
            cmd.CommandText = "DELETE FROM `students` WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", studentrow.Id);
            cmd.ExecuteNonQuery();
            var source = gridViews.GetValue(ItemsControl.ItemsSourceProperty) as BindingList<StudentModel>;
            source.Remove(studentrow);


            doController.DeleteFileFromBucket($"{studentrow.FirstName} {studentrow.LastName}");

            

          

        }

        public static void UpdateStudent(ListChangedEventArgs e, BindingList<StudentModel> studentsList)
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
