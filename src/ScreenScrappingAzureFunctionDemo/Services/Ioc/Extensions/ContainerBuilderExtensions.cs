using System;
using Autofac;
using ScreenScrappingAzureFunctionDemo.Services.Extensions;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc.Extensions
{
    /// <summary>
    ///     This represents the extension entity for the <see cref="ContainerBuilder" /> class.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder" /> instance.</param>
        /// <param name="delegate">The function.</param>
        public static void RegisterAsInstancePerDependency<TImplementer, TService>(this ContainerBuilder containerBuilder, Func<IComponentContext, TImplementer> @delegate)
        {
            containerBuilder.ThrowIfNullOrDefault();
            @delegate.ThrowIfNullOrDefault();
            containerBuilder.Register(@delegate).As<TService>().InstancePerDependency();
        }

        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder" /> instance.</param>
        /// <param name="delegate">The function.</param>
        public static void RegisterAsSingleInstance<TImplementer, TService>(this ContainerBuilder containerBuilder, Func<IComponentContext, TImplementer> @delegate)
        {
            containerBuilder.ThrowIfNullOrDefault();
            @delegate.ThrowIfNullOrDefault();
            containerBuilder.Register(@delegate).As<TService>().SingleInstance();
        }

        /// <summary>
        ///     Registers a type as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder" /> instance.</param>
        public static void RegisterTypeAsInstancePerDependency<TImplementer, TService>(this ContainerBuilder containerBuilder)
        {
            containerBuilder.ThrowIfNullOrDefault();
            containerBuilder.RegisterType<TImplementer>().As<TService>().InstancePerDependency();
        }

        /// <summary>
        ///     Registers a type as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder" /> instance.</param>
        public static void RegisterTypeAsSingleInstance<TImplementer, TService>(this ContainerBuilder containerBuilder)
        {
            containerBuilder.ThrowIfNullOrDefault();
            containerBuilder.RegisterType<TImplementer>().As<TService>().SingleInstance();
        }
    }
}
