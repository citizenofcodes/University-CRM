using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using University_CRM.Infrastructure.Commands;
using University_CRM.Models;
using University_CRM.Services;

namespace University_CRM.ViewModels
{
    public class AddStudentWindowViewModel:BaseVm
    {
        private readonly IStudentRepository _studentRepository;
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Course { get; set; }
        public ICommand AddStudent { get; }
        
        public AddStudentWindowViewModel(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
            FirstName = "First Name";
            LastName = "Last Name";
            Course = "Course";
            AddStudent = new Command(Add);
        }

        public void Add()
        {

            var student = new StudentModel() { FirstName = FirstName, LastName = LastName, Course = Course };

            _studentRepository.AddStudent(student);
            //AddToList(student);
        }
    }
}
