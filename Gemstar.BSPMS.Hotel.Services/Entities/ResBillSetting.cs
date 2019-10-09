using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [LogCName("账单设置")]
    [Table("ResBillSetting")]
    public class ResBillSetting
    {
        [Key]
        public int Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogCName("主单号")]
        [LogAnywayWhenEdit]
        [LogStartsWithHid]
        [LogKey]
        [Column(TypeName = "varchar")]
        public string Resid { get; set; }

        [LogCName("账单代码")]
        [Column(TypeName = "varchar")]
        public string BillCode { get; set; }

        [LogCName("账单名称")]
        [Column(TypeName = "varchar")]
        public string BillName { get; set; }

        [LogCName("消费项目")]
        [LogRefrenceName(Sql = "SELECT name FROM item WHERE id = {0}")]
        [Column(TypeName = "varchar")]
        public string DebitTypeId { get; set; }

        [LogCName("备注")]
        [Column(TypeName = "varchar")]
        public string Remarks { get; set; }

    }
}
