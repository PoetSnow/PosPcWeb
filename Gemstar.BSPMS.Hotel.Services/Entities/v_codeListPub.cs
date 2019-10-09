using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{

    [Table("v_codeListPub")]
    public class V_codeListPub
    {
        [Column(TypeName = "varchar")]
        public string TypeCode { get; set; } 
        [Column(TypeName = "varchar")]
        public string typename { get; set; }

        [Column(TypeName = "varchar")]
        public string code { get; set; }

        [Column(TypeName = "varchar")]
        public string name { get; set; }

        
        public int? status { get; set; }

        
        public int? seqid { get; set; }

         
        public string name2 { get; set; }
 
        public string name3 { get; set; }

       
        public string name4 { get; set; }
    }
}
