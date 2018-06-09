using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 好友
    /// </summary>
    public class Friend : DbModelBase
    {
        /// <summary>
        /// 正在申请
        /// </summary>
        [NotMapped]
        public static readonly short Applying = 0;
        /// <summary>
        /// 已经同意
        /// </summary>
        [NotMapped]
        public static readonly short Applied = 1;
        public Friend()
        {
        }
        [ForeignKey("EaUserId")]
        public string Creator { get; set; }
        [ForeignKey("EaUserId")]
        public string Target { get; set; }

        /// <summary>
        /// 当前状态 默认0 申请中,1已是朋友,IsEnable=0就是被删除了
        /// </summary>
        public short Status { get; set; }
    }
}
