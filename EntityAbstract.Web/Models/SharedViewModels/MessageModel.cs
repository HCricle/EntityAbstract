using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.SharedViewModels
{
    /// <summary>
    /// 信息页的视图
    /// </summary>
    public class MessageModel
    {
        public MessageModel()
        {
        }
        public MessageModel(string text, string controller, string action, string goText)
        {
            Text = text;
            Controller = controller;
            Action = action;
            GoText = goText;
            Title = "提示信息";
        }
        public string Title { get; set; }
        /// <summary>
        /// 提示的文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 下方转到的控制器
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 下方转到的方法
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 下方转到的文本
        /// </summary>
        public string GoText { get; set; }
    }
}
