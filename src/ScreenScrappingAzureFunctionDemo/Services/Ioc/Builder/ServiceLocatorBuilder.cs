using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using CommonServiceLocator;
using ScreenScrappingAzureFunctionDemo.Services.Extensions;
using ScreenScrappingAzureFunctionDemo.Services.Ioc.Extensions;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc.Builder
{
    /// <summary>
    ///     This represents the builder entity for service locator.
    /// </summary>
    public class ServiceLocatorBuilder : IServiceLocatorBuilder
    {
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceLocatorBuilder" /> class.
        /// </summary>
        public ServiceLocatorBuilder()
        {
            ContainerBuilder = new ContainerBuilder();
        }

        public IContainer Container { get; private set; }

        public ContainerBuilder ContainerBuilder { get; private set; }

        /// <summary>
        ///     Creates a service locator with the component registrations.
        ///     More info: http://docs.autofac.org/en/latest/integration/csl.html
        /// </summary>
        /// <returns>Returns the <see cref="IServiceLocator" />.</returns>
        public IServiceLocator Build()
        {
            // Perform registrations and build the container.
            Container = ContainerBuilder.Build();

            // Set the service locator to an AutofacServiceLocator.
            var csl = new AutofacServiceLocator(Container);

            // Set the service locator created
            CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => csl);

            return CommonServiceLocator.ServiceLocator.Current;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="delegate">The delegate.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        public IServiceLocatorBuilder RegisterAsInstancePerDependency<TImplementer, TService>(Func<IComponentContext, TImplementer> @delegate)
        {
            ContainerBuilder.RegisterAsInstancePerDependency<TImplementer, TService>(@delegate);
            return this;
        }

        /// <summary>
        ///     Registers a delegate as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <param name="delegate">The delegate.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        public IServiceLocatorBuilder RegisterAsSingleInstance<TImplementer, TService>(Func<IComponentContext, TImplementer> @delegate)
        {
            ContainerBuilder.RegisterAsSingleInstance<TImplementer, TService>(@delegate);
            return this;
        }

        /// <summary>
        ///     Registers a module.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="handler"><see cref="RegistrationHandler" /> instance.</param>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        public IServiceLocatorBuilder RegisterModule<TModule>(RegistrationHandler handler = null) where TModule : IModule, new()
        {
            ContainerBuilder.RegisterModule<TModule>();
            if (handler.IsNullOrDefault())
            {
                return this;
            }
            if (handler != null && handler.RegisterTypeAsInstancePerDependency.IsNullOrDefault())
            {
                return this;
            }
            handler?.RegisterTypeAsInstancePerDependency(ContainerBuilder);
            return this;
        }

        /// <summary>
        /// Scan all assemblies and registers all modules
        /// </summary>
        /// <returns></returns>
        public IServiceLocatorBuilder RegisterAssemblyModules()
        {
            var assembliesInAppDomain = AppDomain.CurrentDomain.GetAssemblies().OrderBy(assembly => assembly.FullName);
            ContainerBuilder.RegisterAssemblyModules(assembliesInAppDomain.ToArray());
            return this;
        }

        /// <summary>
        ///     Registers a type as a component where every dependent component gets a new instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        public IServiceLocatorBuilder RegisterTypeAsInstancePerDependency<TImplementer, TService>()
        {
            ContainerBuilder.RegisterTypeAsInstancePerDependency<TImplementer, TService>();
            return this;
        }

        /// <summary>
        ///     Registers a type as a component where every dependent component gets the same instance.
        /// </summary>
        /// <typeparam name="TImplementer">The type of the instance.</typeparam>
        /// <typeparam name="TService">The type of the component.</typeparam>
        /// <returns>Returns the <see cref="IServiceLocatorBuilder" /> instance.</returns>
        public IServiceLocatorBuilder RegisterTypeAsSingleInstance<TImplementer, TService>()
        {
            ContainerBuilder.RegisterTypeAsSingleInstance<TImplementer, TService>();
            return this;
        }
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value that determines whether</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                ReleaseManagedResources();
            }
            ReleaseUnmanagedResources();
            _disposed = true;
        }

        /// <summary>
        ///     Releases managed resources during the disposing event.
        /// </summary>
        protected virtual void ReleaseManagedResources()
        {
            // Release managed resources here.
        }

        /// <summary>
        ///     Releases unmanaged resources during the disposing event.
        /// </summary>
        protected virtual void ReleaseUnmanagedResources()
        {
            ContainerBuilder = null;
        }
    }
}
