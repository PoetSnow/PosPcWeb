using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Tools;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage
{
    /// <summary>
    /// 预订客账服务的ef实现类
    /// </summary>
    public class ResFolioService : IResFolioService
    {
        public ResFolioService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        #region 入账
        /// <summary>
        /// 根据酒店id和预订明细id返回对应的预订主单客账所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regId">预订明细id</param>
        /// <returns>对应的预订主单客账所需要的所有信息</returns>
        public ResFolioMainInfo GetResFolioMainInfoByRegId(string hid, string regId)
        {
            var details = _pmsContext.Database.SqlQuery<ResFolioDetailInfo>("exec up_queryResDetailsForFolioByRegId @hid=@hid,@regId = @regId"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@regId", regId ?? "")
                ).ToList();
            //转换明细中的状态为对应的名称，以方便显示
            foreach (var d in details)
            {
                d.StatuName = EnumExtension.GetDescription(typeof(ResDetailStatus), d.StatuName);
            }
            var mainInfo = new ResFolioMainInfo
            {
                FolioDetails = details
            };
            return mainInfo;
        }
        /// <summary>
        /// 根据酒店id和预订id返回对应的预订主单客账所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">预订id</param>
        /// <returns>对应的预订主单客账所需要的所有信息</returns>
        public List<ResFolioDetailInfo> GetResFolioDetailInfoByResId(string hid, string resId)
        {
            var details = _pmsContext.Database.SqlQuery<ResFolioDetailInfo>("exec up_queryResDetailsForFolioByResId @hid=@hid,@resId = @resId"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@resId", resId ?? "")
                ).ToList();
            //转换明细中的状态为对应的名称，以方便显示
            foreach (var d in details)
            {
                d.StatuName = EnumExtension.GetDescription(typeof(ResDetailStatus), d.StatuName);
            }
            return details;
        }

        /// <summary>
        /// 查询满足条件的客账账务明细
        /// </summary>
        /// <param name="para">查询条件</param>
        /// <returns>满足条件的客账账务明细</returns>
        public List<ResFolioFolioInfo> QueryResFolioFolioInfos(ResFolioQueryPara para)
        {
            return _pmsContext.Database.SqlQuery<ResFolioFolioInfo>("exec up_queryResFolioForFolio @hid=@hid,@regIds=@regIds,@status=@status,@transDateBegin=@transDateBegin,@transDateEnd=@transDateEnd,@itemTypeIds=@itemTypeIds,@roomNo=@roomNo,@resBillCode=@resBillCode"
                , new SqlParameter("@hid", para.Hid ?? "")
                , new SqlParameter("@regIds", para.RegIds ?? "")
                , new SqlParameter("@status", (int)para.Status)
                , new SqlParameter("@transDateBegin", para.TransDateBegin ?? "")
                , new SqlParameter("@transDateEnd", para.TransDateEnd ?? "")
                , new SqlParameter("@itemTypeIds", para.ItemTypeIds ?? "")
                , new SqlParameter("@roomNo",para.RoomNo??"")
                , new SqlParameter("@resBillCode", para.ResBillCode)
                ).ToList();
        }
        /// <summary>
        /// 查询指定订单下的用于入账的账单选择列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>订单下用于入账的账单列表</returns>
        public List<UpQueryResDetailsForFolioAddByResIdResult> QueryResDetailForFolioAdd(string hid, string resId)
        {
            return _pmsContext.Database.SqlQuery<UpQueryResDetailsForFolioAddByResIdResult>("exec up_queryResDetailsForFolioAddByResId @hid=@hid,@resId =@resId"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@resId", resId ?? "")
                ).ToList();
        }
        /// <summary>
        /// 增加账务明细的消费记录
        /// </summary>
        /// <param name="para">要增加的消费记录相关参数</param>
        public upProfileCaInputResult AddFolioDebit(ResFolioDebitPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ApplicationException("请指定账务明细所属的酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.RegId))
            {
                throw new ApplicationException("请指定账务明细所属的客账id");
            }
            if (string.IsNullOrWhiteSpace(para.ItemId))
            {
                throw new ApplicationException("请指定账务明细的付款方式");
            }
            return _pmsContext.Database.SqlQuery<upProfileCaInputResult>("exec up_resFolio_input @hid=@hid,@regid=@regid,@itemid=@itemid,@quantity=@quantity,@amount=@amount,@invNo=@invNo,@inputUser=@inputUser,@remark=@remark,@transShift=@transShift"
                , new SqlParameter("@hid", SqlDbType.VarChar) { Value = para.Hid }
                , new SqlParameter("@regid", SqlDbType.VarChar) { Value = para.RegId }
                , new SqlParameter("@itemid", SqlDbType.VarChar) { Value = para.ItemId }
                , new SqlParameter("@quantity", SqlDbType.Decimal) { Value = para.Quantity??1 }
                , new SqlParameter("@amount", SqlDbType.Decimal) { Value = para.Amount }
                , new SqlParameter("@invNo", SqlDbType.VarChar) { Value = para.InvNo ?? "" }
                , new SqlParameter("@inputUser", SqlDbType.VarChar) { Value = para.UserName ?? "" }
                , new SqlParameter("@remark", SqlDbType.VarChar) { Value = para.Remark ?? "" }
                , new SqlParameter("@transShift", SqlDbType.VarChar) { Value = para.TransShift??""}
                ).FirstOrDefault();
        }
        /// <summary>
        /// 增加账务明细的付款记录
        /// </summary>
        /// <param name="para">要增加的付款记录相关参数</param>
        public upProfileCaInputResult AddFolioCredit(ResFolioCreditPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ApplicationException("请指定账务明细所属的酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.RegId))
            {
                throw new ApplicationException("请指定账务明细所属的客账id");
            }
            if (string.IsNullOrWhiteSpace(para.ItemId))
            {
                throw new ApplicationException("请指定账务明细的付款方式");
            }
            var pInputDate = new SqlParameter();
            pInputDate.ParameterName = "@inputDate";
            if (para.InputDate.HasValue)
            {
                pInputDate.Value = para.InputDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }else
            {
                pInputDate.Value = DBNull.Value;
            }
            return _pmsContext.Database.SqlQuery<upProfileCaInputResult>("exec up_resFolio_input @hid=@hid,@regid=@regid,@itemid=@itemid,@oriAmount=@oriAmount,@amount=@amount,@invNo=@invNo,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@status=@status,@transShift=@transShift,@paymentdesc=@paymentdesc,@inputDate=@inputDate,@depositType=@depositType"
                , new SqlParameter("@hid", SqlDbType.VarChar) { Value = para.Hid }
                , new SqlParameter("@regid", SqlDbType.VarChar) { Value = para.RegId }
                , new SqlParameter("@itemid", SqlDbType.VarChar) { Value = para.ItemId }
                , new SqlParameter("@oriAmount", SqlDbType.Decimal) { Value = para.OriAmount }
                , new SqlParameter("@amount", SqlDbType.Decimal) { Value = para.Amount }
                , new SqlParameter("@invNo", SqlDbType.VarChar) { Value = para.InvNo ?? "" }
                , new SqlParameter("@inputUser", SqlDbType.VarChar) { Value = para.UserName ?? "" }
                , new SqlParameter("@remark", SqlDbType.VarChar) { Value = para.Remark ?? "" }
                , new SqlParameter("@refno", SqlDbType.VarChar) { Value = para.RefNo ?? "" }
                ,new SqlParameter("@status", SqlDbType.TinyInt) { Value = para.IsWaitPay ? 52 : (para.Invalid ? 51 : 1) }
                ,new SqlParameter("@transShift", SqlDbType.VarChar) { Value = para.TransShift??""}
                ,new SqlParameter("@paymentdesc", SqlDbType.VarChar) { Value=para.Paymentdesc??""}
                , new SqlParameter("@depositType", SqlDbType.VarChar) { Value = para.FolioDepositType ?? "" }
                , pInputDate
                ).FirstOrDefault();
        }

        #endregion
        #region 日租半日租
        /// <summary>
        /// 检查是否要收取全日租或半日租，在客人离店时调用
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的明细id</param>
        /// <returns>列表为空，则表示不需要收取，有记录时，则显示界面让操作员确认如何收取</returns>
        public List<upResFolioDayChargeCheckResult> DayChargeCheck(string hid, string regIds)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                throw new ApplicationException("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(regIds))
            {
                throw new ApplicationException("请选择房间");
            }
            return _pmsContext.Database.SqlQuery<upResFolioDayChargeCheckResult>("exec up_resFolio_dayCharge_check @hid=@hid,@regids=@regids"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@regids", regIds ?? "")
                ).ToList();
        }
        /// <summary>
        /// 增加收取的日租半日租
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="userName">操作员姓名</param>
        /// <param name="shiftId">当前班次id</param>
        /// <param name="chargeInfos">收取的日租半日租信息</param>
        /// <param name="isTemp">是否临时账务</param>
        /// <returns>收取的交易号，多个之间以逗号分隔</returns>
        public string DayChargeAdd(string hid, string userName, string shiftId, List<upResFolioDayChargeCheckResult> chargeInfos, bool isTemp = false)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                throw new ApplicationException("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(shiftId))
            {
                throw new ApplicationException("请指定当前班次");
            }
            if (chargeInfos == null || chargeInfos.Count == 0)
            {
                throw new ApplicationException("请指定要收取的日租半日租信息");
            }
            var opInfo = new StringBuilder();
            opInfo.Append("<regids>");
            foreach (var info in chargeInfos)
            {
                opInfo.AppendFormat("<regid id=\"{0}\"  type=\"{1}\"  amount=\"{2}\"></regid>", info.RegId, info.Type, info.Amount);
            }
            opInfo.Append("</regids>");
            return _pmsContext.Database.SqlQuery<string>("exec up_resFolio_dayCharge_op @hid=@hid,@opInfo=@opInfo,@inputUser=@inputUser,@shiftid=@shiftid,@isTemp=@isTemp"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@opInfo", opInfo.ToString())
                , new SqlParameter("@inputUser", userName ?? "")
                , new SqlParameter("@shiftid", shiftId ?? "")
                , new SqlParameter("@isTemp", isTemp ? 1 : 0)
                ).SingleOrDefault();
        }
        /// <summary>
        /// 免收日租半日租
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的regid</param>
        /// <param name="freeReason">免收原因</param>
        public void DayChargeFree(string hid, string regIds, string freeReason)
        {
            _pmsContext.Database.ExecuteSqlCommand("exec up_resFolio_dayCharge_free @hid=@hid,@regIds=@regIds,@freeReason=@freeReason"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@regIds", regIds ?? "")
                , new SqlParameter("@freeReason", freeReason ?? "")
                );
        }
        /// <summary>
        /// 查询之前加收的日租，半日租，用于记录日租半日租日志
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的regid</param>
        /// <returns>之前加收的日租，半日租</returns>
        public List<UpResFolioDayChargeQueryValidResult> DayChargeQuery(string hid, string regIds)
        {
            return _pmsContext.Database.SqlQuery<UpResFolioDayChargeQueryValidResult>("exec up_resFolio_dayCharge_queryValid @hid=@hid,@regids=@regids"
                , new SqlParameter("@hid",hid??"")
                , new SqlParameter("@regids",regIds??"")
                ).ToList();
        }

        /// <summary>
        /// 移除之前加收的日租，半日租
        /// 在回收成功后，结账不成功，或者是没有进行结账操作，直接关闭结账窗口时触发
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="regIds">以逗号分隔的客账id</param>
        /// <param name="inputUser">操作用户代码</param>
        /// <param name="shiftId">班次id</param>
        public void DayChargeRemove(string hid, string regIds, string inputUser, string shiftId)
        {
            _pmsContext.Database.ExecuteSqlCommand("exec up_resFolio_dayCharge_delete @hid=@hid,@regids=@regids,@inputUser=@inputUser,@shiftid=@shiftid"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@regids", regIds ?? "")
                , new SqlParameter("@inputUser", inputUser ?? "")
                , new SqlParameter("@shiftid",shiftId??"")
                );
        }
        #endregion
        #region 账务处理结账清账迟付等
        /// <summary>
        /// 检查选中的客单明细和客账明细是否可以直接结账或清账
        /// </summary>
        /// <param name="para">检查参数</param>
        /// <returns>null：可以直接结账，有值时则表示还需要收取或退回的金额，需要弹出入账窗口让操作员进行操作</returns>
        public UpResFolioOpCheckResult ResFolioOpCheck(ResFolioOpPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ApplicationException("请指定要检查的酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.OpType))
            {
                throw new ApplicationException("请指定要检查的的检查类型");
            }
            if (string.IsNullOrWhiteSpace(para.InputUser))
            {
                throw new ApplicationException("请指定要检查的的操作员姓名");
            }
            if (string.IsNullOrWhiteSpace(para.Shiftid))
            {
                throw new ApplicationException("请指定要检查的班次id");
            }
            return _pmsContext.Database.SqlQuery<UpResFolioOpCheckResult>("exec up_resFolio_op @hid=@hid,@optype=@optype,@regids=@regids,@transids=@transids,@inputUser=@inputUser,@shiftid=@shiftid"
                , new SqlParameter("@hid",para.Hid??"")
                ,new SqlParameter("@optype",para.OpType??"")
                ,new SqlParameter("@regids",para.RegIds??"")
                ,new SqlParameter("@transids",para.TransIds??"")
                ,new SqlParameter("@inputUser",para.InputUser??""),
                new SqlParameter("@shiftid",para.Shiftid??"")
                ).FirstOrDefault();
        }
        /// <summary>
        /// 执行账务处理
        /// </summary>
        /// <param name="para">处理参数</param>
        public UpResFolioOpResult ResFolioOp(ResFolioOpPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ApplicationException("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.OpType))
            {
                throw new ApplicationException("请指定类型");
            }
            if (string.IsNullOrWhiteSpace(para.InputUser))
            {
                throw new ApplicationException("请指定操作员姓名");
            }
            if (string.IsNullOrWhiteSpace(para.Shiftid))
            {
                throw new ApplicationException("请指定班次id");
            }
            if(para.OpType == "out" && string.IsNullOrWhiteSpace(para.OutReason))
            {
                throw new ApplicationException("请输入迟付原因");
            }
            return _pmsContext.Database.SqlQuery<UpResFolioOpResult>("exec up_resFolio_op @hid=@hid,@optype=@optype,@regids=@regids,@transids=@transids,@inputUser=@inputUser,@shiftid=@shiftid,@PayLateRemark=@PayLateRemark"
                , new SqlParameter("@hid",para.Hid??"")
                ,new SqlParameter("@optype",para.OpType??"")
                ,new SqlParameter("@regids",para.RegIds??"")
                ,new SqlParameter("@transids",para.TransIds??"")
                ,new SqlParameter("@inputUser",para.InputUser??"")
                ,new SqlParameter("@shiftid",para.Shiftid??"")
                ,new SqlParameter("@PayLateRemark",para.OutReason??"")
                ).FirstOrDefault();            
        }
        /// <summary>
        /// 执行账务处理的反处理
        /// </summary>
        /// <param name="para">反处理参数</param>
        public UpResFolioOpResult ResFolioReOp(ResFolioOpPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ApplicationException("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.OpType))
            {
                throw new ApplicationException("请指定类型");
            }
            if (string.IsNullOrWhiteSpace(para.InputUser))
            {
                throw new ApplicationException("请指定操作员姓名");
            }
            if (string.IsNullOrWhiteSpace(para.Shiftid))
            {
                throw new ApplicationException("请指定班次id");
            }
            UpResFolioOpResult result = new UpResFolioOpResult();
            using (var tx = new TransactionScope())
            {
                result = _pmsContext.Database.SqlQuery<UpResFolioOpResult>("exec up_resFolio_reOp @hid=@hid,@optype=@optype,@regids=@regids,@billid=@billid,@inputUser=@inputUser,@shiftid=@shiftid"
                    , new SqlParameter("@hid", para.Hid ?? "")
                    , new SqlParameter("@optype", para.OpType ?? "")
                    , new SqlParameter("@regids", para.RegIds ?? "")
                    , new SqlParameter("@billid", para.TransIds ?? "")
                    , new SqlParameter("@inputUser", para.InputUser ?? ""),
                    new SqlParameter("@shiftid", para.Shiftid ?? "")
                    ).FirstOrDefault();
                tx.Complete();
            }
            return result;
        }
        /// <summary>
        /// 查询可以取消结账的批次号信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>可以取消结账的批次号列表</returns>
        public List<UpQueryResFolioCheckoutBatchNosResult> QueryResFolioCheckoutBatchNos(string hid, string resId)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                throw new ArgumentException("请指定要查询可以取消结账批次的酒店id", "hid");
            }
            if (string.IsNullOrWhiteSpace(resId))
            {
                throw new ArgumentException("请指定要查询可以取消结账批次的订单id", "resId");
            }
            return _pmsContext.Database.SqlQuery<UpQueryResFolioCheckoutBatchNosResult>("exec up_queryResFolioCheckoutBatchNos @hid=@hid,@resid = @resId"
                , new SqlParameter("@hid",hid??"")
                , new SqlParameter("@resId",resId??"")
                ).ToList();
        }
        #endregion
        #region 预授权
        /// <summary>
        /// 查询指定订单对应的预授权信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resId">订单id</param>
        /// <returns>订单对应的预授权信息</returns>
        public List<UpQueryCardAuthByResIdResult> QueryCardAuths(string hid, string resId)
        {
            return _pmsContext.Database.SqlQuery<UpQueryCardAuthByResIdResult>("exec up_queryCardAuthByResId @hid=@hid,@resid=@resId"
                , new SqlParameter("@hid",hid??"")
                , new SqlParameter("@resId",resId??"")
                ).ToList();
        }
        /// <summary>
        /// 增加预授权
        /// </summary>
        /// <param name="para">要增加的预授权参数</param>
        public void AddCardAuth(ResFolioCardAuthAddPara para)
        {
            if (string.IsNullOrWhiteSpace(para.Hid))
            {
                throw new ArgumentException("请指定预授权所属酒店id");
            }
            if (string.IsNullOrWhiteSpace(para.RegId))
            {
                throw new ArgumentException("请指定预授权所属的房间");
            }
            if (string.IsNullOrWhiteSpace(para.ItemId))
            {
                throw new ArgumentException("请指定预授权对应的付款项目，以便在完成预授权时自动增加付款记录");
            }
            if (string.IsNullOrWhiteSpace(para.CardNo))
            {
                throw new ArgumentException("请输入预授权的信用卡卡号");
            }
            if (string.IsNullOrWhiteSpace(para.ValidDate))
            {
                throw new ArgumentException("请输入信用卡的有效期，如2018-01");
            }
            if (string.IsNullOrWhiteSpace(para.AuthNo))
            {
                throw new ArgumentException("请输入预授权的授权号");
            }
            if (!para.AuthAmount.HasValue)
            {
                throw new ArgumentException("请输入预授权的授权金额");
            }
            if (para.AuthAmount.Value <= 0)
            {
                throw new ArgumentException("输入的预授权金额必须大于0");
            }
            _pmsContext.Database.ExecuteSqlCommand("exec up_resFolio_cardAuth_add @hid=@hid,@regId=@regId,@itemId=@itemId,@cardNo=@cardNo,@validDate=@validDate,@authNo=@authNo,@authAmount=@authAmount,@createUser=@createUser,@remark=@remark"
                , new SqlParameter("@hid", para.Hid ?? "")
                , new SqlParameter("@regId", para.RegId ?? "")
                , new SqlParameter("@itemId", para.ItemId ?? "")
                , new SqlParameter("@cardNo", para.CardNo ?? "")
                , new SqlParameter("@validDate", para.ValidDate ?? "")
                , new SqlParameter("@authNo", para.AuthNo ?? "")
                , new SqlParameter("@authAmount", para.AuthAmount.Value)
                , new SqlParameter("@createUser", para.CreateUser ?? "")
                , new SqlParameter("@remark", para.Remark ?? "")
                );
            UpResDetail_UpdateCardAuthAmount(para.Hid, para.RegId);
        }
        /// <summary>
        /// 更新预授权，包含修改，完成，取消
        /// </summary>
        /// <param name="originJsonStr">修改前的原始json字符串</param>
        /// <param name="update">要修改的对象</param>
        /// <param name="shiftId">班次id</param>
        public JsonResultData UpdateCardAuth(string originJsonStr, CardAuth update,string shiftId, int isCheckout = 0)
        {
            if (update.Status == 2)
            {
                if (!update.BillAmount.HasValue || update.BillAmount <= 0)
                {
                    throw new ArgumentException("完成预授权时必须指定扣款金额");
                }
            }
            var serializer = new JavaScriptSerializer();
            var origin = serializer.Deserialize<CardAuth>(originJsonStr);
            upProfileCaInputResult addResult = null;
            var updateEntityInDb = _pmsContext.CardAuthes.SingleOrDefault(w => w.Id == update.Id);
            if (origin.Status == 2 && update.Status == 51)
            {
                //如果修改对象的状态是从 完成 改为 取消，则不修改内容，只修改预授权状态和账务状态
                if(origin.Id == update.Id)
                {
                    if(updateEntityInDb != null && updateEntityInDb.Status == 2)
                    {
                        string refNo = updateEntityInDb.Id.ToString();
                        var resFolioEntity = _pmsContext.ResFolios.Where(c => c.Regid == updateEntityInDb.Regid && c.RefNo == refNo && c.Status == 1).FirstOrDefault();
                        if(resFolioEntity != null)
                        {
                            updateEntityInDb.Status = 51;
                            resFolioEntity.Status = 51;
                            resFolioEntity.CancelAndRecoveryDate = DateTime.Now;
                            _pmsContext.Entry(updateEntityInDb).State = System.Data.Entity.EntityState.Modified;
                            _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.预授权更新);
                            _pmsContext.Entry(resFolioEntity).State = System.Data.Entity.EntityState.Modified;
                            _pmsContext.SaveChanges();
                            uPResDetailUpdateAmount(resFolioEntity.Hid, resFolioEntity.Regid);
                            return JsonResultData.Successed();
                        }
                    }
                }
                return JsonResultData.Failure("与此项预授权相关联的账务必须是未结状态才能取消预授权。");
            }
            using (var tx = new TransactionScope())
            {
                //如果新修改的对象的状态是完成，则调用入账存储过程来增加付款记录
                if (update.Status == 2 && updateEntityInDb.Status == 1)
                {
                    addResult = AddFolioCredit(new ResFolioCreditPara
                    {
                        Hid = update.Hid,
                        RegId = update.Regid,
                        ItemId = update.Itemid,
                        OriAmount = update.BillAmount.Value,
                        UserName = update.CompleteUse,
                        Remark = string.Format("预授权完成时自动插入的付款"),
                        InvNo = "",
                        RefNo = update.Id.ToString(),
                        TransShift = shiftId,
                        Paymentdesc = isCheckout == 1 ? (update.BillAmount.Value > 0 ? "C" : "D") : ""
                    });
                }
                var needUpdateFieldNames = new List<string> { "Regid", "Itemid", "CardNo", "ValidDate", "AuthNo", "AuthAmount", "BillAmount", "Remark", "Status" };
                if(update.Status != 1)
                {
                    needUpdateFieldNames.AddRange(new List<string> { "CompleteDate", "CompleteBsnsDate", "CompleteUse" });
                }
                CRUDService<CardAuth>.Update<CardAuth>(_pmsContext, update, origin, needUpdateFieldNames);
                _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.预授权更新);
                _pmsContext.SaveChanges();
                tx.Complete();
            }
            UpResDetail_UpdateCardAuthAmount(update.Hid, (update.Regid + "," + origin.Regid));
            return JsonResultData.Successed((addResult != null && !string.IsNullOrWhiteSpace(addResult.Data)) ? addResult.Data : "");
        }
        /// <summary>
        /// 获取预授权主键ID列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">账号列表</param>
        /// <param name="status">预授权状态</param>
        /// <returns></returns>
        public List<Guid> GetCardAuthIds(string hid, List<string> regids, int status = 1)
        {
            var cardAuthIds = _pmsContext.CardAuthes.Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.Status == status).OrderBy(c => c.Regid).Select(c => c.Id).ToList();
            if (cardAuthIds != null && cardAuthIds.Count > 0)
            {
                return cardAuthIds;
            }
            return new List<Guid>();
        }
        /// <summary>
        /// 更新订单明细表授权金额
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="regid"></param>
        public void UpResDetail_UpdateCardAuthAmount(string hid, string regid)
        {
            if(string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regid)) { return; }
            try
            {
                _pmsContext.Database.ExecuteSqlCommand("exec up_resDetail_updateCardAuthAmount @hid=@hid,@regids=@regids;"
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@regids", regid ?? "")
                );
            }
            catch { }
        }
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
        public JsonResultData Transfer(ICurrentInfo currentInfo, string transIds, string targetRegId)
        {
            try
            {
                string hid = currentInfo.HotelId;
                string username = currentInfo.UserName;
                string shiftId = currentInfo.ShiftId;
                if (string.IsNullOrWhiteSpace(hid)) { return JsonResultData.Failure("请指定酒店id"); }
                if (string.IsNullOrWhiteSpace(transIds)){ return JsonResultData.Failure("请选择要转账的账务明细"); }
                if (string.IsNullOrWhiteSpace(targetRegId)){ return JsonResultData.Failure("请选择要转到的目标客账"); }

                List<Guid> transIdList = GetTransIds(transIds);
                var originResFolios = _pmsContext.ResFolios.Where(c => c.Hid == hid && transIdList.Contains(c.Transid)).AsNoTracking().ToList();

                var result = _pmsContext.Database.SqlQuery<UpResFolioOpResult>("exec up_resFolio_transfer @hid=@hid,@transids=@transids,@regId=@regId,@inputUser=@inputUser,@shiftid=@shiftid"
                    , new SqlParameter("@hid",hid??"")
                    , new SqlParameter("@transids",transIds??"")
                    , new SqlParameter("@regId",targetRegId??"")
                    , new SqlParameter("@inputUser",username??"")
                    , new SqlParameter("@shiftid",shiftId??"")
                    ).FirstOrDefault();
                TransferAddResFolioLog(currentInfo, transIdList, originResFolios, targetRegId);
                return JsonResultData.Successed(result);
            }catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        #endregion
        #region 支付回调处理
        /// <summary>
        /// 更改账务明细状态为支付失败
        /// </summary>
        /// <param name="pmsContext">数据库上下文</param>
        /// <param name="hid">酒店id</param>
        /// <param name="folioTransId">账务明细id</param>
        public static void UpdateFolioAfterPayFailure(DbHotelPmsContext pmsContext,string hid,string folioTransId)
        {
            UpdateFolioAfterPayment(pmsContext, hid, folioTransId, "", 51);
        }
        /// <summary>
        /// 更改账务明细状态为支付成功
        /// </summary>
        /// <param name="pmsContext">数据库上下文</param>
        /// <param name="hid">酒店id</param>
        /// <param name="folioTransId">账务明细id</param>
        /// <param name="payno">支付号，用于对账</param>
        public static void UpdateFolioAfterPaySuccess(DbHotelPmsContext pmsContext, string hid, string folioTransId,string payno)
        {
            UpdateFolioAfterPayment(pmsContext, hid, folioTransId, payno, 1);
        }

        /// <summary>
        /// 更新客账明细的状态，在支付完成后调用
        /// </summary>
        /// <param name="pmsContext">数据库上下文</param>
        /// <param name="hid">酒店id</param>
        /// <param name="folioTransId">客账明细id</param>
        /// <param name="refno">支付号，用于对账</param>
        /// <param name="status">状态值</param>
        private static void UpdateFolioAfterPayment(DbHotelPmsContext pmsContext,string hid,string folioTransId,string refno,byte status)
        {
            //将transId还原成数据库认识的格式
            var transId = Guid.Parse(folioTransId).ToString();
            pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.resFolio SET refNo = @refno,status = @status WHERE hid = @hid AND transid = @transid"
                , new SqlParameter("@refno", refno ?? "")
                , new SqlParameter("@status", status)
                , new SqlParameter("@hid", hid ?? "")
                , new SqlParameter("@transid", transId ?? "")
                );
            //更新订单明细表汇总金额
            try
            {
                var transIdGuid = Guid.Parse(folioTransId);
                if (transIdGuid != null && transIdGuid != Guid.Empty)
                {
                    var regid = pmsContext.ResFolios.Where(c => c.Hid == hid && c.Transid == transIdGuid).Select(c => c.Regid).AsNoTracking().FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(regid))
                    {
                        new ResFolioService(pmsContext).uPResDetailUpdateAmount(hid, regid);
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// 增加账务明细待支付记录
        /// </summary>
        /// <param name="pmsContext">数据库上下文</param>
        /// <param name="productType">产品类型</param>
        /// <param name="productTransId">产品业务id</param>
        /// <param name="payAction">支付方式</param>
        /// <param name="payOrderNo">支付系统生成的临时支付订单号</param>
        /// <param name="qrCodeUrl">二维码地址</param>
        /// <returns>待支付记录的id</returns>
        public static string AddFolioPayInfo(DbHotelPmsContext pmsContext, PayProductType productType, string productTransId, string payAction, string payOrderNo, string qrCodeUrl,string hid,string itemId,decimal amount)
        {
            Guid temp;
            if(Guid.TryParse(productTransId,out temp))
            {
                productTransId = temp.ToString();
            }
            var payInfo = new ResFolioPayInfo
            {
                ProductType = productType.ToString(),
                ProductTransId = productTransId,
                Cdate = DateTime.Now,
                PayAction = payAction,
                PayOrderNo = payOrderNo,
                Status = 0,
                QrcodeUrl = qrCodeUrl,
                Hid = hid,
                ItemId = itemId,
                Amount = amount
            };
            pmsContext.ResFolioPayInfos.Add(payInfo);
            pmsContext.SaveChanges();
            return payInfo.Id.ToString();
        }
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
        public JsonResultData CancelAndRecoveryFolio(string hid, string transid, string shiftid, bool isCheck)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("请指定酒店id");
            }
            if (string.IsNullOrWhiteSpace(transid))
            {
                return JsonResultData.Failure("请选择账务");
            }
            if (string.IsNullOrWhiteSpace(shiftid))
            {
                return JsonResultData.Failure("请指定班次id");
            }
            try
            {
                var result = _pmsContext.Database.SqlQuery<string>("exec up_resFolio_cancelAndRecovery @hid=@hid,@transid=@transid,@shiftid=@shiftid,@ischeck=@ischeck"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@transid", transid)
                    , new SqlParameter("@shiftid", shiftid)
                    , new SqlParameter("@ischeck", isCheck)
                ).FirstOrDefault();
                if(!string.IsNullOrWhiteSpace(result))
                {
                    return JsonResultData.Successed(result);
                }
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
            return JsonResultData.Successed();
        }
        #endregion

        #region 账务记录
        /// <summary>
        /// 获取账务列表
        /// </summary>
        /// <param name="transIds">要转账的账务明细id，多项之间以逗号分隔</param>
        /// <returns></returns>
        private List<Guid> GetTransIds(string transIds)
        {
            //获取账务
            List<Guid> transIdList = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(transIds) && transIds.Contains(","))
            {
                var temp = transIds.Split(',').ToList();
                foreach (var item in temp)
                {
                    Guid id = Guid.Empty;
                    if (Guid.TryParse(item, out id))
                    {
                        if (id != null && id != Guid.Empty)
                        {
                            transIdList.Add(id);
                        }
                    }
                }
            }
            else
            {
                Guid id = Guid.Empty;
                if (Guid.TryParse(transIds, out id))
                {
                    if (id != null && id != Guid.Empty)
                    {
                        transIdList.Add(id);
                    }
                }
            }
            return transIdList;
        }
        /// <summary>
        /// 账务转账记录
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="transIdList">要转账的账务明细id列表</param>
        /// <param name="originRegIdList">源客账id列表</param>
        /// <param name="targetRegId">要转到的目标客账id</param>
        /// <param name="username">转账操作员</param>
        /// <param name="shiftId">当前班次id</param>
        /// <returns></returns>
        private void TransferAddResFolioLog(ICurrentInfo currentInfo, List<Guid> transIdList, List<ResFolio> originResFolios, string targetRegId)
        {
            string hid = currentInfo.HotelId;
            string username = currentInfo.UserName;
            string shiftId = currentInfo.ShiftId;
            var targetRegEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == targetRegId).AsNoTracking().FirstOrDefault(); if(targetRegEntity == null) { return; }
            //添加记录
            if(transIdList != null && transIdList.Count > 0)
            {
                int hidLength = hid.Length;
                string ip = UrlHelperExtension.GetRemoteClientIPAddress();
                DateTime nowDateTime = DateTime.Now;
                foreach (var transId in transIdList)
                {
                    var originFolio = originResFolios.Where(c => c.Transid == transId).FirstOrDefault();
                    if(originFolio != null)
                    {
                        _pmsContext.ResFolioLogs.Add(new ResFolioLog
                        {
                            CDate = nowDateTime,
                            CUser = username ?? "",
                            Hid = hid,
                            Id = Guid.NewGuid(),
                            Ip = ip,
                            Transid = transId,
                            XType = 0,
                            Value1 = originFolio.Regid,
                            Value2 = targetRegId,
                            Other1 = originFolio.RoomNo,
                            Other2 = targetRegEntity.RoomNo,
                            Remark = shiftId,
                            Describle = originFolio.RegidFrom,
                        });
                    }
                }
                //转账日志
                string textFormat = "转账 账号：{0}，房号：{1}，消费：{2}，付款：{3}转到目标账号：{4}，目标房号：{5}，班次：{6}";
                var regids = originResFolios.Select(c => c.Regid).Distinct().ToList();
                foreach(var regid in regids)
                {
                    string roomNo = originResFolios.Where(c => c.Regid == regid).Select(c => c.RoomNo).FirstOrDefault();
                    var dAmount = originResFolios.Where(c => c.Regid == regid && c.Dcflag == "D").Sum(c => c.Amount);
                    var cAmount = originResFolios.Where(c => c.Regid == regid && c.Dcflag == "C").Sum(c => c.Amount);

                    _pmsContext.OpLogs.Add(new OpLog
                    {
                        CDate = nowDateTime,
                        CText = string.Format(textFormat, regid.Substring(hidLength), roomNo, dAmount, cAmount, targetRegEntity.Regid.Substring(hidLength), targetRegEntity.RoomNo, currentInfo.ShiftName),
                        CUser = username ?? "",
                        Hid = hid,
                        Ip = ip,
                        Keys = regid,
                        XType = "转账",
                    });
                }

                _pmsContext.SaveChanges();
            }
        }
        /// <summary>
        /// 根据账务ID获取账务信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <returns></returns>
        public JsonResultData GetFolioByTransid(string hid, Guid transid)
        {
            var entity = _pmsContext.ResFolios.Where(c => c.Hid == hid && c.Transid == transid).AsNoTracking().FirstOrDefault();
            if (entity != null)
            {
                string TransShiftName = null;//发生班次名称
                string SettleShiftName = null;//结账班次名称
                #region
                List<string> shiftIdList = new List<string>();
                if (!string.IsNullOrWhiteSpace(entity.TransShift))
                {
                    shiftIdList.Add(entity.TransShift);
                }
                if (!string.IsNullOrWhiteSpace(entity.SettleShift))
                {
                    shiftIdList.Add(entity.SettleShift);
                }
                if (shiftIdList.Count > 0)
                {
                    var temp = _pmsContext.Shifts.Where(c => c.Hid == hid && shiftIdList.Contains(c.Id)).Select(c => new { c.Id, c.ShiftName }).AsNoTracking().ToList();
                    TransShiftName = temp.Where(c => c.Id == entity.TransShift).Select(c => c.ShiftName).FirstOrDefault();
                    SettleShiftName = temp.Where(c => c.Id == entity.SettleShift).Select(c => c.ShiftName).FirstOrDefault();
                }
                #endregion

                string ItemName = null;
                if (!string.IsNullOrWhiteSpace(entity.Itemid))
                {
                    ItemName = _pmsContext.Items.Where(c => c.Hid == hid && c.Id == entity.Itemid).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                }

                var CancelAndRecoveryFolioLog = _pmsContext.ResFolioLogs.Where(c => c.Hid == hid && c.Transid == transid && c.XType == 1).OrderBy(c => c.CDate).AsNoTracking().ToList();

                var result = new { Entity = entity, TransShiftName = TransShiftName, SettleShiftName = SettleShiftName, ItemName = ItemName, CancelAndRecoveryFolioLog = CancelAndRecoveryFolioLog };

                return JsonResultData.Successed(result);
            }
            return JsonResultData.Failure("");
        }
        /// <summary>
        /// 根据账务ID获取账务信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="transid">账务ID</param>
        /// <returns></returns>
        public ResFolio GetEntity(string hid, Guid transid)
        {
            return _pmsContext.ResFolios.Where(c => c.Hid == hid && c.Transid == transid).AsNoTracking().FirstOrDefault();
        }
        #endregion

        #region 账务日志
        /// <summary>
        /// 添加账务日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userName">操作员</param>
        /// <param name="transId">账务ID</param>
        /// <param name="type">类型（0转账，1账务作废与恢复,2水电费记录）</param>
        /// <param name="value1">更换前</param>
        /// <param name="value2">更换后</param>
        /// <param name="other1">相关属性1</param>
        /// <param name="other2">相关属性2</param>
        /// <param name="remark">备注</param>
        /// <param name="describle">描述</param>
        public void AddResFolioLog(string hid, string userName, Guid transId, byte type, string value1, string value2, string other1 = null, string other2 = null, string remark = null, string describle = null)
        {
            try
            {
                _pmsContext.ResFolioLogs.Add(new ResFolioLog
                {
                    CDate = DateTime.Now,
                    CUser = userName ?? "",
                    Hid = hid,
                    Id = Guid.NewGuid(),
                    Ip = UrlHelperExtension.GetRemoteClientIPAddress(),
                    Transid = transId,
                    XType = type,
                    Value1 = value1,
                    Value2 = value2,
                    Remark = remark,
                    Other1 = other1,
                    Other2 = other2,
                    Describle = describle,
                });
                _pmsContext.SaveChanges();
            }
            catch { }
        }
        #endregion

        #region 更新订单明细表汇总金额
        /// <summary>
        /// 更新订单明细表汇总金额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">账号ID，多个直接用逗号隔开</param>
        private void uPResDetailUpdateAmount(string hid, string regids)
        {
            try
            {
                _pmsContext.Database.ExecuteSqlCommand("exec up_resDetail_updateAmount @hid=@hid,@regids=@regids;"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@regids", regids));
            }
            catch{ }
        }
        #endregion


        #region 长包房功能
        private class InAdvanceCheckout_GetEndDateEntity
        {
            public string regid { get; set; }
            public string shortRegid { get; set; }
            public string endDate { get; set; }
        }
        /// <summary>
        /// 获取预结日期
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regIds">账号ID列表</param>
        /// <returns></returns>
        public JsonResultData InAdvanceCheckout_GetEndDate(ICurrentInfo currentInfo, string regIds)
        {
            try
            {
                string hid = currentInfo.HotelId;
                string userName = currentInfo.UserName;

                var list = _pmsContext.Database.SqlQuery<InAdvanceCheckout_GetEndDateEntity>("exec up_getEndDate_Permanent @hid=@hid,@regids=@regIds"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@regIds", regIds)
                    ).ToList();

                return JsonResultData.Successed(list);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 预结
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regIds">账号ID列表</param>
        /// <param name="endDate">预结日期</param>
        /// <returns></returns>
        public JsonResultData InAdvanceCheckout(ICurrentInfo currentInfo, string regIds, DateTime endDate)
        {
            try
            {
                string hid = currentInfo.HotelId;
                string userName = currentInfo.UserName;

                _pmsContext.Database.ExecuteSqlCommand("exec up_audit_genRoomCharge_Permanent @hid=@hid,@操作员名字=@userName,@regids=@regIds,@endBsnsdate=@endBsnsdate"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@userName", userName)
                    , new SqlParameter("@regIds", regIds)
                    , new SqlParameter("@endBsnsdate", endDate)
                    );

                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 水电费 本次抄表数 入账 检查
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="list">参数</param>
        /// <returns></returns>
        public JsonResultData WaterAndElectricity_AddFolioDebit_Check(ICurrentInfo currentInfo, List<PermanentRoomFolioPara.WaterAndElectricityFolioPara> list)
        {
            string hid = currentInfo.HotelId;
            //Regid,Action,ThisTimeMeterReading 这三个是参数
            if (list == null || list.Count <= 0)
            {
                return JsonResultData.Failure("参数错误！");
            }
            bool isCheck = true;
            foreach (var item in list)
            {
                if (string.IsNullOrWhiteSpace(item.Regid))
                {
                    isCheck = false;
                    break;
                }
                if (item.Action != "51" && item.Action != "52" && item.Action != "53")
                {
                    isCheck = false;
                    break;
                }
                if (item.ThisTimeMeterReading <= 0)
                {
                    isCheck = false;
                    break;
                }
                item.AmountD = null;
                item.InvNo = null;
                item.ItemId = null;
                item.ItemName = null;
                item.LastTimeMeterReading = 0;
                item.Price = null;
                item.Quantity = null;
                item.Remark = null;
                item.RoomNo = null;
                item.ShortRegid = null;
            }
            if (isCheck == false)
            {
                return JsonResultData.Failure("参数错误！");
            }

            var regids = list.Select(c => c.Regid).Distinct().ToList();
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.Status == "I").AsNoTracking().ToList();
            if (resDetailList.Count != regids.Count)
            {
                return JsonResultData.Failure("参数错误！");
            }

            var itemList = _pmsContext.Items.Where(c => c.Hid == hid && c.DcFlag == "D" && (c.Action == "51" || c.Action == "52" || c.Action == "53") && c.Status == EntityStatus.启用).AsNoTracking().ToList();
            if(itemList == null || itemList.Count <= 0)
            {
                return JsonResultData.Failure("参数错误！");
            }

            var itemids = itemList.Select(c => c.Id).Distinct().ToList();
            var folioList = _pmsContext.ResFolios.Where(c => c.Hid == hid && regids.Contains(c.Regid) && itemids.Contains(c.Itemid) && c.Dcflag == "D" && (c.Status == 1 || c.Status == 2)).AsNoTracking().Select(c => new { c.Transid, c.Itemid }).ToList();
            var transids = folioList.Select(c => c.Transid).Distinct().ToList();
            var lastTimeMeterReading = _pmsContext.ResFolioLogs.Where(c => c.Hid == hid && c.XType == 2 && transids.Contains(c.Transid) && regids.Contains(c.Other1)).AsNoTracking().ToList();

            var rermanentRoomSets = _pmsContext.PermanentRoomSets.Where(c => regids.Contains(c.Regid) && c.Hid == hid).AsNoTracking().ToList();

            foreach (var item in list)
            {
                string actionName = "";
                if (item.Action == "51")
                {
                    actionName = "水费";
                }
                else if (item.Action == "52")
                {
                    actionName = "电费";
                }
                else if (item.Action == "53")
                {
                    actionName = "燃气费";
                }
                var itemEntity = itemList.Where(c => c.Action == item.Action).FirstOrDefault();
                if(itemEntity == null)
                {
                    return JsonResultData.Failure("没有找到["+ actionName + "]此处理方式的消费项目！");
                }
                var orderEntity = resDetailList.Where(c => c.Regid == item.Regid).FirstOrDefault();
                if(orderEntity == null)
                {
                    return JsonResultData.Failure("账号：" + item.Regid.Substring(hid.Length) + "不是在住订单！");
                }
                int meterReading = -2;
                var actionTransids = folioList.Where(c => c.Itemid == itemEntity.Id).Select(c => c.Transid).Distinct().ToList();
                string value = lastTimeMeterReading.Where(c => c.Hid == hid && c.XType == 2 && c.Other1 == orderEntity.Regid && c.Other2 == orderEntity.RoomNo && actionTransids.Contains(c.Transid) && !string.IsNullOrWhiteSpace(c.Value2)).OrderByDescending(c => c.CDate).Select(c => c.Value2).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Int32.TryParse(value, out meterReading);
                }
                if (meterReading <= 0)
                {
                    var setEntity = rermanentRoomSets.Where(c => c.Regid == orderEntity.Regid).FirstOrDefault();
                    if (setEntity != null)
                    {
                        if (item.Action == "51")
                        {
                            meterReading = setEntity.WaterMeter;
                        }
                        else if (item.Action == "52")
                        {
                            meterReading = setEntity.WattMeter;
                        }
                        else if (item.Action == "53")
                        {
                            meterReading = setEntity.NaturalGas;
                        }
                    }
                    else
                    {
                        return JsonResultData.Failure(string.Format("房号：{0}，不是长租房！", orderEntity.RoomNo));
                    }
                }

                if((item.ThisTimeMeterReading - meterReading) <= 0)
                {
                    return JsonResultData.Failure("账号：" + item.Regid.Substring(hid.Length) + "，"+ actionName + "-上次读表数："+ meterReading + "，本次读表数必须大于上次读表数！");
                }
                if(itemEntity.Price == null || itemEntity.Price.HasValue == false || itemEntity.Price.Value <= 0)
                {
                    return JsonResultData.Failure(actionName + "，请设置在消费项目中设置单价！");
                }

                //Regid,Action,ThisTimeMeterReading
                if (itemEntity != null)
                {
                    item.ShortRegid = orderEntity.Regid.Substring(0, orderEntity.Hid.Length);
                    item.RoomNo = orderEntity.RoomNo;
                    
                    
                    item.ItemId = itemEntity.Id;
                    item.ItemName = itemEntity.Name;
                    item.LastTimeMeterReading = meterReading;
                    item.Price = itemEntity.Price;
                    item.Quantity = Convert.ToInt32(item.ThisTimeMeterReading - meterReading);
                    item.AmountD = item.Price * item.Quantity;

                    item.InvNo = null;
                    item.Remark = null;
                }
            }
            return JsonResultData.Successed();

        }
        #endregion

        #region 拆账
        /// <summary>
        /// 拆账
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="para">参数类</param>
        /// <returns></returns>
        public JsonResultData SplitFolio(ICurrentInfo currentInfo, ResFolioSplitInfo.Para para)
        {
            string hid = currentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("酒店ID错误！");
            }

            #region 验证
            if (para == null)
            {
                return JsonResultData.Failure("参数不正确！");
            }
            if(para.SplitCount != 2 && para.SplitCount != 3 && para.SplitCount != 4)
            {
                return JsonResultData.Failure("拆账份数不正确！");
            }
            //Bill
            if (para.Bill == null)
            {
                return JsonResultData.Failure("所属账单不能为空！");
            }
            char A = char.Parse("A");
            char Z = char.Parse("Z");
            if (!char.IsWhiteSpace(para.Bill.Amount1))
            {
                if (para.Bill.Amount1 < A && para.Bill.Amount1 > Z)
                {
                    return JsonResultData.Failure("金额1所属账单错误！");
                }
            }
            if (!char.IsWhiteSpace(para.Bill.Amount2))
            {
                if (para.Bill.Amount2 < A && para.Bill.Amount2 > Z)
                {
                    return JsonResultData.Failure("金额2所属账单错误！");
                }
            }
            if (!char.IsWhiteSpace(para.Bill.Amount3))
            {
                if (para.Bill.Amount3 < A && para.Bill.Amount3 > Z)
                {
                    return JsonResultData.Failure("金额3所属账单错误！");
                }
            }
            if (!char.IsWhiteSpace(para.Bill.Amount4))
            {
                if (para.Bill.Amount4 < A && para.Bill.Amount4 > Z)
                {
                    return JsonResultData.Failure("金额4所属账单错误！");
                }
            }
            //FolioList
            if (para.FolioList == null || para.FolioList.Count <= 0)
            {
                return JsonResultData.Failure("请勾选账务！");
            }
            foreach(var item in para.FolioList)
            {
                if(item.TransId == Guid.Empty)
                {
                    return JsonResultData.Failure("账务错误，请联系管理员！");
                }
                if(item.Amount1 == 0)
                {
                    return JsonResultData.Failure("请输入金额1！");
                }
                if (item.Amount2 == 0)
                {
                    return JsonResultData.Failure("请输入金额2！");
                }
                if(para.SplitCount >= 3)
                {
                    if (item.Amount3 == 0)
                    {
                        return JsonResultData.Failure("请输入金额3！");
                    }
                }
                else
                {
                    if (item.Amount3 != 0)
                    {
                        return JsonResultData.Failure("金额3错误！");
                    }
                }
                if(para.SplitCount >= 4)
                {
                    if (item.Amount4 == 0)
                    {
                        return JsonResultData.Failure("请输入金额4！");
                    }
                }
                else
                {
                    if (item.Amount4 != 0)
                    {
                        return JsonResultData.Failure("金额4错误！");
                    }
                }
            }
            #endregion

            var tranids = para.FolioList.Select(c => c.TransId).ToList();
            if(tranids.Count != tranids.Distinct().ToList().Count)
            {
                return JsonResultData.Failure("账务不能重复！");
            }

            var folioList = _pmsContext.ResFolios.Where(c => tranids.Contains(c.Transid) && c.Hid == hid && c.Status == 1).ToList();
            if(folioList.Count != tranids.Count)
            {
                return JsonResultData.Failure("账务已被修改，请获取账务最新内容！");
            }

            //执行
            foreach (var folioEntity in folioList)
            {
                if(folioEntity.Amount == null || folioEntity.Amount.HasValue == false || folioEntity.Amount == 0)
                {
                    return JsonResultData.Failure("账务金额不能为0！");
                }

                var splitEntity = para.FolioList.FirstOrDefault(c => c.TransId == folioEntity.Transid);
                if(splitEntity == null)
                {
                    return JsonResultData.Failure("账务异常，请联系管理员！");
                }

                var sumAmount = (splitEntity.Amount1 + splitEntity.Amount2 + splitEntity.Amount3 + splitEntity.Amount4);
                if(sumAmount != folioEntity.Amount)
                {
                    return JsonResultData.Failure("拆账金额汇总不正确！");
                }

                decimal percent = 0;//比例
                if (folioEntity.Dcflag.ToUpper() == "D")
                {
                    if(folioEntity.TaxAmount != null && folioEntity.TaxAmount.HasValue && folioEntity.TaxAmount.Value != 0)
                    {
                        percent = folioEntity.TaxAmount.Value / folioEntity.Amount.Value;
                    }
                }
                else if (folioEntity.Dcflag.ToUpper() == "C")
                {
                    if(folioEntity.OriAmount != null && folioEntity.OriAmount.HasValue && folioEntity.OriAmount.Value != 0)
                    {
                        percent = folioEntity.OriAmount.Value / folioEntity.Amount.Value;
                    }
                }
                else
                {
                    return JsonResultData.Failure("账务类型异常，请联系管理员！");
                }

                decimal addSumAmount = 0;
                for (var i = 2; i <= para.SplitCount; i++)
                {
                    #region 增加新账务
                    var newFolio = new ResFolio();
                    AutoSetValueHelper.SetValues(folioEntity, newFolio);
                    newFolio.Transid = Guid.NewGuid();
                    newFolio.Quantity = 0;//新记录 数量为0，为了不影响统计
                    newFolio.Nights = 0;//新记录 剑也是为0，为了不影响统计
                    if (i == 2)
                    {
                        newFolio.Amount = splitEntity.Amount2;
                        if (!char.IsWhiteSpace(para.Bill.Amount2))
                        {
                            newFolio.resBillCode = para.Bill.Amount2.ToString();
                        }
                    }
                    else if(i == 3)
                    {
                        newFolio.Amount = splitEntity.Amount3;
                        if (!char.IsWhiteSpace(para.Bill.Amount3))
                        {
                            newFolio.resBillCode = para.Bill.Amount3.ToString();
                        }
                    }
                    else if (i == 4)
                    {
                        newFolio.Amount = splitEntity.Amount4;
                        if (!char.IsWhiteSpace(para.Bill.Amount4))
                        {
                            newFolio.resBillCode = para.Bill.Amount4.ToString();
                        }
                    }
                    //根据消费、付款，计算含税金额和原币金额
                    #region
                    var tempAmount = newFolio.Amount.Value * percent;
                    tempAmount = decimal.Parse(Extension.ToString(tempAmount, 2));
                    addSumAmount += tempAmount;

                    if (newFolio.Dcflag.ToUpper() == "D")
                    {
                        newFolio.TaxAmount = tempAmount;
                        newFolio.OriAmount = newFolio.Amount.Value;
                    }
                    else if (newFolio.Dcflag.ToUpper() == "C")
                    {
                        newFolio.TaxAmount = 0;
                        newFolio.OriAmount = tempAmount;
                    }
                    else
                    {
                        return JsonResultData.Failure("账务类型异常，请联系管理员！");
                    }
                    #endregion
                    _pmsContext.ResFolios.Add(newFolio);
                    #endregion
                }

                #region 修改源账务
                //源账务=金额1
                folioEntity.Amount = splitEntity.Amount1;
                if (!char.IsWhiteSpace(para.Bill.Amount1))
                {
                    folioEntity.resBillCode = para.Bill.Amount1.ToString();
                }
                //根据消费、付款，计算含税金额和原币金额
                #region
                if (folioEntity.Dcflag.ToUpper() == "D")
                {
                    folioEntity.TaxAmount = folioEntity.TaxAmount - addSumAmount;
                    folioEntity.OriAmount = folioEntity.Amount;
                }
                else if (folioEntity.Dcflag.ToUpper() == "C")
                {
                    folioEntity.OriAmount = folioEntity.OriAmount - addSumAmount;
                    folioEntity.TaxAmount = 0;
                }
                else
                {
                    return JsonResultData.Failure("账务类型异常，请联系管理员！");
                }
                #endregion
                _pmsContext.Entry(folioEntity).State = EntityState.Modified;
                #endregion

            }
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }
        #endregion

        #region 迟付账务处理权限
        /// <summary>
        /// 订单是否迟付状态
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">参数类型（1：regId 或 regIds，2：transId 或 transIds）</param>
        /// <param name="value">参数值</param>
        public bool IsOutStatusOrder(string hid, int type, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return false; }

            List<string> regIds = new List<string>();

            if(type == 1)
            {
                regIds = value.Split(',').Distinct().ToList();
            }
            else if(type == 2)
            {
                List<Guid> transIds = new List<Guid>();
                var transids = value.Split(',').ToList();
                foreach (var transid in transids)
                {
                    var temp = Gemstar.BSPMS.Common.Extensions.Extension.ParseGuid(transid);
                    if (temp != null && temp != Guid.Empty && !transIds.Contains(temp))
                    {
                        transIds.Add(temp);
                    }
                }
                if (transIds != null && transIds.Count > 0)
                {
                    regIds = _pmsContext.ResFolios.AsNoTracking().Where(c => c.Hid == hid && transIds.Contains(c.Transid)).Select(c => c.Regid).Distinct().ToList();
                }
            }

            if(regIds != null && regIds.Count > 0)
            {
                return _pmsContext.ResDetails.AsNoTracking().Where(c => c.Hid == hid && regIds.Contains(c.Regid) && c.Status == "O").Any();
            }
            return false;
        }
        #endregion

        #region 退款
        /// <summary>
        /// 获取可用退款的账务
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">子单ID列表</param>
        /// <returns></returns>
        public List<ResFolioRefundFolioInfo> GetRefundFolios(string hid, string regids)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                throw new ApplicationException("酒店id为空");
            }
            if (string.IsNullOrWhiteSpace(regids))
            {
                throw new ApplicationException("请勾选订单");
            }
            return _pmsContext.Database.SqlQuery<ResFolioRefundFolioInfo>("exec up_list_refundFolio @h99hid=@h99hid,@regids=@regids"
                , new SqlParameter("@h99hid", SqlDbType.VarChar) { Value = hid }
                , new SqlParameter("@regids", SqlDbType.VarChar) { Value = regids }
                ).ToList();
        }
        #endregion

        private DbHotelPmsContext _pmsContext;
    }
}
