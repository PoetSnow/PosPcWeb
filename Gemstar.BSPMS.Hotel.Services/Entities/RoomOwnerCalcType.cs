using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomOwnerCalcType")]
    [LogCName("分成类型定义")]
    public class RoomOwnerCalcType
    { 
        [Key]
        [Display(Name = "分成类型id")]
        [LogCName("分成类型id")]
        public Guid TypeId { get; set; }
         
        [Display(Name = "酒店id")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入类型名称")]
        [Display(Name = "类型名称")]
        [LogCName("类型名称")]
        public string TypeName { get; set; }

        [Display(Name = "类型说明")]
        [LogCName("类型说明")]
        public string TypeDesc { get; set; }

        [Display(Name = "类型计算公式")]
        [LogCName("类型计算公式")]
        public string CalcFormula { get; set; }

        [Display(Name = "排序号")]
        [LogCName("排序号")]
        public int? SeqId { get; set; }

    }
}
