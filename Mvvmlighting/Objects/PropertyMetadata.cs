using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Objects
{
    /// <summary>
    /// 数据注册信息
    /// </summary>
    public class PropertyMetadata
    {
        public PropertyMetadata()
        {
        }

        public PropertyMetadata(Func<object> defaultValue, PropertyChangedCallback propertyChangedCallback=null, CoerceValueCallback coerceValueCallback=null)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
        }

        /// <summary>
        /// 属性默认值
        /// </summary>
        public Func<object> DefaultValue { get; set; }
        /// <summary>
        /// 属性改变回调
        /// </summary>
        public PropertyChangedCallback PropertyChangedCallback { get; set; }
        /// <summary>
        /// 属性转换
        /// </summary>
        public CoerceValueCallback CoerceValueCallback { get; set; }

    }
}
