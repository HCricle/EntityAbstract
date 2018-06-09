using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 属性层,当寻找属性时，会一层一层地进入寻找，如果不存在则抛出异常，如果存在则返回寻找
    /// </summary>
    public abstract class PropertyLayer
    {
        public PropertyLayer(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path不能为空");
            }
            Path = path;
        }
        public virtual string Path { get; }
        public abstract object SteptInto(object value);
        protected void ValiValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentException("value不能为空");
            }
        }
        protected PropertyInfo GetProperty(object value, string relyPath = null)
        {
            if (value==null)
            {
                throw new ArgumentException($"源对象{value}为空");
            }
            var t = value.GetType();
            relyPath = relyPath ?? Path;
            var prop = t.GetProperty(relyPath);
            return prop?? throw new InvalidOperationException($"在{value}中，无法寻找到属性{Path}");
        }
    }
}
