using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 表示这个集合可以修改
    /// </summary>
    public interface ICollectionModifiable<TEntity>
        where TEntity:class,new()
    {
        bool Update(TEntity item);
    }
}
