using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// Pos扫码点餐Banner
    /// </summary>
    public interface IPosMBannerService : ICRUDService<PosMBanner>
    {
        /// <summary>
        /// 判断指定Id的Banner是否已经存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsExists(string hid, string id);
        /// <summary>
        /// 获取Banner列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<PosMBanner> GetMBannerList(string hid);
    }
}