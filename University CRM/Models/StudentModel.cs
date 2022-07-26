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

        
    }
}
