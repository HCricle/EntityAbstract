using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Shared.Core.Interactive.Models
{
    /// <summary>
    /// 窗口控制栏的项
    /// </summary>
    public class CommandItem
    {
        public CommandItem()
        {
        }

        public CommandItem(object content, ICommand command, object commandParam=null)
        {
            Content = content;
            Command = command;
            CommandParam = commandParam;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// 绑定的命令
        /// </summary>
        public ICommand Command { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public object CommandParam { get; set; }
    }
}
