using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos开台项目服务接口
    /// </summary>
    public interface IPosTabOpenItemService : ICRUDService<PosTabOpenItem>
    {
        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">日期类型</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="unitid">单位id</param>
        /// <param name="isCharge">收费状态</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? isCharge, byte? iTagperiod, string startTime, string endTime);
        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">日期类型</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="unitid">单位id</param>
        /// <param name="isCharge">收费状态</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptId">要排队的服务费政策id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? isCharge, byte? iTagperiod, string startTime, string endTime, Guid? exceptId);
        /// <summary>
        /// 根据酒店、模块、营业点、餐台类型等获取服务费政策
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">客人类型id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="openTime">开台时间</param>
        /// <returns></returns>
        List<PosTabOpenItem> GetPosTabOpenItemList(string hid, string module, string refeid, string tabTypeid, string customerTypeid, byte? iTagperiod, DateTime openTime);
        /// <summary>
        /// 根据酒店、模块、营业点、餐台类型等获取服务费政策
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">客人类型id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="openTime">开台时间</param>
        /// <returns></returns>
        List<up_pos_list_tabOpenItemByTabTypeResult> GetPosTabOpenItemByTabType(string hid, string module, string refeid, string tabTypeid, string customerTypeid, byte? iTagperiod, DateTime openTime);
    }
}