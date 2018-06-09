using Microsoft.EntityFrameworkCore;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 每天签到响应
    /// </summary>
    public class PreLoadRepository : Repository<PreLoad, EaDbContext>
    {
        public PreLoadRepository(EaDbContext dbContext)
            : base(dbContext,dbContext.PreLoads)
        {
        }

        public override Task AutoFillAsync(IEnumerable<PreLoad> entities)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// 该用户是否第一天登陆
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> IsUserFirstLoadAsync(string uid)
        {
            var nt = DateTime.Now;
            var pl = await DbSet.AsNoTracking().Where(p => p.EaUserId == uid && p.CreateTime.Year == nt.Year && p.CreateTime.Month == nt.Month && p.CreateTime.Day == nt.Day)
                                               .FirstOrDefaultAsync();
            return pl != null;
        }
        /// <summary>
        /// 加入用户第一日登陆
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task AddUserFirstLoadAsync(string uid)
        {
            DbSet.Add(new PreLoad(uid));
            await DbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 获取用户已经登陆了几天
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<int> GetUserLoadDayAsync(string uid)
        {
            return await DbSet.CountAsync(p => p.EaUserId == uid && p.IsEnable);
        }
    }
}
