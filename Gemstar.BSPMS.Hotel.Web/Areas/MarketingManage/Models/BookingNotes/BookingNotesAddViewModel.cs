using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.BookingNotes
{
    public class BookingNotesAddViewModel
    {

        [Display(Name = "名称")]
        [Required(ErrorMessage ="请输入名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name="预订须知内容")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

    }
}