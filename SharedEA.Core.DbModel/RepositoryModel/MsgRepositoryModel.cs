using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    /// <summary>
    /// 对消息响应的返回模型
    /// </summary>
    public class MsgRepositoryModel : ArrayRepository<Msg>
    {
        public MsgRepositoryModel()
        {
        }

        public MsgRepositoryModel(int totalCount, int skip, int take, params Msg[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
