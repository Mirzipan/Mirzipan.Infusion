using System;
using System.Reflection;
using Mirzipan.Extensions.Collections;

namespace Mirzipan.Infusion.Meta
{
    public class InjectableMethodInfo
    {
        public readonly Action<object, object[]> Invoke;
        public readonly Parameter[] Parameters;

        public InjectableMethodInfo(MethodInfo method)
        {
            Invoke = (target, parameters) => method.Invoke(target, parameters);
            
            var parameters = method.GetParameters();
            if (parameters.IsNullOrEmpty())
            {
                Parameters = Array.Empty<Parameter>();
                return;
            }
            
            Parameters = new Parameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameters[i] = new Parameter(parameters[i]);
            }
        }
        
        
    }
}