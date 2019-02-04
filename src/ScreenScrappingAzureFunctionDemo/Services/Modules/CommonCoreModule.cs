using System.Net.Http.Formatting;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ScreenScrappingAzureFunctionDemo.Services.Formatters;
using ScreenScrappingAzureFunctionDemo.Services.Ioc.Extensions;

namespace ScreenScrappingAzureFunctionDemo.Services.Modules
{
    public class CommonCoreModule : Module
    {
        /// <inheritdoc />
        /// <summary>
        ///     Add registrations to the container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter() },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            builder.RegisterAsSingleInstance<JsonSerializerSettings, JsonSerializerSettings>(_ => serializerSettings);

            // Register Formatters
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter
            {
                UseDataContractJsonSerializer = true
            };
            builder.RegisterAsSingleInstance<JsonMediaTypeFormatter, JsonMediaTypeFormatter>(_ => jsonMediaTypeFormatter);

            var yamlMediaTypeFormatter = new YamlMediaTypeFormatter();
            builder.RegisterAsSingleInstance<YamlMediaTypeFormatter, YamlMediaTypeFormatter>(_ => yamlMediaTypeFormatter);

            builder.RegisterType<HttpRequestMessageHelper>();
        }
    }
}