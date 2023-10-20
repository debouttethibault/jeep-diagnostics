using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JeepDiag.WPF
{
    public partial class MainWindow : Window
    {
        private ICommunication _comms;

        public MainWindow()
        {
            _comms = new Communication();

            InitializeComponent();

            InitButtons();
        }

        private void InitButtons()
        {
            SetButtonsEnabled(false);

            BtnConnect.Click += BtnConnect_Click;
            BtnClear.Click += BtnClear_Click;

            //BtnReadData.Click += BtnReadData_Click;
            BtnReadDTC.Click += BtnReadDTC_Click;
            BtnResetDTC.Click += BtnResetDTC_Click;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtOutput.Text = string.Empty;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnabled(false);

            if (_comms.HasToShowSerialPortDialog())
            {
                var dialog = new SelectSerialPortDialog(Communication.GetSerialPortNames());
                dialog.ShowDialog();

                var selected = dialog.SelectedSerialPortName;
                if (!string.IsNullOrEmpty(selected) && selected.Equals(Communication.EmulatorPortName))
                    _comms = new CommunicationEmulator();
                else
                    _comms.SetSerialPortName(selected);
            }

            try
            {
                _comms.Connect();
                StatusCOM.Content = $"COM Port: {_comms.SerialPortName} {(_comms.IsSerialPortOpen ? "Open" : "Closed")}";
            } 
            catch (CommunicationException ex)
            {
                MessageBox.Show(ex.Message, "Communication error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var open = _comms.IsSerialPortOpen;
            SetButtonsEnabled(open);
            BtnConnect.IsEnabled = !open;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            //BtnReadData.IsEnabled = enabled;
            BtnReadDTC.IsEnabled = enabled;
            BtnResetDTC.IsEnabled = enabled;
        }

        private async void BtnResetDTC_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnabled(false);
            
            var status = await _comms.ResetDTCs();
            AppendOutput(LabelClearDTCs, status);

            SetButtonsEnabled(true);
        }

        private async void BtnReadDTC_Click(object sender, RoutedEventArgs e)
        {
            SetButtonsEnabled(false);
            
            var dtcs = await _comms.RequestStoredDTC();
            if (!dtcs.Any())
                AppendOutput(LabelStoredDTCs, "/");
            AppendOutput(LabelStoredDTCs, dtcs);

            dtcs = await _comms.RequestPendingDTC();
            if (!dtcs.Any())
                AppendOutput(LabelPendingDTCs, "/");
            AppendOutput(LabelPendingDTCs, dtcs);

            SetButtonsEnabled(true);
        }

        private const string LabelPendingDTCs = "Pending DTCs";
        private const string LabelStoredDTCs = "Stored DTCs";
        private const string LabelClearDTCs = "Clear DTCs";

        private void AppendOutput(string label, string output)
        {
            TxtOutput.Text += $"{label}:\n";
            TxtOutput.Text += $"  {output}\n";
            TxtOutput.Text += "\n";

            TxtOutput.ScrollToEnd();
        }

        private void AppendOutput(string label, ICollection<string> output)
        {
            TxtOutput.Text += $"{label}:\n";
            foreach (var item in output)
                TxtOutput.Text += $"  {item}\n";
            TxtOutput.Text += "\n";

            TxtOutput.ScrollToEnd();
        }

        //private async void BtnReadData_Click(object sender, RoutedEventArgs e)
        //{
        //    SetButtonsEnabled(false);
        //    SetButtonsEnabled(true);
        //}
    }
}
