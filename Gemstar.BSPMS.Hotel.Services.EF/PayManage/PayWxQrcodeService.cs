using System;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.WxProviderPay;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 微信服务商扫码支付，客人扫描二维码进行支付
    /// </summary>
    public class PayWxQrcodeService:PayBaseService
    {
        public PayWxQrcodeService(WxPayConfigPara paraInfo,IPayLogService payLogService,DbHotelPmsContext pmsContext,UrlHelper urlHelper,string hid)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
            _urlHelper = urlHelper;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            if(string.IsNullOrWhiteSpace(_paraInfo.WxProviderAppId) || string.IsNullOrWhiteSpace(_paraInfo.WxProviderMchId) || string.IsNullOrWhiteSpace(_paraInfo.WxProviderKey))
            {
                throw new WxPayException("微信支付的服务商参数设置不正确，请与软件开发商联系");
            }
            if (string.IsNullOrWhiteSpace(_paraInfo.MchID))
            {
                throw new WxPayException("微信支付的酒店对应子商户号设置不正确，请前往系统参数中设置");
            }
            return new PayResult { RefNo = "", IsWaitPay = true };
        }
        public override PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara)
        {
            try
            {
                var paraDic = GetParaDicFromJsonStr(jsonStrPara);

                var subAppId = _paraInfo.AppID;
                var subMchId = _paraInfo.MchID;
                var body = paraDic["body"];
                var itemId = GetValueSafely(paraDic, "itemId");
                var orderAmount = Convert.ToDecimal(paraDic["amount"]);
                string outTradeNo;
                Guid tempOutTrandeNo;
                if(Guid.TryParse(payTransId,out tempOutTrandeNo))
                {
                    outTradeNo = tempOutTrandeNo.ToString("N");
                }else
                {
                    outTradeNo = payTransId;
                }
                //开始下单
                var payData = new WxPayData();
                payData.SetValue("appid", _paraInfo.WxProviderAppId);
                payData.SetValue("mch_id", _paraInfo.WxProviderMchId);
                payData.SetValue("body", body);
                payData.SetValue("out_trade_no", outTradeNo);
                payData.SetValue("total_fee", Convert.ToInt32(orderAmount * 100));
                payData.SetValue("trade_type", "NATIVE");
                payData.SetValue("product_id", outTradeNo);
                var attach = new WxPayAttach
                {
                    HotelId = _hid,
                    ProductType = productType
                };
                payData.SetValue("attach", attach.GetAttachStr());

                var resultData = WxPayApi.UnifiedOrder(payData, _paraInfo,_payLogService,_hid, _paraInfo.WxProviderNotifyUrl);

                if (resultData.GetValue("return_code").ToString() != "SUCCESS")
                {
                    var msg = resultData.GetValue("return_msg").ToString();
                    //更改客账明细状态
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = false,
                        OutTradeNo = payTransId,
                        ProductType = productType,
                        PaidError = msg
                    });
                    throw new WxPayException(msg);
                }
                if (resultData.GetValue("result_code").ToString() != "SUCCESS")
                {
                    var msg = resultData.GetValue("err_code_des").ToString();
                    //更改客账明细状态
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = false,
                        OutTradeNo = payTransId,
                        ProductType = productType,
                        PaidError = msg
                    });
                    throw new WxPayException(msg);
                }

                var qrcodeUrl = resultData.GetValue("code_url").ToString();
                //增加待查询状态的记录，以便稍后查询,并且返回的查询id需要是新增加的记录的主键值，这样以后如果查询的时候需要额外的参数，则只需要修改表结构，增加字段，不再需要修改传到前端页面和页面传回参数的代码，都是根据主键值直接从数据库中获取。比如支付方式等
                var queryTransId = ResFolioService.AddFolioPayInfo(_pmsContext,productType, payTransId, "WxQrcode","",qrcodeUrl,_hid, itemId,orderAmount);
                return new PayAfterResult { Statu = PayStatu.WaitPay, Callback = "folio_WxQrcode_afterConfirm", QueryTransId = queryTransId, QrCodeUrl = qrcodeUrl };
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayWxQrcodeService", ex);
                throw;
            }
        }
        private WxPayConfigPara _paraInfo;
        private string _hid;
        private IPayLogService _payLogService;
        private DbHotelPmsContext _pmsContext;
        private UrlHelper _urlHelper;
    }
}
