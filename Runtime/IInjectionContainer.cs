using System;
using Mirzipan.Infusion.Collections;

namespace Mirzipan.Infusion
{
    public interface IInjectionContainer
    {
        IInjectionContainer Parent { get; set; }
        IInjectionContainer CreateChildContainer();
        void Inject(object obj);
        T Resolve<T>(string name = null, bool requireInstance = false, object[] args = null) where T : class;
        object Resolve(Type baseType, string name = null, bool requireInstance = false, object[] constructorArgs = null);
        T Instantiate<T>(object[] constructorArgs = null);
        object Instantiate(Type type, object[] constructorArgs = null);
        void Bind<T>(T instance) where T : class;
        void Bind<T>(T instance, bool injectNow) where T : class;
        void Bind<T>(T instance, string name, bool injectNow = true) where T : class;
        void Bind(Type baseType, object instance, string name = null, bool injectNow = true);
        void BindWithInterfaces<T>(T instance) where T : class;
        void BindWithInterfaces<T>(T instance, bool injectNow) where T : class;
        void BindWithInterfaces<T>(T instance, string name, bool injectNow = true) where T : class;
        void BindWithInterfaces(Type baseType, object instance, string name = null, bool injectNow = true);
    }
}