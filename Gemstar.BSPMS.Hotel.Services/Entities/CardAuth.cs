using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("CardAuth")]
    [LogCName("信用卡授权记录")]
    public class CardAuth
    {
        [Key]
        [LogCName("流水号")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("账号")]
        [LogAnywayWhenEdit]
        [LogStartsWithHid]
        [LogKey]
        public string Regid { get; set; }

        [LogCName("营业日")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateFormatStr)]
        public DateTime? TransBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目")]
        [LogRefrenceName(Sql = "SELECT name FROM item WHERE id={0}")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("卡号")]
        public string CardNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("有效期")]
        public string ValidDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("授权码")]
        public string AuthNo { get; set; }

        [LogCName("授权金额")]
        public decimal? AuthAmount { get; set; }

        [LogCName("扣款金额")]
        public decimal? BillAmount { get; set; }

        [LogCName("授权时间")]
        public DateTime? CreateDate { get; set; }

        [LogCName("授权营业日")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateFormatStr)]
        public DateTime? CreateBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("授权操作员")]
        public string CreateUser { get; set; }

        [LogCName("完成时间")]
        public DateTime? CompleteDate { get; set; }

        [LogCName("完成营业日")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateFormatStr)]
        public DateTime? CompleteBsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("完成操作员")]
        public string CompleteUse { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("状态")]
        [LogEnum(typeof(Gemstar.BSPMS.Hotel.Services.Enums.CardAuthStatus))]
        public byte? Status { get; set; }

    }
}
