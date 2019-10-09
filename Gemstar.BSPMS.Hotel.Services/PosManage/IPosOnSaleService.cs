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
    /// <summary>
    /// 特价菜项目接口
    /// </summary>
    public interface IPosOnSaleService : ICRUDService<PosOnSale>
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
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime);
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
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptId">要排队的服务费政策id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime, Guid? exceptId);

        /// <summary>
        /// 特价菜列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="tabTypeId">餐台类型ID</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <returns></returns>
        List<PosOnSale> GetPosOnSaleList(string hid, string module, string itemId, string unitId, string refeId, string tabTypeId, byte? iTagperiod);

        /// <summary>
        /// 特价菜消费项目列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">日期类型</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="tabTypeid">餐台类型ID</param>
        /// <returns></returns>

        List<up_pos_list_itemByPosOnSaleResult> GetItemListByPosOnSale(string hid, string refeId, string itagperiod, string customerTypeid, string tabTypeid, int pageIndex, int pageSize);


        /// <summary>
        /// 获取特价菜的数量
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">日期类型</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="tabTypeid">餐台类型ID</param>
        /// <returns></returns>
        int GetPosOnSaleTotal(string hid, string refeId, string itagperiod, string customerTypeid, string tabTypeid);


        /// <summary>
        /// 获取消费项目单位列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">特价菜日期类型</param>
        /// <param name="customerTypeid">客人类型</param>
        /// <param name="tabTypeid">餐台类型ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<up_pos_list_ItemPriceByItemidResult> GetUnitByPosOnSaleItem(string hid, string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid, int pageIndex, int pageSize);

        /// <summary>
        /// 特价菜单位数量
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemId"></param>
        /// <param name="refeId"></param>
        /// <param name="itagperiod"></param>
        /// <param name="customerTypeid"></param>
        /// <param name="tabTypeid"></param>
        /// <returns></returns>
        int GetUnitByPosOnSaleItemTotal(string hid, string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid);

        /// <summary>
        /// 根据条件获取特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">日期类型</param>
        /// <param name="customerTypeid">客人类型</param>
        /// <param name="tabTypeid">餐台类型</param>
        /// <param name="unitId">单位ID</param>
        /// <returns></returns>
        PosOnSale GetPosOnSaleByItemId(string hid,string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid, string unitId);

        /// <summary>
        /// 批量操作页面 根据条件获取特价菜列表
        /// </summary>
        /// <param name="itemname">消费项目名称</param>
        /// <param name="unitid">单位id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台类型id</param>
        /// <param name="customerid">客户类型ID</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="CmpType">计算类型</param>
        /// <param name="isUsed">是否启用</param>
        /// <returns></returns>
        List<up_pos_list_BatchHandlePosOnSaleResult> GetBatchHandlePosOnSale(string hid, string itemname, string unitid, string refeid, string tabid, string customerid, string iTagperiod, Int16? CmpType, int? isUsed);

        /// <summary>
        /// 根据酒店ID 和lambda表达式获取特价菜列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="func">lambda表达式</param>
        /// <returns></returns>
        List<PosOnSale> GetPosOnSales(string hid, Func<PosOnSale, bool> func);

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
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptIdlist">要排队的id集合，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的了，false：没有相同的</returns>
        bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime, List<Guid> exceptIdlist);




    }
}
