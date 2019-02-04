using Autofac;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using ScreenScrappingAzureFunctionDemo.Services.Logging;
using ScreenScrappingAzureFunctionDemo.Services.Logging.Serilog.Sinks;

namespace ScreenScrappingAzureFunctionDemo.Services.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var loggingStorageTableName = CloudConfigurationManager.GetSetting("Logging.Storage.TableName");
            var storageConnectingString = CloudConfigurationManager.GetSetting("StorageAccount.ConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectingString);
            builder.Register(c => new SerilogToAzureTableStorage(nameof(ServicesModule), storageAccount, loggingStorageTableName)).As<ILog>();
            builder.RegisterType<ScreenScrappingService>().As<IScreenScrappingService>();
        }
    }
}
