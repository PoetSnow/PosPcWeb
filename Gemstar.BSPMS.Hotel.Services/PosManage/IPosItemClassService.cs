using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos消费项目大类服务接口
    /// </summary>
    public interface IPosItemClassService : ICRUDService<PosItemClass>
    {
        /// <summary>
        /// 判断指定的代码或者名称的消费项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目大类代码</param>
        /// <param name="name">消费项目大类名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目大类了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的消费项目大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目大类代码</param>
        /// <param name="name">消费项目大类名称</param>
        /// <param name="exceptId">要排队的消费项目大类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目大类了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店下的消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的消费项目大类列表</returns>
        List<PosItemClass> GetPosItemClass(string hid);
        /// <summary>
        /// 获取指定酒店和模块下的消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的消费项目大类列表</returns>
        List<PosItemClass> GetPosItemClassByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的消费项目大类以及分类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的消费项目大类列表</returns>
        List<v_pos_ItemClassUnionItemResult> GetPosItemClassAndSubClass(string hid);

        /// <summary>
        /// 获取指定营业点下的项目大类总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="TabTypeId">餐台类型ID</param>
        /// <returns></returns>
        int GetPosItemClassTotal(string hid, string refeid, string posId, string customerTypeid = "", string TabTypeId = "");

        /// <summary>
        /// 根据酒店和营业点获取消费项目大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="TabTypeId">餐台类型ID</param>
        /// <returns></returns>
        List<up_pos_list_ItemClassBySingleResult> GetPosItemClassByRefe(string hid, string refeid, int pageIndex, int pageSize, string posId, string customerTypeid = "", string TabTypeId = "");

        /// <summary>
        /// 查询消费项目大类
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="wherefunc"></param>
        /// <returns></returns>
        List<PosItemClass> GetPosItemClass(string hid,Func<PosItemClass,bool> wherefunc);

        /// <summary>
        /// 查询大类对应的做法
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="itemClassId">大类Id</param>
        /// <returns></returns>
        List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemClassId(string hid, string itemClassId);


    }
}