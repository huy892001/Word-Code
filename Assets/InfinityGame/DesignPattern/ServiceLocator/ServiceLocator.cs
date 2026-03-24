using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityGame.DesignPattern.ServiceLocator
{
    /// <summary>
    /// A robust Service Locator implementation for decoupling dependencies.
    /// Can be used as a Global, Scene, or Local locator.
    /// </summary>
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static ServiceLocator Global { get; } = new ServiceLocator();
        public static ServiceLocator Local { get; internal set; } = new ServiceLocator();

        /// <summary>
        /// Registers a service.
        /// </summary>
        public void Register<T>(T service)
        {
            _services[typeof(T)] = service;
            if (this == Global) 
            {
                if (service is Component comp)
                {
                    if (comp.gameObject.transform.parent == null)
                    {
                        UnityEngine.Object.DontDestroyOnLoad(comp.gameObject);
                    }
                }
            }
        }

        public void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }

        public T Get<T>()
        {
            Type type = typeof(T);
            if (!_services.TryGetValue(type, out object service))
            {
                if (this != Global) return Global.Get<T>();

                throw new Exception($"[ServiceLocator] Service {type.Name} not registered.");
            }

            return (T)service;
        }

        /// <summary>
        /// Clears all services and disposes those that implement IDisposable.
        /// </summary>
        public void Clear()
        {
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _services.Clear();
        }
    }

    /// <summary>
    /// Extension methods for easy access to the Global Service Locator.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        public static T GetServiceGlobal<T>()
        {
            return ServiceLocator.Global.Get<T>();
        }

        public static T GetServiceLocal<T>()
        {
            return ServiceLocator.Local.Get<T>();
        }
    }
}