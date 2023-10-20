using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeepDiag.WPF
{
    internal class CommunicationEmulator : ICommunication
    {
        private bool _open;

        public bool IsSerialPortOpen => _open;

        public string? SerialPortName { get; private set; } = "Emulator";

        public void Connect()
        {
            _open = true;
        }

        public void Disconnect()
        {
            _open = false;
        }

        public bool HasToShowSerialPortDialog()
        {
            return false;
        }

        public Task<List<string>> RequestStoredDTC()
        {
            CheckOpen();

            return Task.FromResult(
                TakeRandomDTCs(2).ToList()
            );
        }

        public Task<List<string>> RequestPendingDTC()
        {
            CheckOpen();

            return Task.FromResult(
                TakeRandomDTCs(2).ToList()
            );
        }

        public Task<string> ResetDTCs()
        {
            CheckOpen();

            return Task.FromResult("SUCCESS");
        }

        public Task<byte[]> SendRequest(byte[] data)
        {
            CheckOpen();

            return Task.FromResult(data);
        }

        private void CheckOpen()
        {
            if (!_open)
                throw new InvalidOperationException("Serial port not open");
        }

        public void SetSerialPortName(string? serialPortName)
        {
            SerialPortName = serialPortName;
        }

        public static IEnumerable<string> TakeRandomDTCs(int count) => Communication.DTCs
                .Select(x => x.Value)
                .ToList()
                .Shuffle()
                .Take(count);
    }
}
