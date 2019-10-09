using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单明细中的门锁卡信息，主要用于获取门锁卡的最新状态并表格显示
    /// </summary>
    public class ResDetailLockInfo
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 子单ID
        /// </summary>
        public string RegId { get; set; }

        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 房间锁ID
        /// </summary>
        public string RoomLockId { get; set; }

        /// <summary>
        /// 入住日期
        /// </summary>
        public string ArrDate { get; set; }

        /// <summary>
        /// 离店日期
        /// </summary>
        public string DepDate { get; set; }

        /// <summary>
        /// 房间内一张或多张门锁卡
        /// </summary>
        public List<ResDetailLockDetailInfo> LockCardList { get; set; }

        /// <summary>
        /// 是否有 发卡和重写卡 权限 true有，false无
        /// </summary>
        public bool IsWrite { get; set; }
        /// <summary>
        /// 是否有密码锁
        /// </summary>
        public bool IsOnlineLock { get; set; }
    }

    /// <summary>
    /// 门锁卡详细信息
    /// </summary>
    public class ResDetailLockDetailInfo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 发卡时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
    }


}
