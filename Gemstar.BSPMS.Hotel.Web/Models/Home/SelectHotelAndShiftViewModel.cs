using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Models.Home
{
    /// <summary>
    /// 选择酒店和班次视图模型
    /// </summary>
    public class SelectHotelAndShiftViewModel:SelectShiftViewModel
    {
        /// <summary>
        /// 可操作分店列表
        /// </summary>
        public List<UpQueryResortListForOperatorResult> ResortList { get; set; }
        public string CurrentHotelId { get; set; }
        public string CurrentHotelName { get; set; }
    }
}