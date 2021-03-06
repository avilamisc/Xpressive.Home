using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Xpressive.Home.Contracts.Messaging;

namespace Xpressive.Home.Contracts.Gateway
{
    public abstract class GatewayBase : IGateway, IMessageQueueListener<CommandMessage>
    {
        private readonly ILog _log;
        private readonly string _name;
        protected readonly ConcurrentBag<DeviceBase> _devices;
        protected bool _canCreateDevices;

        protected GatewayBase(string name)
        {
            _log = LogManager.GetLogger(GetType());
            _name = name;
            _devices = new ConcurrentBag<DeviceBase>();
        }

        public string Name => _name;
        public IEnumerable<IDevice> Devices => _devices.ToList();
        public bool CanCreateDevices => _canCreateDevices;
        
        public IDevicePersistingService PersistingService { get; set; }

        public bool AddDevice(IDevice device)
        {
            var d = device as DeviceBase;
            return AddDeviceInternal(d);
        }

        public abstract IEnumerable<IAction> GetActions(IDevice device);

        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract IDevice CreateEmptyDevice();

        public void Notify(CommandMessage message)
        {
            if (!message.ActionId.StartsWith(_name, StringComparison.Ordinal))
            {
                return;
            }

            var parts = message.ActionId.Split('.');

            if (parts.Length != 3)
            {
                return;
            }

            var deviceId = parts[1];
            var actionName = parts[2];
            var device = Devices.SingleOrDefault(d => d.Id.Equals(deviceId, StringComparison.Ordinal));

            if (device == null)
            {
                return;
            }

            var action = GetActions(device).SingleOrDefault(a => a.Name.Equals(actionName, StringComparison.Ordinal));

            if (action == null)
            {
                return;
            }

            StartActionInNewTask(device, action, message.Parameters);
        }

        protected async Task LoadDevicesAsync(Func<string, string, DeviceBase> emptyDevice)
        {
            try
            {
                var devices = await PersistingService.GetAsync(Name, emptyDevice);

                foreach (var device in devices)
                {
                    _devices.Add(device);
                }
            }
            catch (Exception e)
            {
                _log.Error("Unable to load persisted devices.", e);
            }
        }

        protected void StartActionInNewTask(IDevice device, IAction action, IDictionary<string, string> values)
        {
            Task.Factory.StartNew(async () => await ExecuteInternalAsync(device, action, values));
        }

        protected virtual bool AddDeviceInternal(DeviceBase device)
        {
            if (!_canCreateDevices || device == null || !device.IsConfigurationValid())
            {
                return false;
            }

            _devices.Add(device);
            PersistingService.SaveAsync(Name, device);
            return true;
        }

        protected abstract Task ExecuteInternalAsync(IDevice device, IAction action, IDictionary<string, string> values);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) { }

        ~GatewayBase()
        {
            Dispose(false);
        }
    }
}
