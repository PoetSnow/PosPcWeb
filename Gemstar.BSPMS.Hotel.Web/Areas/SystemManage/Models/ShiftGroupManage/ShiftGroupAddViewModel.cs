using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Web.Models.BasicDatas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ShiftGroupManage
{
    public class ShiftGroupAddViewModel : BasicDataGroupAddViewModel
    { 

        [Column(TypeName = "varchar")]
        [LogCName("班次代码")]
        [Display(Name = "班次代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("班次名")]
        [Display(Name = "班次名")]
        [LogAnywayWhenEdit]
        public string ShiftName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        [Display(Name = "开始时间")]
        public string BeginTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        [Display(Name = "结束时间")]
        public string EndTime { get; set; } 
         

        [LogCName("顺序")]
        [Display(Name = "排序号")]
        public int? Seqid { get; set; }
    }
}