using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("WakeCallDetil")]
    [LogCName("叫醒提醒记录通知明细记录表")]
    public class WakeCallDetil
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("通知ID")]
        public Guid Id { get; set; }

        [LogCName("叫醒ID")]
        public Guid NotifyId { get; set; }

        [LogCName("操作员ID")]
        public Guid? UserId { get; set; }

        [LogCName("操作员名字")]
        [Column(TypeName = "varchar")]
        public string UserName { get; set; }

        [LogCName("状态")]
        public byte? Status { get; set; }

        [LogCName("接单人")]
        public string Reader { get; set; }

        [LogCName("接单时间")]
        public DateTime? ReadTime { get; set; }

        [LogCName("处理人")]
        [Column(TypeName = "varchar")]
        public string DealMan { get; set; }

        [LogCName("处理时间")]
        public DateTime? DealTime { get; set; }

        [LogCName("处理内容")]
        [Column(TypeName = "varchar")]
        public string DealContent { get; set; }

    }
}
