using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 消息详细响应
    /// </summary>
    public class MsgDetailRepository : Repository<MsgDetail, EaDbContext>
    {
        private readonly ACmdRepository _aCmdRepository;
        public MsgDetailRepository(EaDbContext dbContext,ACmdRepository aCmdRepository)
            : base(dbContext,dbContext.MsgDetails)
        {
            _aCmdRepository = aCmdRepository;
        }

        public override Task AutoFillAsync(IEnumerable<MsgDetail> entities)
        {
            return Task.CompletedTask;
        }
    }
}
