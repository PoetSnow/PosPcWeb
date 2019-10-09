using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.MbrCardTypeManage
{
    public class MbrCardTypeAddViewModel
    {
        [Required(ErrorMessage = "请输入会员卡类型代码")]
        [Display(Name = "会员卡类型代码")]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Required(ErrorMessage = "请输入会员卡类型名称")]
        [Column(TypeName = "varchar")]
        [Display(Name = "会员卡类型名称")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "价格代码")]
        public string RateCodeid { get; set; }
         
        [Display(Name = "升级间夜数")]
        public int? Nights { get; set; }
         
        [Display(Name = "升级积分数")]
        public int? Score { get; set; }

        [Required(ErrorMessage = "请输入会员类型等级顺序")]
        [Display(Name = "会员类型等级顺序")]
        public int Seqid { get; set; }

      
        [Display(Name = "是否自动升级")]
        [Required(ErrorMessage = "请输入是否自动升级")]
        public bool? IsAutoUp { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "积分比率")]
        public decimal? ScoreRate { get; set; }

        [Display(Name = "升级消费金额")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "请输入会员有效时长(月份)")]
        [Display(Name = "会员有效时长(月份)")] 
        public int Validdate { get; set; }

        [Display(Name = "积分有效时长(月份)")]
        public int? ScoreVdate { get; set; }

        [Display(Name ="卡费")]
        [Required(ErrorMessage = "请输入卡费")]
        public decimal CardFee { get; set; }

        [Column(TypeName = "varchar")]
        [Display(Name = "积分单位")]
        public decimal? scoreUnit { get; set; }
    }
}