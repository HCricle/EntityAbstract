using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 表示可以异步获取
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGetable<TEntity>
        where TEntity:class
    {
        Task<IEnumerable<TEntity>> DirectGetAsync(Func<TEntity, bool> func, int count = -1, int skip = 0, bool isAutoFill = false);

    }
}
