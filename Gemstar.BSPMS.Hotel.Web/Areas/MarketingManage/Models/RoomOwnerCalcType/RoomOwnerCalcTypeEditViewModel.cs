using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerCalcType
{
    public class RoomOwnerCalcTypeEditViewModel : BaseEditViewModel
    {
        [Key]
        [Display(Name = "分成类型id")] 
        public Guid TypeId { get; set; }
         
        [Display(Name = "酒店id")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入类型名称")]
        [Display(Name = "类型名称")]
        [Column(TypeName = "varchar")]
        public string TypeName { get; set; }

        [Display(Name = "类型说明")]
        [Column(TypeName = "varchar")]
        public string TypeDesc { get; set; }

        [Display(Name = "类型计算公式")]
        [Column(TypeName = "varchar")]
        public string CalcFormula { get; set; }

        [Display(Name = "排序号")] 
        public int? SeqId { get; set; }
    }
}