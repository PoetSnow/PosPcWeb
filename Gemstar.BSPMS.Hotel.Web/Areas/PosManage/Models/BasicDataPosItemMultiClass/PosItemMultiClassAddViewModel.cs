using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass
{
    public class PosItemMultiClassAddViewModel
    {
        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "消费项目")]
        public string Itemid { get; set; }
        
        [Display(Name = "上级分类")]
        [Required(ErrorMessage = "上级分类")]
        public string ItemClassid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}