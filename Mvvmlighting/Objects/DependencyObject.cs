using Mvvmlighting.Objects.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvvmlighting.Objects
{
    /// <summary>
    /// 依赖属性
    /// </summary>
    public abstract class DependencyObject : DispatcherObject
    {
        private Dictionary<int,object> Properties { get; }
        public DependencyObject()
        {
            Properties = new Dictionary<int, object>();
        }
        /// <summary>
        /// 依赖属性改变了
        /// </summary>
        internal event Action<DependencyObject, DependencyPropertyChangedEventArgs> DependencyPropertyChanged;
        /// <summary>
        /// 获取值，如果值不存在返回默认值
        /// </summary>
        /// <param name="dp">依赖属性</param>
        /// <returns></returns>
        public object GetValue(DependencyProperty dp)
        {
            if (dp==null)
            {
                throw new ArgumentNullException(nameof(dp));
            }
            object value;
            if (Properties.ContainsKey(dp.GlobalIndex))
            {
                value = Properties[dp.GlobalIndex];
            }
            else
            {
                value = dp.DefaultMetadata.DefaultValue.Invoke();//获取默认值
            }
            if (dp.DefaultMetadata.CoerceValueCallback!=null)
            {
                value = dp.DefaultMetadata.CoerceValueCallback(this, value);
            }
            return value;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="dp">依赖属性</param>
        /// <param name="value">值</param>
        public void SetValue(DependencyProperty dp,object value)
        {
            if (dp.ReadOnly)//如果只读
            {
                throw new InvalidOperationException("此属性是只读");
            }
            var oldValue = GetValue(dp);//旧值，或者默认值
            if (oldValue.Equals(value))//如果一样就不设置了
            {
                return;
            }
            if (dp.ValidateValueCallback!=null)
            {
                if (!dp.ValidateValueCallback(value))//验证失败
                {
                    throw new ArgumentException("新属性value验证失败");
                }
            }
            if (Properties.ContainsKey(dp.GlobalIndex))
            {
                Properties[dp.GlobalIndex] = value;
            }
            else
            {
                Properties.Add(dp.GlobalIndex, value);
            }
            var args = new DependencyPropertyChangedEventArgs(dp, oldValue, value);
            dp.DefaultMetadata.PropertyChangedCallback?.Invoke(this, args);
            OnPropertyChanged(args);
            DependencyPropertyChanged?.Invoke(this,args);
        }
        /// <summary>
        /// 是否包含此属性
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool ContrainProperty(DependencyProperty dp)
        {
            return Properties.Keys.Contains(dp.GlobalIndex);
        }
        /// <summary>
        /// 重新计算值
        /// </summary>
        /// <param name="dp">依赖属性</param>
        public void InvalidateProperty(DependencyProperty dp)
        {
            var value = GetValue(dp);
            var res=dp.DefaultMetadata.CoerceValueCallback?.Invoke(this, value);
            if (res!=null)
            {
                SetValue(dp, res);
            }
        }
        /// <summary>
        /// 读本地值，如果不存在返回DependencyProperty.UnsetValue
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public object ReadLocalValue(DependencyProperty dp)
        {
            if (Properties.ContainsKey(dp.GlobalIndex))
            {
                return Properties[dp.GlobalIndex];
            }
            return DependencyProperty.UnsetValue;
        }
        /// <summary>
        /// 清除一个值
        /// </summary>
        /// <param name="dp"></param>
        public void CleanValue(DependencyProperty dp)
        {
            if (Properties.ContainsKey(dp.GlobalIndex))
            {
                Properties.Remove(dp.GlobalIndex);
            }
        }
        /// <summary>
        /// 重新转换属性
        /// </summary>
        /// <param name="dp"></param>
        public void CoerceValue(DependencyProperty dp)
        {
            var oldValue = GetValue(dp);
            if (oldValue!=null)
            {
                dp.DefaultMetadata.CoerceValueCallback?.Invoke(this, oldValue);
            }
        }
        /// <summary>
        /// 属性改变回调
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        public override int GetHashCode()
        {
            return Properties.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == Properties.GetHashCode();
        }
        /// <summary>
        /// 将2个依赖属性相加，如果存在相同实例属性，则将值2代替值1，如果不存在则新增属性，
        /// 注意，设置新建属性时，不会调用setvalue
        /// </summary>
        /// <param name="do1">被操作者</param>
        /// <param name="do2">源</param>
        /// <returns></returns>
        public static DependencyObject operator+(DependencyObject do1,DependencyObject do2)
        {
            foreach (var item in do2.Properties)
            {
                if (do1.Properties.ContainsKey(item.Key))
                {
                    do1.Properties[item.Key] = item.Value;
                }
                else
                {
                    do1.Properties.Add(item.Key, item.Value);
                }
            }
            return do1;
        }
    }
}
