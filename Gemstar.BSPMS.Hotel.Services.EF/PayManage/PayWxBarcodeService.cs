using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.WxProviderPay;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 微信服务商刷卡支付，条码支付
    /// </summary>
    public class PayWxBarcodeService:PayBaseService
    {
        public PayWxBarcodeService(WxPayConfigPara paraInfo,IPayLogService payLogService,DbHotelPmsContext pmsContext,string hid)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
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
                var outTradeNo = payTransId;
                var orderAmount = Convert.ToDecimal(paraDic["amount"]);
                var authCode = paraDic["barcode"];
                var itemId = GetValueSafely(paraDic, "itemId");
                //开始下单
                var payData = new WxPayData();
                payData.SetValue("body", body);
                payData.SetValue("out_trade_no", outTradeNo);
                payData.SetValue("total_fee", Convert.ToInt32(orderAmount * 100));
                payData.SetValue("auth_code", authCode);
                var attach = new WxPayAttach
                {
                    HotelId = _hid,
                    ProductType = productType
                };
                payData.SetValue("attach", attach.GetAttachStr());

                var resultData = WxPayApi.Micropay(payData, _paraInfo,_payLogService,_hid);

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
                    //如果错误码是支付结果未知状态的，则循环来获取订单状态，直到订单状态为确认状态的，再返回相应结果
                    var errCode = resultData.GetValue("err_code").ToString();
                    if (errCode.Equals("SYSTEMERROR", StringComparison.OrdinalIgnoreCase) || errCode.Equals("BANKERROR", StringComparison.OrdinalIgnoreCase) || errCode.Equals("USERPAYING", StringComparison.OrdinalIgnoreCase))
                    {
                        _payLogService.Debug(_hid, "PayWxBarcodeService", string.Format("条码支付后返回结果未知的错误码:{0}，等待稍后订单查询到确切状态后再行返回", errCode));
                        //增加待查询状态的记录，以便稍后查询,并且返回的查询id需要是新增加的记录的主键值，这样以后如果查询的时候需要额外的参数，则只需要修改表结构，增加字段，不再需要修改传到前端页面和页面传回参数的代码，都是根据主键值直接从数据库中获取。比如支付方式等
                        var queryTransId = ResFolioService.AddFolioPayInfo(_pmsContext,productType, payTransId, "WxBarcode","", "",_hid,itemId,orderAmount);
                        return new PayAfterResult { Statu = PayStatu.WaitPay,Callback = "folio_WxBarcode_afterConfirm",QueryTransId = queryTransId, QrCodeUrl = "" };
                    }
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
                else
                {
                    var totalFee = resultData.GetValue("total_fee").ToString();
                    var transactionId = resultData.GetValue("transaction_id").ToString();
                    var timeEnd = resultData.GetValue("time_end").ToString();
                    var resultStr = string.Format("{0}|{1}|{2}", transactionId, timeEnd, totalFee);
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = true,
                        OutTradeNo = payTransId,
                        PaidAmount = decimal.Parse(totalFee),
                        PaidTransId = transactionId,
                        ProductType = productType
                    });
                    return new PayAfterResult { Statu = PayStatu.Successed, Callback = "", QueryTransId = "", QrCodeUrl = "" };
                }
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayWxBarcodeService", ex);
                throw;
            }
        }
        private WxPayConfigPara _paraInfo;
        private string _hid;
        private IPayLogService _payLogService;
        private DbHotelPmsContext _pmsContext;
    }
}
