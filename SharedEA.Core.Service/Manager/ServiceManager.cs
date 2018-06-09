using SharedEA.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedEA.Server.Manager
{
    /// <summary>
    /// 服务管理器，一个类型只能存在一个实例 ioc
    /// </summary>
    public class ServiceManager
    {
        private Dictionary<Type, object> Services;
        public ServiceManager()
        {
            Services = new Dictionary<Type, object>();
        }
        public ServiceManager AddService<TService>(params object[] args)//加入后就有一个实例了
            where TService : IService
        {
            var t = typeof(TService);
            if (Services.Keys.Where(k => k.FullName == t.FullName).Count() > 0) 
            {
                throw new ArgumentException($"{t.Name} 该服务已存在，请勿重复添加");
            }
            Services.Add(t, Activator.CreateInstance(t, args));
            return this;
        }
        public TService GetService<TService>()
            where TService:IService
        {
            var t = typeof(TService).FullName;
            var ser = Services.Where(s => s.Key.FullName == t).FirstOrDefault().Value;
            if (ser==null)
            {
                throw new ArgumentException($"{t} 该服务不存在");
            }
            return (TService)ser;
        }
        public IEnumerable<IService> GetServices()
        {
            return Services.Select(s=>s.Value).Cast<IService>();
        }
        public void RemoveService(IService service)
        {
            var s = Services.Where(ser => (ser.Value == service)).FirstOrDefault();
            if (s.Key == null || s.Value == null)  
            {
                throw new ArgumentException("服务移除失败");
            }
            Services.Remove(s.Key);
        }
        public void CleanService()
        {
            Services.Clear();
        }
    }
}
