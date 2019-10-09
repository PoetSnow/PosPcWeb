using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.ComponentModel.DataAnnotations;
namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ShiftManage
{ 
      public class ShiftManageViewModel
    { 
        [Required(ErrorMessage = "请输入酒店代码")]
        [Display(Name = "酒店代码")]
        public string hid { get; set; }

        [Required(ErrorMessage = "请输入班次id")]
        [Display(Name = "班次id")]
        public string id { get; set; }

        [Display(Name = "班次代码")]
        public string code { get; set; }

        [Display(Name = "班次名")]
        public string shiftName { get; set; }

        [Display(Name = "开始时间")]
        public string beginTime { get; set; }

        [Display(Name = "结束时间")]
        public string endTime { get; set; }

        [Display(Name = "班次登录状态")]
        public string loginStatus { get; set; }

        [Required(ErrorMessage = "请输入状态")]
        [Display(Name = "状态")]
        public string status { get; set; }

        [Display(Name = "顺序")]
        public string seqid { get; set; }

        public string OriginshiftJsonData { get; set; }
    }
}
