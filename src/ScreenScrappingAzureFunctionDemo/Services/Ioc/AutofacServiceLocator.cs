using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CommonServiceLocator;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    /// <summary>
    ///     Autofac implementation of the Microsoft CommonServiceLocator.
    /// </summary>
    public class AutofacServiceLocator : IServiceLocator
    {
        /// <summary>
        ///     The <see cref="Autofac.IComponentContext" /> from which services
        ///     should be located.
        /// </summary>
        private readonly IComponentContext _container;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutofacServiceLocator" /> class.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="Autofac.IComponentContext" /> from which services
        ///     should be located.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if <paramref name="container" /> is <see langword="null" />.
        /// </exception>
        public AutofacServiceLocator(IComponentContext container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        ///     Implementation of <see cref="IServiceProvider.GetService" />.
        /// </summary>
        /// <param name="serviceType">The requested service.</param>
        /// <exception cref="ActivationException">if there is an error in resolving the service instance.</exception>
        /// <returns>The requested object.</returns>
        public virtual object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        ///     Get an instance of the given <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">
        ///     if there is an error resolving
        ///     the service instance.
        /// </exception>
        /// <returns>The requested service instance.</returns>
        public virtual object GetInstance(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        ///     Get an instance of the given named <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">
        ///     if there is an error resolving
        ///     the service instance.
        /// </exception>
        /// <returns>The requested service instance.</returns>
        public virtual object GetInstance(Type serviceType, string key)
        {
            try
            {
                return DoGetInstance(serviceType, key);
            }
            catch (Exception ex)
            {
                throw new ActivationException(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Get all instances of the given <paramref name="serviceType" /> currently
        ///     registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">
        ///     if there is are errors resolving
        ///     the service instance.
        /// </exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType" />.</returns>
        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            try
            {
                return DoGetAllInstances(serviceType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Get an instance of the given <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">
        ///     if there is are errors resolving
        ///     the service instance.
        /// </exception>
        /// <returns>The requested service instance.</returns>
        public virtual TService GetInstance<TService>()
        {
            return (TService) GetInstance(typeof(TService), null);
        }

        /// <summary>
        ///     Get an instance of the given named <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">
        ///     if there is are errors resolving
        ///     the service instance.
        /// </exception>
        /// <returns>The requested service instance.</returns>
        public virtual TService GetInstance<TService>(string key)
        {
            return (TService) GetInstance(typeof(TService), key);
        }

        /// <summary>
        ///     Get all instances of the given <typeparamref name="TService" /> currently
        ///     registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">
        ///     if there is are errors resolving
        ///     the service instance.
        /// </exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService" />.</returns>
        public virtual IEnumerable<TService> GetAllInstances<TService>()
        {
            foreach (var item in GetAllInstances(typeof(TService)))
            {
                yield return (TService) item;
            }
        }

        /// <summary>
        ///     Resolves the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be <see langword="null" />.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        protected object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            return key != null ? _container.ResolveNamed(key, serviceType) : _container.Resolve(serviceType);
        }

        /// <summary>
        ///     Resolves all requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        protected IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            var instance = _container.Resolve(enumerableType);
            return ((IEnumerable) instance).Cast<object>();
        }
    }
}
