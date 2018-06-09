using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Data.Repositories
{
    public class ContentFileRepository : Repository<ContentFile,EaDbContext>
    {
        public ContentFileRepository(EaDbContext dbContext)
            : base(dbContext,dbContext.ContentFiles)
        {
        }
        /// <summary>
        /// 加入一个文件数据,不包含复制文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task AddFile(ContentFile file)
        {
            if (file==null)
            {
                throw new ArgumentException("加入文件时参数不能为null");
            }
            await this.AddAsync(file);
        }

        public override Task AutoFillAsync(IEnumerable<ContentFile> entities)
        {
            return Task.CompletedTask;
        }
    }
}
