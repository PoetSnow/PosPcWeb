using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PercentagesPolicyCleanRoom")]
    [LogCName("打扫房间提成政策")]
    public class PercentagesPolicyCleanRoom
    {
        [Key]
        public Guid PolicyId { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string PolicyDesciption { get; set; }

        [Column(TypeName = "varchar")]
        public string RoomTypeId { get; set; }

        public decimal? ContinuedToLivePrice { get; set; }

        public decimal? CheckOutPrice { get; set; }

    }
}
