using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 冲销减免类
    /// </summary>
    public class ResFolioAbatementInfo
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 客人名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ItemCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 单据号码
        /// </summary>
        public string InvNo { get; set; }
        /// <summary>
        /// 说明备注
        /// </summary>
        public string Remark { get; set; }
    }
}
