using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    /// <summary>
    /// 查询高级功能列表
    /// </summary>
    public class PosAdvanceFuncViewModel
    {
        public PosAdvanceFuncViewModel()
        {
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }

        [Display(Name = "营业点ID")]
        public string refeId { get; set; }

        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }

    }
}