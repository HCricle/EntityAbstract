using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 属性路径
    /// </summary>
    public class PropertyPath
    {
        public PropertyPath(string path)
        {
            Path = path ?? throw new ArgumentException("path 不能为Null");
            Spliteds = new Collection<PropertyLayer>();
            /*
            var res=Regex.Matches(path, @"\.?(.+?)\.?");
            foreach (Match item in res)
            {
                Spliteds.Add(new ValuePropertyLayer(item.Groups[1].Value));
            }
            */
            
            var s = path.Split('.');
            string indexFlag = "[";
            foreach (var item in s)
            {
                if (item.Contains(indexFlag))//这里要改
                {
                    Spliteds.Add(new CollectionPropertyLayer(item));
                }
                else
                {
                    Spliteds.Add(new ValuePropertyLayer(item));
                }
            }
            
        }
        private object[] @params = { null };
        private MethodInfo relySetter;
        private object relySource;
        public string Path { get; }
        public Collection<PropertyLayer> Spliteds { get; }
        /// <summary>
        /// 逐层获取值
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public object GetValue(object source)
        {
            var res = source;
            foreach (var item in Spliteds)
            {
                res = item.SteptInto(res);
            }
            return res;
        }
        public void SetRelySource(object source)
        {
            relySource = source;
            relySetter = GetSetter(source);
        }
        private MethodInfo GetSetter(object source)
        {
            if (Spliteds.Count == 1)
            {
                var prop = source.GetType().GetProperty(Spliteds[0].Path);
                var setDelegate = prop.GetSetMethod(true);//??不会优化了
                return setDelegate;
            }
            if (Spliteds.Count != 0)
            {
                var res = source;
                for (int i = 0; i < Spliteds.Count - 1; i++)
                {
                    res = Spliteds[i].SteptInto(res);
                }
                var m = res.GetType().GetProperty(Spliteds[Spliteds.Count - 1].Path);
                return  m.GetSetMethod(true);
            }
            return null;
        }
        [STAThread]
        public void SetValue(object source,object value)
        {
            @params[0] = value;
            GetSetter(source)?.Invoke(source, @params);
        }
        [STAThread]
        public void SetValue(object value)
        {
            if (relySetter == null)
            {
                throw new InvalidOperationException("在调用此重载方法时，必须调用SetRelySource设置依赖源");
            }
            @params[0] = value;
            relySetter.Invoke(relySetter, @params);
        }
        /// <summary>
        /// 走层
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="subStept">走到倒数第几层</param>
        public object GetValueFromSteptSub(object source, int subStept)
        {
            var count = Spliteds.Count - subStept;
            if (count<0)
            {
                throw new ArgumentException($"最大层数为{Spliteds.Count}，超过了最大层数");
            }
            if (count == 1)
            {
                var prop = source.GetType().GetProperty(Spliteds[0].Path);
                return prop.GetValue(source);
            }
            else
            {
                var res = source;
                for (int i = 0; i < Spliteds.Count - 1; i++)
                {
                    res = Spliteds[i].SteptInto(res);
                }
                return res.GetType().GetProperty(Spliteds[Spliteds.Count - 1].Path).GetValue(res);
            }
        }
    }
}
