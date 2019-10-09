using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomServiceLog")]
    [LogCName("房间维修停用记录")]
    public class RoomServiceLog
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间id")]
        public string Roomid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        public string RoomNo { get; set; }

        [LogCName("房型id")]
        public byte Type { get; set; }

        [LogCName("开始营业日")]
        public DateTime? BeginBsnsDate { get; set; }

        [LogCName("结束营业日")]
        public DateTime? EndBsnsDate { get; set; }

        [LogCName("开始时间")]
        public DateTime? BeginDate { get; set; }

        [LogCName("结束日期")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("维修原因")]
        public string Reasons { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("创建操作员")]
        public string CreateUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("完成操作员")]
        public string EndUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("维修员")]
        public string Worker { get; set; }

        [LogCName("预计完成时间")]
        public DateTime? OrderEndDate { get; set; }

        [LogCName("房间清洁")]
        public bool IsRoomClean { get; set; }

        [LogCName("服务员")]
        public string CleanWaiter { get; set; }


    }
}
