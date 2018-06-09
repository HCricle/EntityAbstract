using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 对象绑定时，只能手动触发更新数据
    /// </summary>
    public class ObjectBinding : BindingBase
    {
        public ObjectBinding(object source, PropertyPath sourcePath, object target, PropertyPath targetPath, IValueConverter converter = null, object converterParameter = null, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged, BindingMode model = BindingMode.OneWay) : base(source, sourcePath, target, targetPath, converter, converterParameter, updateSourceTrigger, model)
        {
            ApplyBind();
        }

        public override void ApplyBind()
        {
            base.ApplyBind();
        }
        public override void UnApplyBind()
        {
            base.UnApplyBind();
        }
        /// <summary>
        /// 手动更新
        /// </summary>
        public new void Update()
        {
            base.Update();
        }
    }
}
