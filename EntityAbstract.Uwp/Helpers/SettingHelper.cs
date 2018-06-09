using EntityAbstract.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EntityAbstract.Uwp.Helpers
{
    public class SettingHelper:ISettingHelper
    {
        private ApplicationDataContainer dataContainer;
        public ApplicationDataContainer DataContainer=>dataContainer;
        public SettingHelper(string containerName,ApplicationDataCreateDisposition DispositionType= ApplicationDataCreateDisposition.Always)
        {
            dataContainer=ApplicationData.Current.LocalSettings.CreateContainer(containerName, DispositionType);
        }
        public object this[string key]
        {
            get => GetValue<object>(key);
        }
        /// <summary>
        /// 从设置中获取一个值，如果不存在返回default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            if (key!=null)
            {
                if (Exist(key))
                {
                    return (T)dataContainer.Values[key];
                }
            }
            return default(T);
        }
        /// <summary>
        /// 判断此键是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exist(string key)
        {
            return dataContainer.Values.Keys.Contains(key);
        }
        /// <summary>
        /// 从设置中设置一个值，如果存在则覆盖，不存在则生成
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key,object value)
        {
            if (Exist(key))
            {
                dataContainer.Values[key]= value;
            }
            else
            {
                dataContainer.Values.Add(key, value);
            }
        }

        public object[] GetAllValue()
        {
            return dataContainer.Values.Values.ToArray();
        }
    }
}
