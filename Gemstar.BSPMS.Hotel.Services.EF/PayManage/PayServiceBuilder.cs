using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Common.PayManage.WxProviderPay;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Common.Services.EF;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 根据付款支付处理方式创建支付服务实例
    /// </summary>
    public class PayServiceBuilder : IPayServiceBuilder
    {
        public PayServiceBuilder(DbCommonContext commDb, DbHotelPmsContext pmsContext, ICurrentInfo currentInfo, IPayLogService payLogService, UrlHelper urlHelper)
        {
            _commDb = commDb;
            _pmsContext = pmsContext;
            _currentInfo = currentInfo;
            _payLogService = payLogService;
            _urlHelper = urlHelper;
        }
        /// <summary>
        /// 根据付款处理方式代码获取对应的支付服务实例
        /// </summary>
        /// <param name="action">支付处理方式</param>
        /// <param name="commonPayParas">通用的支付参数列表</param>
        /// <param name="hotelPayParas">酒店的支付参数列表</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <returns>对应的支付服务实例</returns>
        public IPayService GetPayService(string action, List<M_v_payPara> commonPayParas, List<PmsPara> hotelPayParas, bool isEnvTest)
        {
            if (action.Equals("credit", StringComparison.OrdinalIgnoreCase))
            {
                //信用卡支付
                return new PayCreditService();
            }
            if (action.Equals("bankCard", StringComparison.OrdinalIgnoreCase))
            {
                //银联卡支付
                return new PayBankCardService();
            }
            if (action.Equals("mbrCard", StringComparison.OrdinalIgnoreCase))
            {
                //会员卡储值支付
                return new PayMbrCardService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId);
            }
            if (action.Equals("mbrCashTicket", StringComparison.OrdinalIgnoreCase))
            {
                //会员现金券
                return new PayMbrCashTicketService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId);
            }
            if (action.Equals("mbrLargess", StringComparison.OrdinalIgnoreCase))
            {
                //会员卡增值金额支付
                return new PayMbrLargessService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId);
            }
            if (action.Equals("mbrCardAndLargess", StringComparison.OrdinalIgnoreCase))
            {
                //会员卡储值增值金额支付
                return new PayMbrCardAndLargessService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId);
            }
            if (action.Equals("roomFolio", StringComparison.OrdinalIgnoreCase))
            {
                //会员卡储值增值金额支付
                return new PayRoomFolioService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId);
            }
            if (action.Equals("corp", StringComparison.OrdinalIgnoreCase))
            {
                //合约单位挂账
                return new PayCorpService(_pmsContext, _currentInfo.HotelId, _currentInfo.UserName);
            }
            if (action.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase))
            {
                //微信服务商刷卡支付，条码支付
                var wxParaInfo = GetWxPayConfigPara(commonPayParas, hotelPayParas, _currentInfo.HotelName);
                return new PayWxBarcodeService(wxParaInfo, _payLogService, _pmsContext, _currentInfo.HotelId);
            }
            if (action.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase))
            {
                //微信服务商扫码支付
                var wxParaInfo = GetWxPayConfigPara(commonPayParas, hotelPayParas, _currentInfo.HotelName);
                return new PayWxQrcodeService(wxParaInfo, _payLogService, _pmsContext, _urlHelper, _currentInfo.HotelId);
            }
            if (action.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase))
            {
                var paraInfo = GetAliPayConfigPara(commonPayParas, hotelPayParas, isEnvTest);
                return new PayAliBarcodeService(paraInfo, _payLogService, _pmsContext, _currentInfo.HotelId, _currentInfo.UserName);
            }
            if (action.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase))
            {
                var paraInfo = GetAliPayConfigPara(commonPayParas, hotelPayParas, isEnvTest);
                return new PayAliQrcodeService(paraInfo, _payLogService, _pmsContext, _currentInfo.HotelId, _currentInfo.UserName);
            }
            if (action.Equals("AliCredit", StringComparison.OrdinalIgnoreCase))
            {
                //阿里信用住支付
                return new PayAliCreditService(_commDb, _pmsContext, _currentInfo.HotelId, _currentInfo.UserName, _currentInfo.ShiftId, isEnvTest);
            }
            return null;
        }
        /// <summary>
        /// 根据付款处理方式代码获取对应的支付退款服务实例
        /// </summary>
        /// <param name="action">支付处理方式</param>
        /// <param name="commonPayParas">通用的支付参数列表</param>
        /// <param name="hotelPayParas">酒店的支付参数列表</param>
        /// <param name="isEnvTest">是否测试环境</param>
        /// <returns>对应的支付服务实例</returns>
        public IPayService GetPayRefundService(string action, List<M_v_payPara> commonPayParas, List<PmsPara> hotelPayParas, bool isEnvTest)
        {
            if (action.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase) || action.Equals("WxQrcode", StringComparison.OrdinalIgnoreCase))
            {
                var wxParaInfo = GetWxPayConfigPara(commonPayParas, hotelPayParas, _currentInfo.HotelName);
                return new PayWxRefundService(wxParaInfo, _payLogService, _pmsContext, _currentInfo.HotelId);
            }
            if (action.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase) || action.Equals("AliQrcode", StringComparison.OrdinalIgnoreCase))
            {
                var paraInfo = GetAliPayConfigPara(commonPayParas, hotelPayParas, isEnvTest);
                return new PayAliRefundService(paraInfo, _payLogService, _pmsContext, _currentInfo.HotelId, _currentInfo.UserName);
            }
            return null;
        }
        public static AliPayConfigPara GetAliPayConfigPara(List<M_v_payPara> commonPayParas, List<PmsPara> hotelPayParas, bool isEnvTest)
        {
            var appIdPara = hotelPayParas.SingleOrDefault(w => w.Code == "POSAlipayAppId");
            var privateKeyPara = hotelPayParas.SingleOrDefault(w => w.Code == "POSAlipayPrivateKey");
            var pidPara = hotelPayParas.SingleOrDefault(w => w.Code == "POSAlipayPId");
            var privateKey = privateKeyPara == null ? "" : privateKeyPara.Value;
            privateKey = privateKey.Replace(" ", "");
            privateKey = privateKey.Replace("\r", "");
            privateKey = privateKey.Replace("\n", "");
            privateKey = privateKey.Replace("-----BEGINRSAPRIVATEKEY-----", "-----BEGIN RSA PRIVATE KEY----- ");
            privateKey = privateKey.Replace("-----ENDRSAPRIVATEKEY-----", " -----END RSA PRIVATE KEY-----");
            var publicKey = hotelPayParas.Single(w => w.Code == "POSAlipayPublicKey").Value;
            publicKey = publicKey.Replace(" ", "");
            publicKey = publicKey.Replace("\r", "");
            publicKey = publicKey.Replace("\n", "");
            publicKey = publicKey.Replace("-----BEGINPUBLICKEY-----", "-----BEGIN PUBLIC KEY----- ");
            publicKey = publicKey.Replace("-----ENDPUBLICKEY-----", " -----END PUBLIC KEY-----");

            var urlParaCode = isEnvTest ? "alipayNotifyUrlTest" : "alipayNotifyUrl";
            return new AliPayConfigPara
            {
                AliPayCallbackUrl = commonPayParas.Single(w => w.Code == urlParaCode).Value,
                AlipayPublicKey = publicKey,
                AppId = appIdPara == null ? "" : appIdPara.Value,
                Charset = commonPayParas.Single(w => w.Code == "alipayCharset").Value,
                PID = pidPara == null ? "" : pidPara.Value,
                PrivateKey = privateKey,
                ServerUrl = commonPayParas.Single(w => w.Code == "alipayServerUrl").Value,
                SignType = hotelPayParas.Single(w => w.Code == "POSAlipaySignType").Value ?? "RSA2",
                SysServiceProviderId = commonPayParas.Single(w => w.Code == "alipayProviderID").Value,
                Version = commonPayParas.Single(w => w.Code == "alipayVersion").Value,
                MapiUrl = commonPayParas.Single(w => w.Code == "alipayMapiUrl").Value
            };
        }

        public static WxPayConfigPara GetWxPayConfigPara(List<M_v_payPara> commonPayParas, List<PmsPara> hotelPayParas, string hotelName)
        {
            var appIdPara = hotelPayParas.SingleOrDefault(w => w.Code == "POSWxProviderAppID");
            var mchIdPara = hotelPayParas.SingleOrDefault(w => w.Code == "POSWxProviderMchID");

            return new WxPayConfigPara
            {
                AppID = appIdPara == null ? "" : appIdPara.Value,
                MchID = mchIdPara == null ? "" : mchIdPara.Value,
                IP = GetLocalIP(),
                ProxyUrl = commonPayParas.Single(w => w.Code == "WxProviderProxyUrl").Value,
                ReportLevenl = Convert.ToInt32(commonPayParas.Single(w => w.Code == "WxProviderReportLevenl").Value),
                ResortName = hotelName,
                WxProviderAppId = commonPayParas.Single(w => w.Code == "WxProviderAppId").Value,
                WxProviderKey = commonPayParas.Single(w => w.Code == "WxProviderKey").Value,
                WxProviderMchId = commonPayParas.Single(w => w.Code == "WxProviderMchId").Value,
                WxProviderNotifyUrl = commonPayParas.Single(w => w.Code == "WxProviderNotifyUrl").Value,
                WxRefundTransferUrl = commonPayParas.Single(w => w.Code == "WxRefundTransferUrl").Value,
            };
        }

        public static string GetLocalIP()
        {
            var ipStr = "8.8.8.8";
            var ipaddress = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ipaddress)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipStr = ip.ToString();
                }
            }
            return ipStr;
        }
        private DbCommonContext _commDb;
        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;
        private IPayLogService _payLogService;
        private UrlHelper _urlHelper;
    }
}