using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.WxProviderPay;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Hotel.Services.EF.PosManage;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using HttpHelper = Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models.HttpHelper;
using PosTabService = Gemstar.BSPMS.Hotel.Services.EF.PosManage.PosTabService;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Controllers
{
    [NotAuth]
    public class OrderController : BaseScanOrderController
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <param name="openid">Openid</param>
        /// <returns></returns>
        public ActionResult Index(string hid, string tabid, string openid, string billId)
        {
            ViewBag.Title = "首页";


            SetBaseInfo(hid, tabid, openid);

            var hotelDb = GetHotelDb(BaseHid);

            //获取酒店名称
            var hotelInfoService = GetService<IHotelInfoService>();
            ViewBag.Title = hotelInfoService.GetHotelShortName(hid) ?? "";

            //banner
            var bannerService = new PosMBannerService(hotelDb);
            var banners = bannerService.GetMBannerList(BaseHid);

            //滚动菜式
            var scrollService = new PosMScrollService(hotelDb);

            var scrolls = scrollService.GetPosMScrollItemList(BaseHid);

            //餐台
            var tabService = new PosTabService(hotelDb);
            var tab = tabService.Get(BaseTabid);

            var refeService = new PosRefeService(hotelDb);
            var refe = refeService.Get(tab.Refeid);

            ViewBag.BillId = billId;

            ViewBag.TabName = tab.Cname;    //餐台名
            ViewBag.RefeName = refe.Cname;//营业点名称
            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;
            ViewBag.Openid = BaseOpenid;

            ViewBag.BannerList = banners;
            ViewBag.ScrollList = scrolls;

            //ViewBag.BillDetailList = ScanbillDetailList;
            //ViewBag.BillDetailActionList = billDetailActionList;
            return View();


        }

        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <returns></returns>
        public ActionResult DownForm(string hid, string tabid)
        {
            SetBaseInfo(hid, tabid);

            ViewBag.Title = "订单";
            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;

            return View();
        }

        /// <summary>
        /// 买单成功
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="billDetailId">付款明细ID</param>
        ///  <param name="openFlag">控制跳转界面（1：支付成功之后显示订单详情，2：支付成功直接关闭）</param>
        /// <returns></returns>
        public ActionResult PaymentSuccess(string hid, string tabid, string billid, long billDetailId, string openFlag = "")
        {
            ViewBag.Title = "支付成功";

            SetBaseInfo(hid, tabid);


            if (!string.IsNullOrWhiteSpace(BaseHid) && !string.IsNullOrWhiteSpace(BaseTabid) && !string.IsNullOrWhiteSpace(billid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var billService = new PosBillService(hotelDb);
                var bill = billService.Get(billid);

                var billDetailService = new PosBillDetailService(hotelDb);


                if (bill != null && (bill.Status == (byte)PosBillStatus.开台 || bill.Status == (byte)PosBillStatus.扫码点餐默认状态))
                {
                    var refeService = new PosRefeService(hotelDb);
                    var refe = refeService.GetEntity(BaseHid, bill.Refeid);

                    var posService = new PosPosService(hotelDb);
                    var pos = posService.Get(refe.PosId);

                    //点菜记录的状态全部修改成已支付
                    var morderListService = new PosMorderListService(hotelDb);
                    var morderList = morderListService.GetPosMorderListByBillId(BaseHid, bill.Billid);
                    if (morderList != null)
                    {
                        foreach (var item in morderList)
                        {
                            item.IStatus = (byte)PosMorderListIStatus.已付;
                            morderListService.Update(item, new PosMorderList());
                            morderListService.Commit();
                        }
                    }


                    var oldBillDetail = billDetailService.GetEntity(BaseHid, billDetailId);

                    var paymentTotal = billDetailService.GetBillDetailForPaymentTotalByBillid(BaseHid, oldBillDetail.Billid);
                    PosBillDetail billDetail = new PosBillDetail()
                    {
                        Hid = BaseHid,
                        Tabid = bill.Tabid,
                        ItemCode = oldBillDetail.ItemCode,
                        ItemName = oldBillDetail.ItemName,
                        DcFlag = PosItemDcFlag.C.ToString(),
                        Status = (byte)PosBillDetailStatus.正常,
                        Isauto = (byte)PosBillDetailIsauto.付款,
                        IsCheck = true,

                        Settleid = oldBillDetail.Settleid,
                        SettleDate = DateTime.Now,
                        SettleShiftId = oldBillDetail.SettleShiftId,
                        SettleBsnsDate = oldBillDetail.SettleBsnsDate,
                        SettleShuffleid = oldBillDetail.SettleShuffleid,
                        SettleUser = "扫码点餐",
                        OrderType = PosOrderType.Mobile.ToString(),
                    };

                    #region 更新账务

                    //更新待支付
                    hotelDb.Database.ExecuteSqlCommand($"update WaitPayList SET PayDate = GETDATE(),[Status] = 1 WHERE WaitPayId = '{Guid.Parse(oldBillDetail.SettleTransno)}' AND ProductType = '{PayProductType.PosPayment.ToString()}' AND [Status] = 0");

                    //更新付款状态
                    oldBillDetail.IsCheck = true;
                    oldBillDetail.Status = (byte)PosBillDetailStatus.正常;
                    billDetailService.Update(oldBillDetail, new PosBillDetail());
                    billDetailService.AddDataChangeLog(OpLogType.Pos账单付款明细修改);
                    billDetailService.Commit();

                    #endregion 更新账务

                    //获取付款金额合计数据
                    var paymentTotalNew = billDetailService.GetBillDetailForPaymentTotalByBillid(BaseHid, oldBillDetail.Billid);
                    var rateUnPaid = billDetailService.GetAmountByBillTailProcessing(BaseHid, bill.Refeid, oldBillDetail.Amount ?? 0);
                    if (paymentTotalNew.Paid >= paymentTotalNew.Total || billDetail.Dueamount == rateUnPaid)
                    {
                        #region 消费项目结账信息添加

                        var posBillDetails = billDetailService.GetBillDetailByDcFlag(BaseHid, bill.Billid, PosItemDcFlag.D.ToString()).Where(w => w.Settleid == null).ToList();
                        if (posBillDetails != null && posBillDetails.Count > 0)
                        {
                            foreach (var detail in posBillDetails)
                            {
                                detail.IsCheck = true;
                                detail.Settleid = oldBillDetail.Settleid;
                                detail.SettleDate = DateTime.Now;
                                detail.SettleShiftId = oldBillDetail.SettleShiftId;
                                detail.SettleBsnsDate = oldBillDetail.SettleBsnsDate;
                                detail.SettleShuffleid = oldBillDetail.SettleShuffleid;
                                detail.SettleUser = "扫码点餐";

                                billDetailService.Update(detail, new PosBillDetail());
                                billDetailService.Commit();
                            }
                        }

                        #endregion 消费项目结账信息添加

                        #region 尾数处理差额

                        PosBillDetail billDetailWs = new PosBillDetail()
                        {
                            Hid = BaseHid,
                            Billid = oldBillDetail.Billid,
                            MBillid = oldBillDetail.MBillid,
                            ItemCode = "抹零",
                            ItemName = "抹零",
                            IsCheck = true,
                            DcFlag = PosItemDcFlag.D.ToString(),
                            Isauto = (byte)PosBillDetailIsauto.抹零,
                            Status = (byte)PosBillDetailStatus.正常,
                            IsProduce = (byte)PosBillDetailIsProduce.已出品,

                            TransUser = "扫码点餐",
                            TransBsnsDate = bill.BillBsnsDate,
                            TransShiftid = bill.Shiftid,
                            TransShuffleid = bill.Shuffleid,
                            TransDate = DateTime.Now,

                            Settleid = oldBillDetail.Settleid,
                            SettleDate = DateTime.Now,
                            SettleShiftId = oldBillDetail.SettleShiftId,
                            SettleBsnsDate = oldBillDetail.SettleBsnsDate,
                            SettleShuffleid = refe.ShuffleId,
                            SettleUser = "扫码点餐",
                        };

                        //汇率换算导致的差额
                        decimal tailDifference = 0;
                        if (billDetail.Dueamount == rateUnPaid)
                        {
                            tailDifference = Convert.ToDecimal(paymentTotalNew.Paid - paymentTotalNew.Consume);
                            billDetailWs.Dueamount = tailDifference;
                            billDetailWs.Amount = tailDifference;
                        }

                        //有抹零则修改，无抹零则增加
                        var temp = billDetailService.GetEntity(BaseHid, oldBillDetail.Billid, (byte)PosBillDetailIsauto.抹零);
                        if (temp == null)
                        {
                            billDetailService.Add(billDetailWs);
                            billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            billDetailService.Commit();
                        }
                        else
                        {
                            temp.CanReason = "";
                            temp.Dueamount = billDetailWs.Dueamount;
                            temp.Amount = billDetailWs.Amount + temp.Amount;
                            temp.Status = (byte)PosBillDetailStatus.正常;
                            billDetailService.Update(temp, new PosBillDetail());
                            billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                            billDetailService.Commit();
                        }

                        #endregion 尾数处理差额

                        #region 增加找赎明细

                        if (paymentTotalNew.Paid - tailDifference > paymentTotalNew.Total)
                        {
                            try
                            {
                                decimal? smallChange = 0; //找赎
                                smallChange = paymentTotalNew.Total - paymentTotalNew.Paid;
                                oldBillDetail.Amount = smallChange;
                                billDetail.Dueamount = smallChange;
                                billDetail.Isauto = (byte)PosBillDetailIsauto.找赎;
                                billDetail.Status = (byte)PosBillDetailStatus.找赎;
                                if (oldBillDetail.Amount < 0)
                                {
                                    var itemService = new PosItemService(hotelDb);
                                    var item = itemService.Get(oldBillDetail.Itemid);

                                    decimal exchangeRate = smallChange / item.Rate ?? 0;

                                    billDetail.Dueamount = billDetailService.GetAmountByBillTailProcessing(BaseHid, bill.Refeid, exchangeRate);

                                    billDetailService.Add(billDetail);
                                    billDetailService.AddDataChangeLog(OpLogType.Pos账单付款明细增加);
                                    billDetailService.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonResultData.Failure(ex));
                            }
                        }

                        #endregion 增加找赎明细

                        #region 回填BillNo
                        if (bill.Status == (byte)PosBillStatus.扫码点餐默认状态)
                        {
                            var newPosBill = billService.GetLastBillId(BaseHid, refe.Id, pos.Business);
                            bill.BillNo = newPosBill.BillNo;
                            billService.Update(bill, new PosBill());
                            billService.AddDataChangeLog(OpLogType.Pos账单修改);
                            billService.Commit();
                        }
                        #endregion

                        #region 清台

                        if (refe.Isclrtab == true)
                        {
                            try
                            {
                                //修改账单状态
                                bill.DepBsnsDate = pos.Business;
                                bill.MoveUser = "扫码点餐";
                                bill.DepDate = DateTime.Now;
                                bill.Status = (byte)PosBillStatus.清台;

                                billService.Update(bill, new PosBill());
                                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                                billService.Commit();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonResultData.Failure(ex));
                            }
                        }
                        else if (refe.Isclrtab == false)
                        {
                            try
                            {
                                //修改账单状态
                                bill.DepBsnsDate = pos.Business;
                                bill.MoveUser = "扫码点餐";
                                bill.DepDate = DateTime.Now;
                                if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                                {
                                    bill.Status = (byte)PosBillStatus.结账;
                                }
                                else
                                {
                                    bill.Status = (byte)PosBillStatus.清台;
                                }
                                billService.Update(bill, new PosBill());
                                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                                billService.Commit();
                            }
                            catch (Exception ex)
                            {
                                return Json(JsonResultData.Failure(ex));
                            }
                        }
                        //修改餐台状态
                        PosCommon common = new PosCommon();
                        SetTabStatus(bill, hotelDb);
                        //  common.SetTabStatus(bill);

                        var statusService = new PosTabStatusService(hotelDb);

                        //清理锁台记录
                        var tabLogService = new PosTabLogService(hotelDb);
                        var tabLogList = tabLogService.GetPosTabLogListByTab(BaseHid, bill.Refeid, bill.Tabid, bill.TabNo);
                        if (tabLogList != null && tabLogList.Count > 0)
                        {
                            foreach (var tabLog in tabLogList)
                            {
                                if (tabLog.Billid == bill.Billid)
                                {
                                    tabLogService.Delete(tabLog);
                                    tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                                    tabLogService.Commit();
                                }
                            }
                        }

                        #endregion 清台

                    }
                }

            }

            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;
            ViewBag.OpenFlag = openFlag;
            ViewBag.Billid = billid;
            return View();
        }

        /// <summary>
        /// 设置餐台状态
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="db"></param>
        private void SetTabStatus(PosBill bill, DbHotelPmsContext db)
        {
            var statusService = new PosTabStatusService(db);

            var billService = new PosBillService(db);
            //修改餐台状态
            var searList = billService.GetSmearListByClearTab(BaseHid, bill.Tabid);
            if (searList != null && searList.Count < 1)
            {
                if (bill.TabFlag == (byte)PosBillTabFlag.物理台)
                {
                    var tabStatus = statusService.Get(bill.Tabid);

                    if (tabStatus != null)
                    {
                        var newtabStatus = new PosTabStatus();
                        AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                        if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消 || bill.Status == (byte)PosBillStatus.迟付)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                            newtabStatus.OpTabid = null;
                            newtabStatus.OpenGuest = null;
                            newtabStatus.GuestName = null;
                            newtabStatus.OpenRecord = null;
                        }
                        else if (bill.Status == (byte)PosBillStatus.结账)
                        {
                            newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                        }

                        statusService.Update(newtabStatus, tabStatus);
                        statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                        statusService.Commit();
                    }
                }
            }
            else
            {
                //判断
                var tabStatus = statusService.Get(bill.Tabid);


                if (tabStatus != null && tabStatus.OpTabid == bill.Billid)
                {
                    var newtabStatus = new PosTabStatus();
                    AutoSetValueHelper.SetValues(tabStatus, newtabStatus);
                    //if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.取消)
                    //{
                    //  newtabStatus.TabStatus = (byte)PosTabStatusEnum.空净;
                    newtabStatus.OpTabid = null;
                    newtabStatus.OpenGuest = null;
                    newtabStatus.GuestName = null;
                    newtabStatus.OpenRecord = null;
                    //}
                    //else if (bill.Status == (byte)PosBillStatus.结账)
                    //{
                    //    newtabStatus.TabStatus = (byte)PosTabStatusEnum.已买单未离座;
                    //}

                    statusService.Update(newtabStatus, tabStatus);
                    statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                    statusService.Commit();
                }


            }
        }

        /// <summary>
        /// 购物车视图
        /// </summary>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult _ShopCart(string list)
        {
            decimal? totalPrice = 0.00M;

            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
            var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

            foreach (var billDetail in billDetailList)
            {
                totalPrice += billDetail.Price * billDetail.Quantity;
            }

            ViewBag.TotalPrice = totalPrice ?? 0.00M;
            return PartialView(billDetailList);
        }

        /// <summary>
        /// 加数量
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <param name="flag">区分正常添加数量与选择规格界面</param>
        /// <returns></returns>
        public ActionResult AddQuantity(ScanPosBillDetail model, BillDetailListAndActionList allList, string flag = "A")
        {
            SetBaseInfo(model.Hid, model.Tabid);

            // var AllModel = jsonToEntityList(allList);   //处理消费项目以及作法匹配
            var list = string.IsNullOrWhiteSpace(allList.billDetailList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetail>()) : allList.billDetailList;
            var billDetailList = JsonConvert.DeserializeObject<List<ScanPosBillDetail>>(list);

            var billDetailActions = new List<ScanPosBillDetailAction>();// 作法
            var hotelDb = GetHotelDb(BaseHid);      //获取数据库连接
            decimal? addPriceAmount = 0.00M;   //作法加价金额

            var actionGroups = new List<ActionGroup>(); //作法分组

            //作法
            var actionStr = string.IsNullOrWhiteSpace(allList.ActionList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetailAction>()) : allList.ActionList;
            billDetailActions = JsonConvert.DeserializeObject<List<ScanPosBillDetailAction>>(actionStr);

            //作法分组
            var actionGroupStr = string.IsNullOrWhiteSpace(allList.GroupList) ? JsonConvert.SerializeObject(new List<ActionGroup>()) : allList.GroupList;
            actionGroups = JsonConvert.DeserializeObject<List<ActionGroup>>(actionGroupStr);

            if (model != null)
            {
                var isAdd = true;
                foreach (var billDetail in billDetailList)
                {
                    if (model.Itemid == billDetail.Itemid)
                    {
                        if (flag == "A")
                        {

                            billDetail.Quantity = model.Quantity;
                            billDetail.Amount = model.Price * model.Quantity;
                        }


                        isAdd = false;
                        break;
                    }

                }

                if (isAdd)
                {
                    model.Hid = BaseHid;
                    model.Tabid = BaseTabid;
                    model.Amount = model.Price * model.Quantity;
                    model.OrderId = (billDetailList == null || billDetailList.Count <= 0) ? 1 : billDetailList.Max(w => w.OrderId) + 1;
                    //model.OrderId = billDetailList.Max(w => w.OrderId)==null?1: billDetailList.Max(w => w.OrderId) + 1;
                    if (flag == "B")    //选择规格界面之前的操作。
                    {

                        var addActionList = billDetailActionList(hotelDb, model.Itemid, model.Quantity, model.OrderId, out addPriceAmount);
                        foreach (var item in addActionList)
                        {
                            billDetailActions.Add(item);
                        }


                        var AddactionGroupList = billDetailActions.GroupBy(g => g.Igroupid).Select(s => new ActionGroup { OrderId = model.OrderId, Igroupid = s.First().Igroupid, ActionIds = string.Join(",", s.Select(a => a.ActionNo.Trim())), ActionNames = string.Join(",", s.Select(a => a.ActionName.Trim())) }).ToList();
                        foreach (var item in AddactionGroupList)
                        {
                            actionGroups.Add(item);
                        }


                        var actions = "";
                        foreach (var temp in AddactionGroupList)
                        {
                            actions += "/" + temp.ActionNames;
                        }

                        model.Action = actions.Trim('/');
                        model.AddPrice = addPriceAmount;
                    }
                    billDetailList.Add(model);

                }
            }
            return Json(JsonResultData.Successed(new { DetailList = billDetailList, ActionList = billDetailActions, GroupList = actionGroups }));


        }



        /// <summary>
        /// 减数量
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult ReductionQuantity(PosBillDetail model, string list)
        {
            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
            var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

            if (model != null)
            {
                var billDetails = new List<PosBillDetail>();
                foreach (var billDetail in billDetailList)
                {
                    if (model.Itemid == billDetail.Itemid)
                    {
                        if (model.Quantity == null || model.Quantity == 0)
                        {
                            billDetails.Add(billDetail);
                        }
                        else
                        {
                            billDetail.Quantity = model.Quantity;
                            billDetail.Amount = model.Price * model.Quantity;
                        }
                    }
                }

                foreach (var temp in billDetails)
                {
                    billDetailList.Remove(temp);
                }
            }

            return Json(JsonConvert.SerializeObject(billDetailList));
        }

        /// <summary>
        /// 账单明细列表
        /// </summary>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult _BillDetailList(string list)
        {
            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
            var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);
            return PartialView(billDetailList);
        }

        /// <summary>
        /// 获取总价
        /// </summary>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult RefreshTotalPrice(string list)
        {
            decimal? totalPrice = 0.00M;
            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
            var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

            foreach (var billDetail in billDetailList)
            {
                totalPrice += billDetail.Price * billDetail.Quantity;
            }

            return Content(totalPrice.Value.ToString(""));
        }

        /// <summary>
        /// 获取总价
        /// </summary>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult RefreshItemPrice(string itemid, string list)
        {
            decimal totalPrice = 0.00M;
            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
            var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

            foreach (var billDetail in billDetailList)
            {
                if (billDetail.Itemid == itemid)
                {
                    var price = billDetail.Price ?? 0.00M;
                    var quantity = billDetail.Quantity ?? 0.00M;

                    totalPrice = quantity * price;
                }
            }

            return Content(totalPrice.ToString("F2"));
        }

        /// <summary>
        /// 消费项目分类列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <returns></returns>
        public ActionResult _ItemSubClassList(string hid, string tabid)
        {
            SetBaseInfo(hid, tabid);

            var model = new List<PosItem>();
            var hotelDb = GetHotelDb(BaseHid);
            var tab = hotelDb.PosTabs.Where(m => m.Hid == BaseHid && m.Id == BaseTabid).FirstOrDefault();
            if (tab != null)
            {
                var service = new PosItemService(hotelDb);
                model = service.GetScanPosSubClassList(BaseHid, tab.Refeid);

                //model = hotelDb.PosItems.Where(m => m.Hid == BaseHid && m.IsSubClass == true && m.DcFlag == PosItemDcFlag.D.ToString()).OrderBy(m => m.Seqid).ToList();

                //var items = new List<PosItem>();
                //foreach (var temp in model)
                //{

                //    if (!hotelDb.PosItems.Any(m => m.Hid == BaseHid && m.IsSubClass == false && m.SubClassid == temp.Id && m.DcFlag == PosItemDcFlag.D.ToString()))
                //    {
                //        items.Add(temp);
                //    }
                //}

                //foreach (var temp in items)
                //{
                //    model.Remove(temp);
                //}
            }

            return PartialView(model);
        }

        /// <summary>
        /// 根据分类获取消费项目列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="classid">分类ID</param>
        /// <returns></returns>
        public ActionResult _ItemList(string hid, string classid)
        {
            SetBaseInfo(hid);

            var model = new List<up_pos_scan_list_ItemListBySubClassidResult>();
            if (!string.IsNullOrEmpty(BaseHid) && !string.IsNullOrEmpty(classid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                model = hotelDb.Database.SqlQuery<up_pos_scan_list_ItemListBySubClassidResult>("exec up_pos_scan_list_ItemListBySubClassid @hid = @hid,@subClassid = @subClassid"
                    , new SqlParameter("hid", BaseHid)
                    , new SqlParameter("subClassid", classid)).ToList();
            }

            return PartialView(model);
        }

        /// <summary>
        /// Banner视图
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public ActionResult _MBannerList(string hid)
        {
            SetBaseInfo(hid);

            var model = new List<PosMBanner>();
            if (!string.IsNullOrWhiteSpace(BaseHid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var bannerService = new PosMBannerService(hotelDb);

                model = bannerService.GetMBannerList(BaseHid);
            }

            return PartialView(model);
        }

        /// <summary>
        /// 滚动菜式视图
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public ActionResult _MScrollList(string hid)
        {
            SetBaseInfo(hid);

            var model = new List<PosMScroll>();
            var list = new List<MscroList>();
            if (!string.IsNullOrEmpty(BaseHid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var scrollService = new PosMScrollService(hotelDb);

                model = scrollService.GetPosMScrollList(BaseHid);
                //新的集合接收数据


                var priceService = new PosItemPriceService(hotelDb);

                foreach (var item in model)
                {
                    var price = priceService.GetPosItemPriceByUnitid(BaseHid, item.Itemid, item.Unitid);
                    var mscroList = new MscroList();
                    AutoSetValueHelper.SetValues(item, mscroList);
                    mscroList.Price = price.Price;
                    list.Add(mscroList);
                }
            }

            return PartialView(list);
        }

        /// <summary>
        /// 选规格视图
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult _SelectSpec(ScanPosBillDetail model, BillDetailListAndActionList allList, string subclassid)
        {
            SetBaseInfo(model.Hid);

            var selectSpec = new SelectSpecModel();
            if (!string.IsNullOrWhiteSpace(BaseHid) && model != null && !string.IsNullOrWhiteSpace(model.Itemid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                //账单明细
                var list = string.IsNullOrWhiteSpace(allList.billDetailList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetail>()) : allList.billDetailList;
                var billDetailList = JsonConvert.DeserializeObject<List<ScanPosBillDetail>>(list);

                //作法
                var actionStr = string.IsNullOrWhiteSpace(allList.ActionList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetailAction>()) : allList.ActionList;
                var ActionList = JsonConvert.DeserializeObject<List<ScanPosBillDetailAction>>(actionStr);

                var newActionList = new List<ScanPosBillDetailAction>();
                //作法分组
                var actionGroupStr = string.IsNullOrWhiteSpace(allList.GroupList) ? JsonConvert.SerializeObject(new List<ActionGroup>()) : allList.GroupList;
                var actionGroupList = new List<ActionGroup>();// JsonConvert.DeserializeObject<List<ActionGroup>>(actionGroupStr);

                foreach (var billDetail in billDetailList)
                {
                    if (model.Itemid == billDetail.Itemid)
                    {
                        model = billDetail;
                        foreach (var action in ActionList.Where(m => m.OrderId == billDetail.OrderId))
                        {
                            newActionList.Add(action);
                        }
                        break;
                    }
                }

                model.Amount = model.Price * model.Quantity;


                var priceService = new PosItemPriceService(hotelDb);
                var priceByItemidResults = priceService.GetPosItemPriceByItemId(BaseHid, model.Itemid);

                //获取消费项目对应作法
                var itemActionService = new PosItemActionService(hotelDb);
                var actionByItemidResults = itemActionService.GetPosItemActionListByItemId(BaseHid, model.Itemid);

                //获取要求列表
                var requestService = new PosRequestService(hotelDb);
                var requests = requestService.GetPosRequestByModule(BaseHid, ModuleCode.CY.ToString());

                //var billDetailActions = new List<PosBillDetailAction>();
                var actionGroups = new List<ActionGroup>();
                actionGroups = newActionList.GroupBy(g => g.Igroupid).Select(s => new ActionGroup { Igroupid = s.First().Igroupid, ActionIds = string.Join(",", s.Select(a => a.ActionNo.Trim())), ActionNames = string.Join(",", s.Select(a => a.ActionName.Trim())) }).ToList();
                var actions = "";
                foreach (var temp in actionGroups)
                {
                    actions += "/" + temp.ActionNames;
                }

                model.Action = actions.Trim('/');

                selectSpec.PosBillDetailActions = newActionList;
                selectSpec.PosItemActions = actionByItemidResults;
                selectSpec.PosItemPrices = priceByItemidResults;
                selectSpec.ActionGroups = actionGroups;
                selectSpec.PosRequests = requests;
                selectSpec.BillDetail = model;
                selectSpec.Subclassid = subclassid;
            }

            return PartialView(selectSpec);
        }

        /// <summary>
        /// 更新消费项目作法
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult UpdateItemAction(string actionId, string igroupid, PosBillDetail detail, string detailList, string actionList, string groupList)
        {
            //消费项目
            var hotelDb = GetHotelDb(BaseHid);
            var itemService = new PosItemService(hotelDb);
            var item = itemService.GetEntity(BaseHid, detail.Itemid);

            var actionService = new PosActionService(hotelDb);
            var itemActionService = new PosItemActionService(hotelDb);

            //账单明细
            detailList = string.IsNullOrWhiteSpace(detailList) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : detailList;
            var details = JsonConvert.DeserializeObject<List<PosBillDetail>>(detailList);

            //明细对应作法列表
            actionList = string.IsNullOrWhiteSpace(actionList) ? JsonConvert.SerializeObject(new List<PosBillDetailAction>()) : actionList;
            var billDetailActions = JsonConvert.DeserializeObject<List<PosBillDetailAction>>(actionList);

            //作法分组列表
            groupList = string.IsNullOrWhiteSpace(groupList) ? JsonConvert.SerializeObject(new List<ActionGroup>()) : groupList;
            var actionGroups = JsonConvert.DeserializeObject<List<ActionGroup>>(groupList);

            #region 更新账单明细作法
            if (!string.IsNullOrEmpty(actionId))
            {
                var itemAction = itemActionService.GetPosItemActionListById(BaseHid, Guid.Parse(actionId));
                if (itemAction != null)
                {
                    var action = actionService.GetActionByID(BaseHid, itemAction.Actionid);
                    if (action != null)
                    {
                        var detailAction = billDetailActions.Where(w => w.Hid == BaseHid && w.ActionNo == action.Code).FirstOrDefault();
                        if (detailAction == null)
                        {
                            #region 账单明细对应作法赋值

                            var billDetailAction = new PosBillDetailAction
                            {
                                Hid = BaseHid,
                                ActionName = action.Cname,
                                ActionNo = action.Code,
                                AddPrice = itemAction.AddPrice,

                                IByGuest = itemAction.IsByGuest,
                                IByQuan = itemAction.IsByQuan,
                                Igroupid = Convert.ToInt32(igroupid),
                                Memo = "扫码点餐",
                                ModiDate = DateTime.Now,
                                ModiUser = "扫码点餐",
                                Nmultiple = itemAction.Multiple,
                                PrtNo = itemAction.ProdPrinter,

                                DeptClassid = item.DeptClassid,
                                Quan = detail.Quantity,
                            };

                            #endregion 账单明细对应作法赋值

                            #region 作法加价

                            decimal amount = 0.00M;
                            if (action.AddPrice != null && action.AddPrice > 0)
                            {
                                if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                                {
                                    amount = Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(detail.Quantity) * 1;
                                }
                                else if (action.IsByQuan != null && action.IsByQuan.Value)
                                {
                                    amount = Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(detail.Quantity);
                                }
                                else if (action.IsByGuest != null && action.IsByGuest.Value)
                                {
                                    amount = Convert.ToDecimal(action.AddPrice) * 1;
                                }
                                else
                                {
                                    amount = Convert.ToDecimal(action.AddPrice);
                                }
                            }

                            billDetailAction.Amount = amount;
                            detail.Amount += billDetailAction.Amount;

                            billDetailActions.Add(billDetailAction);
                            #endregion 作法加价
                        }
                        else    //移除
                        {
                            billDetailActions.Remove(detailAction);
                        }

                        #region 添加作法到账单明细
                        actionGroups = billDetailActions.GroupBy(g => g.Igroupid).Select(s => new ActionGroup { Igroupid = s.First().Igroupid, ActionIds = string.Join(",", s.Select(a => a.ActionNo.Trim())), ActionNames = string.Join(",", s.Select(a => a.ActionName.Trim())) }).ToList();

                        var actions = "";
                        foreach (var temp in actionGroups)
                        {
                            actions += "/" + temp.ActionNames;
                        }

                        detail.Action = actions.Trim('/');
                        #endregion 添加作法到账单明细
                    }
                }
            }
            #endregion 更新账单明细作法

            # region 修改账单明细
            var billDetail = details.Any(m => m.Hid == BaseHid && m.Itemid == detail.Itemid);
            if (billDetail)
            {
                foreach (var temp in details)
                {
                    if (temp.Itemid == detail.Itemid)
                    {
                        temp.Action = detail.Action;
                    }
                }
            }
            else
            {
                details.Add(detail);
            }
            #endregion

            return Json(JsonResultData.Successed(new { Detail = detail, DetailList = details, ActionList = billDetailActions, GroupList = actionGroups }));
        }

        /// <summary>
        /// 添加账单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult AddBill(string hid, string tabid, string billId, string openid, BillDetailListAndActionList allList)
        {
            SetBaseInfo(hid, tabid);



            if (!string.IsNullOrEmpty(BaseHid) && !string.IsNullOrEmpty(BaseTabid))
            {
                var hotelDb = GetHotelDb(BaseHid);


                var tabService = new Services.EF.PosManage.PosTabService(hotelDb);
                var tab = tabService.GetEntity(BaseHid, BaseTabid);

                var refeService = new PosRefeService(hotelDb);
                var refe = refeService.GetEntity(BaseHid, tab.Refeid);

                var posService = new PosPosService(hotelDb);
                var pos = posService.Get(refe.PosId);

                var billService = new PosBillService(hotelDb);

                var billDetailService = new PosBillDetailService(hotelDb);


                PosBill posBill = new PosBill();

                //获取传递进来的账单明细数据
                var billDetailListStr = string.IsNullOrWhiteSpace(allList.billDetailList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetail>()) : allList.billDetailList;
                var billDetailList = JsonConvert.DeserializeObject<List<ScanPosBillDetail>>(billDetailListStr);

                //获取传递进来的账单明细作法
                var billDetailActionStr = string.IsNullOrWhiteSpace(allList.ActionList) ? JsonConvert.SerializeObject(new List<ScanPosBillDetailAction>()) : allList.ActionList;
                var billDetailActionList = JsonConvert.DeserializeObject<List<ScanPosBillDetailAction>>(billDetailActionStr);

                try
                {
                    //餐台类型数据
                    var tabTypeService = new PosTabtypeService(hotelDb);
                    var tabType = tabTypeService.Get(tab.TabTypeid);
                    //判断是否有账单ID
                    if (!string.IsNullOrWhiteSpace(billId))
                    {



                        posBill = billService.GetEntity(BaseHid, billId);

                        if (posBill.Status == (byte)PosBillStatus.扫码点餐默认状态)
                        {
                            //var newPosBill = billService.GetLastBillId(hid, refe.Id, pos.Business);

                            //posBill.BillNo = newPosBill.BillNo;
                            posBill.BillDate = DateTime.Now;
                            posBill.BillBsnsDate = pos.Business;
                            billService.Update(posBill, new PosBill());
                            billService.Commit();
                        }
                        AddBillDetail(hotelDb, posBill, billDetailList, billDetailActionList);
                        //重算金额
                        billDetailService.CmpBillAmountByScanOrder(BaseHid, billId);
                        if (posBill.Status == (byte)PosBillStatus.开台)
                            //处理出品程序
                            billDetailService.CmpProducelist(BaseHid, billId);
                        // var oldBillDetail = billDetailService.GetEntity(BaseHid, billDetailId);

                        var result = new { Billid = posBill.Billid, wxPaytype = tabType.WxPaytype.ToString() };

                        return Json(JsonResultData.Successed(result));
                    }
                    else
                    {
                        DateTime openTime = DateTime.Now;
                        var posTabServiceService = new PosTabServiceService(hotelDb);
                        var posTabService = posTabServiceService.GetPosTabService(BaseHid, ModuleCode.CY.ToString(), tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                        //如果不存在账单ID。判断餐台的类型是否是后付还是先付 先付模式直接产生客账号，后付模式查看是否有存在的客账号
                        if (tabType.WxPaytype == 2)   //后付模式
                        {
                            posBill = billService.GetPosbillByScanOrder(BaseHid, BaseTabid);
                            if (posBill == null)
                            {
                                posBill = billService.GetLastBillId(BaseHid, tab.Refeid, pos.Business);
                                posBill.Hid = BaseHid;
                                posBill.Tabid = tab.Id;
                                posBill.TabNo = tab.TabNo;
                                posBill.CustomerTypeid = "";
                                posBill.Billid = posBill.Billid;
                                posBill.BillNo = tabType.WxPaytype == 1 ? "" : posBill.BillNo;  //1是点菜支付，客账号清空
                                posBill.MBillid = posBill.Billid;
                                posBill.InputUser = "扫码点餐";
                                posBill.BillDate = openTime;
                                posBill.IsService = true;
                                posBill.IsLimit = true;

                                posBill.ServiceRate = posTabService.Servicerate ?? 1;
                                posBill.Limit = posTabService.NLimit ?? 0;
                                posBill.IsByPerson = posTabService.IsByPerson == 0 ? false : true;
                                posBill.IHour = posTabService.LimitTime ?? 0;
                                posBill.Discount = posTabService.Discount ?? 1;

                                posBill.TabFlag = (byte)PosBillTabFlag.物理台;
                                posBill.Status = tabType.WxPaytype == 1 ? (byte)PosBillStatus.扫码点餐默认状态 : (byte)PosBillStatus.开台;
                                posBill.Shiftid = pos.ShiftId;
                                posBill.Shuffleid = refe.ShuffleId;

                                posBill.Refeid = refe.Id;
                                posBill.BillBsnsDate = pos.Business;
                                posBill.IGuest = 1;
                                posBill.ServiceRate = posBill.ServiceRate > 1 && posBill.ServiceRate <= 100 ? posBill.ServiceRate / 100 : posBill.ServiceRate;
                                posBill.Discount = posBill.Discount > 1 && posBill.Discount <= 100 ? posBill.Discount / 100 : posBill.Discount;
                                posBill.OpenWxid = openid;
                                billService.Add(posBill);
                                //billService.AddDataChangeLog(OpLogType.Pos账单增加);
                                billService.Commit();
                                AddOperationLog(OpLogType.Pos账单增加, "单号：" + posBill.Billid + ",台号：" + posBill.Tabid + "，状态：" + Enum.GetName(typeof(PosBillStatus), posBill.Status), posBill.BillNo);
                            }
                        }
                        else
                        {
                            //先付模式 产生客账号
                            posBill = billService.GetLastBillId(BaseHid, tab.Refeid, pos.Business);
                            //新增账单
                            posBill.Hid = BaseHid;
                            posBill.Tabid = tab.Id;
                            posBill.TabNo = tab.TabNo;
                            posBill.CustomerTypeid = "";
                            posBill.Billid = posBill.Billid;
                            posBill.BillNo = tabType.WxPaytype == 1 ? "" : posBill.BillNo;  //1是点菜支付，客账号清空
                            posBill.MBillid = posBill.Billid;
                            posBill.InputUser = "扫码点餐";
                            posBill.BillDate = openTime;
                            posBill.IsService = true;
                            posBill.IsLimit = true;

                            posBill.ServiceRate = posTabService.Servicerate ?? 1;
                            posBill.Limit = posTabService.NLimit ?? 0;
                            posBill.IsByPerson = posTabService.IsByPerson == 0 ? false : true;
                            posBill.IHour = posTabService.LimitTime ?? 0;
                            posBill.Discount = posTabService.Discount ?? 1;

                            posBill.TabFlag = (byte)PosBillTabFlag.物理台;
                            posBill.Status = tabType.WxPaytype == 1 ? (byte)PosBillStatus.扫码点餐默认状态 : (byte)PosBillStatus.开台;
                            posBill.Shiftid = pos.ShiftId;
                            posBill.Shuffleid = refe.ShuffleId;

                            posBill.Refeid = refe.Id;
                            posBill.BillBsnsDate = pos.Business;
                            posBill.IGuest = 1;
                            posBill.ServiceRate = posBill.ServiceRate > 1 && posBill.ServiceRate <= 100 ? posBill.ServiceRate / 100 : posBill.ServiceRate;
                            posBill.Discount = posBill.Discount > 1 && posBill.Discount <= 100 ? posBill.Discount / 100 : posBill.Discount;
                            posBill.OpenWxid = openid;

                            billService.Add(posBill);
                            //billService.AddDataChangeLog(OpLogType.Pos账单增加);
                            billService.Commit();
                            AddOperationLog(OpLogType.Pos账单增加, "单号：" + posBill.Billid + ",台号：" + posBill.Tabid + "，状态：" + Enum.GetName(typeof(PosBillStatus), posBill.Status), posBill.BillNo);

                        }



                        #region 抹台的餐台号 已注释

                        //判断是否存在已经开台的账单
                        //string SmearTabNo = "";
                        //var billSmearList = billService.GetSmearList(BaseHid, tabid, pos.Business);
                        //if (billSmearList != null && billSmearList.Count > 0)
                        //{
                        //    //存在已经未买单的账单
                        //    posBill = billService.GetLastBillId(BaseHid, tab.Refeid, pos.Business);
                        //    var searList = billService.GetSmearList(BaseHid, tabid, pos.Business.Value.Date);
                        //    if (searList != null && searList.Count > 0)
                        //    {
                        //        List<string> tabNoList = new List<string>();
                        //        foreach (var tabNo in searList)
                        //        {
                        //            var Letter = tabNo.TabNo.Substring(tabNo.TabNo.Length - 1, 1);
                        //            //不是数字添加进数组
                        //            if (!CheckLetter(Letter))
                        //            {
                        //                tabNoList.Add(Letter);
                        //            }
                        //        }
                        //        string max;
                        //        if (tabNoList.Count <= 0)
                        //        {
                        //            max = "A";
                        //        }
                        //        else
                        //        {
                        //            max = tabNoList.Max();
                        //            max = Convert.ToChar(Convert.ToInt16(max.ToCharArray()[0]) + 1).ToString();
                        //            if (max == "Z")
                        //            {
                        //                //如果为Z暂补考虑
                        //            }
                        //        }
                        //        SmearTabNo = max;
                        //    }

                        #endregion

                        if (tabType.WxPaytype == 2) //如果是餐后支付。锁台。添加锁台记录
                        {
                            PosTabStatus tabStatus = new PosTabStatus() { Hid = BaseHid, GuestName = "扫码点餐", Tabid = BaseTabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = posBill.Billid, OpenRecord = DateTime.Now, OpenGuest = posBill.IGuest };
                            updateTabStatus(tabStatus);

                            var tabStatusService = new PosTabStatusService(hotelDb); 
                            tabStatusService.SetTabStatus(BaseHid, refe.Id, (byte)PosTabStatusOpType.初始化, "", "", "");

                        }

                        AddBillDetail(hotelDb, posBill, billDetailList, billDetailActionList);

                        billDetailService.CmpBillAmountByScanOrder(BaseHid, billId);

                        #region 增加手机点菜记录

                        var morderListService = new PosMorderListService(hotelDb);
                        var morderList = new PosMorderList
                        {
                            Id = Guid.NewGuid(),
                            Hid = BaseHid,
                            Billid = posBill.Billid,
                            OrderType = PosOrderType.Mobile.ToString(),
                            OrderPara = "账单：" + JsonConvert.SerializeObject(posBill) + "账单明细：" + allList.billDetailList + "作法：" + allList.ActionList,
                            IStatus = (byte)PosMorderListIStatus.落单,
                            Wxid = BaseOpenid,
                            Remark = "",
                            Creator = "扫码点餐",
                            Createdate = DateTime.Now,
                            IWaiter = false,
                        };
                        morderListService.Add(morderList);
                        morderListService.AddDataChangeLog(OpLogType.Pos手机点菜记录添加);
                        morderListService.Commit();

                        #endregion 增加手机点菜记录


                        var result = new { Billid = posBill.Billid, wxPaytype = tabType.WxPaytype.ToString() };

                        return Json(JsonResultData.Successed(result));
                    }
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }

            return Json(JsonResultData.Failure("订单提交失败"));
        }

        /// <summary>
        /// 处理账单明细与账单明细作法
        /// </summary>
        /// <param name="hotelDb"></param>
        /// <param name="posBill"></param>
        /// <param name="billDetailList"></param>
        /// <param name="billDetailActionList"></param>
        private void AddBillDetail(DbHotelPmsContext hotelDb, PosBill posBill, List<ScanPosBillDetail> billDetailList, List<ScanPosBillDetailAction> billDetailActionList)
        {
            var itemService = new PosItemService(hotelDb);
            var billDetailService = new PosBillDetailService(hotelDb);

            var billDetailActionService = new PosBillDetailActionService(hotelDb);



            var tabService = new PosTabService(hotelDb);
            var tab = tabService.Get(posBill.Tabid);

            var tabTypeService = new PosTabtypeService(hotelDb);

            var tabType = tabTypeService.Get(tab.TabTypeid);

            if (posBill.Status == (byte)PosBillStatus.扫码点餐默认状态)
            {
                //删除作法明细
                var dbActionList = billDetailActionService.GetPosBillDetailActionByModule(BaseHid, posBill.Billid);
                foreach (var item in dbActionList)
                {
                    billDetailActionService.Delete(item);
                    billDetailActionService.Commit();

                }

                //删除账单明细
                var dbDetailList = billDetailService.GetBillDetailByDcFlag(BaseHid, posBill.Billid);
                foreach (var item in dbDetailList)
                {
                    billDetailService.Delete(item);
                    billDetailService.Commit();

                }
            }
            else if (posBill.Status == (byte)PosBillStatus.结账 && tabType.WxPaytype == 1)
            {
                //删除作法明细
                //var dbActionList = billDetailActionService.GetPosBillDetailActionByModule(BaseHid, posBill.Billid);
                //foreach (var item in dbActionList)
                //{
                //    billDetailActionService.Delete(item);
                //    billDetailActionService.Commit();

                //}

                //删除账单明细
                var dbDetailList = billDetailService.GetBillDetailByDcFlag(BaseHid, posBill.Billid).Where(w => w.IsCheck != true).ToList();
                foreach (var item in dbDetailList)
                {
                    var dbActionList = billDetailActionService.GetPosBillDetailActionByModule(BaseHid, posBill.Billid).Where(w => w.Mid == item.Id).ToList();
                    foreach (var action in dbActionList)
                    {
                        billDetailActionService.Delete(action);
                        billDetailActionService.Commit();
                    }
                    billDetailService.Delete(item);
                    billDetailService.Commit();

                }
            }


            foreach (ScanPosBillDetail ScanbillDetail in billDetailList)
            {
                var item = itemService.GetEntity(BaseHid, ScanbillDetail.Itemid);
                if (item != null)
                {
                    //bool isExists = billDetailService.IsExistsForLD(BaseHid, bill.Billid, ScanbillDetail.Itemid);

                    #region 添加账单明细
                    var dueamount = ScanbillDetail.Price * ScanbillDetail.Quantity;
                    var amount = item.IsDiscount ?? true ? dueamount * posBill.Discount ?? 1 : dueamount;
                    var serviceAmount = posBill.IsService ?? true ? dueamount * posBill.ServiceRate ?? 1 : 0;

                    PosBillDetail billDetail = new PosBillDetail();

                    CopyModel(billDetail, ScanbillDetail);

                    billDetail.Hid = BaseHid;
                    billDetail.Billid = posBill.Billid;
                    billDetail.MBillid = posBill.MBillid ?? posBill.Billid;
                    billDetail.DcFlag = PosItemDcFlag.D.ToString();
                    billDetail.IsCheck = false;
                    billDetail.Isauto = (byte)PosBillDetailIsauto.录入项目;
                    billDetail.Status = (byte)PosBillDetailStatus.正常;
                    billDetail.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                    billDetail.Dueamount = dueamount;
                    billDetail.Discount = posBill.Discount;
                    billDetail.Amount = amount;
                    billDetail.Service = serviceAmount;
                    billDetail.SP = ScanbillDetail.SP == null ? false : ScanbillDetail.SP;
                    billDetail.SD = ScanbillDetail.SD == null ? false : ScanbillDetail.SD;

                    billDetail.TransUser = "扫码点餐";
                    billDetail.TransBsnsDate = posBill.BillBsnsDate;
                    billDetail.TransShiftid = posBill.Shiftid;
                    billDetail.TransShuffleid = posBill.Shuffleid;
                    billDetail.TransDate = DateTime.Now;
                    billDetail.OrderType = "Mobile";
                    billDetail.PriceOri = ScanbillDetail.Price;
                    billDetail.OpenWxid = posBill.OpenWxid;
                    billDetail.ItemSubid = ScanbillDetail.itemClassId;

                    billDetailService.Add(billDetail);

                    // billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);

                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "单号：" + billDetail.Id + ",台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额：" + billDetail.Dueamount + "，状态：" + Enum.GetName(typeof(PosBillDetailStatus), billDetail.Status), posBill.BillNo);
                    #endregion

                    var addAmount = 0.0M;

                    #region 添加账单明细作法


                    foreach (var action in billDetailActionList.Where(w => w.OrderId == ScanbillDetail.OrderId))
                    {
                        PosBillDetailAction detailAction = new PosBillDetailAction
                        {
                            Hid = BaseHid,
                            MBillid = posBill.Billid,
                            Mid = billDetail.Id,
                            Igroupid = action.Igroupid,
                            ActionNo = action.ActionNo,
                            ActionName = action.ActionName,
                            AddPrice = action.AddPrice,
                            Nmultiple = action.Nmultiple,
                            IByGuest = action.IByGuest,
                            IByQuan = action.IByQuan,
                            Quan = billDetail.Quantity,
                            Amount = billDetail.Quantity * action.AddPrice,
                            DeptClassid = action.DeptClassid,
                            PrtNo = action.PrtNo,
                            ModiUser = BaseOpenid,
                            ModiDate = DateTime.Now,
                            Memo = "扫码点餐"
                        };

                        billDetailActionService.Add(detailAction);
                        billDetailActionService.Commit();

                        if (action.IByQuan != null && action.IByQuan.Value && action.IByGuest != null && action.IByGuest.Value)
                        {
                            //数量相关最低数量大于点菜数量。作法加价不乘数量
                            if (action.LimitQuan != null && action.LimitQuan > 0 && ScanbillDetail.Quantity < action.LimitQuan)
                            {
                                addAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(posBill.IGuest);

                            }
                            else
                            {
                                addAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(ScanbillDetail.Quantity) * 1;
                            }

                        }
                        else if (action.IByQuan != null && action.IByGuest.Value)
                        {
                            //数量相关最低数量大于点菜数量。作法加价不乘数量
                            if (action.LimitQuan != null && action.LimitQuan > 0 && ScanbillDetail.Quantity < action.LimitQuan)
                            {
                                addAmount += 0;
                            }
                            else
                            {
                                addAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(ScanbillDetail.Quantity);
                            }
                        }
                        else if (action.IByGuest != null && action.IByGuest.Value)
                        {

                            addAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(posBill.IGuest);
                        }
                        else
                        {
                            addAmount += Convert.ToDecimal(action.AddPrice);
                        }

                    }
                    #endregion

                    billDetail.AddPrice = addAmount;
                    billDetailService.Update(billDetail, new PosBillDetail());
                    billDetailService.Commit();

                }
            }
        }



        /// <summary>
        /// 更新餐台状态表
        /// </summary>
        /// <param name="posTabStatus">餐台状态</param>
        public void updateTabStatus(PosTabStatus posTabStatus)
        {
            var hotelDb = GetHotelDb(posTabStatus.Hid);
            var tabStatusService = new PosTabStatusService(hotelDb);
            var tabStatus = tabStatusService.Get(posTabStatus.Tabid);
            tabStatus.TabStatus = posTabStatus.TabStatus;
            tabStatus.OpTabid = posTabStatus.OpTabid;
            tabStatus.OpenRecord = posTabStatus.OpenRecord;
            tabStatus.GuestName = posTabStatus.GuestName;
            tabStatus.OpenGuest = posTabStatus.OpenGuest == null ? 1 : posTabStatus.OpenGuest;
            tabStatusService.Update(tabStatus, new PosTabStatus());
            tabStatusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
            tabStatusService.Commit();
        }

        /// <summary>
        /// 获取账单付款金额合计
        /// </summary>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>

        public JsonResult GetPaymentTotal(string hid, string billid)
        {
            try
            {
                SetBaseInfo(hid);

                var hotelDb = GetHotelDb(BaseHid);
                var service = new PosBillDetailService(hotelDb);
                var paymentTotal = service.GetBillDetailForPaymentTotalByBillid(BaseHid, billid);
                return Json(JsonResultData.Successed(paymentTotal));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 微信买单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <param name="openid">微信openid</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        public JsonResult PayByWX(string hid, string tabid, string openid, string billid)
        {
            try
            {
                var hotelDb = GetHotelDb(BaseHid);
                //重算账单金额
                var service = new PosBillDetailService(hotelDb);



                #region 增加付款明细

                SetBaseInfo(hid, tabid, openid);
                Guid? settleid = Guid.NewGuid();

                var billService = new PosBillService(hotelDb);
                var bill = billService.Get(billid);

                var refeService = new PosRefeService(hotelDb);
                var refe = refeService.Get(bill.Refeid);

                var posService = new PosPosService(hotelDb);
                var pos = posService.Get(refe.PosId);

                var itemService = new PosItemService(hotelDb);
                var item = itemService.GetItem(BaseHid, "WxBarcode");

                var tabService = new PosTabService(hotelDb);
                var tab = tabService.Get(BaseTabid);

                var tabTypeService = new PosTabtypeService(hotelDb);
                var tabType = tabTypeService.Get(tab.TabTypeid);


                var billDetails = service.GetBillDetailByBillid(BaseHid, billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.正常);

                var settleTransno = Guid.NewGuid().ToString("N");
                PosBillDetail billDetail = new PosBillDetail()
                {
                    Hid = BaseHid,
                    Tabid = bill.Tabid,
                    Billid = bill.Billid,
                    MBillid = bill.MBillid,

                    Itemid = item.Id,
                    ItemCode = item.Code,
                    ItemName = "扫码点餐-微信支付",

                    DcFlag = PosItemDcFlag.C.ToString(),
                    Status = (byte)PosBillDetailStatus.作废,
                    Isauto = (byte)PosBillDetailIsauto.付款,
                    IsCheck = true,

                    Settleid = settleid,
                    SettleBsnsDate = pos.Business,
                    SettleShuffleid = refe.ShuffleId,
                    SettleShiftId = pos.ShiftId,
                    SettleDate = DateTime.Now,
                    SettleUser = "扫码点餐",
                    SettleTransno = settleTransno,
                    SettleTransName = "买单 - " + bill.Billid,
                    OrderType = "Mobile"
                };

                billDetail.Amount = service.GetBillDetailForPaymentTotalByBillid(BaseHid, billid).UnPaid;
                billDetail.Dueamount = service.GetBillDetailForPaymentTotalByBillid(BaseHid, billid).UnPaid;
                service.Add(billDetail);
                service.AddDataChangeLog(OpLogType.Pos账单付款明细增加);
                service.Commit();

                billDetail = service.GetBillDetailBySettleTransno(BaseHid, billid, settleTransno);

                #endregion 增加付款明细



                if (billDetail != null)
                {
                    //进行支付
                    var businessPara = new PaymentOperateBusinessPara
                    {
                        Hid = BaseHid,
                        UserName = "扫码点餐",
                        PosId = refe.PosId,
                        PosName = pos.Name,
                        Billid = bill.Billid,
                        Refeid = bill.Refeid,
                        Tabid = bill.Tabid,

                        //账单明细
                        IsCheck = 1,
                        SettleId = billDetail.Settleid,
                        SettleBsnsDate = pos.Business,
                        SettleShuffleid = refe.ShuffleId,
                        SettleShiftId = pos.ShiftId,
                        SettleUser = "扫码点餐",
                        SettleTransNo = settleTransno,
                        SettleTransName = "买单 - " + bill.Billid,

                        //餐台状态
                        TabStatus = (byte)PosTabStatusEnum.空净,
                        OpTabid = "",

                        //账单
                        DepBsnsDate = pos.Business,
                        MoveUser = "扫码点餐",
                        Status = (byte)PosBillStatus.清台,
                        WaitStatus = 1,
                        BillDetailStatus = (byte)PosBillDetailStatus.正常,
                        Isauto = (byte)PosBillDetailIsauto.付款
                    };

                    //增加一笔待支付记录，并且将待支付记录的id值做为外部业务单号
                    var serializer = new JavaScriptSerializer();
                    var resWaitPayParaStr = serializer.Serialize(businessPara);
                    var waitPay = new WaitPayList
                    {
                        Status = 0,
                        CreateDate = DateTime.Now,
                        ProductType = PayProductType.PosPayment.ToString(),
                        WaitPayId = Guid.Parse(settleTransno),
                        BusinessPara = resWaitPayParaStr
                    };
                    hotelDb.WaitPayLists.Add(waitPay);
                    hotelDb.SaveChanges();

                    //调用接口进行支付

                    // 1 获取所需参数               
                    var interfacePara = new GsWxInterfaceModel(hotelDb, hid);
                    if (interfacePara.IsEnable == true)
                    {
                        var comid = interfacePara.CompanyId;
                        var createOrderUrl = interfacePara.CreatePayOrderUrl;
                        var payUrl = interfacePara.PayOrderUrl;
                        if (string.IsNullOrEmpty(comid) || string.IsNullOrEmpty(createOrderUrl) || string.IsNullOrEmpty(payUrl))
                        {
                            return Json(JsonResultData.Failure("缺少必要支付参数"));
                        }

                        // string hid, string tabid, string billid, long billDetailId
                        // 2 组建相关参数预下单
                        var notifyUrl = Url.Encode(string.Format("http://" + Request.Url.Host.ToString() + "/ScanOrder/Order/PaymentSuccess?hid={0}&tabid={1}&billid={2}&billDetailId={3}&&openFlag={4}", hid, tabid, billid, billDetail.Id, tabType.WxPaytype));
                        var returnUrl = Url.Encode(string.Format("http://{0}/ScanOrder/Order/_OrderInfo?hid={1}&tabid={2}&openid={3}&billid={4}", Request.Url.Host.ToString(), hid, tabid, openid, billid));
                        // var returnUrl = Url.Encode(string.Format("http://{0}/ScanOrder/Order/_OrderInfo?hid={1}&tabid={2}&openid={3}&billid={4}", Request.Url.Host.ToString(), hid, tabid, openid, billid));
                        var hotelCode = hid;
                        var prem = new Dictionary<string, string>
                        {
                            { "name", "扫码点餐" },
                            { "businessId", billid },
                            { "totalFee", (decimal.Round(Convert.ToDecimal(billDetail.Amount) * 100)).ToString("0")},
                            { "orderSn", waitPay.WaitPayId.ToString().Replace("-","").ToUpper() },
                            { "hotelCode",hotelCode},
                            { "payMode", "PMS" },
                            { "notifyUrl", notifyUrl},
                            { "returnUrl", notifyUrl}
                        };
                        var sysLogService = GetService<ISysLogService>();

                        string res = HttpHelper.Post(createOrderUrl, prem);//{"status":"200","msg":"ok"}http://wxtest.gshis.net/pay/createPayOrder.do
                        if (!res.Contains("200"))
                        {
                            return Json(JsonResultData.Failure("创建订单失败：" + res));
                        }

                        SysLog sysLog = new SysLog() { Hid = hid, CDate = DateTime.Now, Info = "name=扫码点餐,businessId=" + billid + "totalFee=" + (decimal.Round(Convert.ToDecimal(billDetail.Amount) * 100)).ToString("0") + "orderSn=" + waitPay.WaitPayId.ToString().Replace("-", "").ToUpper() + "payMode=POS,notifyUrl=" + notifyUrl + ",returnUrl=" + returnUrl + ",hotelCode=" + hid, Url = "/ScanOrder/Order/PayByWX" };
                        //记录日志生成的参数
                        // var sysLogService = GetService<ISysLogService>();
                        sysLog = new SysLog() { Hid = hid, CDate = DateTime.Now, Info = "name=扫码点餐,businessId=" + billid + "totalFee=" + (decimal.Round(Convert.ToDecimal(billDetail.Amount) * 100)).ToString("0") + "orderSn=" + waitPay.WaitPayId.ToString().Replace("-", "").ToUpper() + "payMode=POS,notifyUrl=" + notifyUrl + ",returnUrl=" + returnUrl + ",hotelCode=" + hid, Url = "/ScanOrder/Order/PayByWX" };
                        sysLogService.Add(sysLog);
                        sysLogService.Commit();
                        //返回支付链接
                        //return Content(payUrl + "?comid=" + comid + "&orderSn=" + waitPay.WaitPayId.ToString().Replace("-", "").ToUpper());
                        return Json(JsonResultData.Successed(new { type = "1", Parameters = payUrl + "?comid=" + comid + "&orderSn=" + waitPay.WaitPayId.ToString().Replace("-", "").ToUpper(), billDetailId = billDetail.Id }), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {



                        //取出支付参数
                        var logService = DependencyResolver.Current.GetService<IPayLogService>();
                        var pmsParas = hotelDb.PmsParas.Where(c => c.Hid == BaseHid).ToList();
                        //var sysParas = hotelDb.M_v_payParas.ToList();
                        var payMode = pmsParas.SingleOrDefault(w => w.Code == "l_wx_payMode");

                        WxPayData payResult;
                        string wxSignKey;

                        var notifyUrl = Url.Action("PaymentSuccess", "Order");
                        notifyUrl = Url.GetAbsoulteUrlForSpecialUrl(notifyUrl);
                        var attach = new AttchModel { Hid = BaseHid, Tabid = BaseTabid, Billid = billid, BillDetailId = billDetail.Id };
                        //没有系统参数或者系统参数的值为服务商支付
                        if (payMode == null || payMode.Value == "1")
                        {
                            //var payServiceBuilder = new PayServiceBuilder(hotelDb);
                            var commonDb = GetService<DbCommonContext>();
                            var pmsParaService = new PmsParaService(hotelDb);

                            var commonPayParas = commonDb.M_v_payParas.ToList();
                            var hotelPayParas = pmsParaService.GetPmsParas(BaseHid);

                            var payPara = PayServiceBuilder.GetWxPayConfigPara(commonPayParas, hotelPayParas, "");

                            //构建支付信息,统一下单
                            var data = new WxPayData();
                            data.SetValue("appid", payPara.WxProviderAppId);
                            data.SetValue("mch_id", payPara.WxProviderMchId);
                            data.SetValue("body", billDetail.SettleTransName);
                            data.SetValue("attach", JsonConvert.SerializeObject(attach));
                            data.SetValue("out_trade_no", waitPay.WaitPayId.ToString("N"));
                            data.SetValue("total_fee", ((decimal)(billDetail.Amount * 100)).ToString("0"));
                            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
                            data.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss"));
                            //data.SetValue("goods_tag", "test");
                            data.SetValue("trade_type", "JSAPI");
                            data.SetValue("sub_openid", openid);
                            wxSignKey = payPara.WxProviderKey;
                            payResult = WxPayApi.UnifiedOrder(data, payPara, logService, BaseHid, notifyUrl);
                        }
                        else
                        {
                            var commonDb = GetService<DbCommonContext>();
                            var pmsParaService = new PmsParaService(hotelDb);

                            var commonPayParas = commonDb.M_v_payParas.ToList();
                            var hotelPayParas = pmsParaService.GetPmsParas(BaseHid);
                            //普通商户模式支付
                            var payPara = PayServiceBuilder.GetWxPayConfigPara(commonPayParas, hotelPayParas, "");
                            //构建支付信息,统一下单
                            var data = new WxPayData();
                            data.SetValue("body", billDetail.SettleTransName);
                            data.SetValue("attach", JsonConvert.SerializeObject(attach));
                            data.SetValue("out_trade_no", waitPay.WaitPayId.ToString("N"));
                            data.SetValue("total_fee", ((decimal)(billDetail.Amount * 100)).ToString("0"));
                            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
                            data.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss"));
                            //data.SetValue("goods_tag", "test");
                            data.SetValue("trade_type", "JSAPI");
                            data.SetValue("openid", openid);
                            wxSignKey = payPara.WxProviderKey;
                            payPara.WxProviderNotifyUrl = notifyUrl;
                            payResult = WxPayApi.UnifiedOrder(data, payPara, logService, BaseHid, notifyUrl);
                        }

                        if (payResult.IsSet("appid") && payResult.IsSet("prepay_id") && payResult.GetValue("prepay_id").ToString() != "")
                        {
                            var jsApiParam = new WxPayData();
                            jsApiParam.SetValue("appId", payResult.GetValue("appid"));
                            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
                            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
                            jsApiParam.SetValue("package", "prepay_id=" + payResult.GetValue("prepay_id"));
                            jsApiParam.SetValue("signType", "MD5");
                            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(wxSignKey));

                            string parameters = jsApiParam.ToJson();
                            //WxPayPara = parameters;
                            return Json(JsonResultData.Successed(new { type = "2", Parameters = parameters, billDetailId = billDetail.Id }), JsonRequestBehavior.AllowGet);
                        }
                        return Json(JsonResultData.Failure("获取支付信息失败：" + payResult.ToPrintStr()), JsonRequestBehavior.AllowGet);


                    }



                }

                return Json(JsonResultData.Failure("订单创建失败"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新单位
        /// </summary>
        /// <param name="model">消费项目单位</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public JsonResult UpdateUnit(UnitModel model, string list)
        {
            try
            {
                list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
                var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

                if (model != null)
                {
                    foreach (var billDetail in billDetailList)
                    {
                        if (model.Itemid == billDetail.Itemid)
                        {
                            billDetail.Unitid = model.Unitid;
                            billDetail.UnitCode = model.UnitCode;
                            billDetail.UnitName = model.UnitName;
                            billDetail.Price = model.Price;
                            billDetail.Amount = model.Price * billDetail.Quantity;
                            break;
                        }
                    }
                }

                return Json(JsonConvert.SerializeObject(billDetailList));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 更新单位
        /// </summary>
        /// <param name="model">消费项目单位</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public JsonResult UpdateRequest(string hid, string requestid, string itemid, string list)
        {
            try
            {
                SetBaseInfo(hid);

                var hotelDb = GetHotelDb(BaseHid);

                var requestService = new PosRequestService(hotelDb);
                var request = requestService.Get(requestid);

                list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<PosBillDetail>()) : list;
                var billDetailList = JsonConvert.DeserializeObject<List<PosBillDetail>>(list);

                if (request.ITagOperator == (byte)PosRequestOperator.全单)
                {
                    if (billDetailList != null && billDetailList.Count > 0)
                    {
                        foreach (var billDetail in billDetailList)
                        {
                            var oldBillDetail = new PosBillDetail() { Request = billDetail.Request, IsProduce = billDetail.IsProduce };

                            billDetail.Request = request.Cname;
                        }
                    }
                }

                if (request.ITagOperator == (byte)PosRequestOperator.单道)
                {
                    foreach (var billDetail in billDetailList)
                    {
                        if (billDetail.Itemid == itemid)
                        {
                            billDetail.Request = request.Cname;
                        }
                    }
                }

                if (request.ITagOperator == (byte)PosRequestOperator.本单)
                {
                    if (billDetailList != null && billDetailList.Count > 0)
                    {
                        foreach (var billDetail in billDetailList)
                        {
                            var oldBillDetail = new PosBillDetail() { Request = billDetail.Request, IsProduce = billDetail.IsProduce };

                            billDetail.Request = request.Cname;
                        }
                    }
                }

                return Json(JsonResultData.Successed(billDetailList));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        /// <summary>
        /// 消费项目对应作法视图
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult _ItemActionList(string hid, string itemid)
        {
            SetBaseInfo(hid);

            if (!string.IsNullOrWhiteSpace(BaseHid) && !string.IsNullOrWhiteSpace(itemid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var itemActionService = new PosItemActionService(hotelDb);
                var list = itemActionService.GetPosItemActionListByItemId(BaseHid, itemid);
                return PartialView("_ItemActionList", list);
            }

            return PartialView("_ItemActionList", new List<up_pos_list_ItemActionByItemidResult>());
        }

        /// <summary>
        /// 作法分组视图
        /// </summary>
        /// <param name="model">消费项目</param>
        /// <param name="list">已点账单明细列表</param>
        /// <returns></returns>
        public ActionResult _ActionGroupList(string list)
        {
            list = string.IsNullOrWhiteSpace(list) ? JsonConvert.SerializeObject(new List<ActionGroup>()) : list;
            var actionGroups = JsonConvert.DeserializeObject<List<ActionGroup>>(list);
            return PartialView("_ActionGroupList", actionGroups);
        }

        /// <summary>
        /// 处理出品数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <returns></returns>

        public ActionResult CmpProductList(string hid, string billid, string billDetailId)
        {
            var hotelDb = GetHotelDb(BaseHid);
            //处理出品数据
            var service = new PosBillDetailService(hotelDb);
            service.CmpProducelist(BaseHid, billid);

            var model = service.Get(billDetailId);
            if (model != null)
            {
                model.Status = (byte)PosBillDetailStatus.正常;
                service.Update(model, new PosBillDetail());
                service.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 查询菜式界面
        /// </summary>
        /// <returns></returns>
        public ActionResult _QueryItemList()
        {
            return PartialView("_QueryItemList");
        }


        /// <summary>
        /// 查询结构界面
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public ActionResult _QueryItemResultList(string searchName)
        {
            var hotelDb = GetHotelDb(BaseHid);
            var service = new PosItemService(hotelDb);

            var tab = hotelDb.PosTabs.Where(m => m.Hid == BaseHid && m.Id == BaseTabid).FirstOrDefault();
            if (tab != null && !string.IsNullOrEmpty(searchName))
            {
                var SubClassIdList = service.GetScanPosSubClassList(BaseHid, tab.Refeid);
                string subClassId = "";
                for (int i = 0; i < SubClassIdList.Count; i++)
                {
                    subClassId += SubClassIdList[i].Id + ",";
                }

                var list = hotelDb.Database.SqlQuery<up_pos_scan_list_ItemListBySubClassidResult>("exec up_pos_scan_list_ItemListBySubClassid @hid = @hid,@subClassid = @subClassid"
                    , new SqlParameter("hid", BaseHid)
                    , new SqlParameter("subClassid", subClassId)).Where(m => m.Cname.Contains(searchName)).ToList();


                //hotelDb.PosItems.Where(m => m.Hid == BaseHid && m.IsSubClass == false
                //    && m.DcFlag == PosItemDcFlag.D.ToString() && arr.Contains(m.SubClassid) && m.Cname.Contains(searchName))
                //    .OrderBy(m => m.Seqid).ToList();

                return PartialView("_QueryItemResultList", list);
            }
            else
            {
                return PartialView("_QueryItemResultList");
            }

        }

        /// <summary>
        /// 订单详情界面
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public ActionResult _OrderInfo(string hid, string tabid, string openid, string billid)
        {
            ViewBag.Title = "订单";


            SetBaseInfo(hid, tabid, openid);

            var hotelDb = GetHotelDb(BaseHid);

            //获取酒店名称
            var hotelInfoService = GetService<IHotelInfoService>();
            ViewBag.Title = hotelInfoService.GetHotelShortName(hid) ?? "";


            //餐台
            var tabService = new PosTabService(hotelDb);
            var tab = tabService.Get(BaseTabid);

            //餐台类型
            var tabTypeSerice = new PosTabtypeService(hotelDb);
            // PosMorderList posMorderModel = null;
            var tabType = tabTypeSerice.Get(tab.TabTypeid);

            //账单信息
            var billService = new PosBillService(hotelDb);
            PosBill posBill = null;
            if (tabType.WxPaytype == 1)
            {
                posBill = billService.GetPosBillByScanOrder(hid, tabid, openid);
            }
            else
            {
                posBill = billService.GetPosbillByScanOrder(hid, tabid);
            }
            //营业点信息
            var refeService = new PosRefeService(hotelDb);
            var refe = refeService.Get(tab.Refeid);

            //收银点信息
            var posService = new PosPosService(hotelDb);
            var posModel = posService.Get(refe.PosId);

            var BillId = string.IsNullOrWhiteSpace(billid) ? posBill == null ? "" : posBill.Billid : billid;   //账单ID




            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;
            ViewBag.Openid = BaseOpenid;
            ViewBag.TabType = tabType.WxPaytype.ToString();

            if (string.IsNullOrEmpty(BillId))
            {
                if (posModel.IsBrushOrder == true)//开启扫码点餐
                {

                    return RedirectToAction("Index", "Order", new { hid = BaseHid, tabid = BaseTabid, openid = BaseOpenid, billId = BillId });
                }
                else
                {
                    //关闭状态
                    return RedirectToAction("_ErrHtml", "Order", new { hid = BaseHid, tabid = BaseTabid, openid = BaseOpenid, billId = BillId });
                }

            }
            else
            {
                //后付模式
                if (tabType.WxPaytype == 2)
                {
                    var billDetailService = new PosBillDetailService(hotelDb);
                    var posbillDetail = billDetailService.GetBillDetailByDcFlag(BaseHid, BillId, PosItemDcFlag.D.ToString()).Where(w => w.Amount > 0).ToList();
                    ViewBag.BillDetail = posbillDetail;

                    var bill = billService.GetEntity(BaseHid, BillId);
                    ViewBag.Bill = bill;
                    decimal? amount = 0.0M;
                    foreach (var item in posbillDetail)
                    {
                        amount += item.Amount == null ? 0 : item.Amount;
                    }
                    ViewBag.PayAmt = amount;
                    ViewBag.BillId = bill.Billid;
                    return View();
                }
                else
                {
                    //先付模式
                    if (posModel.IsBrushOrder == true)//开启扫码点餐
                    {
                        var billDetailService = new PosBillDetailService(hotelDb);
                        var posbillDetail = billDetailService.GetBillDetailByDcFlag(BaseHid, BillId, PosItemDcFlag.D.ToString()).Where(w => w.Amount > 0 && w.IsCheck == true).ToList();
                        ViewBag.BillDetail = posbillDetail;

                        var bill = billService.GetEntity(BaseHid, BillId);
                        ViewBag.Bill = bill;
                        decimal? amount = 0.0M;
                        foreach (var item in posbillDetail)
                        {
                            amount += item.Amount == null ? 0 : item.Amount;
                        }
                        ViewBag.PayAmt = amount;
                        ViewBag.BillId = bill.Billid;
                        return View();
                        //return RedirectToAction("Index", "Order", new { hid = BaseHid, tabid = BaseTabid, openid = BaseOpenid, billId = BillId });
                    }
                    else
                    {
                        //关闭状态
                        return RedirectToAction("_ErrHtml", "Order", new { hid = BaseHid, tabid = BaseTabid, openid = BaseOpenid, billId = BillId });
                    }
                }

            }
        }

        /// <summary>
        /// 获取账单信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billId"></param>
        /// <returns></returns>

        public ActionResult GetBillDetailList(string hid, string billId)
        {
            var hotelDb = GetHotelDb(BaseHid);
            List<ScanPosBillDetail> ScanbillDetailList = new List<ScanPosBillDetail>();
            List<ScanPosBillDetailAction> billDetailActionList = new List<ScanPosBillDetailAction>();

            if (!string.IsNullOrEmpty(billId))
            {
                var billService = new PosBillService(hotelDb);
                var posbill = billService.GetEntity(BaseHid, billId);
                if (posbill.Status == (byte)PosBillStatus.扫码点餐默认状态)
                {
                    var billDetailService = new PosBillDetailService(hotelDb);
                    var billDetailList = billDetailService.GetBillDetailByDcFlag(BaseHid, billId, PosItemDcFlag.D.ToString()).ToList();

                    var actionService = new PosBillDetailActionService(hotelDb);


                    int i = 1;
                    foreach (var item in billDetailList)
                    {
                        var scabBillDetail = new ScanPosBillDetail();
                        CopyModel(scabBillDetail, item);
                        scabBillDetail.OrderId = i;
                        //   scabBillDetail.itemClassId=
                        ScanbillDetailList.Add(scabBillDetail);
                        var actionList = actionService.GetPosBillDetailActionByModule(BaseHid, billId).Where(w => w.Mid == item.Id);
                        foreach (var action in actionList)
                        {
                            var scanAction = new ScanPosBillDetailAction();
                            CopyModel(scanAction, action);
                            scanAction.OrderId = i;
                            billDetailActionList.Add(scanAction);
                        }
                        i++;
                    }

                }
            }
            return Json(JsonResultData.Successed(new { DetailList = ScanbillDetailList, ActionList = billDetailActionList/*, GroupList = actionGroups */}));
        }

        /// <summary>
        /// 扫码点餐关闭的提示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult _ErrHtml(string hid, string tabid, string openid, string billId)
        {
            ViewBag.Title = "提示";
            SetBaseInfo(hid, tabid, openid);
            var hotelDb = GetHotelDb(BaseHid);
            //banner
            var bannerService = new PosMBannerService(hotelDb);
            var banners = bannerService.GetMBannerList(BaseHid);

            ViewBag.BannerList = banners;
            return View();
        }

    }
}