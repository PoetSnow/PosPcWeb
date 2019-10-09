using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Hotel.Web.Models.BasicDatas;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.BookingNotes
{
    public class BookingNotesGroupEditViewModel: BasicDataGroupEditViewModel
    {
        [Display(Name = "酒店id")]
        [Column(TypeName = "char")]
        public string Hid { get; set; }
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name = "预订须知内容")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

    }
}