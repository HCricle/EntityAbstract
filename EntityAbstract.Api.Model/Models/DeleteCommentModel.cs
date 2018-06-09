using System;
using System.Collections.Generic;
using System.Text;

namespace EntityAbstract.Api.Model.Models
{
    public class DeleteCommentModel
    {
        public string ApiKey { get; set; }
        public int CommentId { get; set; }
    }
}
