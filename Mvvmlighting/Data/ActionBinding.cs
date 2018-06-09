using Mvvmlighting.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 这是针对安卓ios的,Target要是DependencyObject
    /// </summary>
    public class ActionBinding : BindingBase
    {
        public ActionBinding(object source,
            PropertyPath sourcePath, 
            object target, 
            PropertyPath targetPath,
            Action<object,DependencyPropertyChangedEventArgs> sourceChangedHandle,
            Action<object,DependencyPropertyChangedEventArgs> targetChangedHandle,
            IValueConverter converter = null,
            object converterParameter = null, 
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged, 
            BindingMode model = BindingMode.OneWay) 
            : base(source, sourcePath, target, targetPath, converter, converterParameter, updateSourceTrigger, model)
        {
            if (!target.GetType().IsSubclassOf(typeof(DependencyObject)))
            {
                throw new ArgumentException("target必须继承于DependencyObject");
            }
            SourceChangedHandle = sourceChangedHandle;
            TargetChangedHandle = targetChangedHandle;
            ApplyBind();
        }
        /// <summary>
        /// 源改变的处理 object=target
        /// </summary>
        public Action<object, DependencyPropertyChangedEventArgs> SourceChangedHandle { get; set; }
        /// <summary>
        /// 绑定目标改变的处理
        /// </summary>
        public Action<object, DependencyPropertyChangedEventArgs> TargetChangedHandle { get; set; }
        private DependencyObject bindDp;

        public override void ApplyBind()
        {
            if (Target is DependencyObject dpo)
            {
                dpo.DependencyPropertyChanged += Dpo_DependencyPropertyChanged;
                if (Model==  BindingMode.TwoWay)
                {
                    var dpotValue = TargetPath.GetValueFromSteptSub(Target, 1);//倒数第二层
                    if (dpotValue == null)
                    {
                        throw new ArgumentException("绑定目标属性为空");
                    }
                    if (dpotValue is DependencyObject dpotDpValue)
                    {
                        bindDp = dpotDpValue;
                        bindDp.DependencyPropertyChanged +=Sdpo_DependencyPropertyChanged;//双向绑定监视属性
                    }

                }
            }
            else
            {
                throw new ArgumentException("Target必须要继承于DependencyObject");
            }
            base.ApplyBind();
        }

        

        public override void UnApplyBind()
        {
            base.UnApplyBind();
            if (Target is DependencyObject dpo)
            {
                dpo.DependencyPropertyChanged -= Dpo_DependencyPropertyChanged;
            }
            if (Model == BindingMode.TwoWay)
            {

                bindDp.DependencyPropertyChanged -= Sdpo_DependencyPropertyChanged;

            }
        }
        private void Sdpo_DependencyPropertyChanged(DependencyObject arg1, DependencyPropertyChangedEventArgs arg2)
        {
            if (UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
            {
                SourceChangedHandle?.Invoke(Source, arg2);
            }
        }
        private void Dpo_DependencyPropertyChanged(DependencyObject arg1, DependencyPropertyChangedEventArgs arg2)
        {
            if (UpdateSourceTrigger== UpdateSourceTrigger.PropertyChanged)
            {
                TargetChangedHandle?.Invoke(Target, arg2);
            }
        }
    }
}
