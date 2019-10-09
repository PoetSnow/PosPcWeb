using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("v_codeListReserve")]
    public class V_codeListReserve
    {
        [Column(TypeName = "varchar")]
        public string Typecode { get; set; }

        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Nname { get; set; }

        [Column(TypeName = "varchar")]
        public string Nname2 { get; set; }

        [Column(TypeName = "varchar")]
        public string Nname3 { get; set; }

        [Column(TypeName = "varchar")]
        public string Nname4 { get; set; }
    }
}
