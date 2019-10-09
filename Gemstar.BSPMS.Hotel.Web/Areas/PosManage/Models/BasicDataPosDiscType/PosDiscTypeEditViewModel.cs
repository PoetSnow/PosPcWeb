using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDiscType
{
    public class PosDiscTypeEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "折扣代码")]
        [Required(ErrorMessage = "请输入折扣代码")]
        public string Code { get; set; }

        [Display(Name = "折扣名称")]
        [Required(ErrorMessage = "请输入折扣名称")]
        public string Cname { get; set; }
        
        [Display(Name ="英文名")]
        public string Ename { get; set; }
        
        [Display(Name ="模块")]
        public string Module { get; set; }

        [Display(Name ="折扣")]
        public decimal? Discount { get; set; }

        [Display(Name ="排列序号")]
        public int? Seqid { get; set; }
        
        [Display(Name ="备注")]
        public string Remark { get; set; }

        [Display(Name ="修改时间")]
        public DateTime? ModifiedDate { get; set; }


        [Display(Name = "类型")]
        public byte? DiscType { get; set; }
    }
}