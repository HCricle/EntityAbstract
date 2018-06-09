using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 集合绑定,只能oneWay
    /// </summary>
    public class CollectionBinding : BindingBase
    {
        public CollectionBinding(object source, PropertyPath sourcePath, object target, PropertyPath targetPath, IValueConverter converter = null, object converterParameter = null, BindingMode model = BindingMode.OneWay)
            : base(source, sourcePath, target, targetPath, converter, converterParameter,  UpdateSourceTrigger.OnlyActionUpdate, model)
        {
            
        }
        /// <summary>
        /// 每一条数据的动作
        /// 在集合改变时候，会调用此动作
        /// </summary>
        public Action<object> PreItemAction { get; set; }
        /// <summary>
        /// 集合改变了，通常不用
        /// </summary>
        public event Action<CollectionBinding, NotifyCollectionChangedEventArgs> CollectionChanged;
        public override void ApplyBind()
        {
            if (Source is INotifyCollectionChanged incc) 
            {
                if (!(Source is IEnumerable))
                {
                    throw new ArgumentException("Source 必须是可枚举类型");
                }
                incc.CollectionChanged += Incc_CollectionChanged;
            }
            else
            {
                throw new ArgumentException("绑定集合时，Source必须实现了INotifyCollectionChanged");
            }
            base.ApplyBind();
        }
        internal override void UpdateSource()
        {
            throw new InvalidOperationException("在集合绑定时，无法更新Source");
        }
        public override void UnApplyBind()
        {
            base.UnApplyBind();
            if (Source is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged -= Incc_CollectionChanged;
            }
        }
        private void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PreItemAction!=null)
            {
                if (Source is IEnumerable ien)
                {
                    var enter = ien.GetEnumerator();
                    while (enter.MoveNext())
                    {
                        PreItemAction(enter);
                    }
                }
            }
            CollectionChanged?.Invoke(this, e);
        }
    }
}
