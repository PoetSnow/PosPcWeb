using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.OnlineInterfaceManage;
using Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage.Invoice;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    public class InvoiceController : Controller
    {
        /// <summary>
        /// 添加开票记录
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="billid">账单Id</param>
        /// <returns></returns>
        public ActionResult InvoiceConsum(string hid, string billid)
        {
            var currentInfo = GetService<ICurrentInfo>();
            currentInfo.HotelId = hid;

            var service = invoiceService(currentInfo);
            service.ConsumInfo(billid);

            return Json(JsonResultData.Successed());
        }


        /// <summary>
        /// 撤销开票记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <returns></returns>

        public ActionResult ConsumInfoRepeal(string hid, string billid)
        {
            var currentInfo = GetService<ICurrentInfo>();
            currentInfo.HotelId = hid;

            var service = invoiceService(currentInfo);
            service.ConsumInfoRepeal(billid);
            return Json(JsonResultData.Successed());
        }
        #region 买单调用发票接口



        /// <summary>
        /// 是否启用发票接口
        /// </summary>
        /// <returns></returns>
        private bool IsEnableInvoice(string hid, out string InvoiceAppId, out string InvoiceAppSecret)
        {
            var service = GetService<IPmsParaService>();

            InvoiceAppId = "";
            InvoiceAppSecret = "";
            var isInvoice = service.GetValue(hid, "hasEInvoice");
            if (!string.IsNullOrEmpty(isInvoice))
            {
                if (isInvoice == "0") { return false; }
                else
                {
                    InvoiceAppId = service.GetValue(hid, "InvoiceAppId");
                    InvoiceAppSecret = service.GetValue(hid, "InvoiceAppSecret");
                    return true;
                }
            }
            else { return false; }
        }
        #endregion


        private T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        private InvoiceService invoiceService(ICurrentInfo currentInfo)
        {
            string InvoiceAppId = "";
            string InvoiceAppSecret = "";
            if (!IsEnableInvoice(currentInfo.HotelId, out InvoiceAppId, out InvoiceAppSecret))
            {
                return null;
            }
            //接口定义参数
            var model = new InvoiceModel()
            {
                RequestUrl = RequestUrl(),
                InvoiceAppId = InvoiceAppId,
                InvoiceAppSecret = InvoiceAppSecret

            };

            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelEntity = hotelInfoService.GetHotelInfo(currentInfo.HotelId);
            if (hotelEntity == null || hotelEntity.Hid != currentInfo.HotelId && hotelEntity.Status != EntityStatus.启用)
            {
                return null;
            }

            //获取数据库
            var isConnectViaInternet = hotelInfoService.IsConnectViaInternte();
            var _pmsContext = new DbHotelPmsContext(ConnStrHelper.GetConnStr(hotelEntity.DbServer, hotelEntity.DbName, hotelEntity.Logid, hotelEntity.LogPwd, "permanentRoomJob", hotelEntity.DbServerInternet, isConnectViaInternet), currentInfo.HotelId, "permanentRoomJobEvent", null);
            if (_pmsContext == null)
            {
                return null;
            }

            //实例化接口
            var service = new InvoiceService(hotelInfoService, _pmsContext, currentInfo, model);

            return service;
        }

        private string RequestUrl()
        {
            var IsEnvTest = MvcApplication.IsTestEnv;
            if (IsEnvTest)
            {
                return "http://vtest.gshis.com";
            }
            else
            {
                var url = Request.Url.ToString();
                if (url.Contains("vip2"))
                {
                    return "http://vip2.v.gshis.com";
                }
                return "http://v.gshis.com";
            }
        }
    }
}