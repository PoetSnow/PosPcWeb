using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosDepot")]
    [LogCName("二级仓库")]
    public class PosDepot
    {
       
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        
        [Column(TypeName = "varchar")]
        [LogCName("仓库代码")]
        public string Code { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("仓库名称")]
        public string Cname { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

       
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

       
        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

       
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

      
        [LogCName("状态")]
        public byte? IStatus { get; set; }

    }
}
