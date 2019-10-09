using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    /// <summary>
    /// 会员卡费收取视图模型
    /// </summary>
    public class MbrCardCardFeeViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "付款方式")]
        [Required(ErrorMessage = "请选择付款方式")]
        public string PayWayId { get; set; }

        [Display(Name = "原币卡费金额")]
        [Required(ErrorMessage = "请输入原币卡费金额")]
        public decimal OriginPayMoney { get; set; }

        [Display(Name = "卡费金额")]
        [Required(ErrorMessage = "请输入卡费金额")]
        public decimal PayMoney { get; set; }

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
        /// <summary>
        /// 是否在卡费收取成功后自动充值
        /// </summary>
        public int IsRecharge { get; set; }
    }
}