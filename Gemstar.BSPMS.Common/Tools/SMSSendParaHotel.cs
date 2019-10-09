namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 使用酒店账号进行发送短信
    /// 此类重写了参数有效性检测，要求必须传递短信发送用户名和短信发送密码
    /// </summary>
    public abstract class SMSSendParaHotel : SMSSendPara
    {
        /// <summary>
        /// 酒店名称
        /// 发送短信都要求酒店进行实名登记的
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// 重写了参数有效性检测，要求必须传递短信发送用户名和短信发送密码
        /// </summary>
        /// <param name="invalidMsg">无效原因</param>
        /// <returns>是否有效</returns>
        protected override bool IsValid(out string invalidMsg)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                invalidMsg = "短信发送用户名不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                invalidMsg = "短信发送密码不能为空";
                return false;
            }
            if (string.IsNullOrWhiteSpace(HotelName))
            {
                invalidMsg = "酒店名称不能为空";
                return false;
            }
            return base.IsValid(out invalidMsg);
        }
    }
}
