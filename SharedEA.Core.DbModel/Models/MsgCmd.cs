using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.Models
{
    /// <summary>
    /// 用json序列化和反序列化
    /// </summary>
    public class MsgCmd
    {
        public MsgCmd()
        {
        }

        public MsgCmd(int id, short type, List<string> @params)
        {
            Id = id;
            Type = type;
            Params = @params;
        }

        /// <summary>
        /// 命令id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 适合哪一个平台,默认0全平台
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 命令开始行数
        /// </summary>
        public int StartPos { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public List<string> Params { get; set; }
        /// <summary>
        /// 被分解的字符串
        /// </summary>
        [JsonIgnore]
        public string ParsedText { get; set; }
        /// <summary>
        /// 命令进入位置，就是插入位置
        /// </summary>
        public int InPos { get; set; }

    }
}
