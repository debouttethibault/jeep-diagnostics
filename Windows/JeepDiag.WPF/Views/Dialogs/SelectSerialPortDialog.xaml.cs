using JeepDiag.WPF.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace JeepDiag.WPF.Views.Dialogs;

public partial class SelectSerialPortDialog : FluentWindow
{
    public SelectSerialPortViewModel ViewModel { get; }

    public SelectSerialPortDialog(SelectSerialPortViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();

        BtnSelect.Click += BtnSelect_Click;
        BtnLoad.Click += BtnLoad_Click;
        LstPorts.SelectionChanged += LstPorts_SelectionChanged;
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        LoadSerialPorts();
    }


    private void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        LoadSerialPorts();
    }

    private void LoadSerialPorts()
    {
        BtnLoad.IsEnabled = false;
        BtnSelect.IsEnabled = false;
        LstPorts.IsEnabled = false;
        LblLoading.Visibility = Visibility.Visible;

        Task.Run(() =>
        {
            ViewModel.SerialPortNames = ViewModel.SerialPortSelector.Invoke();

            Dispatcher.Invoke(() =>
            {
                LstPorts.ItemsSource = ViewModel.SerialPortNames;
                LstPorts.SelectedIndex = -1;
                LstPorts.IsEnabled = true;
                BtnLoad.IsEnabled = true;

                LblLoading.Visibility = Visibility.Hidden;
            });
        });
    }

    private void BtnSelect_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ViewModel.SelectedSerialPortName))
            Close();
    }

    private void LstPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        BtnSelect.IsEnabled = LstPorts.SelectedIndex != -1;

        var selectedItem = LstPorts.SelectedItem as KeyValuePair<string, string>?;
        ViewModel.SelectedSerialPortName = selectedItem?.Key;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        DialogResult = !string.IsNullOrEmpty(ViewModel.SelectedSerialPortName);
    }
}
