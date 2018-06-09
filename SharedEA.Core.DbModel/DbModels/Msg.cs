using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 消息，可以是对方给你的，可以是系统给你的
    /// 存在cd,现在只能1v1
    /// </summary>
    public class Msg: DbModelBase
    {

        public Msg()
        {
        }
        /// <summary>
        /// 发送者
        /// </summary>
        [ForeignKey("EaUserId")]
        public string Creator { get; set; }
        /// <summary>
        /// 接收者
        /// </summary>
        [ForeignKey("EaUserId")]
        public string Target { get; set; }
        /// <summary>
        /// 目标的名字
        /// </summary>
        [NotMapped]
        public string TargetName { get; set; }
        /// <summary>
        /// 消息详细
        /// </summary>
        public List<MsgDetail> Details { get; set; }
    }
}
