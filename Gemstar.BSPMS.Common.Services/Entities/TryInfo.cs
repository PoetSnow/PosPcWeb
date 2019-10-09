using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("TryInfos")]
    public class TryInfo
    {
        [LogIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Mobile { get; set; }

        [LogIgnore]
        public DateTime? TryDate { get; set; }

    }
}
