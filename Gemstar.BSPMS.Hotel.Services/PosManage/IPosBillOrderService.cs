using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 账单预定详细信息
    /// </summary>
    public interface IPosBillOrderService : ICRUDService<PosBillOrder>
    {
        /// <summary>
        /// 获取预定信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单Id</param>
        /// <param name="columnName">列名</param>
        /// <param name="tabName">表名</param>
        /// <returns></returns>
        PosBillOrder GetBillOrder(string hid, string billId, string columnName, string tabName);
    }
}
