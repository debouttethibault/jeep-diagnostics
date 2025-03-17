using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeepDiag.WPF.DRB;
using JeepDiag.WPF.Navigation;
using System.Windows;

namespace JeepDiag.WPF.ViewModels;

public partial class MainViewModel : ObservableObject, IViewModel
{
    public CommunicationStatus CommunicationStatus { get; }
    
    public INavigator Navigator { get; }

    private readonly DialogService _dialogService;

    private readonly Communication _communication;
    private readonly DrbManager _drbManager;

    public MainViewModel(INavigator navigator, DialogService dialogService, Communication communication, DrbManager drbManager, CommunicationStatus communicationStatus)
    {
        Navigator = navigator;

        _dialogService = dialogService;

        _communication = communication;
        _drbManager = drbManager;
        
        CommunicationStatus = communicationStatus;
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
            MessageBox.Show(ex.Message, "Communication error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        CommunicationStatus.IsConnected = _communication.IsSerialPortOpen;
    }

    [RelayCommand]
    private void NavigateToDatabaseView() => Navigator.NavigateTo<DatabaseViewModel>();
    
}
