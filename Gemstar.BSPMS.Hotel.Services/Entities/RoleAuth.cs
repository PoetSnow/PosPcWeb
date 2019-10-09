using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoleAuth")]
    [LogCName("操作员权限")]
    public class RoleAuth
    {
        [Key]
        [Column(Order = 1)]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Key]
        [Column(Order = 2)]
        [LogCName("角色id")]
        [LogAnywayWhenEdit]
        public Guid RoleId { get; set; }

        [Key]
        [Column(Order = 3)]
        [LogCName("权限id")]
        [LogAnywayWhenEdit]
        public string AuthCode { get; set; }

        [LogCName("功能按钮权限值")]
        public Int64 AuthButtonValue { get; set; }

    }
}
