using System.Threading.Tasks;
using System.Windows;

namespace JeepDiag.WPF
{
    public partial class MainWindow : Window
    {
        private readonly Communication _comms;

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

            BtnReadData.Click += BtnReadData_Click;
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
            BtnReadData.IsEnabled = enabled;
            BtnReadDTC.IsEnabled = enabled;
            BtnResetDTC.IsEnabled = enabled;
        }

        private void BtnResetDTC_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        private void BtnReadDTC_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        private void BtnReadData_Click(object sender, RoutedEventArgs e)
        {
            SendRequest();
        }

        public async void SendRequest()
        {
            SetButtonsEnabled(false);
        
            await Task.Delay(1000);

            SetButtonsEnabled(true);
        }
    }
}
