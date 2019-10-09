using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.CompanyManage
{
    public class CompanyAddViewModel
    {
        [Display(Name = "合约单位代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "合约单位名称")]
        [Required(ErrorMessage = "请输入合约单位名称")]
        public string Name { get; set; }

        [Display(Name = "合约单位类型")]
        public string CompanyTypeid { get; set; }

        [Display(Name = "合约单位电话")]
        public string Tel { get; set; }

        [Display(Name = "合约单位地址")]
        public string Add { get; set; }

        [Display(Name = "联系人")]
        public string Contact { get; set; }

        [Display(Name = "联系人职位")]
        public string Position { get; set; }

        [Display(Name = "联系人手机")]
        public string ContactMobile { get; set; }

        [Display(Name = "发票类型")]
        public byte? TaxType { get; set; }

        [Display(Name = "税务登记号")]
        public string TaxNo { get; set; }

        [Display(Name = "开户银行")]
        public string Bank { get; set; }

        [Display(Name = "开户银行账号")]
        public string BankAccount { get; set; }

        [Display(Name = "生效日期")]
        [Required(ErrorMessage = "请选择生效日期")]
        public DateTime? BeginDate { get; set; }

        [Display(Name = "有效日期")]
        public DateTime? ValidDate { get; set; }

        [Display(Name = "价格代码")]
        public string RateCode { get; set; }

        [Display(Name = "挂账限额")]
        [Range(1, Int32.MaxValue, ErrorMessage = "挂账限额不能小于0")]
        public int? LimitAmount { get; set; }

        [Display(Name = "业务员")]
        [Required(ErrorMessage = "请选择业务员")]
        public string Sales { get; set; }

        [Display(Name = "发票抬头")]
        public string TaxName { get; set; }

        [Display(Name = "发票地址和电话")]
        public string TaxAddTel { get; set; }

        [Display(Name = "发票银行和账号")]
        public string TaxBankAccount { get; set; }

        [Display(Name = "联系人邮箱")]
        [RegularExpression(RegexHelper.EmailRegexString, ErrorMessage = "请输入有效的邮箱")]
        public string TaxEmail { get; set; }
    }
}
