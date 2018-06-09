using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 每天签到
    /// </summary>
    public class PreLoad : DbModelBase
    {
        public PreLoad()
        {
        }

        public PreLoad(string eaUserId)
        {
            EaUserId = eaUserId;
            
        }

        public string EaUserId { get; set; }
    }
}
