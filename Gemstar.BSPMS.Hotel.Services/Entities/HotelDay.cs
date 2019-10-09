using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("HotelDay")]
    public class HotelDay
    {
        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        public int Seqid { get; set; }

    }
}