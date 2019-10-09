using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RateManage
{
    public class RateEditViewModel : BaseEditViewModel
    {
        [Key]
        [Column(TypeName = "varchar")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "酒店代码")]
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }
         

        [Required(ErrorMessage = "价格名称不能为空")]
        [Display(Name = "名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name = "备注")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }


        [Display(Name = "半日租收取时间")]
        [Column(TypeName = "char")]
        public string HalfTime { get; set; }


        [Display(Name = "日租收取时间")]
        [Column(TypeName = "char")]
        public string DayTime { get; set; }


        [Display(Name = "是否有早餐")]
        public byte? Bbf { get; set; }


        [Display(Name = "服务费率")]
        [Column(TypeName = "varchar")]
        public string SrvRate { get; set; }


        [Display(Name = "市场分类")]
        [Column(TypeName = "varchar")]
        public string Marketid { get; set; }


        [Display(Name = "客人来源")]
        [Column(TypeName = "varchar")]
        public string Sourceid { get; set; }


        [Display(Name = "预订须知")]
        public Guid? BookingNotesid { get; set; }


        [Display(Name = "是否适用散客")]
        public bool? IsWalkIn { get; set; }


        [Display(Name = "适用合约单位类型")]
        [Column(TypeName = "varchar")]
        public string CompanyTypes { get; set; }


        [Display(Name = "适用会员类型")]
        [Column(TypeName = "varchar")]
        public string MbrCardTypes { get; set; }


        [Display(Name = "适合渠道")]
        [Column(TypeName = "varchar")]
        public string Channelids { get; set; }


        [Display(Name = "适合房型")]
        [Column(TypeName = "varchar")]
        public string RoomTypeids { get; set; }


        [Display(Name = "线上支付方式")]
        [Column(TypeName = "varchar")]
        public string PayTypeids { get; set; }


        [Display(Name = "价格方式")]
        [Column(TypeName = "varchar")]
        public string PriceMode { get; set; }


        [Display(Name = "关联价格")]
        [Column(TypeName = "varchar")]
        public string RefRateid { get; set; }


        [Display(Name = "增减方式")]
        public bool? AddMode { get; set; }


        [Display(Name = "增减金额")]
        public decimal? AddAmount { get; set; }


        [Display(Name = "增减％")]
        public decimal? AddRate { get; set; }

        [Display(Name = "携程价格代码")]
        [Column(TypeName = "varchar")]
        public string CtripCode { get; set; }

        [Display(Name = "生效时间")]
        public DateTime? beginDate { get; set; }

        [Display(Name = "失效时间")]
        public DateTime? endDate { get; set; }


        [Display(Name = "钟点房")]
        public bool? isHou { get; set; }

        [Display(Name = "基础时长")]
        public short? baseMinute { get; set; }

        [Display(Name = "超时分钟")]
        public short? addMinute { get; set; }

        [Display(Name = "分钟价格")]
        public decimal? addPrice { get; set; }

        [Display(Name = "排序号")]
        public int Seqid { get; set; } 

        [Display(Name = "需检查会员")]
        public string NoPrintProfile { get; set; }

        [Display(Name = "需检查合约单位")]
        public string NoPrintCompany { get; set; }

        [Display(Name = "长包房")]
        public bool? isMonth { get; set; }



        [Display(Name = "白日房")]
        public bool? IsDayRoom { get; set; }

        [Display(Name = "白日房超时 时间点")]
        [Column(TypeName = "char")]
        public string DayRoomTime { get; set; }

        [Display(Name = "白日房超时 每多少分钟")]
        public short? DayRoomAddMinute { get; set; }

        [Display(Name = "白日房超时 每多少分钟多少价格")]
        public decimal? DayRoomAddPrice { get; set; }



        [Display(Name = "时间段内可入住 开始时间")]
        [Column(TypeName = "char")]
        public string StartTime { get; set; }

        [Display(Name = "时间段内可入住 结束时间")]
        [Column(TypeName = "char")]
        public string EndTime { get; set; }

        [Display(Name = "是否可调价")]
        public bool? IsPriceAdjustment { get; set; }

        [Display(Name = "是否积分换房")]
        public bool IsUseScore { get; set; }

        [Display(Name = "是否可积分")]
        public bool IsGetScore { get; set; }
    }
}
