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
    [Table("PosBillOrder")]
    [LogCName("此表是其它详细内容可以是账单、预订或其它表上的详情内容，因为不想在原表上去增加相关的字段，都可以借助这个表来保存了。")]
    public class PosBillOrder
    {
        [LogIgnore]
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Billid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("表名")]
        public string TableName { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("列名")]
        public string ColumnName { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("列值")]
        public string ColumnValue { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string TransUser { get; set; }

        [LogIgnore]
        [LogCName("创建时间")]
        public DateTime? CreateDate { get; set; }

    }
}
