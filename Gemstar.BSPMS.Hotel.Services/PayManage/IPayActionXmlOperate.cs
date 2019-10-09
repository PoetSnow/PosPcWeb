namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付方式付款处理动作的xml处理接口
    /// </summary>
    public interface IPayActionXmlOperate
    {

        /// <summary>
        /// 捷信达外部接口
        /// </summary>
        /// <param name="grpid">酒店id</param>
        /// <param name="channelCode">渠道代码</param>
        /// <param name="xmlStr">业务xml</param>
        /// <returns>接口返回内容</returns>
        string RealOperate(string grpid, string channelCode, string xmlStr);
    }
}
