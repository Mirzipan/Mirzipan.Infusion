using System;
using Mirzipan.Infusion.Collections;

namespace Mirzipan.Infusion
{
    public interface IInjectionContainer
    {
        TypeRelationCollection Relationships { get; set; }
        TypeInstanceCollection Instances { get; set; }
        TypeMappingCollection Mappings { get; set; }
        void Inject(object obj);
        T Resolve<T>(string name = null, bool requireInstance = false, object[] args = null) where T : class;
        object Resolve(Type baseType, string name = null, bool requireInstance = false, object[] constructorArgs = null);
        object CreateInstance(Type type, object[] constructorArgs = null);
    }
}