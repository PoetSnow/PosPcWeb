using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Invoice")]
    [LogCName(" 发票")]
    public class Invoice
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("流水号")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票号码")]
        [LogAnywayWhenEdit]
        public string No { get; set; }

        [LogCName("发票的关联类型")]
        [LogEnum(typeof(Gemstar.BSPMS.Hotel.Services.Enums.TaxRefType))]
        public byte? Reftype { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订单号")]
        public string Resid { get; set; }

        [LogCName("会员账务id")]
        public Guid? ProfileCaid { get; set; }

        [LogCName("合约单位账务id")]
        public Guid? Companycaid { get; set; }

        [LogCName("发票类型")]
        [LogEnum(typeof(Gemstar.BSPMS.Hotel.Services.Enums.TaxType))]
        public byte? TaxType { get; set; }

        [LogCName("发票打印类型")]
        [LogEnum(typeof(Gemstar.BSPMS.Hotel.Services.Enums.TaxPrintType))]
        public byte? PrintType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票抬头")]
        public string TaxName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务登记号")]
        public string TaxNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票地址和电话")]
        public string TaxAddTel { get; set; }

        [LogCName("税率")]
        public decimal? RateTax { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票银行和账号")]
        public string TaxBankAccount { get; set; }

        [LogCName("开票营业日")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateFormatStr)]
        public DateTime? BsnsDate { get; set; }

        [LogCName("开票时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime? CDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开票人姓名")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务发票代码")]
        public string InvoiceCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务发票号")]
        public string InvoiceNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务发票批号")]
        public string InvoiceSeq { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("原税务发票代码")]
        public string InvoiceCode0 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("原税务发票号")]
        public string InvoiceNo0 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("红字发票信息")]
        public string RedInfo { get; set; }

        [LogCName("是否冲销")]
        [LogBool("是", "否")]
        public bool? Isread { get; set; }

        [LogCName("是否作废")]
        [LogBool("是", "否")]
        public bool? IsCancel { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

    }
}