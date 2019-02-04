using System;
using Microsoft.Azure.WebJobs.Description;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class InjectAttribute : Attribute
    {
    }
}
