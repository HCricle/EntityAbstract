using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    /// <summary>
    /// 内容响应返回模型
    /// </summary>
    public class ContentRepositoryModel : ArrayRepository<Content>
    {
        public ContentRepositoryModel()
        {
        }

        public ContentRepositoryModel(bool ok) : base(ok)
        {
        }

        public ContentRepositoryModel(int totalCount, int skip, int take, params Content[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
