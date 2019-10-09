using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("UserRole")]
    [LogCName("操作员权限")]
    public class UserRole
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

        [LogCName("操作员id")]
        [LogAnywayWhenEdit]
        public Guid Userid { get; set; }

        [LogCName("角色id")]
        public Guid Roleid { get; set; }

    }
}
