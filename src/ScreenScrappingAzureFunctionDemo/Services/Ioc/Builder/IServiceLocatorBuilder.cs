using System;
using Autofac;
using Autofac.Core;
using CommonServiceLocator;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc.Builder
{
    /// <summary>
    ///     This provides interfaces to the <see cref="ServiceLocatorBuilder" /> class.
    /// </summary>
    public interface IServiceLocatorBuilder : IDisposable
    {
        /// <summary>
        ///     Builds the service locator.
        /// </summary>
        /// <returns>Returns the <see cref="IServiceLocator" />.</returns>
        IServiceLocator Build();

        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="delegate">The delegate.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        IServiceLocatorBuilder RegisterAsInstancePerDependency<TImplementer, TService>(Func<IComponentContext, TImplementer> @delegate);

        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="delegate">The delegate.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        IServiceLocatorBuilder RegisterAsSingleInstance<TImplementer, TService>(Func<IComponentContext, TImplementer> @delegate);

        /// <summary>
        ///     Registers a type as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        IServiceLocatorBuilder RegisterTypeAsInstancePerDependency<TImplementer, TService>();

        /// <summary>
        ///     Registers a type as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        IServiceLocatorBuilder RegisterTypeAsSingleInstance<TImplementer, TService>();

        /// <summary>
        ///     Registers a module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="handler"><see cref="RegistrationHandler" /> instance.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        IServiceLocatorBuilder RegisterModule<TModule>(RegistrationHandler handler = null) where TModule : IModule, new();
    }
}
