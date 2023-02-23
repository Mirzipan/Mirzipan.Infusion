using System;
using System.Collections.Generic;
using Mirzipan.Infusion.Meta;

namespace Mirzipan.Infusion.Collections
{
    public class TypeRelationCollection : Dictionary<TypeMapping, Type>
    {
        public Type this[Type from, Type to]
        {
            get => TryGetValue(new TypeMapping(from, to), out Type mapping) ? mapping : null;
            set => this[new TypeMapping(from, to)] = value;
        }
    }
}