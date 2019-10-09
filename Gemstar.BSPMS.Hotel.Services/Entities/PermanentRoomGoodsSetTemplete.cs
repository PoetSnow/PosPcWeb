using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PermanentRoomGoodsSetTemplete")]
    public class PermanentRoomGoodsSetTemplete
    {
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        public Guid TempleteId { get; set; }

        [Column(TypeName = "varchar")]
        public string TempleteName { get; set; }
    }
}
