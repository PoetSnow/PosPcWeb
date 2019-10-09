namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
    /// <summary>
    /// 酒店自定义的logo和名称信息
    /// </summary>
    public class HotelLogoAndNameInfo
    {
        /// <summary>
        /// 自定义logo图片地址，没有自定义时为空
        /// </summary>
        public string LogoUrl { get; set; }
        /// <summary>
        /// 酒店是否允许显示捷信达公司logo
        /// </summary>
        public string ShowGSLogo { get; set; }
        /// <summary>
        /// 酒店自定义标题
        /// </summary>
        public string GSSysTitle { get; set; }
    }
}
