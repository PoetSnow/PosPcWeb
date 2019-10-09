using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PosItemAllQueryViewModel
    {
        [Display(Name = "部门")]
        public string DeptClassid { get; set; }

        [Display(Name = "大类")]
        public string ItemClassid { get; set; }

        [Display(Name = "分类")]
        public string SubClassid { get; set; }

        [Display(Name = "名称（编码）")]
        public string CodeAndName { get; set; }
    }
}