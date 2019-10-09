using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("InvoiceDetail")]
    [LogCName("发票明细表")]
    public class InvoiceDetail
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogCName("发票主表id")]
        public Guid Invoceid { get; set; }

        [Key]
        [LogCName("发票明细id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目")]
        [LogAnywayWhenEdit]
        public string ItemTaxid { get; set; }

        [LogCName("含税金额")]
        public decimal? Amount { get; set; }

        [LogCName("税金")]
        public decimal? AmountTax { get; set; }

        [LogCName("不含税金额")]
        public decimal? AmountNoTax { get; set; }

        [LogCName("ratetax")]
        public decimal? RateTax { get; set; }


    }
}