using System;
using Aop.Api.Request;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 支付宝支付直接退款，用于非入账的退款
    /// </summary>
    public class PayAliRefundDirectly:PayBaseService
    {
        public PayAliRefundDirectly(AliPayConfigPara paraInfo, IPayLogService payLogService, DbHotelPmsContext pmsContext, string hid, string userName)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
            _userName = userName;
        }
        public PayResult DoRefund(PayProductType productType,string payTransId,string refundId,decimal refundAmount,string refundReason)
        {
            if (string.IsNullOrWhiteSpace(_paraInfo.AlipayPublicKey) || string.IsNullOrWhiteSpace(_paraInfo.ServerUrl) || string.IsNullOrWhiteSpace(_paraInfo.SysServiceProviderId))
            {
                throw new AliPayException("支付宝支付的通用参数设置不正确，请与软件开发商联系");
            }
            if (string.IsNullOrWhiteSpace(_paraInfo.AppId) || string.IsNullOrWhiteSpace(_paraInfo.PrivateKey) || string.IsNullOrWhiteSpace(_paraInfo.PID))
            {
                throw new AliPayException("支付宝支付的酒店参数设置不正确，请前往系统参数中设置");
            }

            try
            {
                _payLogService.Debug(_hid, "PayAliRefundDirectly", "开始参数封装");

                var builder = new AlipayTradeRefundContentBuilder();
                builder.out_trade_no = PayBaseService.GetAlipayOutTradeNo(_hid, productType, payTransId);
                builder.out_request_no = refundId;
                builder.refund_amount = refundAmount.ToString("0.##");
                builder.refund_reason = refundReason;

                _payLogService.Debug(_hid, "PayAliRefundDirectly", "退款开始");
                var request = new AlipayTradeRefundRequest();
                request.BizContent = builder.BuildJson();
                var client = GetClient(_payLogService, _hid, _paraInfo);
                var response = client.Execute(request);
                _payLogService.Debug(_hid, "PayAliRefundDirectly", "详情：" + (response == null ? "response为null" : response.Body));
                if (IsSuccessCode(response))
                {
                    var totalFee = response.RefundFee;
                    var transactionId = response.TradeNo;
                    var timeEnd = response.GmtRefundPay;
                    var resultStr = string.Format("交易号：{0};退款时间：{1};退款金额：{2}", transactionId, timeEnd, totalFee);
                    _payLogService.Debug(_hid, "PayAliRefundDirectly", "退款成功：" + resultStr);
                    return new PayResult { IsWaitPay = false, RefNo = transactionId };
                }
                else
                {
                    _payLogService.Debug(_hid, "PayAliRefundDirectly", "退款失败");
                    var msg = FailResult(response);
                    throw new AliPayException(msg);
                }
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayAliRefundDirectly", ex);
                throw ex;
            }
        }
        private AliPayConfigPara _paraInfo;
        private string _hid;
        private IPayLogService _payLogService;
        private DbHotelPmsContext _pmsContext;
        private string _userName;
    }
}
