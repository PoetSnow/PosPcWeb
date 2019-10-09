using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 班次服务接口
    /// </summary>
    public interface IShiftService:ICRUDService<Shift>
    {
        /// <summary>
        /// 获取指定酒店下的班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的班次列表</returns>
        List<Shift> GetShifts(string hid);
        /// <summary>
        /// 获取指定酒店下的状态为可用的班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的状态为可用的班次列表</returns>
        List<Shift> GetShiftsAvailable(string hid);
        JsonResultData Enable(string id);
        JsonResultData Disable(string id);
        /// <summary>
        /// 打开指定酒店下的班次
        /// 要求是已经通过权限验证后才调用此方法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="shiftId">班次id</param>
        /// <returns>打开结果</returns>
        JsonResultData OpenShift(string hid, string shiftId);
        /// <summary>
        /// 关闭指定酒店下的班次
        /// 只能关闭打开状态下的班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="shiftId">班次id</param>
        /// <returns>关闭结果</returns>
        JsonResultData CloseShift(string hid, string shiftId);

        /// <summary>
        /// 获取班次键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid);
        /// <summary>
        /// 获取班次代码
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的班次列表</returns>
        bool IsExsitrResFolio(string hid, string id);
    }
}
