using System.Windows;
using System.Windows.Controls;
using University_CRM.Models;

namespace University_CRM
{
    /// <summary>
    /// Логика взаимодействия для AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        private DataGrid GridViews;
        public AddStudentWindow(DataGrid gridViews)
        {
            InitializeComponent();
            this.GridViews = gridViews;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StudentModel.AddStudent(FirstName.Text, LastName.Text, Course.Text, GridViews);
            MessageBox.Show("Success!");
            this.Close();
            
            
        }
    }
}
