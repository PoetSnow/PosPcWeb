using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IRoomOwnerCalcParaDefineService : ICRUDService<RoomOwnerCalcParaDefine>
    {
        /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        List<RoomOwnerCalcParaDefine> List(string hid);
        List<RoomOwnerCalcParaDefine> ListbyParatype(string hid, string paratype);
        /// <summary>
        /// 获取可用于业主计算的函数列表
        /// </summary>
        /// <returns>可用于业主计算的函数列表</returns>
        List<string> GetCalcFunctions();
        string  updateRoomOwnerCalcParaDefine(List<RoomOwnerCalcParaDefine> listRoomOwner, string hid);
        List<V_dataType> getDataType();
        V_dataType getDataTypeName(string code);
    }
}
