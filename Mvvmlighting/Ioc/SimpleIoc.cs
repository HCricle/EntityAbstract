using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mvvmlighting.Ioc
{
    /// <summary>
    /// Ioc控制反转
    /// </summary>
    public class SimpleIoc
    {
        private static SimpleIoc inst;
        public static SimpleIoc Inst => inst ?? (inst=new SimpleIoc());

        private Dictionary<object, object> Insts;
        /// <summary>
        /// 实例个数
        /// </summary>
        public int InstCount => Insts.Count;
        private SimpleIoc()
        {
            Insts = new Dictionary<object, object>();
        }
        public object this[object key]
        {
            get
            {
                return GetInstance<object>(key);
            }
        }
        /// <summary>
        /// 注册一个类
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="key">获取实例的键值</param>
        /// <param name="param">创建类的参数</param>
        /// <returns>成功返回的(键,值)</returns>
        public T RegisterClass<T>(object key,params object[] param)
            where T:class
        {
            EnsureNoneKey(key);   
            var res = Activator.CreateInstance(typeof(T), param);
            Insts.Add(key, res);
            return (T)res;
        }
        private void EnsureNoneKey(object key)
        {
            if (Insts.ContainsKey(key))
            {
                throw new ArgumentException($"{key}-该键已存在");
            }
        }
        /// <summary>
        /// 查询是否已存在此类型实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ContrainInst<T>()
        {
            return Insts.Values.Any(v => v is T);
        }
        public bool ContrainInst(Type type)
        {
            return Insts.Values.Any(v => v.GetType().IsInstanceOfType(type));
        }
        /// <summary>
        /// 添加一个已存在实例
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inst"></param>
        public void AddInstance(object key,object inst)
        {
            EnsureNoneKey(key);
            Insts.Add(key, inst);
        }
        /// <summary>
        /// 移除一个实例
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>移除是否成功</returns>
        public bool RemoveClass(object key)
        {
            if (!ContainsKey(key))
            {
                return false;
            }
            Insts.Remove(key);
            return true;
        }
        /// <summary>
        /// 该键值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(object key)
        {
            return Insts.ContainsKey(key);
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetInstance<T>(object key)
            where T:class
        {
            if (!ContainsKey(key))
            {
                throw new ArgumentException($"{key}-该键不存在");
            }
            return (T)Insts[key];
        }
        /// <summary>
        /// 清除所有实例
        /// </summary>
        public void CleanClass()
        {
            Insts.Clear();
        }
        
    }
}
