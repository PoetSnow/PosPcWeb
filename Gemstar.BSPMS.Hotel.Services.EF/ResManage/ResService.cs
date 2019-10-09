using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Common.Tools;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using System.Web;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System.Data;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResManage
{
    public class ResService : IResService
    {
        public ResService(DbHotelPmsContext pmsContext, INotifyService notifyService)
        {
            _pmsContext = pmsContext;
            _notifyService = notifyService;
        }
        /// <summary>
        /// 获取订单ID
        /// </summary>
        /// <param name="regid">子单ID</param>
        /// <returns></returns>
        public string GetResId(string regid)
        {
            return _pmsContext.ResDetails.Where(c => c.Regid == regid).Select(c => c.Resid).FirstOrDefault();
        }
        /// <summary>
        /// 查询满足条件的预订单
        /// </summary>
        /// <param name="queryPara">查询条件</param>
        /// <returns>满足查询条件的预订单列表</returns>
        public List<UpQueryResDetailResult> QueryResDetails(ResDetailQueryPara queryPara)
        {
            var procResults = _pmsContext.Database.SqlQuery<UpQueryResDetailResult>("exec up_queryResDetail @hotelId=@hotelId,@arrDateBegin=@arrDateBegin,@arrDateEnd=@arrDateEnd,@depDateBegin=@depDateBegin,@depDateEnd=@depDateEnd,@status=@status,@isSettle=@isSettle,@orderNo=@orderNo,@name=@name,@mobileNo=@mobileNo,@roomNo=@roomNo,@orderName=@orderName,@companyName=@companyName,@regId=@regId,@orderNameAndUserNameAndCompanyName=@orderNameAndUserNameAndCompanyName,@roomNoAndRegId=@roomNoAndRegId,@roomType=@roomType,@rateCode=@rateCode,@customerSource=@customerSource,@isGroupByResid=@isGroupByResid"
                , new SqlParameter("@hotelId", queryPara.HotelId)
                , new SqlParameter("@arrDateBegin", queryPara.ArrDateBegin ?? "")
                , new SqlParameter("@arrDateEnd", queryPara.ArrDateEnd ?? "")
                , new SqlParameter("@depDateBegin", queryPara.DepDateBegin ?? "")
                , new SqlParameter("@depDateEnd", queryPara.DepDateEnd ?? "")
                , new SqlParameter("@status", queryPara.Status ?? "")
                , new SqlParameter("@isSettle", queryPara.IsSettle == null ? DBNull.Value as object : queryPara.IsSettle)
                , new SqlParameter("@orderNo", queryPara.OrderNo ?? "")
                , new SqlParameter("@orderName", queryPara.OrderName ?? "")
                , new SqlParameter("@name", queryPara.Name ?? "")
                , new SqlParameter("@mobileNo", queryPara.MobileNo ?? "")
                , new SqlParameter("@roomNo", queryPara.RoomNo ?? "")
                , new SqlParameter("@companyName", queryPara.CompanyName ?? "")
                , new SqlParameter("@regId", queryPara.RegId ?? "")
                , new SqlParameter("@orderNameAndUserNameAndCompanyName", queryPara.OrderNameAndUserNameAndCompanyName ?? "")
                , new SqlParameter("@roomNoAndRegId", queryPara.RoomNoAndRegId ?? "")
                , new SqlParameter("@roomType", (queryPara.RoomType != null && queryPara.RoomType.Count > 0) ? string.Join(",", queryPara.RoomType) : "")
                , new SqlParameter("@rateCode", (queryPara.RateCode != null && queryPara.RateCode.Count > 0) ? string.Join(",", queryPara.RateCode) : "")
                , new SqlParameter("@customerSource", (queryPara.CustomerSource != null && queryPara.CustomerSource.Count > 0) ? string.Join(",", queryPara.CustomerSource) : "")
                , new SqlParameter("@isGroupByResid", queryPara.IsGroupByResid == true ? 1 : 0)
                ).ToList();

            return procResults;
        }
        /// <summary>
        /// 查询用于通用客账选择窗口的客账信息
        /// </summary>
        /// <param name="queryPara">查询条件</param>
        /// <returns>指定酒店中满足条件的客账信息</returns>
        public List<UpQueryResDetailsForCommonResult> QueryResDetails(ResDetailsForCommonPara queryPara)
        {
            var results = _pmsContext.Database.SqlQuery<UpQueryResDetailsForCommonResult>("exec up_queryResDetailsForCommon @hid=@hid,@isSettle=@isSettle,@status=@status,@roomNo=@roomNo,@mobileNo=@mobileNo,@guestName=@guestName,@billType=@billType,@resName=@resName,@regid=@regid,@notRegids=@notRegids"
                , new SqlParameter("@hid", queryPara.Hid ?? "")
                , new SqlParameter("@isSettle", queryPara.IsSettle.Value)
                , new SqlParameter("@status", queryPara.Status ?? "")
                , new SqlParameter("@roomNo", queryPara.RoomNo ?? "")
                , new SqlParameter("@mobileNo", queryPara.MobileNo ?? "")
                , new SqlParameter("@guestName", queryPara.GuestName ?? "")
                , new SqlParameter("@billType", queryPara.BillType)
                , new SqlParameter("@resName", queryPara.ResName ?? "")
                , new SqlParameter("@regid", queryPara.RegId ?? "")
                , new SqlParameter("@notRegids", queryPara.NotRegIds ?? "")
                ).ToList();
            foreach (var r in results)
            {
                r.StatusName = EnumExtension.GetDescription(typeof(ResDetailStatus), r.StatusName);
            }
            return results;
        }

        /// <summary>
        /// 根据订单明细的id获取对应的整个订单的详细信息，用于订单维护
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regId">订单明细的id</param>
        /// <returns>对应的整个订单的详细信息</returns>
        public ResMainInfo GetResMainInfoByRegId(ICurrentInfo currentInfo, string regId)
        {
            var hid = currentInfo.HotelId;
            var resDetail = _pmsContext.ResDetails.AsNoTracking().SingleOrDefault(w => w.Hid == hid && w.Regid == regId);
            if (resDetail == null)
            {
                return new ResMainInfo();
            }
            var allRegIds = "";
            var resMainInfo = GetResMainInfo(hid, resDetail.Resid, resDetail.Regid, null, out allRegIds);
            return resMainInfo;
        }
        public ResMainInfo GetResMainInfoByResId(ICurrentInfo currentInfo, string resId)
        {
            var allRegIds = "";
            var resMainInfo = GetResMainInfo(currentInfo.HotelId, resId, null, null, out allRegIds);
            return resMainInfo;
        }

        /// <summary>
        /// 增加或修改预订单信息
        /// </summary>
        /// <param name="resMainInfo">预订单信息实例</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns>保存成功后的预订单及所有明细信息</returns>
        public JsonResultData AddOrUpdateRes(ResMainInfo resMainInfo, ICurrentInfo currentInfo, DateTime businessDate)
        {
            try
            {
                #region 检查数据有效性
                JsonResultData validResult = Validate(resMainInfo, currentInfo, businessDate);
                if (!validResult.Success)
                {
                    return validResult;
                }
                #endregion
                string hid = currentInfo.HotelId;
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var oldResDetailAllInfo = GetResDetailAllInfo(hid, resMainInfo.ResDetailInfos[0].Regid);
                #region 增加修改主单
                if (string.IsNullOrWhiteSpace(resMainInfo.Resid))
                {
                    AddRes(resMainInfo, currentInfo);
                }
                else
                {
                    var originResMain = jsSerializer.Deserialize<Res>(resMainInfo.OriginResMainInfoJsonData);
                    UpdateRes(resMainInfo, originResMain);
                }
                #endregion
                #region 增加修改主单发票信息
                //if (resMainInfo.ResInvoiceInfo != null)
                //{
                //    if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.OriginResInvoiceInfoJsonData))
                //    {
                //        AddResInvoiceInfo(resMainInfo.ResInvoiceInfo, currentInfo, resMainInfo.Resid);
                //    }
                //    else
                //    {
                //        var originResInvoiceInfo = jsSerializer.Deserialize<Entities.ResInvoiceInfo>(resMainInfo.ResInvoiceInfo.OriginResInvoiceInfoJsonData);
                //        UpdateResInvoiceInfo(resMainInfo.ResInvoiceInfo, originResInvoiceInfo);
                //    }
                //}
                #endregion
                #region 增加修改明细
                bool isUseScoreToCheckin = false;
                bool isAddOrder = false;
                bool SelectRegIdIsNewCheckIn = false;
                string otherMessage = null;
                string oldRateCode = null; List<ResDetailPlan> oldResDetailPlans = null;
                foreach (var detailInfo in resMainInfo.ResDetailInfos)
                {
                    if (string.IsNullOrWhiteSpace(detailInfo.Regid))
                    {
                        AddResDetail(detailInfo, currentInfo, resMainInfo.Resno, businessDate);
                        if(detailInfo.Status == ResDetailStatus.I.ToString())
                        {
                            SelectRegIdIsNewCheckIn = true;
                            isUseScoreToCheckin = GetRateIsUseScore(hid, detailInfo.RateCode);
                        }
                        isAddOrder = true;
                    }
                    else
                    {
                        oldResDetailPlans = GetResDetailPlans(hid, detailInfo.Regid, out oldRateCode);
                        if (detailInfo.Status == ResDetailStatus.I.ToString())
                        {
                            if (GetRateIsUseScore(hid, detailInfo.RateCode) && detailInfo.RateCode != oldRateCode)
                            {
                                throw new Exception("在住客单不能修改价格代码为积分换房！");
                            }
                        }
                        UpdateResDetail(detailInfo, jsSerializer, currentInfo, businessDate, out otherMessage);
                    }
                }
                #endregion
                #region 积分换房提示
                if (isUseScoreToCheckin && resMainInfo.UseScoreSaveContinue != "1")
                {
                    return CheckUseScoreToCheckin(hid, new List<string> { resMainInfo.ResDetailInfos[0].Regid });
                }
                #endregion
                string authUserName = null;string reason = "";
                var checkAuthAdjustResult = CheckAuthAdjustPrice(currentInfo, new List<ResAdjustPriceInfo.AdjustPriceOrderModel> { new ResAdjustPriceInfo.AdjustPriceOrderModel { RegId = resMainInfo.ResDetailInfos[0].Regid, RateCodeId = resMainInfo.ResDetailInfos[0].RateCode, RoomTypeId = resMainInfo.ResDetailInfos[0].RoomTypeId, OriginRateCodeId = (oldResDetailAllInfo != null && oldResDetailAllInfo.ResDetailEntity != null ? oldResDetailAllInfo.ResDetailEntity.RateCode : null), OriginRoomTypeId = (oldResDetailAllInfo != null && oldResDetailAllInfo.ResDetailEntity != null ? oldResDetailAllInfo.ResDetailEntity.RoomTypeid : null), GuestName = resMainInfo.ResDetailInfos[0].Guestname, RoomNo = resMainInfo.ResDetailInfos[0].RoomNo } }, (isAddOrder ? ResAdjustPriceInfo.AdjustPriceOperationSource.OrderAdded : ResAdjustPriceInfo.AdjustPriceOperationSource.OrderModified));
                if (!checkAuthAdjustResult.Success)
                {
                    if (!CheckAuthAdjustPriceResult(currentInfo, resMainInfo.AuthorizationSaveContinue, resMainInfo.ResDetailInfos[0].Regid, out authUserName,out reason))
                    {
                        return checkAuthAdjustResult;
                    }
                }

                if(isUseScoreToCheckin && resMainInfo.UseScoreSaveContinue == "1")
                {
                    #region 积分换房
                    JsonResultData useScoreToCheckin_Result = JsonResultData.Failure("积分换房失败！");
                    try
                    {
                        using (var tc = new System.Transactions.TransactionScope())
                        {
                            _pmsContext.SaveChanges();
                            useScoreToCheckin_Result = ExecUseScoreToCheckin(currentInfo, new List<string> { resMainInfo.ResDetailInfos[0].Regid });
                            if(useScoreToCheckin_Result != null && useScoreToCheckin_Result.Success)
                            {
                                tc.Complete();
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                    }
                    catch { return useScoreToCheckin_Result; }
                    #endregion
                }
                else
                {
                    _pmsContext.SaveChanges();
                }
                if (resMainInfo.IsRelationUpdateAllRoonTypeRatePlan)
                {
                    string updateMessage = "";
                    RelationUpdateRate(currentInfo, resMainInfo.ResDetailInfos[0].Regid, businessDate, out updateMessage);
                    otherMessage += updateMessage;
                }
                if (resMainInfo.IsRelationUpdateAllRemark)
                {
                    string updateMessage = "";
                    RelationUpdateRemark(currentInfo, resMainInfo.ResDetailInfos[0].Regid, out updateMessage);
                    otherMessage += updateMessage;
                }
                var regIdsAll = "";
                var result = GetResMainInfo(hid, resMainInfo.Resid, resMainInfo.ResDetailInfos[0].Regid, resMainInfo.ResDetailInfos[0].SelectCustomerId, out regIdsAll);
                result.SelectRegIdIsNewCheckIn = SelectRegIdIsNewCheckIn;
                result.OtherMessage = otherMessage + ExistsCerId(hid, resMainInfo.ResDetailInfos[0]) + ExistsProfileId(hid, resMainInfo.ResDetailInfos[0]) + GetResOrderTips(hid, resMainInfo.Resid);
                #region 调用存储过程更改房态
                _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                    , new SqlParameter("@hid", currentInfo.HotelId)
                    , new SqlParameter("@ids", regIdsAll)
                    );
                #endregion
                ResDetailPlanChangedRecords(hid, resMainInfo.ResDetailInfos[0].Regid, oldRateCode, oldResDetailPlans, currentInfo.UserName);
                ResSaveToLog(currentInfo, oldResDetailAllInfo, GetResDetailAllInfo(hid, resMainInfo.ResDetailInfos[0].Regid), authUserName,reason);
                return JsonResultData.Successed(result);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                return JsonResultData.Failure("订单已被修改，请获取订单最新内容！");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 批量预订或者入住保存
        /// </summary>
        /// <param name="addModel">批量预订或者入住模型</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns>保存成功后的预订单及所有明细信息</returns>
        public JsonResultData BatchAddRes(ResBatchAddModel addModel, ICurrentInfo currentInfo, DateTime businessDate)
        {
            try
            {
                #region 检查数据有效性
                JsonResultData validResult = Validate(addModel, currentInfo, businessDate);
                if (!validResult.Success)
                {
                    return validResult;
                }
                #endregion
                string hid = currentInfo.HotelId;
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                #region 增加修改主单
                var resMain = new ResMainInfo
                {
                    Name = addModel.Name,
                    ResNoExt = addModel.ResNoExt,
                    ResCustMobile = addModel.ResCustMobile,
                    ResCustName = addModel.ResCustName,
                    IsGroup = addModel.IsGroup,
                };
                if (!string.IsNullOrWhiteSpace(addModel.Cttid))
                {
                    resMain.Cttid = Guid.Parse(addModel.Cttid);
                }
                AddRes(resMain, currentInfo);
                #endregion
                #region 增加修改明细

                #region 源价格
                var roomTypeIds = addModel.RoomTypeInfos.Select(c => c.roomTypeId).ToList();
                var beginDate = addModel.IsCheckIn == 1 ? businessDate : addModel.arriveDate.Value.Date;
                var endDate = addModel.depDate.Value.Date;
                var originRates = _pmsContext.RateDetails.Where(w => w.Hid == hid && w.Rateid == addModel.rateCode && roomTypeIds.Contains(w.RoomTypeid) && w.RateDate >= beginDate && w.RateDate <= endDate).Select(s => new { s.RoomTypeid, s.RateDate, s.Rate  }).ToList();
                #endregion
                List<ResAdjustPriceInfo.AdjustPriceOrderModel> adjustPriceOrderList = new List<ResAdjustPriceInfo.AdjustPriceOrderModel>();
                List<KeyValuePairModel<string, string>> rateCodeIdsTemp = new List<KeyValuePairModel<string, string>>();
                var firstRegId = "";
                var regIdsAll = new StringBuilder();
                var split = "";
                foreach (var roomTypeInfo in addModel.RoomTypeInfos)
                {
                    byte bbf = 0;
                    if (!string.IsNullOrWhiteSpace(roomTypeInfo.Bbf))
                    {
                        bbf = Convert.ToByte(roomTypeInfo.Bbf);
                    }
                    var rates = roomTypeInfo.priceStr.Split(',');
                    var count = 0;
                    if (roomTypeInfo.RoomInfos != null)
                    {
                        foreach (var roomInfo in roomTypeInfo.RoomInfos)
                        {
                            //增加子单
                            var resDetail = new ResDetailInfo
                            {
                                RateCode = addModel.rateCode,
                                RoomTypeId = roomTypeInfo.roomTypeId,
                                Sourceid = addModel.customerSource,
                                Marketid = addModel.marketType,
                                Bbf = bbf,
                                Roomid = roomInfo.roomId,
                                RoomNo = roomInfo.roomNo,
                                RoomQty = 1,
                                ArrDate = addModel.arriveDate.ToString(),
                                DepDate = addModel.depDate.ToString(),
                                HoldDate = addModel.holdDate.ToString(),
                                Profileid = addModel.ProfileId,
                                Remark = addModel.remark,
                                Spec = addModel.special,
                                Guestname = addModel.ResCustName,
                                GuestMobile = addModel.ResCustMobile,
                                Rate = Convert.ToDecimal(rates[0]),
                                OrderDetailRegInfos = new List<ResDetailRegInfo> {
                                new ResDetailRegInfo {GuestName = addModel.ResCustName, CerType = "01", Gender = "M", IsMast = "1", Nation = "中国" }
                            },
                                OrderDetailPlans = new List<ResDetailPlanInfo> { },
                                Status = addModel.IsCheckIn == 1 ? ResDetailStatus.I.ToString() : ResDetailStatus.R.ToString()
                            };
                            //转换价格信息
                            var priceDate = addModel.IsCheckIn == 1 ? businessDate : addModel.arriveDate.Value;
                            foreach (var price in rates)
                            {
                                var originPrice = originRates.Where(c => c.RoomTypeid == resDetail.RoomTypeId && c.RateDate == priceDate.Date).Select(c => c.Rate).FirstOrDefault();
                                resDetail.OrderDetailPlans.Add(new ResDetailPlanInfo { Price = Convert.ToDecimal(price), Ratedate = priceDate.ToShortDateString(), OriginPrice = originPrice });
                                priceDate = priceDate.AddDays(1);
                            }
                            AddResDetail(resDetail, currentInfo, resMain.Resno, businessDate);
                            rateCodeIdsTemp.Add(new KeyValuePairModel<string, string>(resDetail.Regid, resDetail.RateCode));
                            regIdsAll.Append(split).Append(resDetail.Regid);
                            split = ",";
                            if (string.IsNullOrWhiteSpace(firstRegId))
                            {
                                firstRegId = resDetail.Regid;
                            }
                            adjustPriceOrderList.Add(new ResAdjustPriceInfo.AdjustPriceOrderModel { RateCodeId = resDetail.RateCode, RoomTypeId = resDetail.RoomTypeId, RegId = resDetail.Regid, GuestName = resDetail.Guestname, RoomNo = resDetail.RoomNo });
                        }
                        count = roomTypeInfo.RoomInfos.Count;
                    }
                    //处理指定房数大于选择房间的情况
                    var balance = roomTypeInfo.qty - count;
                    if (balance > 0)
                    {
                        //增加子单
                        var resDetail = new ResDetailInfo
                        {
                            RateCode = addModel.rateCode,
                            RoomTypeId = roomTypeInfo.roomTypeId,
                            Sourceid = addModel.customerSource,
                            Marketid = addModel.marketType,
                            Bbf = bbf,
                            Roomid = "",
                            RoomNo = "",
                            RoomQty = balance,
                            ArrDate = addModel.arriveDate.ToString(),
                            DepDate = addModel.depDate.ToString(),
                            HoldDate = addModel.holdDate.ToString(),
                            Profileid = addModel.ProfileId,
                            Remark = addModel.remark,
                            Spec = addModel.special,
                            Guestname = addModel.ResCustName,
                            GuestMobile = addModel.ResCustMobile,
                            Rate = Convert.ToDecimal(rates[0]),
                            OrderDetailRegInfos = new List<ResDetailRegInfo> {
                                new ResDetailRegInfo {GuestName = addModel.ResCustName, CerType = "01", Gender = "M", IsMast = "1", Nation = "中国" }
                            },
                            OrderDetailPlans = new List<ResDetailPlanInfo> { },
                            Status = addModel.IsCheckIn == 1 ? ResDetailStatus.I.ToString() : ResDetailStatus.R.ToString()
                        };
                        //转换价格信息
                        var priceDate = addModel.arriveDate.Value;
                        foreach (var price in rates)
                        {
                            var originPrice = originRates.Where(c => c.RoomTypeid == resDetail.RoomTypeId && c.RateDate == priceDate.Date).Select(c => c.Rate).FirstOrDefault();
                            resDetail.OrderDetailPlans.Add(new ResDetailPlanInfo { Price = Convert.ToDecimal(price), Ratedate = priceDate.ToShortDateString(), OriginPrice = originPrice });
                            priceDate = priceDate.AddDays(1);
                        }
                        AddResDetail(resDetail, currentInfo, resMain.Resno, businessDate);
                        rateCodeIdsTemp.Add(new KeyValuePairModel<string, string>(resDetail.Regid, resDetail.RateCode));
                        regIdsAll.Append(split).Append(resDetail.Regid);
                        split = ",";
                        if (string.IsNullOrWhiteSpace(firstRegId))
                        {
                            firstRegId = resDetail.Regid;
                        }
                        adjustPriceOrderList.Add(new ResAdjustPriceInfo.AdjustPriceOrderModel { RateCodeId = resDetail.RateCode, RoomTypeId = resDetail.RoomTypeId, RegId = resDetail.Regid, GuestName = resDetail.Guestname, RoomNo = resDetail.RoomNo });
                    }
                }
                #endregion

                #region 积分换房提示
                if (addModel.IsCheckIn == 1 && addModel.UseScoreSaveContinue != "1")
                {
                    var regids_UseScore = GetRateIsUseScoreByRegIds(hid, rateCodeIdsTemp);
                    if(regids_UseScore != null && regids_UseScore.Count > 0)
                    {
                        return CheckUseScoreToCheckin(hid, regids_UseScore);
                    }
                }
                #endregion
                string authUserName = null;string reason = "";
                var checkAuthAdjustResult = CheckAuthAdjustPrice(currentInfo, adjustPriceOrderList, ResAdjustPriceInfo.AdjustPriceOperationSource.OrderBatchAdded);
                if (!checkAuthAdjustResult.Success)
                {
                    if (!CheckAuthAdjustPriceResult(currentInfo, addModel.AuthorizationSaveContinue, regIdsAll.ToString(), out authUserName,out reason))
                    {
                        return checkAuthAdjustResult;
                    }
                }
                if (addModel.IsCheckIn == 1 && addModel.UseScoreSaveContinue == "1")
                {
                    var regids_UseScore = GetRateIsUseScoreByRegIds(hid, rateCodeIdsTemp);
                    if (regids_UseScore != null && regids_UseScore.Count > 0)
                    {
                        #region 积分换房
                        JsonResultData useScoreToCheckin_Result = JsonResultData.Failure("积分换房失败！");
                        try
                        {
                            using (var tc = new System.Transactions.TransactionScope())
                            {
                                _pmsContext.SaveChanges();
                                useScoreToCheckin_Result = ExecUseScoreToCheckin(currentInfo, regids_UseScore);
                                if (useScoreToCheckin_Result != null && useScoreToCheckin_Result.Success)
                                {
                                    tc.Complete();
                                }
                                else
                                {
                                    throw new Exception("");
                                }
                            }
                        }
                        catch { return useScoreToCheckin_Result; }
                        #endregion
                    }
                    else
                    {
                        _pmsContext.SaveChanges();
                    }
                }
                else
                {
                    _pmsContext.SaveChanges();
                }

                #region 调用存储过程更改房态
                _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                    , new SqlParameter("@hid", currentInfo.HotelId)
                    , new SqlParameter("@ids", regIdsAll.ToString())
                    );
                #endregion

                #region 日志
                string regids = regIdsAll.ToString();
                if (!string.IsNullOrWhiteSpace(regids))
                {
                    string[] regidList = regids.Split(',');
                    if(regidList != null && regidList.Length > 0)
                    {
                        foreach(var regid in regidList)
                        {
                            ResSaveToLog(currentInfo, null, GetResDetailAllInfo(hid, regid), authUserName,reason);
                        }
                    }
                }
                #endregion

                return JsonResultData.Successed(firstRegId);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        #region 内部使用 增加或修改预订单信息

        #region 获取订单信息表中字段ID所对应的名称
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
        public void GetFieldName(string hid, ResMainInfo resInfo, out string companyName, out string roomTypeName, out string rateCodeName, out string sourceName, out string marketName)
        {
            companyName = "";
            roomTypeName = "";
            rateCodeName = "";
            sourceName = "";
            marketName = "";
            if (resInfo != null && resInfo.ResDetailInfos != null && resInfo.ResDetailInfos.Count > 0)
            {
                if (resInfo.Cttid != null)
                {
                    Guid ccid = resInfo.Cttid.Value;
                    companyName = _pmsContext.Companys.Where(c => c.Id == ccid && c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                }
                if (!string.IsNullOrWhiteSpace(resInfo.ResDetailInfos[0].RoomTypeId))
                {
                    string roomTypeId = resInfo.ResDetailInfos[0].RoomTypeId;
                    roomTypeName = _pmsContext.RoomTypes.Where(c => c.Id == roomTypeId && c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                }
                if (!string.IsNullOrWhiteSpace(resInfo.ResDetailInfos[0].RateCode))
                {
                    string rateCode = resInfo.ResDetailInfos[0].RateCode;
                    rateCodeName = _pmsContext.Rates.Where(c => c.Id == rateCode && c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                }
                if (!string.IsNullOrWhiteSpace(resInfo.ResDetailInfos[0].Sourceid))
                {
                    string Sourceid = resInfo.ResDetailInfos[0].Sourceid;
                    sourceName = _pmsContext.CodeLists.Where(c => c.Id == Sourceid && c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                }
                if (!string.IsNullOrWhiteSpace(resInfo.ResDetailInfos[0].Marketid))
                {
                    string marketid = resInfo.ResDetailInfos[0].Marketid;
                    marketName = _pmsContext.CodeLists.Where(c => c.Id == marketid && c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                }
            }
        }
        #endregion

        #region 增加或修改预订单信息验证
        private JsonResultData Validate(ResMainInfo resMainInfo, ICurrentInfo currentInfo, DateTime businessDate)
        {
            #region 检查主单
            if (resMainInfo == null || resMainInfo.ResDetailInfos == null || resMainInfo.ResDetailInfos.Count <= 0)
            {
                return JsonResultData.Failure("请填写表单");
            }
            if (string.IsNullOrWhiteSpace(resMainInfo.ResCustName))
            {
                return JsonResultData.Failure("请输入预订人/入住人");
            }
            if (!string.IsNullOrWhiteSpace(resMainInfo.ResCustMobile))
            {
                if (!RegexHelper.IsRightMobile(resMainInfo.ResCustMobile))
                {
                    return JsonResultData.Failure("手机号码格式不正确");
                }
            }
            if (resMainInfo.IsGroup == null || (resMainInfo.IsGroup != 0 && resMainInfo.IsGroup != 1))
            {
                return JsonResultData.Failure("请选择团体/散客");
            }
            #endregion

            //子单验证
            while (resMainInfo.ResDetailInfos.Count > 1) { resMainInfo.ResDetailInfos.RemoveAt(resMainInfo.ResDetailInfos.Count - 1); }//页面操作方式，一次只保存一个子单
            var detail = resMainInfo.ResDetailInfos[0];

            #region 检查子单
            if (string.IsNullOrWhiteSpace(detail.RateCode))
            {
                return JsonResultData.Failure("请选择价格代码");
            }
            var rateEntity = _pmsContext.Rates.Where(c => c.Hid == currentInfo.HotelId && c.Id == detail.RateCode).AsNoTracking().FirstOrDefault();
            if(rateEntity == null)
            {
                return JsonResultData.Failure("请选择价格代码");
            }
            else
            {
                if(rateEntity.NoPrintCompany == "2" && (resMainInfo.Cttid == null || resMainInfo.Cttid.HasValue == false))
                {
                    return JsonResultData.Failure("此价格代码需要录入合约单位，请录入合约单位再保存！");
                }
                if (rateEntity.NoPrintProfile == "2" && (detail.Profileid == null || detail.Profileid.HasValue == false))
                {
                    return JsonResultData.Failure("此价格代码需要录入会员，请录入会员再保存！");
                }
            }
            if (string.IsNullOrWhiteSpace(detail.RoomTypeId))
            {
                return JsonResultData.Failure("请选择房间类型");
            }
            if (!ValidateRateCodeAndRoomType(currentInfo.HotelId, detail.RateCode, detail.RoomTypeId))
            {
                string rateCodeName = _pmsContext.Rates.Where(c => c.Hid == currentInfo.HotelId && c.Id == detail.RateCode).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                return JsonResultData.Failure(string.Format("{0}价格代码不适用当前房型", (!string.IsNullOrWhiteSpace(rateCodeName)?("<"+ rateCodeName + ">"):"")));
            }
            if (string.IsNullOrWhiteSpace(detail.Status))
            {
                return JsonResultData.Failure("订单状态错误");
            }
            var statusList = EnumExtension.ToSelectList(typeof(ResDetailStatus), EnumValueType.Text, EnumValueType.Description);
            if (!statusList.Select(c => c.Value).Contains(detail.Status))
            {
                return JsonResultData.Failure("订单状态错误");
            }
            if(detail.Status != ResDetailStatus.R.ToString() && detail.Status != ResDetailStatus.I.ToString() && detail.Status != ResDetailStatus.N.ToString())
            {
                return JsonResultData.Failure("只能保存预订或入住");
            }
            if (detail.Status == ResDetailStatus.I.ToString() && (string.IsNullOrWhiteSpace(detail.Roomid) || string.IsNullOrWhiteSpace(detail.RoomNo)))
            {
                return JsonResultData.Failure("请选择房间");
            }
            if(!string.IsNullOrWhiteSpace(detail.Roomid) && !string.IsNullOrWhiteSpace(detail.RoomNo) && detail.Status == ResDetailStatus.I.ToString())
            {
                var roomEntity = _pmsContext.Rooms.Where(c => c.Id == detail.Roomid && c.RoomNo == detail.RoomNo).FirstOrDefault();
                if(roomEntity != null)
                {
                    if(roomEntity.RoomTypeid != detail.RoomTypeId)
                    {
                        return JsonResultData.Failure("房号不属于此房间类型");
                    }
                }
                else
                {
                    return JsonResultData.Failure("房间号已不存在");
                }
            }
            if (string.IsNullOrWhiteSpace(detail.Sourceid))
            {
                return JsonResultData.Failure("请选择客人来源");
            }
            if (string.IsNullOrWhiteSpace(detail.Marketid))
            {
                return JsonResultData.Failure("请选择市场分类");
            }
            if (detail.Bbf < 0)
            {
                return JsonResultData.Failure("请输入早餐份数");
            }
            if (detail.RoomQty <= 0)
            {
                return JsonResultData.Failure("请输入房数");
            }
            if (string.IsNullOrWhiteSpace(detail.ArrDate))
            {
                return JsonResultData.Failure("请选择抵店时间");
            }
            if (string.IsNullOrWhiteSpace(detail.DepDate))
            {
                return JsonResultData.Failure("请选择离店时间");
            }
            if (string.IsNullOrWhiteSpace(detail.HoldDate))
            {
                return JsonResultData.Failure("请选择保留时间");
            }
            DateTime arriveDateValue;
            if (!DateTime.TryParse(detail.ArrDate, out arriveDateValue))
            {
                return JsonResultData.Failure("抵店时间格式不正确，请重新选择");
            }
            DateTime depDateValue;
            if (!DateTime.TryParse(detail.DepDate, out depDateValue))
            {
                return JsonResultData.Failure("离店时间格式不正确，请重新选择");
            }
            DateTime holdDateValue;
            if (!DateTime.TryParse(detail.HoldDate, out holdDateValue))
            {
                return JsonResultData.Failure("保留时间格式不正确，请重新选择");
            }
            if (detail.Status == ResDetailStatus.R.ToString() || detail.Status == ResDetailStatus.N.ToString())
            {
                if (arriveDateValue <= DateTime.Now)
                {
                    return JsonResultData.Failure("预订抵店时间必须大于当前时间");
                }
            }
            if (string.IsNullOrWhiteSpace(detail.Regid))
            {
                if (detail.Status == ResDetailStatus.I.ToString())
                {
                    arriveDateValue = DateTime.Now;
                    detail.ArrDate = arriveDateValue.ToDateTimeString();
                }
            }
            if (depDateValue <= arriveDateValue)
            {
                return JsonResultData.Failure("离店时间必须大于抵店时间");
            }
            if (holdDateValue < arriveDateValue && (detail.Status == ResDetailStatus.R.ToString() || detail.Status == ResDetailStatus.N.ToString()))
            {
                return JsonResultData.Failure("保留时间必须大于抵店时间");
            }
            if (holdDateValue > depDateValue && (detail.Status == ResDetailStatus.R.ToString() || detail.Status == ResDetailStatus.N.ToString()))
            {
                return JsonResultData.Failure("保留时间必须小于离店时间");
            }
            if (detail.Status == ResDetailStatus.I.ToString() && arriveDateValue >= DateTime.Now.AddDays(1).Date)
            {
                return JsonResultData.Failure("抵店时间不大于今天才能入住");
            }
            if (detail.OrderDetailPlans == null || detail.OrderDetailPlans.Count <= 0)
            {
                return JsonResultData.Failure("请通过房价修改来指定房价");
            }
            foreach (var item in detail.OrderDetailPlans)
            {
                DateTime rateDateValue;
                if (!DateTime.TryParse(item.Ratedate, out rateDateValue))
                {
                    return JsonResultData.Failure("房价修改中时间格式不正确");
                }
                if (item == null || !item.Price.HasValue || item.Price < 0)
                {
                    return JsonResultData.Failure("房价修改中价格不正确");
                }
                if(item.OriginPrice < 0)
                {
                    item.OriginPrice = null;
                }
            }
            if(!string.IsNullOrWhiteSpace(detail.Regid) && detail.Status == ResDetailStatus.I.ToString())
            {
                //价格代码改变验证
                var originDetail = _pmsContext.ResDetails.Where(c => c.Hid == currentInfo.HotelId && c.Regid == detail.Regid && c.Status == "I").AsNoTracking().FirstOrDefault();
                if(originDetail != null && originDetail.RateCode != detail.RateCode && originDetail.ArrBsnsDate != businessDate)
                {
                    var isHot = _pmsContext.Rates.Where(c => c.Hid == currentInfo.HotelId && c.Id == detail.RateCode).AsNoTracking().Select(c => c.IsHou).FirstOrDefault();
                    if (isHot != null && isHot == true)
                    {
                        return JsonResultData.Failure("此订单已入住已超过一天，不能转为钟点房！");
                    }
                }
            }
            if(detail.Status == ResDetailStatus.I.ToString())
            {
                bool isTrueRateTime = false;
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;

                DateTime arrDate = DateTime.Parse(detail.ArrDate);
                bool isTrueStartTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.StartTime), out startTime);
                bool isTrueEndTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.EndTime), out endTime);

                if(isTrueStartTime && isTrueEndTime && startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    if(startTime < endTime && arrDate >= startTime && arrDate <= endTime)
                    {
                        isTrueRateTime = true;
                    }
                }
                if (!isTrueRateTime)
                {
                    return JsonResultData.Failure(string.Format("抵店时间：{0}，当前价格代码：{1}，只有在时间段内[{2}-{3}]可入住！", arrDate.ToString("HH:mm"), rateEntity.Name, rateEntity.StartTime, rateEntity.EndTime));
                }
            }
            if (rateEntity.IsDayRoom == true)
            {
                if (DateTime.Parse(detail.ArrDate).Date != DateTime.Parse(detail.DepDate).Date)
                {
                    return JsonResultData.Failure(string.Format("当前价格代码：{0}，属于白日房，抵店时间和离店时间要求必须是同一天！", rateEntity.Name));
                }
            }
            #endregion

            #region 检查客人资料
            if (detail.OrderDetailRegInfos.Count <= 0)
            {
                return JsonResultData.Failure("请填写客人信息");
            }
            foreach (var reg in detail.OrderDetailRegInfos)
            {
                reg.IsMast = "0";
                if (string.IsNullOrWhiteSpace(reg.GuestName))
                {
                    return JsonResultData.Failure("请输入客人姓名");
                }
                if (!string.IsNullOrWhiteSpace(reg.Birthday))
                {
                    DateTime birthdayValue;
                    if (!DateTime.TryParse(reg.Birthday, out birthdayValue))
                    {
                        return JsonResultData.Failure("生日格式不正确，请重新选择");
                    }
                }
                if (!string.IsNullOrWhiteSpace(reg.Email))
                {
                    if (!RegexHelper.IsRightEmail(reg.Email))
                    {
                        return JsonResultData.Failure("邮箱格式不正确");
                    }
                }
            }
            #endregion

            #region 检查发票信息
            //if (resMainInfo.ResInvoiceInfo != null)
            //{

            //    if (resMainInfo.ResInvoiceInfo.InvoiceType == null)
            //    {
            //        if (!string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxName)
            //            ||
            //            !string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxNo)
            //            ||
            //            !string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxAddTel)
            //            ||
            //            !string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxBankAccount))
            //        {
            //            return JsonResultData.Failure("请选择发票类型");
            //        }
            //    }
            //    else if (resMainInfo.ResInvoiceInfo.InvoiceType == true)
            //    {
            //        if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxName))
            //        {
            //            return JsonResultData.Failure("请输入发票抬头");
            //        }
            //        if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxNo))
            //        {
            //            return JsonResultData.Failure("请输入税务登记号");
            //        }
            //        if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxAddTel))
            //        {
            //            return JsonResultData.Failure("请输入地址和电话");
            //        }
            //        if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxBankAccount))
            //        {
            //            return JsonResultData.Failure("请输入开户银行和账号");
            //        }
            //    }
            //    else if (resMainInfo.ResInvoiceInfo.InvoiceType == false)
            //    {
            //        if (string.IsNullOrWhiteSpace(resMainInfo.ResInvoiceInfo.TaxName))
            //        {
            //            return JsonResultData.Failure("请输入发票抬头");
            //        }
            //    }
            //}
            #endregion

            #region 给明细记录赋值客人名和客人手机为所有客人信息中的第一位客人名和客人手机
            detail.Guestname = detail.OrderDetailRegInfos[0].GuestName;
            detail.GuestMobile = detail.OrderDetailRegInfos[0].Mobile;
            detail.OrderDetailRegInfos[0].IsMast = "1";
            #endregion

            #region 确定子单执行价
            //在房价计划中查询，日期等于今天的记录，其价格作为子单的执行价。
            detail.Rate = null;
            string nowDate = businessDate.ToDateString();
            detail.Rate = detail.OrderDetailPlans.Where(c => c.Ratedate == nowDate).Select(s => s.Price).SingleOrDefault();
            if (detail.Rate == null)
            {
                List<DateTime> dateList = new List<DateTime>();
                foreach (var item in detail.OrderDetailPlans)
                {
                    dateList.Add(DateTime.Parse(item.Ratedate));
                }
                //给明细记录赋值房价计划中的第一天价格做为子单的执行价,
                if (businessDate.Date <= dateList.Min())
                {
                    string minDate = dateList.Min().ToDateString();
                    detail.Rate = detail.OrderDetailPlans.Where(c => c.Ratedate == minDate).Select(s => s.Price).Single();
                }
                //给明细记录赋值房价计划中的最后一天价格做为子单的执行价,
                if (businessDate.Date >= dateList.Max())
                {
                    string maxDate = dateList.Max().ToDateString();
                    detail.Rate = detail.OrderDetailPlans.Where(c => c.Ratedate == maxDate).Select(s => s.Price).Single();
                }
            }
            #endregion

            var detail_arrDate = string.IsNullOrWhiteSpace(detail.Regid) && detail.Status == ResDetailStatus.I.ToString() ? businessDate.ToDateString() : detail.ArrDate;
            JsonResultData checkResult = UpRoomEnableCheck(currentInfo.HotelId, new List<string> { detail.Regid }, GetStatus(detail.Status), new List<string> { UpRoomEnableCheck_checkInfo(detail.Roomid, detail.RoomTypeId, detail_arrDate, detail.DepDate, detail.RoomQty.ToString(), detail.Sourceid) }, resMainInfo.SaveContinue);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            return JsonResultData.Successed();
        }
        private JsonResultData Validate(ResBatchAddModel resMainInfo, ICurrentInfo currentInfo, DateTime businessDate)
        {
            #region 检查主单
            if (resMainInfo == null)
            {
                return JsonResultData.Failure("请填写表单");
            }
            if (string.IsNullOrWhiteSpace(resMainInfo.ResCustName))
            {
                return JsonResultData.Failure("请输入联系人");
            }
            if (!string.IsNullOrWhiteSpace(resMainInfo.ResCustMobile))
            {
                if (!RegexHelper.IsRightMobile(resMainInfo.ResCustMobile))
                {
                    return JsonResultData.Failure("手机号码格式不正确");
                }
            }
            if (resMainInfo.IsGroup == null || (resMainInfo.IsGroup != 0 && resMainInfo.IsGroup != 1))
            {
                return JsonResultData.Failure("请选择团体/散客");
            }
            #endregion            

            #region 检查子单
            if (string.IsNullOrWhiteSpace(resMainInfo.rateCode))
            {
                return JsonResultData.Failure("请选择价格代码");
            }
            var rateEntity = _pmsContext.Rates.Where(c => c.Hid == currentInfo.HotelId && c.Id == resMainInfo.rateCode).AsNoTracking().FirstOrDefault();
            if (rateEntity == null)
            {
                return JsonResultData.Failure("请选择价格代码");
            }
            else
            {
                if (rateEntity.NoPrintCompany == "2" && string.IsNullOrWhiteSpace(resMainInfo.Cttid))
                {
                    return JsonResultData.Failure("此价格代码需要录入合约单位，请录入合约单位再保存！");
                }
                if (rateEntity.NoPrintProfile == "2" && (resMainInfo.ProfileId == null || resMainInfo.ProfileId.HasValue == false))
                {
                    return JsonResultData.Failure("此价格代码需要录入会员，请录入会员再保存！");
                }
            }
            if (resMainInfo.RoomTypeInfos == null || resMainInfo.RoomTypeInfos.Count <= 0)
            {
                return JsonResultData.Failure("请选择房间或者指定房型下的数量");
            }
            foreach (var roomTypeInfo in resMainInfo.RoomTypeInfos)
            {
                if (!ValidateRateCodeAndRoomType(currentInfo.HotelId, resMainInfo.rateCode, roomTypeInfo.roomTypeId))
                {
                    string rateCodeName = _pmsContext.Rates.Where(c => c.Hid == currentInfo.HotelId && c.Id == resMainInfo.rateCode).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                    return JsonResultData.Failure(string.Format("{0}价格代码不适用当前房型", (!string.IsNullOrWhiteSpace(rateCodeName) ? ("<" + rateCodeName + ">") : "")));
                }
                if (string.IsNullOrWhiteSpace(roomTypeInfo.priceStr))
                {
                    return JsonResultData.Failure("请通过调整选中房型价格来指定房价");
                }
            }
            if (string.IsNullOrWhiteSpace(resMainInfo.customerSource))
            {
                return JsonResultData.Failure("请选择客人来源");
            }
            if (string.IsNullOrWhiteSpace(resMainInfo.marketType))
            {
                return JsonResultData.Failure("请选择市场分类");
            }
            if (!resMainInfo.arriveDate.HasValue)
            {
                return JsonResultData.Failure("请选择抵店时间");
            }
            if (!resMainInfo.depDate.HasValue)
            {
                return JsonResultData.Failure("请选择离店时间");
            }
            if (!resMainInfo.holdDate.HasValue)
            {
                return JsonResultData.Failure("请选择保留时间");
            }
            if (resMainInfo.depDate <= resMainInfo.arriveDate)
            {
                return JsonResultData.Failure("离店时间必须大于抵店时间");
            }
            if (resMainInfo.holdDate < resMainInfo.arriveDate)
            {
                return JsonResultData.Failure("保留时间必须大于抵店时间");
            }
            if (resMainInfo.holdDate > resMainInfo.depDate)
            {
                return JsonResultData.Failure("保留时间必须小于离店时间");
            }
            if (resMainInfo.IsCheckIn == 1 && resMainInfo.arriveDate >= DateTime.Now.AddDays(1).Date)
            {
                return JsonResultData.Failure("抵店时间不大于今天才能入住");
            }
            if (resMainInfo.IsCheckIn == 0)
            {
                if (resMainInfo.arriveDate <= DateTime.Now)
                {
                    return JsonResultData.Failure("预订抵店时间必须大于当前时间");
                }
            }
            if(resMainInfo.IsCheckIn == 1)
            {
                bool isTrueRateTime = false;
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;

                DateTime arrDate = DateTime.Now;
                bool isTrueStartTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.StartTime), out startTime);
                bool isTrueEndTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.EndTime), out endTime);

                if (isTrueStartTime && isTrueEndTime && startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                {
                    if (startTime < endTime && arrDate >= startTime && arrDate <= endTime)
                    {
                        isTrueRateTime = true;
                    }
                }
                if (!isTrueRateTime)
                {
                    return JsonResultData.Failure(string.Format("抵店时间：{0}，当前价格代码：{1}，只有在时间段内[{2}-{3}]可入住！", arrDate.ToString("HH:mm"), rateEntity.Name, rateEntity.StartTime, rateEntity.EndTime));
                }
            }
            if (rateEntity.IsDayRoom == true)
            {
                DateTime arrDateTime = resMainInfo.arriveDate.Value.Date;
                if (resMainInfo.IsCheckIn == 1)
                {
                    arrDateTime = DateTime.Now;
                }
                if (arrDateTime != resMainInfo.depDate.Value.Date)
                {
                    return JsonResultData.Failure(string.Format("当前价格代码：{0}，属于白日房，抵店时间和离店时间要求必须是同一天！", rateEntity.Name));
                }
            }
            #endregion

            #region 检测房间可用性及冲突
            var roomEnableCheckInfo = new List<string>();
            foreach (var roomTypeInfo in resMainInfo.RoomTypeInfos)
            {
                var count = 0;
                if (roomTypeInfo.RoomInfos != null)
                {
                    foreach (var roomInfo in roomTypeInfo.RoomInfos)
                    {
                        roomEnableCheckInfo.Add(UpRoomEnableCheck_checkInfo(roomInfo.roomId, roomTypeInfo.roomTypeId, (resMainInfo.IsCheckIn == 1 ? businessDate.ToDateString() : resMainInfo.arriveDate.ToString()), resMainInfo.depDate.ToString(), "1", ""));
                    }
                    count = roomTypeInfo.RoomInfos.Count;
                }
                //如果房型中选中数量大于选择的房间数，则再传一条可用性检测记录
                var balance = roomTypeInfo.qty - count;
                if (balance > 0)
                {
                    roomEnableCheckInfo.Add(UpRoomEnableCheck_checkInfo("", roomTypeInfo.roomTypeId, (resMainInfo.IsCheckIn == 1 ? businessDate.ToDateString() : resMainInfo.arriveDate.ToString()), resMainInfo.depDate.ToString(), balance.ToString(), ""));
                }
            }

            JsonResultData checkResult = UpRoomEnableCheck(currentInfo.HotelId, null, resMainInfo.IsCheckIn == 1 ? ResDetailStatus.I : ResDetailStatus.R, roomEnableCheckInfo, resMainInfo.saveContinue);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            #endregion
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 验证 房间类型 是否 在 价格代码适合房型列表 内
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="rateCodeId">价格代码ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <returns>true在列表内，false不在列表内</returns>
        private bool ValidateRateCodeAndRoomType(string hid, string rateCodeId, string roomTypeId)
        {
            var isTrue = false;
            var roomTypeIds = _pmsContext.Rates.Where(c => c.Hid == hid && c.Id == rateCodeId).Select(c => c.RoomTypeids).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(roomTypeIds))
            {
                if (roomTypeIds.Contains(","))
                {
                    string[] temp = roomTypeIds.Split(',');
                    isTrue = (temp != null && temp.Length > 0 && temp.Contains(roomTypeId)) ? true : false;
                }
                else
                {
                    isTrue = (roomTypeIds == roomTypeId);
                }
            }
            else
            {
                isTrue = true;//没有内容则默认为不限制
            }
            return isTrue;
        }
        #endregion

        #region 增加修改订单主单
        private void AddRes(ResMainInfo resMainInfo, ICurrentInfo currentInfo)
        {
            string hid = currentInfo.HotelId;
            string resNo = _pmsContext.GetBaseNoForRes(hid);
            Res res = new Res
            {
                Hid = currentInfo.HotelId,
                Resid = string.Format("{0}{1}", hid, resNo),
                Resno = resNo,
                ResTime = DateTime.Now,
                CDate = DateTime.Now,
                OrderFrom = 1,
                ResUser = currentInfo.UserName,

                Name = resMainInfo.Name,
                ResNoExt = resMainInfo.ResNoExt,
                ResCustMobile = resMainInfo.ResCustMobile,
                ResCustName = resMainInfo.ResCustName,
                Cttid = resMainInfo.Cttid,
                IsGroup = resMainInfo.IsGroup,
                IsCustemSecret = resMainInfo.IsCustemSecret.HasValue? Convert.ToBoolean(resMainInfo.IsCustemSecret.Value):false,
                IsHidePrice = resMainInfo.IsHidePrice.HasValue? Convert.ToBoolean(resMainInfo.IsHidePrice.Value): false
            };
            resMainInfo.Resid = res.Resid;
            resMainInfo.Resno = res.Resno;
            if(resMainInfo.Cttid != null && resMainInfo.Cttid.HasValue)
            {
                res.CttSales = _pmsContext.Companys.AsNoTracking().Where(c => c.Hid == hid && c.Id == resMainInfo.Cttid.Value).Select(c => c.Sales).FirstOrDefault();
            }
            _pmsContext.Reses.Add(res);
        }
        private void UpdateRes(ResMainInfo resMainInfo, Res originRes)
        {
            string CttSales = null;
            if (resMainInfo.Cttid != originRes.Cttid)
            {
                if(resMainInfo.Cttid != null && resMainInfo.Cttid.HasValue)
                {
                    CttSales = _pmsContext.Companys.AsNoTracking().Where(c => c.Hid == originRes.Hid && c.Id == resMainInfo.Cttid.Value).Select(c => c.Sales).FirstOrDefault();
                }
            }
            else
            {
                CttSales = originRes.CttSales;
            }

            Res res = new Res
            {
                Resid = resMainInfo.Resid,
                Name = resMainInfo.Name,
                ResNoExt = resMainInfo.ResNoExt,
                ResCustName = resMainInfo.ResCustName,
                ResCustMobile = resMainInfo.ResCustMobile,
                Cttid = resMainInfo.Cttid,
                IsGroup = resMainInfo.IsGroup,
                IsCustemSecret = resMainInfo.IsCustemSecret.HasValue ? Convert.ToBoolean(resMainInfo.IsCustemSecret.Value) : false,
                IsHidePrice = resMainInfo.IsHidePrice.HasValue ? Convert.ToBoolean(resMainInfo.IsHidePrice.Value) : false,
                CttSales = CttSales
            };
            if (res != null && originRes != null && res.Resid == originRes.Resid)
            {
                CRUDService<Res>.Update<Res>(_pmsContext, res, originRes, new List<string> { "Name", "ResNoExt", "ResCustName", "ResCustMobile", "Cttid", "IsGroup", "IsCustemSecret", "IsHidePrice","CttSales" });
            }
        }
        #endregion

        #region 增加修改主单发票信息
        private void AddResInvoiceInfo(Services.ResManage.ResInvoiceInfo invoiceInfo, ICurrentInfo currentInfo, string resId)
        {
            if (invoiceInfo.InvoiceType != null && !_pmsContext.ResInvoiceInfos.Where(c => c.Hid == currentInfo.HotelId && c.Resid == resId).Any())
            {
                //增加
                _pmsContext.ResInvoiceInfos.Add(new Entities.ResInvoiceInfo
                {
                    Hid = currentInfo.HotelId,
                    Resid = resId,
                    Id = Guid.NewGuid(),
                    InvoiceType = invoiceInfo.InvoiceType,
                    TaxName = invoiceInfo.TaxName,
                    TaxNo = invoiceInfo.TaxNo,
                    TaxAddTel = invoiceInfo.TaxAddTel,
                    TaxBankAccount = invoiceInfo.TaxBankAccount
                });
            }
        }
        private void UpdateResInvoiceInfo(Services.ResManage.ResInvoiceInfo invoiceInfo, Entities.ResInvoiceInfo originInvoiceInfo)
        {
            var invoice = new Entities.ResInvoiceInfo
            {
                Id = (Guid)invoiceInfo.Id,
                InvoiceType = invoiceInfo.InvoiceType,
                TaxName = invoiceInfo.TaxName,
                TaxNo = invoiceInfo.TaxNo,
                TaxAddTel = invoiceInfo.TaxAddTel,
                TaxBankAccount = invoiceInfo.TaxBankAccount
            };
            if (invoice != null && originInvoiceInfo != null && invoice.Id == originInvoiceInfo.Id)
            {
                if (invoice.InvoiceType == null
                    &&
                    string.IsNullOrWhiteSpace(invoice.TaxName)
                    &&
                    string.IsNullOrWhiteSpace(invoice.TaxNo)
                    &&
                    string.IsNullOrWhiteSpace(invoice.TaxAddTel)
                    &&
                    string.IsNullOrWhiteSpace(invoice.TaxBankAccount))
                {
                    var entity = _pmsContext.ResInvoiceInfos.SingleOrDefault(c => c.Id == invoice.Id);
                    _pmsContext.ResInvoiceInfos.Remove(entity);
                }
                else
                {
                    CRUDService<Entities.ResInvoiceInfo>.Update<Entities.ResInvoiceInfo>(_pmsContext, invoice, originInvoiceInfo, new List<string> { "InvoiceType", "TaxName", "TaxNo", "TaxAddTel", "TaxBankAccount" });
                }
            }

        }
        #endregion

        #region 增加修改订单明细
        private void AddResDetail(ResDetailInfo detailInfo, ICurrentInfo currentInfo, string resNo, DateTime bsnsDate)
        {
            //增加子单
            var resDetail = new ResDetail
            {
                Hid = currentInfo.HotelId,
                Regid = _pmsContext.GetBaseNoForRegId(currentInfo.HotelId),
                Resid = string.Format("{0}{1}", currentInfo.HotelId, resNo),
                Resno = resNo,
                RateCode = detailInfo.RateCode,
                RoomTypeid = detailInfo.RoomTypeId,
                Sourceid = detailInfo.Sourceid,
                Marketid = detailInfo.Marketid,
                Bbf = detailInfo.Bbf,
                Roomid = detailInfo.Roomid,
                RoomNo = detailInfo.RoomNo,
                RoomQty = detailInfo.RoomQty,
                ArrDate = Convert.ToDateTime(detailInfo.ArrDate),
                DepDate = Convert.ToDateTime(detailInfo.DepDate),
                HoldDate = Convert.ToDateTime(detailInfo.HoldDate),
                Profileid = detailInfo.Profileid,
                Remark = detailInfo.Remark,
                Spec = detailInfo.Spec,
                Cdate = DateTime.Now,
                BsnsDate = bsnsDate,
                Billtype = (byte)0,
                Guestid = detailInfo.Guestid,
                Guestname = detailInfo.Guestname,
                GuestMobile = detailInfo.GuestMobile,
                Rate = detailInfo.Rate,
                IsSettle = false,
                InputUser = currentInfo.UserName,
            };
            if (!string.IsNullOrWhiteSpace(resDetail.Roomid) && !string.IsNullOrWhiteSpace(resDetail.RoomNo))
            {
                resDetail.RoomQty = 1;
            }
            detailInfo.Regid = resDetail.Regid;
            if (detailInfo.Status == ResDetailStatus.R.ToString())
            {
                resDetail.ResStatus = detailInfo.Status;
                if (resDetail.ArrDate <= DateTime.Now)
                {
                    throw new Exception("预订抵店时间必须大于当前时间");
                }
            }
            else if (detailInfo.Status == ResDetailStatus.I.ToString())
            {
                resDetail.RecStatus = detailInfo.Status;
                resDetail.ArrBsnsDate = bsnsDate;
                resDetail.RegQty = 1;
                resDetail.ArrDate = Convert.ToDateTime(DateTime.Now.ToDateTimeWithoutSecondString());
                resDetail.liveInputUser = currentInfo.UserName;
            }
            else
            {
                throw new Exception("只能增加预订或入住");
            }
            _pmsContext.ResDetails.Add(resDetail);
            //增加随行人
            foreach (var regInfo in detailInfo.OrderDetailRegInfos)
            {
                var id = AddResDetailRegInfo(regInfo, resDetail.Regid, currentInfo);
                detailInfo.SelectCustomerId = id.ToString();
                regInfo.Id = id.ToString();
            }
            //增加房价
            foreach (var price in detailInfo.OrderDetailPlans)
            {
                var ratedate = Convert.ToDateTime(price.Ratedate);
                if (ratedate >= bsnsDate && ratedate <= resDetail.DepDate)
                {
                    AddResDetailPlan(price, resDetail.Regid);
                }
            }
        }
        private void UpdateResDetail(ResDetailInfo detailInfo, JavaScriptSerializer jsSerializer, ICurrentInfo currentInfo, DateTime businessDate, out string msg)
        {
            msg = "";
            //修改子单
            var originDetail = jsSerializer.Deserialize<ResDetail>(detailInfo.OriginResDetailInfo);
            var resDetail = new ResDetail
            {
                Regid = detailInfo.Regid,
                RateCode = detailInfo.RateCode,
                RoomTypeid = detailInfo.RoomTypeId,
                Sourceid = detailInfo.Sourceid,
                Marketid = detailInfo.Marketid,
                RoomQty = detailInfo.RoomQty,
                ArrDate = Convert.ToDateTime(detailInfo.ArrDate),
                DepDate = Convert.ToDateTime(detailInfo.DepDate),
                HoldDate = Convert.ToDateTime(detailInfo.HoldDate),
                Bbf = detailInfo.Bbf,
                Remark = detailInfo.Remark,
                Spec = detailInfo.Spec,
                Profileid = detailInfo.Profileid,
                Guestid = detailInfo.Guestid,
                Guestname = detailInfo.Guestname,
                GuestMobile = detailInfo.GuestMobile,
                Rate = detailInfo.Rate,
                Roomid = originDetail.Roomid,
                RoomNo = originDetail.RoomNo,
                ResStatus = originDetail.ResStatus,
                RecStatus = originDetail.RecStatus,
            };
            if (originDetail.RoomTypeid != resDetail.RoomTypeid)
            {
                resDetail.Roomid = "";
                resDetail.RoomNo = "";
            }
            if (!string.IsNullOrWhiteSpace(resDetail.Roomid) && !string.IsNullOrWhiteSpace(resDetail.RoomNo))
            {
                resDetail.RoomQty = 1;
            }
            if (resDetail == null || originDetail == null || resDetail.Regid != originDetail.Regid)
            {
                throw new Exception("表单提交的子单参数为空 或 子单主键ID不一致");
            }
            if (originDetail.Status == ResDetailStatus.I.ToString())
            {
                resDetail.ArrDate = originDetail.ArrDate;
            }
            else if (originDetail.Status == ResDetailStatus.R.ToString() || originDetail.Status == ResDetailStatus.N.ToString())
            {
                if (resDetail.ArrDate <= DateTime.Now)
                {
                    throw new Exception("预订抵店时间必须大于当前时间");
                }
            }
            else
            {
                throw new Exception("只能维护预订或入住");
            }
            List<string> needUpdateFieldNames = new List<string> { "ResStatus", "RecStatus", "RoomTypeid", "Sourceid", "Marketid", "RoomQty", "ArrDate", "DepDate", "RateCode", "Bbf", "HoldDate", "Remark", "Spec", "Profileid", "Guestid", "Guestname", "GuestMobile", "Rate", "Roomid", "RoomNo" };
            if (detailInfo.Status == ResDetailStatus.N.ToString() && originDetail.Status == ResDetailStatus.N.ToString())
            {
                ////NoShwow状态改为预订
                resDetail.IsSettle = false;
                resDetail.ResStatus = ResDetailStatus.R.ToString();
                resDetail.RecStatus = null;
                needUpdateFieldNames.AddRange(new List<string> { "IsSettle" });
            }
            CRUDService<ResDetail>.Update<ResDetail>(_pmsContext, resDetail, originDetail, needUpdateFieldNames);
            //修改客人
            var regInfoIds = new List<string>();
            foreach (var regInfo in detailInfo.OrderDetailRegInfos)
            {
                if (string.IsNullOrWhiteSpace(regInfo.OriginRegInfoJsonData))
                {
                    var id = AddResDetailRegInfo(regInfo, resDetail.Regid, currentInfo);
                    detailInfo.SelectCustomerId = id.ToString();
                    regInfo.Id = id.ToString();
                }
                else
                {
                    regInfoIds.Add(regInfo.Id);
                    var originRegInfo = jsSerializer.Deserialize<RegInfo>(regInfo.OriginRegInfoJsonData);
                    UpdateResDetailRegInfo(regInfo, originRegInfo);
                }
            }
            //还需要删除那些已经在数据里面了，但是本次没有传递回来的那些客人信息，那些表示操作员在前端删除了
            var hid = currentInfo.HotelId;
            var deletedRegInfos = _pmsContext.RegInfos.Where(w => w.Hid == hid && w.Regid == detailInfo.Regid).ToList();
            foreach (var deleted in deletedRegInfos)
            {
                if (!regInfoIds.Contains(deleted.Id.ToString()))
                {
                    _pmsContext.RegInfos.Remove(deleted);
                }
            }
            //修改房价（价格计划始终是先删除原来的，再重新添加）
            var oldDetailPlans = _pmsContext.ResDetailPlans.Where(w => w.Regid == resDetail.Regid && w.Ratedate >= businessDate).ToList();
            _pmsContext.ResDetailPlans.RemoveRange(oldDetailPlans);
            if (detailInfo.OrderDetailPlans != null && detailInfo.OrderDetailPlans.Count > 0)
            {
                foreach (var price in detailInfo.OrderDetailPlans)
                {
                    var ratedate = DateTime.Parse(price.Ratedate);
                    if (ratedate >= businessDate && ratedate <= resDetail.DepDate)
                    {
                        AddResDetailPlan(price, resDetail.Regid);
                    }
                }
            }
            //价格代码是否改变
            if (originDetail.Status == ResDetailStatus.I.ToString() && originDetail.RateCode != resDetail.RateCode)
            {
                msg = "价格代码已改变，如需调整离店时间请用延期功能。\n";
            }
        }

        #region 增加价格计划
        private void AddResDetailPlan(ResDetailPlanInfo price, string regid)
        {
            if(price != null || price.Price != null && price.Price.HasValue)
            {
                var plan = new ResDetailPlan
                {
                    Regid = regid,
                    Ratedate = Convert.ToDateTime(price.Ratedate),
                    Price = price.Price,
                    OriginPrice = price.OriginPrice,
                };
                _pmsContext.ResDetailPlans.Add(plan);
            }
        }
        #endregion

        #region 增加修改客人信息
        private Guid AddResDetailRegInfo(ResDetailRegInfo regInfo, string regid, ICurrentInfo currentInfo)
        {
            var photo = UploadPhoto(currentInfo.HotelId,regInfo.PhotoUrl);
            var reg = new RegInfo
            {
                Id = Guid.NewGuid(),
                Hid = currentInfo.HotelId,
                Regid = regid,
                GuestName = regInfo.GuestName,
                Cerid = regInfo.CerId,
                CerType = regInfo.CerType,
                Gender = regInfo.Gender,
                City = regInfo.City,
                Address = regInfo.Address,
                Mobile = regInfo.Mobile,
                Qq = regInfo.Qq,
                Email = regInfo.Email,
                IsMast = regInfo.IsMast,
                Interest = regInfo.Interest,
                CarNo = regInfo.CarNo,
                Nation = regInfo.Nation,
                PhotoUrl = photo,
            };
            DateTime birthDay;
            if (!string.IsNullOrEmpty(regInfo.Birthday) && DateTime.TryParse(regInfo.Birthday, out birthDay))
            {
                reg.Birthday = birthDay;
            }
            _pmsContext.RegInfos.Add(reg);
            return reg.Id;
        }
        private void UpdateResDetailRegInfo(ResDetailRegInfo regInfo, RegInfo originRegInfo)
        {
            var photo = UploadPhoto(originRegInfo.Hid,regInfo.PhotoUrl);
            var reg = new RegInfo
            {
                Id = Guid.Parse(regInfo.Id),
                Regid = regInfo.RegId,
                GuestName = regInfo.GuestName,
                Cerid = regInfo.CerId,
                CerType = regInfo.CerType,
                Gender = regInfo.Gender,
                City = regInfo.City,
                Address = regInfo.Address,
                Mobile = regInfo.Mobile,
                Qq = regInfo.Qq,
                Email = regInfo.Email,
                IsMast = regInfo.IsMast,
                Interest = regInfo.Interest,
                CarNo = regInfo.CarNo,
                Nation = regInfo.Nation,
                PhotoUrl = photo,
            };
            DateTime birthDay;
            if (!string.IsNullOrEmpty(regInfo.Birthday) && DateTime.TryParse(regInfo.Birthday, out birthDay))
            {
                reg.Birthday = birthDay;
            }
            if (reg != null && originRegInfo != null && reg.Id == originRegInfo.Id)
            {
                CRUDService<RegInfo>.Update<RegInfo>(_pmsContext, reg, originRegInfo, new List<string> { "GuestName", "Cerid", "CerType", "Gender", "City", "Address", "Mobile", "Qq", "Email", "IsMast", "Interest", "CarNo", "Birthday", "Nation", "PhotoUrl" });
            }
        }
        //上传身份证照片
        private string UploadPhoto(string hid,string PhotoBase64)
        {
            try
            {
                if (!DependencyResolver.Current.GetService<IPmsParaService>().IsScanSavePhoto(hid))//是否启用扫描身份证保存照片功能
                {
                    return "";
                }
                if (!string.IsNullOrWhiteSpace(PhotoBase64) && PhotoBase64.StartsWith("data:image/bmp;base64,"))
                {                    
                    var photo = PhotoBase64.Replace("data:image/bmp;base64,", "");
                    byte[] data = Convert.FromBase64String(photo);
                    var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                    var qiniuPara = _sysParaService.GetQiniuPara();
                    string bucket = (qiniuPara.ContainsKey("bucket") ? qiniuPara["bucket"] : "jxd-res");
                    string access_key = qiniuPara.ContainsKey("access_key") ? qiniuPara["access_key"] : "7TVp7dAC9xHLtd8VHPnHjAJOy9YLh7rhwbzZe7s2";
                    string secret_key = qiniuPara.ContainsKey("secret_key") ? qiniuPara["secret_key"] : "Ic96Wia-MQ4T2ma1wQfeqG_zlj1aRMhnZTeIsMGg";
                    string saveKey = Guid.NewGuid().ToString() + ".bmp";
                    QiniuHelper.UploadFile(bucket, access_key, secret_key, data, saveKey);
                    return "http://res.gshis.com/" + saveKey;
                }
                else
                {
                    if (PhotoBase64.Length > 200)
                    {
                        return "";
                    }
                    else
                    {
                        return PhotoBase64;
                    }
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion
        #endregion

        #region 检查房间的可用性
        /// <summary>
        /// 根据文本值获取枚举状态值
        /// </summary>
        /// <param name="text">文本值</param>
        /// <returns></returns>
        private ResDetailStatus GetStatus(string text)
        {
            switch (text)
            {
                case "C":
                    return ResDetailStatus.C;
                case "I":
                    return ResDetailStatus.I;
                case "N":
                    return ResDetailStatus.N;
                case "O":
                    return ResDetailStatus.O;
                case "R":
                    return ResDetailStatus.R;
                case "X":
                    return ResDetailStatus.X;
                case "Z":
                    return ResDetailStatus.Z;
            }
            return ResDetailStatus.Z;
        }
        /// <summary>
        /// UpRoomEnableCheck方法checkInfos参数值
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <param name="arrDate">抵店时间</param>
        /// <param name="depDate">离店时间</param>
        /// <param name="roomQty">房间数量</param>
        /// <param name="sourceId">客人来源</param>
        /// <returns></returns>
        private string UpRoomEnableCheck_checkInfo(string roomId, string roomTypeId, string arrDate, string depDate, string roomQty, string sourceId)
        {
            return string.Format("<room roomid=\"{0}\" roomtypeid=\"{1}\" arrdate=\"{2}\" depdate=\"{3}\" roomqty = \"{4}\" channelid=\"{5}\"></room>", roomId, roomTypeId, arrDate, depDate, roomQty, sourceId);
        }
        /// <summary>
        /// 检查房间的可用性
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regIds">子单ID列表</param>
        /// <param name="status">状态</param>
        /// <param name="checkInfos">xml格式，要检查的房间列表</param>
        /// <param name="saveContinue">是否继续保存，默认为0。在保存时根据情况，提示后，只有在操作员选择为继续保存时，才更改为1并且重新提交数据</param>
        /// <returns></returns>
        private JsonResultData UpRoomEnableCheck(string hid, List<string> regIds, ResDetailStatus status, List<string> checkInfos, string saveContinue)
        {
            string ids = "";
            if (regIds != null && regIds.Count > 0)
            {
                ids = string.Join(",", regIds.ToArray());
            }
            string checkinfo = "";
            if (checkInfos != null && checkInfos.Count > 0)
            {
                checkinfo = "<rooms>" + string.Join("", checkInfos.ToArray()) + "</rooms>";
            }
            var enableCheckResult = _pmsContext.Database.SqlQuery<UpRoomEnableCheckResult>(
                "exec up_roomEnableCheck @hid=@hid,@ids=@ids,@type=@type,@checkinfo=@checkinfo"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", ids)
                , new SqlParameter("@type", status.ToString())
                , new SqlParameter("@checkinfo", checkinfo)
                ).ToList();
            if (enableCheckResult.Count > 0)
            {
                //如果不是操作员选择继续保存，则直接返回
                if (saveContinue != "1")
                {
                    return JsonResultData.Failure(enableCheckResult);
                }
                else
                {
                    //如果是操作员选择继续保存，则要求结果中所有提示都是可以继续保存的，如果有不可以继续保存的，则仍然不能保存
                    bool canSave = true;
                    foreach (var check in enableCheckResult)
                    {
                        if (check.CanSave == 0)
                        {
                            canSave = false;
                            break;
                        }
                    }
                    if (!canSave)
                    {
                        return JsonResultData.Failure(enableCheckResult);
                    }
                }
            }
            return JsonResultData.Successed();
        }
        #endregion

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="regId">子单ID（定位使用：如果有值，则选中指定的子单。如果为空，则选中第一个子单）</param>
        /// <param name="customerId">随行人ID（定位使用：如果有值，则选中指定的随行人。如果为空，则选中第一个随行人）</param>
        /// <param name="regIdsAll">返回登记单ID列表</param>
        /// <returns></returns>
        private ResMainInfo GetResMainInfo(string hid, string resId, string regId, string customerId, out string regIdsAll)
        {
            var jsSerializer = new JavaScriptSerializer();
            //主单
            var main = _pmsContext.Reses.Where(w => w.Resid == resId && w.Hid == hid).AsNoTracking().SingleOrDefault();
            //主单发票
            //var invoiceInfo = _pmsContext.ResInvoiceInfos.Where(c => c.Resid == resId && c.Hid == hid).AsNoTracking().SingleOrDefault();
            //子单
            var details = _pmsContext.ResDetails.Where(w => w.Resid == resId && w.Hid == hid).AsNoTracking().ToList();
            //子单房价
            var regIds = details.Select(w => w.Regid).ToList();
            var detailPlans = _pmsContext.ResDetailPlans.Where(w => regIds.Contains(w.Regid)).AsNoTracking().ToList();
            //子单随行人
            var regInfos = _pmsContext.RegInfos.Where(w => regIds.Contains(w.Regid) && w.Hid == hid).AsNoTracking().ToList();
            //子单房间类型
            var roomTypeIds = details.Select(w => w.RoomTypeid).ToList();
            var roomTypes = _pmsContext.RoomTypes.Where(w => roomTypeIds.Contains(w.Id) && w.Hid == hid).Select(w => new { w.Id, w.Name, w.IsNotRoom }).AsNoTracking().ToList();
            //子单会员
            var profileIds = details.Select(w => w.Profileid).ToList();
            var profiles = _pmsContext.MbrCards.Where(w => profileIds.Contains(w.Id)).Select(w => new { w.Id, w.GuestName, w.MbrCardNo, w.Mobile }).AsNoTracking().ToList();
            //子单价格代码
            var rateCodeIds = details.Select(c => c.RateCode).Distinct().ToList();
            var rateCodes = _pmsContext.Rates.Where(c => c.Hid == hid && rateCodeIds.Contains(c.Id)).AsNoTracking().ToList();
            //登记单ID列表
            regIdsAll = String.Join(",", regIds);

            var result = new ResMainInfo();
            #region 主单
            if (main != null)
            {
                result.Resid = main.Resid;
                result.Resno = main.Resno;
                result.Name = main.Name;
                result.ResNoExt = main.ResNoExt;
                result.ResCustMobile = main.ResCustMobile;
                result.ResCustName = main.ResCustName;
                result.ResTime = main.ResTime == null ? "" : main.ResTime.ToDateTimeWithoutSecondString();
                result.IsGroup = main.IsGroup.HasValue ? main.IsGroup.Value : (byte)0;
                result.IsCustemSecret = Convert.ToByte(main.IsCustemSecret);
                result.IsHidePrice = Convert.ToByte(main.IsHidePrice);
                result.Cttid = main.Cttid == null ? null : main.Cttid;
                result.OriginResMainInfoJsonData = jsSerializer.Serialize(main);
                result.SelectRegId = regId;
                var extType = (details.FirstOrDefault() != null) ? (details.FirstOrDefault().ExtType ?? 0) : 0;
                result.ExtType = EnumExtension.GetDescription(typeof(ExtType), extType.ToString());
            }
            #endregion
            #region 主单发票
            //if (invoiceInfo != null)
            //{
            //    result.ResInvoiceInfo = new Services.ResManage.ResInvoiceInfo
            //    {
            //        Id = invoiceInfo.Id,
            //        InvoiceType = invoiceInfo.InvoiceType,
            //        TaxName = invoiceInfo.TaxName,
            //        TaxNo = invoiceInfo.TaxNo,
            //        TaxAddTel = invoiceInfo.TaxAddTel,
            //        TaxBankAccount = invoiceInfo.TaxBankAccount,
            //        OriginResInvoiceInfoJsonData = jsSerializer.Serialize(invoiceInfo)
            //    };
            //}
            #endregion

            foreach (var detail in details)
            {
                //房间类型
                string roomTypeName = "";
                bool roomTypeIsNotRoom = false;
                var roomType = roomTypes.SingleOrDefault(w => w.Id == detail.RoomTypeid);
                if (roomType != null)
                {
                    roomTypeName = roomType.Name;
                    roomTypeIsNotRoom = (roomType.IsNotRoom != null && roomType.IsNotRoom.HasValue) ? roomType.IsNotRoom.Value : false;
                }
                //会员
                string ProfileNo = "";
                string ProfileName = "";
                string ProfileMobile = "";
                if (detail.Profileid != null)
                {
                    var entity = profiles.SingleOrDefault(c => c.Id == detail.Profileid);
                    if (entity != null)
                    {
                        ProfileNo = entity.MbrCardNo;
                        ProfileName = entity.GuestName;
                        ProfileMobile = entity.Mobile;
                    }
                }
                //价格代码
                Rate rateCodeEntity = null;
                if (!string.IsNullOrWhiteSpace(detail.RateCode))
                {
                    var entity = rateCodes.SingleOrDefault(c => c.Id == detail.RateCode);
                    if (entity != null)
                    {
                        rateCodeEntity = entity;
                    }
                }
                //子单
                var detailInfo = new ResDetailInfo
                {
                    #region
                    Hid = detail.Hid,
                    Regid = detail.Regid,
                    Sourceid = detail.Sourceid,
                    Marketid = detail.Marketid,
                    RateCode = detail.RateCode,
                    RoomTypeId = detail.RoomTypeid,
                    RoomTypeName = roomTypeName,
                    RoomTypeIsNotRoom = roomTypeIsNotRoom,
                    Bbf = (byte)detail.Bbf,
                    ArrDate = detail.ArrDate.ToDateTimeString(),
                    DepDate = detail.DepDate.ToDateTimeString(),
                    HoldDate = detail.HoldDate.ToDateTimeString(),
                    Roomid = detail.Roomid,
                    RoomNo = detail.RoomNo,
                    OrderDetailPlans = detailPlans.Where(w => w.Regid == detail.Regid).Select(w => new ResDetailPlanInfo { Regid = w.Regid, Price = w.Price, Ratedate = w.Ratedate.ToDateString(), OriginPrice = w.OriginPrice }).ToList(),
                    Status = detail.Status,
                    StatusName = EnumExtension.GetDescription(typeof(ResDetailStatus), detail.Status),
                    ResStatus = detail.ResStatus,
                    RecStatus = detail.RecStatus,
                    Remark = detail.Remark,
                    Spec = detail.Spec,
                    Profileid = detail.Profileid,
                    ProfileNo = ProfileNo,
                    ProfileName = ProfileName,
                    ProfileMobile = ProfileMobile,
                    RoomQty = (int)detail.RoomQty,
                    OriginResDetailInfo = jsSerializer.Serialize(detail),
                    SelectCustomerId = detail.Regid == regId ? customerId : null,
                    Guestname = detail.Guestname,
                    Guestid = detail.Guestid,
                    RateCodeEntity = rateCodeEntity,
                    ArrBsnsDate = (detail.ArrBsnsDate != null && detail.ArrBsnsDate.HasValue ? detail.ArrBsnsDate.Value.ToDateString() : "")
                    #endregion
                };
                #region 子单随行人
                var detailRegInfos = regInfos.Where(w => w.Regid == detailInfo.Regid).OrderByDescending(c => c.IsMast).ToList();
                foreach (var reg in detailRegInfos)
                {
                    var regInfo = new ResDetailRegInfo
                    {
                        Id = reg.Id.ToString(),
                        RegId = reg.Regid,
                        GuestName = reg.GuestName,
                        CerType = reg.CerType,
                        CerId = reg.Cerid,
                        Gender = reg.Gender,
                        City = reg.City,
                        Address = reg.Address,
                        Birthday = reg.Birthday.ToDateString(),
                        Mobile = reg.Mobile,
                        Qq = reg.Qq,
                        Email = reg.Email,
                        Interest = reg.Interest,
                        CarNo = reg.CarNo,
                        OriginRegInfoJsonData = jsSerializer.Serialize(reg),
                        Nation = reg.Nation,
                        PhotoUrl = reg.PhotoUrl,
                    };
                    detailInfo.OrderDetailRegInfos.Add(regInfo);
                }
                #endregion
                result.ResDetailInfos.Add(detailInfo);
            }
            return result;
        }

        /// <summary>
        /// 获取分房 获取房号列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（R:预订，N:nowshow，X:预订取消，I:入住，O:离店迟付状态，C:离店且结帐）</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        public List<UpQueryRoomAutoChooseResult> GetRoomFor(string hid, ResDetailStatus type, string roomTypeId, DateTime arrDate, DateTime depDate, string regId)
        {
            List<UpQueryRoomAutoChooseResult> returnList = new List<UpQueryRoomAutoChooseResult>();
            returnList = UpRoomAutoChoose(hid, type, new List<string> { UpRoomAutoChoose_checkInfo("0", roomTypeId, arrDate.ToString(), depDate.ToString()) });
            if (!string.IsNullOrWhiteSpace(regId))
            {
                //获取子单
                var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid).AsNoTracking().FirstOrDefault();
                if (resDetailEntity != null && !string.IsNullOrWhiteSpace(resDetailEntity.Roomid) && !string.IsNullOrWhiteSpace(resDetailEntity.RoomNo))
                {
                    var entity = returnList.Where(c => c.Roomid == resDetailEntity.Roomid && c.roomno == resDetailEntity.RoomNo).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.remark = "已选择";
                    }
                    else
                    {
                        returnList.Add(new UpQueryRoomAutoChooseResult { remark = "已选择", Roomid = resDetailEntity.Roomid, roomno = resDetailEntity.RoomNo, seqid = 0 });
                    }
                }
            }
            return returnList;
        }
        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="regId">子单ID</param>
        /// <param name="type">类型（预订R还是入住I）</param>
        /// <returns></returns>
        public List<UpQueryRoomTypeChooseResult> GetRoomType(string hid, DateTime arrDate, DateTime depDate, string regId, ResDetailStatus type)
        {
            var procResults = _pmsContext.Database.SqlQuery<UpQueryRoomTypeChooseResult>("exec [dbo].[up_roomTypeChoose] @hid=@hid,@regid=@regid,@arrDate=@arrDate,@depDate=@depDate,@type=@type"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@arrDate", arrDate.ToString())
                , new SqlParameter("@depDate", depDate.ToString())
                , new SqlParameter("@regid", string.IsNullOrWhiteSpace(regId) ? "" : regId)
                , new SqlParameter("@type", type.ToString())
                ).ToList();
            if (procResults == null)
            {
                procResults = new List<UpQueryRoomTypeChooseResult>();
            }
            return procResults;
        }
        /// <summary>
        /// 获取房间类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="arrDate">开始时间</param>
        /// <param name="depDate">结束时间</param>
        /// <param name="rateId">价格码id</param>
        /// <param name="type">类型（预订R还是入住I）</param>
        /// <returns></returns>
        public List<UpQueryRoomTypeChooseResult> GetRoomTypeForRate(string hid, DateTime arrDate, DateTime depDate, string rateId, ResDetailStatus type)
        {
            var procResults = _pmsContext.Database.SqlQuery<UpQueryRoomTypeChooseResult>("exec [dbo].[up_roomTypeChoose] @hid=@hid,@regid='',@arrDate=@arrDate,@depDate=@depDate,@rateid=@rateid,@type=@type"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@arrDate", arrDate.ToString())
                , new SqlParameter("@depDate", depDate.ToString())
                , new SqlParameter("@rateid", string.IsNullOrWhiteSpace(rateId) ? "" : rateId)
                , new SqlParameter("@type", type.ToString())
                ).ToList();
            return procResults;
        }
        /// <summary>
        /// 关联更新 同一个主单内的 同价格代码并且同房间类型并且在住或预订的 订单的价格
        /// </summary>
        /// <param name="CurrentInfo">登录信息</param>
        /// <param name="regid">订单ID</param>
        /// <param name="businessDate">营业日</param>
        public bool RelationUpdateRate(ICurrentInfo CurrentInfo, string regid, DateTime businessDate, out string message)
        {
            string hid = CurrentInfo.HotelId;
            bool isSuccess = false;
            message = "";
            try
            {
                var regEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid && (c.Status == "I" || c.Status == "R")).Select(c => new { c.Resid, c.Regid, c.RateCode, c.RoomTypeid }).AsNoTracking().FirstOrDefault();
                if (regEntity == null || string.IsNullOrWhiteSpace(regEntity.Resid) || string.IsNullOrWhiteSpace(regEntity.RateCode) || string.IsNullOrWhiteSpace(regEntity.RoomTypeid))
                {
                    return isSuccess;
                }

                var regPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == regEntity.Regid && c.Ratedate >= businessDate).AsNoTracking().ToList();
                if (regPlans == null || regPlans.Count <= 0)
                {
                    return isSuccess;
                }

                var regIds = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == regEntity.Resid && c.Regid != regEntity.Regid && c.RateCode == regEntity.RateCode && c.RoomTypeid == regEntity.RoomTypeid && (c.Status == "I" || c.Status == "R")).Select(c => c.Regid).AsNoTracking().ToList();
                if (regIds == null || regIds.Count <= 0)
                {
                    return isSuccess;
                }

                var list = _pmsContext.ResDetailPlans.Where(c => regIds.Contains(c.Regid) && c.Ratedate >= businessDate).ToList();
                if (list == null || list.Count <= 0)
                {
                    return isSuccess;
                }

                string ip = UrlHelperExtension.GetRemoteClientIPAddress();
                List<string> updateRegids = new List<string>();
                foreach (var item in regPlans)
                {
                    var temp = list.Where(c => c.Ratedate == item.Ratedate).ToList();
                    foreach (var itemTemp in temp)
                    {
                        if(itemTemp.Price != item.Price)
                        {
                            _pmsContext.ResLogs.Add(new ResLog { Hid = hid, Regid = itemTemp.Regid, XType = 1, Value1 = (itemTemp.Price.HasValue ? itemTemp.Price.ToString() : ""), Value2 = (item.Price.HasValue ? item.Price.ToString() : ""), Other1 = "", Other2 = itemTemp.Ratedate.ToShortDateString(), CDate = DateTime.Now, CUser = CurrentInfo.UserName, Ip = ip, Id = Guid.NewGuid() });

                            itemTemp.Price = item.Price;
                            _pmsContext.Entry(itemTemp).State = EntityState.Modified;
                            updateRegids.Add(itemTemp.Regid);
                        }
                    }
                }
                if (updateRegids.Count > 0)
                {
                    _pmsContext.SaveChanges();
                    updateRegids = updateRegids.Distinct().ToList();
                    #region 更新当前营业日房价
                    bool isEdit = false;
                    var resDetails = _pmsContext.ResDetails.Where(c => c.Hid == hid && updateRegids.Contains(c.Regid)).ToList();
                    if(resDetails != null && resDetails.Count > 0)
                    {
                        foreach(var item in resDetails)
                        {
                            var price = GetCurrentResDetailPrice(item.Regid, businessDate);
                            if(price != null && item.Rate != price)
                            {
                                item.Rate = price;
                                _pmsContext.Entry(item).State = EntityState.Unchanged;
                                _pmsContext.Entry(item).Property("Rate").IsModified = true;
                                isEdit = true;
                            }
                        }
                    }
                    if (isEdit)
                    {
                        _pmsContext.SaveChanges();
                    }
                    #endregion
                    isSuccess = true;
                    for(int i = 0; i < updateRegids.Count; i++)
                    {
                        updateRegids[i] = updateRegids[i].StartsWith(hid) ? updateRegids[i].Substring(hid.Length) : updateRegids[i];
                    }
                    message = string.Format("关联更新了账号：{0}的价格！\n", string.Join(",", updateRegids));
                }
            }
            catch{ }
            return isSuccess;
        }
        /// <summary>
        /// 关联更新 同一个主单内的 在住或预订的 订单的备注
        /// </summary>
        /// <param name="CurrentInfo">登录信息</param>
        /// <param name="regid">账号ID</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public bool RelationUpdateRemark(ICurrentInfo CurrentInfo, string regid, out string message)
        {
            string hid = CurrentInfo.HotelId;
            bool isSuccess = false;
            message = "";
            try
            {
                var regEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid && (c.Status == "I" || c.Status == "R")).Select(c => new { c.Resid, c.Remark }).AsNoTracking().FirstOrDefault();
                if (regEntity == null || string.IsNullOrWhiteSpace(regEntity.Resid))
                {
                    return isSuccess;
                }

                var regList = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == regEntity.Resid && c.Regid != regid && (c.Status == "I" || c.Status == "R")).ToList();
                if (regList == null || regList.Count <= 0)
                {
                    return isSuccess;
                }

                List<string> updateRegids = new List<string>();
                foreach (var item in regList)
                {
                    if(item.Remark != regEntity.Remark)
                    {
                        string shotRegid = (item.Regid.StartsWith(hid) ? item.Regid.Substring(hid.Length) : item.Regid);
                        if (!updateRegids.Contains(shotRegid))
                        {
                            updateRegids.Add(shotRegid);
                        }
                        AddOperationLog(CurrentInfo, BSPMS.Common.Services.Enums.OpLogType.客单备注修改, string.Format("账号：{0}，原备注：{1}，新备注：{2}。", shotRegid, item.Remark, regEntity.Remark), item.Regid);

                        item.Remark = regEntity.Remark;
                        _pmsContext.Entry(item).State = EntityState.Modified;
                    }
                }
                if (updateRegids.Count > 0)
                {
                    _pmsContext.SaveChanges();
                    isSuccess = true;
                    message = string.Format("关联更新了账号：{0}的备注！\n", string.Join(",", updateRegids));
                }
            }
            catch { }
            return isSuccess;
        }
        #endregion

        #region 内部使用 获取可用房
        /// <summary>
        /// UpRoomAutoChoose方法checkInfos参数值
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <param name="arrDate">抵店时间</param>
        /// <param name="depDate">离店时间</param>
        /// <param name="roomQty">房间数量</param>
        /// <param name="sourceId">客人来源</param>
        /// <returns></returns>
        private string UpRoomAutoChoose_checkInfo(string seqid, string roomTypeId, string arrDate, string depDate)
        {
            return string.Format("<room seqid=\"{0}\" roomtypeid=\"{1}\" arrdate=\"{2}\" depdate=\"{3}\"></room>", seqid, roomTypeId, arrDate, depDate);
        }

        /// <summary>
        /// 执行分房存储过程，获取可用的房间数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（R:预订，N:nowshow，X:预订取消，I:入住，O:离店迟付状态，C:离店且结帐）</param>
        /// <param name="checkInfos">UpRoomAutoChoose_checkInfo方法</param>
        /// <returns></returns>
        private List<UpQueryRoomAutoChooseResult> UpRoomAutoChoose(string hid, ResDetailStatus type, List<string> checkInfos)
        {
            string checkinfo = "";
            if (checkInfos != null && checkInfos.Count > 0)
            {
                checkinfo = "<rooms>" + string.Join("", checkInfos.ToArray()) + "</rooms>";
            }
            return UpRoomAutoChoose(hid, type, checkinfo);
        }

        /// <summary>
        /// 执行分房存储过程，获取可用的房间数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（R:预订，N:nowshow，X:预订取消，I:入住，O:离店迟付状态，C:离店且结帐）</param>
        /// <param name="checkInfos">xml字符串</param>
        /// <returns></returns>
        private List<UpQueryRoomAutoChooseResult> UpRoomAutoChoose(string hid, ResDetailStatus type, string checkInfos)
        {
            var procResults = _pmsContext.Database.SqlQuery<UpQueryRoomAutoChooseResult>("exec up_roomAutochoose @hid=@hid,@type=@type,@checkinfo=@checkinfo"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@type", type.ToString())
                , new SqlParameter("@checkinfo", checkInfos)
                ).ToList();
            if (procResults == null)
            {
                procResults = new List<UpQueryRoomAutoChooseResult>();
            }
            return procResults;
        }
        #endregion

        #region（取消、恢复）子单
        /// <summary>
        /// 取消子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        public JsonResultData CancelOrderDetail(string hid, bool isEnvTest, string regId, string saveContinue, out string opLog)
        {
            opLog = null;
            try
            {
                #region 验证
                if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regId))
                {
                    return JsonResultData.Failure("参数错误");
                }
                opLog = "账号：" + regId.Replace(hid, "") + "，状态：";
                var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regId).SingleOrDefault();
                if (resDetailEntity == null)
                {
                    return JsonResultData.Failure("账号不存在");
                }
                if (resDetailEntity.Status != ResDetailStatus.R.ToString() && resDetailEntity.Status != ResDetailStatus.I.ToString())
                {
                    return JsonResultData.Failure("此订单状态内不能执行取消操作");
                }
                opLog += EnumExtension.GetDescription(typeof(ResDetailStatus), resDetailEntity.Status);
                #endregion

                #region 修改状态
                //ResDetailStatus newStatus = ResDetailStatus.Z;
                //if (resDetailEntity.Status == ResDetailStatus.R.ToString())//预订状态
                //{
                //    if (resDetailEntity.ResStatus == ResDetailStatus.R.ToString() && string.IsNullOrWhiteSpace(resDetailEntity.RecStatus))
                //    {
                //        //直接预订，没有其他操作。最后结果是：预订状态
                //        resDetailEntity.ResStatus = ResDetailStatus.X.ToString();
                //        newStatus = ResDetailStatus.X;
                //    }
                //    else if (resDetailEntity.ResStatus == ResDetailStatus.R.ToString() && resDetailEntity.RecStatus == ResDetailStatus.Z.ToString())
                //    {
                //        //先预订，再入住，再取消入住。最后结果是：预订状态
                //        resDetailEntity.ResStatus = ResDetailStatus.X.ToString();
                //        newStatus = ResDetailStatus.X;
                //    }
                //}
                //else if(resDetailEntity.Status == ResDetailStatus.I.ToString())//在住状态
                //{
                //    if(string.IsNullOrWhiteSpace(resDetailEntity.ResStatus) && resDetailEntity.RecStatus == ResDetailStatus.I.ToString())
                //    {
                //        //直接入住，没有其他操作。最后结果是：在住状态
                //        resDetailEntity.RecStatus = ResDetailStatus.X.ToString();
                //        newStatus = ResDetailStatus.X;
                //    }
                //    else if (resDetailEntity.ResStatus == ResDetailStatus.R.ToString() && resDetailEntity.RecStatus == ResDetailStatus.I.ToString())
                //    {
                //        //先预订，在入住。最后结果是：在住状态
                //        resDetailEntity.RecStatus = ResDetailStatus.Z.ToString();
                //        newStatus = ResDetailStatus.R;
                //    }
                //}
                //if(newStatus == ResDetailStatus.Z)
                //{
                //    return JsonResultData.Failure("此订单状态内不能执行取消操作");
                //}
                #endregion

                //房间可用性
                //JsonResultData checkResult = UpRoomEnableCheck(hid, new List<string> { regId }, newStatus, new List<string> { UpRoomEnableCheck_checkInfo(resDetailEntity.Roomid, resDetailEntity.RoomTypeid, resDetailEntity.ArrDate.ToString(), resDetailEntity.DepDate.ToString(), resDetailEntity.RoomQty.ToString(), resDetailEntity.Sourceid) }, saveContinue);
                //if (!checkResult.Success)
                //{
                //    return checkResult;
                //}
                //保存
                //_pmsContext.Entry(resDetailEntity).State = EntityState.Modified;
                //_pmsContext.SaveChanges();
                try
                {
                    _pmsContext.Database.ExecuteSqlCommand("exec up_resDetail_cancel @hid=@hid,@regid=@regid", new SqlParameter("@hid", hid), new SqlParameter("@regid", regId));
                }
                catch (Exception ex)
                {
                    return JsonResultData.Failure(ex);
                }

                var regIdsAll = "";
                var result = GetResMainInfo(hid, resDetailEntity.Resid, resDetailEntity.Regid, null, out regIdsAll);
                #region 调用存储过程更改房态
                _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@ids", regIdsAll)
                    );
                #endregion

                #region 订单状态变更通知
                _notifyService.NotifyOtaResChanged(hid, isEnvTest, resDetailEntity.Sourceid, resDetailEntity.Resid);
                #endregion

                if (result != null && result.ResDetailInfos != null && result.ResDetailInfos.Count > 0)
                {
                    var temp = result.ResDetailInfos.Where(c => c.Regid == regId && c.Hid == hid).FirstOrDefault();
                    if (temp != null)
                    {
                        opLog += "=>" + temp.StatusName;
                    }
                }
                return JsonResultData.Successed(result);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 恢复子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        public JsonResultData RecoveryOrderDetail(string hid, bool isEnvTest, string regId, string saveContinue, out string opLog)
        {
            opLog = null;
            try
            {
                #region 验证
                if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regId))
                {
                    return JsonResultData.Failure("参数错误");
                }
                opLog = "账号：" + regId.Replace(hid, "") + "，状态：";
                var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regId).SingleOrDefault();
                if (resDetailEntity == null)
                {
                    return JsonResultData.Failure("账号不存在");
                }
                if (resDetailEntity.Status != ResDetailStatus.X.ToString() && resDetailEntity.Status != ResDetailStatus.R.ToString())
                {
                    return JsonResultData.Failure("此订单状态内不能执行恢复操作");
                }
                opLog += EnumExtension.GetDescription(typeof(ResDetailStatus), resDetailEntity.Status);
                #endregion

                #region 修改状态
                ResDetailStatus newStatus = ResDetailStatus.Z;

                if (resDetailEntity.ResStatus == ResDetailStatus.X.ToString() && string.IsNullOrWhiteSpace(resDetailEntity.RecStatus))
                {
                    //直接预订，再取消预订。最后结果是：取消状态
                    resDetailEntity.ResStatus = ResDetailStatus.R.ToString();
                    newStatus = ResDetailStatus.R;
                }
                else if (resDetailEntity.ResStatus == ResDetailStatus.X.ToString() && resDetailEntity.RecStatus == ResDetailStatus.Z.ToString())
                {
                    //先预订，再入住，再取消入住，再取消预订。最后结果是：取消状态
                    resDetailEntity.ResStatus = ResDetailStatus.R.ToString();
                    newStatus = ResDetailStatus.R;
                }
                else if (string.IsNullOrWhiteSpace(resDetailEntity.ResStatus) && resDetailEntity.RecStatus == ResDetailStatus.X.ToString())
                {
                    //直接入住，再取消入住。最后结果是：取消状态
                    resDetailEntity.RecStatus = ResDetailStatus.I.ToString();
                    newStatus = ResDetailStatus.I;
                }
                else if (resDetailEntity.ResStatus == ResDetailStatus.R.ToString() && resDetailEntity.RecStatus == ResDetailStatus.Z.ToString())
                {
                    //先预订，再入住，再取消入住。最后结果是：预订状态
                    resDetailEntity.RecStatus = ResDetailStatus.I.ToString();
                    newStatus = ResDetailStatus.I;
                }
                if (newStatus == ResDetailStatus.Z)
                {
                    return JsonResultData.Failure("此订单状态内不能执行恢复操作");
                }
                #endregion

                //房间可用性
                JsonResultData checkResult = UpRoomEnableCheck(hid, new List<string> { regId }, newStatus, new List<string> { UpRoomEnableCheck_checkInfo(resDetailEntity.Roomid, resDetailEntity.RoomTypeid, resDetailEntity.ArrDate.ToString(), resDetailEntity.DepDate.ToString(), resDetailEntity.RoomQty.ToString(), resDetailEntity.Sourceid) }, saveContinue);
                if (!checkResult.Success)
                {
                    return checkResult;
                }
                //保存
                _pmsContext.Entry(resDetailEntity).State = EntityState.Modified;
                _pmsContext.SaveChanges();
                var regIdsAll = "";
                var result = GetResMainInfo(hid, resDetailEntity.Resid, resDetailEntity.Regid, null, out regIdsAll);
                #region 调用存储过程更改房态
                _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                    , new SqlParameter("@hid", hid)
                    , new SqlParameter("@ids", regIdsAll)
                    );
                #endregion
                #region 订单状态变更通知
                _notifyService.NotifyOtaResChanged(hid, isEnvTest, resDetailEntity.Sourceid, resDetailEntity.Resid);
                #endregion
                if (result != null && result.ResDetailInfos != null && result.ResDetailInfos.Count > 0)
                {
                    var temp = result.ResDetailInfos.Where(c => c.Regid == regId && c.Hid == hid).FirstOrDefault();
                    if (temp != null)
                    {
                        opLog += "=>" + temp.StatusName;
                    }
                }
                return JsonResultData.Successed(result);
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 批量取消或恢复订单
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="resId">主单ID</param>
        /// <param name="type">类型（CancelR：取消预订，CancelI：取消入住，RecoveryR：恢复预订，RecoveryI：恢复入住）</param>
        /// <param name="regIds">子单ID列表</param>
        /// <returns></returns>
        public JsonResultData BatchCancelAndRecoveryOrderDetail(ICurrentInfo currentInfo, string resId, string type, List<string> regIds, bool isEnvTest)
        {
            string hid = currentInfo.HotelId;
            //验证参数
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(resId) || string.IsNullOrWhiteSpace(type) || regIds == null || regIds.Count <= 0)
            {
                return JsonResultData.Failure("参数不正确！");
            }
            if (type != "CancelR" && type != "CancelI" && type != "RecoveryR" && type != "RecoveryI")
            {
                return JsonResultData.Failure("参数不正确！");
            }
            //获取订单
            var list = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == resId && regIds.Contains(c.Regid)).ToList();
            if(list == null || list.Count <= 0 || list.Count != regIds.Count)
            {
                return JsonResultData.Failure("参数不正确！");
            }
            List<KeyValuePairModel<string, string>> resultList = new List<KeyValuePairModel<string, string>>();
            List<KeyValuePairModel<string, string>> logList = new List<KeyValuePairModel<string, string>>();
            //循环执行
            foreach (var resDetailEntity in list)
            {
                try
                {
                    logList.Add(new KeyValuePairModel<string, string>(resDetailEntity.Regid, EnumExtension.GetDescription(typeof(ResDetailStatus), resDetailEntity.Status)));
                    if (type == "CancelR" || type == "CancelI")//取消订单
                    {
                        if (resDetailEntity.Status != ResDetailStatus.R.ToString() && resDetailEntity.Status != ResDetailStatus.I.ToString())
                        {
                            throw new Exception("此订单状态内不能执行取消操作");
                        }
                        _pmsContext.Database.ExecuteSqlCommand("exec up_resDetail_cancel @hid=@hid,@regid=@regid", new SqlParameter("@hid", hid), new SqlParameter("@regid", resDetailEntity.Regid));
                        resultList.Add(new KeyValuePairModel<string, string>(resDetailEntity.Regid, "成功"));
                    }
                    else if (type == "RecoveryR" || type == "RecoveryI")//恢复订单
                    {
                        if (resDetailEntity.Status != ResDetailStatus.X.ToString() && resDetailEntity.Status != ResDetailStatus.R.ToString())
                        {
                            throw new Exception("此订单状态内不能执行恢复操作");
                        }
                        #region 修改状态
                        ResDetailStatus newStatus = ResDetailStatus.Z;
                        if (resDetailEntity.ResStatus == ResDetailStatus.X.ToString() && string.IsNullOrWhiteSpace(resDetailEntity.RecStatus))
                        {
                            //直接预订，再取消预订。最后结果是：取消状态
                            resDetailEntity.ResStatus = ResDetailStatus.R.ToString();
                            newStatus = ResDetailStatus.R;
                        }
                        else if (resDetailEntity.ResStatus == ResDetailStatus.X.ToString() && resDetailEntity.RecStatus == ResDetailStatus.Z.ToString())
                        {
                            //先预订，再入住，再取消入住，再取消预订。最后结果是：取消状态
                            resDetailEntity.ResStatus = ResDetailStatus.R.ToString();
                            newStatus = ResDetailStatus.R;
                        }
                        else if (string.IsNullOrWhiteSpace(resDetailEntity.ResStatus) && resDetailEntity.RecStatus == ResDetailStatus.X.ToString())
                        {
                            //直接入住，再取消入住。最后结果是：取消状态
                            resDetailEntity.RecStatus = ResDetailStatus.I.ToString();
                            newStatus = ResDetailStatus.I;
                        }
                        else if (resDetailEntity.ResStatus == ResDetailStatus.R.ToString() && resDetailEntity.RecStatus == ResDetailStatus.Z.ToString())
                        {
                            //先预订，再入住，再取消入住。最后结果是：预订状态
                            resDetailEntity.RecStatus = ResDetailStatus.I.ToString();
                            newStatus = ResDetailStatus.I;
                        }
                        if (newStatus == ResDetailStatus.Z)
                        {
                            throw new Exception("此订单状态内不能执行恢复操作");
                        }
                        #endregion
                        //房间可用性
                        JsonResultData checkResult = UpRoomEnableCheck(hid, new List<string> { resDetailEntity.Regid }, newStatus, new List<string> { UpRoomEnableCheck_checkInfo(resDetailEntity.Roomid, resDetailEntity.RoomTypeid, resDetailEntity.ArrDate.ToString(), resDetailEntity.DepDate.ToString(), resDetailEntity.RoomQty.ToString(), resDetailEntity.Sourceid) }, "1");
                        if (!checkResult.Success)
                        {
                            resultList.Add(new KeyValuePairModel<string, string> { Key = resDetailEntity.Regid, Value = "失败", Data = checkResult.Data });
                            continue;
                        }
                        //保存
                        _pmsContext.Entry(resDetailEntity).State = EntityState.Modified;
                        _pmsContext.SaveChanges();
                        resultList.Add(new KeyValuePairModel<string, string>(resDetailEntity.Regid, "成功"));
                    }
                }
                catch(Exception ex)
                {
                    resultList.Add(new KeyValuePairModel<string, string> { Key = resDetailEntity.Regid, Value = "失败", Data = ex.Message });
                }
            }

            #region 调用存储过程更改房态
            string regIdsAll = String.Join(",", list.Select(c => c.Regid).ToList());
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            #region 订单状态变更通知
            _notifyService.NotifyOtaResChanged(hid, isEnvTest, list[0].Sourceid, list[0].Resid);
            #endregion
            int hidLength = hid.Length;
            #region 日志
            var regIds_Success = resultList.Where(c => c.Value == "成功").Select(c => c.Key).ToList();
            if(regIds_Success != null && regIds_Success.Count > 0)
            {
                var newList = _pmsContext.ResDetails.AsNoTracking().Where(c => c.Hid == hid && regIds_Success.Contains(c.Regid)).Select(c => new { c.Regid, c.Status }).ToList();
                foreach(var item in newList)
                {
                    var old = logList.Where(c => c.Key == item.Regid).FirstOrDefault();
                    if(old != null)
                    {
                        old.Value = old.Value + "=>" + EnumExtension.GetDescription(typeof(ResDetailStatus), item.Status);
                    }
                }
            }
            if(logList != null && logList.Count > 0)
            {
                logList.RemoveAll(c => !c.Value.Contains("=>"));
                if(logList != null && logList.Count > 0)
                {
                    var groupLog = logList.GroupBy(g => g.Value).Select(s => new { RegIds = string.Join(",", s.Select(p => p.Key.Substring(hidLength))), Content = s.Key }).ToList();
                    if(groupLog != null && groupLog.Count > 0)
                    {
                        BSPMS.Common.Services.Enums.OpLogType logType = BSPMS.Common.Services.Enums.OpLogType.取消预订;
                        switch (type)
                        {
                            case "CancelR":
                                logType = BSPMS.Common.Services.Enums.OpLogType.取消预订;
                                break;
                            case "CancelI":
                                logType = BSPMS.Common.Services.Enums.OpLogType.取消入住;
                                break;
                            case "RecoveryR":
                                logType = BSPMS.Common.Services.Enums.OpLogType.恢复预订;
                                break;
                            case "RecoveryI":
                                logType = BSPMS.Common.Services.Enums.OpLogType.恢复入住;
                                break;
                        }
                        foreach (var item in groupLog)
                        {
                            AddOperationLog(currentInfo, logType, string.Format("账号：{0}，状态：{1}", item.RegIds, item.Content), logList[0].Key);
                        }
                        _pmsContext.SaveChanges();
                    }
                }
            }
            #endregion

            foreach (var item in resultList)
            {
                item.Key = item.Key.Substring(hidLength);
            }
            return JsonResultData.Successed(resultList);
        }
        #endregion

        #region 门卡
        /// <summary>
        /// 获取门锁卡信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <returns></returns>
        public List<ResDetailLockInfo> GetLockInfo(string hid, string resId)
        {
            //验证
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(resId))
            {
                throw new Exception("参数不正确");
            }
            //获取入住子单信息
            List<ResDetail> resDetailList = _pmsContext.ResDetails.Where(c => c.Resid == resId && c.Hid == hid).ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return new List<ResDetailLockInfo>();
            }
            string onlineLockType = null;
            var masterServiceObject = DependencyResolver.Current.GetService(typeof(IMasterService));
            var masterService = (masterServiceObject as IMasterService);
            if (masterService != null)
            {
                onlineLockType = masterService.GetHotelOnlineLockType(hid);
            }
            var ispwdlock = false;
            if(onlineLockType == "locstarLock" || onlineLockType == "danbay" || onlineLockType == "beda_wx")
            {
                ispwdlock = true;
            }
            //获取门锁卡信息
            var regidList = resDetailList.Select(c => c.Regid).ToList();
            List<LockLog> lockLogList = _pmsContext.LockLogs.Where(c => regidList.Contains(c.Regid) && c.Hid == hid).ToList();
            //获取房间信息
            var roomidList = resDetailList.Select(c => c.Roomid).ToList();
            var lockidList = _pmsContext.Rooms.Where(c => roomidList.Contains(c.Id) && c.Hid == hid && c.Status == EntityStatus.启用).Select(s => new { s.Id, s.Lockid }).ToList();
            //组合结果
            List<ResDetailLockInfo> resultList = new List<ResDetailLockInfo>();
            foreach (var item in resDetailList)
            {
                ResDetailLockInfo entity = new ResDetailLockInfo
                {
                    Hid = item.Hid,
                    RegId = item.Regid,
                    RoomNo = item.RoomNo,
                    RoomLockId = lockidList.Where(c => c.Id == item.Roomid).Select(c => c.Lockid).FirstOrDefault(),
                    ArrDate = item.ArrDate.Value.ToDateTimeWithoutSecondString(),
                    DepDate = item.DepDate.Value.ToDateTimeWithoutSecondString(),
                    LockCardList = new List<ResDetailLockDetailInfo>(),
                    IsWrite = (item.Status == "I" || (item.Status == "R" && item.Roomid != null && item.RoomNo != null && item.Roomid.Trim().Length > 0 && item.RoomNo.Trim().Length > 0)) ? true : false,
                    IsOnlineLock = ispwdlock,
                };
                var itemLockList = lockLogList.Where(c => c.Regid == item.Regid).OrderByDescending(c => c.CreateDate).ToList();
                foreach (var itemlock in itemLockList)
                {
                    entity.LockCardList.Add(new ResDetailLockDetailInfo
                    {
                        Id = itemlock.Seqid,
                        CardNo = itemlock.CardNo,
                        BeginDate = itemlock.BeginDate.ToDateTimeWithoutSecondString(),
                        EndDate = itemlock.EndDate.ToDateTimeWithoutSecondString(),
                        CreateDate = itemlock.CreateDate.ToDateTimeWithoutSecondString(),
                        Status = itemlock.Status,
                        StatusName = ((LockStatus)itemlock.Status).ToString(),
                        RoomNo = itemlock.Roomno,
                    });
                }
                resultList.Add(entity);
            }
            return resultList;
        }
        /// <summary>
        /// 检测此房间号是否有在住或预定的，并且不是此单号的发过卡，防止不同客人可以进同一房间
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="regid"></param>
        /// <returns></returns>
        public string QueryIsLockWrite(string hid,string regid)
        {
            var result = _pmsContext.Database.SqlQuery<string>("EXEC dbo.up_query_lock @hid=@hid, @regid = @regid", 
                new SqlParameter("@hid", hid),
                new SqlParameter("@regid", regid)
                ).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// 获取门锁写卡参数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="userName">操作人</param>
        /// <param name="regId">子单ID</param>
        /// <param name="cardNo">卡号</param>
        /// <returns></returns>
        public JsonResultData GetLockWriteCardPara(string hid, string userName, string regId, string cardNo)
        {
            //验证
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(regId))
            {
                throw new Exception("参数不正确");
            }
            //获取子单信息
            ResDetail resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid && (c.Status == "I" || (c.Status == "R" && c.Roomid != null && c.RoomNo != null && c.Roomid.Trim().Length > 0 && c.RoomNo.Trim().Length > 0))).SingleOrDefault();            
            if (resDetailEntity == null)
            {
                throw new Exception("参数不正确");
            }
            //获取房间信息
            var roomEntity = _pmsContext.Rooms.Where(c => c.Id == resDetailEntity.Roomid && c.Hid == hid && c.Status == EntityStatus.启用).SingleOrDefault();
            if (roomEntity == null)
            {
                throw new Exception("此订单房号信息错误");
            }
            if (string.IsNullOrWhiteSpace(roomEntity.Lockid))
            {
                throw new Exception("此订单房号门锁信息错误");
            }
            RegInfo reginfo = _pmsContext.RegInfos.Where(r => r.Regid == regId && r.Hid == hid).OrderByDescending(r => r.IsMast).FirstOrDefault();
            //判断此房间 是否第一张卡（房间新入住卡 还是 原有客人增加一张卡  0：增加卡 1:新客人卡）
            string IsNew = "1";
            string Seqid = "";//主键ID，空新卡，否则重写卡。
            string lockType = "";
            if (string.IsNullOrWhiteSpace(cardNo))
            {
                //发新卡
                if (_pmsContext.LockLogs.Where(c => c.Regid == resDetailEntity.Regid && c.Hid == hid && c.Status == 0).Any())
                {
                    IsNew = "0";
                }
            }
            else
            {
                //重写卡
                var lockEntity = _pmsContext.LockLogs.Where(c => c.Regid == resDetailEntity.Regid && c.Hid == hid && c.CardNo == cardNo && c.Status == 0).SingleOrDefault();
                if (lockEntity == null)
                {
                    throw new Exception("此订单门锁卡信息错误");
                }
                var mainLockEntity = _pmsContext.LockLogs.Where(c => c.Regid == resDetailEntity.Regid && c.Hid == hid).OrderBy(c => c.Seqid).FirstOrDefault();
                if (mainLockEntity.Seqid != lockEntity.Seqid)
                {
                    IsNew = "0";
                }
                Seqid = lockEntity.Seqid.ToString();
                if(!string.IsNullOrWhiteSpace(lockEntity.Locktype))
                {
                    lockType = lockEntity.Locktype;
                }
                
            }

            string seqNo = "0";//流水号,如果增加一张卡，要注意这个号码与之前那张卡要相同
            var lastCard = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.Roomno == resDetailEntity.RoomNo && c.Status == 0 && c.Locktype != "lockstar").OrderByDescending(c => c.CreateDate).AsNoTracking().FirstOrDefault();
            if (lastCard != null && !string.IsNullOrWhiteSpace(lastCard.SeqNo))
            {
                seqNo = lastCard.SeqNo;
            }

            string idx = "1";//客人第几张卡
            if (seqNo != "0")
            {
                idx = (_pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.SeqNo == seqNo).AsNoTracking().Count() + (string.IsNullOrWhiteSpace(Seqid) ? 1 : 0)).ToString();
            }

            string beginTime = "";//开始时间
            var firstCard = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.Roomno == resDetailEntity.RoomNo && c.Status == 0).OrderBy(c => c.CreateDate).AsNoTracking().FirstOrDefault();
            if (resDetailEntity.Status == "R")
            {
                //对于预订：第一张卡用预抵时间减10分钟，第二张卡用第一张时间(同一房号同一账号)，第三张也是用第一张卡
                if(firstCard == null)
                {
                    beginTime = DateTime.Now.ToDateTimeWithoutSecondString();
                }
                else
                {
                    beginTime = firstCard.BeginDate.ToDateTimeWithoutSecondString();
                }
            }
            else
            {
                //对于在住：第一张卡用当前时间-10分钟，第二张卡用第一张卡的时间(同一房号同一账号)
                if (firstCard == null)
                {
                    beginTime = DateTime.Now.ToDateTimeWithoutSecondString();
                }
                else
                {
                    beginTime = firstCard.BeginDate.ToDateTimeWithoutSecondString();
                }
            }

            //组织结果
            var result = new
            {
                LockId = roomEntity.Lockid,
                LockInfo = roomEntity.LockInfo,
                BeginTime = beginTime,
                EndTime = resDetailEntity.DepDate.ToDateTimeWithoutSecondString(),
                GuestName = resDetailEntity.Guestname,
                GuestMobile = reginfo.Mobile,
                GuestCerId = reginfo.Cerid,
                IsNew = IsNew,
                SeqNo = seqNo,
                UserName = userName,
                Seqid = Seqid,
                IsRes = (resDetailEntity.Status == "R") ? "1" : "0",
                Idx = idx,
                OnLockType = lockType,
            };
            return JsonResultData.Successed(result);
        }

        /// <summary>
        /// 写卡
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="regId">子单ID</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="seqId">门锁表主键ID（有值：重写卡，无值：发新卡）</param>
        public void WriteLock(ICurrentInfo currentInfo, string regId, string cardNo, string seqId, string seqNo,string lockType)
        {
            string hid = currentInfo.HotelId;
            string inputUser = currentInfo.UserName;
            //验证
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(cardNo))
            {
                throw new Exception("参数不正确");
            }
            //获取子单信息
            ResDetail resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid && (c.Status == "I" || (c.Status == "R" && c.Roomid != null && c.RoomNo != null && c.Roomid.Trim().Length > 0 && c.RoomNo.Trim().Length > 0))).SingleOrDefault();
            if (resDetailEntity == null)
            {
                throw new Exception("参数不正确");
            }
            //验证卡号是否已存在
            if (lockType != "lockstar")
            {
                if (_pmsContext.LockLogs.Where(c => c.Hid == hid && c.CardNo == cardNo && c.Status == 0).Any())
                {
                    throw new Exception("此卡号已存在");
                }
            }
            string beginTime = "";//开始时间
            var firstCard = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.Roomno == resDetailEntity.RoomNo && c.Status == 0).OrderBy(c => c.CreateDate).AsNoTracking().FirstOrDefault();
            if (resDetailEntity.Status == "R")
            {
                #region 是否允许预订发卡
                string authCode = "2002010";
                long authButtonValue = (Int64)AuthFlag.UpgradeCard;
                bool isAuth = DependencyResolver.Current.GetService<Services.AuthManages.IAuthCheck>().HasAuth(currentInfo.UserId, authCode, authButtonValue, currentInfo.HotelId);
                if (!isAuth)
                {
                    throw new Exception("预订状态不能发卡，原因：没有预订发卡权限");
                }
                #endregion
                //对于预订：第一张卡用预抵时间减10分钟，第二张卡用第一张时间(同一房号同一账号)，第三张也是用第一张卡
                if (firstCard == null)
                {
                    beginTime = DateTime.Now.ToDateTimeWithoutSecondString();
                }
                else
                {
                    beginTime = firstCard.BeginDate.ToDateTimeWithoutSecondString();
                }
            }
            else
            {
                //对于在住：第一张卡用当前时间-10分钟，第二张卡用第一张卡的时间(同一房号同一账号)
                if (firstCard == null)
                {
                    beginTime = DateTime.Now.ToDateTimeWithoutSecondString();
                }
                else
                {
                    beginTime = firstCard.BeginDate.ToDateTimeWithoutSecondString();
                }
            }
            if(lockType == "lockstar")
            {                
                if(string.IsNullOrWhiteSpace(resDetailEntity.GuestMobile))
                {
                    throw new Exception("需填写手机号");
                }
                string onlineLockType = null;
                var masterServiceObject = DependencyResolver.Current.GetService(typeof(IMasterService));
                var masterService = (masterServiceObject as IMasterService);
                if (masterService != null)
                {
                    onlineLockType = masterService.GetHotelOnlineLockType(hid);
                }
                //检测酒店是否有启用短信模块，没有则直接返回
                var smsInfo = _pmsContext.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
                if(onlineLockType != "beda_wx")
                {
                    if (smsInfo == null || smsInfo.Enable != "1")
                    {
                        throw new Exception("酒店没有启用短信模块");
                    }
                }
                string lockid = "";
                if (onlineLockType != "beda_wx")
                {
                    lockid = _pmsContext.Rooms.Where(r => r.Id == resDetailEntity.Roomid && resDetailEntity.Hid == r.Hid).Select(r => r.Lockid).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(lockid))
                    {
                        throw new Exception("未配置房间锁号");
                    }
                }
                
                PwdLock.ILock ilock = null;
                if (onlineLockType == "locstarLock")
                {
                    ilock = new PwdLock.LockStar();
                }
                else if(onlineLockType == "danbay")
                {
                    ilock = new PwdLock.Danbay();
                }
                else if(onlineLockType == "beda_wx")
                {
                    ilock = new PwdLock.Beda_WX();
                }
                else
                {
                    throw new Exception("酒店没有启用密码锁");
                }
                try
                {
                    cardNo = ilock.WriteLock(masterService,_pmsContext,resDetailEntity,smsInfo,lockid,hid,regId,seqId,beginTime);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }            
            if (string.IsNullOrWhiteSpace(seqId))
            {
                //发新卡
                _pmsContext.LockLogs.Add(new LockLog
                {
                    CardNo = cardNo,
                    Hid = hid,
                    Regid = resDetailEntity.Regid,
                    Roomno = resDetailEntity.RoomNo,
                    BeginDate = DateTime.Parse(beginTime),
                    EndDate = resDetailEntity.DepDate.Value,
                    CreateDate = DateTime.Now,
                    Status = (byte)LockStatus.发卡,
                    InputUser = inputUser,
                    SeqNo = seqNo,
                    Locktype = lockType,
                });
                _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.门锁发新卡);
            }
            else
            {
                //重写卡
                long Seqid = -1;
                long.TryParse(seqId, out Seqid);
                if (Seqid <= 0)
                {
                    throw new Exception("参数不正确");
                }
                var lockEntity = _pmsContext.LockLogs.Where(c => c.Seqid == Seqid && c.Status == 0).SingleOrDefault();
                if (lockEntity == null)
                {
                    throw new Exception("参数不正确");
                }
                if (!(lockEntity.Hid == resDetailEntity.Hid && lockEntity.Regid == resDetailEntity.Regid))
                {
                    throw new Exception("参数不正确");
                }
                lockEntity.CardNo = cardNo;
                lockEntity.Hid = hid;
                lockEntity.Regid = resDetailEntity.Regid;
                lockEntity.Roomno = resDetailEntity.RoomNo;
                lockEntity.BeginDate = resDetailEntity.ArrDate.Value;
                lockEntity.EndDate = resDetailEntity.DepDate.Value;
                lockEntity.Status = (byte)LockStatus.发卡;
                lockEntity.InputUser = inputUser;
                lockEntity.CreateDate = DateTime.Now;
                lockEntity.SeqNo = seqNo;
                lockEntity.Locktype = lockType;
                _pmsContext.Entry(lockEntity).State = EntityState.Modified;
                _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.门锁重写卡);
            }
            _pmsContext.SaveChanges();
        }
        /// <summary> 
        /// 生成随机密码锁
        /// </summary> 
        private string GenerateRandomText(int length)
        {
            const string txtChars = "0123456789";
            var sb = new StringBuilder(length);
            int maxLength = txtChars.Length;
            var _rand = new Random();
            for (int n = 0, len = length - 1; n <= len; n++)
            {
                sb.Append(txtChars.Substring(_rand.Next(maxLength), 1));
            }
            return sb.ToString();
        }
        private string getToken(string code, string pwd ,string url)
        {            
            var postString = new StringBuilder();
            postString.Append("code=").Append(HttpUtility.UrlEncode(code));
            postString.Append("&password=").Append(HttpUtility.UrlEncode(pwd));
            var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
            return SMSSendHelper.PostGetData("post", url, contentBytes);
        }
        /// <summary>
        /// 注销卡
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="status">21注销，22无卡注销</param>
        public JsonResultData CancelLock(string hid, string cardNo, int status,string seqId)
        {
            //验证
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(cardNo) || (status != 21 && status != 22))
            {
                return JsonResultData.Failure("注销卡参数不正确");
            }
            LockLog lockEntity;
            long Seqid = -1;
            long.TryParse(seqId, out Seqid);
            if (Seqid <= 0)
            {
                lockEntity = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.CardNo == cardNo && c.Status == 0).SingleOrDefault();
            }
            else
            {
                lockEntity = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.CardNo == cardNo && c.Status == 0 && c.Seqid == Seqid).SingleOrDefault();
            }
            if (lockEntity == null)
            {
                string msg = "此卡号不存在，卡号：" + cardNo;
                if(_pmsContext.LockLogs.Where(c => c.Hid == hid && c.CardNo == cardNo).Any())
                {
                    msg = "此卡号已注销，卡号：" + cardNo;
                }
                return JsonResultData.Failure(msg);
            }
            if("lockstar".Equals(lockEntity.Locktype))//删除密码锁
            {
                string onlineLockType = null;
                var masterServiceObject = DependencyResolver.Current.GetService(typeof(IMasterService));
                var masterService = (masterServiceObject as IMasterService);
                if (masterService != null)
                {
                    onlineLockType = masterService.GetHotelOnlineLockType(hid);
                }
                PwdLock.ILock ilock = null;
                if (onlineLockType == "locstarLock")
                {
                    ilock = new PwdLock.LockStar();
                }
                else if (onlineLockType == "danbay")
                {
                    ilock = new PwdLock.Danbay();
                }
                else if (onlineLockType == "beda_wx")
                {
                    ilock = new PwdLock.Beda_WX();
                }
                else
                {
                    throw new Exception("酒店没有启用密码锁");
                }
                try
                {
                    ilock.CancelLock(lockEntity, _pmsContext, hid, cardNo);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            lockEntity.Status = (byte)status;
            lockEntity.LogoutDate = DateTime.Now;
            _pmsContext.Entry(lockEntity).State = EntityState.Modified;
            _pmsContext.AddDataChangeLogs(status == 21 ? BSPMS.Common.Services.Enums.OpLogType.门锁注销卡 : BSPMS.Common.Services.Enums.OpLogType.门锁无卡注销);
            _pmsContext.SaveChanges();
            return JsonResultData.Successed(string.Format("注销卡成功，房号：{0}，卡号：{1}。", lockEntity.Roomno, lockEntity.CardNo));
        }

        /// <summary>
        /// 根据门锁信息获取主单ID resid
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNo">卡号</param>
        /// <returns></returns>
        public JsonResultData GetResIdByLockInfo(string hid, string cardNo)
        {
            var lockLogList = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.CardNo == cardNo).AsNoTracking().ToList();//根据酒店ID和卡号 获取发卡记录
            if(lockLogList == null || lockLogList.Count <= 0)
            {
                return JsonResultData.Failure("此卡号不存在，卡号：" + cardNo);
            }
            if (!lockLogList.Where(c => c.Status == 0).Any())
            {
                string msg = "此卡号已注销，卡号：" + cardNo;
                if(lockLogList.Count == 1)
                {
                    string regid = lockLogList[0].Regid;
                    string roomno = lockLogList[0].Roomno;
                    var entity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid && c.RoomNo == roomno).AsNoTracking().FirstOrDefault();
                    if(entity != null)
                    {
                        msg += string.Format("，对应订单信息：账号:{0},房号:{1},状态:{2}。", entity.Regid.Replace(hid, ""), entity.RoomNo, EnumExtension.GetDescription(typeof(ResDetailStatus), entity.Status));
                    }
                }
                return JsonResultData.Failure(msg);
            }
            else
            {
                lockLogList = lockLogList.Where(c => c.Status == 0).ToList();
            }
            var regids = lockLogList.Select(c => c.Regid).Distinct().ToList();//根据发卡记录获取账号
            if(regids == null || regids.Count <= 0)
            {
                return JsonResultData.Failure("发卡记录信息错误，请联系管理员，卡号：" + cardNo);
            }
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Hid == hid && regids.Contains(c.Regid)).AsNoTracking().ToList();//根据酒店ID和账号 订单信息
            if(resDetailList == null || resDetailList.Count <= 0)
            {
                return JsonResultData.Failure("发卡记录信息错误，请联系管理员，卡号：" + cardNo);
            }
            if(!resDetailList.Where(c => (c.Status == "I" || (c.Status == "R" && c.Roomid != null && c.RoomNo != null && c.Roomid.Trim().Length > 0 && c.RoomNo.Trim().Length > 0))).Any())
            {
                //return JsonResultData.Failure(string.Format("卡号：{0}，对应订单信息：账号:{1},房号:{2},状态:{3}。", cardNo, resDetailList[0].Regid.Replace(hid, ""), resDetailList[0].RoomNo, EnumExtension.GetDescription(typeof(ResDetailStatus), resDetailList[0].Status)));
            }
            else
            {
                resDetailList = resDetailList.Where(c => (c.Status == "I" || (c.Status == "R" && c.Roomid != null && c.RoomNo != null && c.Roomid.Trim().Length > 0 && c.RoomNo.Trim().Length > 0))).ToList();
            }
            return JsonResultData.Successed(resDetailList[0].Resid);
        }
        /// <summary>
        /// 根据房号获取主单ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房号</param>
        /// <returns></returns>
        public JsonResultData GetResIdByLockRoomNo(string hid, string roomNo)
        {
            var entity = _pmsContext.ResDetails.AsNoTracking().Where(c => c.Hid == hid && c.RoomNo == roomNo && c.Status == "I").Select(c => new { ResId = c.Resid, RegId = c.Regid }).FirstOrDefault();
            if(entity != null)
            {
                return JsonResultData.Successed(entity);
            }
            return JsonResultData.Failure(string.Format("房号：{0}没有在住客人！", roomNo));
        }

        /// <summary>
        /// 检查账单内是否有未注销的门卡
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">账号ID，多项之间用逗号隔开</param>
        /// <returns></returns>
        public JsonResultData CheckLockInfoByRegIds(string hid, string regids)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("酒店代码为空");
            }
            if (string.IsNullOrWhiteSpace(regids))
            {
                return JsonResultData.Failure("账号为空");
            }
            List<string> regidList = new List<string>();
            if (regids.Contains(","))
            {
                regidList = regids.Split(',').ToList();
            }
            else
            {
                regidList.Add(regids);
            }
            string regid = _pmsContext.LockLogs.Where(c => c.Hid == hid && regidList.Contains(c.Regid) && c.Status == 0).OrderByDescending(c => c.CreateDate).Select(c => c.Regid).FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(regid))
            {
                return JsonResultData.Successed(regid);
            }
            else
            {
                return JsonResultData.Failure("");
            }
        }
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
        public JsonResultData DelayDepDate(string hid, List<KeyValuePairModel<string, DateTime>> data, string saveContinue, string delayContinue, DateTime businessDate, out List<KeyValuePairModel<string, string>> logList, out string msg)
        {
            logList = new List<KeyValuePairModel<string, string>>();
            msg = null;
            //验证
            if (string.IsNullOrWhiteSpace(hid) || data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("参数不正确");
            }
            //获取子单列表
            List<string> regIds = data.Select(c => c.Key).ToList();
            var resDetailList = _pmsContext.ResDetails.Where(c => regIds.Contains(c.Regid) && c.Hid == hid && c.Status == "I").ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return JsonResultData.Failure("参数不正确");
            }
            //获取价格代码列表
            var rateIds = resDetailList.Select(c => c.RateCode).Distinct().ToList();
            var rateList = _pmsContext.Rates.Where(c => c.Hid == hid && rateIds.Contains(c.Id)).Select(c => new { c.Id, c.IsDayRoom, c.Name }).AsNoTracking().ToList();
            foreach (var item in resDetailList)
            {
                var depDate = data.Where(c => c.Key == item.Regid).Select(c => c.Value).FirstOrDefault();
                if (depDate == null || item.ArrDate >= depDate || DateTime.Now >= depDate)
                {
                    return JsonResultData.Failure("延期后的离店时间要大于当前时间和抵店时间");
                }
                var rateEntity = rateList.Where(c => c.Id == item.RateCode).FirstOrDefault();
                if(rateEntity != null && rateEntity.IsDayRoom == true)
                {
                    if(item.ArrDate.Value.Date != depDate.Date)
                    {
                        return JsonResultData.Failure(string.Format("账号：{1}，当前价格代码：{0}，属于白日房，抵店时间和离店时间要求必须是同一天！", rateEntity.Name, item.Regid.Substring(item.Hid.Length)));
                    }
                }
            }
            //检查房间可用性
            List<string> upRoomEnableChecks = new List<string>();
            foreach (var item in resDetailList)
            {
                var depDate = data.Where(c => c.Key == item.Regid).Select(c => c.Value).FirstOrDefault();
                upRoomEnableChecks.Add(UpRoomEnableCheck_checkInfo(item.Roomid, item.RoomTypeid, item.ArrDate.ToString(), depDate.ToString(), item.RoomQty.ToString(), item.Sourceid));
            }
            JsonResultData checkResult = UpRoomEnableCheck(hid, resDetailList.Select(c => c.Regid).ToList(), ResDetailStatus.I, upRoomEnableChecks, saveContinue);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            //修改
            foreach (var item in resDetailList)
            {
                //修改子单
                var oldDepDate = item.DepDate;
                item.DepDate = data.Where(c => c.Key == item.Regid).Select(c => c.Value).FirstOrDefault();
                _pmsContext.Entry(item).State = EntityState.Modified;
                logList.Add(new KeyValuePairModel<string, string>(item.Regid, string.Format("延期 主单号:{0} 账号:{1} 房号:{2} 离店时间:{3}=>{4}", item.Resno, item.Regid.Replace(hid, ""), item.RoomNo, oldDepDate.ToDateTimeWithoutSecondString(), item.DepDate.ToDateTimeWithoutSecondString())));
                msg += string.Format("房号：{0}，延期至：{1}\n", item.RoomNo, item.DepDate.ToDateTimeWithoutSecondString());
                //增加延期价格计划
                DateTime beginRateDate = oldDepDate.Value.Date;
                DateTime endRateDate = item.DepDate.Value.Date;
                if (beginRateDate < endRateDate)
                {
                    var orderDetailPlans = _pmsContext.RateDetails.Where(w => w.Hid == hid && w.Rateid == item.RateCode && w.RoomTypeid == item.RoomTypeid && w.RateDate >= beginRateDate && w.RateDate <= endRateDate && w.Rate != null).Select(s => new ResDetailPlanInfo { Ratedate = s.RateDate.ToString(), Price = s.Rate, OriginPrice = s.Rate }).ToList();
                    if (orderDetailPlans != null && orderDetailPlans.Count > 0)
                    {
                        var oldResDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == item.Regid).ToList();
                        decimal? continueUseLastPrice = null;
                        var lastPlans = oldResDetailPlans.OrderByDescending(c => c.Ratedate).FirstOrDefault();
                        if (lastPlans != null && lastPlans.OriginPrice != null && lastPlans.Price != null)
                        {
                            if (lastPlans.OriginPrice != lastPlans.Price)
                            {
                                continueUseLastPrice = lastPlans.Price;
                            }
                        }
                        var oldResDetailPlans_date = oldResDetailPlans.Select(c => c.Ratedate).ToList();
                        if (oldResDetailPlans_date != null && oldResDetailPlans_date.Count > 0)
                        {
                            orderDetailPlans = orderDetailPlans.Where((c)=> { return !oldResDetailPlans_date.Contains(DateTime.Parse(c.Ratedate)); }).ToList();
                        }
                        if (orderDetailPlans != null && orderDetailPlans.Count > 0)
                        {
                            foreach (var price in orderDetailPlans)
                            {
                                if (continueUseLastPrice != null)
                                {
                                    price.Price = continueUseLastPrice;
                                }
                                var ratedate = DateTime.Parse(price.Ratedate);
                                if (ratedate >= businessDate && ratedate <= item.DepDate)
                                {
                                    AddResDetailPlan(price, item.Regid);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (delayContinue != "1")
                        {
                            return JsonResultData.Failure("此时间段内没有设置价格，请到价格体系中设置。<br />继续延期？", 3);//js判断ErrorCode=3，提示后可以继续延期
                        }
                    }
                }
                else if (beginRateDate > endRateDate)
                {
                    if(item.ArrDate.Value.Date == endRateDate)
                    {
                        _pmsContext.ResDetailPlans.RemoveRange(_pmsContext.ResDetailPlans.Where(c => c.Regid == item.Regid && c.Ratedate > endRateDate).ToList());
                    }
                    else
                    {
                        _pmsContext.ResDetailPlans.RemoveRange(_pmsContext.ResDetailPlans.Where(c => c.Regid == item.Regid && c.Ratedate > endRateDate).ToList());
                    }
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resDetailList[0].Resid, resDetailList[0].Regid, "", out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            return JsonResultData.Successed(result);
        }
        #endregion

        #region 换房
        /// <summary>
        /// 获取分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <param name="roomTypeId">房间类型ID</param>
        /// <returns></returns>
        public List<UpQueryRoomAutoChooseResult> GetRoomForRoomType(string hid, string regId, string roomTypeId)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(roomTypeId))
            {
                return new List<UpQueryRoomAutoChooseResult>();
            }
            //获取子单
            var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid && c.Status == "I").FirstOrDefault();
            if (resDetailEntity == null)
            {
                return new List<UpQueryRoomAutoChooseResult>();
            }
            return UpRoomAutoChoose(hid, ResDetailStatus.I, new List<string> { UpRoomAutoChoose_checkInfo("0", roomTypeId, DateTime.Now.ToString(), resDetailEntity.DepDate.ToString()) }); ;
        }
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
        public List<UpQueryRoomAutoChooseResult> GetRoomForRoomType(string hid, string regId, string roomTypeId, string floors, string features, string roomno)
        {
            var result = GetRoomForRoomType(hid, regId, roomTypeId);
            if(result != null && result.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(roomno))
                {
                    //过滤房号
                    result = result.Where(c => c.roomno == roomno).ToList();
                }                
                if (result != null && result.Count > 0)
                {
                    List<string> floorList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(floors))
                    {
                        if (floors.Contains(","))
                        {
                            floorList = floors.Split(',').ToList();
                        }
                        else
                        {
                            floorList.Add(floors);
                        }
                    }
                    List<string> featureList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(features))
                    {
                        if (features.Contains(","))
                        {
                            featureList = features.Split(',').ToList();
                        }
                        else
                        {
                            featureList.Add(features);
                        }
                    }
                    if((floorList != null && floorList.Count > 0) || (featureList != null && featureList.Count > 0))
                    {
                        var roomids = result.Select(c => c.Roomid).ToList();
                        var rooms = _pmsContext.Rooms.Where(c => c.Hid == hid && roomids.Contains(c.Id)).AsNoTracking().ToList();
                        if (floorList != null && floorList.Count > 0)
                        {
                            //过滤楼层
                            rooms = rooms.Where(c => floorList.Contains(c.Floorid)).ToList();
                        }
                        if(rooms != null && rooms.Count > 0)
                        {
                            if (featureList != null && featureList.Count > 0)
                            {
                                List<KeyValuePairModel<string, string>> temps = new List<KeyValuePairModel<string, string>>();
                                foreach (var item in rooms)
                                {
                                    if (!string.IsNullOrWhiteSpace(item.Feature))
                                    {
                                        if (item.Feature.Contains(","))
                                        {
                                            temps.AddRange(item.Feature.Split(',').Select(c => new KeyValuePairModel<string, string> { Key = item.Id, Value = c }).ToList());
                                        }
                                        else
                                        {
                                            temps.Add(new KeyValuePairModel<string, string>(item.Id, item.Feature));
                                        }
                                    }
                                }
                                var feature_roomids = temps.Where(c => featureList.Contains(c.Value)).Select(c => c.Key).Distinct().ToList();
                                //过滤房间特色
                                rooms = rooms.Where(c => feature_roomids.Contains(c.Id)).ToList();
                            }
                        }
                        roomids = rooms.Select(c => c.Id).ToList();
                        result = result.Where(c => roomids.Contains(c.Roomid)).ToList();
                    }
                }
            }
            return result;
        }

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
        public JsonResultData ChangeRoom(ICurrentInfo currentInfo, string regId, string roomId, List<ResDetailPlanInfo> orderDetailPlans, string remark, string saveContinue, DateTime businessDate, string authorizationSaveContinue)
        {
            string hid = currentInfo.HotelId;
            #region 验证
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regId) || string.IsNullOrWhiteSpace(roomId) || orderDetailPlans == null || orderDetailPlans.Count <= 0)
            {
                return JsonResultData.Failure("参数不正确");
            }
            foreach (var price in orderDetailPlans)
            {
                if (string.IsNullOrWhiteSpace(price.Ratedate))
                {
                    return JsonResultData.Failure("参数不正确");
                }
                DateTime tempDate = new DateTime();
                if (!DateTime.TryParse(price.Ratedate, out tempDate))
                {
                    return JsonResultData.Failure("参数不正确");
                }
                if (price.Price == null || price.Price < 0)
                {
                    return JsonResultData.Failure("请输入房价");
                }
            }
            #endregion

            //获取子单
            var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid && c.Status == "I").FirstOrDefault();
            if (resDetailEntity == null)
            {
                return JsonResultData.Failure("参数不正确");
            }
            var roomEntity = _pmsContext.Rooms.Where(c => c.Id == roomId && c.Hid == hid && c.Status == EntityStatus.启用).Select(c => new { c.RoomNo, c.RoomTypeid }).SingleOrDefault();
            if (roomEntity == null || string.IsNullOrWhiteSpace(roomEntity.RoomTypeid) || string.IsNullOrWhiteSpace(roomEntity.RoomNo))
            {
                return JsonResultData.Failure("参数不正确");
            }
            //赋值
            string originRoomTypeid = resDetailEntity.RoomTypeid;
            string originRoomNo = resDetailEntity.RoomNo;
            resDetailEntity.RoomTypeid = roomEntity.RoomTypeid;
            resDetailEntity.Roomid = roomId;
            resDetailEntity.RoomNo = roomEntity.RoomNo;

            #region 验证
            List<UpQueryRoomAutoChooseResult> checkList = UpRoomAutoChoose(hid, ResDetailStatus.I, new List<string> { UpRoomAutoChoose_checkInfo("0", resDetailEntity.RoomTypeid, DateTime.Now.ToString(), resDetailEntity.DepDate.ToString()) });
            if (!checkList.Where(c => c.Roomid == resDetailEntity.Roomid && c.roomno == resDetailEntity.RoomNo).Any())
            {
                return JsonResultData.Failure("房号不可用，请重新选择！");
            }
            //检查房间可用性
            JsonResultData checkResult = UpRoomEnableCheck(hid, new List<string> { regId }, ResDetailStatus.I, new List<string> { UpRoomEnableCheck_checkInfo(resDetailEntity.Roomid, resDetailEntity.RoomTypeid, resDetailEntity.ArrDate.ToString(), resDetailEntity.DepDate.ToString(), resDetailEntity.RoomQty.ToString(), resDetailEntity.Sourceid) }, saveContinue);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            #endregion
            //在房价计划中查询，日期等于今天的记录，其价格作为子单的执行价。
            resDetailEntity.Rate = null;
            string nowDate = businessDate.ToDateString();
            resDetailEntity.Rate = orderDetailPlans.Where(c => c.Ratedate == nowDate).Select(s => s.Price).SingleOrDefault();
            if (resDetailEntity.Rate == null)
            {
                List<DateTime> dateList = new List<DateTime>();
                foreach (var item in orderDetailPlans)
                {
                    dateList.Add(DateTime.Parse(item.Ratedate));
                }
                //给明细记录赋值房价计划中的第一天价格做为子单的执行价,
                if (businessDate.Date <= dateList.Min())
                {
                    string minDate = dateList.Min().ToDateString();
                    resDetailEntity.Rate = orderDetailPlans.Where(c => c.Ratedate == minDate).Select(s => s.Price).Single();
                }
                //给明细记录赋值房价计划中的最后一天价格做为子单的执行价,
                if (businessDate.Date >= dateList.Max())
                {
                    string maxDate = dateList.Max().ToDateString();
                    resDetailEntity.Rate = orderDetailPlans.Where(c => c.Ratedate == maxDate).Select(s => s.Price).Single();
                }
            }
            //修改子单
            string updatePriceLog = string.Format("换房调价 账号：{0}", GetShortRegid(hid, regId));
            _pmsContext.Entry(resDetailEntity).State = EntityState.Modified;
            _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.换房, "换房", false);
            //价格计划始终是先删除原来的，再重新添加
            var oldDetailPlans = _pmsContext.ResDetailPlans.Where(w => w.Regid == regId && w.Ratedate >= businessDate).ToList();
            _pmsContext.ResDetailPlans.RemoveRange(oldDetailPlans);
            foreach (var price in orderDetailPlans)
            {
                var ratedate = DateTime.Parse(price.Ratedate);
                if(ratedate >= businessDate && ratedate<= resDetailEntity.DepDate)
                {
                    AddResDetailPlan(price, regId);

                    var temp = oldDetailPlans.Where(c => c.Regid == regId && c.Ratedate == ratedate).FirstOrDefault();
                    if(temp != null && temp.Price != price.Price)
                    {
                        updatePriceLog += string.Format("，{0}价格：{1}=>{2}", price.Ratedate, temp.Price, price.Price);
                    }
                }
            }
            string authUserName = null;string reason = "";
            var checkAuthAdjustResult = CheckAuthAdjustPrice(currentInfo, new List<ResAdjustPriceInfo.AdjustPriceOrderModel>() { new ResAdjustPriceInfo.AdjustPriceOrderModel { RegId = resDetailEntity.Regid, RateCodeId = resDetailEntity.RateCode, RoomTypeId = resDetailEntity.RoomTypeid, OriginRateCodeId = resDetailEntity.RateCode, OriginRoomTypeId = originRoomTypeid, GuestName = resDetailEntity.Guestname, RoomNo = resDetailEntity.RoomNo, OriginRoomNo = originRoomNo } }, ResAdjustPriceInfo.AdjustPriceOperationSource.OrderChangeRoom);
            if (!checkAuthAdjustResult.Success)
            {
                if (!CheckAuthAdjustPriceResult(currentInfo, authorizationSaveContinue, regId, out authUserName,out reason))
                {
                    return checkAuthAdjustResult;
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resDetailEntity.Resid, regId, "", out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            if (!string.IsNullOrWhiteSpace(authUserName))
            {
                AddOperationLog(currentInfo, BSPMS.Common.Services.Enums.OpLogType.换房, (updatePriceLog + "，" + reason + "，授权人：" + authUserName), regId, true);
            }
            
            return JsonResultData.Successed(result);
        }
        #endregion

        #region 关联房
        /// <summary>
        /// 获取增加关联房所需要的列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="notResId">不包括订单ID</param>
        /// <param name="roomNo">房间号</param>
        /// <param name="guestName">客人名</param>
        /// <param name="guestMobile">客人手机号</param>
        /// <returns></returns>
        public JsonResultData GetRelationList(string hid, string notResId, string roomNo, string guestName, string guestMobile, string status)
        {
            var list = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid != notResId);
            var permanentRoom_regids = _pmsContext.PermanentRoomSets.Where(c => c.Hid == hid).Select(c => c.Regid).ToList();
            list = list.Where(c => !permanentRoom_regids.Contains(c.Regid));
            if (!string.IsNullOrWhiteSpace(roomNo))
            {
                list = list.Where(c => c.RoomNo == roomNo);
            }
            if (!string.IsNullOrWhiteSpace(guestName))
            {
                list = list.Where(c => c.Guestname.Contains(guestName));
            }
            if (!string.IsNullOrWhiteSpace(guestMobile))
            {
                list = list.Where(c => c.GuestMobile.Contains(guestMobile));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                list = list.Where(c => c.Status == status);
            }
            var returnList = list.Select(s => new { s.Hid, s.Regid, s.Resno, s.RoomNo, s.Guestname, s.GuestMobile, s.ArrDate, s.DepDate, s.Status, StatusName = "" }).AsNoTracking().ToList();
            return JsonResultData.Successed(returnList);
        }
        /// <summary>
        /// 增加关联（把一个或多个子单添加到另一个订单中）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="addRegIds">要增加关联的子单ID列表</param>
        /// <param name="toResId">要添加到的订单ID</param>
        /// <returns></returns>
        public JsonResultData AddRelation(ICurrentInfo currentInfo, string[] addRegIds, string toResId)
        {
            string hid = currentInfo.HotelId;
            //获取resId和resNo
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == toResId && c.Hid == hid).Select(s => new { s.Resid, s.Resno }).SingleOrDefault();
            if (resEntity == null)
            {
                return JsonResultData.Failure("订单ID不存在");
            }
            //把子单移动到另一个订单
            var regList = _pmsContext.ResDetails.Where(c => addRegIds.Contains(c.Regid) && c.Hid == hid).ToList();
            if (regList == null || regList.Count <= 0 || addRegIds.Length != regList.Count)
            {
                return JsonResultData.Failure("子单ID不存在");
            }

            string oldResno = regList[0].Resno; string newResno = resEntity.Resno; List<string> regids = new List<string>(); List<string> roomNos = new List<string>();
            foreach(var regEntity in regList)
            {
                regEntity.Resid = resEntity.Resid;
                regEntity.Resno = resEntity.Resno;
                _pmsContext.Entry(regEntity).State = EntityState.Modified;
                regids.Add(regEntity.Regid.Substring(hid.Length));
                roomNos.Add(regEntity.RoomNo);
            }
            AddOperationLog(currentInfo, BSPMS.Common.Services.Enums.OpLogType.关联房增加, string.Format("关联房增加  主单号：{0}=>{1}，账号：{2}，房号：{3}", oldResno, newResno, string.Join(",", regids), string.Join(",", roomNos)), regList[0].Regid);
            
            //把子单的账务信息移动到另一个订单
            var resFolios = _pmsContext.ResFolios.Where(c => addRegIds.Contains(c.Regid) && c.Hid == hid).ToList();
            if (resFolios != null && resFolios.Count > 0)
            {
                foreach (var item in resFolios)
                {
                    item.Resid = resEntity.Resid;
                    _pmsContext.Entry(item).State = EntityState.Modified;
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resEntity.Resid, "", "", out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            return JsonResultData.Successed(result);
        }
        /// <summary>
        /// 增加关联（把一个订单里的所有子单添加到另一个订单中）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="addResId">订单ID</param>
        /// <param name="toResId">要添加到的订单ID</param>
        /// <returns></returns>
        public JsonResultData AddRelation(ICurrentInfo currentInfo, string addResId, string toResId)
        {
            string hid = currentInfo.HotelId;
            if(!_pmsContext.Reses.Where(c => c.Resid == addResId && c.Hid == hid).AsNoTracking().Any())
            {
                return JsonResultData.Failure("订单ID不存在");
            }
            string[] addRegIds = _pmsContext.ResDetails.Where(c => c.Resid == addResId && c.Hid == hid).Select(c => c.Regid).AsNoTracking().ToArray();
            if (addRegIds == null || addRegIds.Length <= 0)
            {
                return JsonResultData.Failure("子单ID不存在");
            }
            return AddRelation(currentInfo, addRegIds, toResId);
        }

        /// <summary>
        /// 取消关联（把子单从订单中分离出来）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        public JsonResultData RemoveRelation(string hid, string regId)
        {
            //获取子单ID
            var regEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid).SingleOrDefault();
            if (regEntity == null)
            {
                return JsonResultData.Failure("子单ID不存在");
            }
            //获取订单ID
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == regEntity.Resid && c.Hid == hid).SingleOrDefault();
            if (resEntity == null)
            {
                return JsonResultData.Failure("订单ID不存在");
            }
            //获取新的订单ID和订单号
            string newResno = _pmsContext.GetBaseNoForRes(hid);
            string newResid = string.Format("{0}{1}", hid, newResno);
            //把子单移动到新订单中
            regEntity.Resid = newResid;
            regEntity.Resno = newResno;
            _pmsContext.Entry(regEntity).State = EntityState.Modified;
            _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.关联房取消, "关联房取消", false);
            //增加订单
            _pmsContext.Reses.Add(new Res
            {
                Resid = newResid,
                Resno = newResno,
                CDate = resEntity.CDate,
                Cttid = resEntity.Cttid,
                Hid = resEntity.Hid,
                IsGroup = resEntity.IsGroup,
                Marketid = resEntity.Marketid,
                Name = resEntity.Name,
                RateCode = resEntity.RateCode,
                ResCustMobile = resEntity.ResCustMobile,
                ResCustName = resEntity.ResCustName,
                ResNoExt = resEntity.ResNoExt,
                ResTime = resEntity.ResTime,
                ResUser = resEntity.ResUser,
                Sourceid = resEntity.Sourceid,
                OrderFrom = resEntity.OrderFrom,
                CttSales = resEntity.CttSales,
            });
            //增加订单发票
            //var resInvoiceEntity = _pmsContext.ResInvoiceInfos.Where(c => c.Resid == regEntity.Resid && c.Hid == hid).FirstOrDefault();
            //if (resInvoiceEntity != null)
            //{
            //    _pmsContext.ResInvoiceInfos.Add(new Entities.ResInvoiceInfo
            //    {
            //        Resid = newResid,
            //        Id = Guid.NewGuid(),
            //        Hid = resInvoiceEntity.Hid,
            //        InvoiceType = resInvoiceEntity.InvoiceType,
            //        TaxAddTel = resInvoiceEntity.TaxAddTel,
            //        TaxBankAccount = resInvoiceEntity.TaxBankAccount,
            //        TaxName = resInvoiceEntity.TaxName,
            //        TaxNo = resInvoiceEntity.TaxNo
            //    });
            //}
            //把子单账务信息移动到新订单中
            var resFolios = _pmsContext.ResFolios.Where(c => c.Regid == regId && c.Hid == hid).ToList();
            if (resFolios != null && resFolios.Count > 0)
            {
                foreach (var item in resFolios)
                {
                    item.Resid = newResid;
                    _pmsContext.Entry(item).State = EntityState.Modified;
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resEntity.Resid, "", "", out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", (string.IsNullOrWhiteSpace(regIdsAll)? regId : (regIdsAll + "," + regId)))
                );
            #endregion
            return JsonResultData.Successed(result);
        }

        /// <summary>
        /// 获取关联房的主单信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regId">子单ID</param>
        /// <returns></returns>
        public JsonResultData GetRelationResInfo(string hid, string regId)
        {
            var regEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid).AsNoTracking().FirstOrDefault();
            if(regEntity != null)
            {
                string resid = regEntity.Resid;
                if (!string.IsNullOrWhiteSpace(resid))
                {
                    if (_pmsContext.Reses.Where(c => c.Resid == resid && c.Hid == hid).AsNoTracking().Any())
                    {
                        var list = _pmsContext.ResDetails.Where(c => c.Resid == resid && c.Hid == hid && c.Regid != regId).AsNoTracking().ToList();
                        if (list != null && list.Count > 0)
                        {
                            list.Insert(0, regEntity);
                            var result = list.Select(c => new { RoomNo = c.RoomNo, OriginRegId = c.Regid, RegId = (c.Regid.StartsWith(hid) ? c.Regid.Substring(hid.Length) : c.Regid), GuestName = c.Guestname, resNo = c.Resno, resId = c.Resid });
                            return JsonResultData.Successed(result);
                        }
                        return JsonResultData.Successed();
                    }
                }
            }
            return JsonResultData.Failure("订单ID错误");
        }
        #endregion

        #region 分房、入住
        /// <summary>
        /// 自动分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="isAuto">是否自动分房（true是，false否）</param>
        /// <returns></returns>
        public List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>> GetRoomAutoChoose(string hid, string resId, bool isAuto)
        {
            //获取主单
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == resId && c.Hid == hid).AsNoTracking().FirstOrDefault();
            if (resEntity == null)
            {
                return new List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>>();
            }
            //获取子单
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Resid == resEntity.Resid && c.Hid == resEntity.Hid && c.Resno == resEntity.Resno && c.Status == "R").AsNoTracking().ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return new List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>>();
            }
            return GetRoomAutoChoose(hid, resDetailList, isAuto);
        }

        /// <summary>
        /// 保存分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <returns></returns>
        public JsonResultData SaveRooms(string hid, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue)
        {
            //验证并过滤数据
            if (data == null) { return JsonResultData.Failure("请选择房号！"); }
            string msg = "";
            bool isTrue = Validate(data, out msg);
            if (!isTrue)
            {
                return JsonResultData.Failure(msg);
            }
            //获取主单
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == resId && c.Hid == hid).AsNoTracking().FirstOrDefault();
            if (resEntity == null)
            {
                return JsonResultData.Failure("订单不存在！");
            }
            //获取子单
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Resid == resEntity.Resid && c.Hid == resEntity.Hid && c.Resno == resEntity.Resno && c.Status == "R").ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return JsonResultData.Failure("订单子单不存在！");
            }
            //验证数据
            ValidateData(hid, resDetailList, data);
            //检查数据有效性（预订）
            var checkResult = CheckSaveBefore(ResDetailStatus.R, hid, resDetailList, data, saveContinue);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            //循环处理子单
            foreach (var item in resDetailList)
            {
                if (item.Status == ResDetailStatus.R.ToString() && item.RoomQty > 0 && !string.IsNullOrWhiteSpace(item.RoomTypeid) && item.ArrDate.HasValue && item.DepDate.HasValue)
                {
                    #region
                    //获取子单参数
                    List<KeyValuePairModel<string, string>> roomIdAndNo = new List<KeyValuePairModel<string, string>>();
                    var tempRegid = data.Where(c => c.Key == item.Regid).SingleOrDefault();
                    if (tempRegid != null && tempRegid.Value != null && tempRegid.Value.Count > 0)
                    {
                        tempRegid.Value.RemoveAll(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value));
                        if(tempRegid != null && tempRegid.Value != null && tempRegid.Value.Count > 0)
                        {
                            roomIdAndNo = tempRegid.Value;
                        }
                    }

                    if (roomIdAndNo != null && roomIdAndNo.Count > 0 && roomIdAndNo.Count <= item.RoomQty)
                    {
                        int index = 1;
                        foreach (var selectRoom in roomIdAndNo)
                        {
                            if (item.RoomQty == 1 && roomIdAndNo.Count == index)//修改当前记录
                            {
                                item.Roomid = selectRoom.Key;
                                item.RoomNo = selectRoom.Value;
                            }
                            else
                            {
                                if (item.RoomQty >= 2)
                                {
                                    //拆分当前
                                    item.RoomQty = item.RoomQty - 1;
                                    //新订单ID
                                    string newRegid = _pmsContext.GetBaseNoForRegId(item.Hid);
                                    #region 增加新的子单
                                    _pmsContext.ResDetails.Add(new ResDetail
                                    {
                                        Roomid = selectRoom.Key,
                                        RoomNo = selectRoom.Value,
                                        Regid = newRegid,
                                        RoomQty = 1,
                                        Hid = item.Hid,
                                        Resid = item.Resid,
                                        Resno = item.Resno,
                                        Billtype = item.Billtype,
                                        RoomTypeid = item.RoomTypeid,
                                        Sourceid = item.Sourceid,
                                        Marketid = item.Marketid,
                                        ArrDate = item.ArrDate,
                                        DepDate = item.DepDate,
                                        RateCode = item.RateCode,
                                        Rate = item.Rate,
                                        Bbf = item.Bbf,
                                        ResStatus = item.ResStatus,
                                        HoldDate = item.HoldDate,
                                        Remark = item.Remark,
                                        Guestname = item.Guestname,
                                        GuestMobile = item.GuestMobile,
                                        Spec = item.Spec,
                                        Cdate = item.Cdate,
                                        BsnsDate = item.BsnsDate,
                                        RegQty = item.RegQty,
                                        ExtType = item.ExtType,
                                        Guestid = item.Guestid,
                                        Profileid = item.Profileid,
                                        RecStatus = item.RecStatus,
                                        IsSettle = false,
                                        InputUser = item.InputUser,
                                    });
                                    #endregion
                                    #region 增加新的房价计划
                                    var resDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == item.Regid).ToList();
                                    foreach (var itemPlan in resDetailPlans)
                                    {
                                        if(itemPlan != null && itemPlan.Price != null && itemPlan.Price.HasValue)
                                        {
                                            _pmsContext.ResDetailPlans.Add(new ResDetailPlan { Regid = newRegid, Ratedate = itemPlan.Ratedate, Price = itemPlan.Price });
                                        }
                                    }
                                    #endregion
                                    #region 增加新的客人
                                    var reginfoList = _pmsContext.RegInfos.Where(c => c.Hid == hid && c.Regid == item.Regid).ToList();
                                    foreach (var reginfoItem in reginfoList)
                                    {
                                        _pmsContext.RegInfos.Add(new RegInfo
                                        {
                                            Address = reginfoItem.Address,
                                            Birthday = reginfoItem.Birthday,
                                            CarNo = reginfoItem.CarNo,
                                            Cerid = reginfoItem.Cerid,
                                            CerType = reginfoItem.CerType,
                                            City = reginfoItem.City,
                                            Email = reginfoItem.Email,
                                            Gender = reginfoItem.Gender,
                                            GuestEName = reginfoItem.GuestEName,
                                            GuestName = reginfoItem.GuestName,
                                            Hid = reginfoItem.Hid,
                                            Id = Guid.NewGuid(),
                                            Interest = reginfoItem.Interest,
                                            IsMast = reginfoItem.IsMast,
                                            Mobile = reginfoItem.Mobile,
                                            Qq = reginfoItem.Qq,
                                            Regid = newRegid,
                                            Nation = reginfoItem.Nation,
                                        });
                                    }
                                    #endregion
                                }
                            }
                            index++;
                        }
                        _pmsContext.Entry(item).State = EntityState.Modified;
                    }
                    #endregion
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resId, data[0].Key, null, out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            return JsonResultData.Successed(result);
        }

        /// <summary>
        /// 保存分房并且入住
        /// </summary>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <param name="saveContinue">是否继续保存</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns></returns>
        public JsonResultData SaveRoomsAndCheckIn(ICurrentInfo currentInfo, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue, string useScoreSaveContinue, DateTime businessDate, bool isEnvTest)
        {
            string hid = currentInfo.HotelId;
            string inputuser = currentInfo.UserName;
            //验证并过滤数据
            string msg = "";
            bool isTrue = Validate(data, out msg);
            if (!isTrue)
            {
                return JsonResultData.Failure(msg);
            }
            if (data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("请勾选房号！");
            }
            //获取主单
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == resId && c.Hid == hid).AsNoTracking().FirstOrDefault();
            if (resEntity == null)
            {
                return JsonResultData.Failure("订单不存在！");
            }
            //获取子单
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Resid == resEntity.Resid && c.Hid == resEntity.Hid && c.Resno == resEntity.Resno && c.Status == "R").ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return JsonResultData.Failure("订单子单不存在！");
            }
            //判断入住抵店时间
            foreach (var item in data)
            {
                var entityDetail = resDetailList.Where(c => c.Regid == item.Key).FirstOrDefault();
                if (entityDetail != null && entityDetail.ArrDate >= DateTime.Now.AddDays(1).Date)
                {
                    return JsonResultData.Failure("如需提前入住，请修改预订抵店时间");
                }
            }
            //验证数据
            ValidateData(hid, resDetailList, data);
            if (data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("可用房数据已更新，请刷新页面！");
            }
            //检查数据有效性（预订）
            var checkResult = CheckSaveBefore(ResDetailStatus.I, hid, resDetailList, data, saveContinue, businessDate);
            if (!checkResult.Success)
            {
                return checkResult;
            }
            //验证入住时间段
            var resDetailListCheck = resDetailList.Where(c => c.Status == ResDetailStatus.R.ToString() && c.RoomQty > 0 && !string.IsNullOrWhiteSpace(c.RoomTypeid) && c.ArrDate.HasValue && c.DepDate.HasValue).ToList();
            var rateids = resDetailListCheck.Select(c => c.RateCode).Distinct().ToList();
            if(rateids != null && rateids.Count > 0)
            {
                string rateTimeCheckResult = "";
                DateTime nowDate = DateTime.Now;
                var rateList = _pmsContext.Rates.Where(c => c.Hid == hid && rateids.Contains(c.Id)).AsNoTracking().ToList();
                foreach(var rateEntity in rateList)
                {
                    bool isTrueRateTime = false;
                    DateTime startTime = DateTime.MinValue;
                    DateTime endTime = DateTime.MinValue;

                    DateTime arrDate = nowDate;
                    bool isTrueStartTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.StartTime), out startTime);
                    bool isTrueEndTime = DateTime.TryParse((arrDate.ToDateString() + " " + rateEntity.EndTime), out endTime);

                    if (isTrueStartTime && isTrueEndTime && startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                    {
                        if (startTime < endTime && arrDate >= startTime && arrDate <= endTime)
                        {
                            isTrueRateTime = true;
                        }
                    }
                    if (!isTrueRateTime)
                    {
                        string regids = string.Join(",", resDetailListCheck.Where(c => c.RateCode == rateEntity.Id).Select(c => c.Regid.Substring(c.Hid.Length)).ToList());
                        rateTimeCheckResult += string.Format("账号：{4}，入住时间：{0}，当前价格代码：{1}，只有在时间段内[{2}-{3}]可入住！\n", arrDate.ToString("HH:mm"), rateEntity.Name, rateEntity.StartTime, rateEntity.EndTime, regids);
                    }
                }
                //验证白日房
                foreach (var item in resDetailListCheck)
                {
                    var rateEntity = rateList.Where(c => c.Id == item.RateCode).FirstOrDefault();
                    if (rateEntity != null && rateEntity.IsDayRoom == true)
                    {
                        if (nowDate.Date != item.DepDate.Value.Date)
                        {
                            rateTimeCheckResult += string.Format("账号：{1}，当前价格代码：{0}，属于白日房，抵店时间和离店时间要求必须是同一天！\n", rateEntity.Name, item.Regid.Substring(item.Hid.Length));
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(rateTimeCheckResult))
                {
                    return JsonResultData.Failure(rateTimeCheckResult);
                }
            }
            //循环处理子单
            bool isUpdate = false;
            List<string> regidsTemp = new List<string>();
            List<KeyValuePairModel<string,string>> rateCodeIdsTemp = new List<KeyValuePairModel<string, string>>();
            foreach (var item in resDetailList)
            {
                if (item.Status == ResDetailStatus.R.ToString() && item.RoomQty > 0 && !string.IsNullOrWhiteSpace(item.RoomTypeid) && item.ArrDate.HasValue && item.DepDate.HasValue)
                {
                    #region
                    //获取子单参数
                    List<KeyValuePairModel<string, string>> roomIdAndNo = new List<KeyValuePairModel<string, string>>();
                    var tempRegid = data.Where(c => c.Key == item.Regid).SingleOrDefault();
                    if (tempRegid != null && tempRegid.Value != null && tempRegid.Value.Count > 0)
                    {
                        tempRegid.Value.RemoveAll(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value));
                        if (tempRegid != null && tempRegid.Value != null && tempRegid.Value.Count > 0)
                        {
                            roomIdAndNo = tempRegid.Value;
                        }
                    }

                    if (roomIdAndNo != null && roomIdAndNo.Count > 0 && roomIdAndNo.Count <= item.RoomQty)
                    {
                        int index = 1;
                        foreach (var selectRoom in roomIdAndNo)
                        {
                            if (item.RoomQty == 1 && roomIdAndNo.Count == index)//修改当前记录
                            {
                                item.Roomid = selectRoom.Key;
                                item.RoomNo = selectRoom.Value;
                                item.ArrBsnsDate = businessDate;
                                item.RecStatus = ResDetailStatus.I.ToString();
                                item.ArrDate = DateTime.Now;
                                item.liveInputUser = inputuser;
                                isUpdate = true;
                                regidsTemp.Add(item.Regid);
                                rateCodeIdsTemp.Add(new KeyValuePairModel<string, string>(item.Regid, item.RateCode));
                            }
                            else
                            {
                                if (item.RoomQty >= 2)
                                {
                                    //拆分当前
                                    item.RoomQty = item.RoomQty - 1;
                                    //新订单ID
                                    string newRegid = _pmsContext.GetBaseNoForRegId(item.Hid);
                                    isUpdate = true;
                                    regidsTemp.Add(newRegid);
                                    rateCodeIdsTemp.Add(new KeyValuePairModel<string, string>(newRegid, item.RateCode));
                                    #region 增加新的子单
                                    _pmsContext.ResDetails.Add(new ResDetail
                                    {
                                        Roomid = selectRoom.Key,
                                        RoomNo = selectRoom.Value,
                                        Regid = newRegid,
                                        RoomQty = 1,
                                        Hid = item.Hid,
                                        Resid = item.Resid,
                                        Resno = item.Resno,
                                        Billtype = item.Billtype,
                                        RoomTypeid = item.RoomTypeid,
                                        Sourceid = item.Sourceid,
                                        Marketid = item.Marketid,
                                        ArrDate = DateTime.Now,
                                        DepDate = item.DepDate,
                                        RateCode = item.RateCode,
                                        Rate = item.Rate,
                                        Bbf = item.Bbf,
                                        ArrBsnsDate = businessDate,
                                        RecStatus = ResDetailStatus.I.ToString(),
                                        HoldDate = item.HoldDate,
                                        Remark = item.Remark,
                                        Guestname = item.Guestname,
                                        GuestMobile = item.GuestMobile,
                                        Spec = item.Spec,
                                        Cdate = item.Cdate,
                                        BsnsDate = item.BsnsDate,
                                        RegQty = item.RegQty,
                                        ExtType = item.ExtType,
                                        Guestid = item.Guestid,
                                        Profileid = item.Profileid,
                                        ResStatus = item.ResStatus,
                                        IsSettle = false,
                                        InputUser = item.InputUser,
                                        liveInputUser = item.liveInputUser,
                                    });
                                    #endregion
                                    #region 增加新的房价计划
                                    var resDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == item.Regid).ToList();
                                    foreach (var itemPlan in resDetailPlans)
                                    {
                                        if(itemPlan != null && itemPlan.Price != null && itemPlan.Price.HasValue)
                                        {
                                            _pmsContext.ResDetailPlans.Add(new ResDetailPlan { Regid = newRegid, Ratedate = itemPlan.Ratedate, Price = itemPlan.Price });
                                        }
                                    }
                                    #endregion
                                    #region 增加新的客人
                                    var reginfoList = _pmsContext.RegInfos.Where(c => c.Hid == hid && c.Regid == item.Regid).ToList();
                                    foreach (var reginfoItem in reginfoList)
                                    {
                                        _pmsContext.RegInfos.Add(new RegInfo
                                        {
                                            Address = reginfoItem.Address,
                                            Birthday = reginfoItem.Birthday,
                                            CarNo = reginfoItem.CarNo,
                                            Cerid = reginfoItem.Cerid,
                                            CerType = reginfoItem.CerType,
                                            City = reginfoItem.City,
                                            Email = reginfoItem.Email,
                                            Gender = reginfoItem.Gender,
                                            GuestEName = reginfoItem.GuestEName,
                                            GuestName = reginfoItem.GuestName,
                                            Hid = reginfoItem.Hid,
                                            Id = Guid.NewGuid(),
                                            Interest = reginfoItem.Interest,
                                            IsMast = reginfoItem.IsMast,
                                            Mobile = reginfoItem.Mobile,
                                            Qq = reginfoItem.Qq,
                                            Regid = newRegid,
                                            Nation = reginfoItem.Nation,
                                        });
                                    }
                                    #endregion
                                }
                            }
                            index++;
                        }
                        _pmsContext.Entry(item).State = EntityState.Modified;
                    }
                    #endregion
                }
            }

            var regids_UseScore = GetRateIsUseScoreByRegIds(hid, rateCodeIdsTemp);
            if(regids_UseScore != null && regids_UseScore.Count > 0)
            {
                #region 积分换房
                if (useScoreSaveContinue == "1")
                {
                    JsonResultData useScoreToCheckin_Result = JsonResultData.Failure("积分换房失败！");
                    try
                    {
                        using (var tc = new System.Transactions.TransactionScope())
                        {
                            _pmsContext.SaveChanges();
                            useScoreToCheckin_Result = ExecUseScoreToCheckin(currentInfo, regids_UseScore);
                            if (useScoreToCheckin_Result != null && useScoreToCheckin_Result.Success)
                            {
                                tc.Complete();
                            }
                            else
                            {
                                throw new Exception("");
                            }
                        }
                    }
                    catch { return useScoreToCheckin_Result; }
                }
                else
                {
                    _pmsContext.ResDetailPlans.Where(c => regids_UseScore.Contains(c.Regid)).ToList();
                    return CheckUseScoreToCheckin(hid, regids_UseScore);
                }
                #endregion
            }
            else
            {
                //保存
                _pmsContext.SaveChanges();
            }
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resId, data[0].Key, "", out regIdsAll);
            result.SelectRegIdIsNewCheckIn = isUpdate;
            #region 重复证件号提示
            if(regidsTemp != null && regidsTemp.Count > 0)
            {
                string msgTemp = "";
                var resdetailsTemp = result.ResDetailInfos.Where(c => regidsTemp.Contains(c.Regid) && c.Hid == hid && c.Status == "I").ToList();
                foreach (var item in resdetailsTemp)
                {
                    msgTemp += (ExistsCerId(hid, item) + ExistsProfileId(hid, item));
                }
                result.OtherMessage = msgTemp;
            }
            #endregion
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            #region 订单状态变更通知
            if (isUpdate)
            {
                _notifyService.NotifyOtaResChanged(hid, isEnvTest, resDetailList[0].Sourceid, resId);
            }
            #endregion
            return JsonResultData.Successed(result);
        }

        /// <summary>
        /// 清除分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">订单ID</param>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <returns></returns>
        public JsonResultData ClearRooms(string hid, string resId, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data)
        {
            //验证并过滤数据
            string msg = "";
            bool isTrue = Validate(data, out msg);
            if (!isTrue)
            {
                return JsonResultData.Failure(msg);
            }
            if (data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("请勾选房号！");
            }
            //获取主单
            var resEntity = _pmsContext.Reses.Where(c => c.Resid == resId && c.Hid == hid).AsNoTracking().FirstOrDefault();
            if (resEntity == null)
            {
                return JsonResultData.Failure("订单不存在！");
            }
            //获取子单
            var resDetailList = _pmsContext.ResDetails.Where(c => c.Resid == resEntity.Resid && c.Hid == resEntity.Hid && c.Resno == resEntity.Resno && c.Status == "R").ToList();
            if (resDetailList == null || resDetailList.Count <= 0)
            {
                return JsonResultData.Failure("订单子单不存在！");
            }
            //清除房号
            foreach (var regItem in data)
            {
                foreach (var room in regItem.Value)
                {
                    var entity = resDetailList.Where(c => c.Regid == regItem.Key && c.Roomid == room.Key && c.RoomNo == room.Value).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.Roomid = "";
                        entity.RoomNo = "";
                        _pmsContext.Entry(entity).State = EntityState.Modified;
                    }
                }
            }
            //保存
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resId, data[0].Key, null, out regIdsAll);
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            return JsonResultData.Successed(result);
        }

        #region 私有方法
        /// <summary>
        /// 验证检查过滤
        /// </summary>
        /// <param name="data">List<KeyValuePairModel<regId:订单明细表主键ID List<KeyValuePairModel<roomId:房间ID roomNo:房号>>>>参数数据</param>
        /// <param name="msg">错误消息</param>
        /// <returns>true成功，false失败</returns>
        private bool Validate(List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, out string msg)
        {
            //验证数据
            if (data != null && data.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(data[i].Key) || data[i].Value == null || data[i].Value.Count <= 0)
                    {
                        data.Remove(data[i]);
                        i--;
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < data[i].Value.Count; j++)
                        {
                            if (string.IsNullOrWhiteSpace(data[i].Value[j].Key) || string.IsNullOrWhiteSpace(data[i].Value[j].Value))
                            {
                                data[i].Value.Remove(data[i].Value[j]);
                                j--;
                                continue;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(data[i].Key) || data[i].Value == null || data[i].Value.Count <= 0)
                    {
                        data.Remove(data[i]);
                        i--;
                        continue;
                    }
                }
            }
            else
            {
                data = new List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>>();
            }
            if (data.GroupBy(c => c.Key).Count() != data.Count)
            {
                msg = "子单不能相同！";
                return false;
            }
            else
            {
                List<KeyValuePairModel<string, string>> temp = new List<KeyValuePairModel<string, string>>();
                foreach (var item in data)
                {
                    temp.AddRange(item.Value);
                }
                foreach (var item in temp)
                {
                    if (temp.Where(c => c.Key == item.Key).Count() > 1)
                    {
                        msg = "房号不能相同！";
                        return false;
                    }
                    if (temp.Where(c => c.Value == item.Value).Count() > 1)
                    {
                        msg = "房号不能相同！";
                        return false;
                    }
                }
            }
            msg = null;
            return true;
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resDetailList">子单列表</param>
        /// <param name="data">参数数据</param>
        private void ValidateData(string hid, List<ResDetail> resDetailList, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data)
        {
            var checkList = GetRoomAutoChoose(hid, resDetailList, false);
            if (checkList != null && checkList.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    var temp = checkList.Where(c => c.Key == data[i].Key).FirstOrDefault();
                    if (temp != null)
                    {
                        for (int j = 0; j < data[i].Value.Count; j++)
                        {
                            if (!temp.Value.Where(c => c.Roomid == data[i].Value[j].Key && c.roomno == data[i].Value[j].Value).Any())
                            {
                                data[i].Value.Remove(data[i].Value[j]);
                                j--;
                            }
                        }
                    }
                    else
                    {
                        data.Remove(data[i]);
                        i--;
                    }
                }
            }
            else
            {
                data = new List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>>();
            }
        }

        /// <summary>
        /// 保存前检查数据有效性
        /// </summary>
        /// <param name="type">类型（R:预订，I:入住）</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="resDetailList">子单列表</param>
        /// <param name="data">参数数据</param>
        /// <returns></returns>
        private JsonResultData CheckSaveBefore(ResDetailStatus type, string hid, List<ResDetail> resDetailList, List<KeyValuePairModel<string, List<KeyValuePairModel<string, string>>>> data, string saveContinue, DateTime? businessDate = null)
        {
            var checkRegIds = new List<string>();
            var checkInfos = new List<string>();
            foreach (var detail in resDetailList)
            {
                var entity = data.Where(c => c.Key == detail.Regid).FirstOrDefault();
                if (entity != null && entity.Value != null && entity.Value.Count > 0)
                {
                    checkRegIds.Add(detail.Regid);
                    foreach (var item in entity.Value)
                    {
                        checkInfos.Add(UpRoomEnableCheck_checkInfo(item.Key, detail.RoomTypeid, (type == ResDetailStatus.I ? businessDate.ToDateString() : detail.ArrDate.ToString()), detail.DepDate.ToString(), "1", detail.Sourceid));
                    }
                }
            }
            return UpRoomEnableCheck(hid, checkRegIds, type, checkInfos, saveContinue);
        }

        /// <summary>
        /// 自动分房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resDetailList">子单列表</param>
        /// <param name="isAuto">是否自动分房（true是，false否）</param>
        /// <returns>List<KeyValuePairModel<regid, List<UpQueryRoomAutoChooseResult>>></returns>
        private List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>> GetRoomAutoChoose(string hid, List<ResDetail> resDetailList, bool isAuto)
        {
            List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>> resultList = new List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>>();
            if (resDetailList != null && resDetailList.Count > 0)
            {
                if (isAuto)
                {
                    #region 自动
                    //记录 regid 对应的 seqid列表
                    List<KeyValuePairModel<string, List<int>>> tempList = new List<KeyValuePairModel<string, List<int>>>();
                    //配置参数
                    StringBuilder checkinfo = new StringBuilder();
                    checkinfo.Append("<rooms>");
                    int index = 1000;
                    foreach (var item in resDetailList)
                    {
                        if (item.Status == "R" && item.RoomQty > 0 && !string.IsNullOrWhiteSpace(item.RoomTypeid) && item.ArrDate.HasValue && item.DepDate.HasValue)
                        {
                            List<int> tempValue = new List<int>();
                            for (int i = 1; i <= item.RoomQty; i++)
                            {
                                checkinfo.AppendFormat("<room seqid=\"{3}\" roomtypeid=\"{0}\" arrdate=\"{1}\" depdate=\"{2}\"></room>", item.RoomTypeid, item.ArrDate.ToString(), item.DepDate.ToString(), (index + i).ToString());
                                tempValue.Add(index + i);
                            }
                            tempList.Add(new KeyValuePairModel<string, List<int>> { Key = item.Regid, Value = tempValue });
                            index = index + 1000;
                        }
                    }
                    checkinfo.Append("</rooms>");
                    //获取数据
                    List<UpQueryRoomAutoChooseResult> itemList = UpRoomAutoChoose(hid, ResDetailStatus.R, checkinfo.ToString());
                    if (itemList != null && itemList.Count > 0)
                    {
                        //循环记录
                        foreach (var tempItem in tempList)
                        {
                            //从返回值中检索
                            var tempVaule = itemList.Where(c => tempItem.Value.Contains(c.seqid)).OrderBy(o => (o.isService * 1000) + (o.isStop * 100) + ((o.isDirty == 1 ? 1 : 0) * 10) + (o.isReg)).ThenBy(o => o.orderBy).ToList();
                            if (tempVaule != null && tempVaule.Count > 0)
                            {
                                resultList.Add(new KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>
                                {
                                    Key = tempItem.Key,
                                    Value = tempVaule
                                });
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 手动
                    //1.过滤数据
                    var newResDetailList = resDetailList.Where(c => c.Status == "R" && c.RoomQty > 0 && !string.IsNullOrWhiteSpace(c.RoomTypeid) && c.ArrDate.HasValue && c.DepDate.HasValue).ToList();
                    if (newResDetailList.Count <= 0) { return resultList; }
                    //2.分组数据
                    List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>> groupList = new List<KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>>();
                    //3.获取分组数据
                    var regidList = newResDetailList.GroupBy(c => new { c.RoomTypeid, c.ArrDate, c.DepDate }).Select(c => c.Key).ToList();
                    foreach (var item in regidList)
                    {
                        StringBuilder checkinfo = new StringBuilder();
                        checkinfo.Append("<rooms>");
                        checkinfo.AppendFormat("<room seqid=\"0\" roomtypeid=\"{0}\" arrdate=\"{1}\" depdate=\"{2}\"></room>", item.RoomTypeid, item.ArrDate.ToString(), item.DepDate.ToString());
                        checkinfo.Append("</rooms>");
                        groupList.Add(new KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>
                        {
                            Key = string.Format("&&{0}&&{1}&&{2}&&", item.RoomTypeid, item.ArrDate.ToString(), item.DepDate.ToString()),
                            Value = UpRoomAutoChoose(hid, ResDetailStatus.R, checkinfo.ToString())
                        });
                    }
                    //3.分配
                    foreach (var item in newResDetailList)
                    {
                        //检索
                        string key = string.Format("&&{0}&&{1}&&{2}&&", item.RoomTypeid, item.ArrDate.ToString(), item.DepDate.ToString());
                        var entity = groupList.Where(c => c.Key == key).FirstOrDefault();
                        //赋值
                        List<UpQueryRoomAutoChooseResult> value = new List<UpQueryRoomAutoChooseResult>();
                        if (entity != null && entity.Value != null && entity.Value.Count > 0)
                        {
                            value.AddRange(entity.Value);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Roomid) && !string.IsNullOrWhiteSpace(item.RoomNo))
                        {
                            var selectItem = value.Where(c => c.Roomid == item.Roomid && c.roomno == item.RoomNo).FirstOrDefault();
                            if (selectItem != null)
                            {
                                selectItem.remark = "已选择";
                            }
                            else
                            {
                                var room = _pmsContext.Rooms.Where(r => r.Id == item.Roomid &&          r.Hid == hid).FirstOrDefault();
                                string preFix = "";
                                if (room != null)
                                {
                                    preFix = room.PreFix ?? "";
                                }
                                value.Insert(0, new UpQueryRoomAutoChooseResult
                                {
                                    remark = "已选择",
                                    Roomid = item.Roomid,
                                    roomno = item.RoomNo,
                                    preFix = preFix,
                                    seqid = 0
                                });
                            }
                        }
                        resultList.Add(new KeyValuePairModel<string, List<UpQueryRoomAutoChooseResult>>
                        {
                            Key = item.Regid,
                            Value = value.OrderBy(o => (o.isService * 1000) + (o.isStop * 100) + ((o.isDirty == 1 ? 1 : 0) * 10) + (o.isReg)).ThenBy(o => o.orderBy).ToList()
                        });
                    }
                    #endregion
                }
            }
            return resultList;
        }
        #endregion


        #endregion

        /// <summary>
        /// 查询国籍
        /// </summary>
        /// <param name="nation">名称或拼音</param>
        /// <returns></returns>
        public List<V_Nation> GetNationList(string nation)
        {
            var nationResults = _pmsContext.Database.SqlQuery<V_Nation>("select code,name,py from v_nation where py like (@nation + '%') or name like (@nation + '%')"
                , new SqlParameter("@nation", string.IsNullOrWhiteSpace(nation) ? "" : nation)
                ).ToList();
            return nationResults;
        }

        #region 查询订单
        public ResDetail GetResDetailRegid(string hid, string regid)
        {
            return _pmsContext.ResDetails.Where(s => s.Regid == regid && s.Hid == hid).FirstOrDefault();
        }
        #endregion

        #region 客人信息批量修改
        /// <summary>
        /// 根据主单ID获取主单内所有客人信息列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">主单ID</param>
        /// <returns></returns>
        public List<ResDetailCustomerInfos> GetCustomerInfoByResId(string hid, string resId)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(resId))
            {
                return null;
            }
            if(!_pmsContext.Reses.Where(c => c.Hid == hid && c.Resid == resId).AsNoTracking().Any())
            {
                return null;
            }
            var resDetails = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == resId && c.Status == "I").Select(c => new { c.Regid, c.RoomNo }).AsNoTracking().ToList();
            if(resDetails == null || resDetails.Count <= 0)
            {
                return null;
            }
            List<string> regIds = resDetails.Select(c => c.Regid).ToList();
            var regInfos = _pmsContext.RegInfos.Where(c => c.Hid == hid && regIds.Contains(c.Regid)).AsNoTracking().ToList();
            if(regInfos == null || regInfos.Count <= 0)
            {
                return null;
            }
            var result = regInfos.Join(resDetails, c => c.Regid, s => s.Regid, (c,s) => new ResDetailCustomerInfos { Id = c.Id, Regid = c.Regid, RoomNo = s.RoomNo, GuestName = c.GuestName, Gender = c.Gender, Birthday = c.Birthday, CerType = c.CerType, Cerid = c.Cerid, Nation = c.Nation, City = c.City, Address = c.Address,PhotoUrl=c.PhotoUrl });
            return result.OrderBy(c => c.Regid).ToList();
        }

        /// <summary>
        /// 保存客人信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="data">客人信息列表</param>
        /// <returns></returns>
        public JsonResultData SaveCustomerInfos(string hid, List<ResDetailCustomerInfos> data, string userName)
        {
            #region 参数检查
            if (data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("请填写客人信息！");
            }
            foreach (var item in data)
            {
                if (string.IsNullOrWhiteSpace(item.GuestName))
                {
                    return JsonResultData.Failure("客人姓名不能为空！");
                }
                if (string.IsNullOrWhiteSpace(item.Regid))
                {
                    return JsonResultData.Failure("订单号错误！");
                }
                if (item.Id == Guid.Empty || item.Id == null)
                {
                    return JsonResultData.Failure("客人信息错误！");
                }
            }
            var originRegids = data.Select(c => c.Regid).ToList();
            var newRegids = _pmsContext.ResDetails.Where(c => c.Hid == hid && originRegids.Contains(c.Regid) && c.Status == "I").Select(c => c.Regid).ToList();
            data = data.Where(c => newRegids.Contains(c.Regid)).ToList();
            if (data == null || data.Count <= 0)
            {
                return JsonResultData.Failure("此订单数据已过时，请获取订单最新数据！");
            }
            #endregion

            string format = "{0}：{1}=>{2}，";
            StringBuilder logs = new StringBuilder();
            //更新客人信息
            List<ResDetailRegInfo> checklist = new List<ResDetailRegInfo>();
            var regids = data.Select(c => c.Regid).ToList();
            var list = _pmsContext.RegInfos.Where(c => c.Hid == hid && regids.Contains(c.Regid)).ToList();
            foreach (var item in data)
            {
                var entity = list.Where(c => c.Hid == hid && c.Regid == item.Regid && c.Id == item.Id).FirstOrDefault();
                if(entity != null)
                {
                    string customers = "";
                    if (entity.GuestName != item.GuestName)
                    {
                        customers += string.Format(format, "客人姓名", entity.GuestName, item.GuestName);
                        entity.GuestName = item.GuestName;
                    }
                    if (entity.Gender != item.Gender)
                    {
                        customers += string.Format(format, "性别", entity.Gender == "F" ? "女" : "男", item.Gender == "F" ? "女" : "男");
                        entity.Gender = item.Gender;
                    }
                    if (entity.Birthday != item.Birthday)
                    {
                        customers += string.Format(format, "生日", entity.Birthday.ToDateString(), item.Birthday.ToDateString());
                        entity.Birthday = item.Birthday;
                    }
                    if (entity.CerType != item.CerType)
                    {
                        customers += string.Format(format, "证件类型", GetNameById("cerType", hid, entity.CerType), GetNameById("cerType", hid, item.CerType));
                        entity.CerType = item.CerType;
                    }
                    if (entity.Cerid != item.Cerid)
                    {
                        customers += string.Format(format, "证件号", entity.Cerid, item.Cerid);
                        entity.Cerid = item.Cerid;
                    }
                    if(entity.Nation != item.Nation)
                    {
                        customers += string.Format(format, "国籍", entity.Nation, item.Nation);
                        entity.Nation = item.Nation;
                    }
                    if (entity.City != item.City)
                    {
                        customers += string.Format(format, "籍贯", entity.City, item.City);
                        entity.City = item.City;
                    }
                    if (entity.Address != item.Address)
                    {
                        customers += string.Format(format, "地址", entity.Address, item.Address);
                        entity.Address = item.Address;
                    }
                    if (entity.PhotoUrl != item.PhotoUrl)
                    {
                           var photo = UploadPhoto(entity.Hid,item.PhotoUrl);
                        if(!string.IsNullOrWhiteSpace(photo))
                        {
                            entity.PhotoUrl = photo;
                        }                        
                    }
                    if (!string.IsNullOrWhiteSpace(customers))
                    {
                        _pmsContext.Entry(entity).State = EntityState.Modified;

                        if (customers.EndsWith("，"))
                        {
                            customers = customers.Remove(customers.Length - 1);
                        }
                        logs.AppendFormat("[客人姓名：{0}，{1}]，", item.GuestName, customers);
                    }

                    checklist.Add(new ResDetailRegInfo { Address =entity.Address, Birthday =entity.Birthday.ToString(), CarNo =entity.CarNo, City =entity.City, Email =entity.Email, Gender = entity.Gender, GuestName = entity.GuestName, Interest = entity.Interest, IsMast = entity.IsMast, Mobile = entity.Mobile, Nation = entity.Nation, Qq = entity.Qq, CerId = entity.Cerid, CerType = entity.CerType, Id = entity.Id.ToString(), RegId = entity.Regid, PhotoUrl = entity.PhotoUrl });
                }
            }
            var resDetails = _pmsContext.ResDetails.Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.Status == "I").OrderBy(c => c.Regid).ToList();
            if(resDetails != null && resDetails.Count > 0)
            {
                //更新子单信息
                foreach (var item in resDetails)
                {
                    string Guestname = null;
                    Guid? GuestId = null;
                    var mastRegInfo = _pmsContext.RegInfos.Where(c => c.Hid == hid && c.Regid == item.Regid).OrderByDescending(c => c.IsMast).Select(c => new { c.GuestName, c.Id }).FirstOrDefault();
                    if (mastRegInfo != null)
                    {
                        Guestname = data.Where(c => c.Regid == item.Regid && c.Id == mastRegInfo.Id).Select(c => c.GuestName).FirstOrDefault();
                        GuestId = data.Where(c => c.Regid == item.Regid && c.Id == mastRegInfo.Id).Select(c => c.GuestId).FirstOrDefault();
                    }
                    if(GuestId == null || !GuestId.HasValue)
                    {
                        GuestId = data.Where(c => c.Regid == item.Regid && c.GuestId != null && c.GuestId.HasValue).Select(c => c.GuestId).FirstOrDefault();
                    }
                    item.Guestname = Guestname;
                    if(GuestId != null && GuestId.HasValue){ item.Guestid = GuestId; }
                    _pmsContext.Entry(item).State = EntityState.Modified;
                }
                //更新主单信息
                var resid = resDetails[0].Resid;
                var resEntity = _pmsContext.Reses.Where(c => c.Hid == hid && c.Resid == resid).FirstOrDefault();
                if(resEntity != null)
                {
                    if ((resEntity.ResCustName == "新预订客人" && resEntity.Name == "新预订客人") || (resEntity.ResCustName == "新入住客人" && resEntity.Name == "新入住客人"))
                    {
                        resEntity.ResCustName = resDetails[0].Guestname;
                        resEntity.Name = resDetails[0].Guestname;
                        _pmsContext.Entry(resEntity).State = EntityState.Modified;
                    }
                }
            }
            string logsText = logs.ToString();
            if (!string.IsNullOrWhiteSpace(logsText))
            {
                if (logsText.EndsWith("，"))
                {
                    logsText = logsText.Remove(logsText.Length - 1);
                }
                _pmsContext.OpLogs.Add(new OpLog
                {
                    CDate = DateTime.Now,
                    CText = (logsText.Length > 4000 ? logsText.Substring(0, 4000) : logsText),
                    CUser = userName,
                    Hid = hid,
                    Ip = UrlHelperExtension.GetRemoteClientIPAddress(),
                    Keys = (list != null && list.Count > 0 ? list[0].Regid : ""),
                    XType = Gemstar.BSPMS.Common.Services.Enums.OpLogType.客情维护.ToString(),
                });
            }
            _pmsContext.SaveChanges();
            var regIdsAll = "";
            var result = GetResMainInfo(hid, resDetails[0].Resid, resDetails[0].Regid, null, out regIdsAll);
            result.SelectRegIdIsNewCheckIn = false;
            result.OtherMessage = ExistsCerId(hid, new ResDetailInfo { OrderDetailRegInfos = checklist, Status = "I" });
            #region 调用存储过程更改房态
            _pmsContext.Database.ExecuteSqlCommand("exec up_RoomStatusSet @hid=@hid,@opType=1,@ids=@ids"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@ids", regIdsAll)
                );
            #endregion
            return JsonResultData.Successed(result);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 获取子单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号</param>
        /// <returns></returns>
        public ResDetail GetResDetail(string hid, string regid)
        {
            return _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// 查询房间号在住客人信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomNo"></param>
        /// <returns></returns>
        public string GetResDetailByRoomNo(string hid,string roomNo)
        {
             var regid =_pmsContext.ResDetails.Where(c => c.Hid == hid && c.RoomNo == roomNo && c.Status == "I").Select(c=>c.Regid).AsNoTracking().FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(regid))
            {
                var guestname = _pmsContext.RegInfos.Where(c => c.Regid == regid && c.Hid == hid).Select(c => c.GuestName);
                return string.Join("，", guestname);
            }
            return null;
        }
        /// <summary>
        /// 获取当前营业日房价
        /// </summary>
        /// <param name="regid">账号</param>
        /// <param name="businessDate">当前营业日</param>
        /// <returns></returns>
        public decimal? GetCurrentResDetailPrice(string regid, DateTime businessDate)
        {
            decimal? price = null;
            var resDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == regid).AsNoTracking().ToList();
            if(resDetailPlans != null && resDetailPlans.Count > 0)
            {
                price = resDetailPlans.Where(c => c.Ratedate == businessDate).Select(c => c.Price).FirstOrDefault();
                if(price == null)
                {
                    var minDate = resDetailPlans.Select(c => c.Ratedate).Min();
                    var maxDate = resDetailPlans.Select(c => c.Ratedate).Max();
                    if(businessDate <= minDate)
                    {
                        price = resDetailPlans.Where(c => c.Ratedate == minDate).Select(c => c.Price).FirstOrDefault();
                    }
                    if (businessDate >= maxDate)
                    {
                        price = resDetailPlans.Where(c => c.Ratedate == maxDate).Select(c => c.Price).FirstOrDefault();
                    }
                }
            }
            return price;
        }
        /// <summary>
        /// 获取订单内的特殊要求
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public JsonResultData GetSpecialRequirements(string hid, string resid)
        {
            string msg = "";
            var list = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == resid && c.Spec != null && c.Spec.Trim().Length > 0).Select(s => new { s.Regid, s.RoomNo, s.Spec }).AsNoTracking().ToList();
            if(list != null && list.Count > 0)
            {
                list = list.OrderBy(c => c.Regid).ToList();
                foreach(var item in list)
                {
                    string title = "房号";
                    string value = item.RoomNo;
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        title = "账号";
                        value = item.Regid.Replace(hid,"");
                    }
                    msg += string.Format("{0}：{1}，特殊要求：{2}。\n", title, value, item.Spec);
                }
            }

            msg += GetResOrderTips(hid, resid);

            if (!string.IsNullOrWhiteSpace(msg))
            {
                return JsonResultData.Successed(msg);
            }
            
            return JsonResultData.Failure("");
        }


        /// <summary>
        /// 查询在线门锁信息
        /// </summary>
        /// <param name="queryPara">查询参数</param>
        /// <returns>查询结果</returns>
        public OnlineLockQueryResult GetOnlineLockPara(OnlineLockQueryPara queryPara)
        {
            var result = new OnlineLockQueryResult
            {
                QuerySuccessed = false
            };
            if (queryPara.IsValid)
            {
                var regid = queryPara.QueryParaValue;
                //如果是根据transid进行查询的话，则先根据transid取到对应的regid
                if (queryPara.Type == OnlineLockQueryParaType.QueryByTransId)
                {
                    var transid = Guid.Empty;
                    if (Guid.TryParse(queryPara.QueryParaValue, out transid))
                    {
                        if (transid != Guid.Empty)
                        {
                            regid = _pmsContext.ResFolios.Where(c => c.Hid == queryPara.Hid && c.Transid == transid).Select(c => c.Regid).AsNoTracking().FirstOrDefault();
                        }
                    }
                }
                //如果有正确的regid
                if (!string.IsNullOrWhiteSpace(regid))
                {
                    var entity = _pmsContext.ResDetails.Where(c => c.Hid == queryPara.Hid && c.Regid == regid).Select(c => new { c.Regid, c.Roomid, c.Guestname, c.GuestMobile,c.RoomNo }).AsNoTracking().FirstOrDefault();
                    if (entity != null)
                    {
                        result.RegId = entity.Regid;
                        result.RoomCode = entity.Roomid;
                        result.Name = entity.Guestname;
                        result.Mobile = entity.GuestMobile;
                        result.RoomNo = entity.RoomNo;
                        result.QuerySuccessed = true;
                        //取出主客的证件号
                        result.CerNo = _pmsContext.RegInfos.Where(w => w.Hid == queryPara.Hid && w.Regid == regid && w.IsMast == "1").Select(w => w.Cerid).FirstOrDefault();
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// 根据主单ID获取子单列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public List<KeyValuePairModel<string, string>> GetResDetailsByResId(string hid, string resid)
        {
            return _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == resid).Select(c => new KeyValuePairModel<string,string>{ Key = c.Regid, Value = c.RoomNo, Data = c.Regid.Replace(c.Hid,"") }).AsNoTracking().ToList();
        }

        /// <summary>
        /// 根据证件类型和证件号码获取会员信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="cerType"></param>
        /// <param name="cerId"></param>
        public JsonResultData GetProfileInfoByCerId(string hid, string cerType, string cerId, bool isGroup)
        {
            DateTime nowDate = DateTime.Now;
            var result = _pmsContext.MbrCards.Where(c => (isGroup == false ? c.Hid == hid : c.Grpid == hid) && c.CerType == cerType && c.Cerid == cerId && c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate).Select(w => new { Profileid = w.Id, ProfileNo = w.MbrCardNo, ProfileName = w.GuestName, ProfileMobile = w.Mobile }).FirstOrDefault();
            if(result != null)
            {
                return JsonResultData.Successed(result);
            }
            return JsonResultData.Failure("根据证件号没有找到会员信息！");
        }

        /// <summary>
        /// 获取提示信息（长包房、生日）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public string GetResOrderTips(string hid, string resid)
        {
            string resultMessage = null;
            var resDeatils = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == resid).AsNoTracking().ToList();//订单
            var regIds = resDeatils.Select(c => c.Regid).ToList();
            var rateCodeIds = resDeatils.Select(c => c.RateCode).Distinct().ToList();

            //长包房
            var permanentRoomSet_RateCodeIds = _pmsContext.Rates.Where(c => c.Hid == hid && rateCodeIds.Contains(c.Id) && c.isMonth == true).Select(c => c.Id).AsNoTracking().ToList();//长包房价格代码
            if (permanentRoomSet_RateCodeIds != null && permanentRoomSet_RateCodeIds.Count > 0)
            {
                var permanentRoomSet_AllRegIds = resDeatils.Where(c => permanentRoomSet_RateCodeIds.Contains(c.RateCode) && c.Status == "I").Select(c => c.Regid).ToList();//在住长包房订单
                var permanentRoomSet_RegIds = _pmsContext.PermanentRoomSets.Where(c => c.Hid == hid && permanentRoomSet_AllRegIds.Contains(c.Regid)).Select(c => c.Regid).AsNoTracking().ToList();//订单长包房设置
                foreach(var regid in permanentRoomSet_AllRegIds)
                {
                    if (!permanentRoomSet_RegIds.Contains(regid))
                    {
                        resultMessage += string.Format("账号：{0}，请添加长包房设置！\n", regid.Substring(hid.Length));
                    }
                }
            }
            //生日
            var regInfos = _pmsContext.RegInfos.Where(w => w.Hid == hid && regIds.Contains(w.Regid)).Select( s => new { s.Birthday, s.Regid, s.GuestName }).AsNoTracking().ToList();
            foreach(var reg in regInfos)
            {
                if (reg.Birthday != null && reg.Birthday.HasValue)
                {
                    bool isToday = reg.Birthday.Value.ToString("MM-dd") == DateTime.Now.ToString("MM-dd");
                    bool isTomorrow = reg.Birthday.Value.ToString("MM-dd") == DateTime.Now.AddDays(1).ToString("MM-dd");
                    if (isToday || isTomorrow)
                    {
                        resultMessage += string.Format("账号：{0}，客人：{1}，{2}生日！\n", reg.Regid.Substring(hid.Length), reg.GuestName, (isToday ? "今天" : "明天"));
                    }
                }
            }
            return resultMessage;
        }
        #endregion

        #region 调价记录功能
        /// <summary>
        /// 获取订单的房价计划 和 价格代码
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号</param>
        /// <param name="rateCode">价格代码</param>
        /// <returns></returns>
        public List<ResDetailPlan> GetResDetailPlans(string hid, string regid, out string rateCode)
        {
            rateCode = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid).Select(c => c.RateCode).AsNoTracking().FirstOrDefault();
            return _pmsContext.ResDetailPlans.Where(c => c.Regid == regid).AsNoTracking().ToList();
        }
        /// <summary>
        /// 验证是否调价，并且记录调价日志
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号</param>
        /// <param name="oldRateCode">调价前的 价格代码</param>
        /// <param name="oldResDetailPlans">调价前的 价格体系</param>
        /// <param name="userName">操作员</param>
        /// <param name="ip">操作IP</param>
        /// <returns></returns>
        public void ResDetailPlanChangedRecords(string hid, string regid, string oldRateCode, List<ResDetailPlan> oldResDetailPlans, string userName, string ip = null)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = UrlHelperExtension.GetRemoteClientIPAddress();
            }
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regid) || string.IsNullOrWhiteSpace(oldRateCode) || oldResDetailPlans == null || oldResDetailPlans.Count <= 0) { return; }
            oldResDetailPlans = oldResDetailPlans.Where(c => c.Regid == regid).ToList();
            if (oldResDetailPlans.Count <= 0) { return; }

            List<ResLog> resLogList = new List<ResLog>();

            var resDetailEntity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid).AsNoTracking().FirstOrDefault();
            if(resDetailEntity == null) { return; }
            if(resDetailEntity != null && resDetailEntity.RateCode != oldRateCode)
            {
                //价格代码改变
                var rateCodeList = _pmsContext.Rates.Where(c => c.Hid == hid && (c.Id == oldRateCode || c.Id == resDetailEntity.RateCode)).Select(c => new { c.Id, c.Name }).AsNoTracking().ToList();
                var oldRateCodeName = rateCodeList.Where(c => c.Id == oldRateCode).Select(c => c.Name).FirstOrDefault();
                var newRateCodeName = rateCodeList.Where(c => c.Id == resDetailEntity.RateCode).Select(c => c.Name).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(oldRateCodeName)) { oldRateCodeName = oldRateCode;  }
                if (string.IsNullOrWhiteSpace(newRateCodeName)) { newRateCodeName = resDetailEntity.RateCode; }
                resLogList.Add(new ResLog { Hid = hid, Regid = regid, XType = 1, Value1 = oldRateCodeName, Value2 = newRateCodeName, Other1 = resDetailEntity.Guestname, CDate = DateTime.Now, CUser = userName, Ip = ip, Id = Guid.NewGuid() });
            }

            var newResDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == regid).AsNoTracking().ToList();
            foreach(var item in oldResDetailPlans)
            {
                var newPrice = newResDetailPlans.Where(c => c.Ratedate == item.Ratedate).Select(c => c.Price).FirstOrDefault();
                if(item.Price != newPrice)
                {
                    //价格变化
                    resLogList.Add(new ResLog { Hid = hid, Regid = regid, XType = 1, Value1 = (item.Price.HasValue ? item.Price.ToString() : ""), Value2 = (newPrice.HasValue ? newPrice.ToString() : ""), Other1 = resDetailEntity.Guestname, Other2 = item.Ratedate.ToShortDateString(), CDate = DateTime.Now, CUser = userName, Ip = ip, Id = Guid.NewGuid() });
                }
            }

            var oldRatedatas  = oldResDetailPlans.Select(c => c.Ratedate).ToList();
            var addNewResDetailPlans =  newResDetailPlans.Where(c => !oldRatedatas.Contains(c.Ratedate)).ToList();
            foreach(var item in addNewResDetailPlans)
            {
                //价格变化
                resLogList.Add(new ResLog { Hid = hid, Regid = regid, XType = 1, Value1 = "", Value2 = (item.Price.HasValue ? item.Price.ToString() : ""), Other1 = resDetailEntity.Guestname, Other2 = item.Ratedate.ToShortDateString(), CDate = DateTime.Now, CUser = userName, Ip = ip, Id = Guid.NewGuid() });
            }

            if (resLogList != null && resLogList.Count > 0)
            {
                _pmsContext.ResLogs.AddRange(resLogList);
                _pmsContext.SaveChanges();
            }
        }
        #endregion

        #region 验证证件号是否相同
        /// <summary>
        /// 获取重复的证件号码
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="notCustomerId">不包含的客人ID</param>
        /// <param name="cerType">证件类型</param>
        /// <param name="cerId">证件号码</param>
        /// <returns></returns>
        public string ExistsCerId(string hid, ResDetailInfo detailInfo)
        {
            string msg = "";
            try
            {
                if (detailInfo != null)
                {
                    if (detailInfo.Status == ResDetailStatus.I.ToString())
                    {
                        var resDetails = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Status == "I").Select(c => new { c.Regid, c.RoomNo }).AsNoTracking().ToList();
                        if (resDetails != null && resDetails.Count > 0)
                        {
                            var regids = resDetails.Select(c => c.Regid).ToList();
                            foreach (var item in detailInfo.OrderDetailRegInfos)
                            {
                                if (!string.IsNullOrWhiteSpace(item.CerId))
                                {
                                    List<KeyValuePairModel<string, string>> result = new List<KeyValuePairModel<string, string>>();
                                    Guid customerId = Guid.Empty;
                                    Guid.TryParse(item.Id, out customerId);
                                    var customers = _pmsContext.RegInfos.Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.CerType == item.CerType && c.Cerid == item.CerId && c.Id != customerId).Select(c => new { c.Regid, c.GuestName }).AsNoTracking().ToList();
                                    if (customers != null && customers.Count > 0)
                                    {
                                        var list = resDetails.Join(customers, c => c.Regid, d => d.Regid, (c, d) => new KeyValuePairModel<string, string> { Key = c.Regid.Replace(hid, ""), Value = c.RoomNo, Data = d.GuestName }).ToList();
                                        if (list != null && list.Count > 0)
                                        {
                                            result.AddRange(list);
                                        }
                                    }
                                    if (result.Count > 0)
                                    {
                                        msg += string.Format("与\"{0}\"有相同证件号的在住客人：\n", item.GuestName);
                                        foreach (var iitem in result)
                                        {
                                            msg += string.Format("账号：{0}，房号：{1}，客人名：{2}。\n", iitem.Key, iitem.Value, iitem.Data);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return msg;
        }
        #endregion

        #region 验证会员卡号是否相同
        public string ExistsProfileId(string hid, ResDetailInfo detailInfo)
        {
            string msg = "";
            try
            {
                if (detailInfo != null && !string.IsNullOrWhiteSpace(detailInfo.Regid) && !string.IsNullOrWhiteSpace(hid))
                {
                    if(_pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == detailInfo.Regid && c.Status == "I").AsNoTracking().Any())
                    {
                        if (detailInfo.Profileid != null && detailInfo.Profileid != Guid.Empty)
                        {
                            string mbrCardNo = _pmsContext.MbrCards.Where(c => c.Id == detailInfo.Profileid).Select(c => c.MbrCardNo).AsNoTracking().FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(mbrCardNo))
                            {
                                var result = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Profileid == detailInfo.Profileid && c.Regid != detailInfo.Regid && c.Status == "I").Select(c => new { c.Regid, c.RoomNo, c.Guestname }).AsNoTracking().ToList();
                                if (result.Count > 0)
                                {
                                    msg += string.Format("与\"{0}\"有相同会员卡号的在住客人：\n", mbrCardNo);
                                    foreach (var iitem in result)
                                    {
                                        msg += string.Format("账号：{0}，房号：{1}，客人名：{2}。\n", iitem.Regid.Replace(hid, ""), iitem.RoomNo, iitem.Guestname);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return msg;
        }
        #endregion

        #region 更新备注
        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="CurrentInfo">登录信息</param>
        /// <param name="regid">账号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData UpdateRemark(ICurrentInfo CurrentInfo, string regid, string remark)
        {
            string hid = CurrentInfo.HotelId;
            var entity = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Regid == regid && c.Status == ResDetailStatus.C.ToString()).FirstOrDefault();
            if (entity != null)
            {
                if(entity.Remark != remark)
                {
                    AddOperationLog(CurrentInfo, BSPMS.Common.Services.Enums.OpLogType.客单备注修改, string.Format("账号：{0}，原备注：{1}，新备注：{2}。", regid.Replace(CurrentInfo.HotelId, ""), entity.Remark, remark), regid);
                    entity.Remark = remark;
                    _pmsContext.Entry(entity).State = EntityState.Modified;
                    _pmsContext.SaveChanges();
                }

                var regIdsAll = "";
                var result = GetResMainInfo(hid, entity.Resid, entity.Regid, "", out regIdsAll);
                return JsonResultData.Successed(result);
            }
            return JsonResultData.Failure("订单不存在！");
        }
        #endregion

        #region 操作日志
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="currentInfo">登录信息</param>
        /// <param name="type">日志类型</param>
        /// <param name="text">内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(ICurrentInfo currentInfo, Gemstar.BSPMS.Common.Services.Enums.OpLogType type, string text, string keys, bool isSaveChanges = false)
        {
            if (!isSaveChanges)
            {
                _pmsContext.OpLogs.Add(new OpLog
                {
                    CDate = DateTime.Now,
                    Hid = currentInfo.HotelId,
                    CUser = currentInfo.UserName,
                    Ip = UrlHelperExtension.GetRemoteClientIPAddress(),
                    XType = type.ToString(),
                    CText = (text.Length > 4000 ? text.Substring(0, 4000) : text),
                    Keys = keys,
                });
            }
            else
            {
                _pmsContext.Database.ExecuteSqlCommandAsync("INSERT INTO [opLog]([hid],[cDate],[cUser],[ip],[xType],[cText],[keys])VALUES(@hid,@cDate,@cUser,@ip,@xType,@cText,@keys)",
                    new SqlParameter("@hid", currentInfo.HotelId),
                    new SqlParameter("@cDate", DateTime.Now),
                    new SqlParameter("@cUser", currentInfo.UserName),
                    new SqlParameter("@ip", UrlHelperExtension.GetRemoteClientIPAddress()),
                    new SqlParameter("@xType", type.ToString()),
                    new SqlParameter("@cText", (text.Length > 4000 ? text.Substring(0, 4000) : text)),
                    new SqlParameter("@keys", keys)
                    ).Wait();
            }
        }
        #endregion

        #region 保存客情-操作日志
        /// <summary>
        /// 获取 订单明细表及相关联的所有信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="regid"></param>
        /// <returns></returns>
        public ResDetailAllInfo GetResDetailAllInfo(string hid, string regid)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regid)) { return null; }
            var resDetails = _pmsContext.ResDetails.AsNoTracking().FirstOrDefault(c => c.Hid == hid && c.Regid == regid);
            if(resDetails == null || string.IsNullOrWhiteSpace(resDetails.Resid) || string.IsNullOrWhiteSpace(resDetails.Regid)) { return null; }

            var reses = _pmsContext.Reses.AsNoTracking().FirstOrDefault(c => c.Hid == hid && c.Resid == resDetails.Resid);
            var resDetailPlans = _pmsContext.ResDetailPlans.Where(c => c.Regid == resDetails.Regid).OrderBy(c => c.Ratedate).AsNoTracking().ToList();
            var regInfos = _pmsContext.RegInfos.Where(c => c.Hid == hid && c.Regid == resDetails.Regid).OrderByDescending(c => c.IsMast).AsNoTracking().ToList();

            return new ResDetailAllInfo { ResEntity = reses, ResDetailEntity = resDetails, ResDetailPlans = resDetailPlans, RegInfos = regInfos };
        }
        /// <summary>
        /// 保存 客情操作日志
        /// </summary>
        /// <param name="currentInfo">当前酒店信息</param>
        /// <param name="oldResDetailAllInfo">修改之前的信息</param>
        /// <param name="newResDetailAllInfo">修改之后的信息</param>
        public void ResSaveToLog(ICurrentInfo currentInfo, ResDetailAllInfo oldResDetailAllInfo, ResDetailAllInfo newResDetailAllInfo, string authUserName = null,string reason="")
        {
            try
            {
                if (currentInfo == null || string.IsNullOrWhiteSpace(currentInfo.HotelId)) { return; }

                if (newResDetailAllInfo == null || newResDetailAllInfo.ResEntity == null || newResDetailAllInfo.ResDetailEntity == null) { return; }

                string hid = currentInfo.HotelId;
                var type = BSPMS.Common.Services.Enums.OpLogType.预订;
                StringBuilder text = new StringBuilder();
                string keys = "";

                if (oldResDetailAllInfo == null)
                {
                    //插入
                    #region
                    string format = "{0}：{1}，";
                    keys = newResDetailAllInfo.ResDetailEntity.Regid;
                    type = newResDetailAllInfo.ResDetailEntity.Status == ResDetailStatus.I.ToString() ? BSPMS.Common.Services.Enums.OpLogType.入住 : BSPMS.Common.Services.Enums.OpLogType.预订;
                    text.AppendFormat("新{0}　", type.ToString());

                    text.AppendFormat(format, "主单号", newResDetailAllInfo.ResEntity.Resno);
                    text.AppendFormat(format, "账号", GetShortRegid(hid, newResDetailAllInfo.ResDetailEntity.Regid));
                    text.AppendFormat(format, "状态", GetStatusName(newResDetailAllInfo.ResDetailEntity.Status));
                    text.AppendFormat(format, "房号", newResDetailAllInfo.ResDetailEntity.RoomNo);

                    text.AppendFormat(format, "主单名称", newResDetailAllInfo.ResEntity.Name);
                    text.AppendFormat(format, "联系人", newResDetailAllInfo.ResEntity.ResCustName);
                    text.AppendFormat(format, "手机号", newResDetailAllInfo.ResEntity.ResCustMobile);
                    text.AppendFormat(format, "团体/散客", newResDetailAllInfo.ResEntity.IsGroup == 1 ? "团体" : "散客");

                    text.AppendFormat(format, "抵店时间", newResDetailAllInfo.ResDetailEntity.ArrDate.ToDateTimeWithoutSecondString());
                    text.AppendFormat(format, "保留时间", newResDetailAllInfo.ResDetailEntity.HoldDate.ToDateTimeWithoutSecondString());
                    text.AppendFormat(format, "离店时间", newResDetailAllInfo.ResDetailEntity.DepDate.ToDateTimeWithoutSecondString());

                    text.AppendFormat(format, "房间类型", GetNameById("roomTypeId", hid, newResDetailAllInfo.ResDetailEntity.RoomTypeid));
                    text.AppendFormat(format, "房号", newResDetailAllInfo.ResDetailEntity.RoomNo);
                    text.AppendFormat(format, "房数", newResDetailAllInfo.ResDetailEntity.RoomQty);

                    text.AppendFormat(format, "合约单位", GetNameById("companyId", hid, newResDetailAllInfo.ResEntity.Cttid));
                    text.AppendFormat(format, "会员", GetNameById("profileId", hid, newResDetailAllInfo.ResDetailEntity.Profileid));

                    text.AppendFormat(format, "客人名", newResDetailAllInfo.ResDetailEntity.Guestname);
                    text.AppendFormat(format, "客人手机号", newResDetailAllInfo.ResDetailEntity.GuestMobile);

                    text.AppendFormat(format, "价格代码", GetNameById("rateId", hid, newResDetailAllInfo.ResDetailEntity.RateCode));
                    text.AppendFormat(format, "当前房价", newResDetailAllInfo.ResDetailEntity.Rate);
                    text.AppendFormat(format, "早餐份数", newResDetailAllInfo.ResDetailEntity.Bbf);
                    text.AppendFormat(format, "市场分类", GetNameById("marketId", hid, newResDetailAllInfo.ResDetailEntity.Marketid));
                    text.AppendFormat(format, "客人来源", GetNameById("sourceId", hid, newResDetailAllInfo.ResDetailEntity.Sourceid));

                    text.AppendFormat(format, "特殊要求", newResDetailAllInfo.ResDetailEntity.Spec);
                    text.AppendFormat(format, "备注", newResDetailAllInfo.ResDetailEntity.Remark);

                    text.AppendFormat(format, "价格列表", "{" + GetResDetailPlansToString(newResDetailAllInfo.ResDetailPlans) + "}");
                    text.AppendFormat(format, "客人列表", "{" + GetRegInfosToString(hid, newResDetailAllInfo.RegInfos) + "}");
                    #endregion
                }
                else
                {
                    //修改
                    #region
                    string format = "{0}：{1}=>{2}，";

                    #region 主单
                    if (oldResDetailAllInfo.ResEntity != null)
                    {
                        if (oldResDetailAllInfo.ResEntity.Name != newResDetailAllInfo.ResEntity.Name)
                        {
                            text.AppendFormat(format, "主单名称", oldResDetailAllInfo.ResEntity.Name, newResDetailAllInfo.ResEntity.Name);
                        }
                        if (oldResDetailAllInfo.ResEntity.ResCustName != newResDetailAllInfo.ResEntity.ResCustName)
                        {
                            text.AppendFormat(format, "联系人", oldResDetailAllInfo.ResEntity.ResCustName, newResDetailAllInfo.ResEntity.ResCustName);
                        }
                        if (oldResDetailAllInfo.ResEntity.ResCustMobile != newResDetailAllInfo.ResEntity.ResCustMobile)
                        {
                            text.AppendFormat(format, "手机号", oldResDetailAllInfo.ResEntity.ResCustMobile, newResDetailAllInfo.ResEntity.ResCustMobile);
                        }
                        if (oldResDetailAllInfo.ResEntity.IsGroup != newResDetailAllInfo.ResEntity.IsGroup)
                        {
                            text.AppendFormat(format, "团体/散客", oldResDetailAllInfo.ResEntity.IsGroup == 1 ? "团体" : "非团体", newResDetailAllInfo.ResEntity.IsGroup == 1 ? "团体" : "散客");
                        }
                        if (oldResDetailAllInfo.ResEntity.Cttid != newResDetailAllInfo.ResEntity.Cttid)
                        {
                            text.AppendFormat(format, "合约单位", GetNameById("companyId", hid, oldResDetailAllInfo.ResEntity.Cttid), GetNameById("companyId", hid, newResDetailAllInfo.ResEntity.Cttid));
                        }
                    }
                    #endregion
                    #region 子单
                    if (oldResDetailAllInfo.ResDetailEntity != null)
                    {
                        if (oldResDetailAllInfo.ResDetailEntity.Status != newResDetailAllInfo.ResDetailEntity.Status)
                        {
                            text.AppendFormat(format, "状态", GetStatusName(oldResDetailAllInfo.ResDetailEntity.Status), GetStatusName(newResDetailAllInfo.ResDetailEntity.Status));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.ArrDate != newResDetailAllInfo.ResDetailEntity.ArrDate)
                        {
                            text.AppendFormat(format, "抵店时间", oldResDetailAllInfo.ResDetailEntity.ArrDate.ToDateTimeWithoutSecondString(), newResDetailAllInfo.ResDetailEntity.ArrDate.ToDateTimeWithoutSecondString());
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.HoldDate != newResDetailAllInfo.ResDetailEntity.HoldDate)
                        {
                            text.AppendFormat(format, "保留时间", oldResDetailAllInfo.ResDetailEntity.HoldDate.ToDateTimeWithoutSecondString(), newResDetailAllInfo.ResDetailEntity.HoldDate.ToDateTimeWithoutSecondString());
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.DepDate != newResDetailAllInfo.ResDetailEntity.DepDate)
                        {
                            text.AppendFormat(format, "离店时间", oldResDetailAllInfo.ResDetailEntity.DepDate.ToDateTimeWithoutSecondString(), newResDetailAllInfo.ResDetailEntity.DepDate.ToDateTimeWithoutSecondString());
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.RoomTypeid != newResDetailAllInfo.ResDetailEntity.RoomTypeid)
                        {
                            text.AppendFormat(format, "房间类型", GetNameById("roomTypeId", hid, oldResDetailAllInfo.ResDetailEntity.RoomTypeid), GetNameById("roomTypeId", hid, newResDetailAllInfo.ResDetailEntity.RoomTypeid));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.RoomNo != newResDetailAllInfo.ResDetailEntity.RoomNo)
                        {
                            text.AppendFormat(format, "房号", oldResDetailAllInfo.ResDetailEntity.RoomNo, newResDetailAllInfo.ResDetailEntity.RoomNo);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.RoomQty != newResDetailAllInfo.ResDetailEntity.RoomQty)
                        {
                            text.AppendFormat(format, "房数", oldResDetailAllInfo.ResDetailEntity.RoomQty, newResDetailAllInfo.ResDetailEntity.RoomQty);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Profileid != newResDetailAllInfo.ResDetailEntity.Profileid)
                        {
                            text.AppendFormat(format, "会员", GetNameById("profileId", hid, oldResDetailAllInfo.ResDetailEntity.Profileid), GetNameById("profileId", hid, newResDetailAllInfo.ResDetailEntity.Profileid));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Guestname != newResDetailAllInfo.ResDetailEntity.Guestname)
                        {
                            text.AppendFormat(format, "客人名", oldResDetailAllInfo.ResDetailEntity.Guestname, newResDetailAllInfo.ResDetailEntity.Guestname);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.GuestMobile != newResDetailAllInfo.ResDetailEntity.GuestMobile)
                        {
                            text.AppendFormat(format, "客人手机号", oldResDetailAllInfo.ResDetailEntity.GuestMobile, newResDetailAllInfo.ResDetailEntity.GuestMobile);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.RateCode != newResDetailAllInfo.ResDetailEntity.RateCode)
                        {
                            text.AppendFormat(format, "价格代码", GetNameById("rateId", hid, oldResDetailAllInfo.ResDetailEntity.RateCode), GetNameById("rateId", hid, newResDetailAllInfo.ResDetailEntity.RateCode));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Rate != newResDetailAllInfo.ResDetailEntity.Rate)
                        {
                            text.AppendFormat(format, "当前房价", oldResDetailAllInfo.ResDetailEntity.Rate, newResDetailAllInfo.ResDetailEntity.Rate);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Bbf != newResDetailAllInfo.ResDetailEntity.Bbf)
                        {
                            text.AppendFormat(format, "早餐份数", oldResDetailAllInfo.ResDetailEntity.Bbf, newResDetailAllInfo.ResDetailEntity.Bbf);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Marketid != newResDetailAllInfo.ResDetailEntity.Marketid)
                        {
                            text.AppendFormat(format, "市场分类", GetNameById("marketId", hid, oldResDetailAllInfo.ResDetailEntity.Marketid), GetNameById("marketId", hid, newResDetailAllInfo.ResDetailEntity.Marketid));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Sourceid != newResDetailAllInfo.ResDetailEntity.Sourceid)
                        {
                            text.AppendFormat(format, "客人来源", GetNameById("sourceId", hid, oldResDetailAllInfo.ResDetailEntity.Sourceid), GetNameById("sourceId", hid, newResDetailAllInfo.ResDetailEntity.Sourceid));
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Spec != newResDetailAllInfo.ResDetailEntity.Spec)
                        {
                            text.AppendFormat(format, "特殊要求", oldResDetailAllInfo.ResDetailEntity.Spec, newResDetailAllInfo.ResDetailEntity.Spec);
                        }
                        if (oldResDetailAllInfo.ResDetailEntity.Remark != newResDetailAllInfo.ResDetailEntity.Remark)
                        {
                            text.AppendFormat(format, "备注", oldResDetailAllInfo.ResDetailEntity.Remark, newResDetailAllInfo.ResDetailEntity.Remark);
                        }
                    }
                    #endregion
                    #region 价格
                    if (oldResDetailAllInfo.ResDetailPlans != null)
                    {
                        string prices = "";
                        string proceFormat = "[{0}：{1}=>{2}]，";
                        List<DateTime> allRatedates = new List<DateTime>();
                        allRatedates.AddRange(oldResDetailAllInfo.ResDetailPlans.Select(c => c.Ratedate).ToList());
                        allRatedates.AddRange(newResDetailAllInfo.ResDetailPlans.Select(c => c.Ratedate).ToList());
                        allRatedates = allRatedates.Distinct().OrderBy(c => c).ToList();

                        foreach (var ratedate in allRatedates)
                        {
                            var oldItem = oldResDetailAllInfo.ResDetailPlans.Where(c => c.Ratedate == ratedate).FirstOrDefault();
                            var newItem = newResDetailAllInfo.ResDetailPlans.Where(c => c.Ratedate == ratedate).FirstOrDefault();
                            if (oldItem == null && newItem != null)
                            {
                                prices += string.Format(proceFormat, newItem.Ratedate.ToDateString(), "无", newItem.Price);
                            }
                            else if (oldItem != null && newItem == null)
                            {
                                prices += string.Format(proceFormat, oldItem.Ratedate.ToDateString(), oldItem.Price, "无");
                            }
                            else if (oldItem != null && newItem != null)
                            {
                                if (oldItem.Price != newItem.Price)
                                {
                                    prices += string.Format(proceFormat, oldItem.Ratedate.ToDateString(), oldItem.Price, newItem.Price);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(prices))
                        {
                            if (prices.EndsWith("，"))
                            {
                                prices = prices.Remove(prices.Length - 1);
                            }
                            text.AppendFormat("{0}：{1}，", "价格列表", "{" + prices + "}");
                        }
                    }
                    #endregion
                    #region 客人
                    if (oldResDetailAllInfo.RegInfos != null)
                    {
                        string customerList = "";

                        string addCustomers = "";
                        #region
                        foreach (var oldItem in oldResDetailAllInfo.RegInfos)
                        {
                            var newItem = newResDetailAllInfo.RegInfos.Where(c => c.Id == oldItem.Id).FirstOrDefault();
                            if (newItem != null)
                            {
                                string customers = "";
                                if (oldItem.GuestName != newItem.GuestName)
                                {
                                    customers += string.Format(format, "客人姓名", oldItem.GuestName, newItem.GuestName);
                                }
                                if (oldItem.Gender != newItem.Gender)
                                {
                                    customers += string.Format(format, "性别", oldItem.Gender == "F" ? "女" : "男", newItem.Gender == "F" ? "女" : "男");
                                }
                                if (oldItem.CerType != newItem.CerType)
                                {
                                    customers += string.Format(format, "证件类型", GetNameById("cerType", hid, oldItem.CerType), GetNameById("cerType", hid, newItem.CerType));
                                }
                                if (oldItem.Cerid != newItem.Cerid)
                                {
                                    customers += string.Format(format, "证件号", oldItem.Cerid, newItem.Cerid);
                                }
                                if (oldItem.Mobile != newItem.Mobile)
                                {
                                    customers += string.Format(format, "手机号", oldItem.Mobile, newItem.Mobile);
                                }
                                if (oldItem.Birthday != newItem.Birthday)
                                {
                                    customers += string.Format(format, "生日", oldItem.Birthday.ToDateString(), newItem.Birthday.ToDateString());
                                }
                                if (oldItem.Nation != newItem.Nation)
                                {
                                    customers += string.Format(format, "国籍", oldItem.Nation, newItem.Nation);
                                }
                                if (oldItem.City != newItem.City)
                                {
                                    customers += string.Format(format, "籍贯", oldItem.City, newItem.City);
                                }
                                if (oldItem.Address != newItem.Address)
                                {
                                    customers += string.Format(format, "地址", oldItem.Address, newItem.Address);
                                }
                                if (oldItem.Email != newItem.Email)
                                {
                                    customers += string.Format(format, "邮箱", oldItem.Email, newItem.Email);
                                }
                                if (oldItem.CarNo != newItem.CarNo)
                                {
                                    customers += string.Format(format, "车牌", oldItem.CarNo, newItem.CarNo);
                                }
                                //if (oldItem.Qq != newItem.Qq)
                                //{
                                //    customers += string.Format(format, "QQ", oldItem.Qq, newItem.Qq);
                                //}
                                if (oldItem.Interest != newItem.Interest)
                                {
                                    customers += string.Format(format, "喜好", oldItem.Interest, newItem.Interest);
                                }
                                if (oldItem.IsMast != newItem.IsMast)
                                {
                                    customers += string.Format(format, "是否主客", oldItem.IsMast == "1" ? "主客" : "随行人", newItem.IsMast == "1" ? "主客" : "随行人");
                                }
                                if (!string.IsNullOrWhiteSpace(customers))
                                {
                                    if (customers.EndsWith("，"))
                                    {
                                        customers = customers.Remove(customers.Length - 1);
                                    }
                                    addCustomers += string.Format("[客人姓名：{0}，{1}]，", newItem.GuestName, customers);
                                }
                            }
                        }
                        #endregion
                        if (!string.IsNullOrWhiteSpace(addCustomers))
                        {
                            if (addCustomers.EndsWith("，"))
                            {
                                addCustomers = addCustomers.Remove(addCustomers.Length - 1);
                            }
                            customerList += string.Format("[修改随行人，{0}]，", addCustomers);
                        }

                        List<Guid> oldCustomerIds = oldResDetailAllInfo.RegInfos.Select(c => c.Id).ToList();
                        var newAddRegInfos = newResDetailAllInfo.RegInfos.Where(c => !oldCustomerIds.Contains(c.Id)).ToList();
                        if(newAddRegInfos != null && newAddRegInfos.Count > 0)
                        {
                            customerList += string.Format("[增加随行人，{0}]，", GetRegInfosToString(hid, newAddRegInfos));
                        }

                        List<Guid> newCustomerIds = newResDetailAllInfo.RegInfos.Select(c => c.Id).ToList();
                        var oldDelRegInfos = oldResDetailAllInfo.RegInfos.Where(c => !newCustomerIds.Contains(c.Id)).ToList();
                        if(oldDelRegInfos != null && oldDelRegInfos.Count > 0)
                        {
                            customerList += string.Format("[移除随行人，{0}]，", GetRegInfosToString(hid, oldDelRegInfos));
                        }

                        if (!string.IsNullOrWhiteSpace(customerList))
                        {
                            if (customerList.EndsWith("，"))
                            {
                                customerList = customerList.Remove(customerList.Length - 1);
                            }
                            text.AppendFormat("{0}：{1}，", "客人列表", "{" + customerList + "}");
                        }
                    }
                    #endregion

                    if (!string.IsNullOrWhiteSpace(text.ToString()))
                    {
                        keys = newResDetailAllInfo.ResDetailEntity.Regid;
                        if (newResDetailAllInfo.ResDetailEntity.ResStatus == ResDetailStatus.R.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.ResStatus == ResDetailStatus.N.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.ResStatus == ResDetailStatus.X.ToString())
                        {
                            type = BSPMS.Common.Services.Enums.OpLogType.预订;
                        }
                        if (newResDetailAllInfo.ResDetailEntity.RecStatus == ResDetailStatus.I.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.RecStatus == ResDetailStatus.O.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.RecStatus == ResDetailStatus.C.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.RecStatus == ResDetailStatus.Z.ToString() ||
                            newResDetailAllInfo.ResDetailEntity.RecStatus == ResDetailStatus.X.ToString())
                        {
                            type = BSPMS.Common.Services.Enums.OpLogType.入住;
                        }

                        string idInfo = "";
                        string idFormat = "{0}：{1}，";
                        idInfo += string.Format(idFormat, "主单号", newResDetailAllInfo.ResEntity.Resno);
                        idInfo += string.Format(idFormat, "账号", GetShortRegid(hid, newResDetailAllInfo.ResDetailEntity.Regid));
                        idInfo += string.Format(idFormat, "状态", GetStatusName(newResDetailAllInfo.ResDetailEntity.Status));
                        idInfo += string.Format(idFormat, "房号", newResDetailAllInfo.ResDetailEntity.RoomNo);

                        text.Insert(0,string.Format("{0}维护　{1}", type.ToString(), idInfo));
                    }

                    #endregion
                }
                string logText = text.ToString();
                if (logText.EndsWith("，"))
                {
                    logText = logText.Remove(logText.Length - 1);
                }
                if (!string.IsNullOrWhiteSpace(logText))
                {
                    if (!string.IsNullOrWhiteSpace(authUserName))
                    {
                        logText = string.Format("{0}，{2}，授权人：{1}", logText, authUserName, reason);
                    }
                    AddOperationLog(currentInfo, type, logText, keys, true);
                }
            }
            catch{ }
        }

        /// <summary>
        /// 子单价格列表 转换为 字符串
        /// </summary>
        /// <param name="ResDetailPlans">价格列表</param>
        /// <returns></returns>
        private string GetResDetailPlansToString(List<Entities.ResDetailPlan> ResDetailPlans)
        {
            string format = "[{0}：{1}]，";
            string prices = "";
            foreach (var item in ResDetailPlans)
            {
                prices += string.Format(format, item.Ratedate.ToDateString(), item.Price);
            }
            if (prices.EndsWith("，"))
            {
                prices = prices.Remove(prices.Length - 1);
            }
            return prices;
        }
        /// <summary>
        /// 子单客人列表 转换为 字符串
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="RegInfos">客人列表</param>
        /// <returns></returns>
        private string GetRegInfosToString(string hid, List<Entities.RegInfo> RegInfos)
        {
            string format = "{0}：{1}，";
            string customers = "";
            foreach (var item in RegInfos)
            {
                customers += "[";
                customers += string.Format(format, "客人姓名", item.GuestName);
                customers += string.Format(format, "性别", item.Gender == "F" ? "女" : "男");
                customers += string.Format(format, "证件类型", GetNameById("cerType", hid, item.CerType));
                customers += string.Format(format, "证件号", item.Cerid);

                customers += string.Format(format, "手机号", item.Mobile);
                customers += string.Format(format, "生日", item.Birthday.ToDateString());

                customers += string.Format(format, "国籍", item.Nation);
                customers += string.Format(format, "籍贯", item.City);
                customers += string.Format(format, "地址", item.Address);

                customers += string.Format(format, "邮箱", item.Email);
                customers += string.Format(format, "车牌", item.CarNo);
                //customers += string.Format(format, "QQ", item.Qq);
                customers += string.Format(format, "喜好", item.Interest);

                customers += string.Format(format, "是否主客", item.IsMast == "1" ? "主客" : "随行人");
                customers = customers.Remove(customers.Length - 1);
                customers += "]，";
            }
            if (customers.EndsWith("，"))
            {
                customers = customers.Remove(customers.Length - 1);
            }
            return customers;
        }

        /// <summary>
        /// 获取短账号（短账号 = 账号去除前缀酒店ID）
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号ID</param>
        /// <returns></returns>
        private string GetShortRegid(string hid, string regid)
        {
            if(!string.IsNullOrWhiteSpace(hid) && !string.IsNullOrWhiteSpace(regid))
            {
                if (regid.StartsWith(hid))
                {
                    return regid.Remove(0, hid.Length);
                }
            }
            return regid;
        }
        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="status">状态值</param>
        /// <returns></returns>
        private string GetStatusName(string status)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                return EnumExtension.GetDescription(typeof(ResDetailStatus), status);
            }
            return status;
        }
        /// <summary>
        /// 根据ID获取名称
        /// </summary>
        /// <param name="type">类型（roomtypeid：房间类型ID，companyid：合约单位ID，profileid：会员ID，rateid：价格代码ID，marketid：市场分类ID，sourceid：客人来源ID，cerid：证件类型ID）</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">ID值</param>
        /// <returns></returns>
        private string GetNameById(string type, string hid, object idValue)
        {
            if(type == null) { return ""; }
            if (idValue == null || string.IsNullOrWhiteSpace(idValue.ToString())) { return ""; }

            type = type.ToLower();
            string id = idValue.ToString();
            string name = "";
            switch (type)
            {
                case "roomtypeid"://房间类型
                    {
                        name = _pmsContext.RoomTypes.Where(c => c.Hid == hid && c.Id == id).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                    }
                    break;
                case "companyid"://合约单位
                    {
                        Guid newId = Guid.Empty;
                        if(Guid.TryParse(id, out newId))
                        {
                            if(newId != Guid.Empty)
                            {
                                name = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id == newId).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                            }
                        }
                    }
                    break;
                case "profileid"://会员
                    {
                        Guid newId = Guid.Empty;
                        if (Guid.TryParse(id, out newId))
                        {
                            if (newId != Guid.Empty)
                            {
                                name = _pmsContext.MbrCards.Where(c => c.Id == newId).Select(c => c.GuestName).AsNoTracking().FirstOrDefault();
                            }
                        }
                    }
                    break;
                case "rateid"://价格代码
                    {
                        name = _pmsContext.Rates.Where(c => c.Hid == hid && c.Id == id).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                    }
                    break;
                case "marketid"://04市场分类
                case "sourceid"://05客人来源
                    {
                        string typeCode = "";
                        switch (type)
                        {
                            case "marketid":
                                typeCode = "04";
                                break;
                            case "sourceid":
                                typeCode = "05";
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(typeCode))
                        {
                            name = _pmsContext.CodeLists.Where(c => c.Hid == hid && c.Id == id && c.TypeCode == typeCode).Select(c => c.Name).AsNoTracking().FirstOrDefault();
                        }
                    }
                    break;
                case "certype"://证件类型
                    {
                        name = _pmsContext.Database.SqlQuery<string>("select top 1 name from v_codeListPub where typeCode = '09' and code = @code;", new SqlParameter("@code", string.IsNullOrWhiteSpace(id) ? "" : id)).FirstOrDefault();
                    }
                    break;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                name = id;
            }
            return name;
        }
        #endregion

        #region 调价授权
        /// <summary>
        /// 验证调价授权
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="adjustPriceOrderList">调价订单信息</param>
        /// <param name="operationSource">操作来源</param>
        /// <returns></returns>
        private JsonResultData CheckAuthAdjustPrice(ICurrentInfo currentInfo, List<ResAdjustPriceInfo.AdjustPriceOrderModel> adjustPriceOrderList, ResAdjustPriceInfo.AdjustPriceOperationSource operationSource)
        {
            #region 是否有调价权限
            if (currentInfo == null || string.IsNullOrWhiteSpace(currentInfo.HotelId) || string.IsNullOrWhiteSpace(currentInfo.UserId))
            {
                throw new Exception("验证调价参数错误，当前登录信息不能为空");
            }
            string hid = currentInfo.HotelId;
            string userid = currentInfo.UserId;

            var authCheckService = DependencyResolver.Current.GetService<Services.AuthManages.IAuthCheck>();
            string authCode = "";
            long authButtonValue = -1;
            bool isAuthTrue = Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.GetAuthority(1, out authCode, out authButtonValue);
            bool isAuthAdjustPrice = isAuthTrue ? authCheckService.HasAuth(userid, "2002010", (long)Gemstar.BSPMS.Hotel.Services.AuthManages.AuthFlag.AdjustPrice, hid) : false;
            if (isAuthAdjustPrice)
            {
                return JsonResultData.Successed("有调价权限");
            }
            #endregion

            #region 验证参数
            if (adjustPriceOrderList == null || adjustPriceOrderList.Count <= 0)
            {
                throw new Exception("验证调价参数错误");
            }
            if (operationSource != ResAdjustPriceInfo.AdjustPriceOperationSource.OrderBatchAdded && adjustPriceOrderList.Count != 1)
            {
                throw new Exception("验证调价参数错误，只有批量新订单才能传多个值");
            }
            foreach (var item in adjustPriceOrderList)
            {
                if (string.IsNullOrWhiteSpace(item.RegId))
                {
                    throw new Exception("验证调价参数错误，账号不能为空");
                }
                if (string.IsNullOrWhiteSpace(item.RateCodeId))
                {
                    throw new Exception("验证调价参数错误，价格代码不能为空");
                }
                if (string.IsNullOrWhiteSpace(item.RoomTypeId))
                {
                    throw new Exception("验证调价参数错误，房间类型不能为空");
                }

                item.BeginAdjustPriceDate = null;
                item.EndAdjustPriceDate = null;

                switch (operationSource)
                {
                    case ResAdjustPriceInfo.AdjustPriceOperationSource.OrderAdded:
                    case ResAdjustPriceInfo.AdjustPriceOperationSource.OrderBatchAdded:
                        {
                            item.OriginRateCodeId = null;
                            item.OriginRoomTypeId = null;
                        }
                        break;
                    case ResAdjustPriceInfo.AdjustPriceOperationSource.OrderModified:
                    case ResAdjustPriceInfo.AdjustPriceOperationSource.OrderChangeRoom:
                        {
                            if (string.IsNullOrWhiteSpace(item.OriginRateCodeId))
                            {
                                throw new Exception("验证调价参数错误，源价格代码不能为空");
                            }
                            if (string.IsNullOrWhiteSpace(item.OriginRoomTypeId))
                            {
                                throw new Exception("验证调价参数错误，源房间类型不能为空");
                            }
                        }
                        break;
                    default:
                        throw new Exception("验证调价参数错误，操作来源错误");
                }
            }
            #endregion

            List<ResDetailPlan> resDetailPlanList = new List<ResDetailPlan>();//新价格
            #region
            //通过跟踪获取要添加的订单价格，因为订单价格总是删除后再添加。
            var resDetailPlanTrackerList = _pmsContext.ChangeTracker.Entries<ResDetailPlan>().Where(c => c.State == EntityState.Added);
            if (resDetailPlanTrackerList != null && resDetailPlanTrackerList.Count() > 0)
            {
                foreach (var item in resDetailPlanTrackerList)
                {
                    ResDetailPlan resDetailPlanEntity = new ResDetailPlan();
                    foreach (var prop in item.CurrentValues.PropertyNames)
                    {
                        switch (prop)
                        {
                            case "Regid":
                                resDetailPlanEntity.Regid = item.CurrentValues.GetValue<string>(prop);
                                break;
                            case "Ratedate":
                                resDetailPlanEntity.Ratedate = item.CurrentValues.GetValue<DateTime>(prop);
                                break;
                            case "Price":
                                resDetailPlanEntity.Price = item.CurrentValues.GetValue<decimal?>(prop);
                                break;
                            case "OriginPrice":
                                resDetailPlanEntity.OriginPrice = item.CurrentValues.GetValue<decimal?>(prop);
                                break;
                        }
                    }
                    resDetailPlanList.Add(resDetailPlanEntity);
                }
            }
            List<string> regids = resDetailPlanList.Select(c => c.Regid).Distinct().ToList();
            if (regids != null && regids.Count > 0)
            {
                foreach (var regid in regids)
                {
                    var entity = adjustPriceOrderList.Where(c => c.RegId == regid).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.BeginAdjustPriceDate = resDetailPlanList.Where(c => c.Regid == regid).Min(c => c.Ratedate);
                        entity.EndAdjustPriceDate = resDetailPlanList.Where(c => c.Regid == regid).Max(c => c.Ratedate);
                    }
                }
            }
            #endregion
            List<ResDetailPlan> originResDetailPlanList = new List<ResDetailPlan>(); //源价格
            List<RateDetail> rateDetailList = new List<RateDetail>();//新价格体系
            foreach (var item in adjustPriceOrderList)
            {
                if (item.BeginAdjustPriceDate != null && item.EndAdjustPriceDate != null)
                {
                    rateDetailList.AddRange(_pmsContext.RateDetails.Where(c => c.Hid == hid && c.Rateid == item.RateCodeId && c.RoomTypeid == item.RoomTypeId && c.RateDate >= item.BeginAdjustPriceDate && c.RateDate <= item.EndAdjustPriceDate).AsNoTracking().ToList());
                    if (operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderModified || operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderChangeRoom)
                    {
                        originResDetailPlanList.AddRange(_pmsContext.ResDetailPlans.Where(c => c.Regid == item.RegId && c.Ratedate >= item.BeginAdjustPriceDate && c.Ratedate <= item.EndAdjustPriceDate).AsNoTracking().ToList());
                    }
                }
            }

            //3.比较是否调价
            List<ResAdjustPriceInfo.ResDetailPlan> resDetailPlanListResult = new List<ResAdjustPriceInfo.ResDetailPlan>();
            if (operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderAdded || operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderBatchAdded)//新订单
            {
                #region
                foreach (var item in resDetailPlanList)
                {
                    var adjustPriceOrderEntity = adjustPriceOrderList.Where(c => c.RegId == item.Regid).FirstOrDefault();
                    if (adjustPriceOrderEntity != null)
                    {
                        //计划价格
                        decimal? planPrice = rateDetailList.Where(c => c.Hid == hid && c.Rateid == adjustPriceOrderEntity.RateCodeId && c.RoomTypeid == adjustPriceOrderEntity.RoomTypeId && c.RateDate == item.Ratedate).Select(s => s.Rate).FirstOrDefault();
                        if (planPrice != null && planPrice != item.Price)
                        {
                            resDetailPlanListResult.Add(new ResAdjustPriceInfo.ResDetailPlan { Regid = item.Regid, Ratedate = item.Ratedate, OriginPrice = null, PlanPrice = planPrice, Price = item.Price });//调价了
                        }
                    }
                }
                #endregion
            }
            else if (operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderModified || operationSource == ResAdjustPriceInfo.AdjustPriceOperationSource.OrderChangeRoom)//订单修改，可能会改变价格代码、房型、房价。换房，不会改变价格代码。
            {
                #region
                foreach (var item in resDetailPlanList)
                {
                    var adjustPriceOrderEntity = adjustPriceOrderList.Where(c => c.RegId == item.Regid).FirstOrDefault();
                    if (adjustPriceOrderEntity != null)
                    {
                        //源价格
                        decimal? oldPrice = originResDetailPlanList.Where(c => c.Regid == item.Regid && c.Ratedate == item.Ratedate).Select(s => s.Price).FirstOrDefault();
                        //计划价格
                        decimal? planPrice = rateDetailList.Where(c => c.Hid == hid && c.Rateid == adjustPriceOrderEntity.RateCodeId && c.RoomTypeid == adjustPriceOrderEntity.RoomTypeId && c.RateDate == item.Ratedate).Select(s => s.Rate).FirstOrDefault();


                        if (adjustPriceOrderEntity.OriginRateCodeId != adjustPriceOrderEntity.RateCodeId)//价格代码调整了，就是调价了，不论价格明细是否有修改。
                        {
                            if (oldPrice != null && oldPrice != item.Price)
                            {
                                resDetailPlanListResult.Add(new ResAdjustPriceInfo.ResDetailPlan { Regid = item.Regid, Ratedate = item.Ratedate, OriginPrice = oldPrice, PlanPrice = planPrice, Price = item.Price });//调价了
                            }
                        }
                        else if (adjustPriceOrderEntity.OriginRoomTypeId != adjustPriceOrderEntity.RoomTypeId)//换房型，则比较价格体系价格和新房型价格。
                        {
                            if (planPrice != null && planPrice != item.Price)
                            {
                                resDetailPlanListResult.Add(new ResAdjustPriceInfo.ResDetailPlan { Regid = item.Regid, Ratedate = item.Ratedate, OriginPrice = oldPrice, PlanPrice = planPrice, Price = item.Price });//调价了
                            }
                        }
                        else//只换房间 或 只修改价格，则比较源价格和新价格
                        {
                            if (oldPrice != null && oldPrice != item.Price)
                            {
                                resDetailPlanListResult.Add(new ResAdjustPriceInfo.ResDetailPlan { Regid = item.Regid, Ratedate = item.Ratedate, OriginPrice = oldPrice, PlanPrice = planPrice, Price = item.Price });//调价了
                            }
                        }
                    }
                }
                #endregion
            }

            //返回结果
            List<ResAdjustPriceInfo.AdjustPriceResultModel> resultList = new List<ResAdjustPriceInfo.AdjustPriceResultModel>();
            #region
            var rateCodeIds = adjustPriceOrderList.Select(c => c.RateCodeId).ToList();
            var roomTypeIds = adjustPriceOrderList.Select(c => c.RoomTypeId).ToList();
            rateCodeIds.AddRange(adjustPriceOrderList.Where(c => !string.IsNullOrWhiteSpace(c.OriginRateCodeId)).Select(c => c.OriginRateCodeId).ToList());
            roomTypeIds.AddRange(adjustPriceOrderList.Where(c => !string.IsNullOrWhiteSpace(c.OriginRoomTypeId)).Select(c => c.OriginRoomTypeId).ToList());
            rateCodeIds = rateCodeIds.Distinct().ToList();
            roomTypeIds = roomTypeIds.Distinct().ToList();

            var rateCodeIdAndNames = _pmsContext.Rates.Where(c => c.Hid == hid && rateCodeIds.Contains(c.Id)).Select(s => new { s.Id, s.Name }).AsNoTracking().ToList();
            var roomTypeIdAndNames = _pmsContext.RoomTypes.Where(c => c.Hid == hid && roomTypeIds.Contains(c.Id)).Select(s => new { s.Id, s.Name }).AsNoTracking().ToList();

            foreach (var item in adjustPriceOrderList)
            {
                var checkAuthAdjustPriceList = resDetailPlanListResult.Where(c => c.Regid == item.RegId).ToList();
                bool isEditRateCode = (!string.IsNullOrWhiteSpace(item.OriginRateCodeId) && item.OriginRateCodeId != item.RateCodeId);
                if ((checkAuthAdjustPriceList != null && checkAuthAdjustPriceList.Count > 0 && !resultList.Where(c => c.RateCodeId == item.RateCodeId && c.RoomTypeId == item.RoomTypeId).Any())//调价了
                    ||
                    isEditRateCode//价格代码改变了
                    )
                {
                    resultList.Add(new ResAdjustPriceInfo.AdjustPriceResultModel
                    {
                        RegId = item.RegId,
                        GuestName = item.GuestName,
                        RoomNo = item.RoomNo,
                        OriginRoomNo = item.OriginRoomNo,
                        RateCodeId = item.RateCodeId,
                        OriginRateCodeId = item.OriginRateCodeId,

                        RoomTypeId = item.RoomTypeId,
                        OriginRoomTypeId = item.OriginRoomTypeId,

                        RateCodeName = rateCodeIdAndNames.Where(c => c.Id == item.RateCodeId).Select(s => s.Name).FirstOrDefault(),
                        OriginRateCodeName = rateCodeIdAndNames.Where(c => c.Id == item.OriginRateCodeId).Select(s => s.Name).FirstOrDefault(),

                        RoomTypeName = roomTypeIdAndNames.Where(c => c.Id == item.RoomTypeId).Select(s => s.Name).FirstOrDefault(),
                        OriginRoomTypeName = roomTypeIdAndNames.Where(c => c.Id == item.OriginRoomTypeId).Select(s => s.Name).FirstOrDefault(),

                        AdjustPriceList = checkAuthAdjustPriceList,
                        Source = operationSource,
                    });
                }
            }
            #endregion
            //返回
            if (resultList != null && resultList.Count > 0)
            {
                if (!DependencyResolver.Current.GetService<Gemstar.BSPMS.Hotel.Services.IPmsParaService>().IsAllowAuthAdjustPrice(hid))
                {
                    return JsonResultData.Failure("没有调价权限，不允许修改价格！");
                }
                return JsonResultData.Failure(resultList, 4);
            }
            return JsonResultData.Successed("没有调价内容");
        }
        /// <summary>
        /// 获取授权结果，用于确认结果
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="authIdAndTicks">主键ID+授权时间</param>
        /// <param name="keys">外部关联ID</param>
        /// <returns></returns>
        private bool CheckAuthAdjustPriceResult(ICurrentInfo currentInfo, string authIdAndTicks, string keys, out string authUserName,out string reason)
        {
            authUserName = null; reason = "";
             var authCheckService = DependencyResolver.Current.GetService<Services.AuthManages.IAuthorizationService>();
            var result = authCheckService.CheckAndUpdateAuthorization(currentInfo, authIdAndTicks, keys,out reason);
            if (result != null)
            {
                authUserName = result.Data;
                return result.Success;
            }
            return false;
        }
        #endregion

        #region 积分换房
        /// <summary>
        /// 获取价格代码 是否启用了积分换房功能
        /// </summary>
        /// <param name="rateCodeId">价格代码ID</param>
        /// <returns></returns>
        private bool GetRateIsUseScore(string hid, string rateCodeId)
        {
            return _pmsContext.Rates.AsNoTracking().Where(c => c.Hid == hid && c.Id == rateCodeId && c.IsUseScore == true).Any();
        }
        private List<string> GetRateIsUseScoreByRegIds(string hid, List<KeyValuePairModel<string,string>> rateCodeIds)
        {
            if(rateCodeIds == null || rateCodeIds.Count <= 0) { return new List<string>(); }

            var rateCodeIds_IsUseScore = _pmsContext.Rates.AsNoTracking().Where(c => c.Hid == hid && c.IsUseScore == true).Select(c => c.Id).ToList();
            if(rateCodeIds_IsUseScore == null || rateCodeIds_IsUseScore.Count <= 0) { return new List<string>(); }

            return rateCodeIds.Where(c => rateCodeIds_IsUseScore.Contains(c.Value)).Select(c => c.Key).ToList();
        }
        private class UseScoreToCheckinPlanInfo
        {
            public DateTime Ratedate { get; set; }

            public decimal? Price { get; set; }
        }
        private class UseScoreToCheckinInfo
        {
            /// <summary>
            /// 订单ID
            /// </summary>
            public string RegId { get; set; }
            /// <summary>
            /// 会员ID
            /// </summary>
            public Guid ProfileId { get; set; }

            /// <summary>
            /// 房号
            /// </summary>
            public string RoomNo { get; set; }
            /// <summary>
            /// 价格代码名称
            /// </summary>
            public string RateCodeName { get; set; }
            /// <summary>
            /// 价格列表
            /// </summary>
            public List<UseScoreToCheckinPlanInfo> RatePlans { get; set; }
            /// <summary>
            /// 价格积分 入住需扣除多少积分
            /// </summary>
            public int RateScore { get; set; }
            /// <summary>
            /// 会员姓名
            /// </summary>
            public string ProfileName { get; set; }
            /// <summary>
            /// 会员卡号
            /// </summary>
            public string ProfileNo { get; set; }
            /// <summary>
            /// 会员手机号
            /// </summary>
            public string ProfileMobile { get; set; }
            /// <summary>
            /// 会员可用积分
            /// </summary>
            public int ProfileScore { get; set; }
        }
        /// <summary>
        /// 获取 使用积分入住 信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">订单ID列表</param>
        /// <returns></returns>
        private List<UseScoreToCheckinInfo> GetUseScoreToCheckinInfo(string hid, List<string> regids, bool isLocal = true)
        {
            string message = "积分换房失败！原因：";
            //订单 本地
            var regList = new List<ResDetail>();
            if (isLocal)
            {
                regList = _pmsContext.ResDetails.Local.Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.RecStatus == "I").ToList();
            }
            else
            {
                regList = _pmsContext.ResDetails.AsNoTracking().Where(c => c.Hid == hid && regids.Contains(c.Regid) && c.Status == "I").ToList();
            }
            if(regList == null || regList.Count <= 0)
            {
                throw new Exception(message + "订单不能为空！");
            }
            var regids_new = regList.Select(c => c.Regid).Distinct().ToList();
            var ratePlanList = new List<ResDetailPlan>();
            if (isLocal)
            {
                ratePlanList = _pmsContext.ResDetailPlans.Local.Where(c => regids_new.Contains(c.Regid)).ToList();
            }
            else
            {
                ratePlanList = _pmsContext.ResDetailPlans.AsNoTracking().Where(c => regids_new.Contains(c.Regid)).ToList();
            }

            var rateCodeIds = regList.Select(c => c.RateCode).Distinct().ToList();
            var rateList = _pmsContext.Rates.AsNoTracking().Where(c => c.Hid == hid && rateCodeIds.Contains(c.Id)).ToList();

            var profileIds = regList.Select(c => c.Profileid).Distinct().ToList();
            var profileList = _pmsContext.MbrCards.AsNoTracking().Where(c => profileIds.Contains(c.Id)).ToList();
            var profileBalanceList = _pmsContext.MbrCardBalances.AsNoTracking().Where(c => profileIds.Contains(c.profileId)).ToList();

            List<KeyValuePairModel<Guid, int>> profileBalanceScoreList = new List<KeyValuePairModel<Guid, int>>();
            List<UseScoreToCheckinInfo> resultList = new List<UseScoreToCheckinInfo>();
            foreach (var regEntity in regList)
            {
                //价格代码
                if (string.IsNullOrWhiteSpace(regEntity.RateCode))
                {
                    throw new Exception(message + "价格代码不能为空！");
                }
                var rateEntity = rateList.Where(c => c.Hid == hid && c.Id == regEntity.RateCode).FirstOrDefault();
                if (rateEntity == null)
                {
                    throw new Exception(message + "价格代码不能为空！");
                }
                if (rateEntity.IsUseScore == false)
                {
                    throw new Exception(message + "没有启用积分换房！");
                }

                //抵店时间-离店时间 住几晚 要扣除的积分
                if (regEntity.ArrBsnsDate == null || regEntity.ArrBsnsDate.HasValue == false)
                {
                    throw new Exception(message + "入住营业日不能为空！");
                }
                if (regEntity.DepDate == null || regEntity.DepDate.HasValue == false)
                {
                    throw new Exception(message + "离店时间不能为空！");
                }
                DateTime beginDate = regEntity.ArrBsnsDate.Value.Date;
                DateTime endDate = regEntity.DepDate.Value.Date;
                if (beginDate == endDate)
                {
                    endDate = endDate.AddDays(1).Date;
                }
                var ratePlans = ratePlanList.Where(c => c.Regid == regEntity.Regid && c.Ratedate >= beginDate && c.Ratedate < endDate).ToList();
                if (ratePlans == null || ratePlans.Count <= 0)
                {
                    throw new Exception(message + "积分不能为空！");
                }
                if(ratePlans.Where(c => c.Price != null && c.Price.HasValue).Count() != (endDate - beginDate).Days)
                {
                    throw new Exception(message + "请在[调价]窗口填写积分！");
                }
                int ratePlansScore = 0;
                var ratePlansScore_temp = ratePlans.Where(c => c.Price != null && c.Price.HasValue).Sum(c => c.Price);
                if(ratePlansScore_temp != null && ratePlansScore_temp.HasValue)
                {
                    ratePlansScore = Convert.ToInt32(ratePlansScore_temp.Value);
                }

                //会员
                if (regEntity.Profileid == null || regEntity.Profileid == Guid.Empty)
                {
                    throw new Exception(message + "会员不能为空！");
                }
                var profileEntity = profileList.Where(c => c.Id == regEntity.Profileid).FirstOrDefault();
                if (profileEntity == null)
                {
                    throw new Exception(message + "会员不能为空！");
                }

                var profileBalanceScoreEntity = profileBalanceScoreList.FirstOrDefault(c => c.Key == regEntity.Profileid.Value);
                if(profileBalanceScoreEntity == null)
                {
                    int profileBalanceScore = 0;
                    var profileBalanceEntity = profileBalanceList.Where(c => c.profileId == regEntity.Profileid).SingleOrDefault();
                    if (profileBalanceEntity != null && profileBalanceEntity.Score != null && profileBalanceEntity.Score.HasValue)
                    {
                        profileBalanceScore = profileBalanceEntity.Score.Value;
                    }
                    profileBalanceScoreList.Add(new KeyValuePairModel<Guid, int> { Key = regEntity.Profileid.Value, Value = profileBalanceScore });
                    profileBalanceScoreEntity = profileBalanceScoreList.FirstOrDefault(c => c.Key == regEntity.Profileid.Value);
                }

                int profileScore = profileBalanceScoreEntity.Value;
                profileBalanceScoreEntity.Value = Convert.ToInt32(profileBalanceScoreEntity.Value - ratePlansScore);

                if (string.IsNullOrWhiteSpace(regEntity.RoomNo))
                {
                    throw new Exception(message + "房号不能为空！");
                }

                //返回
                var result = new UseScoreToCheckinInfo
                {
                    RegId = regEntity.Regid,
                    ProfileId = regEntity.Profileid.Value,
                    RoomNo = regEntity.RoomNo,
                    RateCodeName = rateEntity.Name,
                    RatePlans = ratePlans.Select(c => new UseScoreToCheckinPlanInfo { Ratedate = c.Ratedate, Price = c.Price }).OrderBy(c => c.Ratedate).ToList(),
                    RateScore = ratePlansScore,
                    ProfileName = profileEntity.GuestName,
                    ProfileNo = profileEntity.MbrCardNo,
                    ProfileMobile = profileEntity.Mobile,
                    ProfileScore = profileScore,
                };
                resultList.Add(result);
            }

            return resultList;
        }
        /// <summary>
        /// 检查 使用积分入住
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">订单ID列表</param>
        /// <returns></returns>
        private JsonResultData CheckUseScoreToCheckin(string hid, List<string> regids)
        {
            try
            {
                return JsonResultData.Failure(GetUseScoreToCheckinInfo(hid, regids), 5);
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 执行积分入住
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regids">订单ID列表</param>
        /// <returns></returns>
        private JsonResultData ExecUseScoreToCheckin(ICurrentInfo currentInfo, List<string> regids)
        {
            try
            {
                string message = "积分换房失败！原因：";

                if (string.IsNullOrWhiteSpace(currentInfo.HotelId)) { return JsonResultData.Failure(message + "酒店ID不能为空！"); }

                string itemId = _pmsContext.Database.SqlQuery<string>("select top 1 id from item where dcflag = 'D' and action = '01' and status < 51 and hid = @hid order by (case when code = '01' then 0 else 1 end),code", new SqlParameter("@hid", SqlDbType.VarChar) { Value = currentInfo.HotelId }).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(itemId))
                {
                    return JsonResultData.Failure(message + "夜审消费项目ID不能为空！");
                }

                var list = GetUseScoreToCheckinInfo(currentInfo.HotelId, regids, false);
                if (list == null || list.Count <= 0) { return JsonResultData.Failure(message + "积分入住信息不能为空！"); }
                foreach (var item in list)
                {
                    if (item.ProfileScore - item.RateScore < 0)
                    {
                        return JsonResultData.Failure(message + "积分不足！");
                    }
                }

                foreach (var item in list)
                {
                    string invNo = DateTime.Now.Ticks.ToString();
                    string refNo = "";
                    string prifileRemark = string.Format("[积分换房，账号：{0}]", item.RegId.Substring(currentInfo.HotelId.Length));

                    //积分调整 扣积分
                    var _mbrCardService = DependencyResolver.Current.GetService<Services.MbrCardCenter.IMbrCardService>();
                    var scoreActionResult = _mbrCardService.ScoreAction(item.ProfileId, "11", (-Math.Abs(item.RateScore)), invNo, prifileRemark);
                    if (scoreActionResult != null && scoreActionResult.Success)
                    {
                        refNo = scoreActionResult.Data != null ? scoreActionResult.Data.ToString() : "";
                    }
                    else
                    {
                        return scoreActionResult;
                    }

                    int count = item.RatePlans.Count;
                    int index = 1;
                    foreach (var rateItem in item.RatePlans)
                    {
                        string orderRemark = string.Format("[积分换房，会员：{0}，共{1}晚：{2}分，{3}：{4}分]", item.ProfileNo, count.ToString(), item.RateScore.ToString(), rateItem.Ratedate.Date.ToDateString(), Convert.ToInt32((rateItem.Price != null ? rateItem.Price.Value : 0)).ToString());
                        //增加账务
                        var folioInput = _pmsContext.Database.SqlQuery<upProfileCaInputResult>(@"exec up_resFolio_input 
                         @hid=@hid
                        ,@regid=@regid
                        ,@itemid=@itemid
                        ,@quantity=@quantity
                        ,@amount=@amount
                        ,@invNo=@invNo
                        ,@inputUser=@inputUser
                        ,@remark=@remark
                        ,@transShift=@transShift
                        ,@isauto=@isauto
                        ,@refno=@refno
                        ,@inputDate=@inputDate
                        ,@transBsnsDate=@transBsnsDate"
                        , new SqlParameter("@hid", SqlDbType.VarChar) { Value = currentInfo.HotelId }
                        , new SqlParameter("@regid", SqlDbType.VarChar) { Value = item.RegId }
                        , new SqlParameter("@itemid", SqlDbType.VarChar) { Value = itemId }
                        , new SqlParameter("@quantity", SqlDbType.Decimal) { Value = 1 }
                        , new SqlParameter("@amount", SqlDbType.Decimal) { Value = 0 }//金额
                        , new SqlParameter("@invNo", SqlDbType.VarChar) { Value = invNo ?? "" }
                        , new SqlParameter("@inputUser", SqlDbType.VarChar) { Value = currentInfo.UserName ?? "" }
                        , new SqlParameter("@remark", SqlDbType.VarChar) { Value = orderRemark }
                        , new SqlParameter("@transShift", SqlDbType.VarChar) { Value = currentInfo.ShiftId ?? "" }
                        , new SqlParameter("@isauto", SqlDbType.TinyInt) { Value = 1 }//夜租
                        , new SqlParameter("@refno", SqlDbType.VarChar) { Value = string.Format("{0}-{1}", refNo, index) ?? "" }
                        , new SqlParameter("@inputDate", SqlDbType.DateTime) { Value = rateItem.Ratedate.Date.AddDays(1).AddMinutes(-1) }
                        , new SqlParameter("@transBsnsDate", SqlDbType.Date) { Value = rateItem.Ratedate.Date }
                        ).FirstOrDefault();
                        if (folioInput == null || folioInput.Success == false)
                        {
                            return JsonResultData.Failure(message + folioInput.Data);
                        }
                        index++;
                    }
                }
                return JsonResultData.Successed();
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        #endregion

        private DbHotelPmsContext _pmsContext;
        private INotifyService _notifyService;
    }
}
