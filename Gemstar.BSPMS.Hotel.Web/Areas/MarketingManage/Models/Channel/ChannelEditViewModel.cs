using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.Channel
{
    [Table("Channel")]
    public class ChannelEditViewModel : BaseEditViewModel
    {
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string Id { get; set; }

        [Display(Name = "渠道代码")]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Display(Name = "渠道名")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name = "市场分类")]
        [Column(TypeName = "varchar")]
        public string Marketid { get; set; }

        [Display(Name = "渠道签约代码")]
        [Column(TypeName = "varchar")]
        public string Refno { get; set; }

        [Display(Name = "担保记账代码")]
        [Column(TypeName = "varchar")]
        public string GuaranteeItemid { get; set; }

        [Display(Name = "预付记账代码")]
        [Column(TypeName = "varchar")]
        public string PayItemid { get; set; } 

        [Display(Name = "可用房型")]
        [Column(TypeName = "varchar")]
        public string Roomtypeid { get; set; }
         
        public bool? Isvalid { get; set; }

        [Display(Name = "是否共享房量")] 
        public bool? Isshareroom { get; set; }

        [Display(Name = "接口版本")]
        public string ItfVersion { get; set; }
         
        [Display(Name = "发预订短信给客人")]
        public bool? isSmsToGuest { get; set; } 

        [Display(Name = "发预订短信给客服")]
        public bool? isSmsToCustomerservice { get; set; }
         
        [Display(Name = "发预订微信给客服")]
        public bool? isWeChatToCustomerservice { get; set; }

        [Display(Name = "客服人员")]
        [Column(TypeName = "varchar")]
        public string Customerservice { get; set; }
    }
}