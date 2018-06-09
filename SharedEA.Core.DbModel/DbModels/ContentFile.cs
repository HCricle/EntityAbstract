using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    public class ContentFile:DbModelBase
    {
        public ContentFile()
        {
            CanDownload = true;
        }

        public ContentFile(string path, string fileName, string originalName, string extensionName, string useType, bool canDownload=true)
        {
            Path = path;
            FileName = fileName;
            CanDownload = canDownload;
            OriginalName = originalName;
            ExtensionName = extensionName;
            UseType = useType;
        }
        [ForeignKey("ContentId")]
        public uint ContentId { get; set; }

        /// <summary>
        /// 遵从~/wwwroot/{Path}
        /// </summary>
        [MaxLength(1024)]
        public string Path { get; set; }
        /// <summary>
        /// 遵从~/wwwroot/{Path}/{FileName}
        /// </summary>
        [MaxLength(1024)]
        public string FileName { get; set; }
        /// <summary>
        /// 上传时候的名字
        /// </summary>
        [MaxLength(1024)]
        public string OriginalName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        [MaxLength(64)]
        public string ExtensionName { get; set; }
        /// <summary>
        /// 用作的类型，包含js,css,html,.........(自定义全部都要小写)
        /// </summary>
        [MaxLength(64)]
        public string UseType { get; set; }
        /// <summary>
        /// 被下载的次数
        /// </summary>
        public uint DownloadCount { get; set; }//没实现
        /// <summary>
        /// 是否可以下载,默认是可以下载
        /// </summary>
        public bool CanDownload { get; set; }
    }
}
