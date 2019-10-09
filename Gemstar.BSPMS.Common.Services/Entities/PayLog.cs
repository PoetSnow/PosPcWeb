using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("PayLog")]
    public class PayLog
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string Level { get; set; }

        [Column(TypeName = "varchar")]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? CDate { get; set; }

    }
}