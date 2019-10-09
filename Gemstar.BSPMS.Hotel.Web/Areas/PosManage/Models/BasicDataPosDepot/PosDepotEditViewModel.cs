using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDepot
{
    public class PosDepotEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "仓库代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }


        [Display(Name = "仓库名称")]
        [Required(ErrorMessage = "请输入名称")]
        public string Cname { get; set; }


        [Display(Name = "英文名")]
        public string Ename { get; set; }


        [Display(Name = "模块")]
        public string Module { get; set; }


        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }


        [Display(Name = "备注")]
        public string Remark { get; set; }


        public DateTime? ModifiedDate { get; set; }
    }
}