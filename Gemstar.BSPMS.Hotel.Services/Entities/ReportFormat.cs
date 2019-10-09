using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ReportFormat")]
    [LogCName("报表自定义格式保存")]
    public class ReportFormat
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }
 

        [Column(TypeName = "varchar")]
        [LogCName("报表名称或单据")]
        [LogAnywayWhenEdit]
        public string ReportName { get; set; }

        [Column("ReportFormat")]
        public string ReportTemplate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("格式名称")]
        [LogAnywayWhenEdit]
        public string StyleName { get; set; }
    }
}
