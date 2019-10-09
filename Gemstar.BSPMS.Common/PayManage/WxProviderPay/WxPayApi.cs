using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    /// <summary>
    /// 微信支付API
    /// </summary>
    public class WxPayApi
    {
        #region 提交被扫支付API
        /// <summary>
        /// 提交被扫支付API
        /// 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
        /// 由商户收银台或者商户后台调用该接口发起支付。
        /// </summary>
        /// <param name="inputObj">提交给被扫支付API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="logService">日志记录服务实例</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>成功时返回调用结果，其他抛异常</returns>
        public static WxPayData Micropay(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 10)
        {
            string url = "https://api.mch.weixin.qq.com/pay/micropay";
            //检测必填参数
            if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数body！");
            }
            else if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("auth_code"))
            {
                throw new WxPayException("提交被扫支付API接口中，缺少必填参数auth_code！");
            }

            inputObj.SetValue("spbill_create_ip", paraInfo.IP);//终端ip
            inputObj.SetValue("appid", paraInfo.WxProviderAppId);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);//商户号
            inputObj.SetValue("device_info", paraInfo.ResortName);
            if (!string.IsNullOrWhiteSpace(paraInfo.AppID))
            {
                inputObj.SetValue("sub_appid", paraInfo.AppID);//子公众账号ID
            }
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);//子商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            logService.Debug(hid, "WxPayApi", "MicroPay request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);//调用HTTP通信接口以提交数据到API
            logService.Debug(hid, "WxPayApi", "MicroPay response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 查询订单
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="folioTransId">客账明细id</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>成功时返回订单查询结果，其他抛异常</returns>
        public static WxPayData OrderQuery(WxPayConfigPara paraInfo, string folioTransId, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";

            var inputObj = new WxPayData();
            inputObj.SetValue("appid", paraInfo.WxProviderAppId);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);//商户号
            inputObj.SetValue("sub_appid", paraInfo.AppID);//子公众账号ID
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);//子商户号
            inputObj.SetValue("out_trade_no", folioTransId);
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");
            }

            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            logService.Debug(hid, "WxPayApi", "OrderQuery request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);//调用HTTP通信接口提交数据
            logService.Debug(hid, "WxPayApi", "OrderQuery response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 撤销订单API接口
        /// <summary>
        /// 撤销订单API接口
        /// </summary>
        /// <param name="inputObj">提交给撤销订单API接口的参数，out_trade_no和transaction_id必填一个</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">接口超时时间</param>
        /// <returns>成功时返回API调用结果，其他抛异常</returns>
        public static WxPayData Reverse(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("撤销订单API接口中，参数out_trade_no和transaction_id必须填写一个！");
            }

            inputObj.SetValue("appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            logService.Debug(hid, "WxPayApi", "Reverse request : " + xml);

            string response = HttpService.Post(xml, url, true, timeOut, paraInfo, logService, hid, true);

            logService.Debug(hid, "WxPayApi", "Reverse response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 申请退款
        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="inputObj">提交给申请退款API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>成功时返回接口调用结果，其他抛异常</returns>
        public static WxPayData Refund(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid = "000000", int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
            }
            else if (!inputObj.IsSet("out_refund_no"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数out_refund_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("refund_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数refund_fee！");
            }
            //else if (!inputObj.IsSet("op_user_id"))
            //{
            //    throw new WxPayException("退款申请接口中，缺少必填参数op_user_id！");
            //}

            inputObj.SetValue("appid", paraInfo.WxProviderAppId);
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);
            inputObj.SetValue("sub_appid", paraInfo.AppID);
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名

            string xml = inputObj.ToXml();
            //如果参数中的中转接口地址有值，则优先使用中转接口地址进行退款操作，以避免出现.net直接退款时报证书错误，无法建立安全连接的问题
            if (!string.IsNullOrWhiteSpace(paraInfo.WxRefundTransferUrl)) {
                try {
                    xml = string.Format("xmlData={0}",HttpUtility.UrlEncode(xml));
                    logService.Debug(hid, "WxPayApi_java", "Refund request : " + xml);
                    string javaResponse = HttpService.Post(xml:xml, url:paraInfo.WxRefundTransferUrl, isUseCert:false,timeout: timeOut,paraInfo: paraInfo,logService: logService,hid: hid,provider: true,contentType: "application/x-www-form-urlencoded");//调用HTTP通信接口提交数据到API
                    logService.Debug(hid, "WxPayApi_java", "Refund response : " + javaResponse);
                    //将xml格式的结果转换为对象以返回
                    WxPayData javaResult = new WxPayData();
                    javaResult.FromXml(javaResponse);
                    return javaResult;
                    //正常退款成功时，直接返回结果，后面的正常退款操作将不会执行
                } catch(Exception ex) {
                    //报错时，仅记录一下日志，然后继续下面的直接退款操作
                    logService.Error(hid, "WxPayApi_java", ex);
                }
            }
            var start = DateTime.Now;

            logService.Debug(hid, "WxPayApi", "Refund request : " + xml);
            string response = HttpService.Post(xml, url, true, timeOut, paraInfo, logService, hid, true);//调用HTTP通信接口提交数据到API
            logService.Debug(hid, "WxPayApi", "Refund response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 查询退款
        /// <summary>
        /// 查询退款
        /// 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
        /// 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
        /// out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
        /// </summary>
        /// <param name="inputObj">提交给查询退款API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">接口超时时间</param>
        /// <returns>成功时返回，其他抛异常</returns>
        public static WxPayData RefundQuery(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/refundquery";
            //检测必填参数
            if (!inputObj.IsSet("out_refund_no") && !inputObj.IsSet("out_trade_no") &&
                !inputObj.IsSet("transaction_id") && !inputObj.IsSet("refund_id"))
            {
                throw new WxPayException("退款查询接口中，out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个！");
            }

            inputObj.SetValue("appid", paraInfo.WxProviderAppId);
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);
            inputObj.SetValue("sub_appid", paraInfo.AppID);
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名

            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            logService.Debug(hid, "WxPayApi", "RefundQuery request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);//调用HTTP通信接口以提交数据到API
            logService.Debug(hid, "WxPayApi", "RefundQuery response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 下载对账单
        /// <summary>
        /// 下载对账单
        /// </summary>
        /// <param name="inputObj">提交给下载对账单API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">接口超时时间</param>
        /// <returns>成功时返回，其他抛异常</returns>
        public static WxPayData DownloadBill(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/downloadbill";
            //检测必填参数
            if (!inputObj.IsSet("bill_date"))
            {
                throw new WxPayException("对账单接口中，缺少必填参数bill_date！");
            }

            inputObj.SetValue("appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名

            string xml = inputObj.ToXml();

            logService.Debug(hid, "WxPayApi", "DownloadBill request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);//调用HTTP通信接口以提交数据到API
            logService.Debug(hid, "WxPayApi", "DownloadBill result : " + response);

            WxPayData result = new WxPayData();
            //若接口调用失败会返回xml格式的结果
            if (response.Substring(0, 5) == "<xml>")
            {
                result.FromXml(response);
            }
            //接口调用成功则返回非xml格式的数据
            else
                result.SetValue("result", response);

            return result;
        }
        #endregion

        #region 转换短链接
        /// <summary>
        /// 转换短链接
        /// 该接口主要用于扫码原生支付模式一中的二维码链接转成短链接(weixin://wxpay/s/XXXXXX)，
        /// 减小二维码数据量，提升扫描速度和精确度。
        /// </summary>
        /// <param name="inputObj">提交给转换短连接API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">接口超时时间</param>
        /// <returns>成功时返回，其他抛异常</returns>
        public static WxPayData ShortUrl(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/tools/shorturl";
            //检测必填参数
            if (!inputObj.IsSet("long_url"))
            {
                throw new WxPayException("需要转换的URL，签名用原串，传输需URL encode！");
            }

            inputObj.SetValue("appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串	
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            logService.Debug(hid, "WxPayApi", "ShortUrl request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);
            logService.Debug(hid, "WxPayApi", "ShortUrl response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 统一下单
        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="inputObj">提交给统一下单API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="notifyUrl">支付结果通知地址</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>成功时返回，其他抛异常</returns>
        public static WxPayData UnifiedOrder(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, string notifyUrl, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //\(^o^)/~ 非微信浏览器h5支付必填参数
            if (inputObj.IsSet("trade_type").ToString() == "MWEB")
            {
                if (!inputObj.IsSet("scene_info"))
                {
                    throw new WxPayException("H5支付缺少统一支付接口必填参数scene_info！");
                }
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid") && !inputObj.IsSet("sub_openid"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid或者sub_openid！trade_type为JSAPI时，openid和sub_openid为必填参数，必须二选其一！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为NATIVE时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", notifyUrl);//异步通知url
            }

            inputObj.SetValue("sub_appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("spbill_create_ip", paraInfo.IP);//终端ip	  	    
            inputObj.SetValue("device_info", paraInfo.ResortName);
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));
            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            logService.Debug(hid, "WxPayApi", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);
            logService.Debug(hid, "WxPayApi", "UnfiedOrder response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 关闭订单
        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="inputObj">提交给关闭订单API的参数</param>
        /// <param name="paraInfo">配置参数实例，其中包含所有需要的配置参数值</param>
        /// <param name="timeOut">接口超时时间</param>
        /// <returns>成功时返回，其他抛异常</returns>
        public static WxPayData CloseOrder(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/closeorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("关闭订单接口中，out_trade_no必填！");
            }

            inputObj.SetValue("appid", paraInfo.WxProviderAppId);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);//商户号
            inputObj.SetValue("sub_appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串		
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名
            string xml = inputObj.ToXml();

            var start = DateTime.Now;//请求开始时间

            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

            ReportCostTime(url, timeCost, result, paraInfo, logService, hid);//测速上报

            return result;
        }
        #endregion

        #region 测速上报
        /// <summary>
        /// 测速上报
        /// </summary>
        /// <param name="interfaceUrl"> 接口URL</param>
        /// <param name="timeCost">接口耗时</param>
        /// <param name="inputObj">inputObj参数数组</param>
        /// <param name="paraInfo">支付信息实例对象</param>
        private static void ReportCostTime(string interfaceUrl, int timeCost, WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid)
        {
            //如果不需要进行上报
            if (paraInfo.ReportLevenl == 0)
            {
                return;
            }

            //如果仅失败上报
            if (paraInfo.ReportLevenl == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
             inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            {
                return;
            }

            //上报逻辑
            WxPayData data = new WxPayData();
            data.SetValue("interface_url", interfaceUrl);
            data.SetValue("execute_time_", timeCost);
            //返回状态码
            if (inputObj.IsSet("return_code"))
            {
                data.SetValue("return_code", inputObj.GetValue("return_code"));
            }
            //返回信息
            if (inputObj.IsSet("return_msg"))
            {
                data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            }
            //业务结果
            if (inputObj.IsSet("result_code"))
            {
                data.SetValue("result_code", inputObj.GetValue("result_code"));
            }
            //错误代码
            if (inputObj.IsSet("err_code"))
            {
                data.SetValue("err_code", inputObj.GetValue("err_code"));
            }
            //错误代码描述
            if (inputObj.IsSet("err_code_des"))
            {
                data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            }
            //商户订单号
            if (inputObj.IsSet("out_trade_no"))
            {
                data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            }
            //设备号
            if (inputObj.IsSet("device_info"))
            {
                data.SetValue("device_info", inputObj.GetValue("device_info"));
            }

            try
            {
                Report(data, paraInfo, logService, hid);
            }
            catch
            {
                //不做任何处理
            }
        }

        /// <summary>
        /// 测速上报接口实现
        /// </summary>
        /// <param name="inputObj">提交给测速上报接口的参数</param>
        /// <param name="paraInfo">配置参数实例，包含所有需要的参数信息</param>
        /// <param name="timeOut">测速上报接口超时时间</param>
        /// <returns>成功时返回测速上报接口返回的结果，其他抛异常</returns>
        public static WxPayData Report(WxPayData inputObj, WxPayConfigPara paraInfo, IPayLogService logService, string hid, int timeOut = 1)
        {
            string url = "https://api.mch.weixin.qq.com/payitil/report";
            //检测必填参数
            if (!inputObj.IsSet("interface_url"))
            {
                throw new WxPayException("接口URL，缺少必填参数interface_url！");
            }
            if (!inputObj.IsSet("return_code"))
            {
                throw new WxPayException("返回状态码，缺少必填参数return_code！");
            }
            if (!inputObj.IsSet("result_code"))
            {
                throw new WxPayException("业务结果，缺少必填参数result_code！");
            }
            if (!inputObj.IsSet("user_ip"))
            {
                throw new WxPayException("访问接口IP，缺少必填参数user_ip！");
            }
            if (!inputObj.IsSet("execute_time_"))
            {
                throw new WxPayException("接口耗时，缺少必填参数execute_time_！");
            }

            inputObj.SetValue("appid", paraInfo.WxProviderAppId);//公众账号ID
            inputObj.SetValue("mch_id", paraInfo.WxProviderMchId);//商户号
            inputObj.SetValue("sub_appid", paraInfo.AppID);//公众账号ID
            inputObj.SetValue("sub_mch_id", paraInfo.MchID);//商户号
            inputObj.SetValue("user_ip", paraInfo.IP);//终端ip
            inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(paraInfo.WxProviderKey));//签名
            string xml = inputObj.ToXml();

            string response = HttpService.Post(xml, url, false, timeOut, paraInfo, logService, hid, true);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            return result;
        }
        #endregion

        #region 时间戳
        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <returns>时间戳</returns>
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region 随机串
        /// <summary>
        /// 生成随机串，随机串包含字母或数字
        /// </summary>
        /// <returns>随机串</returns>
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        #endregion
    }
}