using Gemstar.BSPMS.Common.Services.Entities;
using System.Collections.Generic;
using System.Linq;


namespace Gemstar.BSPMS.Common.Services
{
    public interface IMasterService
    {
        /// <summary>
        /// 获取所有省份信息
        /// </summary>
        /// <returns></returns>
        IQueryable<Province> GetProvince();

        /// <summary>
        /// 获取单个省份信息
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        Province GetProvince(string provinceCode);

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        List<CityMaster> GetCity();

        /// <summary>
        /// 根据省份获取城市信息
        /// </summary>
        /// <param name="provinceCode"></param>
        /// <returns></returns>
        List<CityMaster> GetCity(string provinceCode, string cityCode = null);

        IList<StarLevel> GetStarLevel();

        /// <summary>
        /// 获取广告设置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        IQueryable<AdSet> GetAdSet(string position);

        /// <summary>
        /// 获取渠道信息
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        List<M_v_channelCode> GetM_v_channelCode();

        /// <summary>
        /// 更新酒店渠道信息（渠道签约代码）
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
       int updateHotelChannel(string hid,string code,string refno);

       /// <summary>
       /// 获取指定酒店的在线门锁类型
       /// </summary>
       /// <param name="hid">酒店ID</param>
       /// <returns></returns>
       string GetHotelOnlineLockType(string hid);

       /// <summary>
       /// 获取系统参数
       /// </summary>
       /// <param name="code">代码</param>
       /// <returns>返回value值</returns>
       string GetSysParaValue(string code);

        /// <summary>
        /// 获取酒店付款处理方式
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        string GetHotelItemAction(string hid);
    }
}
