using Mvvmlighting.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mvvmlighting.Ioc
{
    public static class IocHelper
    {
        /// <summary>
        /// 确保此类含[NeedInIoc]的属性已在simpleioc内
        /// 需要是静态公开属性
        /// </summary>
        public static void EnsureIocKey(object obj)
        {
            var t = obj.GetType();
            var props = t.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            foreach (var item in props)
            {
                var attr = item.GetCustomAttribute<NeedKeyInIocAttribute>();
                if (attr != null)
                {
                    var value = item.GetValue(obj);
                    if (!SimpleIoc.Inst.ContainsKey(value))
                    {
                        throw new Exception($"key {value} 声明是必须在ioc容器里的,但当前验证过程不存在");
                    }
                }
            }
        }
        public static void EnsureIocInst(object obj)
        {
            var t = obj.GetType();
            var attrs = t.GetCustomAttributes(typeof(NeedInstInIocAttribute), false).Cast<NeedInstInIocAttribute>();
            foreach (var item in attrs)
            {
                if (!SimpleIoc.Inst.ContrainInst(item.Type))
                {
                    throw new Exception($"类型{item.Type.FullName}不存在于ioc容器");
                }
            }
        }
    }
}
