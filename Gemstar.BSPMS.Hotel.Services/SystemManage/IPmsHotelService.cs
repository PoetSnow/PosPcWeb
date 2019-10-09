using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// pms酒店服务接口
    /// </summary>
    public interface IPmsHotelService : ICRUDService<PmsHotel>
    {
        /// <summary>
        /// 获取指定集团下的所有酒店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <returns>集团下的所有酒店列表</returns>
        List<PmsHotel> GetHotelsInGroup(string grpid);
        /// <summary>
        /// 获取指定集团下除集团管理公司外的所有分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <returns>集团下除集团管理公司外的所有分店列表</returns>
        List<PmsHotel> GetHotelsInGroupExceptGroupHotel(string grpid);
        /// <summary>
        /// 获取指定集团下除集团管理公司外的指定管理类型的分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="manageType">管理类型代码，比如直营，加盟等对应的代码</param>
        /// <returns>集团下除集团管理公司外的所有分店列表</returns>
        List<PmsHotel> GetHotelsInGroupExceptGroupHotel(string grpid,string manageType);

        /// <summary>
        /// 是否开启总裁驾驶舱
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <returns></returns>
        bool IsOpenAnalysis(string hid);
    }
}
