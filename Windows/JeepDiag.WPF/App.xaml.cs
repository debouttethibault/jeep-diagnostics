using JeepDiag.WPF;
using JeepDiag.WPF.Navigation;
using JeepDiag.WPF.ViewModels;
using JeepDiag.WPF.ViewModels.Dialogs;
using JeepDiag.WPF.Views;
using JeepDiag.WPF.Views.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using JeepDiag.WPF.DRB;

namespace JeepDiag.WPF
{
    public partial class App
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;
        public IConfiguration Configuration { get; private set; } = null!;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)!)
                .AddJsonFile("appsettings.json", false);
            Configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services, Configuration);
            ServiceProvider = services.BuildServiceProvider();

            ServiceProvider.GetRequiredService<MainWindow>().Show();
        }

        private static void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Communication>();
            services.AddSingleton<DrbManager>();
            services.AddSingleton<CommunicationStatus>();

            services.AddSingleton<DialogService>();

            services.AddTransient<SelectSerialPortViewModel>();
            services.AddTransient<SelectSerialPortDialog>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<INavigator, Navigator>();

            services.AddSingleton<HomeViewModel>();
            services.AddTransient<HomeView>();

            services.AddSingleton<DtcViewModel>();
            services.AddTransient<DtcView>();

            services.AddSingleton<DatabaseViewModel>();
            services.AddTransient<DatabaseView>();
        }
    }
}
