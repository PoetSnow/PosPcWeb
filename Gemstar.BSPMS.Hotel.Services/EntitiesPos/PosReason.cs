using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosReason")]
    [LogCName("原因")]
    public class PosReason
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("原因代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("原因名称")]
        public string Cname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogCName("类型")]
        public byte? IstagType { get; set; }

        [LogCName("是否加回库存")]
        public bool? Isreuse { get; set; }

        [LogCName("是否自动沽清")]
        public bool? Isautosellout { get; set; }

        [LogCName("是否出品")]
        public bool? Isproduce { get; set; }

        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }


        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }

    }
}
