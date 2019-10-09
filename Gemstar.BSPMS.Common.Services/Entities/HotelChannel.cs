using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("HotelChannel")]
    public class HotelChannel
    {
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string ChannelCode { get; set; }

        [Column(TypeName = "varchar")]
        public string ChannelName { get; set; }

        [Column(TypeName = "varchar")]
        public string Refno { get; set; }

        public bool? Isvalid { get; set; }

        [Column(TypeName = "varchar")]
        public string InterfaceKey { get; set; }

        [Column(TypeName = "varchar")]
        public string NotifyUrl { get; set; }
    }
}
