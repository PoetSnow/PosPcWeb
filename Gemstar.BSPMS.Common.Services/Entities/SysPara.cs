using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("SysPara")]
    public class SysPara
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Value { get; set; }

        [Column(TypeName = "varchar")]
        public string RefValue { get; set; }

        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        public int SeqId { get; set; }

    }
}
