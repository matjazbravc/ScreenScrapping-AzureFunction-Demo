using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    internal class InjectAttributeValueProvider : IValueProvider
    {
        private readonly ParameterInfo _parameterInfo;

        public InjectAttributeValueProvider(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo;
        }

        public Type Type => _parameterInfo.ParameterType;

        public object GetValue()
        {
            return ServiceLocator.GetInstance(Type);
        }

        public Task<object> GetValueAsync()
        {
            return Task.FromResult(ServiceLocator.GetInstance(Type));
        }

        public string ToInvokeString()
        {
            return Type.ToString();
        }
    }
}