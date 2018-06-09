using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public interface IAsyncCollection<TEntity>:ICollectionModifiable<TEntity>,IAyncCollectionModifiable<TEntity>
       where TEntity : class, new()
    {
        void Add(TEntity item);

        void Remove(TEntity item);

        Task AddAsync(TEntity item, CancellationToken token = default(CancellationToken));

        Task AddRangeAsync(TEntity[] item, CancellationToken token = default(CancellationToken));

        Task RemoveRange(TEntity[] items, CancellationToken token = default(CancellationToken));

    }
}
