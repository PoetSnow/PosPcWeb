using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItemMultiClass")]
    [LogCName("消费项目对应大类")]
    public class PosItemMultiClass
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("大类id")]
        public string ItemClassid { get; set; }

        [LogCName("是否分类")]
        public bool? IsSubClass { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }
    }
}
