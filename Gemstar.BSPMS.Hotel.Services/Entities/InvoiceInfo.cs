using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("InvoiceInfo")]
    [LogCName("开票信息表")]
    public class InvoiceInfo
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("流水号")]
        public Guid Id { get; set; }

        [LogCName("发票类型")]
        [LogAnywayWhenEdit]
        public byte? TaxType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票抬头")]
        public string TaxName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("税务登记号")]
        public string TaxNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票地址和电话")]
        public string TaxAddTel { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发票银行和账号")]
        public string TaxBankAccount { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("拼音码")]
        public string Py { get; set; }
    }
}