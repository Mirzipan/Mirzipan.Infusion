using System;

namespace Mirzipan.Infusion.Extensions
{
    public static class InjectionContainerExtensions
    {
        public static void BindWithInterfaces<T>(this IInjectionContainer @this, T instance) where T : class
        {
            BindWithInterfaces(@this, instance, true);
        }

        public static void BindWithInterfaces<T>(this IInjectionContainer @this, T instance, bool injectNow)
            where T : class
        {
            BindWithInterfaces(@this, instance, null, injectNow);
        }

        public static void BindWithInterfaces<T>(this IInjectionContainer @this, T instance, string identifier,
            bool injectNow = true) where T : class
        {
            BindWithInterfaces(@this, typeof(T), instance, identifier, injectNow);
        }

        public static void BindWithInterfaces(this IInjectionContainer @this, Type baseType, object instance,
            string identifier = null, bool injectNow = true)
        {
            var interfaces = baseType.GetInterfaces();
            foreach (var entry in interfaces)
            {
                @this.Bind(entry, instance, identifier);
            }

            @this.Bind(baseType, instance, identifier, injectNow);
        }
    }
}