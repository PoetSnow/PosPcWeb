using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.ExchangesManage
{
    public class SplitAccountsViewModel
    {
        [Display(Name = "主键ID")]
        [Required(ErrorMessage = "请输入主键ID")]
        public Guid Id { get; set; }

        [Display(Name = "交易类型")]
        [Required(ErrorMessage = "请输入交易类型")]
        public string Type { get; set; }

        [Display(Name = "合约单位ID")]
        public Guid CompanyId { get; set; }

        [Display(Name = "合约单位名称")]
        public string CompanyName { get; set; }

        [Display(Name = "当前应付余额")]
        public decimal? Payable { get; set; }

        [Display(Name = "付款方式名称")]
        [Required(ErrorMessage = "请选择付款方式名称")]
        public string ItemName { get; set; }

        [Display(Name = "付款金额")]
        [Required(ErrorMessage = "请输入付款金额")]
        public decimal Amount { get; set; }

        [Display(Name = "单号")]
        [Required(ErrorMessage = "请输入单号")]
        public string Invno { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "拆账金额")]
        [Required(ErrorMessage = "请输入拆账金额")]
        public decimal SplitAmount { get; set; }

        [Display(Name = "拆账备注")]
        public string SplitRemark { get; set; }
    }
}