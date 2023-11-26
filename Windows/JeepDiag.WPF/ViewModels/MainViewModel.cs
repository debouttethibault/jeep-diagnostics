using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeepDiag.WPF.DRB;
using JeepDiag.WPF.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeepDiag.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DialogService _dialogService;

    private readonly Communication _communication;
    private readonly DrbManager _drbManager;

    public MainViewModel(DialogService dialogService)
    {
        _communication = new Communication();
        _drbManager = new DrbManager(_communication);
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void OnOpenSerialPort() 
    {
        if (_communication.HasToShowSerialPortDialog())
        {
            var selectedPortName = _dialogService.ShowSelectSerialPortDialog(() => Communication.GetSerialPortNames());
            _communication.SetSerialPortName(selectedPortName);
        }

        try
        {
            _communication.Connect();
        } 
        catch (CommunicationException ex)
        {
            _dialogService.ShowMessageDialog(ex.Message, "Communication error", DialogService.MessageDialogType.Error);
        }

        IsConnected = _communication.IsSerialPortOpen;
        UpdateStatusMessage();
    }

    private void UpdateStatusMessage()
    {
        StatusMessage = $"{(string.IsNullOrEmpty(_communication.SerialPortName) ? "" : $"{_communication.SerialPortName} ")}{(_communication.IsSerialPortOpen ? "Opened" : "Closed")}";
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    private bool _isConnected = false;
    public bool IsNotConnected => !IsConnected;

    [ObservableProperty]
    private string _applicationTitle = "Jeep Diagnostics";

    [ObservableProperty]
    private string _statusMessage = string.Empty;
}
