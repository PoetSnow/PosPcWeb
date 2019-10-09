using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PmsPara")]
    [LogCName("系统参数")]
    public class PmsPara
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参数代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参数名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参数的说明")]
        public string Remark { get; set; }

        [LogCName("顺序")]
        public int Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参数值")]
        public string Value { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("默认值")]
        public string DefaultValue { get; set; }

        [LogCName("是否可见")]
        public byte IsVisible { get; set; }


    }
}
