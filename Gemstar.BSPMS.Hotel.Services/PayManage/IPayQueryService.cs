using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付后的查询service，用于查询最终的支付结果
    /// </summary>
    public interface IPayQueryService
    {
        JsonResultData Query(string hid,PayProductType productType, string productTransId);
    }
}
