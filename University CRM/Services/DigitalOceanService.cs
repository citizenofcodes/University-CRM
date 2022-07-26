using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;



namespace University_CRM.Services
{
    public interface IDigitalOceanService
    {
        Task<byte[]> GetFileInBytes(string fileName);
        Task UploadFileFromStream(Stream response, string fileName);
        Task DeleteFileFromBucket(string fileName);
    }

    internal class DigitalOceanService : IDigitalOceanService
    {
        private AmazonS3Config config;
        private AmazonS3Client awsClient;
        readonly string AccessKey = ConfigurationManager.AppSettings.Get("DoAccessKey");
        readonly string SecretKey = ConfigurationManager.AppSettings.Get("DoSecretKey");
        readonly string BucketName = ConfigurationManager.AppSettings.Get("DoBucketName");

        public DigitalOceanService()
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
