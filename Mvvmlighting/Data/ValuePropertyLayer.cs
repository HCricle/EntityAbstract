using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 直接寻属性
    /// </summary>
    public class ValuePropertyLayer : PropertyLayer
    {
        public ValuePropertyLayer(string path) : base(path)
        {

        }

        public override object SteptInto(object value)
        {
            ValiValue(value);
            var prop = GetProperty(value);
            return prop.GetValue(value);
        }
    }
}
