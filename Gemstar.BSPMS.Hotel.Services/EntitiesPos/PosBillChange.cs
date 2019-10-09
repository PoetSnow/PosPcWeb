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
    [Table("posBillChange")]
    [LogCName("账单操作表")]

    public class PosBillChange
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("酒店代码")]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogCName("营业日")]
        public DateTime? BillBsnsDate { get; set; }

        [LogCName("营业点Id")]
        [Column(TypeName = "varchar")]
        public string Refeid { get; set; }

        [LogCName("餐台ID")]
        [Column(TypeName = "varchar")]
        public string Tabid { get; set; }

        [LogCName("主单号")]
        [Column(TypeName = "varchar")]
        public string MBillid { get; set; }

        [LogCName("新主单号")]
        [Column(TypeName = "varchar")]
        public string NmBillid { get; set; }

        [LogCName("新营业点ID")]
        [Column(TypeName = "varchar")]
        public string Nrefeid { get; set; }

        [LogCName("新餐台ID")]
        [Column(TypeName = "varchar")]
        public string Ntabid { get; set; }

        [LogCName("账单状态")]
        public byte? iStatus { get; set; }

        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [LogCName("账单明细")]
        [Column(TypeName = "varchar")]
        public string BillRow { get; set; }

        [LogCName("模块")]
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
