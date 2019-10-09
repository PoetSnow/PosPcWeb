using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 预订管理服务接口
    /// </summary>
    public interface IResService
    {
        /// <summary>
        /// 获取订单ID
        /// </summary>
        /// <param name="regid">子单ID</param>
        /// <returns></returns>
        string GetResId(string regid);
        /// <summary>
        /// 查询满足条件的预订单
        /// </summary>
        /// <param name="queryPara">查询条件</param>
        /// <returns>满足查询条件的预订单列表</returns>
        List<UpQueryResDetailResult> QueryResDetails(ResDetailQueryPara queryPara);
        /// <summary>
        /// 查询用于通用客账选择窗口的客账信息
        /// </summary>
        /// <param name="queryPara">查询条件</param>
        /// <returns>指定酒店中满足条件的客账信息</returns>
        List<UpQueryResDetailsForCommonResult> QueryResDetails(ResDetailsForCommonPara queryPara);
        /// <summary>
        /// 根据订单明细的id获取对应的整个订单的详细信息，用于订单维护
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regId">订单明细的id</param>
        /// <returns>对应的整个订单的详细信息</returns>
        ResMainInfo GetResMainInfoByRegId(ICurrentInfo currentInfo, string regId);
        ResMainInfo GetResMainInfoByResId(ICurrentInfo currentInfo, string resId);
        /// <summary>
        /// 增加或修改预订单信息
        /// </summary>
        /// <param name="resMainInfo">预订单信息实例</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns>保存成功后的预订单及所有明细信息</returns>
        JsonResultData AddOrUpdateRes(ResMainInfo resMainInfo,ICurrentInfo currentInfo,DateTime businessDate);
        /// <summary>
        /// 批量预订或者入住保存
        /// </summary>
        /// <param name="addModel">批量预订或者入住模型</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns>保存成功后的预订单及所有明细信息</returns>
        JsonResultData BatchAddRes(ResBatchAddModel addModel, ICurrentInfo currentInfo, DateTime businessDate);
        /// <summary>
        /// 获取分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（R:预订，N:nowshow，X:预订取消，I:入住，O:离店迟付状态，C:离店且结帐）</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        List<UpQueryRoomAutoChooseResult> GetRoomFor(string hid, ResDetailStatus type, string roomTypeId, DateTime arrDate, DateTime depDate, string regId);

        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="regId">子单ID</param>
        /// <param name="type">类型（预订R还是入住I）</param>
        /// <returns></returns>
        List<UpQueryRoomTypeChooseResult> GetRoomType(string hid, DateTime arrDate, DateTime depDate, string regId, ResDetailStatus type);

        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="rateId">价格码id</param>
        /// <param name="type">类型（预订R还是入住I）</param>
        /// <returns></returns>
        List<UpQueryRoomTypeChooseResult> GetRoomTypeForRate(string hid, DateTime arrDate, DateTime depDate, string rateId, ResDetailStatus type);

        #region（取消、恢复）子单
        /// <summary>
        /// 取消子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        JsonResultData CancelOrderDetail(string hid, bool isEnvTest, string regId, string saveContinue, out string opLog);
        /// <summary>
        /// 恢复子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        JsonResultData RecoveryOrderDetail(string hid, bool isEnvTest, string regId, string saveContinue, out string opLog);
        /// <summary>
        /// 批量取消或恢复订单
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="resId">主单ID</param>
        /// <param name="type">类型（CancelR：取消预订，CancelI：取消入住，RecoveryR：恢复预订，RecoveryI：恢复入住）</param>
        /// <param name="regIds">子单ID列表</param>
        /// <returns></returns>
        JsonResultData BatchCancelAndRecoveryOrderDetail(ICurrentInfo currentInfo, string resId, string type, List<string> regIds, bool isEnvTest);
        #endregion

        #region 门卡
        /// <summary>
        /// 获取门锁卡信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <returns></returns>
        List<ResDetailLockInfo> GetLockInfo(string hid, string resId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="regid"></param>
        /// <returns></returns>
        string QueryIsLockWrite(string hid ,string regid);
        /// <summary>
        /// 获取门锁写卡参数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userName">操作人</param>
        /// <param name="regId">子单ID</param>
        /// <param name="cardNo">卡号（有值：重写卡，返回门锁表主键ID。无值：发新卡，返回空）</param>
        /// <returns></returns>
        JsonResultData GetLockWriteCardPara(string hid, string userName, string regId, string cardNo);

        /// <summary>
        /// 写卡
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regId">子单ID</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="seqId">门锁表主键ID（有值：重写卡，无值：发新卡）</param>
        void WriteLock(ICurrentInfo currentInfo, string regId, string cardNo, string seqId, string seqNo,string LockType);

        /// <summary>
        /// 注销卡
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="status">21注销，22无卡注销</param>
        JsonResultData CancelLock(string hid, string cardNo, int status,string seqId);

        /// <summary>
        /// 根据门锁信息获取主单ID resid
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNo">卡号</param>
        /// <returns></returns>
        JsonResultData GetResIdByLockInfo(string hid, string cardNo);
        /// <summary>
        /// 根据房号获取主单ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房号</param>
        /// <returns></returns>
        JsonResultData GetResIdByLockRoomNo(string hid, string roomNo);

        /// <summary>
        /// 检查账单内是否有未注销的门卡
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">账号ID，多项之间用逗号隔开</param>
        /// <returns></returns>
        JsonResultData CheckLockInfoByRegIds(string hid, string regids);
        #endregion

        #region 延期
        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="data">（子单ID 和 延期时间）列表</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <param name="delayContinue">是否继续延期</param>
        /// <param name="businessDate">当前营业日</param>
        /// <returns></returns>
        JsonResultData DelayDepDate(string hid, List<KeyValuePairModel<string, DateTime>> data, string saveContinue, string delayContinue, DateTime businessDate, out List<KeyValuePairModel<string, string>> logList, out string msg);
        #endregion

        #region 换房
        /// <summary>
        /// 获取分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <returns></returns>
        List<UpQueryRoomAutoChooseResult> GetRoomForRoomType(string hid, string regId, string roomTypeId);
        /// <summary>
        /// 获取分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <param name="floors">楼层 多项之间用逗号隔开</param>
        /// <param name="features">房间特色 多项之间用逗号隔开</param>
        /// <param name="roomno">房号</param>
        /// <returns></returns>
        List<UpQueryRoomAutoChooseResult> GetRoomForRoomType(string hid, string regId, string roomTypeId, string floors, string features, string roomno);
        /// <summary>
        /// 换房
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="regId">子单ID</param>
        /// <param name="roomId">房间ID</param>
        /// <param name="orderDetailPlans">房价列表</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <param name="businessDate">当前营业日</param>
        /// <param name="authorizationSaveContinue">授权完成后继续保存订单（授权主键ID+授权时间）</param>
        /// <returns></returns>
        JsonResultData ChangeRoom(ICurrentInfo currentInfo, string regId, string roomId, List<ResDetailPlanInfo> orderDetailPlans, string remark, string saveContinue, DateTime businessDate, string authorizationSaveContinue);
        #endregion

        #region 关联房
        /// <summary>
        /// 增加关联（把一个或多个子单添加到另一个订单中）
        /// </summary>
        /// <param name="currentInfo">酒店ID</param>
        /// <param name="addRegIds">要增加关联的子单ID，多个之间用逗号隔开</param>
        /// <param name="toResId">要添加到的订单ID</param>
        /// <returns></returns>
        JsonResultData AddRelation(ICurrentInfo currentInfo, string[] addRegIds, string toResId);

        /// <summary>
        /// 增加关联（把一个订单里的所有子单添加到另一个订单中）
        /// </summary>
        /// <param name="currentInfo">酒店ID</param>
        /// <param name="addResId">订单ID</param>
        /// <param name="toResId">要添加到的订单ID</param>
        /// <returns></returns>
        JsonResultData AddRelation(ICurrentInfo currentInfo, string addResId, string toResId);

        /// <summary>
        /// 取消关联（把子单从订单中分离出来）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        JsonResultData RemoveRelation(string hid, string regId);

        /// <summary>
        /// 获取增加关联房所需要的列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="notResId">不包括订单ID</param>
        /// <param name="roomNo">房间号</param>
        /// <param name="guestName">客人名</param>
        /// <param name="guestMobile">客人手机号</param>
        /// <returns></returns>
        JsonResultData GetRelationList(string hid, string notResId, string roomNo, string guestName, string guestMobile, string status);

        /// <summary>
        /// 获取关联房的主单信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        JsonResultData GetRelationResInfo(string hid, string regId);
        #endregion

        #region 分房、入住
        /// <summary>
        /// 自动分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="isAuto">是否自动分房（true是，false否）</param>
        /// <returns></returns>
        List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>> GetRoomAutoChoose(string hid, string resId, bool isAuto);

        /// <summary>
        /// 保存分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <returns></returns>
        JsonResultData SaveRooms(string hid, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue);

        /// <summary>
        /// 保存分房并且入住
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns></returns>
        JsonResultData SaveRoomsAndCheckIn(ICurrentInfo currentInfo, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue, string useScoreSaveContinue, DateTime businessDate, bool isEnvTest);

        /// <summary>
        /// 清除分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <returns></returns>
        JsonResultData ClearRooms(string hid, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data);
        #endregion

        /// <summary>
        /// 获取订单信息表中字段ID所对应的名称
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resInfo">订单实体</param>
        /// <param name="companyName">合约单位名称</param>
        /// <param name="roomTypeName">房间类型名称</param>
        /// <param name="rateCodeName">房价代码名称</param>
        /// <param name="sourceName">客人来源名称</param>
        /// <param name="marketName">市场分类名称</param>
        void GetFieldName(string hid, ResMainInfo resInfo, out string companyName, out string roomTypeName, out string rateCodeName, out string sourceName, out string marketName);

        /// <summary>
        /// 查询国籍
        /// </summary>
        /// <param name="nation">名称或拼音</param>
        /// <returns></returns>
        List<Entities.V_Nation> GetNationList(string nation);
        /// <summary>
        /// 按regid查询订单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="regid"></param>
        /// <returns></returns>
        Entities.ResDetail GetResDetailRegid(string hid, string regid);

        #region 客人信息批量修改
        /// <summary>
        /// 根据主单ID获取主单内所有客人信息列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">主单ID</param>
        /// <returns></returns>
        List<ResDetailCustomerInfos> GetCustomerInfoByResId(string hid, string resId);
        /// <summary>
        /// 保存客人信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="data">客人信息列表</param>
        /// <returns></returns>
        JsonResultData SaveCustomerInfos(string hid, List<ResDetailCustomerInfos> data, string userName);
        #endregion

        #region 其他
        /// <summary>
        /// 获取子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号</param>
        /// <returns></returns>
        Gemstar.BSPMS.Hotel.Services.Entities.ResDetail GetResDetail(string hid, string regid);
        /// <summary>
        /// 查询房号在住的客人信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        string GetResDetailByRoomNo(string hid, string roomNo);
        /// <summary>
        /// 获取当前营业日房价
        /// </summary>
        /// <param name="regid">账号</param>
        /// <param name="businessDate">当前营业日</param>
        /// <returns></returns>
        decimal? GetCurrentResDetailPrice(string regid, DateTime businessDate);
        /// <summary>
        /// 获取订单内的特殊要求
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        JsonResultData GetSpecialRequirements(string hid, string resid);
        /// <summary>
        /// 查询在线门锁信息
        /// </summary>
        /// <param name="queryPara">查询参数</param>
        /// <returns>查询结果</returns>
        OnlineLockQueryResult GetOnlineLockPara(OnlineLockQueryPara queryPara);
        /// <summary>
        /// 根据主单ID获取子单列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        List<KeyValuePairModel<string, string>> GetResDetailsByResId(string hid, string resid);
        /// <summary>
        /// 根据证件类型和证件号码获取会员信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="cerType"></param>
        /// <param name="cerId"></param>
        JsonResultData GetProfileInfoByCerId(string hid, string cerType, string cerId, bool isGroup);
        #endregion

        #region 更新备注
        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="CurrentInfo">登录信息</param>
        /// <param name="regid">账号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData UpdateRemark(ICurrentInfo CurrentInfo, string regid, string remark);
        #endregion
    }
    #region 在线门锁
    /// <summary>
    /// 在线门锁查询类型
    /// </summary>
    public enum OnlineLockQueryParaType
    {
        QueryByTransId = 1,
        QueryByRegId = 2
    }
    /// <summary>
    /// 在线门锁查询参数
    /// </summary>
    public class OnlineLockQueryPara
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 查询参数类型
        /// </summary>
        public OnlineLockQueryParaType Type { get; set; }
        /// <summary>
        /// 对应的查询参数值
        /// </summary>
        public string QueryParaValue { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Hid) && !string.IsNullOrWhiteSpace(QueryParaValue);
            }
        }
    }
    /// <summary>
    /// 在线门锁查询结果
    /// </summary>
    public class OnlineLockQueryResult
    {
        /// <summary>
        /// 查询是否成功
        /// </summary>
        public bool QuerySuccessed { get; set; }
        /// <summary>
        /// 订单账号，用户后续查询订单入住状态
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 客房代码，后续需要根据客房代码查询锁号
        /// </summary>
        public string RoomCode { get; set; }
        /// <summary>
        /// 客人名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 客人手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 客人证件号
        /// </summary>
        public string CerNo { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
    } 
    #endregion
}
