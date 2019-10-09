using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.EF.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 支付成功后的回调，更新相应的业务记录状态
    /// </summary>
    public static class PayCallbackUpdate
    {
        #region 接收到支付后的回调通知时，调用此方法来更新相应的产品状态

        /// <summary>
        /// 更新支付完成后的业务记录状态
        /// </summary>
        /// <param name="pmsContext">业务数据库实例</param>
        /// <param name="para">支付相关参数</param>
        public static void UpdateProductStatu(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            switch (para.ProductType)
            {
                case PayProductType.ResFolio:
                    UpdateResFolio(pmsContext, para);
                    break;

                case PayProductType.MbrRecharge:
                    UpdateMbrRecharge(pmsContext, para);
                    break;

                case PayProductType.CorpReceive:
                    UpdateCompanyCa(pmsContext, para);
                    break;

                case PayProductType.MbrCardFee:
                    UpdateMbrCardFee(pmsContext, para);
                    break;

                case PayProductType.PosPayment:
                    UpdatePosBill(pmsContext, para);
                    break;

                default:
                    throw new ArgumentException("不支持的产品类型" + para.ProductType.ToString());
                    break;
            }
        }

        #endregion 接收到支付后的回调通知时，调用此方法来更新相应的产品状态

        #region 更新客账支付状态

        /// <summary>
        /// 更新客账支付状态
        /// </summary>
        /// <param name="pmsContext">业务数据库实例</param>
        /// <param name="para">支付相关参数</param>
        private static void UpdateResFolio(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            para.OutTradeNo = Guid.Parse(para.OutTradeNo).ToString();
            if (para.IsPaidSuccess)
            {
                //更新对应账务明细状态
                ResFolioService.UpdateFolioAfterPaySuccess(pmsContext, para.HotelId, para.OutTradeNo, para.PaidTransId);
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payTransId = @paidTransId,payDate = GETDATE(),payAmount = @paidAmount,status = 1 WHERE ProductType = 'ResFolio' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@paidTransId", para.PaidTransId ?? "")
                    , new SqlParameter("@paidAmount", para.PaidAmount.Value)
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
            else
            {
                //更新对应账务明细状态
                ResFolioService.UpdateFolioAfterPayFailure(pmsContext, para.HotelId, para.OutTradeNo);
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payError = @payError,status = 2 WHERE ProductType = 'ResFolio' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@payError", para.PaidError ?? "")
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
        }

        #endregion 更新客账支付状态

        #region 更新会员充值支付状态

        /// <summary>
        /// 更新会员充值支付状态
        /// </summary>
        /// <param name="pmsContext">业务数据库实例</param>
        /// <param name="para">支付相关参数</param>
        private static void UpdateMbrRecharge(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            para.OutTradeNo = Guid.Parse(para.OutTradeNo).ToString();
            if (para.IsPaidSuccess)
            {
                //更新对应账务明细状态
                var businessPara = pmsContext.Database.SqlQuery<string>("SELECT BusinessPara FROM WaitPayList WHERE WaitPayId = @waitPayId AND ProductType='MbrRecharge' AND Status = 0"
                    , new SqlParameter("@waitPayId", para.OutTradeNo)
                    ).SingleOrDefault();
                if (!string.IsNullOrWhiteSpace(businessPara))
                {
                    pmsContext.Database.ExecuteSqlCommand("update WaitPayList SET PayDate = GETDATE(),Status = 1 WHERE WaitPayId = @waitPayId AND ProductType='MbrRecharge' AND Status = 0"
                        , new SqlParameter("@waitPayId", para.OutTradeNo));
                    var serializer = new JavaScriptSerializer();
                    var paraInfo = serializer.DeserializeObject(businessPara) as Dictionary<string, object>;
                    var invNo = paraInfo["invno"];
                    var remark = paraInfo["remark"];
                    var originPayAmount = paraInfo["originFloatAmount"];
                    var shiftId = paraInfo.ContainsKey("shiftId") ? paraInfo["shiftId"].ToString() : "";
                    var profilecaInfo = pmsContext.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=@freeAmount,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@originFloatAmount=@originFloatAmount,@shiftId=@shiftId",
                    new SqlParameter("@hid", para.HotelId),
                    new SqlParameter("@profileId", paraInfo["profileId"].ToString()),
                    new SqlParameter("@balanceType", "01"),
                    new SqlParameter("@type", "01"),
                    new SqlParameter("@outLetCode", "01"),
                    new SqlParameter("@floatAmount", paraInfo["floatAmount"].ToString()),
                    new SqlParameter("@freeAmount", paraInfo["freeAmount"].ToString()),
                    new SqlParameter("@invno", (invNo == null ? "" : invNo.ToString())),
                    new SqlParameter("@inputUser", paraInfo["inputUser"].ToString()),
                    new SqlParameter("@remark", (remark == null ? "" : remark)),
                    new SqlParameter("@refno", para.OutTradeNo),
                    new SqlParameter("@itemid", paraInfo["itemid"].ToString()),
                    (originPayAmount != null ? new SqlParameter("@originFloatAmount", originPayAmount.ToString()) : new SqlParameter("@originFloatAmount", DBNull.Value)),
                    new SqlParameter("@shiftId", shiftId)
                    ).FirstOrDefault();
                    if (profilecaInfo != null && profilecaInfo.Success)
                    {
                        var smsSendService = new SMSSendService(pmsContext);
                        var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                        var smsCenterParas = _sysParaService.GetSMSSendPara();
                        var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                        smsSendService.SendMbrRecharge(para.HotelId, Guid.Parse(paraInfo["profileId"].ToString()), profilecaInfo.Data.ToString(), smsCenterParas, smsLogService);
                    }
                }
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payTransId = @paidTransId,payDate = GETDATE(),payAmount = @paidAmount,status = 1 WHERE ProductType = 'MbrRecharge' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@paidTransId", para.PaidTransId ?? "")
                    , new SqlParameter("@paidAmount", para.PaidAmount.Value)
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
            else
            {
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payError = @payError,status = 2 WHERE ProductType = 'MbrRecharge' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@payError", para.PaidError ?? "")
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
        }

        #endregion 更新会员充值支付状态

        #region 更新会员卡费支付状态

        /// <summary>
        /// 更新会员充值支付状态
        /// </summary>
        /// <param name="pmsContext">业务数据库实例</param>
        /// <param name="para">支付相关参数</param>
        private static void UpdateMbrCardFee(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            para.OutTradeNo = Guid.Parse(para.OutTradeNo).ToString();
            if (para.IsPaidSuccess)
            {
                //更新对应账务明细状态
                var businessPara = pmsContext.Database.SqlQuery<string>("SELECT BusinessPara FROM WaitPayList WHERE WaitPayId = @waitPayId AND ProductType='MbrRecharge' AND Status = 0"
                    , new SqlParameter("@waitPayId", para.OutTradeNo)
                    ).SingleOrDefault();
                if (!string.IsNullOrWhiteSpace(businessPara))
                {
                    pmsContext.Database.ExecuteSqlCommand("update WaitPayList SET PayDate = GETDATE(),Status = 1 WHERE WaitPayId = @waitPayId AND ProductType='MbrCardFee' AND Status = 0"
                        , new SqlParameter("@waitPayId", para.OutTradeNo));
                    var serializer = new JavaScriptSerializer();
                    var paraInfo = serializer.DeserializeObject(businessPara) as Dictionary<string, object>;
                    var invNo = paraInfo["invno"];
                    var remark = paraInfo["remark"];
                    var originPayAmount = paraInfo["originFloatAmount"];
                    var shiftId = paraInfo.ContainsKey("shiftId") ? paraInfo["shiftId"].ToString() : "";
                    var profilecaInfo = pmsContext.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=@freeAmount,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@originFloatAmount=@originFloatAmount,@shiftId=@shiftId",
                    new SqlParameter("@hid", para.HotelId),
                    new SqlParameter("@profileId", paraInfo["profileId"].ToString()),
                    new SqlParameter("@balanceType", ProfileAccountType.AccountCardFee.Code),
                    new SqlParameter("@type", ProfileCaType.CardFee.Code),
                    new SqlParameter("@outLetCode", "01"),
                    new SqlParameter("@floatAmount", paraInfo["floatAmount"].ToString()),
                    new SqlParameter("@freeAmount", 0),
                    new SqlParameter("@invno", (invNo == null ? "" : invNo.ToString())),
                    new SqlParameter("@inputUser", paraInfo["inputUser"].ToString()),
                    new SqlParameter("@remark", (remark == null ? "" : remark)),
                    new SqlParameter("@refno", para.OutTradeNo),
                    new SqlParameter("@itemid", paraInfo["itemid"].ToString()),
                    (originPayAmount != null ? new SqlParameter("@originFloatAmount", originPayAmount.ToString()) : new SqlParameter("@originFloatAmount", DBNull.Value)),
                     new SqlParameter("@shiftId", shiftId)
                    ).FirstOrDefault();
                    if (profilecaInfo != null && profilecaInfo.Success)
                    {
                        //卡费收取成功后，更改会员的状态为启用
                        pmsContext.Database.ExecuteSqlCommand("update profile set status = @normal where id=@profileId and status = @disable"
                            , new SqlParameter("@normal", (byte)MbrCardStatus.Nomal)
                            , new SqlParameter("@profileId", paraInfo["profileId"].ToString())
                            , new SqlParameter("@disable", (byte)MbrCardStatus.Disabled)
                            );
                    }
                }
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payTransId = @paidTransId,payDate = GETDATE(),payAmount = @paidAmount,status = 1 WHERE ProductType = 'MbrCardFee' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@paidTransId", para.PaidTransId ?? "")
                    , new SqlParameter("@paidAmount", para.PaidAmount.Value)
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
            else
            {
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payError = @payError,status = 2 WHERE ProductType = 'MbrCardFee' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@payError", para.PaidError ?? "")
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
        }

        #endregion 更新会员卡费支付状态

        #region 更新合约单位收款支付状态

        private static void UpdateCompanyCa(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            para.OutTradeNo = Guid.Parse(para.OutTradeNo).ToString();
            if (para.IsPaidSuccess)
            {
                //更新对应账务明细状态
                var businessPara = pmsContext.Database.SqlQuery<string>("SELECT BusinessPara FROM WaitPayList WHERE WaitPayId = @waitPayId AND ProductType='CorpReceive' AND Status = 0"
                    , new SqlParameter("@waitPayId", para.OutTradeNo)
                    ).SingleOrDefault();
                if (!string.IsNullOrWhiteSpace(businessPara))
                {
                    pmsContext.Database.ExecuteSqlCommand("update WaitPayList SET PayDate = GETDATE(),Status = 1 WHERE WaitPayId = @waitPayId AND ProductType='CorpReceive' AND Status = 0"
                        , new SqlParameter("@waitPayId", para.OutTradeNo));
                    var serializer = new JavaScriptSerializer();
                    var paraInfo = serializer.DeserializeObject(businessPara) as Dictionary<string, object>;

                    string hotelId = Convert.ToString(paraInfo["hotelId"]);
                    string userName = Convert.ToString(paraInfo["userName"]);
                    Guid? companyid = paraInfo["companyid"] as Guid?;
                    string type = Convert.ToString(paraInfo["type"]);
                    decimal amount = Convert.ToDecimal(paraInfo["amount"]);
                    string itemid = Convert.ToString(paraInfo["itemid"]);
                    string invno = Convert.ToString(paraInfo["invno"]);
                    string remark = Convert.ToString(paraInfo["remark"]);
                    string outletcode = Convert.ToString(paraInfo["outletcode"]);

                    pmsContext.Database.SqlQuery<JsonResultData>(@"
                        exec up_companyca_input
                        @hid=@hid,
                        @inputUser=@inputUser,
                        @companyid=@companyid,
                        @outletcode=@outletcode,
                        @type=@type,
                        @amount=@amount,
                        @itemid=@itemid,
                        @invno=@invno,
                        @remark=@remark,
                        @refno=@refno",
                        new SqlParameter("@hid", hotelId),
                        new SqlParameter("@inputUser", userName),
                        new SqlParameter("@companyid", companyid),
                        new SqlParameter("@outletcode", string.IsNullOrWhiteSpace(outletcode) ? "01" : outletcode),
                        new SqlParameter("@type", type),
                        new SqlParameter("@amount", amount),
                        new SqlParameter("@itemid", itemid),
                        new SqlParameter("@invno", invno),
                        new SqlParameter("@remark", remark),
                        new SqlParameter("@refno", para.OutTradeNo)
                        );
                }
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payTransId = @paidTransId,payDate = GETDATE(),payAmount = @paidAmount,status = 1 WHERE ProductType = 'CorpReceive' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@paidTransId", para.PaidTransId ?? "")
                    , new SqlParameter("@paidAmount", para.PaidAmount.Value)
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
            else
            {
                //更新中间表状态
                pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payError = @payError,status = 2 WHERE ProductType = 'CorpReceive' and ProductTransId = @folioId AND status = 0"
                    , new SqlParameter("@payError", para.PaidError ?? "")
                    , new SqlParameter("@folioId", para.OutTradeNo)
                    );
            }
        }

        #endregion 更新合约单位收款支付状态

        #region 更新Pos账单付款状态

        /// <summary>
        /// 更新Pos账单付款状态
        /// </summary>
        /// <param name="pmsContext"></param>
        /// <param name="para"></param>
        private static void UpdatePosBill(DbHotelPmsContext pmsContext, PayCallbackPara para)
        {
            para.OutTradeNo = Guid.Parse(para.OutTradeNo).ToString();
            if (para.IsPaidSuccess)
            {
                //更新对应账务明细状态
                var businessPara = pmsContext.Database.SqlQuery<string>("SELECT BusinessPara FROM WaitPayList WHERE WaitPayId = @waitPayId AND ProductType=@productType AND Status = 0"
                        , new SqlParameter("@waitPayId", para.OutTradeNo), new SqlParameter("@productType", PayProductType.PosPayment.ToString())
                    ).SingleOrDefault();
                if (!string.IsNullOrWhiteSpace(businessPara))
                {
                    var serializer = new JavaScriptSerializer();
                    var paraInfo = serializer.DeserializeObject(businessPara) as Dictionary<string, object>;

                    var addBillResult = paraInfo["AddedBillResult"] as Dictionary<string, object> ;// serializer.DeserializeObject(paraInfo["AddedBillResult"].ToString()) as Dictionary<string, object>;


                    //如果是guid 那么是添加定金支付
                    if ( IsGuidByReg(addBillResult["DetailId"].ToString()))
                    {
                        //var id = Guid.Parse(addBillResult["DetailId"].ToString());
                        //var ytModel = pmsContext.YtPrepays.Where(w => w.Id == id).FirstOrDefault();
                        //ytModel.IPrepay = (byte)PrePayStatus.交押金;
                        pmsContext.Database.ExecuteSqlCommand(" update WaitPayList SET PayDate = GETDATE(),[Status] = @waitStatus  WHERE WaitPayId = @waitPayId AND ProductType = @productType AND[Status] = 0",
                            new SqlParameter("@waitPayId", para.OutTradeNo ?? ""),
                            new SqlParameter("@waitStatus", Convert.ToByte(paraInfo["WaitStatus"] == null ? "-1" : paraInfo["WaitStatus"])),
                            new SqlParameter("@productType", PayProductType.PosPayment.ToString())
                          );
                       

                        pmsContext.Database.ExecuteSqlCommand("Update ytPrepay SET IPrepay=0 where @hid=@hid AND @id=@id",
                            new SqlParameter("@hid", paraInfo["Hid"] == null ? "" : paraInfo["Hid"].ToString()),
                            new SqlParameter("@id", addBillResult["DetailId"] == null ? "" : addBillResult["DetailId"].ToString())
                            );
                    }
                    else
                    {
                        var profilecaInfo = pmsContext.Database.ExecuteSqlCommand("exec up_Pos_SetBillStatus @hid=@hid, @refeid=@refeid, @tabid=@tabid, @billid=@billid, @waitPayId=@waitPayId, @productType=@productType, @isCheck=@isCheck, @settleid=@settleid, @settlePayId=@settlePayId, @settleBsnsDate=@settleBsnsDate, @settleShuffleid=@settleShuffleid, @settleShiftId=@settleShiftId, @settleUser=@settleUser, @settleTransno=@settleTransno, @settleTransName=@settleTransName, @tabStatus=@tabStatus, @opTabid=@opTabid, @depBsnsDate=@depBsnsDate, @moveUser=@moveUser, @status=@status, @waitStatus=@waitStatus, @billDetailStatus=@billDetailStatus",
                                        new SqlParameter("@hid", paraInfo["Hid"] == null ? "" : paraInfo["Hid"].ToString()),
                                        new SqlParameter("@refeid", paraInfo["Refeid"] == null ? "" : paraInfo["Refeid"].ToString()),
                                        new SqlParameter("@tabid", paraInfo["Tabid"] == null ? "" : paraInfo["Tabid"].ToString()),
                                        new SqlParameter("@billid", paraInfo["Billid"] == null ? "" : paraInfo["Billid"].ToString()),
                                        new SqlParameter("@waitPayId", para.OutTradeNo ?? ""),
                                        new SqlParameter("@productType", PayProductType.PosPayment.ToString()),
                                        new SqlParameter("@isCheck", Convert.ToBoolean(paraInfo["IsCheck"] == null ? 0 : paraInfo["IsCheck"])),
                                        new SqlParameter("@settleid", paraInfo["SettleId"] == null ? "" : paraInfo["SettleId"].ToString()),
                                        new SqlParameter("@settlePayId", para.PaidTransId ?? ""),
                                        new SqlParameter("@settleBsnsDate", Convert.ToDateTime(paraInfo["SettleBsnsDate"] == null ? DateTime.Now.ToString() : paraInfo["SettleBsnsDate"])),
                                        new SqlParameter("@settleShuffleid", paraInfo["SettleShuffleid"] == null ? "" : paraInfo["SettleShuffleid"].ToString()),
                                        new SqlParameter("@settleShiftId", paraInfo["SettleShiftId"] == null ? "" : paraInfo["SettleShiftId"].ToString()),
                                        new SqlParameter("@settleUser", paraInfo["SettleUser"] == null ? "" : paraInfo["SettleUser"].ToString()),
                                        new SqlParameter("@settleTransno", paraInfo["SettleTransNo"] == null ? "" : paraInfo["SettleTransNo"].ToString()),
                                        new SqlParameter("@settleTransName", paraInfo["SettleTransName"] == null ? "" : paraInfo["SettleTransName"].ToString()),
                                        new SqlParameter("@tabStatus", Convert.ToByte(paraInfo["TabStatus"] == null ? "-1" : paraInfo["TabStatus"])),
                                        new SqlParameter("@opTabid", paraInfo["OpTabid"] == null ? "" : paraInfo["OpTabid"].ToString()),
                                        new SqlParameter("@depBsnsDate", Convert.ToDateTime(paraInfo["DepBsnsDate"] == null ? DateTime.Now.ToString() : paraInfo["DepBsnsDate"])),
                                        new SqlParameter("@moveUser", paraInfo["MoveUser"] == null ? "" : paraInfo["MoveUser"].ToString()),
                                        new SqlParameter("@status", Convert.ToByte(paraInfo["Status"] == null ? "-1" : paraInfo["Status"])),
                                        new SqlParameter("@waitStatus", Convert.ToByte(paraInfo["WaitStatus"] == null ? "-1" : paraInfo["WaitStatus"])),
                                        new SqlParameter("@billDetailStatus", Convert.ToByte(paraInfo["BillDetailStatus"] == null ? "-1" : paraInfo["BillDetailStatus"]))
                                        );
                    }


                    //更新中间表状态
                    pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payTransId = @paidTransId,payDate = GETDATE(),payAmount = @paidAmount,status = 1 WHERE ProductType = 'PosPayment' and ProductTransId = @folioId AND status = 0"
                        , new SqlParameter("@paidTransId", para.PaidTransId ?? "")
                        , new SqlParameter("@paidAmount", para.PaidAmount.Value)
                        , new SqlParameter("@folioId", para.OutTradeNo)
                        );
                }
                else
                {
                    //更新中间表状态
                    pmsContext.Database.ExecuteSqlCommand("UPDATE dbo.ResFolioPayInfo SET payError = @payError,status = 2 WHERE ProductType = 'PosPayment' and ProductTransId = @folioId AND status = 0"
                        , new SqlParameter("@payError", para.PaidError ?? "")
                        , new SqlParameter("@folioId", para.OutTradeNo)
                        );
                }
            }
        }
        /// <summary>
        /// 判断是否是guid 
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        static bool IsGuidByReg(string strSrc)
        {
            strSrc = strSrc.ToUpper();
            Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled);
            return reg.IsMatch(strSrc);
        }

        #endregion 更新Pos账单付款状态
    }
}