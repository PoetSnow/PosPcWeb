using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 调价信息类
    /// </summary>
    public class ResAdjustPriceInfo
    {
        /// <summary>
        /// 调价订单
        /// </summary>
        public class AdjustPriceOrderModel
        {
            /// <summary>
            /// 账号ID
            /// </summary>
            public string RegId { get; set; }

            /// <summary>
            /// 客人名
            /// </summary>
            public string GuestName { get; set; }
            /// <summary>
            /// 房号
            /// </summary>
            public string RoomNo { get; set; }
            /// <summary>
            /// 房号
            /// </summary>
            public string OriginRoomNo { get; set; }

            /// <summary>
            /// 价格代码ID
            /// </summary>
            public string RateCodeId { get; set; }
            /// <summary>
            /// 房间类型ID
            /// </summary>
            public string RoomTypeId { get; set; }

            /// <summary>
            /// 源价格代码ID
            /// </summary>
            public string OriginRateCodeId { get; set; }
            /// <summary>
            /// 源房间类型ID
            /// </summary>
            public string OriginRoomTypeId { get; set; }

            /// <summary>
            /// 价格开始时间
            /// </summary>
            public DateTime? BeginAdjustPriceDate { get; set; }
            /// <summary>
            /// 价格结束时间
            /// </summary>
            public DateTime? EndAdjustPriceDate { get; set; }
        }

        /// <summary>
        /// 操作来源
        /// </summary>
        public enum AdjustPriceOperationSource
        {
            /// <summary>
            /// 订单批量添加
            /// </summary>
            [Description("批量新客单")]
            OrderBatchAdded,
            /// <summary>
            /// 新订单
            /// </summary>
            [Description("新客单")]
            OrderAdded,
            /// <summary>
            /// 订单维护
            /// </summary>
            [Description("客单")]
            OrderModified,
            /// <summary>
            /// 换房
            /// </summary>
            [Description("换房")]
            OrderChangeRoom,
        }

        /// <summary>
        /// 调价结果
        /// </summary>
        public class AdjustPriceResultModel
        {
            /// <summary>
            /// 操作来源
            /// </summary>
            public AdjustPriceOperationSource Source { get; set; }

            /// <summary>
            /// 账号
            /// </summary>
            public string RegId { get; set; }
            /// <summary>
            /// 客人名
            /// </summary>
            public string GuestName { get; set; }
            /// <summary>
            /// 房号
            /// </summary>
            public string RoomNo { get; set; }
            /// <summary>
            /// 房号
            /// </summary>
            public string OriginRoomNo { get; set; }

            /// <summary>
            /// 价格代码ID
            /// </summary>
            public string RateCodeId { get; set; }
            /// <summary>
            /// 价格代码名称
            /// </summary>
            public string RateCodeName { get; set; }
            /// <summary>
            /// 房间类型ID
            /// </summary>
            public string RoomTypeId { get; set; }
            /// <summary>
            /// 房间类型名称
            /// </summary>
            public string RoomTypeName { get; set; }

            /// <summary>
            /// 源价格代码ID
            /// </summary>
            public string OriginRateCodeId { get; set; }
            /// <summary>
            /// 源价格代码名称
            /// </summary>
            public string OriginRateCodeName { get; set; }
            /// <summary>
            /// 源房间类型ID
            /// </summary>
            public string OriginRoomTypeId { get; set; }
            /// <summary>
            /// 源房间类型名称
            /// </summary>
            public string OriginRoomTypeName { get; set; }

            /// <summary>
            /// 调价列表
            /// </summary>
            public List<ResDetailPlan> AdjustPriceList { get; set; }
        }

        /// <summary>
        /// 调价信息
        /// </summary>
        public class ResDetailPlan
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Regid { get; set; }
            /// <summary>
            /// 日期
            /// </summary>
            public DateTime Ratedate { get; set; }

            /// <summary>
            /// 原价格
            /// </summary>
            public decimal? OriginPrice { get; set; }
            /// <summary>
            /// 价格体系
            /// </summary>
            public decimal? PlanPrice { get; set; }
            /// <summary>
            /// 新价格
            /// </summary>
            public decimal? Price { get; set; }
        }
    }
}
