using JeepDiag.WPF.ViewModels.Dialogs;
using System;
using System.ComponentModel;
using System.Threading;
using JeepDiag.WPF.DRB;

namespace JeepDiag.WPF.Views.Dialogs;

public partial class SelectSerialPortDialog 
{
    private SelectSerialPortViewModel ViewModel { get; }

    public SelectSerialPortDialog(SelectSerialPortViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        viewModel.SetSerialPortSelector(() => Communication.GetSerialPortNames(true));
        
        InitializeComponent();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        ViewModel.LoadSerialPorts().WaitAsync(CancellationToken.None);
        ViewModel.Close += Close;
    }

    private void Close(object? sender, EventArgs args)
    {
        Close();
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        ViewModel.Close -= Close;
        DialogResult = !string.IsNullOrEmpty(ViewModel.SelectedSerialPortName);
        ViewModel.SerialPortNames = null;
    }
}
