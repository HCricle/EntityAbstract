using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 绑定基类，只能对object，INotifyPropertyChanged,DependencyObject进行绑定，继承写
    /// 绑定模式只有OneWay于TwoWay
    /// 绑定触发只有OnlyActionUpdate于PropertyChanged
    /// </summary>
    public class BindingBase
    {
        internal BindingBase(object source, PropertyPath sourcePath, object target, PropertyPath targetPath, IValueConverter converter = null, object converterParameter = null, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged, BindingMode model = BindingMode.OneWay)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            SourcePath = sourcePath;
            Target = target;
            TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
            Converter = converter;
            ConverterParameter = converterParameter;
            UpdateSourceTrigger = updateSourceTrigger;
            Model = model;
            sourcePath?.SetRelySource(source);
            targetPath?.SetRelySource(target);
            Validates = new Collection<IValidateValue>();
        }
        private object source;
        protected bool isApply;
        /// <summary>
        /// 是否已经启动绑定
        /// </summary>
        public bool IsApply => isApply;
        /// <summary>
        /// 触发条件
        /// </summary>
        public UpdateSourceTrigger UpdateSourceTrigger { get;  }
        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode Model { get;  }
        /// <summary>
        /// 绑定源，数据提供者
        /// </summary>
        public object Source => source;
        /// <summary>
        /// 绑定源数据访问路径
        /// </summary>
        public PropertyPath SourcePath { get; }
        /// <summary>
        /// 绑定目标，数据接受者
        /// </summary>
        public object Target { get; }
        /// <summary>
        /// 绑定 目标数据访问路径
        /// </summary>
        public PropertyPath TargetPath { get; }
        /// <summary>
        /// 数据转换器
        /// </summary>
        public IValueConverter Converter { get; set; }
        /// <summary>
        /// 转换器参数
        /// </summary>
        public object ConverterParameter { get; set; }
        /// <summary>
        /// 验证器
        /// </summary>
        public Collection<IValidateValue> Validates { get;  }
        /// <summary>
        /// 浅层拷贝
        /// </summary>
        /// <returns></returns>
        public virtual BindingBase Clone()
        {
            return (BindingBase)MemberwiseClone();
        }
        /// <summary>
        /// 应用绑定
        /// </summary>
        public virtual void ApplyBind()
        {
            isApply = true;
        }
        /// <summary>
        /// 取消绑定
        /// </summary>
        public virtual void UnApplyBind()
        {
            isApply = false;
        }
        /// <summary>
        /// 手动触发更新目标
        /// </summary>
        internal virtual void UpdateTarget()
        {
            //TargetPath不能为null
            var value = SourcePath.GetValue(source);
            TargetPath.SetValue(GetConvertValue(value));
        }
        /// <summary>
        /// 手动触发更新源,twoWay 通道2
        /// </summary>
        internal virtual void UpdateSource()
        {
            var newValue = TargetPath.GetValue(Target);
            if (SourcePath==null)
            {
                source = newValue;
            }
            else
            {
                SourcePath.SetValue(GetConvertValue(newValue,true));
            }
        }
        /// <summary>
        /// 获取转换后的属性
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private object GetConvertValue(object newValue, bool isback = false)
        {
            //验证属性是否合法，不合法抛出异常
            for (int i = 0; i < Validates.Count; i++)
            {
                if (!Validates[i].IsNewValueVali(newValue))
                {
                    throw new ArgumentException($"新值验证不通过 {newValue}");
                }
            }
            var value = newValue;
            if (Converter != null)//是否可以转换
            {
                if (isback)
                    value = Converter.ConvertBack(newValue, Target.GetType(), ConverterParameter);
                else
                    value = Converter.Convert(newValue, Target.GetType(), ConverterParameter);
            }
            return value;
        }
        /// <summary>
        /// 更新绑定
        /// </summary>
        internal void Update()
        {
            if (!IsApply)
            {
                throw new InvalidOperationException("在没应用绑定时不能更新");
            }
            UpdateTarget();
            if (Model == BindingMode.TwoWay)
            {
                UpdateSource();
            }
        }
    }
}
