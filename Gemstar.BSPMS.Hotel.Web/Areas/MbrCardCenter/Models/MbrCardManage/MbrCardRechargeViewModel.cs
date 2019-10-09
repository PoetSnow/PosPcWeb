using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardRechargeViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "当前储值余额")]
        public decimal? Balance { get; set; }

        [Display(Name = "当前增值余额")]
        public decimal? SendBalance { get; set; }

        [Display(Name = "付款方式")]
        [Required(ErrorMessage = "请选择付款方式")]
        public string PayWayId { get; set; }

        [Display(Name = "原币储值金额")]
        [Required(ErrorMessage = "请输入原币储值金额")]
        public decimal OriginPayMoney { get; set; }

        [Display(Name = "储值金额")]
        [Required(ErrorMessage = "请输入储值金额")]
        public decimal PayMoney { get; set; }

        [Display(Name = "增值金额")]
        [Required(ErrorMessage = "请输入增值金额")]
        public decimal SendMoney { get; set; }

        [Display(Name = "单号")]
        public string InvNo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
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