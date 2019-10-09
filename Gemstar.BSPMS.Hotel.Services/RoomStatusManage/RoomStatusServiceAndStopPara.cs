using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.RoomStatusManage
{
    /// <summary>
    /// 房间维修停用 表单提交的参数
    /// </summary>
    public class RoomStatusServiceAndStopPara
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 类型（1：Service维修，2：Stop停用）
        /// </summary>
        public RoomStatusServiceAndStopFlag Type { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime PlanEndDateTime { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 维修人
        /// </summary>
        public string ServiceUser { get; set; }

        /// <summary>
        /// 房间清洁
        /// </summary>
        public bool IsRoomClean { get; set; }

        /// <summary>
        /// 服务员
        /// </summary>
        public string CleanWaiter { get; set; }
    }
}
