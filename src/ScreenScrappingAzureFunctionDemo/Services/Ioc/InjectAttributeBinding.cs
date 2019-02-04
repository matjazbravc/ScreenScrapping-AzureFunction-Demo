using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public class InjectAttributeBinding : IBinding
    {
        private readonly ParameterInfo _parameterInfo;

        public InjectAttributeBinding(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<IValueProvider>(new InjectAttributeValueProvider(_parameterInfo));
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            return Task.FromResult<IValueProvider>(new InjectAttributeValueProvider(_parameterInfo));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameterInfo.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "Inject services",
                    DefaultValue = "Inject services",
                    Prompt = "Inject services"
                }
            };
        }
    }
}