namespace Gemstar.BSPMS.Hotel.Services.MbrCardCenter
{
    /// <summary>
    /// 会员交易类型
    /// </summary>
    public class ProfileCaType
    {
        /// <summary>
        /// 充值01
        /// </summary>
        public static ProfileCaType Recharge = new ProfileCaType { Code = "01", Name = "充值" };
        /// <summary>
        /// 扣款02
        /// </summary>
        public static ProfileCaType Consume = new ProfileCaType { Code = "02", Name = "扣款" };
        /// <summary>
        /// 获得积分10
        /// </summary>
        public static ProfileCaType ScoreGet = new ProfileCaType { Code = "10", Name = "获得积分" };
        /// <summary>
        /// 使用积分11
        /// </summary>
        public static ProfileCaType ScoreUse = new ProfileCaType { Code = "11", Name = "使用积分" };
        /// <summary>
        /// 调整积分12
        /// </summary>
        public static ProfileCaType ScoreUpdate = new ProfileCaType { Code = "12", Name = "调整积分" };
        /// <summary>
        /// 获得券30
        /// </summary>
        public static ProfileCaType TicketGet = new ProfileCaType { Code = "30", Name = "获得券" };
        /// <summary>
        /// 使用券31
        /// </summary>
        public static ProfileCaType TicketUse = new ProfileCaType { Code = "31", Name = "使用券" };
        /// <summary>
        /// 过期券32
        /// </summary>
        public static ProfileCaType TicketExpired = new ProfileCaType { Code = "32", Name = "过期券" };
        /// <summary>
        /// 缴纳卡费40
        /// </summary>
        public static ProfileCaType CardFee = new ProfileCaType { Code = "40", Name = "缴纳卡费" };
        /// <summary>
        /// 根据指定的代码转换为账户类型
        /// </summary>
        /// <param name="code">代码类型代码</param>
        /// <returns>对应的账户类型</returns>
        public static ProfileCaType CaTypeFromCode(string code)
        {
            return new ProfileCaType
            {
                Code = code,
                Name = code
            };
        }
        private ProfileCaType() { }
        /// <summary>
        /// 交易类型代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 交易类型名称
        /// </summary>
        public string Name { get; set; }
    }
}
