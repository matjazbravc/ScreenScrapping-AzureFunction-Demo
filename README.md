# Screen Scrapping Azure Function Demo

This application demonstrate following functionalities:
- usage of Azure functions,
- dependency injection with [Autofac](https://autofac.org/) (inspired by [Holger Leichsenring Blog Post](http://codingsoul.de/2018/01/19/azure-function-dependency-injection-with-autofac/)),
- logging with [Serilog](https://serilog.net/) sink to Azure Table storage,
- Screen scrapping with [Html Agility Pack](http://html-agility-pack.net/)

## Prerequisites
- [Visual Studio](https://www.visualstudio.com/vs/community) 2017 15.5.5 or greater
- Azure Storage Emulator (the Storage Emulator is available as part of the [Microsoft Azure SDK](https://azure.microsoft.com/en-us/downloads/))

To create and deploy functions, you also need:
- An active Azure subscription. If you don't have an Azure subscription, [free accounts](https://azure.microsoft.com/en-us/free) are available.
- An Azure Storage account. To create a storage account, see [Create a storage account](https://docs.microsoft.com/en-us/azure/storage/common/storage-create-storage-account#create-a-storage-account).

To get started with Azure Functions, you can visit [Microsoft Azure Functions portal](https://azure.microsoft.com/en-us/services/functions/).

## Let's get started!
First, you have to enter your Azure storage account connection string into **_local.settings.json_**:
```json
{
  "IsEncrypted": false,
  "Values": {
    "Logging.Storage.TableName": "LogAzureFunctions",
    "StorageAccount.ConnectionString": "UseDevelopmentStorage=true",
    "Url.Address": "http://dontcodetired.com/blog/archive"
  }
}
```
P.S. More about how to test Azure functions locally you can read [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local).

Second, if you use development storage you have to setup Azure Storage Emulator. See [Use the Azure storage emulator for development and testing](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) and [Configuring and Using the Storage Emulator with Visual Studio](https://docs.microsoft.com/en-us/azure/vs-azure-tools-storage-emulator-using).

Third, build solution and run it with local Azure Functions runtime: 
![](https://github.com/mabravc/ScreenScrappingAzureFunctionDemo/blob/master/res/function_local_runtime.jpg)

Finally test it with [Postman](https://getpostman.com):
![](https://github.com/mabravc/ScreenScrappingAzureFunctionDemo/blob/master/res/postman_function_test.jpg)

Enjoy!

## Licence

Licenced under [MIT](http://opensource.org/licenses/mit-license.php).
Developed by [Matjaž Bravc](https://si.linkedin.com/in/matjazbravc)
