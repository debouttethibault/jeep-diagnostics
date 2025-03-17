using JeepDiag.WPF.ViewModels.Dialogs;
using JeepDiag.WPF.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using JeepDiag.WPF.DRB;

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

        public string? ShowSelectSerialPortDialog(Func<IDictionary<string, string>> func)
        {
            var vm = GetService<SelectSerialPortViewModel>();
            vm.SetSerialPortSelector(func);

            var dialog = GetService<SelectSerialPortDialog>();
            dialog.ShowDialog();

            return vm.SelectedSerialPortName;
        }

        public bool ShowClearDtcDialog()
        {
            var vm = GetService<ClearDtcViewModel>();
            
            var dialog = GetService<SelectSerialPortDialog>();
            dialog.ShowDialog();

            return vm.IsSuccess;
        }
    }
}
