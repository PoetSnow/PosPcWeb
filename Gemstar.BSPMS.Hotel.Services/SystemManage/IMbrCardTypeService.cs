using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 会员类型接口
    /// </summary>
    public interface IMbrCardTypeService:ICRUDService<MbrCardType>
    {
        /// <summary>
        /// 获取指定集团下的所有酒店列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>酒店下的所有会员卡类型列表</returns>
        List<MbrCardType> GetMbrCardType(string hid);

        /// <summary>
        /// 获取会员卡类型键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid);
        /// <summary>
        /// 会员等级是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="seqit"></param>
        /// <returns></returns>
        bool IsMbrCardTypeSeqit(string hid, int seqit,string id);
    }
}
