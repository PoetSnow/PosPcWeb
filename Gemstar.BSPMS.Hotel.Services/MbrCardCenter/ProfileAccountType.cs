namespace Gemstar.BSPMS.Hotel.Services.MbrCardCenter
{
    /// <summary>
    /// 会员账户类型
    /// </summary>
    public class ProfileAccountType
    {
        /// <summary>
        /// 储值01
        /// </summary>
        public static ProfileAccountType AccountRecharge = new ProfileAccountType { Code = "01", Name = "储值" };
        /// <summary>
        /// 增值02
        /// </summary>
        public static ProfileAccountType AccountLargess = new ProfileAccountType { Code = "02", Name = "增值" };
        /// <summary>
        /// 积分11
        /// </summary>
        public static ProfileAccountType AccountScore = new ProfileAccountType { Code = "11", Name = "积分" };
        /// <summary>
        /// 业主分12
        /// </summary>
        public static ProfileAccountType AccountOwnerScore = new ProfileAccountType { Code = "12", Name = "业主分" };
        /// <summary>
        /// 现金券31
        /// </summary>
        public static ProfileAccountType AccountTicketCash = new ProfileAccountType { Code = "31", Name = "现金券" };
        /// <summary>
        /// 项目券32
        /// </summary>
        public static ProfileAccountType AccountTicketItem = new ProfileAccountType { Code = "32", Name = "项目券" };
        /// <summary>
        /// 卡费41
        /// </summary>
        public static ProfileAccountType AccountCardFee = new ProfileAccountType { Code = "41", Name = "卡费" };
        /// <summary>
        /// 根据指定的代码转换为账户类型
        /// </summary>
        /// <param name="code">代码类型代码</param>
        /// <returns>对应的账户类型</returns>
        public static ProfileAccountType AccountFromCode(string code)
        {
            return new MbrCardCenter.ProfileAccountType
            {
                Code = code,
                Name = code
            };
        }
        private ProfileAccountType()
        {

        }
        /// <summary>
        /// 账户类型代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 账户类型名称
        /// </summary>
        public string Name { get; set; }
    }
}
