namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 预订明细查询参数
    /// 因为参数比较多，如果直接写着方法里面，会导致方法签名很长，并且不好扩展增加查询参数
    /// </summary>
    public class ResDetailQueryPara
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string HotelId { get; set; }
        /// <summary>
        /// 开始抵店日期
        /// </summary>
        public string ArrDateBegin { get; set; }
        /// <summary>
        /// 结束抵店日期
        /// </summary>
        public string ArrDateEnd { get; set; }
        /// <summary>
        /// 开始离店日期
        /// </summary>
        public string DepDateBegin { get; set; }
        /// <summary>
        /// 结束离店日期
        /// </summary>
        public string DepDateEnd { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 订单号或外部订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 订单名称
        /// </summary>
        public string OrderName { get; set; }
        /// <summary>
        /// 预订人姓名或者入住人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 预订人手机号或者入住人手机号
        /// </summary>
        public string MobileNo { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 合约单位名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 登记号
        /// </summary>
        public string RegId { get; set; }

        /// <summary>
        /// 订单名称  预订人姓名或者入住人姓名  合约单位名称
        /// </summary>
        public string OrderNameAndUserNameAndCompanyName { get; set; }
        /// <summary>
        /// 房号 登记号
        /// </summary>
        public string RoomNoAndRegId { get; set; }

        /// <summary>
        /// 房间类型列表
        /// </summary>
        public System.Collections.Generic.List<string> RoomType { get; set; }

        /// <summary>
        /// 价格代码列表
        /// </summary>
        public System.Collections.Generic.List<string> RateCode { get; set; }

        /// <summary>
        /// 客人来源列表
        /// </summary>
        public System.Collections.Generic.List<string> CustomerSource { get; set; }

        /// <summary>
        /// 账务状态（全部，0未结，1已结）
        /// </summary>
        public bool? IsSettle { get; set; }

        /// <summary>
        /// 是否按照主单分组显示
        /// </summary>
        public bool IsGroupByResid { get; set; }
    }
}
