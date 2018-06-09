using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedEA.Core.Data.Helpers
{
    public static class ContentParseSettings
    {
        /// <summary>
        /// 是否所有文件都可以上传
        /// </summary>
        public static bool IsAllFileCanAdd { get; set; } = true;
        public static long Cd { get; set; } = 60 * 10;
        public static int MaxUploadFileCount { get; set; } = 5;
        public static uint DescriptLength { get; set; } = 35;
        public static string[] HtmlSavePath { get; set; } = { "htmls", "contents" };
        public static string[] CssSavePath { get; set; } = { "css", "contents" };
        public static string[] JsSavePath { get; set; } = { "js", "contents" };
        public static string[] OtherSavePath { get; set; } = { "others" };
        public static string RootPath { get; set; } = "wwwroot";
        public static string RootNode { get; set; } = "user-content";
        public static readonly string JsUseType = "js";
        public static readonly string CssUseType = "css";
        public static readonly string HtmlUseType = "html";
        /// <summary>
        /// 合并文件目录
        /// </summary>
        /// <param name="type">类型 可以是js,css,htmls,images</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FileFormat(params string[] paths)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), RootPath, Path.Combine(paths));
        }

    }
}
