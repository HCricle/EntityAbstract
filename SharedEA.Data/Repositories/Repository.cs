
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 对数据库的响应基类
    /// </summary>
    public abstract class Repository<TEntity, TDbContext> : IGetable<TEntity>, IAsyncEnumerableAccessor<TEntity>, IAsyncEnumerable<TEntity>, IRepository<TEntity, TDbContext> where TEntity : class,new()
        where TDbContext:DbContext
    {
        public TDbContext DbContext { get; }//依赖注入

        public DbSet<TEntity> DbSet { get; }
        public IAsyncEnumerable<TEntity> AsyncEnumerable => DbSet.ToAsyncEnumerable();
        /// <summary>
        /// 不跟踪异步可枚举对象
        /// </summary>
        public IAsyncEnumerable<TEntity> NonTrackingAsyncEnum => DbSet.AsNoTracking().ToAsyncEnumerable();
        public Repository(TDbContext dbContext,DbSet<TEntity> dbSet)
        {
            DbContext = dbContext;
            DbSet = dbSet;
        }

        public Repository(TDbContext dbContext)
        {
            DbContext = dbContext;
            var t = dbContext.GetType();
            var dbSetProp=t.GetProperties().SingleOrDefault(prop=>prop is DbSet<TEntity>);
            if (dbSetProp == null)
            {
                throw new InvalidOperationException("没找到此DbSet");
            }
            DbSet = dbSetProp.GetValue(dbContext) as DbSet<TEntity>;
        }

        /// <summary>
        /// 自动填充
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public abstract Task AutoFillAsync(IEnumerable<TEntity> entities);
        /// <summary>
        /// 通过条件寻找,这个一调用就已经是发送查询的了
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="count">搜索的数量，如果<0就是全部</param>
        /// <param name="skip">跳过多少个</param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> DirectGetAsync(Func<TEntity, bool> func, int count = -1, int skip = 0, bool isAutoFill = false)
        {
            var qure = DbSet.AsNoTracking().Where(g => func(g)).Skip(skip);
            if (count > 0)
            {
                qure = qure.Take(count);
            }
            var res = await qure.ToListAsync();
            if (isAutoFill)
            {
                await AutoFillAsync(res);
            }
            return res;
        }
        public IAsyncEnumerator<TEntity> GetEnumerator()
        {
            return DbSet.ToAsyncEnumerable().GetEnumerator();
        }
        public static explicit operator TEntity[](Repository<TEntity,TDbContext> repository)
        {
            return repository.DbSet.ToArray();
        }
        public static explicit operator DbSet<TEntity>(Repository<TEntity, TDbContext> repository)
        {
            return repository.DbSet;
        }
        public static explicit operator TDbContext(Repository<TEntity, TDbContext> repository)
        {
            return repository.DbContext;
        }
    }
}
