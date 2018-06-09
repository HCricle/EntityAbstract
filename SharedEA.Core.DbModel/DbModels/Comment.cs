using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{

    /// <summary>
    /// 评论
    /// </summary>
    public class Comment:DbModelBase
    {
        public Comment()
        {
            
        }

        public Comment(string htmlContent)
        {
            HtmlContent = htmlContent;
            CreateTime = DateTime.Now;
        }

        public uint ContentId { get; set; }
        /// <summary>
        /// 发送的用户Id
        /// </summary>
        public string EaUserId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string HtmlContent { get; set; }
    }
}
