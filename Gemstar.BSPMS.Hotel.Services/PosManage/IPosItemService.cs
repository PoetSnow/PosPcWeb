using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos消费项目服务接口
    /// </summary>
    public interface IPosItemService : ICRUDService<PosItem>
    {
        /// <summary>
        /// 判断指定的代码或者名称的消费项目是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目代码</param>
        /// <param name="name">消费项目名称</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string dcFlag);

        /// <summary>
        /// 判断指定的代码或者名称的消费项目是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">消费项目代码</param>
        /// <param name="name">消费项目名称</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <param name="exceptId">要排队的消费项目id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string dcFlag, string exceptId);

        /// <summary>
        /// 根据酒店、付款方式获取处理方式
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">付款方式id</param>
        /// <returns></returns>
        PosItem GetEntity(string hid, string id);

        /// <summary>
        /// 获取指定酒店下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        List<PosItem> GetPosItem(string hid, string dcFlag);

        /// <summary>
        /// 获取指定酒店下的消费项目总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns></returns>
        int GetPosItemTotalByDcFlag(string hid, string dcFlag);

        /// <summary>
        /// 获取指定酒店下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="refeId">营业点id</param>
        /// <returns></returns>
        List<PosItem> GetPosItemByDcFlag(string hid, string dcFlag, int pageIndex, int pageSize, string refeId);

        /// <summary>
        /// 获取指定酒店和模块下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        List<PosItem> GetPosItemByModule(string hid, string moduleCode, string dcFlag);

        /// <summary>
        /// 根据指定酒店、模块、付款标识、是否开台项目获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="dcFlag">付款标识（D：消费，C：付款）</param>
        /// <param name="isOpenItem">是否开台项目</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        List<PosItem> GetPosOpenItemByModule(string hid, string moduleCode, string dcFlag, bool? isOpenItem);

        /// <summary>
        /// 获取指定酒店、模块和是否分类下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="isSubClass">是否是分类</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店、模块和是否分类下的消费项目列表</returns>
        List<PosItem> GetPosItemByItemAndIsSubClass(string hid, string itemClassid, bool isSubClass, string dcFlag);

        /// <summary>
        /// 获取指定项目大类、分类下的项目代码(自增1)
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="subClassid">项目分类ID</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <param name="isSubClass">是否分类，默认：false</param>
        /// <returns>指定酒店、模块和是否分类下的消费项目列表</returns>
        string GetItemCodeByClassid(string hid, string itemClassid, string subClassid, string dcFlag, bool isSubClass = false);

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别、获取消费项目总记录数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refe">营业点id、代码或名称</param>
        /// <param name="tabtype">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <returns></returns>
        int GetPosItemTotal(string hid, string refeid, string itemClassid, string shuffleid, string keyword);

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="itemClassid">项目id</param>
        /// <param name="shuffleid">市别id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        List<up_pos_list_itemByItemClassidResult> GetPosItemByItemClassid(string hid, string refeid, string itemClassid, string shuffleid, string keyword);

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="itemClassid">项目id</param>
        /// <param name="shuffleid">市别id</param>
        /// <returns></returns>
        List<up_pos_list_itemByItemClassidResult> GetPosItemByItemClassid(string hid, string refeid, string itemClassid, string shuffleid, int pageIndex, int pageSize, string keyword);

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        List<up_pos_list_suitItemByRefeidResult> GetSuitItemByRefeid(string hid, string refeid, string keyword);

        /// <summary>
        /// 根据酒店、营业点、项目大类、市别获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        List<up_pos_list_suitItemByRefeidResult> GetSuitItemByRefeid(string hid, string refeid, int pageIndex, int pageSize, string keyword);

        /// <summary>
        /// 根据酒店ID查询出该酒店下最大的code
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        string GetMaxtemCode(string hid);

        /// <summary>
        /// 指定酒店和的营业点是否拥有套餐项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeid">营业点ID</param>
        /// <returns></returns>
        bool isItemSuit(string hid, string refeid);

        /// <summary>
        /// 验证消费项目是否有使用
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        bool IsExistsBillByItemId(string hid, string itemId);

        /// <summary>
        /// 获取酒店下的所有库存
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<PosItem> GetPosCostItem(string hid);

        /// <summary>
        /// 通过 lambda表达式获取 消费项目
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="wherefunc">lambda表达式</param>
        /// <returns></returns>
        List<PosItem> GetItems(string hid, Func<PosItem, bool> wherefunc);

        /// <summary>
        /// 根据酒店和显示设置获取消费项目
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="showSet"></param>
        /// <returns></returns>
        List<up_pos_scan_list_PosItemByShowSetResult> GetPosItemByShowSet(string hid, string showSet);

        /// <summary>
        /// 获取指定酒店和显示设置下的消费项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="showSet">显示设置</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>指定酒店和模块下的消费项目列表</returns>
        List<PosItem> GetPosItemByShowSet(string hid, string moduleCode, string showSet, string dcFlag);

        /// <summary>
        /// 根据消费项目删除相关联的数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="module"></param>
        /// <param name="itemid"></param>

        void DeletePosItemOther(string hid, string itemid);


        //获取Pos支付方式
        /// <summary>
        /// 获取Pos支付方式
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Entities.V_codeListPub GetPosPayType(string code);


        /// <summary>
        /// 获取扫码点餐分类
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <returns></returns>
        List<PosItem> GetScanPosSubClassList(string hid, string refeId);


        /// <summary>
        /// 获取指定项目大类、分类下的项目代码(只判断数字编码，过滤包含字母的编码,获取最小的不连贯的Code + 1)
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemClassid">项目大类ID</param>
        /// <param name="subClassid">项目分类ID</param>
        /// <param name="dcFlag">付款还是消费（D：消费，C：付款）</param>
        /// <returns>代码</returns>
        string GetNewItemCodeByClassid(string hid, string itemClassid, string subClassid, string dcFlag, bool isSubClass = false);

        /// <summary>
        /// 消费项目汉字转其他
        /// </summary>
        /// <param name="name">中文汉字字符串</param>
        /// <param name="type">1:拼音首字母  2:五笔首字母 3:拼音全拼  4:五笔全码</param>
        /// <returns></returns>
        string PosItemHz2other(string name, int type);

        /// <summary>
        /// 验证消费项目是否有库存
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        bool IsExistsPosCostByItemId(string hid, string itemId);

    }
}