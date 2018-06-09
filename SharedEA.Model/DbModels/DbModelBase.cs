using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedEA.Model.DbModels
{
    /// <summary>
    /// 除了用户模型，其它模型的基类
    /// </summary>
    public abstract class IndexDbModelBase
    {
        public IndexDbModelBase()
        {
            IsEnable = true;
            CreateTime = DateTime.Now;
        }

        [Key]
        public uint Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataType( DataType.DateTime)]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
