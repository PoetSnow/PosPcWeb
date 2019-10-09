using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("M_v_codeListPub")]
    public class M_v_codeListPub
    {
        [Column(TypeName = "varchar")]
        public string TypeCode { get; set; }

        [Column(TypeName = "varchar")]
        public string TypeName { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }
            
        public int? Status { get; set; }
       
        public int? Seqid { get; set; }
    }
}
