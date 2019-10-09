using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.PermanentRoomManage
{
    /// <summary>
    /// 长租房价
    /// </summary>
    public interface IPermanentRoomPricePlanService : ICRUDService<PermanentRoomPricePlan>
    {
        /// <summary>
        /// 添加或修改价格
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        JsonResultData AddOrUpdatePrice(string hid, List<PermanentRoomPricePlan> values);

    }
}
