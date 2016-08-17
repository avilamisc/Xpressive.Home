using System;
using System.Collections.Generic;
using System.Linq;
using Xpressive.Home.Contracts.Automation;

namespace Xpressive.Home.Plugins.MyStrom
{
    internal sealed class MyStromScriptObjectProvider : IScriptObjectProvider
    {
        private readonly IMyStromGateway _gateway;

        public MyStromScriptObjectProvider(IMyStromGateway gateway)
        {
            _gateway = gateway;
        }

        public IEnumerable<Tuple<string, object>> GetObjects()
        {
            yield break;
        }

        public IEnumerable<Tuple<string, Delegate>> GetDelegates()
        {
            // mystrom("id")
            // mystrom("id").on();

            var deviceResolver = new Func<string, MyStromScriptObject>(id =>
            {
                var device = _gateway.GetDevices().SingleOrDefault(d => d.Id.Equals(id));
                return device != null ? new MyStromScriptObject(device, _gateway) : null;
            });

            yield return new Tuple<string, Delegate>("mystrom", deviceResolver);
        }

        public class MyStromScriptObject
        {
            private readonly IMyStromGateway _gateway;
            private readonly MyStromDevice _device;

            public MyStromScriptObject(MyStromDevice device, IMyStromGateway gateway)
            {
                _device = device;
                _gateway = gateway;
            }

            public void on()
            {
                _gateway.SwitchOn(_device);
            }

            public void off()
            {
                _gateway.SwitchOff(_device);
            }
        }
    }
}