using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 预订客账服务接口
    /// </summary>
    public interface IResFolioService
    {
        #region 入账
        /// <summary>
        /// 根据酒店id和预订明细id返回对应的预订主单客账所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regId">预订明细id</param>
        /// <returns>对应的预订主单客账所需要的所有信息</returns>
        ResFolioMainInfo GetResFolioMainInfoByRegId(string hid, string regId);
        /// <summary>
        /// 根据酒店id和预订id返回对应的预订主单客账所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">预订id</param>
        /// <returns>对应的预订主单客账所需要的所有信息</returns>
        List<ResFolioDetailInfo> GetResFolioDetailInfoByResId(string hid, string resId);
        /// <summary>
        /// 查询满足条件的客账账务明细
        /// </summary>
        /// <param name="para">查询条件</param>
        /// <returns>满足条件的客账账务明细</returns>
        List<ResFolioFolioInfo> QueryResFolioFolioInfos(ResFolioQueryPara para);
        /// <summary>
        /// 查询指定订单下的用于入账的账单选择列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>订单下用于入账的账单列表</returns>
        List<UpQueryResDetailsForFolioAddByResIdResult> QueryResDetailForFolioAdd(string hid, string resId);
        /// <summary>
        /// 增加账务明细的消费记录
        /// </summary>
        /// <param name="para">要增加的消费记录相关参数</param>
        upProfileCaInputResult AddFolioDebit(ResFolioDebitPara para);
        /// <summary>
        /// 增加账务明细的付款记录
        /// </summary>
        /// <param name="para">要增加的付款记录相关参数</param>
        upProfileCaInputResult AddFolioCredit(ResFolioCreditPara para);
        #endregion
        #region 日租半日租
        /// <summary>
        /// 检查是否要收取全日租或半日租，在客人离店时调用
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的明细id</param>
        /// <returns>列表为空，则表示不需要收取，有记录时，则显示界面让操作员确认如何收取</returns>
        List<upResFolioDayChargeCheckResult> DayChargeCheck(string hid, string regIds);
        /// <summary>
        /// 增加收取的日租半日租
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userName">操作员姓名</param>
        /// <param name="shiftId">当前班次id</param>
        /// <param name="chargeInfos">收取的日租半日租信息</param>
        /// <param name="isTemp">是否临时账务</param>
        /// <returns>收取的交易号，多个之间以逗号分隔</returns>
        string DayChargeAdd(string hid, string userName, string shiftId, List<upResFolioDayChargeCheckResult> chargeInfos, bool isTemp = false);
        /// <summary>
        /// 免收日租半日租
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的regid</param>
        /// <param name="freeReason">免收原因</param>
        void DayChargeFree(string hid, string regIds,string freeReason);
        /// <summary>
        /// 查询之前加收的日租，半日租，用于记录日租半日租日志
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的regid</param>
        /// <returns>之前加收的日租，半日租</returns>
        List<UpResFolioDayChargeQueryValidResult> DayChargeQuery(string hid, string regIds);
        /// <summary>
        /// 移除之前加收的日租，半日租
        /// 在回收成功后，结账不成功，或者是没有进行结账操作，直接关闭结账窗口时触发
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的客账id</param>
        /// <param name="inputUser">操作用户代码</param>
        /// <param name="shiftId">班次id</param>
        void DayChargeRemove(string hid, string regIds, string inputUser, string shiftId);
        #endregion
        #region 账务处理结账清账迟付等
        /// <summary>
        /// 检查选中的客单明细和客账明细是否可以直接结账或清账
        /// </summary>
        /// <param name="para">检查参数</param>
        /// <returns>null：可以直接结账，有值时则表示还需要收取或退回的金额，需要弹出入账窗口让操作员进行操作</returns>
        UpResFolioOpCheckResult ResFolioOpCheck(ResFolioOpPara para);
        /// <summary>
        /// 执行账务处理
        /// </summary>
        /// <param name="para">处理参数</param>
        UpResFolioOpResult ResFolioOp(ResFolioOpPara para);
        /// <summary>
        /// 执行账务处理的反处理
        /// </summary>
        /// <param name="para">反处理参数</param>
        UpResFolioOpResult ResFolioReOp(ResFolioOpPara para);
        /// <summary>
        /// 查询可以取消结账的批次号信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>可以取消结账的批次号列表</returns>
        List<UpQueryResFolioCheckoutBatchNosResult> QueryResFolioCheckoutBatchNos(string hid, string resId);
        #endregion
        #region 预授权
        /// <summary>
        /// 查询指定订单对应的预授权信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>订单对应的预授权信息</returns>
        List<UpQueryCardAuthByResIdResult> QueryCardAuths(string hid, string resId);
        /// <summary>
        /// 增加预授权
        /// </summary>
        /// <param name="para">要增加的预授权参数</param>
        void AddCardAuth(ResFolioCardAuthAddPara para);
        /// <summary>
        /// 更新预授权，包含修改，完成，取消
        /// </summary>
        /// <param name="originJsonStr">修改前的原始json字符串</param>
        /// <param name="update">要修改的对象</param>
        /// <param name="shiftId">班次id</param>
        JsonResultData UpdateCardAuth(string originJsonStr, CardAuth update, string shiftId, int isCheckout = 0);
        /// <summary>
        /// 获取预授权主键ID列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">账号列表</param>
        /// <param name="status">预授权状态</param>
        /// <returns></returns>
        List<System.Guid> GetCardAuthIds(string hid, List<string> regids, int status = 1);
        #endregion
        #region 转账
        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="transIds">要转账的账务明细id，多项之间以逗号分隔</param>
        /// <param name="targetRegId">要转到的目标客账id</param>
        /// <param name="username">转账操作员</param>
        /// <param name="shiftId">当前班次id</param>
        /// <returns></returns>
        JsonResultData Transfer(ICurrentInfo currentInfo, string transIds, string targetRegId);
        #endregion
        #region 账务作废与账务恢复
        /// <summary>
        /// 账务作废与账务恢复
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <param name="shiftid">当前班次ID</param>
        /// <param name="isCheck">是否检查，true只检查，不执行操作。false检查且执行</param>
        /// <returns></returns>
        JsonResultData CancelAndRecoveryFolio(string hid, string transid, string shiftid, bool isCheck);
        #endregion
        #region 账务记录
        /// <summary>
        /// 根据账务ID获取账务信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <returns></returns>
        JsonResultData GetFolioByTransid(string hid, System.Guid transid);
        /// <summary>
        /// 根据账务ID获取账务信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <returns></returns>
        ResFolio GetEntity(string hid, System.Guid transid);
        #endregion
        #region 账务日志
        /// <summary>
        /// 添加账务日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userName">操作员</param>
        /// <param name="transId">账务ID</param>
        /// <param name="type">类型（0转账，1账务作废与恢复）</param>
        /// <param name="value1">更换前</param>
        /// <param name="value2">更换后</param>
        /// <param name="other1">相关属性1</param>
        /// <param name="other2">相关属性2</param>
        /// <param name="remark">备注</param>
        /// <param name="describle">描述</param>
        void AddResFolioLog(string hid, string userName, System.Guid transId, byte type, string value1, string value2, string other1 = null, string other2 = null, string remark = null, string describle = null);
        #endregion



        #region 长包房功能
        JsonResultData InAdvanceCheckout_GetEndDate(ICurrentInfo currentInfo, string regIds);
        /// <summary>
        /// 预结
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regIds">账号ID列表</param>
        /// <param name="endDate">预结日期</param>
        /// <returns></returns>
        JsonResultData InAdvanceCheckout(ICurrentInfo currentInfo, string regIds, System.DateTime endDate);
        /// <summary>
        /// 水电费 本次抄表数 入账 检查
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="list">参数</param>
        /// <returns></returns>
        JsonResultData WaterAndElectricity_AddFolioDebit_Check(ICurrentInfo currentInfo, List<PermanentRoomFolioPara.WaterAndElectricityFolioPara> list);
        #endregion

        #region 拆账
        /// <summary>
        /// 拆账
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="para">参数类</param>
        /// <returns></returns>
        JsonResultData SplitFolio(ICurrentInfo currentInfo, ResFolioSplitInfo.Para para);
        #endregion

        #region 迟付账务处理权限
        /// <summary>
        /// 订单是否迟付状态
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">参数类型（1：regId 或 regIds，2：transId 或 transIds）</param>
        /// <param name="value">参数值</param>
        bool IsOutStatusOrder(string hid, int type, string value);
        #endregion

        #region 退款
        /// <summary>
        /// 获取可用退款的账务
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">子单ID列表</param>
        /// <returns></returns>
        List<ResFolioRefundFolioInfo> GetRefundFolios(string hid, string regids);
        #endregion
    }
}
