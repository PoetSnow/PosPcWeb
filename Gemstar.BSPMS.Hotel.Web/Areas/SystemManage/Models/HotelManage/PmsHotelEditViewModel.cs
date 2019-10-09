using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.HotelManage
{
    public class PmsHotelEditViewModel : BaseEditViewModel
    {
        [Display(Name = "所属集团")]
        [Column(TypeName = "varchar")]
        public string Grpid { get; set; }

        [Display(Name = "酒店代码")]
        [Key]
        [Column(TypeName = "char")]
        public string Hid { get; set; }
        
        [Display(Name = "酒店名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name = "省份")]
        [Column(TypeName = "varchar")]
        public string Provinces { get; set; }

        [Display(Name = "城市")]
        [Column(TypeName = "varchar")]
        public string City { get; set; }

        [Display(Name = "星级")]
        [Column(TypeName = "varchar")]
        public string Star { get; set; }

        [Display(Name = "邮箱")]
        [Column(TypeName = "varchar")]
        public string Email { get; set; }

        [Display(Name = "手机号")]
        [Column(TypeName = "varchar")]
        public string Mobile { get; set; }

        [Display(Name = "状态")]
        public byte Status { get; set; }

        [Display(Name = "详细地址")]
        [Column(TypeName = "varchar")]
        public string address { get; set; }


        [Display(Name = "电话")]
        [Column(TypeName = "varchar")]
        public string tel { get; set; }


        [Display(Name = "排序号")]
        [Column(TypeName = "int")]
        public int? Seqid { get; set; }

        [Display(Name = "经纬度")]
        [Column(TypeName = "varchar")]
        public string Coordinate { get; set; } 

        [Display(Name = "酒店简称")]
        [Column(TypeName = "varchar")]
        public string Hotelshortname { get; set; }

        [Display(Name = "管理类型")]
        [Column(TypeName = "varchar")] 
        public string ManageType { get; set; }
    }
}