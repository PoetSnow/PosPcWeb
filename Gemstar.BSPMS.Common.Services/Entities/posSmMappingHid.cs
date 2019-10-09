using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("posSmMappingHid")]
    public class posSmMappingHid
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string HotelCode { get; set; }

        [Column(TypeName = "varchar")]
        public string HotelName { get; set; }

        [Column(TypeName = "varchar")]
        public string HId { get; set; }

        public bool? IsCs { get; set; }

        [Column(TypeName = "varchar")]
        public string NotifyURL { get; set; }

        public byte? Status { get; set; }

        public DateTime? ModifyDate { get; set; }

        [Column(TypeName = "varchar")]
        public string ModifyUser { get; set; }

        [Column(TypeName = "varchar")]
        public string GsWxComid { get; set; }

        [Column(TypeName = "varchar")]
        public string GsWxOpenidUrl { get; set; }

        [Column(TypeName = "varchar")]
        public string GsWxTemplateMessageUrl { get; set; }

        [Column(TypeName = "varchar")]
        public string GsWxCreatePayOrderUrl { get; set; }

        [Column(TypeName = "varchar")]
        public string GsWxPayOrderUrl { get; set; }
    }
}
