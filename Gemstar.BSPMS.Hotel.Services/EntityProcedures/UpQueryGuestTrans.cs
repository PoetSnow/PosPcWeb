using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryGuestTrans
    {
        /// <summary>
        /// 客历号
        /// </summary>
        public Guid guestid { get; set; }
        /// <summary>
        /// 营业日
        /// </summary>
        public string settleBsnsdate { get; set; }
        /// <summary>
        /// 入住时间
        /// </summary>
        public string arrDate { get; set; }
        /// <summary>
        /// 离店时间
        /// </summary>
        public string depDate { get; set; }
        /// <summary>
        /// 房价
        /// </summary>
        public decimal rate { get; set; }
        /// <summary>
        /// 价格代码
        /// </summary>
        public string rateName { get; set; }
        /// <summary>
        /// 营业点
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 间夜数
        /// </summary>
        public decimal nights { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string roomNo { get; set; }
        /// <summary>
        /// 房间类型
        /// </summary>
        public string roomTypeName { get; set; }

        public string remark { get; set; }
    }
}
