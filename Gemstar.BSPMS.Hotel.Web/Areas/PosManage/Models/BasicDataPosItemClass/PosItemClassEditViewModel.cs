using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemClass
{
    public class PosItemClassEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

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

        [Display(Name = "是否分类")]
        public bool? IsSubClass { get; set; }

        [Display(Name = "所属营业点")]
        public string Refeid { get; set; }

        [Display(Name = "所属营业点")]
        public string[] Refeids
        {
            get { return string.IsNullOrEmpty(Refeid) ? new string[] { } : Refeid.Split(','); }
            set { Refeids = value; }
        }

        [Display(Name = "是否IPAD显示")]
        public bool? IsIpadShow { get; set; }
        
        [Display(Name = "背景图片")]
        public string Bmp { get; set; }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}