using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.ExchangesManage
{
    public class TransferAccountsViewModel
    {
        [Display(Name = "主键ID")]
        [Required(ErrorMessage = "请输入主键ID")]
        public string Id { get; set; }

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

        [Display(Name = "转到合约单位")]
        [Required(ErrorMessage = "请选择合约单位")]
        public string ToCompanyId { get; set; }

        [Display(Name = "转账备注")]
        public string ToRemark { get; set; }

        [Display(Name = "是否批量转账")]
        public bool IsBatch { get; set; }
    }
}
