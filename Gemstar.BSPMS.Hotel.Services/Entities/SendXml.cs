using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("SendXml")]
    public class SendXml
    {
        [Key]
        public int SendId { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        public int? NotifyId { get; set; }

        public DateTime Cdate { get; set; }

        [Column(TypeName = "varchar")]
        public string SendType { get; set; }

        [Column(TypeName = "varchar")]
        public string Url { get; set; }

        public string SendContent { get; set; }

        public DateTime? SendDate { get; set; }

        public string ReceiveContent { get; set; }

        public DateTime? ReceiveDate { get; set; }

    }
}
