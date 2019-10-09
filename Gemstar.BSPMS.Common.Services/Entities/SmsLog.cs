using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("SmsLog")]
    public class SmsLog
    {
        [Key]
        public long Id { get; set; }

        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string Moble { get; set; }

        public DateTime? CreatetDate { get; set; }

        public DateTime? SendDate { get; set; }

        [Column(TypeName = "varchar")]
        public string Msg { get; set; }

        [Column(TypeName = "varchar")]
        public string MsgReturn { get; set; }

        [Column(TypeName = "varchar")]
        public string CreateUse { get; set; }

    }
}
