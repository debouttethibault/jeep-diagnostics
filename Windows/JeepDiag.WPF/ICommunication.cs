using System.Collections.Generic;
using System.Threading.Tasks;

namespace JeepDiag.WPF
{
    public interface ICommunication
    {
        bool IsSerialPortOpen { get; }
        string? SerialPortName { get; }

        void Connect();
        void Disconnect();
        bool HasToShowSerialPortDialog();
        Task<List<string>> RequestPendingDTC();
        Task<List<string>> RequestStoredDTC();
        Task<string> ResetDTCs();
        Task<byte[]> SendRequest(byte[] data);
        void SetSerialPortName(string? serialPortName);
    }
}