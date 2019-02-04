using System;
using Microsoft.Extensions.DependencyInjection;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public static class ServiceLocatorCore
    {
        private static IServiceProvider _serviceProvider;

        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    throw new Exception("ServiceLocatorCore has to be initialized before usage.");
                }
                return _serviceProvider;
            }
            set => _serviceProvider = value;
        }

        public static T Resolve<T>()
        {
            if (_serviceProvider == null)
            {
                throw new Exception("ServiceLocator has to be initialized before usage.");
            }
            var service = _serviceProvider.GetService<T>();
            if (service == null)
            {
                throw new Exception($"Type {typeof(T).FullName} is not registered.");
            }
            return service;
        }
    }
}