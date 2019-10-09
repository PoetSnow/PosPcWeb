
using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Role")]
    [LogCName("角色")]
    public class Role
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [LogCName("角色id")]
        public Guid Roleid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("角色名")]
        [LogAnywayWhenEdit]
        public string Authname { get; set; }

        [LogCName("顺序")]
        public int? Seqid { get; set; }

    }
}
