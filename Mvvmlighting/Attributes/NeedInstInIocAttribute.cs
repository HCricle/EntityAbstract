using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class NeedInstInIocAttribute : Attribute
    {
        public NeedInstInIocAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get;  }
    }
}
