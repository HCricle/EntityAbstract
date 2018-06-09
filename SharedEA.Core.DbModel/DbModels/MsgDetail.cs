using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 对消息的详细
    /// </summary>
    public class MsgDetail : DbModelBase
    {
        public MsgDetail()
        {
        }

        public uint MsgId { get; set; }
        /// <summary>
        /// 消息内容,可以是html代码
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息类型，默认是0用户消息
        /// </summary>
        public short MsgType { get; set; }
        /// <summary>
        /// 消息要执行的命令,json 关联List<MsgCmd>
        /// </summary>
        public string Cmds { get; set; }
        /// <summary>
        /// 消息关联名
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }
        /// <summary>
        /// 命令集合
        /// </summary>
        [NotMapped]
        public ObservableCollection<MsgCmd> SerCmds { get; set; }
        /// <summary>
        /// 命令集合
        /// </summary>
        //public List<ACmd> ACmds { get; set; }
    }
}
