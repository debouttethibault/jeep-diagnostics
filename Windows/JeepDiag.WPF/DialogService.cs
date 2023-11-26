using JeepDiag.WPF.ViewModels.Dialogs;
using JeepDiag.WPF.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace JeepDiag.WPF
{
    public class DialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private T GetService<T>() where T: class => _serviceProvider.GetRequiredService<T>();

        public async void ShowMessageDialog(string message, string title, MessageDialogType type = MessageDialogType.None)
        {
            await ShowMessageDialogAsync(message, title, type);
        }

        public async Task<Wpf.Ui.Controls.MessageBoxResult> ShowMessageDialogAsync(string message, string title, MessageDialogType type = MessageDialogType.None)
        {
            var mb = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = message,
            };
            return await mb.ShowDialogAsync();
        }

        public string? ShowSelectSerialPortDialog(Func<IDictionary<string, string>> func)
        {
            var vm = GetService<SelectSerialPortViewModel>();
            vm.SerialPortSelector = func;

            var dialog = GetService<SelectSerialPortDialog>();
            dialog.ShowDialog();

            return vm.SelectedSerialPortName;
        }

        public enum MessageDialogType 
        { 
            None,
            Information,
            Warning,
            Error
        }
    }
}
