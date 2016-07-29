﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Xpressive.Home.Contracts.Gateway;
using Xpressive.Home.Contracts.Messaging;

namespace Xpressive.Home.Services
{
    internal sealed class LowBatteryDeviceObserver : IStartable
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IList<IGateway> _gateways;

        public LowBatteryDeviceObserver(IMessageQueue messageQueue, IEnumerable<IGateway> gateways)
        {
            _messageQueue = messageQueue;
            _gateways = gateways.ToList();
        }

        public void Start()
        {
            Task.Run(Observe);
        }

        private async Task Observe()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));

                foreach (var gateway in _gateways)
                {
                    foreach (var device in gateway.Devices)
                    {
                        if (device.BatteryStatus == DeviceBatteryStatus.Low)
                        {
                            _messageQueue.Publish(new LowBatteryMessage(gateway.Name, device));
                        }
                    }
                }
            }
        }
    }
}