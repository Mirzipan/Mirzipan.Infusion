using System;
using System.Collections.Generic;
using Mirzipan.Infusion.Meta;

namespace Mirzipan.Infusion.Collections
{
    public class TypeInstanceCollection : Dictionary<TypeInstanceId, object>
    {
        public object this[Type from, string name = null]
        {
            get => TryGetValue(new TypeInstanceId(from, name), out var mapping) ? mapping : null;
            set => this[new TypeInstanceId(from, name)] = value;
        }
    }
}