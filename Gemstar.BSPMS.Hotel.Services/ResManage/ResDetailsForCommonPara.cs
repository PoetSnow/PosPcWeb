namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    public class ResDetailsForCommonPara
    {
        public string Hid { get; set; }
        /// <summary>
        /// 是否结账，默认只查询未结的,0:未结账，1：已结账，9：全部
        /// </summary>
        public byte? IsSettle { get; set; }
        /// <summary>
        /// 默认查询所有状态的，可以传值后只查询指定状态的,多项之间以逗号分隔
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNo { get; set; }
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 0:登记或预订单(统计房间占用用这个) 1：团体虚拟帐单 2:工作帐
        /// </summary>
        public byte BillType { get; set; }
        /// <summary>
        /// 预订名称或者团体名称
        /// </summary>
        public string ResName { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 不包括账号 多个之间逗号隔开
        /// </summary>
        public string NotRegIds { get; set; }
    }
}
