using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SharedEA.Core.Data.Helpers;
using SharedEA.Core.Data.Models;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharedEA.Data.Helpers
{
    public static class FileParseHelper
    {
        private static readonly string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private static Dictionary<string, Func<ParsedDataSource,Task>> parser;
        private static Dictionary<string, string> fileUseTypeDic;
        static FileParseHelper()
        {
            parser = new Dictionary<string, Func<ParsedDataSource, Task>>
            {
                { "png", ParseImgAsync },//所有图片格式都要
                {"jpg",ParseImgAsync },
                {"jpeg",ParseImgAsync },
                {"bmp",ParseImgAsync }
            };
            fileUseTypeDic = new Dictionary<string, string>
            {
                {"zip","compress" },
                {"png","img" },
                {"jpg","img" },
                {"jpeg","img" },
                {"bmp","img" },
            };
        }
        /// <summary>
        /// 获取文件使用类型，如果字典没有返回{extensionName}
        /// </summary>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        public static string GetUseType(string extensionName)
        {
            if (fileUseTypeDic.Keys.Contains(extensionName))
            {
                return fileUseTypeDic[extensionName];
            }
            return extensionName;
        }
        class ParsedDataSource
        {
            public ParsedDataSource(FormFileModel file, XmlDocument xmlDoc, XmlNode node)
            {
                File = file;
                XmlDoc = xmlDoc;
                Node = node;
            }

            public FormFileModel File { get; }
            public XmlDocument XmlDoc { get; }
            public XmlNode Node { get; }
        }
        private async static Task ParseImgAsync(ParsedDataSource source)
        {
            var copyResual= await CopyFileAsync(source.File,"images","contents");
            var ele = source.XmlDoc.CreateElement("img");
            ele.SetAttribute("src", $"~/images/contents/{copyResual}");
            ele.SetAttribute("class", "img-content-them row");
            ele.SetAttribute("type", "img");
            source.Node.AppendChild(ele);
            //解析图片
            //返回就是一行,其中要包含属性type
        }
        /// <summary>
        /// 将文件复制到~/wwwroot/{path[0]}/../{path[n]}/{Guid.NewGuid()}.file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns>返回文件名</returns>
        private static async Task<string> CopyFileAsync(FormFileModel file,params string[] path)
        {
            var p = new List<string>(path.Count() + 2) { wwwrootPath };
            var name = Guid.NewGuid() + "." + file.ExtensionName;
            foreach (var item in path)
            {
                p.Add(item);
            }
            p.Add(name);
            using (var f = File.Create(Path.Combine(p.ToArray())))
            {
                await file.File.CopyToAsync(f);  
            }
            return name;
        }
        private static string GetFileExternName(IFormFile file)
        {
            return file.FileName.Split('.').Last();
        }
        public static bool CanParsed(IFormFile file)
        {
            return parser.Keys.Contains(GetFileExternName(file).ToLower());
        }
        /// <summary>
        /// 解析文件组为xml
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="node"></param>
        /// <param name="files"></param>
        /// <param name="uploadFailCallBack"></param>
        /// <returns>解析成功的文件</returns>
        public static async Task<IEnumerable<FormFileModel>> ParseAsync(XmlDocument xmldoc,XmlElement node,IEnumerable<FormFileModel> files)
        {
            FormFileModel item;
            var okFile = files.Where(f => CanParsed(f.File));
            for (int i = 0; i < okFile.Count(); i++)
            {
                item = okFile.ElementAt(i);
                var name = Guid.NewGuid().ToString();
                item.CanDownload = false;
                await parser[GetFileExternName(item.File).ToLower()].Invoke(new ParsedDataSource(item, xmldoc, node));
            }
            //加入节项
            return okFile;
        }
        /// <summary>
        /// 判断是否有script标签
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool HasDangrageCore(IFormFile file)
        {
            var res = new List<ContentFile>();
            
            var xmlDoc = new HtmlDocument();
            try
            {
                using (var s = file.OpenReadStream())
                {
                    xmlDoc.Load(s);
                }
                return xmlDoc.DocumentNode.SelectNodes("/script")!=null|| 
                    xmlDoc.DocumentNode.SelectNodes("/object")!=null|| 
                    xmlDoc.DocumentNode.SelectNodes("/iframe")!=null;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> SplitHtmlAsync(FormFileModel file,List<ContentFile> files,uint cid)
        {
            if (files==null)
            {
                return false;
            }
            var xmlDoc = new HtmlDocument();
            Stream[] scriptStreams=null;
            try
            {
                using (var s = file.File.OpenReadStream())
                {
                    xmlDoc.Load(s);
                }
                var nodes = xmlDoc.DocumentNode.SelectNodes("script");
                if (nodes!=null)
                {
                    scriptStreams = new Stream[nodes.Count];
                    string jsPath = Path.Combine(ContentParseSettings.JsSavePath);
                    string name = null;
                    string path = null;
                    HtmlNode node;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        node = nodes[i];
                        name = Guid.NewGuid().ToString() + "."+ContentParseSettings.JsUseType;
                        path = ContentParseSettings.FileFormat(jsPath, name);
                        using (var stream=File.Create(path))
                        {
                            using (var swriter=new StreamWriter(stream))
                            {
                                await swriter.WriteAsync(node.InnerText);
                            }
                        }
                        xmlDoc.DocumentNode.RemoveChild(node);
                        files.Add(new ContentFile(jsPath, name, name, "js", GetUseType("js"))
                        {
                            ContentId = cid,
                            //将js,css文件可下载选项关闭
                            CanDownload = false
                        });
                    }
                    var htmlPath = Path.Combine(ContentParseSettings.HtmlSavePath);
                    name = Guid.NewGuid().ToString() + "."+ContentParseSettings.HtmlUseType;
                    path = ContentParseSettings.FileFormat(htmlPath, name);
                    using (var htmlStream=File.Create(path))
                    {
                        xmlDoc.Save(htmlStream);
                    }
                    files.Add(new ContentFile(htmlPath,name, file.OriginalName, file.ExtensionName, GetUseType(file.ExtensionName))
                    {
                        ContentId = cid,
                        CanDownload = false
                    });
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
