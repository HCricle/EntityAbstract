using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.HomeViewModels
{
    /// <summary>
    /// 评论发送模型
    /// </summary>
    public class CommentSendModel
    {
        [Required(ErrorMessage ="你还没填内容咧！")]
        [StringLength(maximumLength:1000,MinimumLength =10,ErrorMessage ="内容长度只能是10-10000")]
        public string SendText { get; set; }

        public uint ContentId { get; set; }
    }
}
