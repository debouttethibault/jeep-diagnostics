using System;
using System.Windows;
using JeepDiag.WPF.ViewModels;

namespace JeepDiag.WPF;

public partial class MainWindow
{
    private MainViewModel ViewModel { get; }

    public MainWindow(MainViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;

        ViewModel.Navigator.NavigateTo<HomeViewModel>();
        
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Application.Current.Shutdown();
    }
}
