using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 二级仓库列表接口
    /// </summary>
    public interface IPosDepotService : ICRUDService<PosDepot>
    {
        /// <summary>
        ///验证代码名称是否重复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <param name="cName">中文名称</param>
        /// <returns></returns>
        bool IsExists(string hid, string code, string cName);

        /// <summary>
        ///验证代码名称是否重复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <param name="cName">中文名称</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        bool IsExists(string hid, string code, string cName, string id);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);


        /// <summary>
        /// 获取酒店二级仓库
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        List<PosDepot> GetPosDepotList(string hid, string module);
    }
}
