﻿using System;
using System.Collections.Generic;
using Mirzipan.Bibliotheca.Disposables;
using Mirzipan.Extensions.Collections;
using Mirzipan.Infusion.Collections;
using Mirzipan.Infusion.Meta;
using Mirzipan.Infusion.Resolvers;

namespace Mirzipan.Infusion
{
    public class InjectionContainer : IInjectionContainer
    {
        private readonly List<InjectionContainer> _children = new();
        private readonly TypeResolverCollection _resolvers = new();
        private InjectionContainer _parent;
        private CompositeDisposable _disposer = new();

        public string Name { get; }
        public IInjectionContainer Parent => _parent;
        public IReadOnlyList<IInjectionContainer> Children => _children;

        #region Lifecycle

        public InjectionContainer(string name)
        {
            Name = name;
            InjectSelf();
        }

        public IInjectionContainer CreateChild(string name)
        {
            var result = new InjectionContainer(name);
            result._parent = this;
            _children.Add(result);
            return result;
        }

        public void Dispose()
        {
            int count = _children.Count;

            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    var child = _children[i];
                    child.Dispose();
                }
            }

            _disposer.Dispose();

            if (_parent != null)
            {
                _parent._children.Remove(this);
                _parent = null;
            }
        }

        #endregion Lifecycle

        #region Queries

        public void Inject(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var objType = instance.GetType();
            var info = GetTypeInfo(objType);

            foreach (var member in info.Members)
            {
                member.Setter(instance, Resolve(member.MemberType, member.InjectName, false, null));
            }

            foreach (var method in info.Methods)
            {
                InjectMethod(method, instance);
            }
        }

        public T Resolve<T>(string identifier = null, bool requireInstance = false, object[] args = null) where T : class
        {
            return (T)Resolve(typeof(T), identifier, requireInstance, args);
        }

        public object Resolve(Type baseType, string identifier = null, bool requireInstance = false, object[] constructorArgs = null)
        {
            IResolver resolver = _resolvers[baseType, identifier];
            if (resolver != null)
            {
                return resolver.Resolve(this);
            }
            
            if (requireInstance && _parent == null)
            {
                return null;
            }

            if (_parent != null)
            {
                return _parent.Resolve(baseType, identifier, requireInstance, constructorArgs);
            }

            return Instantiate(baseType, constructorArgs);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            foreach (var entry in ResolveAll(typeof(T)))
            {
                yield return (T)entry;
            }
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            foreach (var entry in _resolvers)
            {
                if (entry.Key.Type == type && entry.Key.Name.NotNullOrEmpty())
                {
                    yield return entry.Value.Resolve(this);
                }
            }
        }

        public T Instantiate<T>(object[] constructorArgs = null) => (T)Instantiate(typeof(T), constructorArgs);

        public object Instantiate(Type type, object[] constructorArgs = null)
        {
            if (!constructorArgs.IsNullOrEmpty())
            {
                return InstantiateAndInject(type, constructorArgs);
            }

            var info = GetTypeInfo(type);
            var constructor = info.DefaultConstructor;
            if (constructor == null)
            {
                return null;
            }

            if (constructor.Parameters.Length == 0)
            {
                return InstantiateAndInject(type);
            }

            int count = constructor.Parameters.Length;
            var args = new object[count];
            for (int i = 0; i < count; i++)
            {
                var parameter = constructor.Parameters[i];
                if (parameter.Type.IsArray)
                {
                    args[i] = ResolveAll(parameter.Type);
                    continue;
                }

                args[i] = Resolve(parameter.Type) ?? Resolve(parameter.Type, parameter.Name);
            }

            return InstantiateAndInject(type, args);
        }

        #endregion Queries

        #region Bind / Unbind

        public void BindInstance(object instance, string identifier = null)
        {
            BindInstance(instance.GetType(), instance, identifier);
        }
        
        public void BindInstance<T>(T instance)
        {
            BindInstance(instance, null, true);
        }

        public void BindInstance<T>(T instance, bool injectNow)
        {
            BindInstance(instance, null, injectNow);
        }

        public void BindInstance<T>(T instance, string identifier, bool injectNow = true)
        {
            BindInstance(typeof(T), instance, identifier, injectNow);
        }

        public void BindInstance(Type baseType, object instance, string identifier = null, bool injectNow = true)
        {
            var resolver = new InstanceResolver(instance);
            _disposer.Add(resolver);
            _resolvers[baseType, identifier] = resolver;

            if (injectNow)
            {
                Inject(instance);
            }
        }
        
        public void BindLazy<T>(string identifier = null)
        {
            BindLazy(typeof(T), identifier);
        }

        public void BindLazy(Type type, string identifier = null)
        {
            var resolver = new LazyInstanceResolver(type);
            _disposer.Add(resolver);
            _resolvers[type, identifier] = resolver;
        }

        public void BindFactory<TBase, TConcrete>(string identifier = null) where TConcrete : TBase
        {
            BindFactory(typeof(TBase), typeof(TConcrete), identifier);
        }

        public void BindFactory(Type baseType, Type concreteType, string identifier = null)
        {
            var resolver = new FactoryResolver(concreteType);
            _disposer.Add(resolver);
            _resolvers[baseType, identifier] = resolver;
        }

        public void BindFunction<T>(Func<T> factory, string identifier = null)
        {
            var resolver = new FunctionResolver(factory as Func<object>);
            _disposer.Add(resolver);
            _resolvers[typeof(T), identifier] = resolver;
        }

        public void Unbind<T>(string identifier = null)
        {
            Unbind(typeof(T), identifier);
        }

        public void Unbind(Type forType, string identifier = null)
        {
            _resolvers.Remove(new TypeInstanceId(forType, identifier));
        }

        public bool HasBinding<T>(string identifier = null)
        {
            return HasBinding(typeof(T), identifier);
        }

        public bool HasBinding(Type type, string identifier = null)
        {
            return _resolvers.ContainsKey(new TypeInstanceId(type, identifier));
        }

        #endregion Bind / Unbind

        #region Private

        private void InjectSelf()
        {
            BindInstance(this);
            BindInstance<IInjectionContainer>(this);
        }
        
        private static TypeInjectionInfo GetTypeInfo(Type type)
        {
            InjectionIndexer.Instance.Index(type);
            InjectionIndexer.Instance.TryGetInfo(type, out var result);
            return result;
        }

        private object InstantiateAndInject(Type type)
        {
            var result = Activator.CreateInstance(type);
            Inject(result);
            return result;
        }

        private object InstantiateAndInject(Type type, object[] constructorArgs)
        {
            var result = Activator.CreateInstance(type, constructorArgs);
            Inject(result);
            return result;
        }

        private void InjectMethod(InjectableMethodInfo method, object instance)
        {
            int count = method.Parameters.Length;
            var args = new object[count];
            for (int i = 0; i < count; i++)
            {
                var parameter = method.Parameters[i];
                if (parameter.Type.IsArray)
                {
                    args[i] = ResolveAll(parameter.Type);
                    continue;
                }

                args[i] = Resolve(parameter.Type) ?? Resolve(parameter.Type, parameter.Name);
            }

            method.Invoke(instance, args);
        }

        #endregion Private
    }
}