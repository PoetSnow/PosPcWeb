using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PermanentRoomPricePlan")]
    public class PermanentRoomPricePlan
    {
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string Roomid { get; set; }

        public decimal? RoomPriceByMonth { get; set; }

        public decimal? RoomPriceByDay { get; set; }

    }
}