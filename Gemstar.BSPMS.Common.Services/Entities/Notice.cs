using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("Notice")]
    public class Notice
    {
       
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime SDate { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        [Column(TypeName = "varchar")]
        public string InputUser { get; set; }

        /// <summary>
        /// 级别 数字越大级别越高
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// 版本Ids
        /// </summary>
        public string VersionId { get; set; }

    }
}
