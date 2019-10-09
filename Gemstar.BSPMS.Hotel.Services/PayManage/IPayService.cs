using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付接口
    /// </summary>
    public interface IPayService
    {
        /// <summary>
        /// 在保存账务之前执行支付并且返回支付号
        /// 支付号可以用来进行对账，并且将会保存在folio的refno中
        /// </summary>
        /// <param name="jsonStrPara">支付需要的参数，json格式的字符串，因为每种支付方式需要的参数不同</param>
        /// <returns>支付号，用来进行对账</returns>
        PayResult DoPayBeforeSaveFolio(string jsonStrPara);
        /// <summary>
        /// 在保存账务之后执行，比如提交在线支付等
        /// </summary>
        /// <param name="productType">产品类型</param>
        /// <param name="payTransId">产品业务id，用于获取需要支付的详细信息</param>
        /// <param name="jsonStrPara">支付需要的参数，json格式的字符串，因为每种支付方式需要的参数不同</param>
        PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara);
    }
}
