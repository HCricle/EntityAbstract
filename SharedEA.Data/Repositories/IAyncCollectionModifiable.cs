using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 表示这个集合可以异步修改
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IAyncCollectionModifiable<TEntity>
        where TEntity:class,new()
    {
        Task<bool> UpdateAsync(TEntity item, CancellationToken token = default(CancellationToken));
    }
}
