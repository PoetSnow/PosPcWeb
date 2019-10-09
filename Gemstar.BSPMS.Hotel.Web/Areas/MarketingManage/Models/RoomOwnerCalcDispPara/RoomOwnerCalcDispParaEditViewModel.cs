using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerCalcDispPara
{
    public class RoomOwnerCalcDispParaEditViewModel : BaseEditViewModel
    {
        [Key]
        [Display(Name = "项目id")]
        public Guid TypeId { get; set; }

        [Display(Name = "酒店id")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入项目名称")]
        [Display(Name = "项目名称")]
        [Column(TypeName = "varchar")]
        public string TypeName { get; set; }

        [Display(Name = "项目说明")]
        [Column(TypeName = "varchar")]
        public string TypeDesc { get; set; }

        [Display(Name = "项目计算公式")]
        [Column(TypeName = "varchar")]
        public string CalcFormula { get; set; }

        [Display(Name = "排序号")]
        public int? SeqId { get; set; }

        [Display(Name = "是否隐藏")] 
        public bool? isHidden { get; set; }

        [Display(Name = "是否汇总")]
        public bool? isNeedSum { get; set; }

        [Display(Name = "数据类型")]
        public string dataType { get; set; }
    }
}