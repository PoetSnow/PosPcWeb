namespace Gemstar.BSPMS.Hotel.Web.Models.Home
{
    /// <summary>
    /// 设置班次结果
    /// </summary>
    public class SetShiftResultModel
    {
        /// <summary>
        /// 是否可以继续设置
        /// </summary>
        public ShiftResultType ResultType { get; set; }
        /// <summary>
        /// 不能设置的原因或者是提示信息
        /// </summary>
        public string Message { get; set; }
    }
    public enum ShiftResultType:byte
    {
        /// <summary>
        /// 不能设置
        /// </summary>
        CantSet = 0,
        /// <summary>
        /// 提示是否继续设置
        /// </summary>
        Promot = 1,
        /// <summary>
        /// 已经设置成功，直接转向到指定的地址上
        /// </summary>
        Redirect = 2
    }
}