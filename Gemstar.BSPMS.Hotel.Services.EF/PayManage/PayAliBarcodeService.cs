using System;
using Aop.Api.Request;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 支付宝刷卡支付，条码支付
    /// </summary>
    public class PayAliBarcodeService:PayBaseService
    {
        public PayAliBarcodeService(AliPayConfigPara paraInfo,IPayLogService payLogService,DbHotelPmsContext pmsContext,string hid,string userName)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
            _userName = userName;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            if(string.IsNullOrWhiteSpace(_paraInfo.AlipayPublicKey) || string.IsNullOrWhiteSpace(_paraInfo.ServerUrl) || string.IsNullOrWhiteSpace(_paraInfo.SysServiceProviderId))
            {
                throw new AliPayException("支付宝支付的通用参数设置不正确，请与软件开发商联系");
            }
            if (string.IsNullOrWhiteSpace(_paraInfo.AppId) || string.IsNullOrWhiteSpace(_paraInfo.PrivateKey) || string.IsNullOrWhiteSpace(_paraInfo.PID))
            {
                throw new AliPayException("支付宝支付的酒店参数设置不正确，请前往系统参数中设置");
            }
            return new PayResult { RefNo = "", IsWaitPay = true };
        }
        public override PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara)
        {
            try
            {
                var paraDic = GetParaDicFromJsonStr(jsonStrPara);

                _payLogService.Debug(_hid, "AlipayBarcodePay", "开始参数封装");
                //扫码枪扫描到的用户手机钱包中的付款条码
                var builder = new AlipayTradePayContentBuilder();
                //线上联调时，请输入真实的外部订单号。
                builder.out_trade_no = GetAlipayOutTradeNo(_hid,productType, payTransId);
                builder.scene = "bar_code";
                builder.auth_code = paraDic["authCode"].ToString();
                builder.total_amount = Convert.ToDouble(paraDic["amount"]).ToString("0.##");
                builder.operator_id = _userName;
                builder.subject = paraDic["subject"].ToString();
                //builder.store_id = _hid;
                //builder.seller_id = strArray[7];       //可以是具体的收款账号
                //传入商品信息详情
                //List<GoodsInfo> gList = new List<GoodsInfo>();

                //GoodsInfo goods = new GoodsInfo();
                //goods.goods_id = "goods id";
                //goods.goods_name = "goods name";
                //goods.price = "0.01";
                //goods.quantity = "1";
                //gList.Add(goods);
                //builder.goods_detail = gList;
                //扩展参数
                //系统商接入可以填此参数用作返佣
                var exParam = new ExtendParams();
                exParam.sys_service_provider_id = _paraInfo.SysServiceProviderId;
                builder.extend_params = exParam;
                var serviceClient = GetClient(_payLogService, _hid, _paraInfo);
                var request = new AlipayTradePayRequest();
                request.BizContent = builder.BuildJson();
                var payResult = serviceClient.Execute(request);
                _payLogService.Debug(_hid, "AlipayBarcodePay", "详情：" + (payResult == null ? "response为null" : payResult.Body));
                //不管支付结果，直接调用一次查询，以查询的结果为准
                var endDate = DateTime.Now.AddSeconds(60);
                var queryDate = DateTime.Now.AddSeconds(2);
                var queryBuilder = new AlipayTradeQueryContentBuilder();
                queryBuilder.out_trade_no = builder.out_trade_no;
                var queryRequest = new AlipayTradeQueryRequest();
                queryRequest.BizContent = queryBuilder.BuildJson();
                while (DateTime.Now < endDate)
                {
                    if (DateTime.Now < queryDate)
                    {
                        continue;
                    }
                    queryDate = DateTime.Now.AddSeconds(2);
                    var queryResponse = serviceClient.Execute(queryRequest);
                    _payLogService.Debug(_hid,"AlipayBarcodePay", "查询结果详情：" + (queryResponse == null ? "queryResponse为null" : queryResponse.Body));
                    if (IsSuccessCode(queryResponse))
                    {
                        if (queryResponse.TradeStatus == "TRADE_SUCCESS" || queryResponse.TradeStatus == "TRADE_FINISHED")
                        {
                            var totalFee = queryResponse.ReceiptAmount;
                            var transactionId = queryResponse.TradeNo;
                            var timeEnd = DateTime.Parse(queryResponse.SendPayDate).ToString("yyyyMMddHHmmss");
                            var resultStr = string.Format("交易号：{0};支付时间：{1};支付金额：{2}", transactionId, timeEnd, totalFee);
                            PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                            {
                                HotelId = _hid,
                                IsPaidSuccess = true,
                                OutTradeNo = payTransId,
                                PaidAmount = decimal.Parse(totalFee),
                                PaidTransId = transactionId,
                                ProductType = productType
                            });
                            _payLogService.Debug(_hid, "AlipayBarcodePay", "支付成功：" + resultStr);
                            return new PayAfterResult { Statu = PayStatu.Successed, Callback = "", QueryTransId = "", QrCodeUrl = "" };
                        } else if (queryResponse.TradeStatus == "TRADE_CLOSED")
                        {
                            //更改客账明细状态
                            var msg = "未付款交易超时关闭，或支付完成后全额退款";
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
                    } else if (queryResponse.Code == ResultCode.ServiceUnavailable || queryResponse.Code == ResultCode.WaitingUser)
                    {
                        continue;
                    } else
                    {
                        var msg = FailResult(queryResponse);
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
                throw new AliPayException("查询时间已经到达，仍没有确切结果，请稍后再查询");
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayAliBarcodeService", ex);
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
