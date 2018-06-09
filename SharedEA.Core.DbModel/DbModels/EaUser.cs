using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 用户
    /// </summary>
    public class EaUser : IdentityUser
    {
        /// <summary>
        /// 头像目录部分~/wwwroot/{HeadImgPath}
        /// </summary>
        public static readonly string HeadImgPath = "heads";
        /// <summary>
        /// 用户类型
        /// </summary>
        [StringLength(16)]
        public string Type { get; set; }
        /// <summary>
        /// 头像路径 ~/wwwroot/heads/{HImg}
        /// </summary>
        [StringLength(128)]
        public string HImg { get; set; }
        /// <summary>
        /// 等级，现在不启用
        /// </summary>
        public ushort Level { get; set; }
        /// <summary>
        /// 钱
        /// </summary>
        public int Money { get; set; }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 用户发送的内容
        /// </summary>
        public List<Content> SendedContents { get; set; }
        /// <summary>
        /// 用户评论的集合
        /// </summary>
        public List<Comment> SendedComments { get; set; }

    }
}
