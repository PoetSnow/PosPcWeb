using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities {

    [Table("HotelProducts")]
    [LogCName("酒店产品模块")]
    public class HotelProducts {
        [Key]
        [Column(TypeName = "varchar",Order =0)]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar",Order =1)]
        [LogCName("产品代码")]
        public string ProductCode { get; set; }

        [LogCName("产品过期时间")]
        public DateTime? ExpireDate { get; set; }

    }
}