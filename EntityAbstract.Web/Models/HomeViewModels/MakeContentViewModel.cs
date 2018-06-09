using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.HomeViewModels
{
    /// <summary>
    /// 发送内容
    /// </summary>
    public class MakeContentViewModel
    {
        public MakeContentViewModel()
        {
            Lable = "default";
        }

        /// <summary>
        /// 文件组，后台判断
        /// </summary>
        public List<IFormFile> Files { get; set; }
        /// <summary>
        /// 标签是 user-content
        /// </summary>
        [MaxLength(2000, ErrorMessage = "标题必须6-2000个字符")]
        public string Content { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage ="标题是必须的")]
        [StringLength(256,ErrorMessage ="标题必须6-256个字符",MinimumLength =6)]
        public string Title { get; set; }

        [Required(ErrorMessage ="标签是必须的")]
        public string Lable { get; set; }
        public List<SelectListItem> SendGroups { get; set; }
        public string TargetGroup { get; set; }
        /// <summary>
        /// 如果html存在script标签，是否将它分解为js文件
        /// </summary>
        public bool AutoJs { get; set; } = true;
    }
}
