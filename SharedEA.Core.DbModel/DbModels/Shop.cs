using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 商店
    /// </summary>
    public class Shop : DbModelBase
    {
        public Shop()
        {
        }
        /// <summary>
        /// 商店名字
        /// </summary>
        [StringLength(256,MinimumLength =1)]
        public string Name { get; set; }
        /// <summary>
        /// 商店描述
        /// </summary>
        [MaxLength(512)]
        public string Descript { get; set; }
        /// <summary>
        /// 商店能在那个平台显示,默认0全部
        /// </summary>
        public short Type { get; set; }
        public List<ShopItem> Items { get; set; }
    }
}
