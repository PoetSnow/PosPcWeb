using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosAdvanceFunc")]
    [LogCName("高级功能")]
    public class PosAdvanceFunc
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }
        
        [LogCName("酒店代码")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        
        [LogCName("营业点")]
        [Column(TypeName = "varchar")]
        public string RefeId { get; set; }
        
        [LogCName("功能代码")]
        [Column(TypeName = "varchar")]
        public string FuncCode { get; set; }
        
        [LogCName("功能名称一")]
        [Column(TypeName = "varchar")]
        public string Name1 { get; set; }
        
        [LogCName("功能名称二")]
        public string Name2 { get; set; }

      
        [LogCName("功能名称三")]
        [Column(TypeName = "varchar")]
        public string Name3 { get; set; }

      
        [LogCName("是否使用此功能")]
        public bool? IsUsed { get; set; }

      
        [LogCName("是否立即落单")]
        public bool? IsSave { get; set; }

      
        [LogCName("是否Ipad使用")]
        public bool? IsIpad { get; set; }

      
        [LogCName("功能类型")]
        [Column(TypeName = "varchar")]
        public string FuncType { get; set; }

      
        [LogCName("功能级别")]
        [Column(TypeName = "varchar")]
        public string FuncGrade { get; set; }


        [LogCName("模块")]
        [Column(TypeName = "varchar")]
        public string Module { get; set; }

    
        [LogCName("备注")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

       
        [LogCName("操作员")]
        [Column(TypeName = "varchar")]
        public string TransUser { get; set; }

      
        [LogCName("创建时间")]
        public DateTime? CreateDate { get; set; }
    }
}
