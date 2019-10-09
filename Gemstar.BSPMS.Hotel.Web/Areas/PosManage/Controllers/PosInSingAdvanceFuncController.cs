using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Newtonsoft.Json;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 收银界面折扣功能权限控制
    /// </summary>

    [AuthPage(ProductType.Pos, "p600002")]
    public class PosInSingAdvanceFuncController : BaseController
    {

        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            return View();
        }

        #region 修改单价

        /// <summary>
        /// 修改单价界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult _PriceNumber(string id, string keyID, string BillId)
        {
            ViewBag.Id = id;

            //改项目的营业点，是否可调整价格为否的时候，也不能修改单价
            var billService = GetService<IPosBillService>();


            var model = billService.Get(BillId);
            if (model != null)
            {
                var posRefeService = GetService<IPosRefeService>();

                var posRefe = posRefeService.Get(model.Refeid);//获取账单营业点ID
                if (posRefe.ITagModifyCurrent == false)
                {
                    return Json(JsonResultData.Failure("该营业厅的不可调整价格，请确认"));
                }
            }

            string itemID = keyID;
            var posITemService = GetService<IPosItemService>();
            //判断该 消费项目实体是否为客修改单价
            var item = posITemService.Get(itemID);
            if (item != null)
            {
                if (item.IsModiPrice != null)
                {
                    bool isModiPrice = (bool)item.IsModiPrice;
                    if (isModiPrice == false)
                    {
                        return Json(JsonResultData.Failure("此消费项目不可修改单价，请确认"));
                    }
                }
            }
            return PartialView("_PriceNumber");
        }

        /// <summary>
        /// 修改单价
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdatePriceNumber(string Id, decimal Price)
        {
            try
            {
                var billDetailService = GetService<IPosBillDetailService>();
                var oldEntity = billDetailService.Get(int.Parse(Id));
                if (oldEntity == null)
                {
                    return Json(JsonResultData.Failure("要修改的消费项目不存在，请重新操作"));
                }
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(oldEntity.Billid);


                var newEntity = new PosBillDetail();
                AutoSetValueHelper.SetValues(oldEntity, newEntity);

                newEntity.Price = Price;

                //修改单价要修改原价
                newEntity.PriceOri = Price;

                newEntity.Dueamount = Price * newEntity.Quantity;
                billDetailService.Update(newEntity, oldEntity);
                billDetailService.AddDataChangeLog(OpLogType.Pos账单修改);
                billDetailService.Commit();
                AddOperationLog(OpLogType.Pos账单消费明细修改, "修改单价：" + oldEntity.Price + "-->" + newEntity.Price + "，金额：" + oldEntity.Dueamount + "-->" + newEntity.Dueamount, bill.BillNo);

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Successed(ex.Message.ToString()));
            }
        }

        #endregion 

        #region 取消打单

        [AuthButton(AuthFlag.Query)]
        public ActionResult CancelPrint(string billId)
        {
            var service = GetService<IPosBillService>();
            var oldEntity = service.Get(billId);
            if (oldEntity == null)
            {
                return Json(JsonResultData.Failure("要操作的数据不存在！"));
            }
            var newEntity = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);
            newEntity.IPrint = 0;   //打单次数修改成0
            service.Update(newEntity, oldEntity);
            service.AddDataChangeLog(OpLogType.Pos账单修改);
            AddOperationLog(OpLogType.Pos账单修改, "取消打单：" + oldEntity.IPrint + "-->" + newEntity.IPrint, newEntity.BillNo);
            service.Commit();

            return Json(JsonResultData.Successed());
        }

        #endregion 取消打单

        #region 手工清台

        [AuthButton(AuthFlag.Update)]
        public ActionResult HandClearTable(string billId)
        {
            var service = GetService<IPosBillService>();
            var oldEntity = service.Get(billId);
            if (oldEntity == null)
            {
                return Json(JsonResultData.Failure("要操作的数据不存在"));
            }
            var billDetailService = GetService<IPosBillDetailService>();

            //获取付款金额与已付金额
            var billDetailList = billDetailService.GetBillDetailForPaymentTotalByBillid(CurrentInfo.HotelId, billId);
            if (billDetailList.Paid != billDetailList.Total)
            {
                return Json(JsonResultData.Failure("账单未全部买单！"));
            }

            //获取收银点
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);
            //源数据赋值到新的实体
            var newEntity = new PosBill();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);



            //修改账单状态
            if (oldEntity.TabFlag == (byte)PosBillTabFlag.物理台)
            {
                newEntity.DepBsnsDate = pos.Business;
                newEntity.MoveUser = CurrentInfo.UserName;
                newEntity.DepDate = DateTime.Now;
                newEntity.Status = (byte)PosBillStatus.清台;

                service.Update(newEntity, oldEntity);
                // service.AddDataChangeLog(OpLogType.Pos账单修改);
                service.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "手工清台：" + oldEntity.Status + "-->" + newEntity.Status, newEntity.BillNo);
            }

            var statusService = GetService<IPosTabStatusService>();
            //修改餐台状态
            PosCommon common = new PosCommon();
            common.SetTabStatus(oldEntity);


            //清理锁台记录
            var tabLogService = GetService<IPosTabLogService>();
            var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, newEntity.Refeid, newEntity.Tabid, newEntity.TabNo);
            //if (tabLogList != null && tabLogList.Count > 0)
            //{
            foreach (var tabLog in tabLogList)
            {
                if (tabLog.Billid == oldEntity.Billid)
                {
                    tabLogService.Delete(tabLog);
                    tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                    tabLogService.Commit();
                }
            }
            //}
            return Json(JsonResultData.Successed());
        }

        #endregion 手工清台

        #region 取消服务费

        /// <summary>
        /// 取消服务费
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Export)]
        public ActionResult CancelServiceRate(string billId)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId); //获取对象
                                            // bill.ServiceRate = 0;

            var oldBill = new PosBill();
            AutoSetValueHelper.SetValues(bill, oldBill);
            bill.IsService = false;
            bill.ServiceRate = 0;
            service.Update(bill, new PosBill());
            service.AddDataChangeLog(OpLogType.Pos账单修改);
            service.Commit();
            AddOperationLog(OpLogType.Pos账单修改, "免服务费：" + oldBill.IsService + "-->" + bill.IsService, bill.BillNo);

            return Json(JsonResultData.Successed());
        }

        #endregion 取消服务费

        #region 取消折扣


        #region 取消折扣验证

        [AuthButton(AuthFlag.CancelOrderDetailY)]
        public ActionResult CheckEditForBillRefe(string BillId)
        {
            var billService = GetService<IPosBillService>();
            var model = billService.Get(BillId);
            if (model != null)
            {
                var posRefeService = GetService<IPosRefeService>();

                var posRefe = posRefeService.Get(model.Refeid);//获取账单营业点ID

                if (posRefe.IsPrintEdit == false && (model.IPrint != null && model.IPrint > 0))
                {
                    return Json(JsonResultData.Failure("打单之后不可以修改！"));
                }

                return Json(JsonResultData.Successed(""));
            }
            else
            {
                return Json(JsonResultData.Failure("账单不存在，请重试"));
            }
        }

        #endregion 取消折扣验证


        /// <summary>
        /// 取消折扣
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CancelDiscount(string billId)
        {
            PosCommon posCommon = new PosCommon();
            var res = posCommon.CancelDisCount(billId);
            return Json(res);
        }
        private ActionResult PriCancelDiscount(string billId)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.GetEntity(CurrentInfo.HotelId, billId);

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                    {
                        return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
                    }
                }

                var oldDiscount = bill.Discount;
                var oldDiscAmount = bill.DiscAmount;

                bill.DiscAmount = 0;    //折扣金额
                bill.Discount = 1;  //折扣率
                bill.DaType = null;
                bill.IsForce = null;
                bill.Profileid = null;
                bill.CardNo = null;
                billService.Update(bill, null);
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "折扣率：" + oldDiscount + "-->" + bill.Discount + "，折扣金额：" + oldDiscAmount + "-->" + bill.DiscAmount, bill.BillNo);

                var billDetailService = GetService<IPosBillDetailService>();
                var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId, "D");
                foreach (var billDetail in billDetailList)
                {
                    var newBillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, newBillDetail);
                    newBillDetail.Price = billDetail.PriceOri != null ? billDetail.PriceOri : billDetail.Price;  //价格恢复为原价
                    newBillDetail.Discount = 1;    //折扣率
                    newBillDetail.DiscAmount = 0;  //折扣金额
                    newBillDetail.IsDishDisc = null;
                    billDetailService.Update(newBillDetail, billDetail);
                    //    billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单修改, "单号：" + newBillDetail.Id + "，折扣率：" + billDetail.Discount + "-->" + newBillDetail.Discount + "，折扣金额：" + billDetail.DiscAmount + "-->" + newBillDetail.DiscAmount, bill.BillNo);

                }
                billDetailService.UpdateBillDetailDisc(CurrentInfo.HotelId, billId, billId);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            return Json(JsonResultData.Successed());
        }




        #endregion 取消折扣

        #region 修改服务费

        /// <summary>
        /// 视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Print)]
        public PartialViewResult _NumberInputByServiceRate(LetterInputViewModel model)
        {
            ViewBag.Version = CurrentVersion;
            return PartialView("_NumberInput", model);
        }

        /// <summary>
        /// 修改服务费率
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="ServiceRate">服务费率</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateServiceRate(string billId, string ServiceRate)
        {
            var service = GetService<IPosBillService>();
            var bill = service.Get(billId);
            if (bill != null)
            {

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                    {
                        return Json(JsonResultData.Failure("结账，清台的账单不能修改服务费"));
                    }
                }

                try
                {
                    var newBillModel = new PosBill();
                    AutoSetValueHelper.SetValues(bill, newBillModel);
                    decimal serviceRate = decimal.Parse(ServiceRate);
                    //统一转换百分比的小数
                    serviceRate = serviceRate > 1 ? serviceRate / 100 : serviceRate;
                    newBillModel.ServiceRate = serviceRate;
                    newBillModel.IsService = true;
                    service.Update(newBillModel, bill);
                    //service.AddDataChangeLog(OpLogType.Pos账单修改);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单修改, "服务费率：" + bill.ServiceRate + "-->" + newBillModel.ServiceRate, bill.BillNo);

                    return Json(JsonResultData.Successed());
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }

            }

            return Json(JsonResultData.Failure("操作出错"));
        }

        #endregion 修改服务费

        #region 并台

        [AuthButton(AuthFlag.Details)]
        public ActionResult _MergeTab(string Flag, string billDetailId)
        {
            int PageIndex = 1;
            int PageSize = 22;
            var service = GetService<IPosTabStatusService>();

            ViewBag.PageIndex = PageIndex;
            ViewBag.PageSize = PageSize;

            ViewBag.Flag = Flag;
            ViewBag.billDetailId = billDetailId;

            var Service = GetService<IPosRefeService>();
            var refeList = Service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

            return PartialView("_MergeTab", refeList);
        }

        /// <summary>
        /// 餐台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _TabStatusListByMergeTab(TabStatusViewModel model)
        {
            if (model != null)
            {
                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosTabStatusService>();
                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null, model.PageIndex, model.PageSize);

                return PartialView("_TabStatusListByMergeTab", list);
            }
            return PartialView("_TabStatusListByMergeTab");
        }

        /// <summary>
        /// 选择餐台类型视图
        /// </summary>
        /// 账单json 字符串
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosSmearList(string model)
        {
            return PartialView("_PosSmearList", JsonConvert.DeserializeObject<List<PosBill>>(model));
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult MergeTab(string billId, string newBillId, string newTabId)
        {
            var tabService = GetService<IPosTabService>();

            var billService = GetService<IPosBillService>();

            var billDetailService = GetService<IPosBillDetailService>();

            //源数据
            var billdetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId);
            var oldBill = billService.Get(billId);
            //目标数据
            PosBill bill = null;
            if (billdetailList == null || billdetailList.Count <= 0)
            {
                return Json(JsonResultData.Failure("没有要操作的数据！"));
            }

            if (string.IsNullOrEmpty(newBillId))
            {
                //没有抹台数据，用台号查询账单并台
                var tab = tabService.Get(newTabId);

                //to并台的数据
                var billResult = billService.GetPosBillByTabId(CurrentInfo.HotelId, tab.Refeid, newTabId);

                bill = billService.Get(billResult.Billid);
                // AutoSetValueHelper.SetValues(billResult, bill);
            }
            else
            {
                //有抹台数据用账单ID 并台
                bill = billService.Get(newBillId);
            }
            try
            {
                decimal amount = 0;
                var billRow = "";
                //循环修改账单明细数据主账单ID
                foreach (var billDetail in billdetailList)
                {
                    if (billDetail.Status == (byte)PosBillDetailStatus.正常 || billDetail.Status == (byte)PosBillDetailStatus.保存)
                    {
                        //计算本次转的金额
                        amount += billDetail.Amount == null ? 0 : Convert.ToDecimal(billDetail.Amount);
                    }
                    billRow += billDetail.Id + ",";
                    //源数据的账单ID 改成目标数据的账单ID
                    billDetail.Billid = bill.Billid;
                    billDetail.MBillid = bill.Billid;
                    billDetail.Memo = CurrentInfo.UserName + "并台";
                    billDetailService.Update(billDetail, new PosBillDetail());
                    billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();

                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，账单ID：" + oldBill.Billid + "-->" + bill.Billid, oldBill.BillNo);
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "并台：" + billRow, bill.BillNo);
                }

                #region 操作账单记录

                var billChange = new PosBillChange
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    BillBsnsDate = bill.BillBsnsDate,
                    Refeid = bill.Refeid,
                    Tabid = oldBill.Tabid,
                    MBillid = oldBill.MBillid,
                    NmBillid = bill.MBillid,
                    Nrefeid = bill.Refeid,
                    Ntabid = bill.Tabid,
                    iStatus = (byte)PosBillChangeStatus.并台,
                    Amount = amount,
                    BillRow = billRow,
                    Module = "CY",
                    TransUser = CurrentInfo.UserName,
                    CreateDate = DateTime.Now
                };

                var billChangeService = GetService<IPosBillChangeService>();
                billChangeService.Add(billChange);
                billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                billChangeService.Commit();

                #endregion 操作账单记录

                //修改主账单状态
                var oldStatus = oldBill.Status;

                oldBill.Status = (byte)PosBillStatus.取消;
                oldBill.MoveUser = CurrentInfo.UserName;

                billService.Update(oldBill, new PosBill());
                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "并台状态：" + oldStatus + "-->" + oldBill.Status, oldBill.BillNo);

                var iguest = bill.IGuest;
                bill.IGuest += oldBill.IGuest;
                billService.Update(bill, new PosBill());
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "并台人数：" + iguest + "-->" + bill.IGuest, bill.BillNo);

                var statusService = GetService<IPosTabStatusService>();
                var tab = statusService.Get(bill.Tabid);
                tab.OpenGuest += oldBill.IGuest;
                statusService.Update(tab, new PosTabStatus());
                statusService.AddDataChangeLog(OpLogType.Pos餐台状态修改);
                statusService.Commit();
                //修改餐台状态
                if (oldBill.Tabid != newTabId && !string.IsNullOrEmpty(newTabId))
                {
                    PosCommon common = new PosCommon();
                    common.SetTabStatus(oldBill);
                }

                //清理锁台记录
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, oldBill.Refeid, oldBill.Tabid, oldBill.TabNo);
                if (tabLogList != null && tabLogList.Count > 0)
                {
                    foreach (var tabLog in tabLogList)
                    {
                        if (tabLog.Billid == oldBill.Billid)
                        {
                            tabLogService.Delete(tabLog);
                            tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                            tabLogService.Commit();
                        }
                    }
                }
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        #endregion 并台

        #region 整单取消

        /// <summary>
        /// 整单取消
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Members)]
        public PartialViewResult _ReasonListByAll(PosReasonViewModel model)
        {
            if (model != null)
            {
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;

                var service = GetService<IPosReasonService>();
                var list = service.GetPosReasonByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.Istagtype, model.PageIndex, model.PageSize);

                ViewBag.pageIndex = model.PageIndex;
                return PartialView("_ReasonListByAll", list);
            }
            return PartialView("_ReasonListByAll", new List<PosReason>());
        }

        /// <summary>
        /// 整单取消
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <param name="canReason">取消原因</param>
        /// <param name="isreuse">是否加回库存</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CancelAllItemByNum(string billId, string canReason, string isreuse)
        {
            //账单服务
            var billService = GetService<IPosBillService>();

            //账单明细服务
            var billDetailService = GetService<IPosBillDetailService>();

            //账单
            var bill = billService.Get(billId);
            var oldBill = new PosBill();
            AutoSetValueHelper.SetValues(bill, oldBill);

            //判断是否有付款信息
            var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, billId);
            if (billDetailList != null || billDetailList.Count > 0)
            {
                //判断是否有付款信息
                var billDetailListByC = billDetailList.Where(m => m.DcFlag == "C").ToList();
                if (billDetailListByC.Count > 0)
                {
                    return Json(JsonResultData.Failure("已存在付款信息，不能整单取消"));
                }
            }
            try
            {
                //修改账单明细状态
                foreach (var billDetail in billDetailList)
                {
                    var oldbillDetail = new PosBillDetail();
                    AutoSetValueHelper.SetValues(billDetail, oldbillDetail);
                    if (billDetail.Status == (byte)PosBillDetailStatus.保存)
                    {
                        billDetail.Status = (byte)PosBillDetailStatus.未落单取消;
                    }
                    else
                    {
                        if (Convert.ToBoolean(isreuse))
                        {
                            billDetail.Status = (byte)PosBillDetailStatus.加回库存取消;
                        }
                        else if (Convert.ToBoolean(isreuse) == false)
                        {
                            billDetail.Status = (byte)PosBillDetailStatus.不加回库取消;
                        }
                    }

                    billDetailService.Update(billDetail, new PosBillDetail());
                    // billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    billDetailService.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，状态：" + oldbillDetail.Status + "-->" + billDetail.Status, bill.BillNo);
                }
                //修改账单状态
                bill.Memo = canReason;
                bill.Status = (byte)PosBillStatus.取消;
                billService.Update(bill, new PosBill());

                //点餐处理出品记录
                billDetailService.CmpProducelist(CurrentInfo.HotelId, billId);

                //billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();
                AddOperationLog(OpLogType.Pos账单修改, "状态：" + oldBill.Status + "-->" + bill.Status, bill.BillNo);

                var statusService = GetService<IPosTabStatusService>();
                //修改餐台状态
                PosCommon common = new PosCommon();
                common.SetTabStatus(bill);

                //清理锁台记录
                var tabLogService = GetService<IPosTabLogService>();
                var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, bill.Refeid, bill.Tabid, bill.TabNo);
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

                //打印取消单
                var orderlist = new List<up_pos_print_billOrderResult>();
                var refe = Session["PosRefe"] as PosRefe;
                if (refe.IPrintBillss == PosPrintBillss.打印)
                {
                    orderlist = billDetailService.GetBillOrderByPrint(CurrentInfo.HotelId, bill.Billid, bill.MBillid).Where(w => w.计费状态 > 50 && w.计费状态 < (byte)PosBillDetailStatus.未落单取消 && Convert.ToInt32(w.点菜单打单次数) == 1).ToList();
                    var list = billDetailService.GetBillDetailByBillidAndStatus(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString(), (byte)PosBillDetailStatus.未落单取消);
                    foreach (var temp in list)
                    {
                        foreach (var order in orderlist)
                        {
                            if (temp.Id == order.id)
                            {
                                var billDetailNew = new PosBillDetail();
                                AutoSetValueHelper.SetValues(temp, billDetailNew);
                                byte iOrderPrint = temp.iOrderPrint ?? 0;
                                billDetailNew.iOrderPrint = Convert.ToByte(iOrderPrint + 1);
                                billDetailService.Update(billDetailNew, new PosBillDetail());
                                billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                                billDetailService.Commit();
                                break;
                            }
                        }
                    }
                }

                return Json(JsonResultData.Successed(orderlist));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        #endregion 整单取消

        #region 转菜

        [AuthButton(AuthFlag.AuthManage)]
        public ActionResult _MergeTabB(string Flag, string billDetailId)
        {
            int PageIndex = 1;
            int PageSize = 22;
            var service = GetService<IPosTabStatusService>();

            ViewBag.PageIndex = PageIndex;
            ViewBag.PageSize = PageSize;

            ViewBag.Flag = Flag;
            ViewBag.billDetailId = billDetailId;

            var Service = GetService<IPosRefeService>();
            var refeList = Service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

            return PartialView("_MergeTab", refeList);
        }

        /// <summary>
        /// 服务费率等餐台资料
        /// </summary>
        /// <param name="newTabId">新餐台ID</param>
        /// <param name="billId">当前账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _TabService(string newTabId, string billId)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, billId);

            ViewBag.oldModel = bill;    //传参到界面

            ChangeTableModel result = new ChangeTableModel();
            //餐台信息
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, newTabId);

                DateTime openTime = DateTime.Now;

                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, posTab.Refeid, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                if (posTabServiceModel != null)
                {
                    result = new ChangeTableModel()
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                        newTabName = posTab.Cname,
                        newTabId = newTabId,
                        newTabNo = posTab.TabNo,
                        newRefeId = posTab.Refeid
                    };
                    //   ViewBag.newTab = result;
                }
            }
            catch (Exception ex)
            {
                return PartialView("_TabService", ex.Message.ToString());
                //  throw;
            }
            return PartialView("_TabService", result);
        }

        /// <summary>
        /// 转菜界面
        /// </summary>
        /// <param name="newTabId">新餐台ID</param>
        /// <param name="billId">当前账单ID</param>
        /// <param name="newBillId">新账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBillDetailListByMerge(string newTabId, string billId, string newBillId)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, billId);

            ViewBag.oldModel = bill;    //传参到界面

            ChangeTableModel result = new ChangeTableModel();
            //餐台信息
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, newTabId);

                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, posTab.Refeid, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                var Newbill = billService.GetPosBillByTabId(CurrentInfo.HotelId, posTab.Refeid, newTabId, newBillId ?? "");// Get(newBillId);
                if (Newbill != null)//已经有账单的餐台
                {
                    result = new ChangeTableModel()
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                        newTabName = posTab.Cname,
                        newTabId = newTabId,
                        newTabNo = posTab.TabNo,
                        newRefeId = posTab.Refeid,
                        oldBillId = billId,
                        newBillId = Newbill.Billid
                    };
                }
                else
                {
                    if (posTabServiceModel != null)
                    {
                        result = new ChangeTableModel()
                        {
                            newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                            newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                            newTabName = posTab.Cname,
                            newTabId = newTabId,
                            newTabNo = posTab.TabNo,
                            newRefeId = posTab.Refeid,
                            oldBillId = billId
                        };
                        //   ViewBag.newTab = result;
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("_PosBillDetailListByMerge", ex.Message.ToString());
                //  throw;
            }
            return PartialView("_PosBillDetailListByMerge", result);
        }

        /// <summary>
        /// 抹台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _PosSmearListByChangeItem(string model)
        {
            return PartialView("_PosSmearListByChangeItem", JsonConvert.DeserializeObject<List<PosBill>>(model));
        }

        /// <summary>
        /// 转菜
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ChangeItemByMerge(ChangeTableModel model)
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var billService = GetService<IPosBillService>();
            var oldBill = billService.Get(model.oldBillId);//当前账单信息

            var posTabServiceService = GetService<IPosTabServiceService>();

            var posTabService = GetService<IPosTabService>();//餐台服务
            var tab = posTabService.Get(model.newTabId);

            var refeSrvice = GetService<IPosRefeService>();
            var refe = refeSrvice.Get(model.newRefeId);

            var billDetailService = GetService<IPosBillDetailService>();

            string billDetailIdList = "";    //本次更改的数据ID 集合
            decimal outAmount = 0;//本次更改的金额
            if (string.IsNullOrEmpty(model.newBillId))
            {
                //获取新的账单ID
                var posBill = billService.GetLastBillId(CurrentInfo.HotelId, model.newRefeId, pos.Business);

                bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                if (isexsit)
                {
                    posBill = billService.GetLastBillId(CurrentInfo.HotelId, model.newRefeId, pos.Business);
                }

                #region 需要转的消费项目

                try
                {
                    #region 新账单处理

                    var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, model.newRefeId, tab.TabTypeid, null, (byte)PosITagperiod.随时, DateTime.Now);
                    PosBill bill = new PosBill()
                    {
                        Hid = CurrentInfo.HotelId,
                        Billid = posBill.Billid,
                        BillNo = posBill.BillNo,
                        MBillid = posBill.Billid,
                        InputUser = CurrentInfo.UserName,
                        BillDate = DateTime.Now,
                        IsService = true,
                        IsLimit = true,

                        ServiceRate = model.ServiceRateFlag == "1" ? oldBill.ServiceRate : model.newServiceRate,
                        Limit = model.ServiceRateFlag == "1" ? oldBill.Limit : model.newLimit,

                        IsByPerson = posTabServiceModel.IsByPerson == 0 ? false : true,
                        IHour = posTabServiceModel.LimitTime,
                        Discount = posTabServiceModel.Discount,

                        TabFlag = (byte)PosBillTabFlag.物理台,
                        Status = (byte)PosBillStatus.开台,
                        Shiftid = pos.ShiftId,
                        Shuffleid = refe.ShuffleId,
                        Refeid = model.newRefeId,
                        BillBsnsDate = pos.Business,
                        IGuest = oldBill.IGuest,
                        Name = oldBill.Name,
                        Tabid = model.newTabId,
                        TabNo = tab.TabNo
                    };

                    billService.Add(bill);
                    billService.AddDataChangeLog(OpLogType.Pos账单增加);
                    billService.Commit();
                    AddOperationLog(OpLogType.Pos账单增加, "转菜开台--营业点：" + bill.Tabid + "，餐台：" + bill.Tabid, bill.BillNo);

                    PosTabStatus tabStatus = new PosTabStatus() { Tabid = model.newTabId, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                    updateTabStatus(tabStatus);

                    //AddTabLog(model.newTabId, tab.TabNo, bill.Billid, model.ComputerName, refe);

                    #endregion 新账单处理

                    #endregion 需要转的消费项目

                    #region 新账单开台项目处理

                    if (model.ItemFlag == "2")    //新台的开台项目
                    {
                        var itemService = GetService<IPosItemService>();

                        var openItemService = GetService<IPosTabOpenItemService>();
                        var openItemList = openItemService.GetPosTabOpenItemByTabType(CurrentInfo.HotelId, tab.Module, tab.Refeid, tab.TabTypeid, "", (byte)PosITagperiod.随时, DateTime.Now);
                        foreach (var temp in openItemList)
                        {
                            if (temp.QuanMode == 1) //数量按人计
                            {
                                temp.Quantity = temp.Quantity * Convert.ToDecimal(bill.IGuest);
                            }

                            var item = itemService.GetEntity(CurrentInfo.HotelId, temp.Itemid);
                            var amount = item.IsDiscount == true ? (temp.Price * temp.Quantity) * bill.Discount : (temp.Price * temp.Quantity);
                            var server = item.IsService == true ? (temp.Price * temp.Quantity * bill.ServiceRate) : 0;

                            //判断开台项目的收费状态
                            var status = (byte)PosBillDetailStatus.保存;
                            if (temp.IsCharge == 0)
                            {
                                status = (byte)PosBillDetailStatus.赠送;
                            }
                            else if (temp.IsCharge == 1)
                            {
                                status = (byte)PosBillDetailStatus.例送;
                            }
                            else
                            {
                                status = (byte)PosBillDetailStatus.保存;
                            }

                            #region 消费项目明细赋值

                            PosBillDetail billDetail = new PosBillDetail()
                            {
                                MBillid = bill.MBillid,
                                Billid = bill.Billid,
                                Price = temp.Price,
                                Quantity = temp.Quantity,
                                Tabid = tab.Id,

                                Hid = CurrentInfo.HotelId,
                                Itemid = temp.Itemid,
                                ItemCode = temp.itemCode,
                                ItemName = temp.itemName,
                                Unitid = temp.Unitid,
                                UnitCode = temp.unitCode,
                                UnitName = temp.unitName,
                                DcFlag = PosItemDcFlag.D.ToString(),

                                IsCheck = false,
                                Isauto = (byte)PosBillDetailIsauto.开台项目,
                                Status = status,
                                //     IsProduce = (byte)PosBillDetailIsProduce.未出品,
                                Dueamount = temp.Price * temp.Quantity,
                                DiscAmount = 0,
                                Discount = item.IsDiscount == true ? bill.Discount : 100,
                                Amount = amount,
                                //   Service = server,

                                SP = false,
                                SD = false,
                                TransUser = CurrentInfo.UserName,
                                TransBsnsDate = bill.BillBsnsDate,
                                TransShiftid = bill.Shiftid,
                                TransShuffleid = bill.Shuffleid,
                                TransDate = DateTime.Now,
                            };

                            billDetailService.Add(billDetail);
                            // billDetailService.AddDataChangeLog(OpLogType.Pos账单消费明细增加);
                            billDetailService.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--名称：" + billDetail.ItemName + "，数量：" + billDetail.Quantity + "，金额" + billDetail.Dueamount + "，类型：" + billDetail.Isauto + "，状态：" + billDetail.Status, bill.BillNo);
                        }

                        #endregion 消费项目明细赋值
                    }

                    #endregion 新账单开台项目处理

                    bill.Discount = bill.Discount >= 1 && bill.Discount <= 100 ? bill.Discount / 100 : bill.Discount;

                    ChangeItemNum(model.ChangeItemJson, bill, oldBill, out billDetailIdList, out outAmount);

                    #region 账单操作记录表

                    var billChange = new PosBillChange
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        BillBsnsDate = oldBill.BillBsnsDate,
                        Refeid = oldBill.Refeid,
                        Tabid = oldBill.Tabid,
                        MBillid = oldBill.MBillid,
                        NmBillid = bill.MBillid,
                        Nrefeid = bill.Refeid,
                        Ntabid = bill.Tabid,
                        iStatus = (byte)PosBillChangeStatus.转菜,
                        Amount = outAmount,
                        BillRow = billDetailIdList,
                        Module = "CY",
                        TransUser = CurrentInfo.UserName,
                        CreateDate = DateTime.Now
                    };

                    var billChangeService = GetService<IPosBillChangeService>();
                    billChangeService.Add(billChange);
                    billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                    billChangeService.Commit();

                    #endregion 账单操作记录表

                    var ListDetail = billDetailService.GetDataSetByChangeItem(CurrentInfo.HotelId, oldBill.Tabid, billDetailIdList).ToList();

                    var serializer = new JavaScriptSerializer();
                    var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));
                    return Json(JsonResultData.Successed(valueStr));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }


            }
            else
            {
                //已存在账单的 转菜
                var newbill = billService.Get(model.newBillId);
                try
                {
                    newbill.Discount = newbill.Discount >= 1 && newbill.Discount <= 100 ? newbill.Discount / 100 : newbill.Discount;

                    ChangeItemNum(model.ChangeItemJson, newbill, oldBill, out billDetailIdList, out outAmount);

                    #region 账单操作记录表

                    var billChange = new PosBillChange
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        BillBsnsDate = oldBill.BillBsnsDate,
                        Refeid = oldBill.Refeid,
                        Tabid = oldBill.Tabid,
                        MBillid = oldBill.MBillid,
                        NmBillid = newbill.MBillid,
                        Nrefeid = newbill.Refeid,
                        Ntabid = newbill.Tabid,
                        iStatus = (byte)PosBillChangeStatus.转菜,
                        Amount = outAmount,
                        BillRow = billDetailIdList,
                        Module = "CY",
                        TransUser = CurrentInfo.UserName,
                        CreateDate = DateTime.Now
                    };

                    var billChangeService = GetService<IPosBillChangeService>();
                    billChangeService.Add(billChange);
                    billChangeService.AddDataChangeLog(OpLogType.Pos账单操作记录增加);
                    billChangeService.Commit();

                    #endregion 账单操作记录表

                    var ListDetail = billDetailService.GetDataSetByChangeItem(CurrentInfo.HotelId, oldBill.Tabid, billDetailIdList).ToList();
                    var serializer = new JavaScriptSerializer();
                    var valueStr = ReplaceJsonDateToDateString(serializer.Serialize(ListDetail));
                    return Json(JsonResultData.Successed(valueStr));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex.Message.ToString()));
                }
            }

            //  return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 处理消费明细
        /// </summary>
        /// <param name="itemJsonStr"></param>
        private void ChangeItemNum(string itemJsonStr, PosBill bill, PosBill oldBill, out string billDetailIdList, out decimal outAmount)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();

            List<ChangeItem> BillDetailList = Serializer.Deserialize<List<ChangeItem>>(itemJsonStr);

            var service = GetService<IPosBillDetailService>();  //账单明细

            var suitService = GetService<IPosItemSuitService>();    //套餐

            var itemService = GetService<IPosItemService>();    //消费项目

            var billDetailActionService = GetService<IPosBillDetailActionService>();//账单明细作法

            var actionService = GetService<IPosActionService>();    //作法

            billDetailIdList = "";
            outAmount = 0;
            foreach (var item in BillDetailList)
            {
                //处理新增数据
                var model = service.Get(int.Parse(item.Id));
                var newModel = new PosBillDetail();
                AutoSetValueHelper.SetValues(model, newModel);
                var quantity = item.Num == null ? model.Quantity : item.Num;
                if (quantity == model.Quantity)//如果数量相同 则修改账单明细的BillId
                {
                    if (model.SP == true && model.SD == false)//套餐
                    {
                        var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid);
                        foreach (var tcBillDetail in tcList)
                        {
                            if (tcBillDetail.Id != model.Id)
                            {
                                var newTcBillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                                newTcBillDetail.Billid = bill.Billid;
                                newTcBillDetail.MBillid = bill.Billid;
                                newTcBillDetail.TransUser = CurrentInfo.UserName;
                                newTcBillDetail.TransBsnsDate = bill.BillBsnsDate;
                                newTcBillDetail.TransShiftid = bill.Shiftid;
                                newTcBillDetail.TransShuffleid = bill.Shuffleid;
                                newTcBillDetail.TransDate = DateTime.Now;

                                service.Update(newTcBillDetail, tcBillDetail);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--单号：" + newTcBillDetail.Id + "，账单：" + oldBill.Billid + "-->" + newTcBillDetail.Billid, oldBill.BillNo);
                                billDetailIdList += newTcBillDetail.Id + ",";
                            }
                        }
                    }
                    newModel.Billid = bill.Billid;
                    newModel.MBillid = bill.Billid;
                    newModel.TransUser = CurrentInfo.UserName;
                    newModel.TransBsnsDate = bill.BillBsnsDate;
                    newModel.TransShiftid = bill.Shiftid;
                    newModel.TransShuffleid = bill.Shuffleid;
                    newModel.TransDate = DateTime.Now;
                    service.Update(newModel, model);
                    // service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--单号：" + newModel.Id + "，账单：" + oldBill.Billid + "-->" + newModel.Billid, oldBill.BillNo);
                    billDetailIdList += newModel.Id + ",";
                    outAmount += Convert.ToDecimal(newModel.Amount);

                }
                else
                {
                    var guid = new Guid();
                    var dueamount = quantity * model.Price;
                    var posItem = itemService.Get(model.Itemid);

                    var amount = posItem.IsDiscount == true ? dueamount * bill.Discount : dueamount;

                    #region 账单明细赋值

                    newModel.Quantity = quantity;
                    newModel.Billid = bill.Billid;
                    newModel.MBillid = bill.Billid;
                    newModel.Dueamount = dueamount;
                    newModel.TransUser = CurrentInfo.UserName;
                    newModel.TransBsnsDate = bill.BillBsnsDate;
                    newModel.TransShiftid = bill.Shiftid;
                    newModel.TransShuffleid = bill.Shuffleid;
                    newModel.TransDate = DateTime.Now;
                    newModel.Discount = bill.Discount;
                    newModel.Amount = amount;
                    if (model.SP == true && model.SD == false)
                    {
                        newModel.Upid = guid;
                    }

                    service.Add(newModel);
                    //service.AddDataChangeLog(OpLogType.Pos消费项目增加);
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细增加, "转菜--名称：" + newModel.ItemName + "，数量：" + newModel.Quantity, bill.BillNo);

                    #endregion 账单明细赋值

                    //复制作法
                    var actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Id);
                    foreach (var billDetailAction in actionList)
                    {
                        var newAction = new PosBillDetailAction();
                        AutoSetValueHelper.SetValues(billDetailAction, newAction);
                        newAction.MBillid = bill.Billid;
                        newAction.Mid = newModel.Id;
                        billDetailActionService.Add(newAction);
                        // billDetailActionService.AddDataChangeLog(OpLogType.Pos账单作法明细增加);
                        billDetailActionService.Commit();
                        AddOperationLog(OpLogType.Pos账单作法明细增加, "转菜--名称：" + newAction.ActionName + "，金额：" + newAction.Amount, bill.BillNo);
                    }
                    var oldQuantity = model.Quantity;//用于记录转菜前的数量
                    var olddueanmount = model.Dueamount;//用于记录转菜前的金额
                    model.Quantity -= quantity;
                    model.Dueamount = model.Quantity * model.Price;
                    //service.Update(model, new PosBillDetail());
                    ////  service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                    //service.Commit();


                    #region 处理作法金额

                    //当前操作的账单
                    actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, model.MBillid, model.Id);
                    decimal actionAmount = 0;
                    foreach (var billDetailAction in actionList)
                    {
                        var action = actionService.GetActionByCode(CurrentInfo.HotelId, billDetailAction.ActionNo, billDetailAction.ActionName);

                        if (action != null && action.AddPrice != null && action.AddPrice > 0)
                        {
                            if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quantity) * Convert.ToDecimal(oldBill.IGuest);
                            }
                            else if (action.IsByQuan != null && action.IsByQuan.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(model.Quantity);
                            }
                            else if (action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(oldBill.IGuest);
                            }
                            else
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice);
                            }
                        }
                    }

                    model.AddPrice = actionAmount;
                    model.Dueamount = model.Price * model.Quantity + actionAmount;
                    service.Update(model, new PosBillDetail());
                    service.Commit();
                    AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + model.Id + "，数量" + oldQuantity + "-->" + model.Quantity + "，金额：" + dueamount + "-->" + model.Dueamount, oldBill.BillNo);

                    //  新账单作法金额计算
                    actionList = billDetailActionService.GetBillDetailActionByMid(CurrentInfo.HotelId, newModel.MBillid, newModel.Id);
                    actionAmount = 0;
                    foreach (var billDetailAction in actionList)
                    {
                        var action = actionService.GetActionByCode(CurrentInfo.HotelId, billDetailAction.ActionNo, billDetailAction.ActionName);

                        if (action != null && action.AddPrice != null && action.AddPrice > 0)
                        {
                            if (action.IsByQuan != null && action.IsByQuan.Value && action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(newModel.Quantity) * Convert.ToDecimal(bill.IGuest);
                            }
                            else if (action.IsByQuan != null && action.IsByQuan.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(newModel.Quantity);
                            }
                            else if (action.IsByGuest != null && action.IsByGuest.Value)
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice) * Convert.ToDecimal(bill.IGuest);
                            }
                            else
                            {
                                actionAmount += Convert.ToDecimal(action.AddPrice);
                            }
                        }
                    }

                    model.AddPrice = actionAmount;
                    newModel.Dueamount = newModel.Price * newModel.Quantity + actionAmount;
                    service.Update(newModel, new PosBillDetail());
                    service.AddDataChangeLog(OpLogType.Pos账单消费明细修改);
                    service.Commit();

                    #endregion 处理作法金额

                    //如果是套餐
                    if (model.SP == true && model.SD == false)
                    {
                        // newModel.Upid = new Guid();
                        //套餐明细
                        var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid);

                        foreach (var tcBillDetail in tcList)
                        {
                            if (tcBillDetail.Id != model.Id)
                            {
                                #region 账单赋值

                                //新账单数据
                                var newTcBillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                                var suitList = suitService.GetPosItemSuitListByItemId(CurrentInfo.HotelId, model.Itemid).Where(m => m.ItemId2 == tcBillDetail.Itemid).FirstOrDefault();

                                newTcBillDetail.Quantity = suitList.Quantity * quantity;
                                newTcBillDetail.Dueamount = tcBillDetail.Quantity * tcBillDetail.Price;

                                newTcBillDetail.ModiDate = DateTime.Now;
                                newTcBillDetail.Amount = newTcBillDetail.Dueamount;
                                newTcBillDetail.Billid = bill.Billid;
                                newTcBillDetail.MBillid = bill.MBillid;
                                newTcBillDetail.Upid = guid;
                                service.Add(newTcBillDetail);
                                //service.AddDataChangeLog(OpLogType.Pos消费项目增加);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + newTcBillDetail.Id + "，数量：" + newTcBillDetail.Quantity + "，金额：" + newTcBillDetail.Dueamount, bill.BillNo);

                                //旧账单数据
                                var tcoldQuantity = tcBillDetail.Quantity;
                                var tcDueamount = tcBillDetail.Dueamount;

                                tcBillDetail.Quantity -= suitList.Quantity * quantity;
                                tcBillDetail.Dueamount = tcBillDetail.Quantity * tcBillDetail.Price;

                                tcBillDetail.ModiDate = DateTime.Now;
                                tcBillDetail.Amount = newTcBillDetail.Dueamount;

                                service.Update(tcBillDetail, new PosBillDetail());
                                //    service.AddDataChangeLog(OpLogType.Pos消费项目修改);
                                service.Commit();
                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + tcBillDetail.Id + "，数量" + tcoldQuantity + "-->" + tcBillDetail.Quantity + "，金额：" + tcDueamount + "-->" + tcBillDetail.Dueamount, oldBill.BillNo);
                                billDetailIdList += newTcBillDetail.Id + ",";

                                #endregion 账单赋值
                            }
                        }
                    }
                    billDetailIdList += newModel.Id + ",";
                    outAmount += Convert.ToDecimal(newModel.Amount);
                }
            }
        }

        private void updateTabStatus(PosTabStatus posTabStatus)
        {
            var tabStatusService = GetService<IPosTabStatusService>();
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
        /// 添加餐台记录
        /// </summary>
        /// <param name="tabId">餐台ID</param>
        /// <param name="tabNo">餐台代码</param>
        /// <param name="billId">账单ID</param>
        /// <param name="refe">营业点</param>
        private void AddTabLog(string tabId, string tabNo, string billId, string computerName, PosRefe refe)
        {
            #region 清理当前操作员之前的锁台记录

            var tabLogService = GetService<IPosTabLogService>();    //餐台记录

            string computer = computerName ?? "";
            var tabLogList = tabLogService.GetPosTabLogListByTab(CurrentInfo.HotelId, refe.Id, tabId, tabNo);
            if (tabLogList != null && tabLogList.Count > 0)
            {
                foreach (var tabLog in tabLogList)
                {
                    if (tabLog.Billid == billId)
                    {
                        tabLogService.Delete(tabLog);
                        tabLogService.AddDataChangeLog(OpLogType.Pos锁台删除);
                        tabLogService.Commit();
                    }
                }
            }

            #endregion 清理当前操作员之前的锁台记录

            #region 添加当前锁台记录

            PosTabLog posTabLog = new PosTabLog()
            {
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                Msg = string.Format($"{tabNo}餐台被{computer} => {CurrentInfo.UserName}在操作"),
                Status = (byte)PosTabLogStatus.开台自动锁台,
                Computer = computer,
                ConnectDate = DateTime.Now,
                Module = refe.Module,
                TransUser = CurrentInfo.UserName,
                CreateDate = DateTime.Now
            };
            posTabLog.Refeid = refe.Id;
            posTabLog.Billid = billId;
            posTabLog.Tabid = tabId;
            posTabLog.TabNo = tabNo;
            posTabLog.Refeid = refe.Id;
            //   AutoSetValueHelper.SetValues(model, posTabLog);
            tabLogService.Add(posTabLog);
            tabLogService.AddDataChangeLog(OpLogType.Pos锁台增加);
            tabLogService.Commit();

            #endregion 添加当前锁台记录
        }

        #endregion


        #region 设置最低消费
        [AuthButton(AuthFlag.Reset)]
        public ActionResult CancelMinConsume(string billid,bool islimit,decimal limit)
        {
            try
            {
                var billService = GetService<IPosBillService>();
                var bill = billService.Get(billid);

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                    {
                        return Json(JsonResultData.Failure("已经买单，清台的账单不能修改"));
                    }
                }

                var newEntity = new PosBill();
                AutoSetValueHelper.SetValues(bill, newEntity);

                newEntity.IsLimit = islimit;
                if (islimit == true)
                {
                    newEntity.Limit = limit;
                }

                billService.Update(newEntity, bill);
                billService.AddDataChangeLog(OpLogType.Pos账单修改);
                billService.Commit();

                AddOperationLog(OpLogType.Pos账单修改, "是否计最低消费：" + bill.IsLimit + "-->" + newEntity.IsLimit, bill.BillNo);

            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            return Json(JsonResultData.Successed());
        }

        #endregion

        #region 复制菜式
        /// <summary>
        /// 餐台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult _CopyTabList()
        {
            int PageIndex = 1;
            int PageSize = 22;
            var service = GetService<IPosTabStatusService>();
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageSize = PageSize;
            var Service = GetService<IPosRefeService>();
            var refeList = Service.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

            return PartialView("_CopyTabList", refeList);
        }
        /// <summary>
        /// 餐台列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult _CopyTabView(TabStatusViewModel model)
        {
            if (model != null)
            {
                model.Refeid = model.Refeid ?? "";
                model.PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
                model.PageSize = model.PageSize < 1 ? 1 : model.PageSize;
                var service = GetService<IPosTabStatusService>();
                var list = service.GetPosTabStatusResult(CurrentInfo.HotelId, model.Tabid, model.Refeid, "", null, model.PageIndex, model.PageSize);
                return PartialView("_CopyTabView", list);
            }
            return PartialView("_CopyTabView");
        }

        /// <summary>
        /// 处理消费明细
        /// </summary>
        /// <param name="itemJsonStr"></param>
        /// <param name="bill">新单</param>
        /// <param name="oldBill"></param>
        private void CopyChangeItemNum(string itemJsonStr, PosBill bill, PosBill oldBill)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();

            List<ChangeItem> BillDetailList = Serializer.Deserialize<List<ChangeItem>>(itemJsonStr);

            var service = GetService<IPosBillDetailService>();  //账单明细

            var suitService = GetService<IPosItemSuitService>();    //套餐

            var itemService = GetService<IPosItemService>();    //消费项目

            var billDetailActionService = GetService<IPosBillDetailActionService>();//账单明细作法

            var actionService = GetService<IPosActionService>();    //作法

            foreach (var item in BillDetailList)
            {
                //处理新增数据
                var model = service.Get(int.Parse(item.Id));
                var newModel = new PosBillDetail();
                AutoSetValueHelper.SetValues(model, newModel);
                //先判断目标单据是否已存在明细
                var targetdetaillist = service.GetBillDetailByBillidAndStatus(CurrentInfo.HotelId, bill.Billid, PosItemDcFlag.D.ToString(), 50).Where(t => t.Itemid == model.Itemid && t.IsProduce == 0 && t.Status == 4).ToList();
                if (targetdetaillist.Count > 0)
                {
                    foreach (var targetitem in targetdetaillist)
                    {
                        if (model.SP == true && model.SD == false)//套餐
                        {
                            var newBillDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(targetitem, newBillDetail);
                            newBillDetail.Quantity += model.Quantity;
                            service.Update(newBillDetail, targetitem);
                            service.Commit();
                            //新单中的对应的套餐明细
                            var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, newBillDetail.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == newBillDetail.Upid && m.SP == false && m.SD == true).ToList();
                            //旧单中的对应的套餐明细
                            var oldtcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid && m.SP == false && m.SD == true).ToList();
                            foreach (var tcBillDetail in tcList)
                            {
                                var newTcBillDetail = new PosBillDetail();
                                AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                                newTcBillDetail.Quantity += oldtcList.Where(t => t.Itemid == tcBillDetail.Itemid).FirstOrDefault().Quantity;
                                service.Update(newTcBillDetail, tcBillDetail);
                                service.Commit();
                            }
                        }
                        else
                        {
                            var newBillDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(targetitem, newBillDetail);
                            newBillDetail.Quantity += model.Quantity;
                            service.Update(newBillDetail, targetitem);
                            service.Commit();
                        }
                    }
                }
                else
                {
                    if (model.SP == true && model.SD == false)//套餐
                    {
                        var guid = Guid.NewGuid();
                        newModel.Billid = bill.Billid;
                        newModel.MBillid = bill.Billid;
                        newModel.TransUser = CurrentInfo.UserName;
                        newModel.TransBsnsDate = bill.BillBsnsDate;
                        newModel.TransShiftid = bill.Shiftid;
                        newModel.TransShuffleid = bill.Shuffleid;
                        newModel.TransDate = DateTime.Now;
                        newModel.IsProduce = 0;
                        newModel.Status = 4;
                        newModel.Upid = guid;
                        service.Add(newModel);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "复制菜--单号：" + newModel.Id + "，账单：" + oldBill.Billid + "-->" + newModel.Billid, oldBill.BillNo);
                        var tcList = service.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString()).Where(m => m.Upid == model.Upid && m.SP == false && m.SD == true).ToList();
                        foreach (var tcBillDetail in tcList)
                        {
                            var newTcBillDetail = new PosBillDetail();
                            AutoSetValueHelper.SetValues(tcBillDetail, newTcBillDetail);
                            newTcBillDetail.Billid = bill.Billid;
                            newTcBillDetail.MBillid = bill.Billid;
                            newTcBillDetail.TransUser = CurrentInfo.UserName;
                            newTcBillDetail.TransBsnsDate = bill.BillBsnsDate;
                            newTcBillDetail.TransShiftid = bill.Shiftid;
                            newTcBillDetail.TransShuffleid = bill.Shuffleid;
                            newTcBillDetail.TransDate = DateTime.Now;
                            newTcBillDetail.IsProduce = 0;
                            newTcBillDetail.Status = 4;
                            newTcBillDetail.Upid = guid;
                            service.Add(newTcBillDetail);
                            service.Commit();
                            AddOperationLog(OpLogType.Pos账单消费明细增加, "复制菜--单号：" + newTcBillDetail.Id + "，账单：" + oldBill.Billid + "-->" + newTcBillDetail.Billid, oldBill.BillNo);
                        }
                    }
                    else
                    {
                        newModel.Billid = bill.Billid;
                        newModel.MBillid = bill.Billid;
                        newModel.TransUser = CurrentInfo.UserName;
                        newModel.TransBsnsDate = bill.BillBsnsDate;
                        newModel.TransShiftid = bill.Shiftid;
                        newModel.TransShuffleid = bill.Shuffleid;
                        newModel.TransDate = DateTime.Now;
                        newModel.IsProduce = 0;
                        newModel.Status = 4;
                        service.Add(newModel);
                        service.Commit();
                        AddOperationLog(OpLogType.Pos账单消费明细增加, "复制菜--单号：" + newModel.Id + "，账单：" + oldBill.Billid + "-->" + newModel.Billid, oldBill.BillNo);
                    }
                }
            }
        }
        /// <summary>
        /// 复制项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult CopyItem(string sourcebillid, string targetbillid, string targettabid, string itemidarry, string CustomerTypeid, int iGuest)
        {
            try
            {
                var tabService = GetService<IPosTabService>();
                var tab = tabService.Get(targettabid);
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.Get(tab.Refeid);
                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);
                var billService = GetService<IPosBillService>();
                //var service = GetService<IPosBillService>();
                if (string.IsNullOrWhiteSpace(targetbillid))
                {
                    var posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                    bool isexsit = billService.IsExists(CurrentInfo.HotelId, posBill.Billid, posBill.BillNo);
                    if (isexsit)
                    {
                        posBill = billService.GetLastBillId(CurrentInfo.HotelId, tab.Refeid, pos.Business);
                    }
                    DateTime openTime = DateTime.Now;
                    var posTabServiceService = GetService<IPosTabServiceService>();
                    var posTabService = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, tab.Refeid, tab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);
                    //根据原餐台id查找当前所要复制的主单据
                    var oldbill = billService.Get(sourcebillid);
                    //新单
                    PosBill bill = new PosBill();
                    AutoSetValueHelper.SetValues(oldbill, bill);
                    bill.InputUser = CurrentInfo.UserName;
                    bill.BillDate = openTime;
                    bill.Billid = posBill.Billid;
                    bill.BillNo = posBill.BillNo;
                    bill.MBillid = posBill.Billid;
                    bill.Refeid = tab.Refeid;
                    bill.Tabid = tab.Id;
                    bill.TabNo = tab.TabNo;
                    bill.CustomerTypeid = CustomerTypeid;
                    bill.IGuest = iGuest;
                    bill.ServiceRate = posTabService.Servicerate;
                    bill.Limit = posTabService.NLimit;
                    bill.IsByPerson = posTabService.IsByPerson == 0 ? false : true;
                    bill.IHour = posTabService.LimitTime;
                    bill.Discount = posTabService.Discount;
                    bill.TabFlag = (byte)PosBillTabFlag.物理台;
                    bill.Status = (byte)PosBillStatus.开台;
                    bill.Shiftid = pos.ShiftId;
                    bill.Shuffleid = refe.ShuffleId;
                    bill.BillBsnsDate = pos.Business;
                    billService.Add(bill);
                    billService.AddDataChangeLog(OpLogType.Pos账单增加);
                    billService.Commit();
                    CopyChangeItemNum(itemidarry, bill, oldbill);
                    PosTabStatus tabStatus = new PosTabStatus() { GuestName = "", Tabid = targettabid, TabStatus = (byte)PosTabStatusEnum.就座, OpTabid = bill.Billid, OpenRecord = DateTime.Now, OpenGuest = bill.IGuest };
                    updateTabStatus(tabStatus);
                    //添加锁台记录
                    PosCommon common = new PosCommon();
                    common.AddTabLog(targettabid, tab.TabNo, bill.Billid, "", refe);
                    return Json(JsonResultData.Successed("复制成功"));
                }
                else
                {
                    var oldbill = billService.Get(sourcebillid);
                    var newbill = billService.Get(targetbillid);
                    if (newbill != null && oldbill != null)
                    {
                        CopyChangeItemNum(itemidarry, newbill, oldbill);
                        return Json(JsonResultData.Successed("复制成功"));
                    }
                    else
                    {
                        return Json(JsonResultData.Successed("复制失败"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }
        }
        /// <summary>
        /// 复制界面
        /// </summary>
        /// <param name="newTabId">新餐台ID</param>
        /// <param name="billId">当前账单ID</param>
        /// <param name="newBillId">新账单ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _CopyTabBillDetail(string newTabId, string billId, string newBillId)
        {
            var billService = GetService<IPosBillService>();
            var bill = billService.GetPosBillByBillid(CurrentInfo.HotelId, billId);

            ViewBag.oldModel = bill;    //传参到界面

            ChangeTableModel result = new ChangeTableModel();
            //餐台信息
            try
            {
                var posTabService = GetService<IPosTabService>();
                var posTab = posTabService.GetEntity(CurrentInfo.HotelId, newTabId);

                DateTime openTime = DateTime.Now;
                var posTabServiceService = GetService<IPosTabServiceService>();
                var posTabServiceModel = posTabServiceService.GetPosTabService(CurrentInfo.HotelId, CurrentInfo.ModuleCode, posTab.Refeid, posTab.TabTypeid, null, (byte)PosITagperiod.随时, openTime);

                var Newbill = billService.GetPosBillByTabId(CurrentInfo.HotelId, posTab.Refeid, newTabId, newBillId ?? "");// Get(newBillId);
                if (Newbill != null)//已经有账单的餐台
                {
                    result = new ChangeTableModel()
                    {
                        newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                        newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                        newTabName = posTab.Cname,
                        newTabId = newTabId,
                        newTabNo = posTab.TabNo,
                        newRefeId = posTab.Refeid,
                        oldBillId = billId,
                        newBillId = Newbill.Billid
                    };
                }
                else
                {
                    if (posTabServiceModel != null)
                    {
                        result = new ChangeTableModel()
                        {
                            newServiceRate = posTabServiceModel.Servicerate == null ? 0 : posTabServiceModel.Servicerate,
                            newLimit = posTabServiceModel.NLimit == null ? 0 : posTabServiceModel.NLimit,
                            newTabName = posTab.Cname,
                            newTabId = newTabId,
                            newTabNo = posTab.TabNo,
                            newRefeId = posTab.Refeid,
                            oldBillId = billId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return PartialView("_CopyTabBillDetail", ex.Message.ToString());
            }
            return PartialView("_CopyTabBillDetail", result);
        }
        #endregion

    }
}