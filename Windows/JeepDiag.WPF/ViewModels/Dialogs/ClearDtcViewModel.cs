using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeepDiag.WPF.DRB;

namespace JeepDiag.WPF.ViewModels.Dialogs;

public partial class ClearDtcViewModel : ObservableObject
{
    public event EventHandler? Close;
    
    [ObservableProperty]
    private bool _isSuccess;
    
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _response;
    
    private readonly DrbManager _drbManager;

    public ClearDtcViewModel(DrbManager drbManager)
    {
        _drbManager = drbManager;
    }

    [RelayCommand]
    private async Task ClearDtcAsync()
    {
        Response = await _drbManager.ResetDtcAsync();
        IsSuccess = Response.Equals("SUCCESS");

        if (IsSuccess)
        {
            MessageBox.Show(Response, "Clear DTC Result", MessageBoxButton.OK, MessageBoxImage.None);
            Close?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (MessageBox.Show(Response, "Clear DTC Result", MessageBoxButton.YesNo, MessageBoxImage.None) == MessageBoxResult.No);
                Close?.Invoke(this, EventArgs.Empty);
        }
    }
}