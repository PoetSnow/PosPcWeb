using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResFolio")]
    [LogCName("账务明细表")]
    public class ResFolio
    {
        [Key]
        [LogCName("账务id")]
        public Guid Transid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订单id")]
        public string Resid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("注册id")]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("原始登记单id")]
        public string RegidFrom { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("账务名")]
        [LogAnywayWhenEdit]
        public string FolioName { get; set; }

        [LogCName(" ")]
        public DateTime? TransBsnsDate { get; set; }

        [LogCName(" ")]
        public DateTime? TransDate { get; set; }

        [LogCName("数量")]
        public decimal? Quantity { get; set; }

        [LogCName("总金额")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("消费还是付款")]
        public string Dcflag { get; set; }

        [LogCName("原币金额")]
        public decimal? OriAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发生的班次")]
        public string TransShift { get; set; }

        [LogCName("间夜数")]
        public decimal? Nights { get; set; }

        [LogCName("  ")]
        public decimal? TaxAmount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string InvNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账单id")]
        public string Billid { get; set; }

        [LogCName("结账营业日")]
        public DateTime? SettleBsnsdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账班次")]
        public string SettleShift { get; set; }

        [LogCName("结账时间")]
        public DateTime? SettleDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结账操作员")]
        public string SettleUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("录入操作员")]
        public string InputUser { get; set; }

        [LogCName("状态")]
        [ConcurrencyCheck]
        public byte? Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("付款类型")]
        public string Paymentdesc { get; set; }

        [LogCName("自动标记")]
        public byte? Isauto { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参考号")]
        public string RefNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("押金单号")]
        public string SeqNo { get; set; }

        [LogCName("作废或恢复时间")]
        public DateTime? CancelAndRecoveryDate { get; set; }

        [LogIgnore]
        [LogCName("账单代码")]
        [Column(TypeName = "varchar")]
        public string resBillCode { get; set; }

        [LogIgnore]
        [LogCName("客人来源")]
        [Column(TypeName = "varchar")]
        public string Sourceid { get; set; }

        [LogIgnore]
        [LogCName("市场分类")]
        [Column(TypeName = "varchar")]
        public string Marketid { get; set; }

        [LogIgnore]
        [LogCName("价格代码")]
        [Column(TypeName = "varchar")]
        public string RateCode { get; set; }

        [LogIgnore]
        [LogCName("结束时间")]
        public DateTime? EndDate { get; set; }

        [LogIgnore]
        [LogCName("押金类型")]
        [Column(TypeName = "varchar")]
        public string DepositType { get; set; }

    }
}
