using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using University_CRM.Services;

namespace University_CRM
{
    internal class DataBaseFiller
    {
        private static readonly IDigitalOceanService _doService;

        static DataBaseFiller()
        {
            _doService = App.AppHost.Services.GetRequiredService<IDigitalOceanService>();
        }

        public static async Task FillDb()
        {
            Random random = new Random();
            List<string> courses = new List<string>() { "It", "Phil", "Math", "Sys", "Act" };

            using HttpClient client = new HttpClient();


            int counter = 1;

            for (int b = 0; b < counter; b++)
            {
                var findFirstNameLastNamePattern = @"name_heading.>(.+?) (.+?)<";

                var nameGeneratorPage = await client.GetStringAsync(new Uri("https://www.name-generator.org.uk/quick/"));
                var generatedNamesCollection = Regex.Matches(nameGeneratorPage, findFirstNameLastNamePattern);

                for (int i = 0; i < generatedNamesCollection.Count; i++)
                {
                    string firstName = generatedNamesCollection[i].Groups[1].Value;
                    string lastName = generatedNamesCollection[i].Groups[2].Value;


                    var response = await client.GetByteArrayAsync(new Uri("https://thispersondoesnotexist.com/image"));
                    MemoryStream ms = new MemoryStream(response);


                    await _doService.UploadFileFromStream(ms, $"avatars/{firstName} {lastName}.jpg");
                    var command = new MySqlCommand($"INSERT INTO `students`(" +
                                                   $"`FirstName`," +
                                                   $"`LastName`," +
                                                   $"`Course`," +
                                                   $" `StudentAvatar`) " +
                                                   $"VALUES (" +
                                                   $"'{firstName}'," +
                                                   $" '{lastName.Replace("'", "''")}'," +
                                                   $" '{courses[random.Next(courses.Count)]}'," +
                                                   $" '{firstName} {lastName.Replace("'", "''")}.jpg')", DB.GetConnection());

                    try
                    {
                        command.ExecuteNonQuery();
                    }

                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }

                Thread.Sleep(1000);

            }
            

        }
    }
}
