using Autofac;
using ScreenScrappingAzureFunctionDemo.Services.Ioc;
using ScreenScrappingAzureFunctionDemo.Services.Modules;

namespace ScreenScrappingAzureFunctionDemo.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        public Module[] CreateModules()
        {
            return new Module[]
            {
                new ServicesModule()
            };
        }
    }
}