using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomTypeManage
{
    public class RoomTypeAddViewModel
    {
        [Display(Name = "房型代码")]
        [Required(ErrorMessage = "请输入房型代码")]
        public string Code { get; set; }

        [Display(Name = "房型名称")]
        [Required(ErrorMessage = "请输入房型名称")]
        public string Name { get; set; }

        [Display(Name = "简称")]
        [Required(ErrorMessage = "请输入简称")]
        public string ShortName { get; set; }

        [Display(Name = "牌价")]
        public decimal? Price { get; set; }

        [Display(Name = "标准早餐数")]
        public byte? Count { get; set; }

        [Display(Name = "最大人数")]
        public byte? MaxCount { get; set; }

        [Display(Name = "是否可加床")]
        public bool? IsAdd { get; set; }

        [Display(Name = "是否渠道可用")]
        public bool? ChanelValid { get; set; }

        [Display(Name = "是否关闭渠道")]
        public bool? IsClose { get; set; }

        [Display(Name = "是否假房")]
        public bool? IsNotRoom { get; set; }

        [Display(Name = "可超预订数")]
        public int? OverQauntity { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "房型描述")]
        public string Cdescribe { get; set; }

        [Display(Name = "图片")]
        public string PicAdd { get; set; }

        [Display(Name = "rt1")]
        public string Rt1 { get; set; }

        [Display(Name = "rt2")]
        public string Rt2 { get; set; }

        [Display(Name = "rt3")]
        public string Rt3 { get; set; }

        [Display(Name = "rt4")]
        public string Rt4 { get; set; }

        [Display(Name = "rt5")]
        public string Rt5 { get; set; }

        [Display(Name = "rt6")]
        public string Rt6 { get; set; }

        [Display(Name = "rt7")]
        public string Rt7 { get; set; }

        [Display(Name = "rt8")]
        public string Rt8 { get; set; }

        [Display(Name = "夜审置脏")]
        public bool? isChangeDirty { get; set; }

        [Display(Name = "离店置脏")]
        public bool? isDepChangeDirty { get; set; }
    }
}