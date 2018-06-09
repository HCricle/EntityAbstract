using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.WebApi.Models
{
    public class MsgSendModel
    {
        public MsgSendModel()
        {
        }

        public MsgSendModel(uint mid, string text, string cmd)
        {
            Mid = mid;
            Text = text;
            Cmd = cmd;
        }

        public uint Mid { get; set; }
        public string Text { get; set; }
        /// <summary>
        /// 关联List<MsgCmd>
        /// </summary>
        public string Cmd { get; set; }
    }
}
