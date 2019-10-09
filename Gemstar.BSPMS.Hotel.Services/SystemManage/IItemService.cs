using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IItemService : ICRUDService<Item>
    {
        /// <summary>
        /// 获取指定酒店下的记账项目列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<Item> GetItem(string hid, string dcflag);

        JsonResultData Enable(string id, string _dcflag);
        JsonResultData Disable(string id, string _dcflag);
        ///// <summary>
        ///// 获取所有处理方式
        ///// </summary> 
        ///// <returns>指定所有处理方式</returns>
        List<V_codeListPub> GetCodeListPub(string typeCode);
        List<V_codeListPub> GetCodeListPub(string typeCode, string code);
        SelectList GetCodeListPubforSel(string typeCode);
        ///// <summary>
        ///// 获取所有类型名称
        ///// </summary> 
        ///// <returns>指定所有处理方式</returns> 
        List<CodeList> GetCodeList(string typecode, string hid);

        ///// <summary>
        ///// 根据id获取发票开票项目
        ///// </summary> 
        ///// <returns></returns> 
        string GetCodeList(string typecode, string hid, string id);
        ///// <summary>
        ///// 获取付款类型名称
        ///// </summary> 
        ///// <returns>指定所有处理方式</returns> 
        string GetCodeListFornameBycode(string typecode, string hid, string code);
        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="dcFlag">D:消费 C:付款</param>
        /// <param name="isCharge">true:可充值 false:不可充值</param>
        /// <returns></returns>
        IQueryable<Item> GetItems(string hid, string dcFlag = "", bool? isCharge = null);
        ///// <summary>
        ///// 判断小于21的处理方式code代码值同一个酒店内不能重复
        ///// </summary> 
        ///// <returns></returns> 
        List<Item> GetItembyAction(string hid, string action, string dcflag, string id);
        SelectList GetStatypesellist();
        List<Item> GetCodeListbyitemTypeid(string hid, string itemtypeid, string Dcflag);
        List<V_itemReserv> IsexistV_itemReserv(string itemcode, string dcflag);
        List<V_codeListReserve> IsexistV_codeListReserve(string code,string typecode);
        /// <summary>
        /// 查询指定条件的项目列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="dcFlag">消费还是付款</param>
        /// <param name="keyword">关键字</param>
        /// <param name="isInput">是否可输入，默认只查询可输入的项目</param>
        /// <returns>满足条件的项目列表</returns>
        List<Item> Query(string hid, string dcFlag, string keyword, bool isInput = true);
        string IsexistItemId(string hid, string itemid);
        /// <summary>
        /// 当通用代码的发票项目的汇率发生改变时同步item表中的汇率
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="invoiceItemid"></param>
        /// <param name="rate"></param>
        void syncRate(string hid, string invoiceItemid, decimal rate);
        /// <summary>
        /// 业主消费项目
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<Item> getOwnerfeeItem(string hid);
    }
}
