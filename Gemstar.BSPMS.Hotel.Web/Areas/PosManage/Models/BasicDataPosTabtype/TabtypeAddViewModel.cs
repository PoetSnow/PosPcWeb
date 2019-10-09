using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosTabtype
{
    public class TabtypeAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "类别")]
        public byte? Istagclass { get; set; }

        [Display(Name = "最大座位数")]
        public int? MaxSeat { get; set; }

        [Display(Name = "出品方式")]
        public byte? ProduceType { get; set; }

        [Display(Name = "赠送所需金额")]
        public decimal? LargAmount { get; set; }

        [Display(Name = "赠送项目")]
        public string LargItem { get; set; }

        [Display(Name = "扫码点餐支付方式")]
        public byte? WxPaytype { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}