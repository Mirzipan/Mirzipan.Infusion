using System;

namespace Mirzipan.Infusion.Meta
{
    public struct TypeInstanceId : IEquatable<TypeInstanceId>
    {
        public readonly Type Type;
        public readonly string Name;

        #region Lifecycle

        public TypeInstanceId(Type type) : this(type, string.Empty)
        {
        }

        public TypeInstanceId(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        #endregion Lifecycle

        #region Public

        public override string ToString()
        {
            return $"{Type?.Name}/{Name}";
        }

        #endregion Public

        #region Equality

        public bool Equals(TypeInstanceId other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            
            return obj is TypeInstanceId id && Equals(id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        #endregion Equality

        #region Operators

        public static bool operator ==(TypeInstanceId a, TypeInstanceId b)
        {
            return a.Type == b.Type && string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(TypeInstanceId a, TypeInstanceId b) => !(a == b);

        #endregion Operators
    }
}