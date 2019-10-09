using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    public class ResFolioSplitInfo
    {
        /// <summary>
        /// 参数类
        /// </summary>
        public class Para
        {
            /// <summary>
            /// 拆分数量（支持：2份、3份、4份）
            /// </summary>
            public int SplitCount { get; set; }
            /// <summary>
            /// 拆分账务列表
            /// </summary>
            public List<Folio> FolioList { get; set; }
            /// <summary>
            /// 拆分账务所属账单
            /// </summary>
            public Bill Bill { get; set; }
        }

        /// <summary>
        /// 拆分账务
        /// </summary>
        public class Folio
        {
            /// <summary>
            /// 账务ID
            /// </summary>
            public Guid TransId { get; set; }
            /// <summary>
            /// 拆分金额1
            /// </summary>
            public decimal Amount1 { get; set; }
            /// <summary>
            /// 拆分金额2
            /// </summary>
            public decimal Amount2 { get; set; }
            /// <summary>
            /// 拆分金额3
            /// </summary>
            public decimal Amount3 { get; set; }
            /// <summary>
            /// 拆分金额4
            /// </summary>
            public decimal Amount4 { get; set; }
        }

        /// <summary>
        /// 账务所属账单（A账单、B账单）
        /// </summary>
        public class Bill
        {
            /// <summary>
            /// 金额1所属账单
            /// </summary>
            public char Amount1 { get; set; }
            /// <summary>
            /// 金额2所属账单
            /// </summary>
            public char Amount2 { get; set; }
            /// <summary>
            /// 金额3所属账单
            /// </summary>
            public char Amount3 { get; set; }
            /// <summary>
            /// 金额4所属账单
            /// </summary>
            public char Amount4 { get; set; }
        }
    }
}
