using System;
using System.Collections.Generic;
using System.Text;

namespace EntityAbstract.Api.Model.Models
{
    public class SendCommentModel
    {
        public SendCommentModel()
        {
        }

        public uint ContentId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string StringContent { get; set; }
    
    }
}
