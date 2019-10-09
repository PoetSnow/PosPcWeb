using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IChargeFreeService : ICRUDService<ChargeFree>
    {
        /// <summary>
        /// 根据充值赠送规则 计算赠送金额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardTypeId">卡类型ID</param>
        /// <param name="money">充值金额</param>
        /// <returns></returns>
        decimal GetSendMoney(string hid, string cardTypeId, decimal money);
    }
}
