using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 批量入住，批量预订增加模型
    /// </summary>
    public class ResBatchAddModel
    {
        /// <summary>
        /// 是否入住
        /// </summary>
        public int? IsCheckIn { get; set; }
        /// <summary>
        /// 继续保存
        /// </summary>
        public string saveContinue { get; set; }
        /// <summary>
        /// 是否团体
        /// </summary>
        public byte? IsGroup { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ResCustName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string ResCustMobile { get; set; }
        /// <summary>
        /// 会员
        /// </summary>
        public Guid? ProfileId { get; set; }
        /// <summary>
        /// 外部订单号
        /// </summary>
        public string ResNoExt { get; set; }
        /// <summary>
        /// 主单号
        /// </summary>
        public string Resno { get; set; }
        /// <summary>
        /// 主单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 合约单位
        /// </summary>
        public string Cttid { get; set; }
        /// <summary>
        /// 抵店时间
        /// </summary>
        public DateTime? arriveDate { get; set; }
        /// <summary>
        /// 保留时间
        /// </summary>
        public DateTime? holdDate { get; set; }
        /// <summary>
        /// 市场分类
        /// </summary>
        public string marketType { get; set; }
        /// <summary>
        /// 特殊要求
        /// </summary>
        public string special { get; set; }
        /// <summary>
        /// 离店时间
        /// </summary>
        public DateTime? depDate { get; set; }
        /// <summary>
        /// 价格代码
        /// </summary>
        public string rateCode { get; set; }
        /// <summary>
        /// 客人来源
        /// </summary>
        public string customerSource { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 此次选择的房型及房间信息
        /// </summary>
        public List<ResBatchAddRoomTypeInfo> RoomTypeInfos { get; set; }
        /// <summary>
        /// 授权完成后继续保存订单（授权主键ID+授权时间）
        /// </summary>
        public string AuthorizationSaveContinue { get; set; }
        /// <summary>
        /// 积分换房 继续保存订单 1：继续保存，其他：返回提示信息
        /// </summary>
        public string UseScoreSaveContinue { get; set; }
    }
    public class ResBatchAddRoomInfo
    {
        public string roomId { get; set; }
        public string roomNo { get; set; }
    }
    public class ResBatchAddRoomTypeInfo
    {
        public int qty { get; set; }
        public string roomTypeId { get; set; }
        /// <summary>
        /// 以逗号分隔的价格字符串
        /// </summary>
        public string priceStr { get; set; }
        /// <summary>
        /// 早餐数
        /// </summary>
        public string Bbf { get; set; }
        public List<ResBatchAddRoomInfo> RoomInfos { get; set; }
    }
}