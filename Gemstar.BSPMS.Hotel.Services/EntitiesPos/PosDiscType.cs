using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosDiscType")]
    [LogCName("折扣类型")]
    public class PosDiscType
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("折扣代码")]
        public string Code { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("折扣名称")]
        public string Cname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [LogCName("折扣")]
        public decimal? Discount { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogCName("折扣类型")]
        public byte? DiscType { get; set; }

    }
}