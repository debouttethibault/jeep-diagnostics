using CommunityToolkit.Mvvm.ComponentModel;
using JeepDiag.WPF.DRB;

namespace JeepDiag.WPF;

public partial class CommunicationStatus : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    [NotifyPropertyChangedFor(nameof(StatusMessage))]
    private bool _isConnected;
    public bool IsNotConnected => !IsConnected;
    public string StatusMessage => $"{(string.IsNullOrEmpty(_communication.SerialPortName) ? "" : $"{_communication.SerialPortName} ")}{(_communication.IsSerialPortOpen ? "Opened" : "Closed")}";

    private readonly Communication _communication;
    
    public CommunicationStatus(Communication communication)
    {
        _communication = communication;
    }
}