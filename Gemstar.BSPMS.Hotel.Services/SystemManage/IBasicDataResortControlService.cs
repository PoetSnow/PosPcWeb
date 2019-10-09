using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
    /// <summary>
    /// 集团分发型基础资料分发属性设置接口
    /// </summary>
    public interface IBasicDataResortControlService:ICRUDService<BasicDataResortControl>
    {
        /// <summary>
        /// 将分发的基础资料同步到集团中，默认为分店可以增加，可以修改，可以禁用，并且分发类型为全部分店
        /// </summary>
        /// <param name="basicDatas">分发型的基础数据列表</param>
        /// <param name="grpId">集团id</param>
        void SyncBasicDatasForCopy(List<M_V_BasicDataType> basicDatas,string grpId);
        /// <summary>
        /// 获取基础数据控制属性
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="hid">当前酒店id</param>
        /// <param name="grpid">当前集团id</param>
        /// <param name="basicDatas">主数据库中的基础资料分发属性列表</param>
        /// <returns>基础数据控制属性</returns>
        BasicDataControl GetBasicDataControl(string basicDataCode, string hid, string grpid,List<M_V_BasicDataType> basicDatas);
        /// <summary>
        /// 获取集团设置的，指定基础数据的分发属性
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="grpid">当前集团id</param>
        /// <returns>集团设置的，指定基础数据的分发属性</returns>
        BasicDataResortControl GetResortControl(string basicDataCode, string grpid);
        /// <summary>
        /// 更新指定基础数据的最后一次选中的分店信息
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="grpid">集团id</param>
        /// <param name="selectedHids">选中的分店信息</param>
        void UpdateResortControlSelectedHids(string basicDataCode, string grpid, string selectedHids);
        /// <summary>
        /// 获取集团设置的基础数据分发方式
        /// </summary>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="hid">当前酒店id</param>
        /// <param name="grpid">当前集团id</param>
        /// <returns>集团设置的基础数据分发方式</returns>
        DataControlType GetGroupSetDataCopyType(string basicDataCode, string hid, string grpid);
        /// <summary>
        /// 获取所有支持的数据分发类型
        /// </summary>
        /// <returns>数据分发类型列表</returns>
        List<DataControlType> GetDataControlTypes();
    }
}
