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
    public class LikeRepository : Repository<Like,EaDbContext>
    {
        public LikeRepository(EaDbContext dbContext)
            :base(dbContext,dbContext.Likes)
        {
        }

        /// <summary>
        /// 设置某个内容喜欢，（如果喜欢就变为非喜欢，反之）
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="contentId">内容id</param>
        /// <returns></returns>
        public async Task<bool> LikeForContentAsync(string userId,uint contentId)
        {
            var like = (await DirectGetAsync(l => l.EaUserId == userId)).FirstOrDefault();
            var hasLike = like == null;
            if (hasLike)//还没点赞
            {
                await this.AddAsync(new Like() { CreateTime = DateTime.Now, ContentId = contentId, EaUserId = userId });
            }
            else
            {
                this.Remove(like);
            }
            return !hasLike;
        }

        public override Task AutoFillAsync(IEnumerable<Like> entities)
        {
            return Task.CompletedTask;
        }
    }
}
