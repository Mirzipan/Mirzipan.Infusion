using System;
using System.Collections.Generic;
using Mirzipan.Infusion.Meta;
using Mirzipan.Infusion.Resolvers;

namespace Mirzipan.Infusion.Collections
{
    public class TypeResolverCollection : Dictionary<TypeInstanceId, IResolver>
    {
        public IResolver this[Type from, string name = null]
        {
            get => TryGetValue(new TypeInstanceId(from, name), out var mapping) ? mapping : null;
            set => this[new TypeInstanceId(from, name)] = value;
        }
    }
}