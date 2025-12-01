using System;

namespace Cashing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CachedAttribute : Attribute {}
}