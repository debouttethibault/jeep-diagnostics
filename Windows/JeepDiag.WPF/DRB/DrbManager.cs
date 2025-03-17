using System.Collections.Generic;
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

        public Task<string> ResetDtcAsync()
        {
            var data = _communication.SendRequest(new [] { Drb.Commands.ClearDtcs });
            return Task.FromResult(Drb.Dtc.DecodeClearDtcResponse(data));
        }

        public Task<ICollection<string>> RequestStoredDtcsAsync()
        {
            var data = _communication.SendRequest(new []{ Drb.Commands.StoredDtcs });
            return Task.FromResult(Drb.Dtc.DecodeStoredDtcResponse(data));
        }
        
        public Task<ICollection<string>> RequestPendingDtcsAsync()
        {
            var data = _communication.SendRequest(new []{ Drb.Commands.PendingDtcs });
            return Task.FromResult(Drb.Dtc.DecodePendingDtcResponse(data));
        }
    }
}
