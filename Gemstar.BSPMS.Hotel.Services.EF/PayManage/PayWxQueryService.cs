using System;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.WxProviderPay;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 微信的查询支付结果服务
    /// </summary>
    public class PayWxQueryService : IPayQueryService
    {
        public PayWxQueryService(WxPayConfigPara paraInfo,IPayLogService logService,DbHotelPmsContext pmsContext)
        {
            _paraInfo = paraInfo;
            _logService = logService;
            _pmsContext = pmsContext;
        }
        public JsonResultData Query(string hid, PayProductType productType, string productTransId)
        {
            //转换一下folio的transid格式，以保证长度为32位
            Guid tempTransId;
            if (Guid.TryParse(productTransId, out tempTransId))
            {
                productTransId = tempTransId.ToString("N");
            }
            var result = WxPayApi.OrderQuery(_paraInfo, productTransId, _logService,hid);
            //根据返回消息进行处理
            if (result.GetValue("return_code").ToString() == "SUCCESS")
            {
                //通信成功，则判断业务结果
                if (result.GetValue("result_code").ToString() == "SUCCESS")
                {
                    //业务结果返回成功，判断支付状态
                    var tradeState = result.GetValue("trade_state").ToString();
                    _logService.Debug(hid,"PayWxQueryService", string.Format("查询到的订单支付状态:{0}", tradeState));
                    var attachStr = result.GetValue("attach")==null ? "" : result.GetValue("attach").ToString();
                    var attach = WxPayAttach.ParseAttrachStr(attachStr,hid);

                    if (tradeState == "SUCCESS")
                    {
                        //支付成功
                        var totalFeeByQuery = result.GetValue("total_fee").ToString();
                        var transactionIdByQuery = result.GetValue("transaction_id").ToString();
                        var timeEndByQuery = result.GetValue("time_end").ToString();

                        PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara { HotelId = attach.HotelId, IsPaidSuccess = true, OutTradeNo = productTransId, PaidAmount = Convert.ToDecimal(totalFeeByQuery), PaidTransId = transactionIdByQuery, ProductType = attach.ProductType });
                        var resultStrByQuery = string.Format("{0}|{1}|{2}", transactionIdByQuery, timeEndByQuery, totalFeeByQuery);
                        return JsonResultData.Successed("支付成功!");                        
                    }
                    else if (tradeState == "USERPAYING" || tradeState == "NOTPAY" || tradeState == "SYSTEMERROR" || tradeState == "BANKERROR")
                    {
                        //用户支付中
                        //NOTPAY是指打印出了二维码，但客人还没有扫描，但接下来客人有可能会继续扫的，所以等下继续查询状态
                        //不再任何处理，等下再次查询即可
                        return JsonResultData.Failure("正在等待用户支付，请稍候...");
                    }
                    else
                    {
                        //支付失败
                        PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara { HotelId = attach.HotelId, IsPaidSuccess = false, OutTradeNo = productTransId, PaidError = result.GetValue("trade_state_desc").ToString(), ProductType = attach.ProductType });
                        return JsonResultData.Failure(string.Format("错误代码{0};错误描述:{1}", tradeState, result.GetValue("trade_state_desc")));
                    }
                }
                else
                {
                    //业务查询失败，记录失败原因到日志里面
                    _logService.Error(hid,"PayWxQueryService", string.Format("查询微信服务商订单状态时遇到业务错误,代码:{0},描述:{1}", result.GetValue("err_code"), result.GetValue("err_code_des")));
                    return JsonResultData.Failure(string.Format("查询微信服务商订单状态时遇到业务错误,代码:{0},描述:{1}", result.GetValue("err_code"), result.GetValue("err_code_des")));
                }
            }
            else
            {
                //通信失败，直接记录失败原因到日志里面
                _logService.Error(hid,"PayWxQueryService", string.Format("查询微信服务商订单状态时遇到通信错误:{0}", result.GetValue("return_msg")));
                return JsonResultData.Failure(string.Format("查询微信服务商订单状态时遇到通信错误:{0}", result.GetValue("return_msg")));
            }
        }
        private WxPayConfigPara _paraInfo;
        private IPayLogService _logService;
        private DbHotelPmsContext _pmsContext;
    }
}
