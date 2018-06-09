using Microsoft.EntityFrameworkCore;

namespace SharedEA.Data.Repositories
{
    public interface IRepository<TEntity, TDbContext>
        where TEntity : class, new()
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
        DbSet<TEntity> DbSet { get; }
    }
}