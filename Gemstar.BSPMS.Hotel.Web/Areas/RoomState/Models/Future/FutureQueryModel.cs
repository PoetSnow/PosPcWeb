using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Future
{
    /// <summary>
    /// 远期房态查询参数
    /// </summary>
    public class FutureQueryModel
    {
        public DateTime? BeginDate { get; set; }
        public int? Days { get; set; }
    }
}