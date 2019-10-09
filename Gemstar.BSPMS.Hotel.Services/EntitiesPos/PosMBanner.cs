using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosMBanner")]
    [LogCName("手机界面banner")]
    public class PosMBanner
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店hid")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("图片文件")]
        public string FileName { get; set; }

        [LogCName("排列序号")]
        public int? OrderBy { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string Creator { get; set; }

        [LogCName("创建时间")]
        public DateTime? Createdate { get; set; }
    }
}