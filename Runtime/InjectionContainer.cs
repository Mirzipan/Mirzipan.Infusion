using System;
using Mirzipan.Extensions.Collections;
using Mirzipan.Infusion.Collections;
using Mirzipan.Infusion.Meta;

namespace Mirzipan.Infusion
{
    public class InjectionContainer : IInjectionContainer, IDisposable
    {
        private IInjectionContainer _parent;

        private TypeRelationCollection _relationships = new TypeRelationCollection();
        private TypeInstanceCollection _instances = new TypeInstanceCollection();
        private TypeMappingCollection _mappings = new TypeMappingCollection();

        public TypeRelationCollection Relationships
        {
            get => _relationships;
            set => _relationships = value;
        }

        public TypeInstanceCollection Instances
        {
            get => _instances;
            set => _instances = value;
        }

        public TypeMappingCollection Mappings
        {
            get => _mappings;
            set => _mappings = value;
        }

        #region Lifecycle

        public InjectionContainer()
        {
        }

        public InjectionContainer(IInjectionContainer parent)
        {
            _parent = parent;
        }

        public void Dispose()
        {
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
            object item = Instances[baseType, name];
            if (item != null)
            {
                return item;
            }

            if (requireInstance)
            {
                return _parent?.Instances[baseType, name];
            }

            Type namedMapping = Mappings[baseType, name];
            if (namedMapping == null)
            {
                return _parent?.Mappings[baseType, name];
            }

            return CreateInstance(namedMapping, constructorArgs);
        }

        public object CreateInstance(Type type, object[] constructorArgs = null)
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

        #endregion Queries

        #region Private

        private TypeInjectionInfo GetTypeInfo(Type type)
        {
            InjectionIndexer.Instance.Index(type);
            InjectionIndexer.Instance.TryGetInfo(type, out var result);
            return result;
        }

        #endregion Private
    }
}