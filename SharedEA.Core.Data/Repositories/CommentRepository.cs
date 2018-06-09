using Microsoft.EntityFrameworkCore;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.DbContext;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 对回贴的反应
    /// </summary>
    public class CommentRepository : Repository<Comment,EaDbContext>
    {
        public CommentRepository(EaDbContext dbContext)
            :base(dbContext,dbContext.Comments)
        {
        }
        public async Task<string> SendCommentAsync(string userId,uint contentId,string sendText)
        {
            if (await DbSet.Where(c =>c.IsEnable&& c.EaUserId == userId && c.ContentId == contentId).CountAsync() > 0) 
            {
                return "一个帖子不能重复发送";
            }
            var lowText = sendText.ToLower();
            if (lowText.Contains("<script")|| lowText.Contains("<object")|| lowText.Contains("<iframe"))
            {
                return "存在危险标签";
            }
            var comment = new Comment(sendText) { ContentId = contentId, EaUserId = userId };
            await this.AddAsync(comment);
            return null;
        }
        public async Task<bool> DeleteCommentAsync(string userId,uint commentId)
        {
            var comment = await DbSet.SingleOrDefaultAsync(c =>c.IsEnable&& c.Id == commentId && c.EaUserId == userId);
            if (comment == null)
            {
                return false;
            }
            comment.IsEnable = false;
            DbSet.Update(comment);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public override Task AutoFillAsync(IEnumerable<Comment> entities)
        {
            return Task.CompletedTask;
        }
    }
}

