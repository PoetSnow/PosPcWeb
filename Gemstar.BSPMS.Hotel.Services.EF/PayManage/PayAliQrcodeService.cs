using System;
using Aop.Api.Request;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.PayManage.AliProviderPay;
using Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 微信服务商扫码支付，客人扫描二维码进行支付
    /// </summary>
    public class PayAliQrcodeService:PayBaseService
    {
        public PayAliQrcodeService(AliPayConfigPara paraInfo,IPayLogService payLogService,DbHotelPmsContext pmsContext,string hid,string userName)
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
            return new PayResult { RefNo = "", IsWaitPay = true };
        }
        public override PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara)
        {
            try
            {
                var paraDic = GetParaDicFromJsonStr(jsonStrPara);

                _payLogService.Debug(_hid, "AlipayQrcodePay", "开始参数封装");
                var builder = new AlipayTradePrecreateContentBuilder();
                //线上联调时，请输入真实的外部订单号。
                builder.out_trade_no = GetAlipayOutTradeNo(_hid, productType, payTransId);
                builder.total_amount = Convert.ToDouble(paraDic["amount"]).ToString("0.##");
                builder.operator_id = _userName;
                builder.subject = paraDic["subject"].ToString();
                //builder.body = "支付宝验证码：" + Settings.PID;
                //builder.body = "订单描述";
                //builder.store_id = "test store id";    //很重要的参数，可以用作之后的营销     
                //builder.seller_id = strArray[5];       //可以是具体的收款账号

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
                var client = GetClient(_payLogService, _hid, _paraInfo);
                var request = new AlipayTradePrecreateRequest();
                _payLogService.Debug(_hid, "AlipayQrcodePay", "设置异步通知Url：" + _paraInfo.AliPayCallbackUrl);
                request.SetNotifyUrl(_paraInfo.AliPayCallbackUrl);
                request.BizContent = builder.BuildJson();
                _payLogService.Debug(_hid, "AlipayQrcodePay", "调用支付接口");
                var response = client.Execute(request);
                _payLogService.Debug(_hid, "AlipayQrcodePay", "详情：" + (response == null ? "response为null" : response.Body));
                if (IsSuccessCode(response))
                {
                    var qrcodeUrl = response.QrCode;
                    //增加待查询状态的记录，以便稍后查询,并且返回的查询id需要是新增加的记录的主键值，这样以后如果查询的时候需要额外的参数，则只需要修改表结构，增加字段，不再需要修改传到前端页面和页面传回参数的代码，都是根据主键值直接从数据库中获取。比如支付方式等
                    var queryTransId = ResFolioService.AddFolioPayInfo(_pmsContext, productType, payTransId, "AliQrcode", "", qrcodeUrl, _hid, GetValueSafely(paraDic, "itemId"), Convert.ToDecimal(GetValueSafely(paraDic, "amount")));
                    return new PayAfterResult { Statu = PayStatu.WaitPay, Callback = "folio_AliQrcode_afterConfirm", QueryTransId = queryTransId, QrCodeUrl = qrcodeUrl };
                } else
                {
                    _payLogService.Debug(_hid, "AlipayQrcodePay", "支付失败");
                    var msg = FailResult(response);
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
            } catch (Exception ex)
            {
                _payLogService.Error(_hid, "AlipayQrcodePay", ex);
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
