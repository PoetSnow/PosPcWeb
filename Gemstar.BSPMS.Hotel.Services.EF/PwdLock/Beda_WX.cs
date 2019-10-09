using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Data.Entity;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Text;
using System.Security.Cryptography;
using Gemstar.BSPMS.Common.Tools;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Gemstar.BSPMS.Hotel.Services.EF.PwdLock
{
    public class Beda_WX : ILock
    {
        public string WriteLock(IMasterService masterService, DbHotelPmsContext _pmsContext, ResDetail resDetailEntity, UpGetSmsInfoResult smsInfo, string lockid, string hid, string regId, string seqId, string beginTime)
        {
            string signStr = "";
            var postString = new StringBuilder();
            var _hotelid = masterService.GetSysParaValue("Beda_wxHotel");
            var _token = masterService.GetSysParaValue("Beda_wxToken");
            var _url = masterService.GetSysParaValue("Beda_wxUrl");
            byte[] contentBytes;
            string result;
            if (string.IsNullOrWhiteSpace(seqId))
            {
                if (_pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.Roomno == resDetailEntity.RoomNo && c.Locktype == "lockstar" && c.CardNo == resDetailEntity.GuestMobile && c.Status==0).Any())
                {
                    throw new Exception("已发放房卡到手机" + resDetailEntity.GuestMobile + ",您可以重写卡或注销后重新发卡");
                }
                signStr = Sha1("checkintimecheckouttimehotelidmobileroom" + _token);
                postString.Append("hotelid=").Append(_hotelid);
                postString.Append("&room=").Append(resDetailEntity.RoomNo);
                postString.Append("&mobile=").Append(resDetailEntity.GuestMobile);
                postString.Append("&checkintime=").Append(DateTime.Parse(beginTime).AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"));
                postString.Append("&checkouttime=").Append(resDetailEntity.DepDate.Value.ToString("yyyy-MM-dd HH:mm"));
                postString.Append("&sign=").Append(signStr);
                contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "api/wxkey/add"), contentBytes);
                var addPwd = FromJsonTo<SuccessMsg>(result);
                if (!string.IsNullOrWhiteSpace(addPwd.code))
                {
                    if (addPwd.code != "0")
                    {
                        throw new Exception(addPwd.msg);
                    }
                }
                else
                {
                    throw new Exception("发卡失败");
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
                if (lockEntity.CardNo != resDetailEntity.GuestMobile)
                {
                    throw new Exception("手机号已改变，请先注销后重新发放房卡");
                }
                if (lockEntity.Roomno != resDetailEntity.RoomNo)
                {
                    signStr = Sha1("hotelidmobilenewroomroom" + _token);
                    postString.Append("hotelid=").Append(_hotelid);
                    postString.Append("&room=").Append(lockEntity.Roomno);
                    postString.Append("&mobile=").Append(resDetailEntity.GuestMobile);
                    postString.Append("&newroom=").Append(resDetailEntity.RoomNo);
                    postString.Append("&sign=").Append(signStr);
                    contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                    result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "api/wxkey/changeroom"), contentBytes);
                    var serializer = new JavaScriptSerializer();
                    var rpara = serializer.Deserialize<Dictionary<string, string>>(result);
                    if (rpara.ContainsKey("code") && !string.IsNullOrWhiteSpace(rpara["code"]))
                    {
                        if (rpara["code"] != "0")
                        {
                            throw new Exception(rpara["msg"]);
                        }
                    }
                    else
                    {
                        throw new Exception("换房制卡失败");
                    }
                }
                else
                {
                    signStr = Sha1("checkintimecheckouttimehotelidmobileroom" + _token);
                    postString.Append("hotelid=").Append(_hotelid);
                    postString.Append("&room=").Append(resDetailEntity.RoomNo);
                    postString.Append("&mobile=").Append(resDetailEntity.GuestMobile);
                    postString.Append("&checkintime=").Append(DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"));
                    postString.Append("&checkouttime=").Append(resDetailEntity.DepDate.Value.ToString("yyyy-MM-dd HH:mm"));
                    postString.Append("&sign=").Append(signStr);
                    contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                    result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "api/wxkey/updatetime"), contentBytes);
                    var serializer1 = new JavaScriptSerializer();
                    var rpara1 = serializer1.Deserialize<Dictionary<string, string>>(result);
                    if (rpara1.ContainsKey("code") && !string.IsNullOrWhiteSpace(rpara1["code"]))
                    {
                        if (rpara1["code"] == "3")
                        {
                            throw new Exception("请注销后重新发放");
                        }
                        if (rpara1["code"] != "0")
                        {
                            throw new Exception(rpara1["msg"]);
                        }
                    }
                    else
                    {
                        throw new Exception("重写卡失败");
                    }
                }
            }
            return resDetailEntity.GuestMobile;
        }
        public void CancelLock(LockLog lockEntity, DbHotelPmsContext _pmsContext, string hid, string cardNo)
        {
            var regId = lockEntity.Regid;
            ResDetail resDetailEntity = _pmsContext.ResDetails.Where(c => c.Regid == regId && c.Hid == hid).OrderByDescending(c => c.Cdate).SingleOrDefault();
            if (resDetailEntity != null)
            {
                var masterServiceObject = DependencyResolver.Current.GetService(typeof(IMasterService));
                var masterService = (masterServiceObject as IMasterService);
                var _hotelid = masterService.GetSysParaValue("Beda_wxHotel");
                var _token = masterService.GetSysParaValue("Beda_wxToken");
                var _url = masterService.GetSysParaValue("Beda_wxUrl");
                string signStr = Sha1("hotelidmobileroom" + _token);
                var postString = new StringBuilder();
                postString.Append("hotelid=").Append(_hotelid);
                postString.Append("&room=").Append(resDetailEntity.RoomNo);
                postString.Append("&mobile=").Append(resDetailEntity.GuestMobile);
                postString.Append("&sign=").Append(signStr);
                var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "api/wxkey/cancel"), contentBytes);
                var serializer = new JavaScriptSerializer();
                var rpara = serializer.Deserialize<Dictionary<string, string>>(result);
                if (rpara.ContainsKey("code") && !string.IsNullOrWhiteSpace(rpara["code"]))
                {
                    if (rpara["code"] != "0")
                    {
                        throw new Exception(rpara["msg"]);
                    }
                }
                else
                {
                    throw new Exception("注销房卡失败");
                }

            }
        }
        [DataContract]
        private class SuccessMsg
        {
            [DataMember]
            public string code { get; set; }
            [DataMember]
            public string msg { get; set; }
            [DataMember]
            public Result data { get; set; }
        }
        [DataContract]
        private class Result
        {
            [DataMember]
            public string keyid { get; set; }
            [DataMember]
            public string hotelid { get; set; }
            [DataMember]
            public string room { get; set; }
            [DataMember]
            public string mobile { get; set; }
            [DataMember]
            public string checkintime { get; set; }
            [DataMember]
            public string checkouttime { get; set; }
            [DataMember]
            public string addtime { get; set; }
        }
        /// <summary>
        /// sha1加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Sha1(string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
        /// <summary>
        /// 序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }
    }
}
