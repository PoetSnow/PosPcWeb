using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 待支付记录服务接口
    /// </summary>
    public interface IWaitPayListService:ICRUDService<WaitPayList>
    {

        /// <summary>
        /// 获取指定酒店指定ID的待支付记录
        /// </summary>
        /// <param name="id">待支付记录ID</param>
        /// <param name="producttype">产品类型</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        WaitPayList GetWaitPayList(string id, string producttype, byte status);



    }
}
