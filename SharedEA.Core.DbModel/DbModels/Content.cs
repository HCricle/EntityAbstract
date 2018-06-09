using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 内容，类似帖子的内容
    /// </summary>
    public class Content:DbModelBase
    {
        public static readonly string JsFileType = "js";
        public static readonly string CssFileType = "css";
        public static readonly string HtmlFileType = "html";
        public Content()
        {
        }

        public Content( string type, string title, ushort level, string lable,params ContentFile[] files)
            :this()
        {
            CreateTime = DateTime.Now;
            Type = type;
            Title = title;
            Level = level;
            Lable = lable;
            ContentFiles = new List<ContentFile>();
            if (files!=null)
            {

                foreach (var file in files)
                {
                    ContentFiles.Add(file);
                }
            }
        }
        [NotMapped]
        private List<ContentFile> jsFile;
        [NotMapped]
        private List<ContentFile> cssFile;
        [NotMapped]
        private List<ContentFile> htmlFile;
        public uint ContentGroupId { get; set; }
        /// <summary>
        /// 发送的用户Id
        /// </summary>
        public string EaUserId { get; set; }//名字全部改为id
        /// <summary>
        /// 内容的类型，可以是漫画，游戏，等等(对于现在，主打漫画)
        /// </summary>
        [StringLength(40)]
        public string Type { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(512)]
        public string Title { get; set; }
        /// <summary>
        /// 内容等级，就是说重视等级
        /// </summary>
        public ushort Level { get; set; }
        /// <summary>
        /// 组标签，以_分割
        /// </summary>
        [StringLength(100)]
        public string Lable { get; set; }

        [NotMapped]
        public IReadOnlyCollection<ContentFile> JsFile => jsFile;
        [NotMapped]
        public IReadOnlyCollection<ContentFile> CssFile => cssFile;
        [NotMapped]
        public IReadOnlyCollection<ContentFile> HtmlFile => htmlFile;

        [NotMapped]
        public bool HasJsFile =>JsFile!=null&& JsFile.Count > 0;
        [NotMapped]
        public bool HasCssFile => CssFile != null && CssFile.Count > 0;
        [NotMapped]
        public bool HasHtmlFile => HtmlFile != null && HtmlFile.Count > 0;
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(100)]
        public string Description { get; set; }
        /// <summary>
        /// 主题权力，包含可否转载，可否下载
        /// </summary>
        public uint PowerType { get; set; }

        /// <summary>
        /// 所有评论
        /// </summary>
        public List<Comment> Comments { get; set; }
        /// <summary>
        /// 包含文件，包含js,html,css
        /// </summary>
        public List<ContentFile> ContentFiles{ get; set; }
        /// <summary>
        /// 加载特别的文件，从ContentFiles,js,css,html
        /// </summary>
        public void LoadSpecialFile()
        {
            jsFile = ContentFiles.Where(f => f.UseType == JsFileType).ToList();
            cssFile = ContentFiles.Where(f => f.UseType == CssFileType).ToList();
            htmlFile = ContentFiles.Where(f => f.UseType == HtmlFileType).ToList();
        }

    }
}
