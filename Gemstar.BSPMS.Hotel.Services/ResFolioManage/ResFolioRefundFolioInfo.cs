using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 用于可退款账务列表
    /// </summary>
    public class ResFolioRefundFolioInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Transid { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Regid { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>
        public string Itemid { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime TransDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string InputUser { get; set; }


    }
}
