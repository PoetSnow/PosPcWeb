using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomStatusLog")]
    public class RoomStatusLog
    {
        [Key]
        public Guid Id { get; set; }

        [LogCName("酒店编号")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogCName("房间id")]
        [Column(TypeName = "varchar")]
        public string Roomid { get; set; }

        [LogCName("房号")]
        [Column(TypeName = "varchar")]
        public string RoomNo { get; set; }

        [LogCName("创建时间")]
        public DateTime CDate { get; set; }

        [LogCName("类型")]
        [Column(TypeName = "varchar")]
        public string ActionType { get; set; }

        [LogCName("原值")]
        [Column(TypeName = "varchar")]
        public string OldValue { get; set; }

        [LogCName("新值")]
        [Column(TypeName = "varchar")]
        public string NewValue { get; set; }

        [LogCName("账号")]
        [Column(TypeName = "varchar")]
        public string Regid { get; set; }

        [LogCName("操作员")]
        [Column(TypeName = "varchar")]
        public string InputUser { get; set; }

        [LogCName("备注")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        [LogCName("服务员")]
        [Column(TypeName = "varchar")]
        public string Waiter { get; set; }

        [LogCName("报表类型")]
        [Column(TypeName = "varchar")]
        public string DirtyType { get; set; }

    }
}
