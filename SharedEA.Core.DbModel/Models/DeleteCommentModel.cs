using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
{
    public class DeleteCommentModel
    {
        public string ApiKey { get; set; }
        public int CommentId { get; set; }
    }
}
