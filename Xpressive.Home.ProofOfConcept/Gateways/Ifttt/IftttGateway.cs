using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xpressive.Home.ProofOfConcept.Contracts;

namespace Xpressive.Home.ProofOfConcept
{
    internal class IftttGateway : GatewayBase
    {
        public IftttGateway() : base("IFTTT")
        {
            _actions.Add(new Action("Web request")
            {
                Fields = { "Event Name" }
            });
        }

        public override bool IsConfigurationValid()
        {
            throw new NotImplementedException();
        }

        protected override async Task ExecuteInternal(DeviceBase device, IAction action, IDictionary<string, string> values)
        {
            var key = ((IftttDevice)device).Key;
            var eventName = values["Event Name"];
            var url = string.Format("https://maker.ifttt.com/trigger/{1}/with/key/{0}", key, eventName);
            await new HttpClient().PostAsync(url, null);
        }
    }
}