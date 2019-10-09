using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Picture
{
    public class IndexViewModel
    {
        /// <summary>
        /// 是否启用清洁房检查功能
        /// </summary>
        public bool IsRoomCheck { get; set; }

        /// <summary>
        /// 是否启用脏房入住
        /// </summary>
        public bool IsDirtyRoomCheckIn { get; set; }

        /// <summary>
        /// 是否启用脏房转净房生成报表
        /// </summary>
        public bool IsDirtyLog { get; set; }
        /// <summary>
        /// 房态刷新时间间隔
        /// </summary>
        public int RefreshIntervalSeconds { get; set; }
        /// <summary>
        /// 是否启用长租管理功能
        /// </summary>
        public bool IsPermanentRoom { get; set; }

        /// <summary>
        /// 是否打开新预订和新入住菜单
        /// </summary>
        public bool IsContainNewOrder { get; set; }
    }
}