using JeepDiag.WPF;
using JeepDiag.WPF.ViewModels;
using JeepDiag.WPF.ViewModels.Dialogs;
using JeepDiag.WPF.Views;
using JeepDiag.WPF.Views.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Jeepdiag.WPF
{
    public partial class App : Application
    {
        private static readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            })
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<DialogService>();

                services.AddSingleton<SelectSerialPortViewModel>();
                services.AddSingleton<SelectSerialPortDialog>();

                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        public static T? GetService<T>() where T : class 
            => _host.Services.GetService<T>();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await _host.StartAsync();

            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _host.Dispose();
        }
    }
}
