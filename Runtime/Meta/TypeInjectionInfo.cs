using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mirzipan.Infusion.Meta
{
    public class TypeInjectionInfo
    {
        private static readonly Type InjectAttributeType = typeof(InjectAttribute);
        
        public readonly InjectableMemberInfo[] Members;

        public TypeInjectionInfo(Type type)
        {
            var members = type.GetMembers();
            var list = new List<InjectableMemberInfo>(members.Length);
            
            foreach (var member in members)
            {
                var attribute = member.GetCustomAttribute<InjectAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var field = member as FieldInfo;
                if (field != null)
                {
                    list.Add(new InjectableMemberInfo(field, attribute.Name));
                    continue;
                }

                var property = member as PropertyInfo;
                if (property != null)
                {
                    list.Add(new InjectableMemberInfo(property, attribute.Name));
                    continue;
                }
            }

            Members = list.ToArray();
        }
    }
}