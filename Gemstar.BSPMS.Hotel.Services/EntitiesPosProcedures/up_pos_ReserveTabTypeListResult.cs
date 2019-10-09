using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 统计餐台类型可用餐台数
    /// </summary>
    public class up_pos_ReserveTabTypeListResult
    {
        /// <summary>
        /// 餐台类型Id
        /// </summary>
        public string TabTypeId { get; set; }

        /// <summary>
        /// 餐台类型名称
        /// </summary>
        public string TabTypeName { get; set; }

        /// <summary>
        /// 可用餐台数量
        /// </summary>
        public int? KYtabNum { get; set; }

        /// <summary>
        /// 最大座位数
        /// </summary>
        public int? MaxTabSeat { get; set; }

        /// <summary>
        /// 已选数
        /// </summary>
        public int selectedQty { get; set; }
    }
}
