using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos出品明细服务接口
    /// </summary>
    public interface IPosProducelistService : ICRUDService<PosProducelist>
    {
        /// <summary>
        /// 根据指定酒店和ID获取出品打印记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        PosProducelist GetEntity(string hid, long id);
    }
}