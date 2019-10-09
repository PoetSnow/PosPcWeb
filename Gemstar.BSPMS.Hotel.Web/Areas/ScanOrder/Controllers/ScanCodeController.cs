using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System.Linq;
using System.Web.Mvc;
using OAuthHelper = Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models.OAuthHelper;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Controllers
{
    [NotAuth]
    public class ScanCodeController : BaseScanOrderController
    {
        // GET: ScanOrder/ScanCode
        public ActionResult Index(string hid, string tabid, string flag)
        {
            if (!string.IsNullOrWhiteSpace(hid) && (string.IsNullOrWhiteSpace(BaseHid) || BaseHid != hid))
            {
                BaseHid = hid;
            }
            if (!string.IsNullOrWhiteSpace(tabid) && (string.IsNullOrWhiteSpace(BaseTabid) || BaseTabid != tabid))
            {
                BaseTabid = tabid;
            }
            if (!string.IsNullOrWhiteSpace(flag) && (string.IsNullOrWhiteSpace(OpenFlag) || OpenFlag != flag))
            {
                OpenFlag = flag;
            }
            currentInfo = GetService<ICurrentInfo>();
            currentInfo.ProductType = ProductType.Pos;
            currentInfo.SaveValues();

            var oAuth = new OAuthAccessTokenResult();
            if (!string.IsNullOrWhiteSpace(BaseHid) && !string.IsNullOrWhiteSpace(BaseTabid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var pmsParaService = new PmsParaService(hotelDb);
                var interfacePara = new GsWxInterfaceModel(hotelDb, hid);

                if (interfacePara.IsEnable == true)
                {
                    var getOpenIdUrl = interfacePara.OpenidUrl;
                    var comid = interfacePara.CompanyId;
                    if (string.IsNullOrEmpty(getOpenIdUrl) || string.IsNullOrEmpty(comid))
                    {
                        return Content("接口参数为空");
                    }
                    var backUrl = "http://" + Request.Url.Host.ToString() + "/scanorder/Order/_OrderInfo@hid=" + hid + "@tabid=" + tabid;
                    var fullUrl = string.Format("{0}?comid={1}&oauth2=true&url={2}", getOpenIdUrl, Server.UrlEncode(comid), Server.UrlEncode(backUrl));
                    return Redirect(fullUrl);
                }
                else
                {
                    var hotelPayParas = pmsParaService.GetPmsParas(BaseHid);

                    var appIdPara = hotelPayParas.SingleOrDefault(w => w.Hid == BaseHid && w.Code == "POSWxProviderAppID");
                    var secretPara = hotelPayParas.SingleOrDefault(w => w.Hid == BaseHid && w.Code == "POSWxProviderSecret");
                    if (appIdPara != null && string.IsNullOrEmpty(appIdPara.Value))
                    {
                        oAuth.errmsg += $"请设置系统参数：{appIdPara.Name}<br />";
                    }
                    if (secretPara != null && string.IsNullOrEmpty(secretPara.Value))
                    {
                        oAuth.errmsg += $"请设置系统参数：{secretPara.Name}<br />";
                    }

                    if (appIdPara != null && secretPara != null)
                    {
                        var appid = appIdPara == null ? "" : appIdPara.Value;
                        var redirectUrl = "http://" + Request.Url.Host + "/scanorder/scancode/GetAccessToken?Hid=" + hid + "&TabId=" + tabid;
                        var url = OAuthHelper.GetWxAuthorizeUrl(appid, redirectUrl, "", OAuthScope.snsapi_base);

                        return Redirect(url + "&flag=" + flag);
                    }
                }

            }

            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;
            ViewBag.Title = "扫码点餐";
            return View("Index", oAuth);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Timeout"></param>
        /// <returns></returns>
        public ActionResult GetAccessToken(string Hid, string TabId, string code, string state)
        {
            var oAuth = new OAuthAccessTokenResult();
            if (!string.IsNullOrWhiteSpace(Hid))
            {
                var hotelDb = GetHotelDb(BaseHid);
                var pmsParaService = new PmsParaService(hotelDb);
                var hotelPayParas = pmsParaService.GetPmsParas(Hid);

                var appIdPara = hotelPayParas.SingleOrDefault(w => w.Hid == Hid && w.Code == "POSWxProviderAppID");
                var secretPara = hotelPayParas.SingleOrDefault(w => w.Hid == Hid && w.Code == "POSWxProviderSecret");

                if (appIdPara != null && string.IsNullOrEmpty(appIdPara.Value))
                {
                    oAuth.errmsg += $"请设置系统参数：{appIdPara.Name}<br />";
                }
                if (secretPara != null && string.IsNullOrEmpty(secretPara.Value))
                {
                    oAuth.errmsg += $"请设置系统参数：{secretPara.Name}<br />";
                }

                if (appIdPara != null && !string.IsNullOrWhiteSpace(appIdPara.Value)
                    && secretPara != null && !string.IsNullOrWhiteSpace(secretPara.Value))
                {
                    var appid = appIdPara.Value;

                    var secret = secretPara.Value;

                    var Openid = OAuthHelper.GetWxOpenId(appid, secret, code);

                    BaseOpenid = Openid;
                    oAuth.openid = Openid;

                    return RedirectToAction("_OrderInfo", "Order", new { hid = Hid, tabid = TabId, openid = Openid });
                }
            }
            else
            {
                oAuth.errmsg = "参数错误";
            }

            ViewBag.Hid = BaseHid;
            ViewBag.Tabid = BaseTabid;
            return View("Index", oAuth);
        }
    }
}