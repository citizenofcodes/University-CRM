using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using University_CRM.Services;
using University_CRM.ViewModels;

namespace University_CRM
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<DataBaseViewer>();
                    services.AddSingleton<AddStudentWindowViewModel>();
                    services.AddSingleton<DataBaseViewerViewModel>();
                    services.AddSingleton<ILiveChartService,LiveChartService>();
                    services.AddSingleton<IDigitalOceanService, DigitalOceanService>();
                    services.AddSingleton<IStudentRepository, StudentRepository>();



                })
                .Build();
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<DataBaseViewer>();
            startupForm.Show();

            base.OnStartup(e);

        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StartAsync();
            base.OnExit(e);
        }
    }
}
