using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public static class InjectJobHostConfigurationExtensions
    {
        public static void UseDependencyInjection(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            var extensionConfig = new InjectAttributeExtensionConfigProvider();
            var extensions = config.GetService<IExtensionRegistry>();
            extensions.RegisterExtension<IExtensionConfigProvider>(extensionConfig);
        }
    }
}