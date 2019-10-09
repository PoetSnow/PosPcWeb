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
    /// 微信支付退款，用于入账的退款
    /// </summary>
    public class PayWxRefundService : PayBaseService
    {
        public PayWxRefundService(WxPayConfigPara paraInfo, IPayLogService payLogService, DbHotelPmsContext pmsContext, string hid)
        {
            _paraInfo = paraInfo;
            _payLogService = payLogService;
            _hid = hid;
            _pmsContext = pmsContext;
        }

        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            if (string.IsNullOrWhiteSpace(_paraInfo.WxProviderAppId) || string.IsNullOrWhiteSpace(_paraInfo.WxProviderMchId) || string.IsNullOrWhiteSpace(_paraInfo.WxProviderKey))
            {
                throw new WxPayException("微信支付的服务商参数设置不正确，请与软件开发商联系");
            }
            if (string.IsNullOrWhiteSpace(_paraInfo.MchID))
            {
                throw new WxPayException("微信支付的酒店对应子商户号设置不正确，请前往系统参数中设置");
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
                string outTradeNo = GetValueSafely(paraDic, "outTradeNo");//选中要退款的账务的主键ID
                string outRefundNo = GetValueSafely(paraDic, "outRefundNo");//退款操作生成新的退款账务ID
                string totalFee = GetValueSafely(paraDic, "totalFee");//原始总金额
                string refundFee = GetValueSafely(paraDic, "refundFee");//退款金额
                string opUserId = GetValueSafely(paraDic, "opUserId");//操作员
                
                var payData = new WxPayData();
                payData.SetValue("out_trade_no", outTradeNo);
                payData.SetValue("out_refund_no", outRefundNo);
                payData.SetValue("total_fee", Convert.ToInt32(Convert.ToDecimal(totalFee) * 100));
                payData.SetValue("refund_fee", Convert.ToInt32(Convert.ToDecimal(refundFee) * 100));
                payData.SetValue("op_user_id", opUserId);

                var result = WxPayApi.Refund(payData, _paraInfo, _payLogService, _hid);
                //根据返回消息进行处理
                if (result.GetValue("return_code").ToString() == "SUCCESS")
                {
                    //通信成功，则判断业务结果
                    if (result.GetValue("result_code").ToString() == "SUCCESS")
                    {
                        //退款申请成功，具体退款状态需要另外查询，零钱的退款需要20分钟左右，其他的需要三个工作日
                        var totalFeeByQuery = result.GetValue("refund_fee").ToString();

                        if (result.IsSet("settlement_refund_fee"))
                        {
                            totalFeeByQuery = result.GetValue("settlement_refund_fee").ToString();
                        }
                        var transactionIdByQuery = result.GetValue("refund_id").ToString();
                        var timeEndByQuery = DateTime.Now.ToString("yyyyMMddHHmmss");
                        _payLogService.Debug(_hid, "PayWxBarcodeService", string.Format("退款申请成功，退款单号:{0},退款时间:{1},退款金额:{2}", transactionIdByQuery, timeEndByQuery, totalFeeByQuery));

                        PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                        {
                            HotelId = _hid,
                            IsPaidSuccess = true,
                            OutTradeNo = payTransId,
                            PaidAmount = decimal.Parse(totalFeeByQuery),
                            PaidTransId = transactionIdByQuery,
                            ProductType = productType
                        });

                        //return new PayResult { IsWaitPay = false, RefNo = transactionIdByQuery };
                        return new PayAfterResult { Statu = PayStatu.Successed, Callback = "", QueryTransId = "", QrCodeUrl = "" };
                    }
                    else
                    {
                        //失败，记录失败原因到日志里面
                        var logStr = string.Format("申请微信服务商退款时遇到业务错误,代码:{0},描述:{1}", result.GetValue("err_code"), result.GetValue("err_code_des"));
                        _payLogService.Debug(_hid, "PayWxBarcodeService", logStr);

                        //更改客账明细状态
                        PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                        {
                            HotelId = _hid,
                            IsPaidSuccess = false,
                            OutTradeNo = payTransId,
                            ProductType = productType,
                            PaidError = logStr
                        });

                        throw new WxPayException(logStr);
                    }
                }
                else
                {
                    //通信失败，直接记录失败原因到日志里面
                    var logStr = string.Format("申请微信服务商退款时遇到通信错误:{0}", result.GetValue("return_msg"));
                    _payLogService.Debug(_hid, "PayWxBarcodeService", logStr);

                    //更改客账明细状态
                    PayCallbackUpdate.UpdateProductStatu(_pmsContext, new PayCallbackPara
                    {
                        HotelId = _hid,
                        IsPaidSuccess = false,
                        OutTradeNo = payTransId,
                        ProductType = productType,
                        PaidError = logStr
                    });

                    throw new WxPayException(logStr);
                }
            }
            catch (Exception ex)
            {
                _payLogService.Error(_hid, "PayWxBarcodeService", ex);
                throw new WxPayException(ex.Message, ex);
            }
        }

        private WxPayConfigPara _paraInfo;
        private string _hid;
        private IPayLogService _payLogService;
        private DbHotelPmsContext _pmsContext;
    }
}
