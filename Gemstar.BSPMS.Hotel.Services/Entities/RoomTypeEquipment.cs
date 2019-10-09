using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomTypeEquipment")]
    [LogCName("房型客房用品数量")]
    public class RoomTypeEquipment
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房型")]
        public string Roomtypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("住店类型")]
        public string LiveType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("商品编号")]
        public string Goodsid { get; set; }

        [LogCName("数量")]
        public int? Quality { get; set; }
    }
}
