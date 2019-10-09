using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using System;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IResBillSettingService : ICRUDService<ResBillSetting>
    {
        /// <summary>
        /// 主单号是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        bool ExistsResId(string hid, string resid);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        List<ResBillSettingInfo> ToList(string hid, string resid);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        List<ResBillSetting> ToListAll(string hid, string resid);

        /// <summary>
        /// 获取有账务的账单列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        List<string> ListItemForResBillId(string hid, string resid);

        /// <summary>
        /// 调账
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">主单ID</param>
        /// <param name="folioIds">账务ID列表</param>
        /// <param name="toResBillCode">目标账单代码</param>
        /// <returns></returns>
        JsonResultData AdjustFolio(string hid, string resId, Guid[] folioIds, string toResBillCode, string userName);

        /// <summary>
        /// 账单设置应用到账务中
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <param name="userName">用户名</param>
        JsonResultData ResBillSettingToFolio(string hid, string resid, string userName);


        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<ResBillSettingTemplete> ToTempleteList(string hid);
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        List<ResBillSetting> ToTempleteDetailList(string hid, Guid id);
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">模板名称</param>
        /// <param name="addVersions">模板内容</param>
        /// <returns></returns>
        JsonResultData AddTemplete(string hid, string name, List<ResBillSetting> addVersions);
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        void DelTemplete(string hid, Guid id);
        #endregion
    }
}
