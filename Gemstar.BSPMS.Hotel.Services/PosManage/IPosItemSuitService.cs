using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IPosItemSuitService : ICRUDService<PosItemSuit>
    {
        /// <summary>
        /// 判断是否有重复的套餐明细与单位
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">套餐ID</param>
        /// <param name="itemId2">套餐明细ID</param>
        /// <param name="iGrade">套餐明细级别</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="Id">主键</param>
        /// <returns></returns>

        bool IsExists(string hid, string itemId, string itemId2, int? iGrade, string unitId, string Id = "");


        /// <summary>
        /// 通过酒店ID，套餐ID，计算金额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">套餐ID</param>
        void CalculationItemSuitAmount(string hid, string itemId);

        /// <summary>
        /// 根据消费项目获取套餐明细数据
        /// </summary>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        List<up_pos_list_itemSuitByItemIdResult> GetPosItemSuitListByItemId(string hid, string itemId);

        /// <summary>
        /// 根据消费项目获取套餐明细数据
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemId">消费项目</param>
        /// <param name="iGrade">级数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<up_pos_list_itemSuitByItemIdResult> GetPosItemSuitListByItemId(string hid, string itemId, int? iGrade, int pageIndex, int pageSize);
    }
}
