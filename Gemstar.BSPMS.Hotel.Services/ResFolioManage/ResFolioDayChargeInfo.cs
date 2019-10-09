using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 房租加收修改授权类
    /// </summary>
    public class ResFolioDayChargeInfo
    {
        public string RegId { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string RoomTypeName { get; set; }
        /// <summary>
        /// 收取的类型,取值：全日租，半日租
        /// </summary>
        public string OriginType { get; set; }
        /// <summary>
        /// 收取的类型,取值：全日租，半日租
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 收取的金额
        /// </summary>
        public decimal OriginAmount { get; set; }
        /// <summary>
        /// 收取的金额
        /// </summary>
        public decimal Amount { get; set; }


    }
}
