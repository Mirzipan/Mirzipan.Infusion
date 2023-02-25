using System;
using System.Reflection;

namespace Mirzipan.Infusion.Meta
{
    public struct Parameter
    {
        public readonly Type Type;
        public readonly string Name;

        public Parameter(ParameterInfo info)
        {
            Type = info.ParameterType;
            Name = info.Name;
        }
    }
}