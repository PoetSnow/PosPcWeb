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
    [Table("WhVoucher")]
    [LogCName("凭证列表")]
    public class WhVoucher
    {
        [Key]
        [LogCName("id")]
        [LogIgnore]
        public int Voucherid { get; set; }

        [LogCName("凭证字")]
        public string Vouchertype { get; set; }

        [LogCName("凭证号")]
        public int Varcharno { get; set; }

        [LogCName("凭证日期")]
        public DateTime VoucherDate { get; set; }

        [LogCName("操作员")]
        public string Creator { get; set; }

        [LogCName("创建日期")]
        public DateTime Createdate { get; set; }

        [LogCName("修改人")]
        public string Modificator { get; set; }

        [LogCName("修改日期")]
        public DateTime ModifiedDate { get; set; }

        [LogCName("状态")]
        public Byte Status { get; set; }

        [LogCName("日期区间")]
        public string Dates { get; set; }

        [LogCName("")]
        public string Bills { get; set; }

        [LogCName("备注")]
        public string Remark { get; set; }


    }
}
