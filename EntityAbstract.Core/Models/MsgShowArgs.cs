using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.Models
{
    public class MsgShowArgs
    {
        public MsgShowArgs(int invialiTime, string msg, MsgDisplayTypes type= MsgDisplayTypes.Text)
        {
            InvialiTime = invialiTime;
            Msg = msg;
            Type = type;
        }

        /// <summary>
        /// 持续世界
        /// </summary>
        public int InvialiTime { get;  }
        /// <summary>
        /// 提示的消息
        /// </summary>
        public string Msg { get;  }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgDisplayTypes Type { get;  }
    }
    public enum MsgDisplayTypes
    {
        Text,
        View
    }
}
