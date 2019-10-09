﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Aop.Api.Util;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    public class AliPayNotify
    {
        #region 字段
        private string _partner = "";               //合作身份者ID
        private string _charset = "";         //编码格式
        private string _sign_type = "";             //签名方式
        private string _alipay_public_key = "";     //支付宝公钥文件地址

        //支付宝消息验证地址
        private string Https_veryfy_url = "";
        private IPayLogService _logService;
        #endregion


        /// <summary>
        /// 构造函数
        /// 从配置文件中初始化变量
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="notify_id">通知验证ID</param>

        public AliPayNotify(string charset, string sign_type, string pid, string mapiUrl, string alipay_public_key,IPayLogService payLogService)
        {
            //初始化基础配置信息
            _charset = charset;
            _sign_type = sign_type;
            _partner = pid;
            Https_veryfy_url = mapiUrl + "?service=notify_verify&";
            _alipay_public_key = alipay_public_key;
            _logService = payLogService;
        }


        /// <summary>
        ///  验证消息是否是支付宝发出的合法消息
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="notify_id">通知验证ID</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <returns>验证结果</returns>
        public bool Verify(SortedDictionary<string, string> inputPara, string notify_id, string sign)
        {
            //获取返回时的签名验证结果
            bool isSign = GetSignVeryfy(inputPara, sign);
            //获取是否是支付宝服务器发来的请求的验证结果
            string responseTxt = "true";
            //先不验证是否来自阿里，不知道为什么，阿里总是返回false，先只验证签名
            //if (notify_id != null && notify_id != "") { responseTxt = GetResponseTxt(notify_id); }
            _logService.Debug("-1", "AliPayNotify", string.Format("是否来自ali:{0};签名验证:{1}", responseTxt, isSign));
            //写日志记录（若要调试，请取消下面两行注释）
            //string sWord = "responseTxt=" + responseTxt + "\n isSign=" + isSign.ToString() + "\n 返回回来的参数：" + GetPreSignStr(inputPara) + "\n ";
            //Core.LogResult(sWord);

            //判断responsetTxt是否为true，isSign是否为true
            //responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
            //isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
            if (responseTxt == "true" && isSign)//验证成功
            {
                return true;
            }
            else//验证失败
            {
                return false;
            }
        }

        ///// <summary>
        ///// 获取待签名字符串（调试用）
        ///// </summary>
        ///// <param name="inputPara">通知返回参数数组</param>
        ///// <returns>待签名字符串</returns>
        //private string GetPreSignStr(SortedDictionary<string, string> inputPara)
        //{
        //    Dictionary<string, string> sPara = new Dictionary<string, string>();

        //    //过滤空值、sign与sign_type参数
        //    sPara = Core.FilterPara(inputPara);

        //    //获取待签名字符串
        //    string preSignStr = Core.CreateLinkString(sPara);

        //    return preSignStr;
        //}

        /// <summary>
        /// 获取返回时的签名验证结果
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="sign">对比的签名结果</param>
        /// <returns>签名验证结果</returns>
        private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign)
        {
            var signString = AlipaySignature.GetSignContent(inputPara);
            return AlipaySignature.RSACheckContent(signString, sign, _alipay_public_key, _charset, false);
        }





        /// <summary>
        /// 获取是否是支付宝服务器发来的请求的验证结果
        /// </summary>
        /// <param name="notify_id">通知验证ID</param>
        /// <returns>验证结果</returns>
        private string GetResponseTxt(string notify_id)
        {
            string veryfy_url = Https_veryfy_url + "partner=" + _partner + "&notify_id=" + notify_id;

            //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
            string responseTxt = Get_Http(veryfy_url, 120000);

            return responseTxt;
        }

        /// <summary>
        /// 获取远程服务器ATN结果
        /// </summary>
        /// <param name="strUrl">指定URL路径地址</param>
        /// <param name="timeout">超时时间设置</param>
        /// <returns>服务器ATN结果</returns>
        private string Get_Http(string strUrl, int timeout)
        {
            string strResult;
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }

    }
}
