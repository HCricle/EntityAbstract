using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 喜欢的帖子
    /// </summary>
    public class Like:DbModelBase
    {
        /// <summary>
        /// 喜欢的用户
        /// </summary>
        public string EaUserId { get; set; }

        /// <summary>
        /// 喜欢的内容id
        /// </summary>
        public uint ContentId { get; set; }

    }
}
