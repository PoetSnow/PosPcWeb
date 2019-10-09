using System;
using Aop.Api.Request;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 支付宝支付退款，用于入账的退款
    /// </summary>
    public class PayAliRefundService : PayBaseService
    {
        public PayAliRefundService(AliPayConfigPara paraInfo, IPayLogService payLogService, DbHotelPmsContext pmsContext, string hid, string userName)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
            _userName = userName;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            if (string.IsNullOrWhiteSpace(_paraInfo.AlipayPublicKey) || string.IsNullOrWhiteSpace(_paraInfo.ServerUrl) || string.IsNullOrWhiteSpace(_paraInfo.SysServiceProviderId))
            {
                throw new AliPayException("支付宝支付的通用参数设置不正确，请与软件开发商联系");
            }
            if (string.IsNullOrWhiteSpace(_paraInfo.AppId) || string.IsNullOrWhiteSpace(_paraInfo.PrivateKey) || string.IsNullOrWhiteSpace(_paraInfo.PID))
            {
                throw new AliPayException("支付宝支付的酒店参数设置不正确，请前往系统参数中设置");
            }
            return new PayResult { RefNo = "", IsWaitPay = false };
        }
        /// <summary>
        /// 支付后保存账务
        /// </summary>
        /// <param name="productType">支付产品类型</param>
        /// <param name="payTransId">退款操作生成新的退款账务ID</param>
        /// <param name="jsonStrPara"></param>
        /// <returns></returns>
        public override PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara)
        {

            try
            {
                var paraDic = GetParaDicFromJsonStr(jsonStrPara);
                string originPayTransId = GetValueSafely(paraDic, "originPayTransId");//选中要退款的账务的主键ID
                string refundId = paraDic["refundId"].ToString();//退款操作生成新的退款账务ID
                decimal refundAmount = Convert.ToDecimal(paraDic["refundAmount"].ToString());//退款金额
                string refundReason = paraDic["refundReason"].ToString();//退款原因
                
                _payLogService.Debug(_hid, "PayAliRefundService", "开始参数封装");

                var builder = new AlipayTradeRefundContentBuilder();
                builder.out_trade_no = PayBaseService.GetAlipayOutTradeNo(_hid, productType, originPayTransId);
                builder.out_request_no = refundId;
                builder.refund_amount = refundAmount.ToString("0.##");
                builder.refund_reason = refundReason;

                _payLogService.Debug(_hid, "PayAliRefundService", "退款开始");
                var request = new AlipayTradeRefundRequest();
                request.BizContent = builder.BuildJson();
                var client = GetClient(_payLogService, _hid, _paraInfo);
                var response = client.Execute(request);

                _payLogService.Debug(_hid, "PayAliRefundService", "详情：" + (response == null ? "response为null" : response.Body));
                if (IsSuccessCode(response))
                {
                    var totalFee = response.RefundFee;
                    var transactionId = response.TradeNo;
                    var timeEnd = response.GmtRefundPay;
                    var resultStr = string.Format("交易号：{0};退款时间：{1};退款金额：{2}", transactionId, timeEnd, totalFee);
                    _payLogService.Debug(_hid, "PayAliRefundService", "退款成功：" + resultStr);

                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = true,
                        OutTradeNo = payTransId,
                        PaidAmount = decimal.Parse(totalFee),
                        PaidTransId = transactionId,
                        ProductType = productType
                    });

                    //return new PayResult { IsWaitPay = false, RefNo = transactionId };
                    return new PayAfterResult { Statu = PayStatu.Successed, Callback = "", QueryTransId = "", QrCodeUrl = "" };
                }
                else
                {
                    _payLogService.Debug(_hid, "PayAliRefundService", "退款失败");
                    var msg = response.SubMsg;
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = response.Msg;
                    }
                    //更改客账明细状态
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = false,
                        OutTradeNo = payTransId,
                        ProductType = productType,
                        PaidError = msg
                    });
                    throw new AliPayException(msg);
                }
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayAliRefundService", ex);
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
