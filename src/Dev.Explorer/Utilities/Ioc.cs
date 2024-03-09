namespace FileExplorer.Utilities
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public sealed class Ioc
    {
        private IServiceProvider serviceProvider;

        private Ioc()
        {
        }

        public static Ioc Default { get; } = new();

        public object GetService(Type serviceType)
        {
            if (this.serviceProvider is null)
            {
                return CreateDesginInstance(serviceType);
            }

            object service = this.serviceProvider!.GetService(serviceType);

            return service;
        }

        public T GetService<T>()
            where T : class
        {
            if (this.serviceProvider is null)
            {
                return CreateDesginInstance<T>();
            }

            T service = this.serviceProvider!.GetService<T>();

            return service;
        }

        private static object CreateDesginInstance(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null && !type.IsAbstract
               ? Activator.CreateInstance(type)
               : throw new InvalidOperationException($"Cannot create instance of type {type.FullName}");
        }

        private static T CreateDesginInstance<T>()
        {
            return (T)CreateDesginInstance(typeof(T));
        }
    }
}