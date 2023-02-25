using System;
using System.Reflection;
using Mirzipan.Extensions.Collections;

namespace Mirzipan.Infusion.Meta
{
    public class InjectableConstructorInfo
    {
        public readonly Parameter[] Parameters;

        public InjectableConstructorInfo(ParameterInfo[] parameters)
        {
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