using System;
using System.Collections.Generic;

namespace Mirzipan.Infusion
{
    public interface IInjectionContainer
    {
        IInjectionContainer Parent { get; set; }
        IInjectionContainer CreateChildContainer();
        void Inject(object instance);
        void InjectAll();
        T Resolve<T>(string identifier = null, bool requireInstance = false, object[] args = null) where T : class;
        object Resolve(Type baseType, string identifier = null, bool requireInstance = false, object[] constructorArgs = null);
        IEnumerable<T> ResolveAll<T>();
        IEnumerable<object> ResolveAll(Type type);
        T Instantiate<T>(object[] constructorArgs = null);
        object Instantiate(Type type, object[] constructorArgs = null);
        TBase ResolveRelation<TFor, TBase>(object[] args = null);
        object ResolveRelation(Type forType, Type baseType, object[] args = null);
        void Bind<T>(T instance) where T : class;
        void Bind<T>(T instance, bool injectNow) where T : class;
        void Bind<T>(T instance, string identifier, bool injectNow = true) where T : class;
        void Bind(Type baseType, object instance, string identifier = null, bool injectNow = true);
        void Bind<TFor, TBase, TConcrete>();
        void Bind(Type forType, Type baseType, Type concreteType);
        void Unbind<T>(string identifier = null);
        void Unbind(Type forType, string identifier = null);
        void Unbind<TFor, TBase>();
        void Unbind(Type forType, Type baseType);
        void UnbindInstances();
        void UnbindRelationship();
        bool HasBinding<T>();
        bool HasBinding<T>(string identifier);
        bool HasBinding(Type type);
        bool HasBinding(Type type, string identifier);
    }
}