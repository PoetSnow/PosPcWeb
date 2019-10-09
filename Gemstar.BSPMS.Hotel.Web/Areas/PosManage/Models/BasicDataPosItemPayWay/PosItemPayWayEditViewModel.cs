using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPayWay
{
    public class PosItemPayWayEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "项目代码")]
        [Required(ErrorMessage = "请输入项目代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名")]
        public string Ename { get; set; }

        [Display(Name = "状态")]
        public byte? Status { get; set; }

        [Display(Name = "微信显示")]
        public bool? IsWxShow { get; set; }

        [Display(Name = "处理方式")]
        public string PayType { get; set; }

        [Display(Name = "币种")]
        public string Montypeno { get; set; }

        [Display(Name = "汇率")]
        public decimal? Rate { get; set; }

        [Display(Name = "计收入")]
        public bool? IsInCome { get; set; }

        [Display(Name = "可找赎")]
        public bool? IsChange { get; set; }

        [Display(Name = "可作订金")]
        public bool? IsSubscription { get; set; }

        [Display(Name = "可支出")]
        public bool? IsPayout { get; set; }

        [Display(Name = "是否可充值")]
        public bool? IsCharge { get; set; }

        [Display(Name = "是否开发票")]
        public bool? IsInvoice { get; set; }

        [Display(Name = "所属营业点")]
        public string Refeid { get; set; }

        [Display(Name = "所属营业点")]
        public string[] Refeids
        {
            get { return string.IsNullOrEmpty(Refeid) ? new string[] { } : Refeid.Split(','); }
            set { Refeids = value; }
        }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "排列顺序")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "操作员")]
        public string OperName { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "内部编码")]
        public string CodeIn { get; set; }

        [Display(Name = "是否可反结")]
        public bool? IsRepay { get; set; }
    }
}