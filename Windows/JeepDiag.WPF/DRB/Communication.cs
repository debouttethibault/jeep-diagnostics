using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace JeepDiag.WPF.DRB
{
    public class Communication
    {
        public string? SerialPortName { get; private set; }
        public bool IsSerialPortOpen => _serialPort.IsOpen;

        private readonly SerialPort _serialPort;

        private readonly byte[] _readBuffer = new byte[100];

        private readonly SemaphoreSlim _requestSemaphore;

        public Communication()
        {
            _requestSemaphore = new SemaphoreSlim(1);

            _serialPort = new SerialPort();
            _serialPort.DataReceived += DataReceived;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _requestSemaphore.Release();
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
            catch (ArgumentException ex)
            {
                SerialPortName = null;
                throw new CommunicationException("Invalid serial port name", ex);
            }
            catch (IOException ex)
            {
                SerialPortName = null;
                throw new CommunicationException("Failed opening the serial port", ex);
            }
        }

        public async Task<byte[]> SendRequest(byte[] data)
        {
            _serialPort.Write(data, 0, data.Length);

            if (await _requestSemaphore.WaitAsync(2000))
                throw new CommunicationException("Timeout waiting for serial port response");

            int read;
            try
            {
                read = _serialPort.Read(_readBuffer, 0, _readBuffer.Length);
            }
            catch (IOException ex)
            {
                throw new CommunicationException("Failed reading from serial port", ex);
            }

            if (read == 0)
                return Array.Empty<byte>();

            return _readBuffer[0..read];
        }

        public void Disconnect()
        {
            _serialPort.Close();
        }

        public static IDictionary<string, string> GetSerialPortNames(bool includeName = true)
        {
            IList<ManagementObject>? portObjects = null;
            if (includeName)
            {
                portObjects = new ManagementObjectSearcher("Select * from Win32_SerialPort").Get()
                    .Cast<ManagementObject>().ToList();
            }

            var ports = new Dictionary<string, string>();
            foreach (var portName in SerialPort.GetPortNames())
            {
                string name = portName;
                if (portObjects != null)
                {
                    name = portObjects.Where(po => portName.Equals(po["DeviceID"].ToString(), StringComparison.OrdinalIgnoreCase))
                        .Select(po => po["Name"].ToString())
                        .SingleOrDefault() ?? portName;
                }
                ports.Add(portName, name);
            }
            return ports;
        }

        public bool HasToShowSerialPortDialog()
        {
            return string.IsNullOrEmpty(SerialPortName) || !GetSerialPortNames(includeName: false).ContainsKey(SerialPortName);
        }

        public void SetSerialPortName(string? serialPortName)
        {
            SerialPortName = serialPortName;
        }

    }
}
