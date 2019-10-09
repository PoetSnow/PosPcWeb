using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.InitSystem
{
    /// <summary>
    /// 初始化系统视图模型
    /// </summary>
    public class InitSystemViewModel
    {
        /// <summary>
        /// 混淆后的手机号，仅用于显示
        /// </summary>
        public string MixedMobile { get; set; }
        /// <summary>
        /// 初始化验证码
        /// </summary>
        [Display(Name ="验证码")]
        public string CheckCode { get; set; }
        /// <summary>
        /// 删除营业数据
        /// </summary>
        [Display(Name ="删除营业数据")]
        public bool DeleteBusinessData { get; set; }
        
        /// <summary>
        /// 删除营销政策
        /// </summary>
        [Display(Name = "删除营销政策")]
        public bool DeleteMarketingPolicy { get; set; }

        /// <summary>
        /// 删除消费项目基础数据
        /// </summary>
        [Display(Name = "删除消费项目基础数据")]
        public bool DeleteItemBaseData { get; set; }

        /// <summary>
        /// 删除营销基础数据
        /// </summary>
        [Display(Name = "删除营销基础数据")]
        public bool DeleteMarketingBasicData { get; set; }


        /// <summary>
        /// 酒店编码
        /// </summary>
        [Display(Name = "酒店编码")]
        public string HotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [Display(Name = "酒店名称")]
        public string HotelName { get; set; }
    }
}
 