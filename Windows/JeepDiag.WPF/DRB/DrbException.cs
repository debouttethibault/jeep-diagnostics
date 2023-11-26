using System;

namespace JeepDiag.WPF.DRB
{
    public class DrbException : ApplicationException
    {
        public DrbException(string? message) : base(message)
        {
        }

        public DrbException(string? message, Exception innerException) : base(message, innerException)
        {
        }

        public DrbException(string? message, byte[] drbData) : base(message)
        {
            DrbData = drbData;
        }

        public byte[]? DrbData { get; }
    }
}
