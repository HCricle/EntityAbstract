using SharedEA.Data.Helpers;
using SharedEA.Core.DbModel.DbContext;
using SharedEA.Core.DbModel.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedEA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharedEA.Core.DataService.Services;
using SharedEA.Core.Data.Helpers;
using SharedEA.Core.Data.Models;
using Microsoft.Extensions.Options;
using SharedEA.Core.WebApi.JWT;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.WebApi.Helpers;

namespace SharedEA.Data.Repositories
{
    /// <summary>
    /// 内容的响应,主要业务逻辑
    /// </summary>
    public class ContentRepository : Repository<Content,EaDbContext>
    {
        private readonly XmlDataService _xmlDataService;
        private readonly ContentFileRepository _contentFileRepository;
        private readonly CommentRepository _commentRepository;
        private readonly IOptions<WebApiSettings> _settings;
        private readonly SignInManager<EaUser> _signInManager;
        private readonly UserManager<EaUser> _userManager;
        private readonly GroupRepository _groupRepository;
        private readonly LikeRepository _likeRepository;
        public ContentRepository(EaDbContext dbContext,
            ContentFileRepository contentFileRepository,
            CommentRepository commentRepository,
            XmlDataService xmlDataService, 
            IOptions<WebApiSettings> settings,
            SignInManager<EaUser> signInManager,
            GroupRepository groupRepository,
            LikeRepository likeRepository,
            UserManager<EaUser> userManager)
            :base(dbContext,dbContext.Contents)
        {
            _xmlDataService = xmlDataService;
            _settings = settings;
            _signInManager = signInManager;
            _groupRepository = groupRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _userManager = userManager;
            _contentFileRepository = contentFileRepository;

        }
        public override async Task AutoFillAsync(IEnumerable<Content> contents)
        {
            Content tmp;
            for (int i = 0; i < contents.Count(); i++)
            {
                tmp = contents.ElementAt(i);
                tmp.Comments = await DbContext.Comments.AsNoTracking().Where(c => c.ContentId == tmp.Id).ToListAsync();
                tmp.ContentFiles = await DbContext.ContentFiles.AsNoTracking().Where(c => c.ContentId == tmp.Id).ToListAsync();
            }
        }
        /// <summary>
        /// 由内容加载文件
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public async Task LoadFilesAsync(params Content[] contents)
        {
            if (contents!=null)
            {
                foreach (var c in contents)
                {
                    c.ContentFiles = (await _contentFileRepository.DirectGetAsync(content => content.ContentId == c.Id)).ToList();
                }
            }
        }
        /// <summary>
        /// 发送内容（帖），如果发送失败返回错误信息，成功返回null
        /// </summary>
        /// <param name="id">发送者id</param>
        /// <param name="content">MakedFile.html内容</param>
        /// <param name="title">标题</param>
        /// <param name="lable">标签</param>
        /// <param name="files">文件组，可以是html,css,js,图片文件,wav(没实现)</param>
        /// <returns></returns>
        public async Task<string> SendContentAsync(string id, string content, string title, string lable, int gid = -1, IEnumerable<FormFileModel> files = null,bool autoJs=true)
        {
            if (!IsAllNotNullOrEmpty(id,content,title,lable))
            {
                return "模型验证失败，存在空数据";
            }
            string errMsg = string.Empty;
            var _content = DbContext.Contents
                    .AsNoTracking()
                    .Where(c => c.EaUserId == id&&c.IsEnable)
                    .OrderByDescending(o=>o.Id).FirstOrDefault();
            if (_content!=null)    
            {
                var ts = new TimeSpan((DateTime.Now.Ticks - _content.CreateTime.Ticks)).TotalSeconds;
                if (ts < ContentParseSettings.Cd) 
                {
                    return "发帖时间CD中，还有 " + (float)(ts / 60) + " 分钟";
                }
            }
            errMsg = await MakeContentAsync(id, content, title, lable, gid, files,autoJs);
            return errMsg;
        }
        private bool IsAllNotNullOrEmpty(params string[] param)
        {
            return param.Any(p => !string.IsNullOrEmpty(p));
        }
        /// <summary>
        /// 提交内容
        /// </summary>
        /// <param name="id">发布者id</param>
        /// <param name="content">内容</param>
        /// <param name="title">标题</param>
        /// <param name="lable">标签_隔开</param>
        /// <param name="files">文件组</param>
        /// <returns>如果返回null就是成功,不成功返回错误信息</returns>
        private async Task<string> MakeContentAsync(string id,string content,string title,string lable,int gid=-1,IEnumerable<FormFileModel> files=null,bool autoJs=true)
        {
            ContentGroup group;
            if (gid!=-1)//TODO:没测试
            {
                group = DbContext.Groups.Single(g => g.Id == gid);
            }
            else
            {
                group = DbContext.Groups.First();//暂时是一个默认组
            }
            Debug.Assert(group != null);
            content = content ?? string.Empty;
            //制作html文件
            var c = new Content("default", title, 0, lable, null, null)
            {
                ContentGroupId = group.Id,
                Id = (uint)(await this.Count()) + 1,//(uint)DbContext.Contents.Count() + 1
                EaUserId = id,
                Description = content.Count() > ContentParseSettings.DescriptLength ? content.Substring(0, (int)ContentParseSettings.DescriptLength - 1) : content,
                ContentFiles = new List<ContentFile>()
            };
            if (files == null && content == null && content.Count() < 10) 
            {
                return "没有发送的内容,只是一个文件或者10个字符";
            }
            var xmlDocBuilder = _xmlDataService.CreateXmlDoc()
                        .AndXmlDoc(builder =>
                        {
                            var nodeContent = builder.XmlDoc.CreateElement(ContentParseSettings.RootNode);
                            nodeContent.SetAttribute("class", "user-content row");
                            nodeContent.InnerText = content ?? string.Empty;
                            builder.RootElement.AppendChild(nodeContent);//加入节点
                            return builder;
                        });
            if (files != null)
            {
                var copyData = files.ToList();
                if (copyData.Count() >= ContentParseSettings.MaxUploadFileCount) 
                {
                    return $"最多只能上传{ContentParseSettings.MaxUploadFileCount}个文件";
                }
                //过滤一次文件，将可解析的文件解析，不可解析的文件转存
                //文件解析暂时可以为 .jpg .png .jpeg .js .css
                //储存makefile
                await xmlDocBuilder.AndXmlDocAsync(async (builder) =>
                {
                    var nodeParsed = builder.XmlDoc.CreateElement("user-parsed");
                    //加入可解析文件
                    var parsedResual = await FileParseHelper.ParseAsync(builder.XmlDoc, nodeParsed, copyData);
                    //copyData=copyData.Except(parsedResual).ToList();//将已经完成解析的移除|不移除了，以后会有用
                    builder.RootElement.AppendChild(nodeParsed);
                    return builder;
                });

                //上面的写好
                //保存其它文件（如果有）--TODO ContentFile参数怎么填是个问题
                //usertype从字典查，如果差不多就用后缀名代替
                string name,path;
                //要特别对待html文件,建议html文件
                var htmlFiles = copyData.Where(f => f.ExtensionName == ContentParseSettings.HtmlUseType);
                /*
                if (htmlFiles.Any(file=>FileParseHelper.HasDangrageCore(file.File)))//FIXME:这里验证不了，出现加载异常
                {
                    return "上传的html文件存在object,iframe,script危险标签，不能上传此文件";
                }
                */
                if (autoJs)
                {
                    FormFileModel ffm;
                    for (int i = 0; i < htmlFiles.Count(); i++)
                    {
                        ffm = htmlFiles.ElementAt(i);
                        var res= await FileParseHelper.SplitHtmlAsync(ffm, c.ContentFiles, c.Id);
                        if (!res)
                        {
                            return $"上传:{ffm.File.Name},时出现异常或文件不存在，上传通道-SplitHtmlAsync";
                        }
                    }
                }
                else if(htmlFiles.Count()!=0)
                {
                    if (htmlFiles.Any(file => FileParseHelper.HasDangrageCore(file.File)))
                    {
                        return "上传的html文件存在object,iframe,script危险标签，不能上传此文件";
                    }
                }
                try
                {
                    string orPath = Path.Combine(ContentParseSettings.OtherSavePath);
                    foreach (var item in copyData)
                    {
                        if (item.ExtensionName != ContentParseSettings.HtmlUseType) //html文件已处理
                        {
                            name = Guid.NewGuid().ToString() + "." + item.ExtensionName;
                            path = ContentParseSettings.FileFormat(orPath, name);
                            await CopyFileAsync(path, item.File);
                            c.ContentFiles.Add(new ContentFile(orPath, name, item.OriginalName, item.ExtensionName, FileParseHelper.GetUseType(item.ExtensionName))
                            {
                                ContentId = c.Id,
                                //将js,css文件可下载选项关闭
                                CanDownload = item.ExtensionName == ContentParseSettings.CssUseType || item.ExtensionName == ContentParseSettings.JsUseType ? false : item.CanDownload
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "上传文件时发生异常:" + ex.Message;
                }
            }
            if (!xmlDocBuilder.IsBuild)
            {
                await xmlDocBuilder.BuildAsync();
            }
            var xmlFileName = Guid.NewGuid().ToString() + "."+ ContentParseSettings.HtmlUseType;
            var xmlFilePath = Path.Combine(ContentParseSettings.HtmlSavePath);
            using (var fs = File.Create(ContentParseSettings.FileFormat(xmlFilePath, xmlFileName)))
            {
                xmlDocBuilder.XmlDoc.Save(fs);
            }
            c.ContentFiles.Add(new ContentFile(xmlFilePath, xmlFileName, xmlFileName, ContentParseSettings.HtmlUseType, ContentParseSettings.HtmlUseType) { CanDownload=false, ContentId=c.Id});

            await this.AddAsync(c);
            return null;
        }
        private static async Task CopyFileAsync(string path,IFormFile sourceFile)
        {
            using (var fs=File.Create(path))
            {
                await sourceFile.CopyToAsync(fs);
            }
        }
        /// <summary>
        /// 通过条件搜索内容
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="count">搜索多少个-1为全部</param>
        /// <param name="skip">跳过多少个</param>
        /// <param name="isAutoFill">是否自动填充外键数据</param>
        /// <returns></returns>
        private async Task<bool> _DeleteContentAsync(string userId,uint contentId)
        {
            var content = await DbSet.SingleOrDefaultAsync(c => c.IsEnable && c.Id == contentId && c.EaUserId == userId);
            if (content==null)
            {
                return false;
            }
            content.IsEnable = false;
            DbSet.Update(content);
            await DbContext.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="gid"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="ai"></param>
        /// <returns></returns>
        public async Task<ContentRepositoryModel> GetContentsAsync(uint gid, int s = 0, int t = 6, bool ai = false)//一页6个
        {
            var cs = NonTrackingAsyncEnum.Where(c => c.ContentGroupId == gid && c.IsEnable);
            var count = await cs.Count();
            var datas = await cs.Skip(s).Take(t).ToArray();
            if (ai)
            {
                await LoadFilesAsync(datas);
            }
            var cpm = new ContentRepositoryModel(count, s, t, datas);
            return cpm;
        }
        /// <summary>
        /// 获取一个组
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        public async Task<ContentGroup> GetGroupAsync(uint gid)
        {
            var data = await _groupRepository.SingleOrDefault(g => g.Id == gid && g.IsEnable);
            return data;
        }
        /// <summary>
        /// 获取组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<GroupRepositoryModel> GetGroupsAsync(int s, int t)
        {
            var gs = _groupRepository.NonTrackingAsyncEnum.Where(g => g.IsEnable);
            var count = await gs.Count();
            var datas = await gs.Skip(s).Take(t).ToArray();
            var gpm = new GroupRepositoryModel(count, s, t, datas);
            return gpm;
        }
        public async Task<bool> IsLikeContentAsync(EaUser user, uint cid)
        {
            var islike = await IsLikeAsync(user.Id, cid);
            return islike;
        }

        /// <summary>
        /// 是否喜欢这个内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<bool> IsLikeContentAsync(string authorization,uint cid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await IsLikeContentAsync(user, cid);
        }
        public async Task<bool> LikeContentAsync(EaUser user, uint cid)
        {
            var islike = await IsLikeAsync(user.Id, cid);
            if (!islike)
            {
                _likeRepository.DbSet.Add(new Like()
                {
                    EaUserId = user.Id,
                    ContentId = cid
                });
                var res = DbContext.SaveChanges();
                return res > 0;
            }
            return false;
        }

        /// <summary>
        /// 喜欢一个内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<bool> LikeContentAsync(string authorization, uint cid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await LikeContentAsync(user, cid);
        }
        public async Task<ContentRepositoryModel> GetLikeContentAsync(EaUser user, int t, int s)
        {
            
            var cs = _likeRepository.Where(c => c.EaUserId == user.Id && c.IsEnable)
                                    .Skip(s)
                                    .Take(t)
                                    .Select(c => c.ContentId);//喜欢的内容页
            var count = await cs.Count();
            var lcs = await cs.ToList();
            var datas = await NonTrackingAsyncEnum.Where(c => lcs.Contains(c.Id) && c.IsEnable).ToArray();
            var cpm = new ContentRepositoryModel(count, s, t, datas);
            return cpm;
        }

        /// <summary>
        /// 获取用户喜欢的贴
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<ContentRepositoryModel> GetLikeContentAsync(string authorization, int t, int s)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new ContentRepositoryModel(false);
            }
            return await GetLikeContentAsync(user, t, s);
        }
        /// <summary>
        /// 获取一个内容
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<Content> GetContentAsync(uint cid)
        {
            var content = await NonTrackingAsyncEnum.SingleOrDefault(c => c.Id == cid);
            return content;
        }
        public async Task<bool> UnLikeContentAsync(EaUser user, uint cid)
        {
            var like = await _likeRepository.SingleOrDefault(l => l.ContentId == cid && l.EaUserId == user.Id);
            if (like != null)
            {
                like.IsEnable = false;
                _likeRepository.DbSet.Update(like);
                DbContext.SaveChanges();
            }
            return false;
        }

        /// <summary>
        /// 取消喜欢的内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<bool> UnLikeContentAsync(string authorization, uint cid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await UnLikeContentAsync(user, cid);
        }
        public async Task<string> SendContentAsync(EaUser user, IFormCollection files)
        {
            var content = files["Content"].FirstOrDefault();
            var title = files["Title"].FirstOrDefault();
            var label = files["Label"].FirstOrDefault();
            var res = await SendContentAsync(user.Id, content, title, label, -1, files.Files.Select(f => new FormFileModel(f)));
            if (res != null)
            {
                return res;
            }
            return null;
        }

