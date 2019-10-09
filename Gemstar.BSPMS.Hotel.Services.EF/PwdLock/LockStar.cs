using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Web;
using Gemstar.BSPMS.Common.Tools;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.EF.PwdLock
{
    //创佳门锁
    public class LockStar : ILock
    {
        //写卡发送密码
        public string WriteLock(IMasterService masterService, DbHotelPmsContext _pmsContext, ResDetail resDetailEntity, UpGetSmsInfoResult smsInfo, string lockid, string hid, string regId, string seqId, string beginTime)
        {
            string cardNo = "";
            var _code = masterService.GetSysParaValue("LockstarCode");
            var _pwd = masterService.GetSysParaValue("LockstarPwd");
            var _url = masterService.GetSysParaValue("LockstarUrl");
            var _token = getToken(_code, _pwd, string.Format("{0}{1}", _url, "iremote/thirdpart/login"));
            if (string.IsNullOrWhiteSpace(_token))
            {
                throw new Exception("密码锁获取token失败");
            }
            var serializer = new JavaScriptSerializer();
            var para = serializer.Deserialize<Dictionary<string, string>>(_token);
            if (para.ContainsKey("resultCode") && para["resultCode"] == "0")
            {
                int UserCode = 1;
                if (string.IsNullOrWhiteSpace(seqId))
                {
                    var usercode = _pmsContext.LockLogs.Where(c => c.Hid == hid && c.Regid == regId && c.Roomno == resDetailEntity.RoomNo && c.Locktype == "lockstar").OrderBy(c => c.CreateDate).AsNoTracking().Select(c => c.CardNo).Max();
                    int.TryParse(usercode, out UserCode);
                    if (UserCode < 0)
                    {
                        UserCode = 0;
                    }
                    UserCode++;
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
                    int.TryParse(lockEntity.CardNo, out UserCode);
                    if (UserCode <= 0)
                    {
                        UserCode = 1;
                    }
                }
                cardNo = UserCode.ToString();
                var LockPwd = GenerateRandomText(6);
                var postString = new StringBuilder();

                postString.Append("token=").Append(HttpUtility.UrlEncode(para["token"]));
                postString.Append("&zwavedeviceid=").Append(HttpUtility.UrlEncode(lockid));
                postString.Append("&usercode=").Append(HttpUtility.UrlEncode(UserCode.ToString()));
                postString.Append("&password=").Append(HttpUtility.UrlEncode(LockPwd));
                postString.Append("&validfrom=").Append(HttpUtility.UrlEncode(DateTime.Parse(beginTime).AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss")));
                postString.Append("&validthrough=").Append(HttpUtility.UrlEncode(resDetailEntity.DepDate.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "iremote/thirdpart/zufang/setlockuserpassword"), contentBytes);
                var jss = new JavaScriptSerializer();
                var depara = serializer.Deserialize<Dictionary<string, string>>(result);
                if (depara.ContainsKey("resultCode") && depara["resultCode"] == "0")
                {
                    var sendService = DependencyResolver.Current.GetService<ISMSSendService>();
                    var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                    var syspara = _sysParaService.GetSMSSendPara();
                    var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                    //异步发送短信
                    var hotelenty = _pmsContext.PmsHotels.Where(c => c.Hid == hid).SingleOrDefault();
                    string hotelName = "";
                    if (hotelenty != null && !string.IsNullOrEmpty(hotelenty.Hotelshortname))
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
                    var code = depara["resultCode"];
                    switch (code)
                    {
                        case "30300": throw new Exception("无效的token");
                        case "30311": throw new Exception("找不到指定的锁");
                        case "10022": throw new Exception("没有权限");
                        default: throw new Exception("门锁超时，请稍后再试");
                    }
                }
            }
            else
            {
                throw new Exception("密码锁登录失败");
            }
            return cardNo;
        }
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
                    var _code = masterService.GetSysParaValue("LockstarCode");
                    var _pwd = masterService.GetSysParaValue("LockstarPwd");
                    var _url = masterService.GetSysParaValue("LockstarUrl");
                    var _token = getToken(_code, _pwd, string.Format("{0}{1}", _url, "iremote/thirdpart/login"));
                    if (!string.IsNullOrWhiteSpace(_token))
                    {
                        var serializer = new JavaScriptSerializer();
                        var para = serializer.Deserialize<Dictionary<string, string>>(_token);
                        if (para.ContainsKey("resultCode") && para["resultCode"] == "0")
                        {
                            var postString = new StringBuilder();

                            postString.Append("token=").Append(HttpUtility.UrlEncode(para["token"]));
                            postString.Append("&zwavedeviceid=").Append(HttpUtility.UrlEncode(lockid));
                            postString.Append("&usercode=").Append(HttpUtility.UrlEncode(cardNo));
                            var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
                            var result = SMSSendHelper.PostGetData("post", string.Format("{0}{1}", _url, "iremote/thirdpart/zufang/deletelockuserpassword"), contentBytes);
                        }
                        else
                        {
                            throw new Exception("获取token失败");
                        }
                    }
                    else
                    {
                        throw new Exception("获取token失败，"+ _token);
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
            postString.Append("code=").Append(HttpUtility.UrlEncode(code));
            postString.Append("&password=").Append(HttpUtility.UrlEncode(pwd));
            var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());
            return SMSSendHelper.PostGetData("post", url, contentBytes);
        }
    }
}
