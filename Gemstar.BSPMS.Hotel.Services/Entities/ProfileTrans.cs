using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ProfileTrans")]
    [LogCName("会员消费记录")]
    public class ProfileTrans
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("客历号")]
        [LogAnywayWhenEdit]
        public Guid Profileid { get; set; }

        [LogCName("日期")]
        public DateTime TransDate { get; set; }

        [LogCName("营业日")]
        public DateTime TransBsnsdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("交易说明")]
        public string Type { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点")]
        public string OutletCode { get; set; }

        [LogCName("消费金额")]
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Invno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部参考")]
        public string Refno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款方式")]
        public string Itemid { get; set; }

        [LogCName("间夜数")]
        public decimal? Nigths { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间号")]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房类型id")]
        public string RoomTypeName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("regid")]
        public string Regid { get; set; }

        [LogCName("积分金额")]
        public decimal? ScoreAmount { get; set; }
    }
}
