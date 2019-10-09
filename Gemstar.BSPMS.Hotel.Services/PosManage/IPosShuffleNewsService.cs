using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IPosShuffleNewsService : ICRUDService<PosShuffleNews>
    {
        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">公用市别代码</param>
        /// <param name="name">公用市别名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的公用市别了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">公用市别代码</param>
        /// <param name="name">公用市别名称</param>
        /// <param name="exceptId">要排队的公用市别id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的公用市别了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 根据酒店、模块获取公用市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        List<PosShuffleNews> GetPosShuffleNewsList(string hid, string module);
    }
}
