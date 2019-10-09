using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("v_itemReserv")]
    public class V_itemReserv
    {
        [Column(TypeName = "varchar")]
        public string Itemcode { get; set; }

        [Column(TypeName = "varchar")]
        public string Itemtypecode { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Dcflag { get; set; }

        [Column(TypeName = "varchar")]
        public string Action { get; set; }

        [Column(TypeName = "varchar")]
        public string StaType { get; set; }
    }
}
