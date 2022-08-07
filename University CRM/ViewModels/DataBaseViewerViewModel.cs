using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LiveCharts;
using University_CRM.Infrastructure.Commands;
using University_CRM.Models;
using University_CRM.Services;

namespace University_CRM.ViewModels
{
    internal class DataBaseViewerViewModel : BaseVm
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILiveChartService _liveChart;
        private readonly IDigitalOceanService _doService;

        public ICommand WindowLoad { get; }
        public ICommand DeleteStudentCommand { get; }
        public ICommand DataGridRowSelected { get; }


        private ObservableCollection<StudentModel> _students;

        private CollectionView _studView;

        public string Title => $"University Base[{_studView?.Count.ToString()}] students";

        private List<string> _courses;

        public List<string> Courses
        {
            get { return _courses; }
            set { _courses = value; OnPropertyChanged(); }
        }

        public SeriesCollection PieCollection => _liveChart.DrawDonut();

        public CollectionView StudView
        {
            get => _studView;
            set
            {
                _studView = value; OnPropertyChanged();
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

        private int _courseIndex;

        public int CourseIndex
        {
            get { return _courseIndex; }
            set { _courseIndex = value; OnFilterChanged(); }
        }

        private StudentModel _selectedStudent;
        public StudentModel SelectedStudent
        {
            get
            {
                return _selectedStudent;
            }
            set
            {
                _selectedStudent = value; OnPropertyChanged();
            }
        }



        public DataBaseViewerViewModel(IStudentRepository studentRepository, ILiveChartService liveChart, IDigitalOceanService doService)
        {
            _studentRepository = studentRepository;
            _liveChart = liveChart;
            _doService = doService;

            WindowLoad = new Command(Load);
            DeleteStudentCommand = new Command(DeleteStudent);
            DataGridRowSelected = new Command(Row_Selected);
            

            _predicate = x =>
            {
                var item = (StudentModel)x;
                var lower = _filterText?.ToLower();

                return (CourseIndex == 0 || item.Course == _courses[_courseIndex]) && (string.IsNullOrEmpty(lower) || item.FirstName.ToLower().Contains(lower) ||
                       item.LastName.ToLower().Contains(lower));
            };



        }

        Predicate<object> _predicate;

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


        private void OnFilterChanged()
        {

            StudView.Filter = _predicate;

            OnPropertyChanged(nameof(Title));

        }


        public async void Load()
        {
            Students = new ObservableCollection<StudentModel>(await _studentRepository.OnLoad());
            Courses = Students.Select(x => x.Course).Distinct().ToList();
            Courses.Insert(0,"any");

            StudView = (CollectionView)CollectionViewSource.GetDefaultView(Students);
            OnPropertyChanged(nameof(Title));
        }

        public void AddToList(StudentModel student)
        {
            Students.Add(student);
            StudView.Refresh();

            if (!Courses.Contains(student.Course))
            {
                Courses.Add(student.Course);
            }

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(PieCollection));
        }

        public void DeleteStudent()
        {
            _studentRepository.DeleteStudentFromDb(SelectedStudent);
            Students.Remove(SelectedStudent);
            StudView.Refresh();
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(PieCollection));

        }



        #region StudentPopUp

        private bool _isOpened;

        public bool IsOpened
        {
            get { return _isOpened;; }
            set { _isOpened = value; OnPropertyChanged(); }
        }

        private BitmapImage _popImage;

        public BitmapImage PopImage
        {
            get => _popImage;
            set { _popImage = value; OnPropertyChanged(); }
        }

        private string _fullname;

        public string Fullname
        {
            get => _fullname;
            set { _fullname = value; OnPropertyChanged(); }
        }

        private string _course;
        public string Course
        {
            get => _course;
            set { _course = value; OnPropertyChanged(); }
        }


        private async void Row_Selected()
        {
            if (SelectedStudent != null)
            {
                PopImage = null;


                var firstName = SelectedStudent.FirstName;
                var lastName = SelectedStudent.LastName;
                var course = SelectedStudent.Course;



                Fullname = $"{firstName} {lastName}";
                Course = $"{course}";

                IsOpened = true;

                try
                {
                    PopImage = await _doService.GetBitmapImageFromDigitalOcean(firstName, lastName);
                }

                catch ( Exception ex)
                {
                    Console.WriteLine("Аватар не найден");
                }

            }
        }

        #endregion

    }
}
