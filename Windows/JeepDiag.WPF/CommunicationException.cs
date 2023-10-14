using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeepDiag.WPF
{
    public class CommunicationException : ApplicationException
    {
        public CommunicationException(string message) : base(message)
        { }
    }
}
