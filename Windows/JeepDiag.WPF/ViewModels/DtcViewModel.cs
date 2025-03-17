using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JeepDiag.WPF.DRB;

namespace JeepDiag.WPF.ViewModels;

public partial class DtcViewModel : ObservableObject, INavigatableViewModel
{
    public enum DtcCommand
    {
        Pending,
        Stored,
        Clear
    }
    public DtcCommand? Command { get; set; }
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private ICollection<string>? _dtcs;

    private readonly CancellationTokenSource _cts = new();
    private readonly DialogService _dialogService;
    private readonly DrbManager _drbManager;
    
    public DtcViewModel(DialogService dialogService, DrbManager drbManager)
    {
        _dialogService = dialogService;
        _drbManager = drbManager;
    }
    
    public void OnNavigate()
    {
        if (Command.HasValue)
            LoadDtcs(Command.Value).WaitAsync(_cts.Token);
    }

    public void OnNavigateAway()
    {
        _cts.Cancel();
    }

    [RelayCommand]
    private async Task LoadDtcs(DtcCommand cmd)
    {
        if (IsLoading)
            return;
        IsLoading = true;

        switch (cmd)
        {
            case DtcCommand.Stored:
                Dtcs = await _drbManager.RequestStoredDtcsAsync();
                break;
            case DtcCommand.Pending:
                Dtcs = await _drbManager.RequestPendingDtcsAsync();
                break;
            case DtcCommand.Clear:
                _dialogService.ShowClearDtcDialog();
                break;
        }

        IsLoading = false;
    }
}