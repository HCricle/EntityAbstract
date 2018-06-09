using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 商店物品
    /// </summary>
    public class ShopItem:DbModelBase
    {
        /// <summary>
        /// 依赖商店id
        /// </summary>
        public uint ShopId { get; set; }
        /// <summary>
        /// 物品名字
        /// </summary>
        [StringLength(256,MinimumLength =1)]
        public string Name { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [StringLength(512)]
        public string Descript { get; set; }
        /// <summary>
        /// 能在那个平台使用，默认0全部，以后再分
        /// </summary>
        public short UseType { get; set; }
        /// <summary>
        /// 如果是脚本
        /// </summary>
        public string UseCommand { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
    }
}
