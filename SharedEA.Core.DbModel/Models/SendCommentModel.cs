using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
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
