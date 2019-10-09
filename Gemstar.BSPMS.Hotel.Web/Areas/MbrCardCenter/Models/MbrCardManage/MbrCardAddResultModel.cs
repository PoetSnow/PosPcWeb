namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    /// <summary>
    /// 会员增加后的结果
    /// </summary>
    public class MbrCardAddResultModel
    {
        /// <summary>
        /// 会员id
        /// </summary>
        public string ProfileId { get; set; }
        /// <summary>
        /// 要收取的卡费金额,有值并且大于0时则表示要收取，否则不收取
        /// </summary>
        public decimal CardFeeAmount { get; set; }
    }
}