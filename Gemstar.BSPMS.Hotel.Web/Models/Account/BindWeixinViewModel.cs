namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 操作员绑定微信视图模型
    /// </summary>
    public class BindWeixinViewModel
    {
        /// <summary>
        /// 已经绑定的微信id
        /// </summary>
        public string WxOpenId { get; set; }
        /// <summary>
        /// 用于绑定的带参二维码图片地址
        /// </summary>
        public string QrCodeImageUrl { get; set; }

        /// <summary>
        /// 已绑定用户昵称
        /// </summary>
        public string UserNickName { get; set; }
        /// <summary>
        /// 已绑定用户头像
        /// </summary>
        public string UserHeadImgUrl { get; set; }
    }
}