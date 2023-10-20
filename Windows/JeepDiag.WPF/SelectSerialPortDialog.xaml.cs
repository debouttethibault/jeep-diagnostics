using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JeepDiag.WPF
{
    public partial class SelectSerialPortDialog : Window
    {
        public string? SelectedSerialPortName { get; private set; }
        private readonly IDictionary<string, string> _serialPortNames;

        public SelectSerialPortDialog(IDictionary<string, string> serialPortNames)
        {
            _serialPortNames = serialPortNames;

            InitializeComponent();

            BtnSelect.IsEnabled = false;
            BtnSelect.Click += BtnSelect_Click;

            LstPorts.SelectionChanged += LstPorts_SelectionChanged;
            LstPorts.ItemsSource = _serialPortNames;
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedSerialPortName))
                Close();
        }

        private void LstPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnSelect.IsEnabled = LstPorts.SelectedIndex != -1;

            var selectedItem = LstPorts.SelectedItem as KeyValuePair<string, string>?;
            SelectedSerialPortName = selectedItem?.Key;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = !string.IsNullOrEmpty(SelectedSerialPortName);
        }
    }
}
