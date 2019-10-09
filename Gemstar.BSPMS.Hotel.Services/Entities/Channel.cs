using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{

    [Table("Channel")]
    [LogCName("渠道定义")]
    public class Channel
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("渠道代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("市场分类")]
        public string Marketid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("签约代码")]
        public string Refno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("担保记账代码")]
        public string GuaranteeItemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("预付记帐代码")]
        public string PayItemid { get; set; }

        [LogCName("现付合约单位")]
        public Guid? NoPayCompanyid { get; set; }

        [LogCName("担保或预付合单位")]
        public Guid? PayCompanyid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("可用房型")]
        public string Roomtypeid { get; set; }

        [LogCName("是否可用")]
        [LogBool("是", "否")]
        public bool? isvalid { get; set; } 

        [LogCName("是否共享房量")]
        [LogBool("是", "否")]
        public bool? Isshareroom { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("接口版本")]
        public string ItfVersion { get; set; }

        [LogCName("发预订短信给客人")]
        [LogBool("是", "否")]
        public bool? isSmsToGuest { get; set; }

        [LogCName("发预订短信给客服")]
        [LogBool("是", "否")]
        public bool? isSmsToCustomerservice { get; set; }

        [LogCName("发预订微信给客服")]
        [LogBool("是", "否")]
        public bool? isWeChatToCustomerservice { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客服人员")]
        public string Customerservice { get; set; }
    }
}