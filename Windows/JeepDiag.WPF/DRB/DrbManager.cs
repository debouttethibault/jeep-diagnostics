using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeepDiag.WPF.DRB
{
    public class DrbManager
    {
        private readonly Communication _communication;

        public DrbManager(Communication communication)
        {
            _communication = communication;
        }
    }
}
