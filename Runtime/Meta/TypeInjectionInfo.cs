﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mirzipan.Infusion.Meta
{
    public class TypeInjectionInfo
    {
        private static readonly Type InjectAttributeType = typeof(InjectAttribute);

        public readonly InjectableConstructorInfo[] Constructors;
        public readonly InjectableMemberInfo[] Members;
        public readonly InjectableMethodInfo[] Methods;

        public readonly InjectableConstructorInfo DefaultConstructor;
        
        public TypeInjectionInfo(Type type)
        {
            Constructors = GetInjectableConstructors(type, out DefaultConstructor).ToArray();
            Members = GetInjectableMembers(type).ToArray();
            Methods = GetInjectableMethods(type).ToArray();
        }

        private static List<InjectableConstructorInfo> GetInjectableConstructors(Type type, out InjectableConstructorInfo @default)
        {
            @default = null;
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var result = new List<InjectableConstructorInfo>(constructors.Length);

            foreach (var constructor in constructors)
            {
                var info = new InjectableConstructorInfo(constructor.GetParameters());
                if (@default == null || @default.Parameters.Length < info.Parameters.Length)
                {
                    @default = info;
                }
                
                result.Add(info);
            }
            
            return result;
        }
        
        private static List<InjectableMemberInfo> GetInjectableMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var members = type.GetMembers(flags);
            var result = new List<InjectableMemberInfo>(members.Length);

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
                    result.Add(new InjectableMemberInfo(field, attribute.Name, attribute.RequireInstance));
                    continue;
                }

                var property = member as PropertyInfo;
                if (property != null)
                {
                    result.Add(new InjectableMemberInfo(property, attribute.Name, attribute.RequireInstance));
                    continue;
                }
            }

            return result;
        }

        private static List<InjectableMethodInfo> GetInjectableMethods(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = type.GetMethods(flags);
            var result = new List<InjectableMethodInfo>(methods.Length);
            
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<InjectAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                result.Add(new InjectableMethodInfo(method));
            }

            return result;
        }
    }
}