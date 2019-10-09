using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass
{
    public class PosItemMultiClassEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "消费项目")]
        public string Itemid { get; set; }

        [Display(Name = "上级分类")]
        [Required(ErrorMessage = "上级分类")]
        public string ItemClassid { get; set; }

        [Display(Name = "是否分类")]
        public bool? IsSubClass { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? Modified { get; set; }
    }
}