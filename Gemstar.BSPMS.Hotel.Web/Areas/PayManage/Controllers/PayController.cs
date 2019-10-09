using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PayManage.Controllers
{
    /// <summary>
    /// 通用的支付管理
    /// </summary>
    public class PayController : PayBaseController
    {

        /// <summary>
        /// 检查酒店是否可以使用指定的付款方式
        /// </summary>
        /// <param name="payAction">付款处理方式代码</param>
        /// <returns>JsonResultData实例，其中指出了是否可以使用，不能使用的原因以及可以使用时的调用地址</returns>
        public ActionResult Check(string payAction)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(payAction))
                {
                    return Json(JsonResultData.Failure("请指定付款处理方式"));
                }
                //检查酒店是否开通了指定付款方式的权限
                //检查酒店是否正确设置了指定付款方式的参数
                if (payAction.Equals("credit", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayCredit")));
                }
                if (payAction.Equals("bankCard", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayBankCard")));
                }
                if (payAction.Equals("mbrCard", StringComparison.OrdinalIgnoreCase) || payAction.Equals("mbrLargess",StringComparison.OrdinalIgnoreCase) || payAction.Equals("mbrCashTicket", StringComparison.OrdinalIgnoreCase) || payAction.Equals("mbrCardAndLargess", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayMbrCard")));
                }
                if (payAction.Equals("corp", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayCorp")));
                }
                if (payAction.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase))
                {
                    if (!IsWxCommonParaReady("WxBarcode"))
                    {
                        return Json(JsonResultData.Failure("微信支付的服务商参数设置不正确，请与软件开发商联系"));
                    }
                    if (!IsWxHotelParaReady())
                    {
                        return Json(JsonResultData.Failure("微信支付的酒店对应子商户号设置不正确，请前往系统参数中设置"));
                    }
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayWxBarcode")));
                }
                if (payAction.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase))
                {
                    if (!IsWxCommonParaReady("WxQrcode"))
                    {
                        return Json(JsonResultData.Failure("微信支付的服务商参数设置不正确，请与软件开发商联系"));
                    }
                    if (!IsWxHotelParaReady())
                    {
                        return Json(JsonResultData.Failure("微信支付的酒店对应子商户号设置不正确，请前往系统参数中设置"));
                    }
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayWxQrcode")));
                }
                if (payAction.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase))
                {
                    if (!IsAliCommonParaReady("AliBarcode"))
                    {
                        return Json(JsonResultData.Failure("支付宝支付的服务商参数设置不正确，请与软件开发商联系"));
                    }
                    if (!IsAliHotelParaReady())
                    {
                        return Json(JsonResultData.Failure("支付宝支付的酒店参数设置不正确，请前往系统参数中设置"));
                    }
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayAliBarcode")));
                }
                if (payAction.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase))
                {
                    if (!IsAliCommonParaReady("AliQrcode"))
                    {
                        return Json(JsonResultData.Failure("支付宝支付的服务商参数设置不正确，请与软件开发商联系"));
                    }
                    if (!IsAliHotelParaReady())
                    {
                        return Json(JsonResultData.Failure("支付宝支付的酒店参数设置不正确，请前往系统参数中设置"));
                    }
                    return Json(JsonResultData.Successed(Url.Action("Input", "PayAliQrcode")));
                }
                return Json(JsonResultData.Successed(""));
            }
            catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 微信支付的通用参数是否设置正确
        /// </summary>
        /// <returns>true:设置正确，false:设置不正确</returns>
        private bool IsWxCommonParaReady(string payAction)
        {
            var commonDb = GetService<DbCommonContext>();
            var count = commonDb.Database.SqlQuery<int>("select count(*) as count from hotel where hid = @hid and itemaction like '%"+ payAction + "%' union SELECT COUNT(*) AS count FROM m_v_payPara WHERE code IN('WxProviderAppId', 'WxProviderKey', 'WxProviderMchId') AND value <> ''", new SqlParameter("@hid",CurrentInfo.HotelId)).ToList();

            var itemCount = count[0];
            var paraCount = count[1];
            return itemCount == 1 && paraCount == 3;
        }
        /// <summary>
        /// 微信支付的酒店参数是否设置正确，要求酒店对应的子商户号必须设置
        /// </summary>
        /// <returns>true:设置正确，false:设置不正确</returns>
        private bool IsWxHotelParaReady()
        {
            var hotelDb = GetService<DbHotelPmsContext>();
            var count = hotelDb.Database.SqlQuery<int>("SELECT COUNT(*) AS count FROM dbo.pmsPara WHERE hid = @hid AND code = 'POSWxProviderMchID' AND value <>''"
                , new SqlParameter("@hid",CurrentInfo.HotelId)
                ).Single();
            return count == 1;
        }
        /// <summary>
        /// 支付宝支付的通用参数是否设置正确
        /// </summary>
        /// <returns>true:设置正确，false:设置不正确</returns>
        private bool IsAliCommonParaReady(string payAction)
        {
            var commonDb = GetService<DbCommonContext>();
            var count = commonDb.Database.SqlQuery<int>("select count(*) as count from hotel where hid = @hid and itemaction like '%" + payAction + "%' union SELECT COUNT(*) AS count FROM m_v_payPara WHERE code IN ('AlipayPublicKey','alipayServerUrl','alipayProviderID') AND value <> ''", new SqlParameter("@hid", CurrentInfo.HotelId)).ToList();

            var itemCount = count[0];
            var paraCount = count[1];
            return itemCount == 1 && paraCount == 3;
        }
        /// <summary>
        /// 支付宝支付的酒店参数是否设置正确
        /// </summary>
        /// <returns>true:设置正确，false:设置不正确</returns>
        private bool IsAliHotelParaReady()
        {
            var hotelDb = GetService<DbHotelPmsContext>();
            var count = hotelDb.Database.SqlQuery<int>("SELECT COUNT(*) AS count FROM dbo.pmsPara WHERE hid = @hid AND code in ('POSAlipayAppId','POSAlipayPrivateKey','POSAlipayPId') AND value <>''"
                , new SqlParameter("@hid", CurrentInfo.HotelId)
                ).Single();
            return count == 3;
        }
        public ActionResult QueryFolioPayStatu(int? folioPayInfoId)
        {
            if (!folioPayInfoId.HasValue)
            {
                return Json(JsonResultData.Failure("请指定要查询的记录"));
            }
            //取出详细信息，然后进行查询
            try
            {
                var hotelDb = GetService<DbHotelPmsContext>();
                var payInfo = hotelDb.ResFolioPayInfos.SingleOrDefault(w => w.Id == folioPayInfoId.Value);
                if(payInfo == null)
                {
                    return Json(JsonResultData.Failure("要查询的id不存在"));
                }
                if (payInfo.PayAction.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) || payInfo.PayAction.Equals("WxQrcode",StringComparison.OrdinalIgnoreCase))
                {
                    //调用微信查询订单接口进行查询
                    var commonDb = GetService<DbCommonContext>();
                    var pmsParaService = GetService<IPmsParaService>();
                    var commonPayParas = commonDb.M_v_payParas.ToList();
                    var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);
                    var logService = GetService<IPayLogService>();
                    var wxParaInfo = PayServiceBuilder.GetWxPayConfigPara(commonPayParas,hotelPayParas,CurrentInfo.HotelName);
                    var queryService = new PayWxQueryService(wxParaInfo, logService, hotelDb);
                    var resultData = queryService.Query(CurrentInfo.HotelId,(PayProductType)Enum.Parse(typeof(PayProductType),payInfo.ProductType),payInfo.ProductTransId);
                    if (resultData.Success)
                    {
                        payInfo.Status = Services.Enums.ResFolioPayStatus.PaidSuccess;
                        hotelDb.Entry(payInfo).State = System.Data.Entity.EntityState.Modified;
                        hotelDb.SaveChanges();
                    }
                    return Json(resultData);
                }
                if(payInfo.PayAction.Equals("AliQrcode",StringComparison.OrdinalIgnoreCase) || payInfo.PayAction.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase))
                {
                    //调用支付宝查询订单接口进行查询
                    var commonDb = GetService<DbCommonContext>();
                    var pmsParaService = GetService<IPmsParaService>();
                    var commonPayParas = commonDb.M_v_payParas.ToList();
                    var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);
                    var logService = GetService<IPayLogService>();
                    var paraInfo = PayServiceBuilder.GetAliPayConfigPara(commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                    var currentInfo = GetService<ICurrentInfo>();
                    var queryService = new PayAliQueryService(paraInfo, logService, hotelDb,currentInfo);
                    var resultData = queryService.Query(CurrentInfo.HotelId, (PayProductType)Enum.Parse(typeof(PayProductType), payInfo.ProductType), payInfo.ProductTransId);
                    if (resultData.Success)
                    {
                        payInfo.Status = Services.Enums.ResFolioPayStatus.PaidSuccess;
                        hotelDb.Entry(payInfo).State = System.Data.Entity.EntityState.Modified;
                        hotelDb.SaveChanges();
                    }
                    return Json(resultData);
                }
                return Json(JsonResultData.Failure(string.Format("不支持的支付方式{0}，无法查询实际的支付状态", payInfo.PayAction)));
            }catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
    }
}