namespace GsBrowser
{
    public class LoginViewModel
    {
        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 体验账号
        /// </summary>
        public string TryHotelId { get; set; }
        /// <summary>
        /// 体验用户
        /// </summary>
        public string TryUserName { get; set; }
        /// <summary>
        /// 体验密码
        /// </summary>
        public string TryUserPass { get; set; }
    }
}