        /// <summary>
        /// 发送内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<string> SendContentAsync(string authorization,IFormCollection files)
        {
            if (files.Any(f => f.Value.Count == 0))
            {
                return "不存在内容";
            }
            //var uid = files["Uid"].FirstOrDefault();
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return "不存在此用户";
            }
            return await SendContentAsync(user, files);
        }
        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="ai"></param>
        /// <returns></returns>
        public async Task<ContentRepositoryModel> SearchContentAsync(string ts, int s, int t, bool ai = false)
        {
            var cs = NonTrackingAsyncEnum.Where(c => c.Title.Contains(ts) && c.IsEnable);
            var count = await cs.Count();
            var selectcs = await cs.Skip(s).Take(t).ToArray();
            if (ai)
            {
                await LoadFilesAsync(selectcs);
            }
            var cpm = new ContentRepositoryModel(count, s, t, selectcs);
            return cpm;
            //return Json((await _contentRepository.GetAsync(c => c.Title.Contains(ts)&&c.IsEnable, preCount, preCount * page, autoin)));
        }
        public async Task<int> GetGroupCountAsync()
        {
            return await _groupRepository.Count(g => g.IsEnable);
        }
        public async Task<int> GetGroupContentCountAsync(uint gid)
        {
            return await NonTrackingAsyncEnum.Count(c => c.ContentGroupId == gid && c.IsEnable);
        }
        public async Task<ContentRepositoryModel> GetUserContentsAsync(EaUser user, int s, int t)
        {
            
            var us = NonTrackingAsyncEnum.Where(c => c.EaUserId == user.Id && c.IsEnable);
            var count = await us.Count();
            var datas = await us.Skip(s).Take(t).ToArray();
            var crm = new ContentRepositoryModel(count, s, t, datas);
            return crm;
        }

