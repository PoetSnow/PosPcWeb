using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using System;

namespace Gemstar.BSPMS.Hotel.Services.PermanentRoomManage
{
    public interface IPermanentRoomGoodsService : ICRUDService<PermanentRoomGoodsSet>
    {
        /// <summary>
        /// 子单号是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">主单ID</param>
        /// <returns></returns>
        bool ExistsRegId(string hid, string regid);
        /// <summary>
        /// 长包房设置表主键ID是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">主单ID</param>
        /// <returns></returns>
        bool ExistsPermanentRoomId(string hid, string permanentRoomSetId);
        /// <summary>
        /// 获取长包房设置表 主键ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">子单ID</param>
        /// <returns></returns>
        string GetPermanentRoomId(string hid, string regid);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">长包房设置表ID</param>
        /// <returns></returns>
        List<PermanentRoomGoodsSet> ToList(string hid, string permanentRoomSetId);

        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<PermanentRoomGoodsSetTemplete> ToTempleteList(string hid);
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        List<PermanentRoomGoodsSet> ToTempleteDetailList(string hid, Guid id);
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">模板名称</param>
        /// <param name="addVersions">模板内容</param>
        /// <returns></returns>
        JsonResultData AddTemplete(string hid, string name, List<PermanentRoomGoodsSet> addVersions);
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
