using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using University_CRM.Infrastructure.Commands;
using University_CRM.Models;
using University_CRM.Services;

namespace University_CRM.ViewModels
{
    internal class DataBaseViewerViewModel : BaseVm
    {
        private readonly IStudentRepository _studentRepository;
        public ICommand WindowLoad { get; }


        private ObservableCollection<StudentModel> _students;

        private CollectionView studView;

        public string Title => $"University Base[{StudentsCounter}] students";

        private string[] _courses;

        public string[] Courses
        {
            get { return _courses; }
            set { _courses = value; OnPropertyChanged(); }
        }


        public string StudentsCounter => studView?.Count.ToString();


        public CollectionView StudView
        {
            get => studView;
            set
            {
                studView = value; OnPropertyChanged();
            }
        }


        public ObservableCollection<StudentModel> Students
        {
            get => _students;
            set
            {
                _students = value; OnPropertyChanged();
            }
        }

        private int courseIndex;

        public int CourseIndex
        {
            get { return courseIndex; }
            set { courseIndex = value; OnFilterChanged(); }
        }



        public DataBaseViewerViewModel(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;

            WindowLoad = new Command(Load);

            _predicate = x =>
            {
                var item = (StudentModel)x;
                var lower = _filterText?.ToLower();

                return (CourseIndex == 0 || item.Course == _courses[courseIndex]) && (string.IsNullOrEmpty(lower) || item.FirstName.ToLower().Contains(lower) ||
                       item.LastName.ToLower().Contains(lower));
            };



        }


        private string _filterText;

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                _filterText = value;
                OnFilterChanged();
            }
        }


        Predicate<object> _predicate;



        private void OnFilterChanged()
        {

            StudView.Filter = _predicate;

            OnPropertyChanged(nameof(Title));

        }


        public async void Load()
        {
            List<string> courses = new List<string>();
            Students = new ObservableCollection<StudentModel>(await _studentRepository.OnLoad(courses));
            Courses = courses.ToArray();
            StudView = (CollectionView)CollectionViewSource.GetDefaultView(Students);
            OnPropertyChanged(nameof(Title));
        }

        public void AddToList(StudentModel student)
        {
            Students.Add(student);
            StudView = (CollectionView)CollectionViewSource.GetDefaultView(Students);
            OnPropertyChanged(nameof(Title));
        }


    }
}
