using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
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

        public ICommand OpenImageCommand { get; set; }

        public MemoryStream ImageStream { get; set; }
        
        public AddStudentWindowViewModel(IStudentRepository studentRepository)
        {
            ImageStream = new MemoryStream(File.ReadAllBytes(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Resources\anonim.jpg"));
            _studentRepository = studentRepository;
            FirstName = "First Name";
            LastName = "Last Name";
            Course = "Course";
            AddStudent = new Command(Add);
            OpenImageCommand = new Command(OpenImage);
        }

        private void OpenImage(object obj)
        {
            
            var openFileDlg = new System.Windows.Forms.OpenFileDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                ImageStream = new MemoryStream(File.ReadAllBytes(openFileDlg.FileName));
            }
        }

        public void Add()
        {

            var student = new StudentModel() { FirstName = FirstName, LastName = LastName, Course = Course };

            _studentRepository.AddStudent(student, ImageStream);
            App.AppHost.Services.GetRequiredService<DataBaseViewerViewModel>().AddToList(student);

        }
    }
}
