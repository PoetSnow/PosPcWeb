using System;
using Aop.Api.Request;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 阿里支付宝的查询支付结果服务
    /// </summary>
    public class PayAliQueryService :PayBaseService, IPayQueryService
    {
        public PayAliQueryService(AliPayConfigPara paraInfo,IPayLogService logService,DbHotelPmsContext pmsContext,ICurrentInfo currentInfo)
        {
            _paraInfo = paraInfo;
            _logService = logService;
            _pmsContext = pmsContext;
            _hid = currentInfo.HotelId;
        }
        public JsonResultData Query(string hid, PayProductType productType, string productTransId)
        {
            //转换一下folio的transid格式，以保证长度为32位
            Guid tempTransId;
            if (Guid.TryParse(productTransId, out tempTransId))
            {
                productTransId = tempTransId.ToString("N");
            }
            //商户订单号
            var out_trade_no = PayBaseService.GetAlipayOutTradeNo(hid, productType, productTransId);
            //开始查询
            _logService.Debug(_hid,"PayAliQueryService", "开始查询支付状态：" + out_trade_no);

            _logService.Debug(_hid,"PayAliQueryService", "支付查询开始");
            var client = GetClient(_logService,_hid,_paraInfo);
            var queryBuilder = new AlipayTradeQueryContentBuilder();
            queryBuilder.out_trade_no = out_trade_no;
            var queryRequest = new AlipayTradeQueryRequest();
            queryRequest.BizContent = queryBuilder.BuildJson();
            var response = client.Execute(queryRequest);
            _logService.Debug(_hid,"PayAliQueryService", "详情：" + (response == null ? "response为null" : response.Body));
            var result = FailResult(response);

            if (IsSuccessCode(response))
            {
                var tradeStatus = response.TradeStatus;
                if (tradeStatus == "TRADE_SUCCESS" || tradeStatus == "TRADE_FINISHED")
                {
                    //支付成功
                    var totalFeeByQuery = response.ReceiptAmount;
                    var transactionIdByQuery = response.TradeNo;
                    var timeEndByQuery = response.SendPayDate;

                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara { HotelId = hid, IsPaidSuccess = true, OutTradeNo = productTransId, PaidAmount = Convert.ToDecimal(totalFeeByQuery), PaidTransId = transactionIdByQuery, ProductType = productType });
                    return JsonResultData.Successed("支付成功!");
                } else if (tradeStatus == "WAIT_BUYER_PAY")
                {
                    //用户支付中
                    //不再任何处理，等下再次查询即可
                    return JsonResultData.Failure("正在等待用户支付，请稍候...");
                } else
                {
                    //支付失败
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara { HotelId = hid, IsPaidSuccess = false, OutTradeNo = productTransId, PaidError = response.Msg, ProductType = productType });
                    return JsonResultData.Failure(string.Format("错误代码{0};错误描述:{1}", response.Code, response.Msg));
                }
            }
            else
            {
                //业务查询失败，记录失败原因到日志里面
                _logService.Error(hid, "PayAliQueryService", string.Format("查询支付宝服务商订单状态时遇到业务错误,代码:{0},描述:{1}", response.Code, response.Msg));
                return JsonResultData.Failure(string.Format("查询支付宝服务商订单状态时遇到业务错误,代码:{0},描述:{1}", response.Code, response.Msg));
            }
        }
        private AliPayConfigPara _paraInfo;
        private IPayLogService _logService;
        private DbHotelPmsContext _pmsContext;
        private string _hid;
    }
}
