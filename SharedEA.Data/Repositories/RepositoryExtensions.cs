using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public static class RepositoryExtensions
    {
        public static async Task<int> GetCountAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, Func<TEntity, bool> func, int count = -1, int skip = 0)
                    where TEntity : class, new()
            where TDbContext : DbContext
        {
            return await repository.DbSet.CountAsync();
        }
        public static async Task<int> GetCountAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository)
                    where TEntity : class, new()
            where TDbContext : DbContext
        {
            return await repository.DbSet.CountAsync();
        }
        public static async Task<bool> AnyAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, Func<TEntity, bool> func)
            where TEntity : class, new()
            where TDbContext : DbContext
        {
            return await repository.DbSet.AnyAsync((f) => func(f));
        }
        public static async Task AddAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity item, CancellationToken token = default(CancellationToken))
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            await repository.DbSet.AddAsync(item, token);
            repository.DbContext.SaveChanges();
        }

        public static void Add<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity item)
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            repository.DbSet.Add(item);
            repository.DbContext.SaveChanges();
        }

        public static void Remove<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity item)
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            repository.DbSet.Remove(item);
            repository.DbContext.SaveChanges();
        }


        public static bool Update<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity item)
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            repository.DbSet.Update(item);
            return repository.DbContext.SaveChanges() > 0;
        }

        public static async Task<bool> UpdateAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity item, CancellationToken token = default(CancellationToken))
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            repository.DbSet.Update(item);
            return await repository.DbContext.SaveChangesAsync() > 0;
        }

        public static async Task AddRangeAsync<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity[] item, CancellationToken token = default(CancellationToken))
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            await repository.DbSet.AddRangeAsync(item);
            repository.DbContext.SaveChanges();
        }

        public static async Task RemoveRange<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, TEntity[] items, CancellationToken token = default(CancellationToken))
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            repository.DbSet.RemoveRange(items);
            await repository.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 通过条件分页寻找
        /// </summary>
        /// <param name="func"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Quire<TEntity, TDbContext>(this Repository<TEntity, TDbContext> repository, Func<TEntity, bool> func, int count = -1, int skip = 0)
             where TEntity : class, new()
             where TDbContext : DbContext
        {
            var qure = repository.DbSet.AsNoTracking().Where(g => func(g)).Skip(skip);
            if (count > 0)
            {
                qure = qure.Take(count);
            }
            return qure;
        }
    }
}
