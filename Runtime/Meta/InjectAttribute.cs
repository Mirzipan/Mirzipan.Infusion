using System;

namespace Mirzipan.Infusion.Meta
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute: Attribute
    {
        public string Name { get; set; }

        public InjectAttribute()
        {
        }

        public InjectAttribute(string name)
        {
            Name = name;
        }
    }
}