        /// <summary>
        /// 获取用户发送过的内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<ContentRepositoryModel> GetUserContentsAsync(string authorization, int s,int t)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return new ContentRepositoryModel(false);
            }
            return await GetUserContentsAsync(user, s, t);
        }
        /// <summary>
        /// 获取内容的评论
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public async Task<CommentRepositoryModel> GetContentCommentsAsync(uint cid, int t, int s)
        {
            var cs = _commentRepository.Where(c => c.ContentId == cid && c.IsEnable);
            var count = await cs.Count();
            var datas = await cs.Skip(s).Take(t).ToArray();
            var coms = new RelComment[datas.Count()];
            for (int i = 0; i < datas.Count(); i++)
            {
                var d = datas[i];
                var user = await _userManager.FindByIdAsync(d.EaUserId);
                coms[i] = new RelComment()
                {
                    Id = d.Id,
                    ContentId = d.ContentId,
                    CreateTime = d.CreateTime,
                    EaUserId = d.EaUserId,
                    HtmlContent = d.HtmlContent,
                    IsEnable = d.IsEnable,
                    UserName = user.UserName,
                    IsSelf = d.EaUserId == user.Id
                };
            }
            var cpm = new CommentRepositoryModel(count, s, t, coms);
            return cpm;
        }
        public async Task<bool> SendCommentAsync(EaUser user, uint cid, string cstring)
        {
            var content = await DbSet.ToAsyncEnumerable().SingleOrDefault(c => c.Id == cid && c.IsEnable);
            Console.WriteLine($"cid={cid} hascontent={content!=null}");
            if (content == null)
            {
                Console.WriteLine("content null");
                return false;
            }
            if (string.IsNullOrEmpty(cstring))
            {
                return false;
            }
            _commentRepository.DbSet.Add(new Comment()
            {
                ContentId = content.Id,
                CreateTime = DateTime.Now,
                HtmlContent = cstring,
                EaUserId = user.Id
            });
            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 发送评论
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <param name="cstring"></param>
        /// <returns></returns>
        public async Task<bool> SendCommentAsync(string authorization, uint cid,  string cstring)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await SendCommentAsync(user, cid, cstring);
        }
        public async Task<bool> DeleteCommentAsync(EaUser user, uint cid)
        {
            
            var res = await _commentRepository.DeleteCommentAsync(user.Id, cid);
            return res;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCommentAsync(string authorization, uint cid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await DeleteCommentAsync(user, cid);
        }
        public async Task<bool> DeleteContentAsync(EaUser user, uint cid)
        {
            var res = await _DeleteContentAsync(user.Id, cid);

            return res;
        }
        /// <summary>
        /// 删除内容
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<bool> DeleteContentAsync(string authorization, uint cid)
        {
            var user = await GetUserAsync(authorization);
            if (user == null)
            {
                return false;
            }
            return await DeleteContentAsync(user, cid);
        }
        public async Task<ContentFileRepositoryModel> GetFileAsync(uint fid)
        {
            var file = await _contentFileRepository.SingleOrDefault(f => f.Id == fid && f.IsEnable);
            if (file != null)
            {
                try
                {
                    return new ContentFileRepositoryModel(Path.Combine(file.Path, file.FileName), "application/octet-stream", file, true);
                }
                catch (Exception)
                {
                    return new ContentFileRepositoryModel(false);
                }
            }
            return new ContentFileRepositoryModel(false);
        }

        private async Task<bool> IsLikeAsync(string id,uint cid)
        {
            var islike = await _likeRepository.NonTrackingAsyncEnum.Any(l => l.IsEnable && l.ContentId == cid && l.EaUserId == id);
            return islike;
        }
        private async Task<EaUser> GetUserAsync(string authorization)
        {
            var token = AuthorizationHelper.ParseTokenByAuthorization(authorization, _settings.Value.SecretKey);
            if (token == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(token.sub);
        }

    }
}
