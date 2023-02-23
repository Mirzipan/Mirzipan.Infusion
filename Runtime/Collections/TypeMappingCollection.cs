using System;
using System.Collections.Generic;
using Mirzipan.Infusion.Meta;

namespace Mirzipan.Infusion.Collections
{
    public class TypeMappingCollection : Dictionary<TypeInstanceId, Type>
    {
        public Type this[Type from, string name = null]
        {
            get => TryGetValue(new TypeInstanceId(from, name), out Type mapping) ? mapping : null;
            set => this[new TypeInstanceId(from, name)] = value;
        }
    }
}