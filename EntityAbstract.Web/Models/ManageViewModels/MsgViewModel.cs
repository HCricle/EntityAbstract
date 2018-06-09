using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.ManageViewModels
{
    /// <summary>
    /// 消息页模型
    /// </summary>
    public class MsgViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Msg> Msgs { get; set; }

        public int MsgCount => Msgs.Count();
    }
}
