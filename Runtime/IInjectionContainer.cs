using System;
using System.Collections.Generic;

namespace Mirzipan.Infusion
{
    public interface IInjectionContainer : IDisposable
    {
        string Name { get; }
        IInjectionContainer Parent { get; }
        IReadOnlyList<IInjectionContainer> Children { get; }
        IInjectionContainer CreateChild(string name);
        void Inject(object instance);
        T Resolve<T>(string identifier = null, bool requireInstance = false, object[] args = null) where T : class;
        object Resolve(Type baseType, string identifier = null, bool requireInstance = false, object[] constructorArgs = null);
        IEnumerable<T> ResolveAll<T>();
        IEnumerable<object> ResolveAll(Type type);
        T Instantiate<T>(object[] constructorArgs = null);
        object Instantiate(Type type, object[] constructorArgs = null);
        void Bind(object instance, string identified = null);
        void Bind<T>(T instance);
        void Bind<T>(T instance, bool injectNow);
        void Bind<T>(T instance, string identifier, bool injectNow = true);
        void Bind(Type baseType, object instance, string identifier = null, bool injectNow = true);
        void Bind<TBase, TConcrete>(string identifier = null) where TConcrete : TBase;
        void Bind(Type baseType, Type concreteType, string identifier = null);
        void Bind<T>(Func<T> factory, string identifier = null);
        void Bind(Type baseType, Func<object> factory, string identifier = null);
        void Unbind<T>(string identifier = null);
        void Unbind(Type forType, string identifier = null);
        bool HasBinding<T>(string identifier = null);
        bool HasBinding(Type type, string identifier = null);
    }
}