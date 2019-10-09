using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("WakeCall")]
    [LogCName("叫醒提醒记录表")]
    public class WakeCall
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [LogCName("叫醒ID")]
        [LogIgnore]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间ID")]
        [LogIgnore]
        public string RoomId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        [LogAnywayWhenEdit]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("提醒内容")]
        public string Content { get; set; }

        [LogCName("提醒时间")]
        public DateTime? CallTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string Creater { get; set; }

        [LogCName("创建时间")]
        public DateTime? CreateTime { get; set; }

        [LogCName("状态")]
        [LogIgnore]
        public byte? Status { get; set; }

        [LogCName("作废人")]
        [Column(TypeName = "varchar")]
        public string InvalidMan { get; set; }

        [LogCName("作废原因")]
        [Column(TypeName = "varchar")]
        public string InvalidReason { get; set; }

        [LogCName("作废时间")]
        public DateTime? InvalidTime { get; set; }

        [LogCName("提醒类型名称")]
        [LogIgnore]
        public string WakeCallTypeName { get; set; }

        [LogCName("提醒方式")]
        [LogIgnore]
        public byte WakeMethod { get; set; }

        [LogCName("外部关联号")]
        [LogIgnore]
        public string Refno { get; set; }

    }
}
