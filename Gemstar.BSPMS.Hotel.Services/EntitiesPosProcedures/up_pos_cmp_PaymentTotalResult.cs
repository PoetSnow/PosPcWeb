using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_cmp_PaymentTotal 执行后的结果集对象
    /// </summary>
    public class up_pos_cmp_PaymentTotalResult
    {
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal? Consume { get; set; }

        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal? Paid { get; set; }

        /// <summary>
        /// 未付金额
        /// </summary>
        public decimal? UnPaid { get; set; }

        /// <summary>
        /// 尾数处理金额
        /// </summary>
        public decimal? TailDifference { get; set; }
    }
}
