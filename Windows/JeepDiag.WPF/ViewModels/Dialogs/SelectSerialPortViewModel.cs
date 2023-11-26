using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeepDiag.WPF.ViewModels.Dialogs
{
    public class SelectSerialPortViewModel
    {
        public string? SelectedSerialPortName { get; set; }
        
        public IDictionary<string, string>? SerialPortNames { get; set; } 

        public Func<IDictionary<string, string>> SerialPortSelector { get; set; } = () => new Dictionary<string, string>();
    }
}
