using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 客账查询
    /// </summary>
    public class QueryBillModel
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 营业日
        /// </summary>
        public DateTime? BillBsnsDate { get; set; }


        /// <summary>
        /// 消费项目
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 最小营业额
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 最大营业额
        /// </summary>
        public decimal? MaxAmount { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayMethod { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        public string PosId { get; set; }


        /// <summary>
        /// 营业点ID
        /// </summary>
        public string RefeId { get; set; }

        /// <summary>
        /// 台号
        /// </summary>
        public string tabNo { get; set; }

    }

}
