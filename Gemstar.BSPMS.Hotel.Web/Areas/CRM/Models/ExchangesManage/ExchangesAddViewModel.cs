using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.ExchangesManage
{
    public class ExchangesAddViewModel
    {
        [Display(Name = "合约单位ID")]
        [Required(ErrorMessage = "请输入合约单位ID")]
        public Guid CompanyId { get; set; }

        [Display(Name = "交易类型")]
        [Required(ErrorMessage = "请输入交易类型")]
        public string Type { get; set; }

        [Display(Name = "合约单位名称")]
        public string CompanyName { get; set; }

        [Display(Name = "当前应付余额")]
        public decimal? Payable { get; set; }

        [Display(Name = "付款方式")]
        [Required(ErrorMessage = "请选择付款方式")]
        public string ItemId { get; set; }

        [Display(Name = "金额")]
        [Required(ErrorMessage = "请输入金额")]
        public decimal Amount { get; set; }

        [Display(Name = "单号")]
        [Required(ErrorMessage = "请输入单号")]
        public string Invno { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "营业点")]
        public string Outletcode { get; set; }
        /// <summary>
        /// 付款项目的处理方式代码
        /// </summary>
        public string FolioItemAction { get; set; }
        /// <summary>
        /// 付款项目的处理方式对应的json格式的参数字符串
        /// </summary>
        public string FolioItemActionJsonPara { get; set; }
    }
}