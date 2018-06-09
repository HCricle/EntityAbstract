using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Objects
{
    /// <summary>
    /// 依赖属性
    /// </summary>
    public sealed class DependencyProperty
    {
        /// <summary>
        /// local没设置值
        /// </summary>
        public static readonly object UnsetValue = Guid.NewGuid();
        private static volatile int id = int.MinValue;
        private static List<DependencyProperty> Dps{ get; } = new List<DependencyProperty>();
        private DependencyProperty(ValidateValueCallback validateValueCallback, PropertyMetadata defaultMetadata, Type ownerType, Type propertyType, string name, bool readOnly = false)
        {
            if (!ownerType.IsSubclassOf(typeof(DependencyObject)))
            {
                throw new ArgumentException($"ownerType 必须是继承于DependencyObject,ownerType类型{ownerType.FullName}");
            }
            ValidateValueCallback = validateValueCallback;
            DefaultMetadata = defaultMetadata ?? throw new ArgumentException("defaultMetadata 不能为Null");
            OwnerType = ownerType;
            PropertyType = propertyType;
            Name = name;
            GlobalIndex = ++id;
            ReadOnly = readOnly;
        }
        
        #region Properties
        /// <summary>
        /// 验证回调
        /// </summary>
        public ValidateValueCallback ValidateValueCallback { get; }
        /// <summary>
        /// 注册的信息
        /// </summary>
        public PropertyMetadata DefaultMetadata { get; }
        /// <summary>
        /// 输入哪个类型
        /// </summary>
        public Type OwnerType { get; }
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; }
        /// <summary>
        /// 属性名字
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 唯一id
        /// </summary>
        public int GlobalIndex { get; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly { get; }
        #endregion
        /// <summary>
        /// 注册一个依赖属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="propertyType">属性类型</param>
        /// <param name="ownerType">所属类类型</param>
        /// <param name="typeMetadata">注册信息</param>
        /// <param name="validateValueCallback">验证回调</param>
        /// <returns></returns>
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback=null)
        {
            var dp = new DependencyProperty(name: name,
                propertyType: propertyType,
                ownerType: ownerType,
                defaultMetadata: typeMetadata,
                validateValueCallback: validateValueCallback,readOnly:false);
            Dps.Add(dp);
            return dp;
        }
        /// <summary>
        /// 注册只读依赖属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="propertyType">属性类型</param>
        /// <param name="ownerType">所属类类型</param>
        /// <param name="typeMetadata">注册信息</param>
        /// <param name="validateValueCallback">验证回调</param>
        /// <returns></returns>
        public static DependencyProperty RegisterReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback)
        {
            var dp = new DependencyProperty(name: name,
                propertyType: propertyType,
                ownerType: ownerType,
                defaultMetadata: typeMetadata,
                validateValueCallback: validateValueCallback,readOnly:true);
            Dps.Add(dp);
            return dp;
        }
        /// <summary>
        /// 验证value是否符合
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValidValue(object value)
        {
            if (ValidateValueCallback!=null)
            {
                return ValidateValueCallback(value);
            }
            return true;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
