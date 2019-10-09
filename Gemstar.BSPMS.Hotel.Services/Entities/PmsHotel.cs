using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PmsHotel")]
    [LogCName("酒店信息")]
     public class PmsHotel
    {
        [Column(TypeName = "varchar")]
        [LogCName("所属集团代码")]
        public string Grpid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("省份")]
        public string Provinces { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("城市")]
        public string City { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("星级")]
        public string Star { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机号")]
        public string Mobile { get; set; }

        [LogCName("状态")]
        public byte Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("地址")]
        public string Address { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("电话")]
        public string Tel { get; set; }

        [Column(TypeName = "int")]
        [LogCName("序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("经纬度")]
        public string Coordinate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店简称")]
        public string Hotelshortname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("管理类型")]
        public string ManageType { get; set; }

    }
}
