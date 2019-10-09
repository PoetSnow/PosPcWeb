using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosHoliday
{
    public class HolidayAddViewModel
    {
       

        [Display(Name = "日期")]
        [Required(ErrorMessage = "请输入日期")]
        public string VDate { get; set; }


        [Display(Name = "节日名")]
        [Required(ErrorMessage = "请输入节日名")]
        public string DaysName { get; set; }


        [Display(Name = "备注")]
        public string Remark { get; set; }


    }
}