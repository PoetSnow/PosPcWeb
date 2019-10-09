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
    [Table("WhVoucherdetail")]
    [LogCName("凭证详情")]
    public class WhVoucherdetail
    {
        [Key]
        [LogCName("id")]
        [LogIgnore]
        public int Voucherid { get; set; }

        [LogCName("酒店id")]
        [LogIgnore]
        // public string Hid { get; set; }

        //varcharno, --凭证号
        public int Varcharno { get; set; }

        //凭证类型
        public string Vouchertype { get; set; }
        // 操作员
        public string Creator { get; set; }
        //  日期范围
        public string Dates { get; set; }
        //  备注
        public string Remark { get; set; }
        //明细 id, 用来排序不用显示
        public int VoucherDetailId { get; set; }
        //科目代友
        public string Ssubjectcode { get; set; }
        //科目 名称
        public string SSubjectName { get; set; }
        //核算项目代码
        public string Saccountcode { get; set; }
        //核算项目名称
        public string SaccountName { get; set; }
        // 借方金额
        public Decimal? SamountD { get; set; }
        //贷方金额
        public Decimal? SamountC { get; set; }
    }
}
