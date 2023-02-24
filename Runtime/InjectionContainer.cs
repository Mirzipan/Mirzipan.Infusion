using System;
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

        public void Inject(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var objType = obj.GetType();
            var info = GetTypeInfo(objType);

            foreach (var member in info.Members)
            {
                member.Setter(obj, Resolve(member.MemberType, member.InjectName, false, null));
            }
        }

        public T Resolve<T>(string name = null, bool requireInstance = false, object[] args = null) where T : class
        {
            return (T)Resolve(typeof(T), name, requireInstance, args);
        }

        public object Resolve(Type baseType, string name = null, bool requireInstance = false, object[] constructorArgs = null)
        {
            object item = _instances[baseType, name];
            if (item != null)
            {
                return item;
            }

            if (requireInstance && _parent == null)
            {
                return null;
            }

            Type namedMapping = _mappings[baseType, name];
            if (namedMapping == null)
            {
                return _parent?.Resolve(baseType, name, requireInstance, constructorArgs);
            }

            return Instantiate(namedMapping, constructorArgs);
        }

        public T Instantiate<T>(object[] constructorArgs = null) => (T)Instantiate(typeof(T), constructorArgs);

        public object Instantiate(Type type, object[] constructorArgs = null)
        {
            if (!constructorArgs.IsNullOrEmpty())
            {
                var result = Activator.CreateInstance(type, constructorArgs);
                Inject(result);
                return result;
            }

            // TODO: more type reflection
            return null;
        }

        public void Bind<T>(T instance) where T : class
        {
            Bind(instance, null, true);
        }

        public void Bind<T>(T instance, bool injectNow) where T : class
        {
            Bind(instance, null, injectNow);
        }

        public void Bind<T>(T instance, string name, bool injectNow = true) where T : class
        {
            Bind(typeof(T), instance, name, injectNow);
        }

        public void Bind(Type baseType, object instance, string name = null, bool injectNow = true)
        {
            _instances[baseType, name] = instance;

            if (injectNow)
            {
                Inject(instance);
            }
        }

        public void BindWithInterfaces<T>(T instance) where T : class
        {
            BindWithInterfaces(instance, true);
        }

        public void BindWithInterfaces<T>(T instance, bool injectNow) where T : class
        {
            BindWithInterfaces(instance, null, injectNow);
        }

        public void BindWithInterfaces<T>(T instance, string name, bool injectNow = true) where T : class
        {
            BindWithInterfaces(typeof(T), instance, name, injectNow);
        }

        public void BindWithInterfaces(Type baseType, object instance, string name = null, bool injectNow = true)
        {
            var interfaces = baseType.GetInterfaces();
            foreach (var entry in interfaces)
            {
                Bind(entry, instance, name);
            }

            _instances[baseType, name] = instance;

            if (injectNow)
            {
                Inject(instance);
            }
        }

        #endregion Queries

        #region Private

        private static TypeInjectionInfo GetTypeInfo(Type type)
        {
            InjectionIndexer.Instance.Index(type);
            InjectionIndexer.Instance.TryGetInfo(type, out var result);
            return result;
        }

        #endregion Private
    }
}