using SharedEA.Core.Data.Helpers;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.HomeViewModels
{
    public class ContentDetailViewModel
    {
        public ContentDetailViewModel()
        {
            PrePageCount = 6;
        }

        public ContentDetailViewModel(Content content)
            :this()
        {
            Content = content;
            JsFile = content.ContentFiles.Where(file => file.UseType == ContentParseSettings.JsUseType).ToArray();
            CssFile = content.ContentFiles.Where(file => file.UseType == ContentParseSettings.CssUseType).ToArray();
            HtmlFile = content.ContentFiles.Where(file => file.UseType == ContentParseSettings.HtmlUseType).ToArray();
            CanDownloadFile = content.ContentFiles
                    .Where(file => file.CanDownload&&file.CanDownload)
                    .ToList();
            CanDownloadFile.Except(JsFile);
            CanDownloadFile.Except(CssFile);
            CanDownloadFile.Except(HtmlFile);
            Lables = content.Lable.Split("_");
        }
        public EaUser Sender { get; set; }
        public Content Content { get; set; }
        //搜出js,css,html
        public IEnumerable<RelComment> Comments { get; set; }
        public CommentSendModel SendModel { get; set; }
        public IEnumerable<ContentFile> JsFile { get; }
        public IEnumerable<ContentFile> CssFile { get; }
        public IEnumerable<ContentFile> HtmlFile { get; }
        public bool HasJsFile => JsFile != null && JsFile.Count() > 0;
        public bool HasCssFile => CssFile != null && CssFile.Count() > 0;
        public bool HasHtmlFile => HtmlFile != null && HtmlFile.Count() > 0;
        public string[] Lables { get; set; }
        public bool HasLable => Lables != null && Lables.Count() > 0;
        /// <summary>
        /// 会剔除js，css，html的
        /// </summary>
        public IEnumerable<ContentFile> CanDownloadFile { get;  }
        public bool HasCanDownloadFile=> CanDownloadFile != null && CanDownloadFile.Count() > 0;
        public int PrePageCount { get; set; }
        /// <summary>
        /// 当前页,从0开始
        /// </summary>
        public int LocPage { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 是否启用js代码
        /// </summary>
        public bool UseJs { get; set; }
        public int PrevPage => LocPage - 1;
        public int NextPage => LocPage + 1;
        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevPage => LocPage > 0;
        public bool HasNextPage => TotalCount - (LocPage + 1) * PrePageCount > 0;
        public bool IsLike { get; set; }
    }
    public class CommentModel
    {
        public CommentModel(Comment comment,string sendUserName, bool isLocUser=false)
        {
            SendUserName = sendUserName;
            Comment = comment;
            IsLocUser = isLocUser;
        }
        /// <summary>
        /// 是否是当前用户发的
        /// </summary>
        public bool IsLocUser { get;  }
        public Comment Comment { get; }
        public string SendUserName { get;  }
    }
}
