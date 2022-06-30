using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;

namespace University_CRM
{
    internal class DataBaseFiller
    {

        public static async Task FillDb()
        {
           

            using HttpClient client = new HttpClient();

            for (int i = 0; i < 10; i++)
            {
                var response = await client.GetByteArrayAsync(new Uri("https://thispersondoesnotexist.com/image"));

                
                File.WriteAllBytes($@"C:\Users\Stanislav\source\repos\University CRM\University CRM\avatar{i}.jpg",
                    response);

                Thread.Sleep(1000);

            }

            //MySqlCommand command = new MySqlCommand(U);




        }
    }
}
