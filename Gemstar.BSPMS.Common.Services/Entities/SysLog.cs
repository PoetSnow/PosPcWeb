using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    /// <summary>
    /// 错误日志
    /// </summary>
    [Table("sysLog")]
    public class SysLog
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "char")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Url { get; set; }

        [Column(TypeName = "varchar")]
        public string Ip { get; set; }

        public DateTime CDate { get; set; }
        
        [Column(TypeName = "varchar")]
        public string User { get; set; }

        [Column(TypeName = "varchar")]
        public string Info { get; set; }
    }
}
