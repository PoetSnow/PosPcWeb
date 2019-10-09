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

    [Table("RoomOwnerCalcDispPara")]
    [LogCName("分成展示项目定义")]
    public class RoomOwnerCalcDispPara
    {
        [Key]
        [Display(Name = "项目id")]
        [LogCName("项目id")]
        public Guid TypeId { get; set; }

        [Display(Name = "酒店id")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入项目名称")]
        [Display(Name = "项目名称")]
        [LogCName("项目名称")]
        public string TypeName { get; set; }

        [Display(Name = "项目说明")]
        [LogCName("项目说明")]
        public string TypeDesc { get; set; }

        [Display(Name = "项目计算公式")]
        [LogCName("项目计算公式")]
        public string CalcFormula { get; set; }

        [Display(Name = "排序号")]
        [LogCName("排序号")]
        public int? SeqId { get; set; }

        [Display(Name = "是否隐藏")]
        [LogCName("是否隐藏")]
        public bool? isHidden { get; set; }

        [Display(Name = "是否汇总")]
        [LogCName("是否汇总")]
        public bool? isNeedSum { get; set; }

        [Display(Name = "数据类型")]
        [LogCName("数据类型")]
        public string dataType { get; set; }
    }
}
