using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosRequest
{
    public class RequestEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名称")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "模块代码")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "要求操作")]

        [Required(ErrorMessage = "请选择要求操作")]
        public int? iTagOperator { get; set; }

        [Display(Name = "前提要求")]
        public string ReQuest { get; set; }

        [Display(Name = "联单打印")]
        public int? IsCombine { get; set; }

        [Display(Name = "要求属性")]

        public int? IsTagProperty { get; set; }

        [Display(Name = "出品状态")]
        public int? IsProduce { get; set; }

        [Display(Name = "要求时间(分钟)")]
        public int? iMinute { get; set; }

        [Display(Name = "所属营业点")]
        public string Refeid { get; set; }

        [Display(Name = "是否微信显示")]
        public bool? iShowWx { get; set; }
        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}