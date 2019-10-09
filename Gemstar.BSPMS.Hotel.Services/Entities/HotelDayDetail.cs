using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("HotelDayDetail")]
    public class HotelDayDetail
    {
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Key]
        public Guid Id { get; set; }

        public Guid? HotelDayid { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "char")]
        public string BeginDay { get; set; }

        [Column(TypeName = "char")]
        public string EndDay { get; set; }

        [Column(TypeName = "varchar")]
        public string Week { get; set; }

    }
}
