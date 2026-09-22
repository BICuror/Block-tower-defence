using System;

namespace Cashing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CachedAttribute : Attribute
    {
        public bool Required { get; private set; }
        
        public CachedAttribute(bool required = true)
        {
            Required = required;
        }
    }
}