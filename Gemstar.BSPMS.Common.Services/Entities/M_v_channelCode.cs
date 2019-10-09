using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("M_v_channelCode")]
    public class M_v_channelCode
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Type { get; set; }

        [Column(TypeName = "varchar")]
        public string Switch { get; set; }

       
        public int? Seq { get; set; }

    }
}
