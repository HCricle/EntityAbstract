using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Attributes
{
    /// <summary>
    /// 表明一定要在simpleioc容器内
    /// </summary>
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class NeedKeyInIocAttribute : Attribute
    {
        public NeedKeyInIocAttribute()
        {
        }

    }
}
