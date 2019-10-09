using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Data;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos单位价格服务接口
    /// </summary>
    public interface IPosItemPriceService : ICRUDService<PosItemPrice>
    {
        /// <summary>
        /// 判断指定的代码或者名称的单位价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unit">单位</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位价格了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string unit);
        /// <summary>
        /// 判断指定的代码或者名称的单位价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unit">单位</param>
        /// <param name="exceptId">要排队的单位价格id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位价格了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string unit, Guid exceptId);
        /// <summary>
        /// 获取指定酒店、项目、单位下的单位价格
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="unitid">单位</param>
        /// <returns></returns>
        PosItemPrice GetPosItemPriceByUnitid(string hid, string itemid, string unitid);
        /// <summary>
        /// 获取指定酒店、项目下的默认单位价格
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <returns></returns>
        PosItemPrice GetPosItemDefaultPriceByUnitid(string hid, string itemid);
        /// <summary>
        /// 获取酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>酒店和项目下的单位价格列表</returns>
        List<up_pos_list_ItemPriceByItemidResult> GetPosItemPriceByItemId(string hid, string itemId);
        /// <summary>
        /// 获取酒店和项目下的单位价格总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns></returns>
        int GetPosItemPriceTotal(string hid, string itemId);
        /// <summary>
        /// 获取酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>酒店和项目下的单位价格列表</returns>
        List<up_pos_list_ItemPriceByItemidResult> GetPosItemPriceByItemId(string hid, string itemId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemid">项目ID</param>
        /// <returns></returns>
        List<PosItemPrice> GetPosItemPriceForCopy(string hid, string itemid);


        /// <summary>
        /// 获取指定酒店和项目下的单位价格列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemid"></param>
        /// <param name="unitid"></param>
        /// <param name="func">lambda表达式</param>
        /// <returns></returns>
        PosItemPrice GetPosItemPriceCountByItemID(string hid, string itemid, string unitid, Func<PosItemPrice, bool> func);

    }
}
