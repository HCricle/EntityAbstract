using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedEA.Core.DbModel.DbModels
{
    /// <summary>
    /// 数据组，可以一组一定相同属性的内容
    /// </summary>
    [Table("congup")]
    public class ContentGroup:DbModelBase
    {
        /// <summary>
        /// 组名
        /// </summary>
        [Required]
        [StringLength(100,MinimumLength =1)]
        public string Name { get; set; }

        /// <summary>
        /// 组标签，以_分割
        /// </summary>
        [StringLength(40)]
        public string Lable { get; set; }

        /// <summary>
        /// 组的内容
        /// </summary>
        public List<Content> Contents { get; set; }
    }
}
