using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace JeepDiag.WPF
{
    public class Communication
    {
        public string? SerialPortName { get; private set; }
        public bool IsSerialPortOpen => _serialPort.IsOpen;

        private readonly SerialPort _serialPort;

        public Communication()
        {
            _serialPort = new SerialPort();
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(SerialPortName))
                throw new CommunicationException("Serial port name not set");

            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.PortName = SerialPortName;
            _serialPort.BaudRate = 115200;

            try
            {
                _serialPort.Open();
            }
            catch (ArgumentException)
            {
                SerialPortName = null;
                throw new CommunicationException("Invalid serial port name");
            }
            catch (IOException)
            {
                SerialPortName = null;
                throw new CommunicationException("Failed opening the serial port");
            }
        }

        public void Disconnect()
        {
            _serialPort.Close();
        }

        public static string[] GetSerialPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public bool HasToShowSerialPortDialog()
        {
            if (string.IsNullOrEmpty(SerialPortName))
                return true;
            if (!GetSerialPortNames().Contains(SerialPortName))
                return true;
            return false;
        }

        public void SetSerialPortName(string? serialPortName)
        {
            SerialPortName = serialPortName;
        }

        public Task<byte[]> SendRequestData(byte[] data)
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }
}
