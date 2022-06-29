using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace University_CRM
{
    internal static class LiveChartPainter
    {
        public static SeriesCollection DrawDonut()
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
            List<string> courList = new List<string>();
            MySqlCommand cmd = new MySqlCommand("", DB.GetConnection());
            cmd.CommandText = @"SELECT Course, COUNT(*) FROM students GROUP BY Course";
            var reader = cmd.ExecuteReader();
            int i = 0;
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