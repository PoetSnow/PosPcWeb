﻿using System;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Senparc.Weixin.MP;
using Gemstar.BSPMS.Common.Services.EF;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 负责接收微信端的任何事件和交互
    /// </summary>
    [NotAuth]
    public class ListenController : BaseWeixinController
    {
        public ListenController()
        {

        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, MvcApplication.WeixinToken))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce,  MvcApplication.WeixinToken) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, MvcApplication.WeixinToken))
            {
                return Content("参数错误！");
            }

            postModel.Token = MvcApplication.WeixinToken;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = MvcApplication.WeixinEncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = MvcApplication.WeixinAppId;//根据自己后台的设置保持一致
            

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, maxRecordCount);

            try
            {
                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;

                //执行微信处理过程
                messageHandler.Execute();

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                //return new WeixinResult(messageHandler);//v0.8+
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                #region 异常处理
                var centerDb = GetCenterDb();
                SysLogService.AddSysLog("weixin/Listen", ex, "auto", centerDb);
                return Content("");
                #endregion
            }
        }

        ///// <summary>
        ///// 最简化的处理流程（不加密）
        ///// </summary>
        //[HttpPost]
        //[ActionName("MiniPost")]
        //public ActionResult MiniPost(PostModel postModel)
        //{
        //    if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
        //    {
        //        //return Content("参数错误！");//v0.7-
        //        return new WeixinResult("参数错误！");//v0.8+
        //    }

        //    postModel.Token = Token;
        //    postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
        //    postModel.AppId = AppId;//根据自己后台的设置保持一致

        //    var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, 10);

        //    messageHandler.Execute();//执行微信处理过程

        //    //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
        //    //return new WeixinResult(messageHandler);//v0.8+
        //    return new FixWeixinBugWeixinResult(messageHandler);//v0.8+
        //}

        ///*
        // * v0.3.0之前的原始Post方法见：WeixinController_OldPost.cs
        // *
        // * 注意：虽然这里提倡使用CustomerMessageHandler的方法，但是MessageHandler基类最终还是基于OldPost的判断逻辑，
        // * 因此如果需要深入了解Senparc.Weixin.MP内部处理消息的机制，可以查看WeixinController_OldPost.cs中的OldPost方法。
        // * 目前为止OldPost依然有效，依然可用于生产。
        // */

        ///// <summary>
        ///// 为测试并发性能而建
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ForTest()
        //{
        //    //异步并发测试（提供给单元测试使用）
        //    DateTime begin = DateTime.Now;
        //    int t1, t2, t3;
        //    System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
        //    System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
        //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
        //    DateTime end = DateTime.Now;
        //    var thread = System.Threading.Thread.CurrentThread;
        //    var result = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
        //            thread.ManagedThreadId,
        //            HttpContext.ApplicationInstance.GetHashCode(),
        //            begin,
        //            end,
        //            t2 - t1
        //            );
        //    return Content(result);
        //}
    }
}