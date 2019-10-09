using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IPosBillCostService : ICRUDService<PosBillCost>
    {
        /// <summary>
        /// 获取物品数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billId">账单明细ID</param>
        /// <param name="costItemid">物品Id</param>
        /// <returns></returns>
        PosBillCost GetBillCost(string hid, string module, long billId, string costItemid);


        /// <summary>
        /// 获取仓库耗用数据列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billId">账单明细ID</param>
        /// <returns></returns>
        List<PosBillCost> GetBillCostList(string hid, string module, long billId);

        /// <summary>
        /// 获取当前营业日已消耗的库存数量
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="whCode">二级仓库</param>
        /// <param name="costItemid">物品ID</param>
        /// <returns></returns>
        decimal? GetBillCostSumQuantity(string hid, string module, DateTime? billBsnsDate, string whCode, string costItemid);

        /// <summary>
        /// 获取当前营业日已消耗的库存数量
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="whCode">二级仓库</param>
        /// <param name="costItemid">物品ID</param>
        /// <param name="billId">账单明细ID</param>
        /// <returns></returns>

        decimal? GetBillCostSumQuantity(string hid, string module, DateTime? billBsnsDate, string whCode, string costItemid, long billId);

        /// <summary>
        /// 根据存储过程获取物品耗用数据
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billDate">营业日</param>
        /// <param name="posId">收银点id</param>
        /// <param name="Module">模块</param>
        /// <returns></returns>
        List<PosBillCost> GetPosBillCostByProc(string hid, string posid, DateTime? Business, string Module);


    }
}
