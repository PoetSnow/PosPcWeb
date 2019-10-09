using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardScoreUseViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "当前积分")]
        public int? CurrentScore { get; set; }

        [Display(Name = "当前业主分")]
        public int? CurrentOwnerScore { get; set; }

        [Display(Name = "兑换项目")]
        [Required(ErrorMessage = "请选择兑换项目")]
        public string ItemId { get; set; }

        [Display(Name = "兑换方式")]
        [Required(ErrorMessage = "请选择兑换方式")]
        public string ItemType { get; set; }

        [Display(Name = "扣除积分")]
        public int Score { get; set; }

        [Display(Name = "扣除积分")]
        public int PartScore { get; set; }

        [Display(Name = "原币金额")]
        public decimal OriginPartMoney { get; set; }

        [Display(Name = "扣除金额")]
        public decimal PartMoney { get; set; }

        [Display(Name = "付款方式")]
        public string PayWayId { get; set; }

        [Display(Name = "单号")]
        public string InvNo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}