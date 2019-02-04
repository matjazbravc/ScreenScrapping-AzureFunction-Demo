using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    internal static class BootstrapperCollector
    {
        public static List<Type> GetBootstrappers()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IBootstrapper).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
        }
    }
}