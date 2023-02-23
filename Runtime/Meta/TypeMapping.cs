using System;

namespace Mirzipan.Infusion.Meta
{
    public struct TypeMapping : IEquatable<TypeMapping>
    {
        public readonly Type From;
        public readonly Type To;

        #region Lifecycle

        public TypeMapping(Type from, Type to)
        {
            From = from;
            To = to;
        }

        #endregion Lifecycle

        #region Equality

        public bool Equals(TypeMapping other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            
            return obj is TypeMapping mapping && Equals(mapping);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((From != null ? From.GetHashCode() : 0) * 397) ^ (To != null ? To.GetHashCode() : 0);
            }
        }

        #endregion Equality

        #region Operators

        public static bool operator ==(TypeMapping a, TypeMapping b) => a.From == b.From && a.To == b.To;

        public static bool operator !=(TypeMapping a, TypeMapping b) => !(a == b);

        #endregion Operators
    }
}