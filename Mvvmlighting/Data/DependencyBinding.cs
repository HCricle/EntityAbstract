using Mvvmlighting.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 依赖绑定，被绑定者必须是依赖属性
    /// </summary>
    public class DependencyBinding : BindingBase
    {
        public DependencyBinding(object source,
            PropertyPath sourcePath,
            object target,
            PropertyPath targetPath,
            IValueConverter converter = null,
            object converterParameter = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            BindingMode model = BindingMode.OneWay) : base(source, sourcePath, target, targetPath, converter, converterParameter, updateSourceTrigger, model)
        {
            if (targetPath==null)
            {
                throw new ArgumentException($"绑定时{nameof(targetPath)}不能为空，因为单双向绑定都会使用此值");
            }
            ApplyBind();
        }
        private DependencyObject bindDp;
        public override void ApplyBind()
        {
            if (Source is DependencyObject dpo)
            {
                if (Model == BindingMode.TwoWay) 
                {
                    if (Target is DependencyObject dpot)
                    {
                        var dpotValue = TargetPath.GetValueFromSteptSub(Target, 1);//倒数第二层
                        if (dpotValue == null)
                        {
                            throw new ArgumentException("绑定目标属性为空");
                        }
                        if (dpotValue is DependencyObject dpotDpValue)
                        {
                            bindDp = dpotDpValue;
                            bindDp.DependencyPropertyChanged += Dpot_PropertyChanged;//双向绑定监视属性
                        }
                        else
                        {
                            throw new ArgumentException("绑定目标属性不为依赖属性");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"双向绑定时,{nameof(Target)}必须是DependencyObject");
                    }
                }
                
                dpo.DependencyPropertyChanged += Dpo_PropertyChanged;
            }
            else
            {
                throw new ArgumentException($"DependencyBinding应用绑定出错,属性{nameof(Source)}不为DependencyObject");
            }
            base.ApplyBind();
        }
        /// <summary>
        /// 双向通道2
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Dpot_PropertyChanged(DependencyObject arg1, DependencyPropertyChangedEventArgs arg2)
        {
            if (UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
            {
                UpdateSource();
            }
        }
        
        public override void UnApplyBind()
        {
            base.UnApplyBind();

            if (Source is DependencyObject dpo)
            {
                dpo.DependencyPropertyChanged -= Dpo_PropertyChanged;
            }
            if (Model == BindingMode.TwoWay)
            {

                bindDp.DependencyPropertyChanged -= Dpot_PropertyChanged;

            }
        }
        /// <summary>
        /// 源属性改变时候,one two way都适用
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void Dpo_PropertyChanged(DependencyObject arg1, DependencyPropertyChangedEventArgs arg2)
        {
            if (UpdateSourceTrigger== UpdateSourceTrigger.PropertyChanged)
            {
                UpdateTarget();
            }
        }
    }
}
