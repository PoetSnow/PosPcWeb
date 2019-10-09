using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("percentagesPlan")]
    [LogCName("提成计划表")]
    public class percentagesPlan
    { 
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; } 
      
        [Column(TypeName = "varchar")]
        [LogCName("业务员")]
        public string SalesmanName { get; set; }
         
        [LogCName("业务员id")]
        public Guid? SalesmanId { get; set; }

        [LogCName("一月")] 
        public decimal? one { get; set; }
         
        [LogCName("二月")]
        public decimal? two { get; set; }
         
        [LogCName("三月")]
        public decimal? three { get; set; }
         
        [LogCName("四月")]
        public decimal? four { get; set; }
         
        [LogCName("五月")]
        public decimal? five { get; set; }
         
        [LogCName("六月")]
        public decimal? six { get; set; }
         
        [LogCName("七月")]
        public decimal? seven { get; set; }
         
        [LogCName("八月")]
        public decimal? eight { get; set; }
         
        [LogCName("九月")]
        public decimal? nine { get; set; }
         
        [LogCName("十月")]
        public decimal? ten { get; set; }
         
        [LogCName("十一月")]
        public decimal? eleven { get; set; }
         
        [LogCName("十二月")]
        public decimal? twelve { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("年份")]
        public string curYear { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("计划内容")]
        public string PlanSource { get; set; }
    }
}
