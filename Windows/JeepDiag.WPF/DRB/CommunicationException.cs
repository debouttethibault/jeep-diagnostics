using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeepDiag.WPF.DRB
{
    public class CommunicationException : ApplicationException
    {
        public CommunicationException(string message) : base(message)
        { }

        public CommunicationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
