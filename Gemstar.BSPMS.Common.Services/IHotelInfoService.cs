using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 酒店信息服务接口
    /// </summary>
    public interface IHotelInfoService : ICRUDService<CenterHotel>
    {
        /// <summary>
        /// 获取所有有效酒店列表
        /// 有效酒店是指状态为有效，并且还没有过期
        /// </summary>
        /// <returns>所有有效酒店列表</returns>
        List<CenterHotel> ListValidHotels();
        /// <summary>
        /// 根据指定的酒店id获取对应的酒店信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>null:如果酒店id不存在，酒店信息实例对象：对应酒店的详细信息</returns>
        UpQueryHotelInfoByIdResult GetHotelInfo(string hid);
        /// <summary>
        /// 根据渠道代码，渠道里酒店的签约代码查找对应的酒店信息
        /// </summary>
        /// <param name="channelExtCode">渠道外部代码</param>
        /// <param name="hotelCodeInChannel">渠道里酒店的签约代码</param>
        /// <returns>null：如果渠道里签约代码没有对应酒店，酒店信息实例对象：对应酒店的详细信息</returns>
        UpQueryHotelInfoByIdResult GetHotelInfo(string channelExtCode, string hotelCodeInChannel);
        /// <summary>
        /// 根据指定的酒店id获取对应的酒店接口信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>null:如果酒店id不存在，接口实例对象：对应酒店的接口信息</returns>
        List<UpQueryHotelInterfaceByIdResult> GetHotelInterface(string hid);
        /// <summary>
        /// 下载接口文件
        /// </summary>
        /// <returns></returns>
        UpHardwareInterfacecs GetDownloadFile(string versionId);
        /// <summary>
        /// 获取门锁卡类型
        /// </summary>
        /// <param name="lockType"></param>
        /// <returns></returns>
        IQueryable<M_v_codeListPub> GetLockType();

        string GetLogoUrl(string hid);
        /// <summary>
        /// 获取身份证扫描类型
        /// </summary>
        /// <param name="idCardType"></param>
        /// <returns></returns>
        IQueryable<M_v_codeListPub> GetIdCardType();

        IQueryable<M_v_codeListPub> GetMbrCardType();

        IQueryable<M_v_codeListPub> GetItemAction();

        IQueryable<M_v_codeListPub> GetOnlineLockType();

        IQueryable<M_v_codeListPub> GetManageType();

        IQueryable<CenterHotel> LoadGroup();

        JsonResultData Enable(string id);

        JsonResultData Disable(string id);

        JsonResultData RepeatCheck(string name, string mobile, string hid = "");
        DataTable getHotelPara(string hid);
        int saveChangetHotelPara(string hid, Dictionary<string, string> para);
        bool IsExistCleanRoom(string hid);
        /// <summary>
        /// 更新总裁驾驶舱功能
        /// </summary>
        /// <param name="hid"></param>
        void SaveChangeIsOpenAnalysis(string hid);

        string GetHotelShortName(string hid);
        DataTable GetHotelExpire();
        /// <summary>
        /// 获取基础资料列表
        /// </summary>
        /// <returns>基础资料列表</returns>
        List<M_V_BasicDataType> GetBasicDataForAll();
        /// <summary>
        /// 获取管控属性为集团分发的基础资料列表
        /// </summary>
        /// <returns>管控属性为集团分发的基础资料列表</returns>
        List<M_V_BasicDataType> GetBasicDataForCopy();
        /// <summary>
        /// 当前的主数据库是否通过外网ip进行连接的
        /// </summary>
        /// <returns>true:是，false：否</returns>
        bool IsConnectViaInternte();
    }
}
