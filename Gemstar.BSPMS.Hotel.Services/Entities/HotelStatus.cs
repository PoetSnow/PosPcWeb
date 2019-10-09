using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("HotelStatus")]
    [LogCName("酒店当前的状态，暂时保存当前营业日这个数据不能缓存，每次使用的时候要重新取。")]
    public class HotelStatus
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogCName("营业日")]
        [LogAnywayWhenEdit]
        public DateTime BsnsDate { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifyDate { get; set; }
    }
}