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
    [Table("RoomHold")]
    [LogCName("保留房设置")]
    public class RoomHold
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("渠道id")]
        [LogAnywayWhenEdit]
        public string Channelid { get; set; }

        [LogCName("日期")]
        public DateTime HoldDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房类id")]
        [LogAnywayWhenEdit]
        public string RoomTypeid { get; set; }

        [LogCName("保留房数")]
        public int RoomQty { get; set; }

        [LogCName("限额")]
        public int? UpperQty { get; set; }

    }
}
