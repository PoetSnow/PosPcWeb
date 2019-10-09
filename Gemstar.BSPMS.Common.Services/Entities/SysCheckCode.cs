using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("sysCheckCode")]
    public class SysCheckCode
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string UserCode { get; set; }

        [Column(TypeName = "varchar")]
        public string GetMethod { get; set; }

        [Column(TypeName = "varchar")]
        public string GetMethodValue { get; set; }

        [Column(TypeName = "varchar")]
        public string CheckCode { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "varchar")]
        public string Func { get; set; }

    }
}
