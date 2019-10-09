using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 基础数据分发接口
    /// </summary>
    public interface IBasicDataCopyService<T> where T:class
    {
        /// <summary>
        /// 获取集团记录在指定分店的分发实例
        /// </summary>
        /// <param name="hid">分店id</param>
        /// <param name="groupModel">集团记录</param>
        T GetNewHotelBasicData(string hid, T groupModel);
        /// <summary>
        /// 获取指定集团记录分发到酒店后的酒店记录实例
        /// </summary>
        /// <param name="hid">分店id</param>
        /// <param name="groupModel">集团记录</param>
        /// <returns>null：分店没有此集团记录的分发记录，分发实例：集团记录分发到此分店后的记录实例</returns>
        T GetCopyedHotelBasicData(string hid, T groupModel, bool iscopyed);
        /// <summary>
        /// 获取指定集团记录分发到酒店后的酒店记录列表
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <returns>指定集团记录分发到酒店后的酒店记录列表</returns>
        List<T> GetCopyedHotelBasicDatas(T groupModel);
    }
}
