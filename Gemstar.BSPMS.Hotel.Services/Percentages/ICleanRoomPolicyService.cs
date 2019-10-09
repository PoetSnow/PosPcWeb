using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Percentages
{
    public interface ICleanRoomPolicyService : ICRUDService<PercentagesPolicyCleanRoom>
    {
        /// <summary>
        /// 添加或修改价格
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        JsonResultData AddOrUpdatePrice(string hid, List<PercentagesPolicyCleanRoom> values);
    }
}
