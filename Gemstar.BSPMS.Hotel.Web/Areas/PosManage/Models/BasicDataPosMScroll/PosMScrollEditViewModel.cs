using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMScroll
{
    public class PosMScrollEditViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
        public Guid Id { get; set; }
        
        [Display(Name = "酒店hid")]
        public string Hid { get; set; }
        
        [Display(Name = "消费项目")]
        public string Itemid { get; set; }
        
        [Display(Name = "项目代码")]
        public string ItemCode { get; set; }
        
        [Display(Name = "项目名称")]
        public string ItemName { get; set; }
        
        [Display(Name = "单位")]
        public string Unitid { get; set; }
        
        [Display(Name = "单位名称")]
        public string UnitName { get; set; }
        
        [Display(Name = "图片文件")]
        public string FileName { get; set; }

        [Display(Name = "排列序号")]
        public int? OrderBy { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}