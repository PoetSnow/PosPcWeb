namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 基础资料集团管控属性
    /// </summary>
    public class M_V_BasicDataType
    {
        /// <summary>
        /// 预订须知代码
        /// </summary>
        public const string BasicDataCodeBookingNotes = "bookingNotes";
        /// <summary>
        /// 房间类型代码
        /// </summary>
        public const string BasicDataCodeRoomType = "Roomtype";
        /// <summary>
        /// 消费项目
        /// </summary>
        public const string BasicDataCodeItemConsume = "02";
        /// <summary>
        /// 消费项目
        /// </summary>
        public const string BasicDataCodeItemPay = "02";
        /// <summary>
        /// 通用代码(市场分类)
        /// </summary>
        public const string BasicDataCodeMarket = "04";
        /// <summary>
        /// 通用代码(客人来源)
        /// </summary>
        public const string BasicDataCodeSource = "05";
        /// <summary>
        /// 通用代码(房间特色)
        /// </summary>
        public const string BasicDataCodeRoomFeature = "08";
        /// <summary>
        /// 通用代码(发票开票项目)
        /// </summary>
        public const string BasicDataCodeInvoice = "13";
        /// <summary>
        /// 通用代码(客房用品)
        /// </summary>
        public const string BasicDataCodeEquipment = "23";
        /// <summary>
        /// 班次
        /// </summary>
        public const string BasicDataCodeShift = "Shift";
        /// <summary>
        /// 价格体系
        /// </summary>
        public const string BasicDataCodeRate = "Ratecode";
        /// <summary>
        /// 消费类别
        /// </summary>
        public const string BasicDataCodeConsume = "07";
        /// <summary>
        /// 付款类别
        /// </summary>
        public const string BasicDataCodePayway = "03";

        public string Code { get; set; }
        public string Name { get; set; }
        public string DataControl { get; set; }
    }
}
