using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.RoomStatusManage
{
    /// <summary>
    /// 房间维修停用 展示需要的参数
    /// </summary>
    public class RoomStatusServiceAndStopInfo
    {
        /// <summary>
        /// 主键ID RoomServiceLog表 房间维修停用记录表
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 类型（1：Service维修，2：Stop停用）内部使用
        /// </summary>
        public byte _Type { get; set; }
        /// <summary>
        /// 类型（1：Service维修，2：Stop停用）
        /// </summary>
        public RoomStatusServiceAndStopFlag Type { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlanEndDateTime { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 设置人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 解除人
        /// </summary>
        public string EndUser { get; set; }

        /// <summary>
        /// /维修人
        /// </summary>
        public string ServiceUser { get; set; }
         
        /// <summary>
        /// 房间打扫
        /// </summary>
        public bool IsRoomClean { get; set; }

        /// <summary>
        /// 服务员
        /// </summary>
        public string CleanWaiter { get; set; }
    }
}