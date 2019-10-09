using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosCustomerType
{
    public class CustomerTypeAddViewModel
    {
        [Required(ErrorMessage = "请输入代码")]
        [Display(Name = "代码")]
        public string Code { get; set; }

        [Required(ErrorMessage = "请输入名称")]
        [Display(Name = "名称")]
        public string Cname { get; set; }
        
        [Display(Name = "英文名")]
        public string Ename { get; set; }
        
        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "状态")]
        public byte? Status { get; set; }

        [Display(Name = "是否默认")]
        public bool? IsDefault { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}