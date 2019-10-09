using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单明细信息，主要用于表格显示，需要将其中的一些外键或枚举类型的值同时增加一名称列
    /// </summary>
    public class ResDetailInfo
    {
        public ResDetailInfo()
        {
            OrderDetailPlans = new List<ResDetailPlanInfo>();
            OrderDetailRegInfos = new List<ResDetailRegInfo>();            
        }

        /// <summary>
        /// 主键ID 登记单ID
        /// </summary>
        public string Regid { get; set; }

        /// <summary>
        /// 价格码
        /// </summary>
        public string RateCode { get; set; }

        /// <summary>
        /// 房间类型
        /// </summary>
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public bool RoomTypeIsNotRoom { get; set; }

        /// <summary>
        /// 客人来源
        /// </summary>
        public string Sourceid { get; set; }

        /// <summary>
        /// 市场分类
        /// </summary>
        public string Marketid { get; set; }

        /// <summary>
        /// 早餐份数
        /// </summary>
        public byte Bbf { get; set; }

        /// <summary>
        /// 房间数量（预订不指定房号时房间数量>=1，入住或预订分房时房间数量=1）
        /// </summary>
        public int RoomQty { get; set; }

        /// <summary>
        /// 抵店时间
        /// </summary>
        public string ArrDate { get; set; }

        /// <summary>
        /// 离店时间
        /// </summary>
        public string DepDate { get; set; }

        /// <summary>
        /// 保留时间
        /// </summary>
        public string HoldDate { get; set; }

        /// <summary>
        /// 会员
        /// </summary>
        public Guid? Profileid { get; set; }
        public string ProfileName { get; set; }
        public string ProfileNo { get; set; }
        public string ProfileMobile { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        public string StatusName { get; set; }

        /// <summary>
        /// 房间（新入住时使用）
        /// </summary>
        public string Roomid { get; set; }
        public string RoomNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 熟客（如果随行人使用了熟客表的数据，提交时保存熟客表ID）
        /// </summary>
        public Guid? Guestid { get; set; }

        /// <summary>
        /// 原始订单明细信息的json字符串
        /// </summary>
        public string OriginResDetailInfo { get; set; }

        /// <summary>
        /// 价格计划
        /// </summary>
        public List<ResDetailPlanInfo> OrderDetailPlans { get; set; }

        /// <summary>
        /// 客人信息列表中的一项Id选中
        /// </summary>
        public string SelectCustomerId { get; set; }
        /// <summary>
        /// 客人信息
        /// </summary>
        public List<ResDetailRegInfo> OrderDetailRegInfos { get; set; }

        /// <summary>
        /// 客人名 内部使用
        /// </summary>
        public string Guestname { get; set; }
        /// <summary>
        /// 客人手机号 内部使用
        /// </summary>
        public string GuestMobile { get; set; }
        /// <summary>
        /// 当前房价 内部使用
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// 酒店ID 内部使用
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 状态 只作为输出参数
        /// </summary>
        public string ResStatus { get; set; }
        public string RecStatus { get; set; }

        /// <summary>
        /// 只作为输出参数使用
        /// </summary>
        public Rate RateCodeEntity { get; set; }
        public string ArrBsnsDate { get; set; }
    }
}