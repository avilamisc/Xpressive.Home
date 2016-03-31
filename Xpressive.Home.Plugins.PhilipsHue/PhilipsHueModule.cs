﻿using Autofac;
using Xpressive.Home.Contracts.Gateway;

namespace Xpressive.Home.Plugins.PhilipsHue
{
    public class PhilipsHueModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PhilipsHueGateway>().As<IGateway>().SingleInstance();

            builder.RegisterType<PhilipsHueDeviceDiscoveringService>()
                .As<IPhilipsHueDeviceDiscoveringService>()
                .SingleInstance();

            builder.RegisterType<PhilipsHueBridgeDiscoveringService>()
                .As<IPhilipsHueBridgeDiscoveringService>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}