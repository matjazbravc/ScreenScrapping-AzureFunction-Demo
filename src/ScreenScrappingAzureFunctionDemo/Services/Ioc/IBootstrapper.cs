using Autofac;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public interface IBootstrapper
    {
        Module[] CreateModules();
    }
}