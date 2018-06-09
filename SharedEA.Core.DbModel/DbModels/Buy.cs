using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 购买记录
    /// </summary>
    public class Buy:DbModelBase
    {
        /// <summary>
        /// 购买者
        /// </summary>
        public string EaUserId { get; set; }
        /// <summary>
        /// 购买的商品id
        /// </summary>
        public uint ShopItemId { get; set; }
    }
}
