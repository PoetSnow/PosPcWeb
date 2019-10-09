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
    [Table("PosPrintBill")]
    [LogCName(" 消费项目表")]
    public class PosPrintBill
    {
        
        [Key]
        [LogCName("主键ID")]
        public Guid Id { get; set; }

        [LogCName("酒店ID")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogCName("账单ID")]
        [Column(TypeName = "varchar")]
        public string BillId { get; set; }

        [LogCName("营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [LogCName("收银点ID")]
        [Column(TypeName = "varchar")]
        public string PosId { get; set; }

        [LogCName("营业点ID")]
        [Column(TypeName = "varchar")]
        public string RefeId { get; set; }

        [LogCName("餐台ID")]
        [Column(TypeName = "varchar")]
        public string TabId { get; set; }

        [LogCName("状态")]
        public byte? iStatus { get; set; }

        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [LogCName("账单明细")]
        [Column(TypeName = "varchar")]
        public string BillRow { get; set; }

        [LogCName("班次ID")]
        [Column(TypeName = "varchar")]
        public string ShiftId { get; set; }

        [LogCName("市别ID")]
        [Column(TypeName = "varchar")]
        public string ShuffleId { get; set; }

        [LogCName("打印位置")]
        public int? PlaceLen { get; set; }

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
