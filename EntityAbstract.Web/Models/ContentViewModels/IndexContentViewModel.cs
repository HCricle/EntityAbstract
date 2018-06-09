using EntityAbstract.Web.Controllers;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.ContentViewModels
{
    public class IndexContentViewModel
    {
        public IndexContentViewModel()
        {
            Action = nameof(ContentController.Index);
            PrePageCount = 6;
            GroupDesp = string.Empty;
        }
        public string Title => Group != null ? Group.Name : GroupDesp;
        public string CreateTime => Group != null ? Group.CreateTime.ToString() : DateTime.Now.ToString();
        /// <summary>
        /// 所属组
        /// </summary>
        public ContentGroup Group { get; set; }
        /// <summary>
        /// 对组的描述，如果Group为null，就会将这个显示出去,默认为""
        /// </summary>
        public string GroupDesp { get; set; }
        /// <summary>
        /// 内容集合
        /// </summary>
        public IEnumerable<Content> Contents { get; set; }
        /// <summary>
        /// 帖子数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 当前页,从0开始
        /// </summary>
        public int LocPage { get; set; }
        /// <summary>
        /// 一页多少个
        /// </summary>
        public int PrePageCount { get; set; }

        #region DivPagePropertys
        
        /// <summary>
        /// 前一页路由属性组
        /// </summary>
        public IEnumerable<RoutePair> PrevRoutes { get; set; }
        /// <summary>
        /// 下一页路由属性组
        /// </summary>
        public IEnumerable<RoutePair> NextRoutes { get; set; }
        /// <summary>
        /// 将PrevRoutes解析成 asp-route-{key}="{value}" 
        /// </summary>
        public string PrevRoutesString => GetRouteString(PrevRoutes);
        /// <summary>
        /// 将NextRoutes解析成 asp-route-{key}="{value}" 
        /// </summary>
        public string NextRoutesString => GetRouteString(NextRoutes);
        /// <summary>
        /// 将PrevRoutes解析成{key}={value}/...
        /// </summary>
        public string PrevRoutesHref => GetRouteHref(PrevRoutes);
        /// <summary>
        /// 将NextRoutes解析成{key}={value}/...
        /// </summary>
        public string NextRoutesHref => GetRouteHref(NextRoutes);
        /// <summary>
        /// 调用方法，默认是Index
        /// </summary>
        public string Action { get; set; }
        #endregion
        /// <summary>
        /// 一共有多少页
        /// </summary>
        public int TotalPage => TotalCount / (PrePageCount+1);
        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevPage => LocPage > 0;
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => TotalCount - (LocPage + 1) * PrePageCount > 0;
        private string GetRouteString(IEnumerable<RoutePair> route)
        {
            var res = new StringBuilder();
            if (route != null)
            {
                RoutePair routePair;
                for (int i = 0; i < route.Count(); i++)
                {
                    routePair = route.ElementAt(i);
                    res.Append($"asp-route-{routePair.Key}=\"{routePair.Value}\" ");
                }
            }
            return res.ToString();
        }
        private string GetRouteHref(IEnumerable<RoutePair> route)
        {
            var res =new StringBuilder();
            string pei;
            if (route != null)
            {
                RoutePair routePair;
                for (int i = 0; i < route.Count(); i++)
                {
                    routePair = route.ElementAt(i);
                    pei = i == 0 ? "" : "/";
                    res.Append($"{pei}{routePair.Key}={routePair.Value}");
                }
            }
            return res.ToString();
        }
    }
}
