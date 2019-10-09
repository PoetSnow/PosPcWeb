using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 入单界面 折扣功能
    /// </summary>

    [AuthPage(ProductType.Pos, "p600001")]
    public class PosInSingleDiscountController : BaseEditInWindowController<PosBill, IPosBillService>
    {
        // GET: PosManage/PosInSingleDiscount
        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 直接修改全单折扣
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult UpdateDiscTypeA(PosBill model)
        {

            try
            {
                var service = GetService<IPosBillService>();
                var bill = service.Get(model.Billid);

                //获取系统参数，
                var paraservice = GetService<IPmsParaService>();
                var IsPayOrderAgain = paraservice.IsPayOrderAgain(CurrentInfo.HotelId);
                if (!IsPayOrderAgain)   //不让二次消费，不允许买单后账单修改
                {
                    if (bill.Status == (byte)PosBillStatus.清台 || bill.Status == (byte)PosBillStatus.结账)
                    {
                        return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
                    }
                }

                if (bill != null)
                {
                    try
                    {
                        PosCommon common = new PosCommon();
                        var result = common.CheckOperDiscount(model.Discount, null, bill.Refeid);
                        if (!result.Success)
                        {
                            return Json(result);
                        }

                        if (model.IsForce == 0)   //全单折
                        {
                            // bill.IsForce = 1;
                            bill.IsForce = (byte)PosBillIsForce.全单折;
                        }
                        else if (model.IsForce == 1)//照单折
                        {
                            // bill.IsForce = 2;
                            bill.IsForce = (byte)PosBillIsForce.照单折;
                        }
                        bill.Discount = model.Discount;
                        //bill.IsForce = (byte)PosBillIsForce.全单折;
                        bill.Approver = CurrentInfo.UserName;
                        var oldBill = new PosBill() { IsForce = bill.IsForce, Discount = bill.Discount };
                        AddOperationLog(OpLogType.Pos账单修改, "单号：" + bill.Billid + "，台号：" + bill.Tabid + "，名称：" + bill.Name + "，全单折扣：" + oldBill.Discount + " -> " + bill.Discount + "，折扣类型：" + oldBill.IsForce + " -> " + bill.IsForce, bill.BillNo);

                        service.Update(bill, new PosBill());
                        service.Commit();

                        var billDetailService = GetService<IPosBillDetailService>();
                        var billDetailList = billDetailService.GetBillDetailByDcFlag(CurrentInfo.HotelId, model.Billid, PosItemDcFlag.D.ToString());
                        if (billDetailList != null && billDetailList.Count > 0)
                        {
                            foreach (var billDetail in billDetailList)
                            {
                                var oldBillDetail = new PosBillDetail() { Amount = billDetail.Amount, Discount = billDetail.Discount, Service = billDetail.Service };

                                billDetail.Discount = bill.Discount;
                                billDetail.Dueamount = billDetail.Price * billDetail.Quantity;
                                billDetail.Amount = (billDetail.Price * billDetail.Quantity - billDetail.DiscAmount) * bill.Discount;
                                billDetail.Service = billDetail.Price * billDetail.Quantity * bill.ServiceRate * bill.Discount;

                                AddOperationLog(OpLogType.Pos账单消费明细修改, "单号：" + billDetail.Id + "，台号：" + billDetail.Tabid + "，名称：" + billDetail.ItemName + "，折扣：" + oldBillDetail.Discount + "->" + billDetail.Discount + "，金额：" + oldBillDetail.Amount + "->" + (billDetail.Price * billDetail.Quantity + "，服务费：" + oldBillDetail.Service + "->" + billDetail.Service), bill.BillNo);

                                billDetailService.Update(billDetail, new PosBillDetail());
                                billDetailService.Commit();
                            }
                        }
                        //重新计算金额
                        //  billDetailService.UpdateBillDetailDisc(CurrentInfo.HotelId, bill.Billid, bill.MBillid);
                        return Json(JsonResultData.Successed(""));
                    }
                    catch (Exception ex)
                    {
                        return Json(JsonResultData.Failure(ex));
                    }
                }
                return Json(JsonResultData.Failure(""));
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        #region 折扣界面
        /// <summary>
        /// 输入折扣界面。权限控制到这里
        /// </summary>
        /// <param name="discType"></param>
        /// <param name="BillId"></param>
        /// <param name="detailIdList"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult _DiscountNumBerA(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }


        [AuthButton(AuthFlag.Add)]
        public ActionResult _DiscountNumBerB(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }

        [AuthButton(AuthFlag.Update)]
        public ActionResult _DiscountNumBerC(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }

        [AuthButton(AuthFlag.Delete)]
        public ActionResult _DiscountNumBerD(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }

        [AuthButton(AuthFlag.Export)]
        public ActionResult _DiscountNumBerE(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }

        [AuthButton(AuthFlag.Print)]
        public ActionResult _DiscountNumBerF(string discType, string BillId, string detailIdList)
        {
            ViewBag.discType = discType;    //折扣类型
            ViewBag.BillId = BillId;        //主账单ID
            ViewBag.detailIdList = detailIdList;        //明细账单ID
            return PartialView("_DiscountNumber");
        }
        #endregion


    }
}