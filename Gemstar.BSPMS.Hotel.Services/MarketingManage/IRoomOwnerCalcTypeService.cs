using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IRoomOwnerCalcTypeService : ICRUDService<RoomOwnerCalcType>
    {
        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        List<RoomOwnerCalcType> List(string hid);
    }
}
