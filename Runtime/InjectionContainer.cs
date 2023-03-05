using System;
using System.Collections.Generic;
using Mirzipan.Extensions.Collections;
using Mirzipan.Infusion.Collections;
using Mirzipan.Infusion.Meta;

namespace Mirzipan.Infusion
{
    public class InjectionContainer : IInjectionContainer, IDisposable
    {
        private IInjectionContainer _parent;

        private readonly TypeRelationCollection _relationships = new TypeRelationCollection();
        private readonly TypeInstanceCollection _instances = new TypeInstanceCollection();
        private readonly TypeMappingCollection _mappings = new TypeMappingCollection();

        public IInjectionContainer Parent
        {
            get => _parent;
            set => _parent = value;
        }

        #region Lifecycle

        public InjectionContainer()
        {
        }

        public InjectionContainer(IInjectionContainer parent)
        {
            _parent = parent;
        }

        public IInjectionContainer CreateChildContainer() => new InjectionContainer(this);

        public void Dispose()
        {
            _relationships.Clear();
            _instances.Clear();
            _mappings.Clear();
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
        }

        public void InjectAll()
        {
            foreach (var entry in _instances)
            {
                Inject(entry);
            }
        }

        public T Resolve<T>(string identifier = null, bool requireInstance = false, object[] args = null) where T : class
        {
            return (T)Resolve(typeof(T), identifier, requireInstance, args);
        }

        public object Resolve(Type baseType, string identifier = null, bool requireInstance = false, object[] constructorArgs = null)
        {
            object item = _instances[baseType, identifier];
            if (item != null)
            {
                return item;
            }

            if (requireInstance && _parent == null)
            {
                return null;
            }

            Type namedMapping = _mappings[baseType, identifier];
            if (namedMapping == null)
            {
                return _parent?.Resolve(baseType, identifier, requireInstance, constructorArgs);
            }

            return Instantiate(namedMapping, constructorArgs);
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
            foreach (var entry in _instances)
            {
                if (entry.Key.Type == type && entry.Key.Name.NotNullOrEmpty())
                {
                    yield return entry.Value;
                }
            }

            foreach (var entry in _mappings)
            {
                if (entry.Key.Name.IsNullOrEmpty())
                {
                    continue;
                }

                bool isAssignableFrom = type.IsAssignableFrom(entry.Key.Type);
                if (!isAssignableFrom)
                {
                    continue;
                }

                yield return InstantiateAndInject(entry.Value);
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

        public TBase ResolveRelation<TFor, TBase>(object[] args = null)
        {
            return (TBase)ResolveRelation(typeof(TFor), typeof(TBase), args);
        }

        public object ResolveRelation(Type forType, Type baseType, object[] args = null)
        {
            var type = _relationships[forType, baseType];
            return type != null ? Instantiate(type, args) : null;
        }

        #endregion Queries

        #region Bind / Unbind

        public void Bind<T>(T instance) where T : class
        {
            Bind(instance, null, true);
        }

        public void Bind<T>(T instance, bool injectNow) where T : class
        {
            Bind(instance, null, injectNow);
        }

        public void Bind<T>(T instance, string identifier, bool injectNow = true) where T : class
        {
            Bind(typeof(T), instance, identifier, injectNow);
        }

        public void Bind(Type baseType, object instance, string identifier = null, bool injectNow = true)
        {
            _instances[baseType, identifier] = instance;

            if (injectNow)
            {
                Inject(instance);
            }
        }

        public void Bind<TBase, TConcrete>(string identifier = null)
        {
            _mappings[new TypeInstanceId(typeof(TBase), identifier)] = typeof(TConcrete);
        }

        public void Bind(Type baseType, Type concreteType, string identifier = null)
        {
            _mappings[new TypeInstanceId(baseType, identifier)] = concreteType;
        }

        public void Bind<TFor, TBase, TConcrete>()
        {
            _relationships[typeof(TFor), typeof(TBase)] = typeof(TConcrete);
        }

        public void Bind(Type forType, Type baseType, Type concreteType)
        {
            _relationships[forType, baseType] = concreteType;
        }

        public void Unbind<T>(string identifier = null)
        {
            Unbind(typeof(T), identifier);
        }

        public void Unbind(Type forType, string identifier = null)
        {
            _instances.Remove(new TypeInstanceId(forType, identifier));
        }

        public void Unbind<TFor, TBase>()
        {
            Unbind(typeof(TFor), typeof(TBase));
        }

        public void Unbind(Type forType, Type baseType)
        {
            _relationships.Remove(new TypeMapping(forType, baseType));
        }

        public void UnbindInstances()
        {
            _instances.Clear();
        }

        public void UnbindRelationship()
        {
            _relationships.Clear();
        }

        public bool HasBinding<T>()
        {
            return HasBinding(typeof(T));
        }

        public bool HasBinding<T>(string identifier)
        {
            return HasBinding(typeof(T), identifier);
        }

        public bool HasBinding(Type type)
        {
            return HasBinding(type, null);
        }

        public bool HasBinding(Type type, string identifier)
        {
            return _instances.ContainsKey(new TypeInstanceId(type, identifier));
        }

        #endregion Bind / Unbind

        #region Private

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

        #endregion Private
    }
}