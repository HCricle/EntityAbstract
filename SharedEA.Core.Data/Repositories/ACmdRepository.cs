using Microsoft.EntityFrameworkCore;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public class ACmdRepository : Repository<ACmd, EaDbContext>
    {
        public ACmdRepository(EaDbContext dbContext)
            : base(dbContext,dbContext.Commands)
        {
        }

        public override Task AutoFillAsync(IEnumerable<ACmd> entities)
        {
            return Task.CompletedTask;
        }
    }
}
