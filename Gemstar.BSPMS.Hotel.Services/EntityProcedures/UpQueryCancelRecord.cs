using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
   public class UpQueryCancelRecord
    {
        public string id { get; set; }
        /// <summary>
        /// 合约单位名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 核销时间
        /// </summary>
        public string checkDate { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public string transDate { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string sales { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal consumptionAmount { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal payAmount { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string paytype { get; set; }
        /// <summary>
        /// 备注 
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 营业点
        /// </summary>
        public string cName { get; set; }

        /// <summary>
        /// 核销号
        /// </summary>
        public Guid checkNo { get; set; }
    }
}
