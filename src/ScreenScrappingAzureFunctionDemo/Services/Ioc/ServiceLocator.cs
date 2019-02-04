using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using CommonServiceLocator;
using ScreenScrappingAzureFunctionDemo.Services.Ioc.Builder;
using ScreenScrappingAzureFunctionDemo.Services.Logging;

namespace ScreenScrappingAzureFunctionDemo.Services.Ioc
{
    public static class ServiceLocator
    {
        private static readonly ConcurrentDictionary<object[], WeakReference> _cache = new ConcurrentDictionary<object[], WeakReference>();
        private static ILifetimeScope _currentLifetimeScope;

        public static IContainer Container { get; set; }

        public static IServiceLocator Instance { get; set; }

        public static void BeginLifetimeScope()
        {
            TryDisposeLifetimeScope();
            _currentLifetimeScope = Container.BeginLifetimeScope();
        }

        public static void Dispose()
        {
            TryDisposeLifetimeScope();
        }
        
        /// <summary>
        /// Get all instances of the given <paramref name="serviceType" /> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType" />.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Instance.GetAllInstances(serviceType);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService" /> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService" />.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<TService> GetAllInstances<TService>()
        {
            return Instance.GetAllInstances<TService>();
        }

        [DebuggerStepThrough]
        public static object GetInstance(Type serviceType)
        {
            return Resolve(serviceType);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <returns>The requested service instance.</returns>
        [DebuggerStepThrough]
        public static object GetInstance(Type serviceType, string key)
        {
            return Instance.GetInstance(serviceType, key);
        }

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <returns>The requested service instance.</returns>
        [DebuggerStepThrough]
        public static TService GetInstance<TService>()
        {
            return Resolve<TService>();
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <returns>The requested service instance.</returns>
        [DebuggerStepThrough]
        public static TService GetInstance<TService>(string key) where TService : class
        {
            return ResolveWithCache<TService>(key);
        }

        public static ILog GetLogger()
        {
            return Resolve<ILog>();
        }

        public static void Initialize()
        {
            using (var builder = new ServiceLocatorBuilder())
            {
                builder.RegisterAssemblyModules();
                Instance = builder.Build();
                Container = builder.Container;
                BeginLifetimeScope();
            }
        }

        public static void Initialize(List<Module> modules)
        {
            using (var builder = new ServiceLocatorBuilder())
            {
                foreach (var module in modules)
                {
                    builder.ContainerBuilder.RegisterModule(module);
                }
                Instance = builder.Build();
                Container = builder.Container;
                BeginLifetimeScope();
            }
        }

        public static void Initialize(Action<IServiceLocatorBuilder> action)
        {
            using (var builder = new ServiceLocatorBuilder())
            {
                action.Invoke(builder);
                Instance = builder.Build();
                Container = builder.Container;
                BeginLifetimeScope();
            }
        }

        [DebuggerStepThrough]
        private static T Resolve<T>()
        {
            return _currentLifetimeScope.Resolve<T>();
        }

        [DebuggerStepThrough]
        private static object Resolve(Type type)
        {
            return _currentLifetimeScope.Resolve(type);
        }

        [DebuggerStepThrough]
        private static T ResolveWith<T>(params object[] parameters) where T : class
        {
            return _currentLifetimeScope.ResolveOptional<T>(parameters.Select(i => new TypedParameter(i.GetType(), i)));
        }

        [DebuggerStepThrough]
        private static T ResolveWithCache<T>(params object[] parameters) where T : class
        {
            foreach (var cacheItem in _cache.ToList())
            {
                if (cacheItem.Key.SequenceEqual(parameters) && cacheItem.Value.IsAlive)
                {
                    return (T)cacheItem.Value.Target;
                }
            }
            var instance = ResolveWith<T>(parameters);
            _cache[parameters] = new WeakReference(instance);
            return instance;
        }

        [DebuggerStepThrough]
        private static void TryDisposeLifetimeScope()
        {
            if (_currentLifetimeScope == null)
            {
                return;
            }
            _currentLifetimeScope.Dispose();
            _currentLifetimeScope = null;
        }
    }
}