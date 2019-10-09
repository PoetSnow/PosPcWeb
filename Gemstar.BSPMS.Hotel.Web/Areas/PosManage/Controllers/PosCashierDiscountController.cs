using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 收银界面折扣功能权限控制
    /// </summary>

    [AuthPage(ProductType.Pos, "p20001001")]
    public class PosCashierDiscountController : BaseController
    {
       
        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            return View();
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


        #region 取消折扣

        /// <summary>
        /// 取消折扣
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AuthManage)]
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
                if (bill.Status == (byte)PosBillStatus.结账 || bill.Status == (byte)PosBillStatus.清台)
                {
                    return Json(JsonResultData.Failure("已经买单，清台的账单不能修改折扣"));
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

        #region 取消折扣验证

        [AuthButton(AuthFlag.Reset)]
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

    }
}