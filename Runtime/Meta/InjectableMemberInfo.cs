using System;
using System.Reflection;

namespace Mirzipan.Infusion.Meta
{
    public class InjectableMemberInfo
    {
        public readonly Type MemberType;
        public readonly string InjectName;
        public readonly bool RequireInstance;

        public readonly Action<object, object> Setter;

        public InjectableMemberInfo(FieldInfo field, string injectName, bool requireInstance)
        {
            MemberType = field.FieldType;
            InjectName = injectName;
            RequireInstance = requireInstance;
            Setter = field.SetValue;
        }

        public InjectableMemberInfo(PropertyInfo property, string injectName, bool requireInstance)
        {
            MemberType = property.PropertyType;
            InjectName = injectName;
            RequireInstance = requireInstance;
            Setter = (target, value) => property.SetValue(target, value, null);
        }
    }
}