using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JeepDiag.WPF.ViewModels.Dialogs
{
    public partial class SelectSerialPortViewModel : ObservableObject
    {
        public event EventHandler? Close; 
        
        [ObservableProperty] private IDictionary<string, string>? _serialPortNames;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LoadingVisibility))]
        [NotifyPropertyChangedFor(nameof(IsSelectEnabled))]
        private bool _isLoading;
        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Hidden;
        public bool IsSelectEnabled => SelectedSerialPortId is > 0;
        
        [ObservableProperty] private int? _selectedSerialPortId;

        public string? SelectedSerialPortName => SelectedSerialPortId is > 0 && SerialPortNames != null
                                                    ? SerialPortNames.Values.ElementAt(SelectedSerialPortId.Value) : null;
        
        private Func<IDictionary<string, string>> _serialPortSelector = () => new Dictionary<string, string>();
        
        [RelayCommand]
        public async Task LoadSerialPorts()
        {
            IsLoading = true;
            await Task.Run(() =>
            {
                SerialPortNames = _serialPortSelector.Invoke();
            });
            IsLoading = false;
        }

        [RelayCommand]
        private void SelectSerialPort()
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetSerialPortSelector(Func<IDictionary<string, string>> selector) =>
            _serialPortSelector = selector;
    }
}
