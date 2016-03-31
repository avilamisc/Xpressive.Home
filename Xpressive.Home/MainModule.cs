﻿using Autofac;
using Quartz.Spi;
using Xpressive.Home.Automation;
using Xpressive.Home.Contracts.Automation;
using Xpressive.Home.Contracts.Messaging;
using Xpressive.Home.Contracts.Variables;
using Xpressive.Home.Messaging;
using Xpressive.Home.Variables;

namespace Xpressive.Home
{
    internal class MainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageQueue>().As<IMessageQueue>().SingleInstance();
            builder.RegisterType<VariablePersistingService>().As<IVariablePersistingService>().SingleInstance();
            builder.RegisterType<RecurrentScriptJobFactory>().As<IJobFactory>();
            builder.RegisterType<ScheduledScriptRepository>().As<IScheduledScriptRepository>();
            builder.RegisterType<ScriptRepository>().As<IScriptRepository>();
            builder.RegisterType<ScriptEngine>().As<IScriptEngine>();
            builder.RegisterType<VariableScriptObjectProvider>().As<IScriptObjectProvider>();

            builder.RegisterType<VariableRepository>()
                .As<IVariableRepository>()
                .As<IMessageQueueListener<UpdateVariableMessage>>()
                .OnActivating(async e => await e.Instance.InitAsync())
                .SingleInstance();

            builder.RegisterType<CronService>()
                .As<ICronService>()
                .OnActivating(async e => await e.Instance.InitAsync());

            base.Load(builder);
        }
    }
}