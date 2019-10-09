using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 负责接收业主微信端的任何事件和交互
    /// </summary>
    [NotAuth]
    public class OwnerListenController : BaseWeixinController
    {
        public OwnerListenController()
        {

        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, MvcApplication.OwnerWeixinToken))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, MvcApplication.OwnerWeixinToken) + "。" +
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
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, MvcApplication.OwnerWeixinToken))
            {
                return Content("参数错误！");
            }

            postModel.Token = MvcApplication.OwnerWeixinToken;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = MvcApplication.OwnerWeixinEncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = MvcApplication.OwnerWeixinAppId;//根据自己后台的设置保持一致


            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomOwnerMessageHandler(Request.InputStream, postModel, maxRecordCount);

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
                SysLogService.AddSysLog("Ownerweixin/Listen", ex, "auto", centerDb);
                return Content("");
                #endregion
            }
        }
        /// <summary>
        /// 业主查询入口
        /// 负责检测业主身份，如果还不是业主则转到绑定界面，如果是则取出其房间所在的酒店信息，如果有多家酒店，则让其选择酒店，否则直接进入到查询界面
        /// </summary>
        /// <returns>根据身份判别进行不同的处理</returns>
        public ActionResult Query(string code, string state)
        {
            if (string.IsNullOrWhiteSpace(code) || !string.Equals("JxdPms", state))
            {
                return View("Error");
            }
            //根据code换取openid
            OAuthAccessTokenResult result;

            if (MvcApplication.IsTestEnv)
            {
                //为了方便测试，测试环境下写死一个值用来测试流程
                result = new OAuthAccessTokenResult
                {
                    openid = "chenql365"
                };
            }
            else
            {
                var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", MvcApplication.OwnerWeixinAppId, MvcApplication.OwnerWeixinAppSecret, code);
                result = Senparc.Weixin.HttpUtility.Get.GetJson<OAuthAccessTokenResult>(url);
            }
            if (string.IsNullOrWhiteSpace(result.openid))
            {
                return View("Error");
            }
            //根据得到的openid查询业主身份
            var centerDb = GetCenterDb();
            var hotelInfos = centerDb.Database.SqlQuery<UpQueryOwnerHotelsByWxOpenIdResult>("exec up_queryOwnerHotelsByWxOpenId @ownerWxOpenId=@ownerWxOpenId", new SqlParameter("@ownerWxOpenId", result.openid)).ToList();
            if (hotelInfos.Count == 0)
            {
                //还没有绑定，转到绑定界面
                ViewBag.openId = result.openid;
                return View("Bind");
            }
            else if (hotelInfos.Count == 1)
            {
                //只是一家酒店的业主，直接转到查询界面
                var profileid = hotelInfos.FirstOrDefault().ProfileId;
                var hid = hotelInfos.FirstOrDefault().Hid;
                Response.Redirect(Url.Action("Index", "RoomOwnerMonthCalc") + "?profileid=" + profileid + "&hid=" + hid + "&openid=" + result.openid);
                return Content("Owner Query");
            }
            else
            {
                //是多家酒店的业主，转到选择酒店界面
                string[] hotelarr = new string[hotelInfos.Count];
                string[][] str = new string[hotelInfos.Count()][];
                for (int i = 0; i < hotelInfos.Count(); i++)
                {

                    str[i] = new string[3];
                    str[i][0] = hotelInfos[i].Hid;
                    str[i][1] = hotelInfos[i].ProfileId.ToString();
                    str[i][2] = hotelInfos[i].Name;
                }
                ViewBag.hotelsinfo = str;
                ViewBag.openid = result.openid;
                return View("HotelChoose");
            }

        }
        public ActionResult SendCheckCode(string hid, string mobile)
        {
            //先检查hid和mobile不能为空，并且mobile是有效的手机号
            //先在master中检查hid是否存在
            //取出hid对应的业务数据库，调用MvcApplication.GetHotelDbConnStr来获取
            //在业务库中根据手机号查找业主，如果没找到就提示还不是业主，就不需要发送验证码
            //如果是业主，则取出profileid，则参考密码找回等功能中的短信发送方法进行发送，发送成功后将验证码进行md5计算后的值和profileid传到视图中，以便后续检查输入的验证码是否正确
            if (string.IsNullOrEmpty(hid))
            {
                return Json(JsonResultData.Failure("酒店代码不能为空"));
            }
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(JsonResultData.Failure("手机号不能为空"));
            }
            if (mobile.Length != 11 || mobile.Substring(0, 1) != "1")
            {
                return Json(JsonResultData.Failure("不是有效的手机号！"));
            }
            var centerDb = GetCenterDb();
            CenterHotel hotel = centerDb.Hotels.Where(w => w.Hid == hid).FirstOrDefault();
            if (hotel == null)
            {
                return Json(JsonResultData.Failure("酒店代码不存在！"));
            }

            var hotelDb = new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(centerDb, hid), hid, "weixinOwnerBind", null);
            var profile = hotelDb.MbrCards.Where(w =>w.Hid==hid && w.Mobile == mobile && w.IsOwner == true).FirstOrDefault();

            if (profile != null)
            {
                string checkcoderet = SendCheckCodeByMobile(mobile, "OwnerBind", hid);
                var result = new { profileid = profile.Id, checkcode = CryptHelper.EncryptMd5(checkcoderet).ToLower() };
                return Json(JsonResultData.Successed(result));
            }
            else
            {
                return Json(JsonResultData.Failure("该手机号不是业主！"));
            }
        }
        public ActionResult HotelBind(string openid)
        {
            ViewBag.openId = openid;
            return View("Bind");
        }
        public ActionResult HotelChoose(string openid)
        {
            var centerDb = GetCenterDb();
            var hotelInfos = centerDb.Database.SqlQuery<UpQueryOwnerHotelsByWxOpenIdResult>("exec up_queryOwnerHotelsByWxOpenId @ownerWxOpenId=@ownerWxOpenId", new SqlParameter("@ownerWxOpenId", openid)).ToList();
            string[] hotelarr = new string[hotelInfos.Count];
            string[][] str = new string[hotelInfos.Count()][];
            for (int i = 0; i < hotelInfos.Count(); i++)
            {

                str[i] = new string[3];
                str[i][0] = hotelInfos[i].Hid;
                str[i][1] = hotelInfos[i].ProfileId.ToString();
                str[i][2] = hotelInfos[i].Name;
            }
            ViewBag.hotelsinfo = str;
            ViewBag.openId = openid;
            return View("HotelChoose");
        }
        public ActionResult Bind(string hid, string mobile, Guid profileId, string openId, string md5CheckCode, string checkCode)
        {
            //将checkCode进行md5运算后，与md5CheckCode进行比较，如果不同则提示请输入正确的验证码
            //如果正确，则表示手机号和hid都是正确的，将记录插入到master库中的weixinOwnerHotelMapping
            //根据hid和profileid转到对应的业主查询界面

            var checkCodes = CryptHelper.EncryptMd5(checkCode).ToLower();
            if (checkCodes != md5CheckCode)
            {
                return Json(JsonResultData.Failure("请输入正确的验证码！"));
            }
            else
            {
                var centerDb = GetCenterDb(); 
                List<WeixinOwnerHotelMapping> wom = centerDb.WeixinOwnerHotelMappings.Where(w => w.Hid == hid && w.OwnerWxOpenId == openId && w.ProfileId == profileId).ToList();
                if (wom.Count > 0)
                {
                    return Json(JsonResultData.Successed("已绑定！"));
                }
                wom = centerDb.WeixinOwnerHotelMappings.Where(w => w.Hid == hid && w.OwnerWxOpenId != openId && w.ProfileId == profileId).ToList();
                if (wom.Count() > 0)
                {
                    centerDb.Entry(wom[0]).State = EntityState.Modified;
                    wom[0].OwnerWxOpenId = openId;
                    centerDb.SaveChanges();
                    return Json(JsonResultData.Successed("重新绑定微信号成功！"));
                }
                centerDb.WeixinOwnerHotelMappings.Add(new WeixinOwnerHotelMapping
                {
                    Hid = hid,
                    ProfileId = profileId,
                    OwnerWxOpenId = openId,
                    Cdate = DateTime.Now,

                });
                centerDb.SaveChanges();
                return Json(JsonResultData.Successed("验证成功！"));
            }
        }
        /// <summary>
        /// 手机验证
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public string SendCheckCodeByMobile(string mobile, string func, string hid)
        {
            try
            {
                var username = "";
                var password = "";
                if (!string.IsNullOrWhiteSpace(hid))
                {
                    var hotelDb = GetHotelDb(hid);
                    var pmsParaService = new PmsParaService(hotelDb);
                    pmsParaService.IsSmsEnable(hid, out username, out password);
                }
                var sendPara = new SMSSendParaCheckCode
                {
                    Mobile = mobile,
                    Func = (CheckFunc)Enum.Parse(typeof(CheckFunc), func, true),
                    UserName = username,
                    Password = password
                };
                return SMSSendHelper.SendCheckCodeandRetn(sendPara);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }



    }
}