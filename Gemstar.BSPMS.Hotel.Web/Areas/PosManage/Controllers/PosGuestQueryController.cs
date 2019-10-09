using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosGuestQuery;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos客账查询
    /// </summary>
    [AuthPage(ProductType.Pos, "p20003")]
    public class PosGuestQueryController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            ViewBag.Version = CurrentVersion;
            return View();
        }

        #region 获取分布视图

        /// <summary>
        /// 反结账单列表视图
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosBillList(Models.PosGuestQuery.QueryBillModel model)
        {
            var list = new List<up_pos_list_billByPosidResult>();

            model.Hid = CurrentInfo.HotelId;
            if (string.IsNullOrEmpty(model.PosId))
            {
                //没有选择收银点的情况下默认是当前操作的收银点
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.GetRefeByPosid(CurrentInfo.HotelId, CurrentInfo.PosId);

                var posService = GetService<IPosPosService>();
                var pos = posService.Get(CurrentInfo.PosId);
                if (refe != null && refe.Count > 0)
                {
                    var service = GetService<IPosBillService>();
                    list = service.GetPosBillGuestQuery(CurrentInfo.HotelId, CurrentInfo.PosId, pos.Business, model.tabNo ?? "");
                }
            }
            else
            {
                var refeService = GetService<IPosRefeService>();
                var refe = refeService.GetRefeByPosid(CurrentInfo.HotelId, string.IsNullOrEmpty(model.PosId) ? CurrentInfo.PosId : model.PosId);

                var QueryBillModel = new Services.EntitiesPosProcedures.QueryBillModel();
                AutoSetValueHelper.SetValues(model, QueryBillModel);

                if (refe != null && refe.Count > 0)
                {
                    var service = GetService<IPosBillService>();
                    list = service.GetPosBillGuestQuery(QueryBillModel);
                }
            }

            return PartialView("_PosBillList", list);
        }

        #endregion 获取分布视图

        #region 获取列表

        /// <summary>
        /// 获取账单明细付款列表
        /// </summary>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemsForBillDetailByBill([DataSourceRequest]DataSourceRequest request, CashierViewModel model)
        {
            var service = GetService<IPosBillDetailService>();
            var list = service.GetBillDetailForPaymentByBillid(CurrentInfo.HotelId, model.Billid);
            return Json(list.ToDataSourceResult(request));
        }

        #endregion 获取列表

        #region 客账查询

        [AuthButton(AuthFlag.None)]
        public ActionResult _QueryBillHtml()
        {
            var posService = GetService<IPosPosService>();
            var pos = posService.Get(CurrentInfo.PosId);

            var model = new Models.PosGuestQuery.QueryBillModel();
            model.PosId = CurrentInfo.PosId;
            model.BillBsnsDate = pos.Business;
            return PartialView("_QueryBillHtml", model);
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult SearchBill(Models.PosGuestQuery.QueryBillModel model)
        {
            model.Hid = CurrentInfo.HotelId;
            if (!string.IsNullOrEmpty(Request["RefeId"]))
            {
                //营业点ID
                model.RefeId = Request["RefeId"].ToString();
            }
            if (!string.IsNullOrEmpty(Request["PayMethod"]))
            {
                //付款方式
                model.PayMethod = Request["PayMethod"].ToString();
            }

            var refeService = GetService<IPosRefeService>();
            var refe = refeService.GetRefeByPosid(CurrentInfo.HotelId, string.IsNullOrEmpty(model.PosId) ? CurrentInfo.PosId : model.PosId);
            var list = new List<up_pos_list_billByPosidResult>();

            var QueryBillModel = new Services.EntitiesPosProcedures.QueryBillModel();
            AutoSetValueHelper.SetValues(model, QueryBillModel);

            if (refe != null && refe.Count > 0)
            {
                var service = GetService<IPosBillService>();
                list = service.GetPosBillGuestQuery(QueryBillModel);
            }
            return PartialView("_PosBillList", list);
        }

        #endregion 客账查询

        #region 打印账单

        [HttpPost]
        [AuthButton(AuthFlag.Export)]
        public ActionResult AddQueryParaTemp(ReportQueryModel model, string print, string Flag)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.AddQueryParaTemp(model, print, Flag,controller);
           
        }

        #endregion 打印账单

        #region 打印埋脚

        [AuthButton(AuthFlag.Print)]
        public ActionResult PrintBillPayMethod(ReportQueryModel model, string print)
        {
            PosCommon common = new PosCommon();
            var controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            return common.PrintBillPayMethod(model, print, controller);
        }

        #endregion 打印埋脚
    }
}