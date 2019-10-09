using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ProfileLog")]
    [LogCName("会员变更记录")]
    public class ProfileLog
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("客历号")]
        public Guid Profileid { get; set; }

        [LogCName("时间")]
        public DateTime Cdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("变更的类型")]
        [LogAnywayWhenEdit]
        public string Type { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("更换前")]
        public string Old { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("更换后")]
        public string New { get; set; }

        [LogCName("使用积分")]
        public int? Score { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("集团id")]
        public string Grpid { get; set; }
    }
}
