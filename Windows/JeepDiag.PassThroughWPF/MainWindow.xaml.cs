using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace JeepDiag.PassThroughWPF
{
    public partial class MainWindow : Window
    {
        private readonly SerialPort _serialPort;

        private int _read = 0;
        private readonly byte[] _readBuffer = new byte[100];

        private readonly byte[] _writeBuffer = new byte[100];

        public MainWindow()
        {
            InitializeComponent();

            TxtOutput.Text = string.Empty;
            
            _serialPort = new SerialPort("COM9", 115200);
            _serialPort.DataReceived += SerialPort_DataReceived;

            OpenSerialPort();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _read = _serialPort.Read(_readBuffer, 0, _readBuffer.Length);
            Dispatcher.Invoke(() => PrintOutput());
        }

        private void PrintOutput()
        {
            TxtOutput.Text += Convert.ToHexString(_readBuffer, 0, _read);
            TxtOutput.Text += "\n";
        }

        private void OpenSerialPort()
        {
            try
            {
                _serialPort.Open();

                BtnSend.Click += BtnSend_Click;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid serial port name", "Serial error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed opening serial port", "Serial error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            var hex = TxtInput.Text.Split('-', StringSplitOptions.TrimEntries);

            int write = hex.Length;
            for (int i = 0 ; i < write; i++)
                _writeBuffer[i] = Convert.ToByte(hex[i], 16);

            _serialPort.Write(_writeBuffer, 0, write);

            TxtInput.Text = string.Empty;
        }
    }
}
