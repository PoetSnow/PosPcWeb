using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardDetailViewModel
    {
        [Display(Name = "酒店代码")]
        public string hid { get; set; }

        public Guid id { get; set; }

        [Display(Name = "会员卡号")]
        public string mbrCardNo { get; set; }

        public string mbrCardTypeId { get; set; }

        [Display(Name = "会员卡类型")]
        public string mbrCardTypeName { get; set; }

        [Display(Name = "姓名")]
        public string guestName { get; set; }

        [Display(Name = "性别")]
        public string gender { get; set; }

        [Display(Name = "业务员")]
        public string sales { get; set; }

        [Display(Name = "入会日期")]
        public DateTime? joinDate { get; set; }

        [Display(Name = "证件类型")]
        public string cerType { get; set; }

        [Display(Name = "证件号")]
        public string cerid { get; set; }

        [Display(Name = "生日")]
        public DateTime? birthday { get; set; }

        [Display(Name = "住址")]
        public string address { get; set; }

        [Display(Name = "籍贯")]
        public string city { get; set; }

        [Display(Name = "手机号")]
        public string mobile { get; set; }

        [Display(Name = "qq号")]
        public string qq { get; set; }

        [Display(Name = "邮箱")]
        public string email { get; set; }

        [Display(Name = "微信号")]
        public string weixin { get; set; }

        [Display(Name = "兴趣爱好")]
        public string interest { get; set; }

        [Display(Name = "备注")]
        public string remark { get; set; }

        [Display(Name = "会员卡密码")]
        public string pwd { get; set; }

        [Display(Name = "主卡号")]
        public string masterCardNo { get; set; }

        [Display(Name = "有效期")]
        public DateTime? validDate { get; set; }

        [Display(Name = "审核状态")]
        public int? isAudit { get; set; }

        public string isAuditName { get; set; }

        [Display(Name = "卡状态")]
        public byte? status { get; set; }

        public string statusName { get; set; }

        [Display(Name = "车牌号")]
        public string carNo { get; set; }

        [Display(Name = "最近消费日期")]
        public DateTime? lastDate { get; set; }

        [Display(Name = "最后入住日期")]
        public DateTime? lastIn { get; set; }

        [Display(Name = "累计间夜数")]
        public decimal? nigths { get; set; }

        [Display(Name = "累计消费")]
        public decimal? Amounts { get; set; }

        [Display(Name = "余额")]
        public decimal? balance { get; set; }

        [Display(Name = "余额_总获取")]
        public decimal? balanceGet { get; set; }

        [Display(Name = "余额_总使用")]
        public decimal? balanceUse { get; set; }

        [Display(Name = "赠送余额")]
        public decimal? free { get; set; }

        [Display(Name = "赠送_总获取")]
        public decimal? freeGet { get; set; }

        [Display(Name = "赠送_总使用")]
        public decimal? freeUse { get; set; }

        [Display(Name = "积分余额")]
        public int? score { get; set; }

        [Display(Name = "积分总获取")]
        public int? scoreGet { get; set; }

        [Display(Name = "积分总使用")]
        public int? scoreUse { get; set; }

        [Display(Name = "现金券可用张数")]
        public int? cashTicket { get; set; }

        [Display(Name = "现金券总张数")]
        public int? cashTicketGet { get; set; }

        [Display(Name = "现金券已使用张数")]
        public int? cashTicketUse { get; set; }

        [Display(Name = "项目券可用张数")]
        public int? itemTicket { get; set; }

        [Display(Name = "项目券总张数")]
        public int? itemTicketGet { get; set; }

        [Display(Name = "项目券已使用张数")]
        public int? itemTicketUse { get; set; }
    }
}