using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Sales")]
    [LogCName("业务员资料表")]
    public class Sales
    {
        [Column(TypeName = "varchar")]
        [LogCName("集团代码")]
        public string Grpid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名字")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机号")]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [LogCName("所属酒店")]
        public string Belonghotel { get; set; }

    }
}
