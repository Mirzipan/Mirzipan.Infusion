using System;
using System.Reflection;

namespace Mirzipan.Infusion.Meta
{
    public class InjectableMemberInfo
    {
        public readonly Type MemberType;
        public readonly string InjectName;

        public readonly Action<object, object> Setter;

        public InjectableMemberInfo(FieldInfo field, string injectName)
        {
            MemberType = field.FieldType;
            InjectName = injectName;
            Setter = field.SetValue;
        }

        public InjectableMemberInfo(PropertyInfo property, string injectName)
        {
            MemberType = property.PropertyType;
            InjectName = injectName;
            Setter = (target, value) => property.SetValue(target, value, null);
        }
    }
}