using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using Microsoft.EntityFrameworkCore;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 对于组的响应
    /// </summary>
    public class GroupRepository :Repository<ContentGroup,EaDbContext>
    {
        public GroupRepository(EaDbContext dbContext)
            : base(dbContext,dbContext.Groups)
        {
        }

        public async override Task AutoFillAsync(IEnumerable<ContentGroup> entities)
        {
            ContentGroup cg;
            for (int i = 0; i < entities.Count(); i++)
            {
                cg = entities.ElementAt(i);
                cg.Contents = await DbContext.Contents.Where(c => c.ContentGroupId == cg.Id).ToListAsync();
            }
        }
    }
}
