using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 当商店物品或某些行为需要执行命令时,
    /// 此数据可以显示某平台需要做什么事情,
    /// 到时读取时,命令组会被序列号成json
    /// </summary>
    public class ACmd:DbModelBase
    {
        /// <summary>
        /// 命令图部文件夹 ~/wwwroot/{CmdImgFolder}
        /// </summary>
        public static readonly string CmdImgFolder = "cmdimgs";
        /// <summary>
        /// 命令名称
        /// </summary>
        [StringLength(256,MinimumLength =1)]
        public string Name { get; set; }
        /// <summary>
        /// 命令描述
        /// </summary>
        [MaxLength(512)]
        public string Descript { get; set; }
        /// <summary>
        /// 图部文件 ~/wwwroot/{CmdImgFolder}/{Img}
        /// </summary>
        [MaxLength(512)]
        public string Img { get; set; }
        /// <summary>
        /// 表示命令内容
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 适合哪一个平台,默认0全平台
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 命令危险级别,默认0没危险
        /// </summary>
        public short DamgStep { get; set; }
    }
}
