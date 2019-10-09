using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// pos出品打印机服务接口
    public interface IPosProdPrinterService : ICRUDService<PosProdPrinter>
    {
        /// <summary>
        /// 判断指定的代码或者名称的班次是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">Prt代码</param>
        /// <param name="name">出品打印机名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的出品打印机是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">Prt代码</param>
        /// <param name="name">出品打印机名称</param>
        /// <param name="exceptId">要排队的班次id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 获取指定酒店下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        List<PosProdPrinter> GetPosProdPrinter(string hid);

        /// <summary>
        /// 获取指定酒店和模块下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的要求列表</returns>
        List<PosProdPrinter> GetPosProdPrinterByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的故障打印机信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        List<PosProdPrinter> GetPosProdPrinterByFault(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店下的出品打印机列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        List<up_Pos_list_ProducelistResult> GetPosProdPrinterResult(string hid);

        /// <summary>
        /// 根据指定酒店和代码获取出品打印机
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">代码</param>
        /// <returns>指定酒店和模块下的出品打印机列表</returns>
        PosProdPrinter GetPosProdPrinterByCode(string hid, string code);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}