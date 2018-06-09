using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Mvvmlighting.Data
{
    /// <summary>
    /// 集合寻值,只能是一位数组，其它不写了哼
    /// </summary>
    public class CollectionPropertyLayer : PropertyLayer
    {
        public CollectionPropertyLayer(string path) 
            : base(path)
        {
            if (!(path.Contains("[") && path.Contains("]")))
            {
                throw new ArgumentException($"{path} 中存在无效集合路径");
            }
            var pl = path.IndexOf("[");
            var pr = path.IndexOf("]");
            propName = path.Substring(0, pl);
            var pint = path.Substring(pl, pr - pl);
            if (!int.TryParse(path, out var res)) 
            {
                throw new ArgumentException($"{pint} 不是一个好的集合引索");
            }
            index = res;
        }
        private string propName;
        private int index;
        public override object SteptInto(object value)
        {
            ValiValue(value);
            var prop = GetProperty(value,propName);
            var propType = prop.GetType();
            var propValue = prop.GetValue(value);
            if (propValue==null)
            {
                throw new ArgumentException($"取 {propName} 得值为空");
            }
            if (propValue is IEnumerable ie)
            {
                var objs = ie.Cast<object>();
                return objs.ElementAt(index);
            }
            throw new InvalidOperationException($"取得值 {propValue} 不为IEnumerable类型");
        }
    }
}
