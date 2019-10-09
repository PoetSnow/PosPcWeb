using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("Province")]
    public class Province
    {
        [Key]
        [Column(TypeName = "char")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

    }
}
