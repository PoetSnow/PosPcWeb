using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("m_v_channelPara")]
    public class M_v_channelPara
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Value { get; set; }

        [Column(TypeName = "varchar")]
        public string Switch { get; set; }
    }
}
