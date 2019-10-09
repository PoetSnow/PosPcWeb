using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ProfileCard")]
    [LogCName("会员卡券表")]
    public class ProfileCard
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("券号")]
        [LogAnywayWhenEdit]
        public string TicketNo { get; set; }

        [LogCName("客历id")]
        public Guid Profileid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("券类型id")]
        public string TicketTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("发放时间")]
        public string CDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("使用时间")]
        public string UseDate { get; set; }

        [LogCName("状态")]
        public byte? Status { get; set; }

        [LogCName("生效时间")]
        public DateTime? BeginDate { get; set; }

        [LogCName("失效时间")]
        public DateTime? EndDate { get; set; }

        [LogCName("备注")]
        public string Remarks { get; set; }
    }
}
