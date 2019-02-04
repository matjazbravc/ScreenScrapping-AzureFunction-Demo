﻿using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public class InjectAttributeExtensionConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            InitializeServiceLocator(context);
            context.Config.RegisterBindingExtensions(new InjectAttributeBindingProvider());
        }

        private static void InitializeServiceLocator(ExtensionConfigContext context)
        {
            var bootstrappers = BootstrapperCollector.GetBootstrappers();
            if (bootstrappers.Count == 0)
            {
                context.Trace.Warning("No bootstrapper instances had been recognized, injection will not function.");
            }
            var modules = new List<Module>();
            foreach (var bootstrapper in bootstrappers)
            {
                var instance = (IBootstrapper) Activator.CreateInstance(bootstrapper);
                modules.AddRange(instance.CreateModules());
            }
            InjectConfiguration.Initialize(modules);
        }
    }
}