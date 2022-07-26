using System;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;

namespace University_CRM.Services
{
    internal interface ILiveChartService
    {
        SeriesCollection DrawDonut();
    }

    internal  class LiveChartService : ILiveChartService
    {
        public SeriesCollection DrawDonut()
        {
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);


            var series = new SeriesCollection
            {

            };
            AddNewPieSeries(series);
            return series;
        }

        public static void AddNewPieSeries(SeriesCollection series)
        {
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = @"SELECT Course, COUNT(*) FROM students GROUP BY Course";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                series.Add(new PieSeries
                {
                    Title = reader.GetString("Course"),
                    Values = new ChartValues<int> { reader.GetInt32("COUNT(*)") },
                    DataLabels = true,


                });

            }
        }

    }
}