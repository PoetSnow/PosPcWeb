using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResFolioPayInfo")]
    [LogCName("待支付列表")]
    public class ResFolioPayInfo
    {
        [Key]
        [LogCName("Id")]
        public int Id { get; set; }
        
        [LogCName("酒店id")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        [LogCName("付款方式")]
        [Column(TypeName = "varchar")]
        public string ItemId { get; set; }
        [LogCName("应付金额")]
        public decimal? Amount { get; set; }
        [LogCName("产品类型")]
        [Column(TypeName = "varchar")]
        public string ProductType { get; set; }

        [LogCName("产品业务id")]
        [Column(TypeName = "varchar")]
        public string ProductTransId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("支付类型")]
        public string PayAction { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("支付订单号")]
        public string PayOrderNo { get; set; }

        [LogCName("创建日期")]
        public DateTime? Cdate { get; set; }

        [LogCName("状态")]
        public ResFolioPayStatus? Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("二维码地址")]
        public string QrcodeUrl { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("支付号")]
        public string PayTransId { get; set; }

        [LogCName("支付时间")]
        public DateTime? PayDate { get; set; }

        [LogCName("支付金额")]
        public string PayAmount { get; set; }

        [LogCName("错误原因")]
        public string PayError { get; set; }


    }
}
