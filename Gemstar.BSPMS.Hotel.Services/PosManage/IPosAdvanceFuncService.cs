using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos 高级功能列表
    /// </summary>
    public interface IPosAdvanceFuncService : ICRUDService<PosAdvanceFunc>
    {
        /// <summary>
        /// 根据酒店ID 与编码查询出对应的数据对象
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="FuncCode">编码</param>
        /// <returns></returns>
        PosAdvanceFunc GetPosAdvanceFuncByFuncCode(string hid,string FuncCode);


        /// <summary>
        /// 查询高级功能列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Module">模块</param>
        /// <returns></returns>
        List<PosAdvanceFunc> GetPosAdvanceFuncList(string hid, string refeId, string Module, int pageIndex, int pageSize);


        /// <summary>
        /// 统计酒店ID与营业点获取高级功能
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Module"></param>
        /// <returns></returns>
        int GetPosAdvanceFuncCount(string hid, string refeId, string Module);

        /// <summary>
        /// 验证数据是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Code">编码</param>
        /// <returns></returns>
        bool IsExists(string hid, string Code,string Id="");

    }
}
