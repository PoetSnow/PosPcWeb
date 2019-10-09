using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("HotelFunctions")]
    public class HotelFunctions
    {
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string FuncCode { get; set; }

        [Column(TypeName = "varchar")]
        public string FuncName { get; set; } 

        public bool? Isvalid { get; set; }

        [Column(TypeName = "varchar")]
        public string InterfaceKey { get; set; }
    }
}
