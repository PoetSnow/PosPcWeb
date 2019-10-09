using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
        [Table("SignatureLog")]
        public class SignatureLog
        {
           
            [Key]
            public Guid Id { get; set; }

            [Column(TypeName = "varchar")]
            public string Hid { get; set; }

           [Column(TypeName = "varchar")]
            public string Url { get; set; }
        
            public byte SType { get; set; }

            public DateTime SDate { get; set; }

            [Column(TypeName = "varchar")]
            public string Remark { get; set; }

            [Column(TypeName = "varchar")]
            public string InputUser { get; set; }

            [Column(TypeName = "varchar")]
            public string Regid { get; set; }

            [Column(TypeName = "varchar")]
            public string RoomNo { get; set; }
    }
}
