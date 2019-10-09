using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
     [Table("CompanyCa")]
    [LogCName("合约单位账务")]
    public class CompanyCa
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("公司id")]
        [LogAnywayWhenEdit]
        public Guid Companyid { get; set; }

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
        [LogCName("借贷标坊")]
        public string Dcflag { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("营业点")]
        public string OutletCode { get; set; }

        [LogCName("交易金额")]
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Invno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("签单人")]
        public string Sign { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部参考")]
        public string Refno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款方式")]
        public string Itemid { get; set; }

        [LogCName("是否已核销")]
        [LogBool("是", "否")]
        public bool? Ischeck { get; set; }

        [LogCName("核销单号")]
        public Guid? CheckNo { get; set; }

        [LogCName("核销时间")]
        public DateTime? CheckDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("核销人")]
        public string CheckUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人名")]
        public string GuestName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("regid")]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("集团id")]
        public string Grpid { get; set; }
    }
}
