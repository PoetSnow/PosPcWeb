using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Web.Script.Serialization;
using System.Web;
using Gemstar.BSPMS.Common.Tools;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace Gemstar.BSPMS.Hotel.Services.EF.PwdLock
{
    //蛋贝门锁
    public class Danbay : ILock
    {
        //写卡发送密码
        public string WriteLock(IMasterService masterService, DbHotelPmsContext _pmsContext, ResDetail resDetailEntity, UpGetSmsInfoResult smsInfo, string lockid, string hid, string regId, string seqId, string beginTime)
        {
            string cardNo = "";
            var _code = masterService.GetSysParaValue("danbayName");
            var _pwd = masterService.GetSysParaValue("danbayPwd");
            var _url = masterService.GetSysParaValue("danbayUrl");
            var _token = getToken(_code, _pwd, string.Format("{0}{1}", _url, "system/connect"));
            if (string.IsNullOrWhiteSpace(_token))
            {
                throw new Exception("密码锁获取token失败");
            }
            var serializer = new JavaScriptSerializer();
            var para = serializer.Deserialize<Dictionary<string, string>>(_token);
            var success = false;
            if (para.ContainsKey("mtoken") && !string.IsNullOrWhiteSpace(para["mtoken"]))
            {
                var LockPwd = GenerateRandomText(6);
                var postString = new StringBuilder();
                if (string.IsNullOrWhiteSpace(seqId))
                {
                    postString.Append("deviceId=").Append(HttpUtility.UrlEncode(lockid)); 
                    postString.Append("&password=").Append(HttpUtility.UrlEncode(LockPwd));
                    postString.Append("&pwdType=").Append(HttpUtility.UrlEncode("3"));
                    postString.Append("&mtoken=").Append(HttpUtility.UrlEncode(para["mtoken"]));
                    var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                    var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "system/deviceCtrl/lockPwd/addPwd"), contentBytes);
                    //var result = "{\"message\":\"新增成功\",\"status\":\"200\",\"result\":{\"pwdID\":1}}";
                    //var addPwd = serializer.Deserialize<SuccessMsg>(result);
                    var addPwd = FromJsonTo<SuccessMsg>(result);
                    if (!string.IsNullOrWhiteSpace(addPwd.status))
                    {
                        if (addPwd.status == "200")
                        {
                            if(!string.IsNullOrWhiteSpace(addPwd.result.pwdID))
                            {
                                cardNo = addPwd.result.pwdID;
                                success = true;
                            }
                        }
                        else
                        {
                            throw new Exception(addPwd.message);
                        }
                    }
                }
                else
                {
                    //重写卡
                    long Seqid = -1;
                    long.TryParse(seqId, out Seqid);
                    if (Seqid <= 0)
                    {
                        throw new Exception("参数不正确");
                    }
                    var lockEntity = _pmsContext.LockLogs.Where(c => c.Seqid == Seqid && c.Status == 0).SingleOrDefault();
                    if (lockEntity == null)
                    {
                        throw new Exception("参数不正确");
                    }
                    if (!(lockEntity.Hid == resDetailEntity.Hid && lockEntity.Regid == resDetailEntity.Regid))
                    {
                        throw new Exception("参数不正确");
                    }
                    postString.Append("deviceId=").Append(HttpUtility.UrlEncode(lockid));
                    postString.Append("&pwdType=").Append(HttpUtility.UrlEncode("3"));
                    postString.Append("&password=").Append(HttpUtility.UrlEncode(LockPwd));
                    postString.Append("&pwdID=").Append(HttpUtility.UrlEncode(lockEntity.CardNo));
                    postString.Append("&mtoken=").Append(HttpUtility.UrlEncode(para["mtoken"]));
                    var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                    var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "system/deviceCtrl/lockPwd/editPwd"), contentBytes);
                    var addPwd = serializer.Deserialize<Dictionary<string, string>>(result);
                    if (addPwd.ContainsKey("status") && !string.IsNullOrWhiteSpace(addPwd["status"]))
                    {
                        if (addPwd["status"] == "200")
                        {
                            cardNo = lockEntity.CardNo;
                            success = true;
                        }
                        else
                        {
                            throw new Exception(addPwd["message"]);
                        }
                    }
                }
                if (success)
                { 
                    var sendService = DependencyResolver.Current.GetService<ISMSSendService>();
                    var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                    var syspara = _sysParaService.GetSMSSendPara();
                    var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                    //异步发送短信
                    
                    var hotelenty = _pmsContext.PmsHotels.Where(c => c.Hid == hid).SingleOrDefault();
                    string hotelName = "";
                    if(hotelenty != null && !string.IsNullOrEmpty(hotelenty.Hotelshortname))
                    {
                        hotelName = hotelenty.Hotelshortname;
                    }
                    else
                    {
                        var _currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
                        hotelName = _currentInfo.HotelName;
                    }
                    var sendPara = new SMSSendParaHotelLockPwd
                    {
                        Mobile = resDetailEntity.GuestMobile,
                        GuestName = resDetailEntity.Guestname,
                        HotelName = hotelName,
                        BeginDate = DateTime.Parse(beginTime),
                        EndDate = resDetailEntity.DepDate.Value,
                        RoomNo = resDetailEntity.RoomNo,
                        Lockpwd = LockPwd
                    };
                    sendService.SendHotelLockPassword(hid, sendPara, smsInfo, syspara, smsLogService);
                }
                else
                {
                   throw new Exception("发卡失败");
                }
            }
            else
            {
                throw new Exception("账号密码错误");
            }
            return cardNo;
        }
        //注销卡
        public void CancelLock(LockLog lockEntity, DbHotelPmsContext _pmsContext, string hid, string cardNo)
        {
            var regId = lockEntity.Regid;
            ResDetail resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid).OrderByDescending(c => c.Cdate).SingleOrDefault();
            if (resDetailEntity != null)
            {
                var masterServiceObject = DependencyResolver.Current.GetService(typeof(IMasterService));
                var masterService = (masterServiceObject as IMasterService);
                var lockid = _pmsContext.Rooms.Where(r => r.Id == resDetailEntity.Roomid && resDetailEntity.Hid == r.Hid).Select(r => r.Lockid).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(lockid))
                {
                    var _code = masterService.GetSysParaValue("danbayName");
                    var _pwd = masterService.GetSysParaValue("danbayPwd");
                    var _url = masterService.GetSysParaValue("danbayUrl");
                    var _token = getToken(_code, _pwd, string.Format("{0}{1}", _url, "system/connect"));
                    if (string.IsNullOrWhiteSpace(_token))
                    {
                        throw new Exception("密码锁获取token失败");
                    }
                    var serializer = new JavaScriptSerializer();
                    var para = serializer.Deserialize<Dictionary<string, string>>(_token);
                    if (para.ContainsKey("mtoken") && !string.IsNullOrWhiteSpace(para["mtoken"]))
                    {
                        var postString = new StringBuilder();

                        postString.Append("deviceId=").Append(HttpUtility.UrlEncode(lockid));
                        postString.Append("&pwdType=").Append(HttpUtility.UrlEncode("3"));
                        postString.Append("&pwdID=").Append(HttpUtility.UrlEncode(cardNo));
                        postString.Append("&mtoken=").Append(HttpUtility.UrlEncode(para["mtoken"]));
                        var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                        var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "system/deviceCtrl/lockPwd/delPwd"), contentBytes);
                        var rpara = serializer.Deserialize<Dictionary<string, string>>(result);
                        if(rpara.ContainsKey("status") && !string.IsNullOrWhiteSpace(rpara["status"]))
                        {
                            if (rpara["status"] != "200")
                            {
                                throw new Exception(rpara["message"]);
                            }
                        }
                        else
                        {
                            throw new Exception("注销卡失败");
                        }
                    }
                    
                }
                else
                {
                    throw new Exception("门锁参数不正确");
                }
            }
        }
        /// <summary> 
        /// 生成随机密码锁
        /// </summary> 
        private string GenerateRandomText(int length)
        {
            const string txtChars = "0123456789";
            var sb = new StringBuilder(length);
            int maxLength = txtChars.Length;
            var _rand = new Random();
            for (int n = 0, len = length - 1; n <= len; n++)
            {
                sb.Append(txtChars.Substring(_rand.Next(maxLength), 1));
            }
            return sb.ToString();
        }
        private string getToken(string code, string pwd, string url)
        {            
            var postString = new StringBuilder();
            var ticket_consume_url = "http://pmsnotify.gshis.com/DataExchange/GetTockenSuccess";
            var return_url = "http://pmsnotify.gshis.com/DataExchange/GetTockenFail";
            postString.Append("mc_username=").Append(HttpUtility.UrlEncode(code));
            postString.Append("&mc_password=").Append(HttpUtility.UrlEncode(pwd));
            postString.Append("&ticket_consume_url=").Append(HttpUtility.UrlEncode(ticket_consume_url));
            postString.Append("&return_url=").Append(HttpUtility.UrlEncode(return_url));
            postString.Append("&random_code=").Append(GenerateRandomText(3));
            var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
            return SMSSendHelper.PostGetData("post", url, contentBytes);
        }
        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }
        //{\"message\":\"新增成功\",\"status\":\"200\",\"result\":{\"pwdID\":1}}";
        [DataContract]
        private class SuccessMsg
        {
            [DataMember]
            public string message { get; set; }
            [DataMember]
            public string status { get; set; }
            [DataMember]
            public Result result { get; set; }
        }
        [DataContract]
        private class Result
        {
            [DataMember]
            public string pwdID { get; set; }
        }
    }
}
