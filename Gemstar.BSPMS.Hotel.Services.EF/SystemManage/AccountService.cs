using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    /// <summary>
    /// 登录服务接口
    /// 此接口里面的方法要求只能从中央数据库获取数据，因为此时还没有登录，无法获取酒店业务数据库数据
    /// </summary>
    public class AccountService : IAccountService
    {
        public AccountService(IHotelInfoService hotelInfoService,HttpRequestBase request)
        {
            _hotelInfoService = hotelInfoService;
            _request = request;
        }
        #region 登录
        public AccountInfo Login(string hid, string username, string pwd)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不能为空" };
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名不能为空" };
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "登录密码不能为空" };
            }
            //先获取hid对应的酒店信息
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            if (hotelInfo == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不正确" };
            }
            if (hotelInfo.Status == EntityStatus.禁用)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不正确" };
            }
            if (string.IsNullOrWhiteSpace(hotelInfo.ServerAddress) || string.IsNullOrWhiteSpace(hotelInfo.DbServer) || string.IsNullOrWhiteSpace(hotelInfo.DbName) || string.IsNullOrWhiteSpace(hotelInfo.Logid) || string.IsNullOrWhiteSpace(hotelInfo.LogPwd))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id未正确设置服务器和数据库信息" };
            } 
            var sendService = DependencyResolver.Current.GetService<ISysParaService>();
            Dictionary<string, string> ExpirePara = sendService.GetExpiredPara();
            string ExpiredRemindContent = ExpirePara["expiredremindcontent"];//到期提醒内容
            int ExpiredLoginDayCount = int.Parse(ExpirePara["expiredlogindaycount"]);//到期后仍能登录使用天数
            DateTime dt = DateTime.Now.AddDays(-ExpiredLoginDayCount);
            if (!hotelInfo.ExpiryDate.HasValue || dt >= hotelInfo.ExpiryDate)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = ExpiredRemindContent };
            }
            //if (!hotelInfo.ExpiryDate.HasValue || hotelInfo.ExpiryDate.Value < DateTime.Now)
            //{
            //    return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id使用期已超过" };
            //}
            //转到对应的业务库里面判断用户名密码
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,isConnectViaInternet);
            var pmsDb = new DbHotelPmsContext(dbConnStr,hid,username,_request);

            var encryptedPwd = PasswordHelper.GetEncryptedPassword(username, pwd);
            var user = pmsDb.PmsUsers.SingleOrDefault(w => w.Code == username && w.Grpid == hid && w.Status < EntityStatus.禁用);
            if (user == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名密码不匹配" };
            }
            if(user.Pwd != encryptedPwd)
            {
                var result = new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名密码不匹配" };
                var encryptedDefaultPassword = PasswordHelper.GetEncryptedPassword(user.Code, PasswordHelper.GetDefaultPasswordFromMobile(user.Mobile));                
                if(user.Pwd == encryptedDefaultPassword)
                {
                    result.ErrorMessage = "用户名密码不匹配<br /><span style=\"color:red;padding:0px;\">(初始登录密码为手机号)</span>";
                }
                return result;
            }
            //处理服务器地址前面的http://
            var serverAddress = hotelInfo.ServerAddress;
            if (!serverAddress.StartsWith("http://"))
            {
                serverAddress = string.Format("http://{0}", serverAddress);
            }

            return new AccountInfo
            {
                LoginSuccess = true,
                DbName = hotelInfo.DbName,
                DbServer = hotelInfo.DbServer,
                DbUser = hotelInfo.Logid,
                DbPwd = hotelInfo.LogPwd,
                Grpid = hotelInfo.Grpid,
                Hid = hotelInfo.Hid,
                HotelName = hotelInfo.Name,
                WebServerUrl = serverAddress,
                UserId = user.Id.ToString(),
                UserCode = user.Code,
                UserName = user.Name,
                IsRegUser = user.IsReg == 1,
                VersionId= hotelInfo.VersionId

            };
        }

        public AccountInfo AutoLogin(string hid, string username, string pwd)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不能为空" };
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名不能为空" };
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "登录密码不能为空" };
            }
            //先获取hid对应的酒店信息
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            if (hotelInfo == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不正确" };
            }
            if (hotelInfo.Status == EntityStatus.禁用)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id不正确" };
            }
            if (string.IsNullOrWhiteSpace(hotelInfo.ServerAddress) || string.IsNullOrWhiteSpace(hotelInfo.DbServer) || string.IsNullOrWhiteSpace(hotelInfo.DbName) || string.IsNullOrWhiteSpace(hotelInfo.Logid) || string.IsNullOrWhiteSpace(hotelInfo.LogPwd))
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id未正确设置服务器和数据库信息" };
            }
            var sendService = DependencyResolver.Current.GetService<ISysParaService>();
            Dictionary<string, string> ExpirePara = sendService.GetExpiredPara();
            string ExpiredRemindContent = ExpirePara["expiredremindcontent"];//到期提醒内容
            int ExpiredLoginDayCount = int.Parse(ExpirePara["expiredlogindaycount"]);//到期后仍能登录使用天数
            DateTime dt = DateTime.Now.AddDays(-ExpiredLoginDayCount);
            if (!hotelInfo.ExpiryDate.HasValue || dt >= hotelInfo.ExpiryDate)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = ExpiredRemindContent };
            }
            //if (!hotelInfo.ExpiryDate.HasValue || hotelInfo.ExpiryDate.Value < DateTime.Now)
            //{
            //    return new AccountInfo { LoginSuccess = false, ErrorMessage = "酒店id使用期已超过" };
            //}
            //转到对应的业务库里面判断用户名密码
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,isConnectViaInternet);
            var pmsDb = new DbHotelPmsContext(dbConnStr, hid, username, _request);

            var encryptedPwd = pwd;
            var user = pmsDb.PmsUsers.SingleOrDefault(w => w.Code == username && w.Grpid == hid && w.Status < EntityStatus.禁用);
            if (user == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名密码不匹配" };
            }
            if (user.Pwd != encryptedPwd)
            {
                var result = new AccountInfo { LoginSuccess = false, ErrorMessage = "用户名密码不匹配" };
                var encryptedDefaultPassword = PasswordHelper.GetEncryptedPassword(user.Code, PasswordHelper.GetDefaultPasswordFromMobile(user.Mobile));
                if (user.Pwd == encryptedDefaultPassword)
                {
                    result.ErrorMessage = "用户名密码不匹配<br /><span style=\"color:red;padding:0px;\">(初始登录密码为手机号)</span>";
                }
                return result;
            }
            //处理服务器地址前面的http://
            var serverAddress = hotelInfo.ServerAddress;
            if (!serverAddress.StartsWith("http://"))
            {
                serverAddress = string.Format("http://{0}", serverAddress);
            }

            return new AccountInfo
            {
                LoginSuccess = true,
                DbName = hotelInfo.DbName,
                DbServer = hotelInfo.DbServer,
                DbUser = hotelInfo.Logid,
                DbPwd = hotelInfo.LogPwd,
                Grpid = hotelInfo.Grpid,
                Hid = hotelInfo.Hid,
                HotelName = hotelInfo.Name,
                WebServerUrl = serverAddress,
                UserId = user.Id.ToString(),
                UserCode = user.Code,
                UserName = user.Name,
                IsRegUser = user.IsReg == 1,
                VersionId = hotelInfo.VersionId
            };
        }

        #endregion
        /// <summary>
        /// 判断用户信息是否正确
        /// </summary>
        /// <param name="hid">酒店编号</param>
        /// <param name="code">登录名</param>
        /// <param name="type">验证类型（手机还是邮箱）</param>
        /// <param name="value">手机号或者邮箱号</param>
        /// <returns>验证结果，错误信息</returns>
        public string CheckUserinfo(string hid, string code, string type, string value)
        {
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            if (hotelInfo == null)
            {
                return "酒店代码不存在，";
            }
            if (string.IsNullOrWhiteSpace(hotelInfo.ServerAddress) || string.IsNullOrWhiteSpace(hotelInfo.DbServer) || string.IsNullOrWhiteSpace(hotelInfo.DbName) || string.IsNullOrWhiteSpace(hotelInfo.Logid) || string.IsNullOrWhiteSpace(hotelInfo.LogPwd))
            {
                return "酒店代码不存在，";
            }
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,isConnectViaInternet);
            var _pmsContext = new DbHotelPmsContext(dbConnStr,hid,"",_request);
            string retval = "";
            if (type == "Mobile")
            {
                if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid).ToList().Count <= 0)
                {
                    retval = "酒店代码不存在，";
                }
                else if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Code == code).ToList().Count <= 0)
                {
                    retval = "登录名不存在，";
                }
                else if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Code == code).ToList().Count <= 0)
                {
                    retval = "登录名不存在，";
                }
                else if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Code == code && w.Mobile == value).ToList().Count <= 0)
                {
                    retval = "手机号不正确，";
                }
            }
            else
            {
                if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid).ToList().Count <= 0)
                {
                    retval = "酒店代码不存在，";
                }
                else if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Code == code).ToList().Count <= 0)
                {
                    retval = "登录名不存在，";
                }
                else if (_pmsContext.PmsUsers.Where(w => w.Grpid == hid && w.Code == code && w.Email == value).ToList().Count <= 0)
                {
                    retval = "邮箱号不正确，";
                }
            }
            return retval;
        }

        private IHotelInfoService _hotelInfoService;
        private HttpRequestBase _request;
    }
}

