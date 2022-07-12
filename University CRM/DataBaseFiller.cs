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
using System.Windows;

namespace University_CRM
{
    internal class DataBaseFiller
    {

        DataBaseFiller() {}


        public static async Task FillDb()
        {
            Random random = new Random();
            List<string> courses = new List<string>() { "It", "Phil", "Math", "Sys", "Act" };
            DigitalOceanSpacesController doController = new DigitalOceanSpacesController();
            
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


                    await doController.UploadFileFromStream(ms, $"avatars/{firstName} {lastName}.jpg");
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

    internal class DigitalOceanSpacesController
    {

        private AmazonS3Config config;
        private AmazonS3Client awsClient;
        readonly string AccessKey = ConfigurationManager.AppSettings.Get("DoAccessKey");
        readonly string SecretKey = ConfigurationManager.AppSettings.Get("DoSecretKey");
        readonly string BucketName = ConfigurationManager.AppSettings.Get("DoBucketName");

        public DigitalOceanSpacesController()
        {
            config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = "https://fra1.digitaloceanspaces.com",

            };

            var credentials = new BasicAWSCredentials(AccessKey, SecretKey);
            awsClient = new AmazonS3Client(credentials, config);
        }



        public async Task<byte[]> GetFileInBytes(string fileName)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                var response = await awsClient.GetObjectAsync(BucketName, $"avatars/{fileName}.jpg");
                await response.ResponseStream.CopyToAsync(ms);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            var objectBytes = ms.ToArray();
            return objectBytes;
        }

        public async Task UploadFileFromStream(Stream response, string fileName)
        {
            var request = new PutObjectRequest()
            {
                BucketName = BucketName,
                InputStream = response,
                Key = fileName

            };

            try
            {
                await awsClient.PutObjectAsync(request);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }


        public async Task DeleteFileFromBucket(string fileName)
        {
            try
            {
                var response = await awsClient.DeleteObjectAsync(BucketName, @$"avatars/{fileName}.jpg");
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



    }
}
