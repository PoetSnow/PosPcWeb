using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTabtype")]
    [LogCName("餐台类型")]
    public class PosTabtype
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台类型代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台类型名称")]
        public string Cname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogCName("类别")]
        public byte? Istagclass { get; set; }

        [LogCName("最大座位数")]
        public int? MaxSeat { get; set; }

        [LogCName("出品方式")]
        public byte? ProduceType { get; set; }

        [LogCName("赠送所需金额")]
        public decimal? LargAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("赠送项目")]
        public string LargItem { get; set; }

        [LogCName("微信点餐支付方式")]
        public byte? WxPaytype { get; set; }

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
