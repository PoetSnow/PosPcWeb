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
    [Table("RoomOwnerCalcParaDefine")]
    [LogCName("分成参数名称定义")]
    public class RoomOwnerCalcParaDefine
    {
        [Key]
        [LogCName("ParaId")]
        public Guid ParaId { get; set; }

        [Display(Name = "酒店id")]
        [LogIgnore]
        public string Hid { get; set; }

        [Display(Name = "参数类型")]
        [LogCName("参数类型")]
        public string ParaType { get; set; }

        [Display(Name = "参数代码")]
        [LogCName("参数代码")]
        public string ParaCode { get; set; }

        [Display(Name = "参数名称")]
        [LogCName("参数名称")]
        public string ParaName { get; set; }

        [Display(Name = "序号")]
        [LogCName("序号")]
        public int? ParaSeqId { get; set; }

        [Display(Name = "是否隐藏")]
        [LogCName("是否隐藏")]
        public bool? isHidden { get; set; }


        [Display(Name = "数据类型")]
        [LogCName("数据类型")]
        public string dataType { get; set; }

    }
}
