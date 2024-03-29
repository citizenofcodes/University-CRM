﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using University_CRM.Models;

namespace University_CRM.Services
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentModel>> OnLoad();
        void AddStudent(StudentModel student, MemoryStream imageStream);
        void DeleteStudentFromDb(StudentModel student);
        void UpdateStudent(ListChangedEventArgs e, BindingList<StudentModel> studentsList);
    }

    internal class StudentRepository : IStudentRepository
    {
        public async Task<IEnumerable<StudentModel>> OnLoad()
        {
            List<StudentModel> studentsList = new List<StudentModel>();
            using (var conn = await DB.GetAsyncConnection())
            {
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = "SELECT  id,FirstName,LastName,Course FROM students";
                var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {

                    studentsList.Add(new StudentModel()
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Course = reader.GetString("Course")
                    });

                }

                reader.Close();

                return studentsList;

            }
        }

        public void AddStudent(StudentModel student , MemoryStream imageStream)
        {
            var st = student;
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "INSERT INTO `students`(`FirstName`,`LastName`,`Course`) VALUES (@FirstName, @LastName, @Course)";
            cmd.Parameters.AddWithValue("@FirstName", st.FirstName);
            cmd.Parameters.AddWithValue("@LastName", st.LastName);
            cmd.Parameters.AddWithValue("@Course", st.Course);
            cmd.ExecuteNonQuery();

            App.AppHost.Services.GetRequiredService<IDigitalOceanService>()
                .UploadFileFromStream(imageStream, $"avatars/{st.FirstName} {st.LastName}.jpg");
        }

        public void DeleteStudentFromDb(StudentModel student)
        {
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = "DELETE FROM `students` WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", student.Id);
            cmd.ExecuteNonQuery();


            //_doService.DeleteFileFromBucket($"{studentrow.FirstName} {studentrow.LastName}");





        }

        public void UpdateStudent(ListChangedEventArgs e, BindingList<StudentModel> studentsList)
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
