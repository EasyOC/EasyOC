using System;

namespace EasyOC.Core.DependencyInjection
{
    public interface IEasyOCLazyServiceProvider
    {
        object LazyGetRequiredService(Type serviceType);
        T LazyGetRequiredService<T>();
        object LazyGetService(Type serviceType);
        object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory);
        object LazyGetService(Type serviceType, object defaultValue);
        T LazyGetService<T>();
        T LazyGetService<T>(Func<IServiceProvider, object> factory);
        T LazyGetService<T>(T defaultValue);
    }
}
