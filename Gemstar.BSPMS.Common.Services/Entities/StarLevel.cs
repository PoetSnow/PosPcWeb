using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    public class StarLevel
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string code { get; set; }
        [Column(TypeName = "varchar")]
        public string name { get; set; }
    }
}
