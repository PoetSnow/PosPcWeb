using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ReportQueryParaTemp")]
    [LogCName("报表查询参数临时存放地。")]
    public class ReportQueryParaTemp
    {
        [LogCName("创建时间")]
        public DateTime Createdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店ID")]
        public string Hid { get; set; }

        [Key]
        [LogCName("主键ID")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("参数内容")]
        public string Value { get; set; }

    }
}