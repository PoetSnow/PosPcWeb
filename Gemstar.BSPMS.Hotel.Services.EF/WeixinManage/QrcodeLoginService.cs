using System;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Tools;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.EF.WeixinManage
{
    public class QrcodeLoginService : IQrcodeLoginService
    {
        private const string domainUrl = "http://pms.gshis.com";
        private DbCommonContext _centerDb;
        public QrcodeLoginService(DbCommonContext centerDb)
        {
            _centerDb = centerDb;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="loginType">登录类型，1：操作员扫码登录，2：售后工程师扫码登录</param>
        /// <param name="authCode">授权码，如果是售后工程师扫码登录则需要先输入授权码</param>
        /// <param name="keyid">返回二维码键Id[WeixinQrcodeLogin.Keyid]</param>
        /// <returns>返回二维码主键ID[WeixinQrcodeLogin.Id]</returns>
        public string GenerateQrcode(byte loginType, string authCode, out string keyid)
        {
            keyid = null;
            try
            {
                var id = Guid.NewGuid();
                var keyId = Guid.NewGuid();
                DateTime nowDate = DateTime.Now;
                _centerDb.WeixinQrcodeLogins.Add(new WeixinQrcodeLogin
                {
                    Id = id,
                    Keyid = keyId,
                    CreateDate = nowDate,
                    ExpireDate = nowDate.AddMinutes(12),
                    Status = (int)WeixinQrcodeLoginStatus.QRCODE_WAIT,
                    LoginType = loginType,
                    LoginHid = authCode
                });
                var i = _centerDb.SaveChanges();
                if (i > 0)
                {
                    keyid = keyId.ToString("N").ToLower();
                    return id.ToString("N").ToLower();
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取二维码键ID
        /// </summary>
        /// <param name="id">主键ID[WeixinQrcodeLogin.Id]</param>
        /// <returns>返回二维码键Id[WeixinQrcodeLogin.Keyid]</returns>
        public string GetQrcodeKeyIdById(string id)
        {
            try
            {
                Guid outId = Guid.Empty;
                if (Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out outId))
                {
                    Guid? keyId = _centerDb.WeixinQrcodeLogins.AsNoTracking().Where(c => c.Id == outId && c.Status == (int)WeixinQrcodeLoginStatus.QRCODE_WAIT).Select(c => c.Keyid).FirstOrDefault();
                    if (keyId != null && keyId != Guid.Empty)
                    {
                        return keyId.Value.ToString("N").ToLower();
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取二维码地址
        /// </summary>
        /// <param name="keyid">键ID</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public string GetQrcodeUrl(string keyid, out string id)
        {
            id = null;
            string url = domainUrl + "/weixin/login/confirmQrcode"; string para = "?errcode=error";
            string data = null; WeixinQrcodeLogin entity = null;
            try
            {
                Guid _id = Guid.Empty;
                Guid _keyid = Guid.Empty;
                if (Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(keyid, out _keyid))
                {
                    _id = _centerDb.WeixinQrcodeLogins.AsNoTracking().Where(c => c.Keyid == _keyid).Select(c => c.Id).SingleOrDefault();
                }
                id = _id.ToString("N").ToLower();
                var status = GetQrcodeStatus(id, out data, out entity);
                if (entity != null)
                {
                    switch (status)
                    {
                        case WeixinQrcodeLoginStatus.QRCODE_WAIT:
                            {
                                //更改状态为 扫码成功，并生成Keyid
                                Guid newKeyid = Guid.NewGuid();
                                QrcodeScaned(entity.Id, newKeyid);
                                para = "?sign=" + newKeyid.ToString("N").ToLower();
                            }
                            break;
                        case WeixinQrcodeLoginStatus.QRCODE_INVALID:
                            {
                                //显示本地微信页面 请求已过期
                                para = "?errcode=invalid";
                            }
                            break;
                        default:
                            {
                                //显示本地微信页面 二维码已使用
                                para = "?errcode=use";
                            }
                            break;
                    }
                    //如果是售后工程师登录，则更改相应的访问地址
                    if (entity.LoginType == 2)
                    {
                        url = domainUrl + "/weixin/AuthLogin/confirmQrcode";
                    }
                    //如果是运营人员登录，则更改相应的访问地址
                    if (entity.LoginType == 3)
                    {
                        url = domainUrl + "/weixin/AdminLogin/confirmQrcode";
                    }
                }
            }
            catch { }
            return url + para;
        }

        /// <summary>
        /// 获取二维码状态
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="data">其他信息</param>
        /// <returns></returns>
        public WeixinQrcodeLoginStatus GetQrcodeStatus(string id, out string data, out WeixinQrcodeLogin entity)
        {
            data = null; entity = null;
            WeixinQrcodeLoginStatus result = WeixinQrcodeLoginStatus.QRCODE_ERROR;
            try
            {
                Guid loginId = Guid.Empty;
                if (Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out loginId))
                {
                    entity = _centerDb.WeixinQrcodeLogins.AsNoTracking().SingleOrDefault(c => c.Id == loginId);
                    if (entity != null)
                    {
                        Enum.TryParse<WeixinQrcodeLoginStatus>(entity.Status.ToString(), out result);
                    }
                    else
                    {
                        result = WeixinQrcodeLoginStatus.QRCODE_ERROR;
                    }

                    switch (result)
                    {
                        case WeixinQrcodeLoginStatus.QRCODE_WAIT:
                        case WeixinQrcodeLoginStatus.QRCODE_SCANED:
                            {
                                if (DateTime.Now > entity.ExpireDate)
                                {
                                    result = WeixinQrcodeLoginStatus.QRCODE_INVALID;
                                    QrcodeInvalid(entity.Id);
                                }
                            }
                            break;
                        case WeixinQrcodeLoginStatus.QRCODE_INVALID:
                        case WeixinQrcodeLoginStatus.QRCODE_EMPTY_ACCOUNT:
                        case WeixinQrcodeLoginStatus.QRCODE_REFUSE_LOGIN:
                            {
                                //流程结束
                            }
                            break;
                        case WeixinQrcodeLoginStatus.QRCODE_LOGIN_SUCCESS:
                            {
                                bool is_login_success = false;
                                if (!string.IsNullOrWhiteSpace(entity.LoginOpenid) && !string.IsNullOrWhiteSpace(entity.LoginHid) && entity.LoginUserid != null && entity.Keyid != null && entity.LoginDate != null)
                                {
                                    if (DateTime.Now < entity.LoginDate.Value.AddSeconds(5))//手机端确认登录后，需要电脑端主动查询是否登录成功。这个时间限制在5秒内。
                                    {
                                        data = string.Format("{0}{1}{2}", entity.Id.ToString("N").ToLower(), entity.Keyid.ToString("N").ToLower(), entity.LoginDate.Value.Ticks);
                                        is_login_success = true;
                                    }
                                }
                                if (!is_login_success)
                                {
                                    result = WeixinQrcodeLoginStatus.QRCODE_ERROR;
                                    data = "状态错误！";
                                }
                            }
                            break;
                        default:
                            {
                                result = WeixinQrcodeLoginStatus.QRCODE_ERROR;
                                data = "状态错误！";
                            }
                            break;
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="jump">{WeixinQrcodeLogin.Id}{WeixinQrcodeLogin.Keyid}{loginDate.LoginDate.Ticks}</param>
        /// <param name="IsCheckWxAuthLogin">是否售后工程师登录 默认False</param>
        public AccountInfo Login(string jump, out bool IsCheckWxAuthLogin)
        {
            IsCheckWxAuthLogin = false; //判断是否是操作员，还是售后工程师 默认为：false(操作员)
            try
            {
                if (string.IsNullOrWhiteSpace(jump))
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不能为空！" };
                }
                if (jump.Length < 64)
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不正确！" };
                }

                string para_id = jump.Substring(0, 32);
                string para_keyid = jump.Substring(32, 32);
                string para_ticks = jump.Substring(64);

                //主键ID
                Guid id = Guid.Empty;
                if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(para_id, out id))
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不正确！" };
                }
                //键ID
                Guid keyid = Guid.Empty;
                if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(para_keyid, out keyid))
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不正确！" };
                }
                //登录时间
                DateTime loginDate = DateTime.MinValue;
                long ticks = 0;
                if (long.TryParse(para_ticks, out ticks))
                {
                    if (ticks > 0)
                    {
                        loginDate = DateTime.Parse(new DateTime(ticks).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                }
                else
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不正确！" };
                }
                if (ticks <= 0 || loginDate == null || loginDate == DateTime.MinValue || loginDate == DateTime.MaxValue)
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "参数不正确！" };
                }
                //验证登录
                var entity = _centerDb.WeixinQrcodeLogins.AsNoTracking().Where(c => c.Id == id && c.Keyid == keyid && c.LoginDate == loginDate && c.Status == (int)WeixinQrcodeLoginStatus.QRCODE_LOGIN_SUCCESS).SingleOrDefault();
                if (entity != null && !string.IsNullOrWhiteSpace(entity.LoginHid) && entity.LoginUserid != null && !string.IsNullOrWhiteSpace(entity.LoginOpenid))
                {
                    if (DateTime.Now < loginDate.AddSeconds(10))//确认登录后还需要返回客户端完成自动登录，这个时间限制在10秒内。
                    {
                        if (entity.LoginType != 2)
                        {
                            if (_centerDb.WeixinOperatorHotelMappings.AsNoTracking().Where(c => c.Hid == entity.LoginHid && c.OperatorId == entity.LoginUserid && c.OperatorWxOpenId == entity.LoginOpenid).Any())
                            {
                                IsCheckWxAuthLogin = false;
                                return CheckLogin(entity.LoginHid, entity.LoginUserid.Value, entity.LoginOpenid);
                            }
                        }
                        else
                        {
                            IsCheckWxAuthLogin = true;
                            return CheckWxAuthLogin(entity.LoginHid, entity.LoginUserid.Value, entity.LoginOpenid);
                        }
                    }
                }
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "登录失败！" };
            }
            catch { }
            return new AccountInfo { LoginSuccess = false, ErrorMessage = "登录失败！" };
        }
        /// <summary>
        /// 执行登录流程
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="userid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public AccountInfo CheckLogin(string hid, Guid userid, string openid)
        {
            var _hotelInfoService = DependencyResolver.Current.GetService<IHotelInfoService>();
            //先获取hid对应的酒店信息
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            if (hotelInfo == null)
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
            DateTime dt = DateTime.Now.AddDays(-ExpiredLoginDayCount).AddHours(-14);
            if (!hotelInfo.ExpiryDate.HasValue || dt >= hotelInfo.ExpiryDate)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = ExpiredRemindContent };
            }
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            //转到对应的业务库里面判断用户名密码
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,isConnectViaInternet);
            var pmsDb = new DbHotelPmsContext(dbConnStr, hid, "weixinEvent", null);

            var user = pmsDb.PmsUsers.SingleOrDefault(w => w.Id == userid && w.Grpid == hid && w.Status < EntityStatus.禁用 && w.WxOpenId == openid);
            if (user == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户不存在或已禁用" };
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
                IsRegUser = user.IsReg == 1
            };
        }
        private AccountInfo CheckWxAuthLogin(string hid, Guid userid, string openid)
        {
            //取出openid对应的售后工程师
            var centerDb = DependencyResolver.Current.GetService<DbCommonContext>();
            var serviceOperator = centerDb.ServicesOperators.FirstOrDefault(w => w.LoginOpenid == openid && w.Status < EntityStatus.禁用);
            if (serviceOperator == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "售后工程师不正确" };
            }
            var _hotelInfoService = DependencyResolver.Current.GetService<IHotelInfoService>();
            //先获取hid对应的酒店信息
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            if (hotelInfo == null)
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
            DateTime dt = DateTime.Now.AddDays(-ExpiredLoginDayCount).AddHours(-14);
            if (!hotelInfo.ExpiryDate.HasValue || dt >= hotelInfo.ExpiryDate)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = ExpiredRemindContent };
            }
            //转到对应的业务库里面判断用户名密码
            var isConnectViaInternet = _hotelInfoService.IsConnectViaInternte();
            var dbConnStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "HotelPmsLogin",hotelInfo.DbServerInternet,isConnectViaInternet);
            var pmsDb = new DbHotelPmsContext(dbConnStr, hid, "weixinEvent", null);

            var user = pmsDb.PmsUsers.SingleOrDefault(w => w.Id == userid && w.Grpid == hid && w.Status < EntityStatus.禁用);
            if (user == null)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = "用户不存在或已禁用" };
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
                UserCode = string.Format("{0}授权给{1}", user.Code, serviceOperator.Code),
                UserName = string.Format("{0}授权给{1}", user.Name, serviceOperator.Name),
                IsRegUser = true
            };
        }

        #region 变更状态操作
        /// <summary>
        /// 变更状态操作 - 二维码过期
        /// </summary>
        /// <param name="id"></param>
        private void QrcodeInvalid(Guid id)
        {
            string sql = string.Format("update WeixinQrcodeLogin set [status] = {0} where id ='{1}' and [status] in ({2},{3})"
                , (int)WeixinQrcodeLoginStatus.QRCODE_INVALID
                , id
                , (int)WeixinQrcodeLoginStatus.QRCODE_WAIT
                , (int)WeixinQrcodeLoginStatus.QRCODE_SCANED
            );
            _centerDb.Database.ExecuteSqlCommand(sql);
        }
        /// <summary>
        /// 变更状态操作 - 扫码成功
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keyid"></param>
        private void QrcodeScaned(Guid id, Guid keyid)
        {
            string sql = string.Format("update WeixinQrcodeLogin set keyid = '{0}', [status] = {1} where id = '{2}' and [status] = {3}"
                , keyid
                , (int)WeixinQrcodeLoginStatus.QRCODE_SCANED
                , id
                , (int)WeixinQrcodeLoginStatus.QRCODE_WAIT
            );
            _centerDb.Database.ExecuteSqlCommand(sql);
        }
        /// <summary>
        /// 变更状态操作 - 没有绑定操作员
        /// </summary>
        /// <param name="id"></param>
        private void QrcodeEmptyAccount(Guid id)
        {
            string sql = string.Format("update WeixinQrcodeLogin set [status] = {0} where id = '{1}' and [status] = {2}"
                , (int)WeixinQrcodeLoginStatus.QRCODE_EMPTY_ACCOUNT
                , id
                , (int)WeixinQrcodeLoginStatus.QRCODE_SCANED
            );
            _centerDb.Database.ExecuteSqlCommand(sql);
        }
        /// <summary>
        /// 设置二维码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openId"></param>
        private void SetOpenId(Guid id, string openId)
        {
            _centerDb.Database.ExecuteSqlCommand("update WeixinQrcodeLogin set loginOpenid = @loginOpenid where id = @id and [status]=@status"
                , new SqlParameter("@loginOpenid", openId)
                , new SqlParameter("@id", id)
                , new SqlParameter("@status", (int)WeixinQrcodeLoginStatus.QRCODE_SCANED)
            );
        }
        /// <summary>
        /// 设置登录结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keyid"></param>
        /// <param name="hid"></param>
        /// <param name="userid"></param>
        /// <param name="openid"></param>
        /// <param name="isSuccess"></param>
        private void QrcodeLoginSuccess(Guid id, Guid keyid, string hid, Guid userid, string openid, bool isSuccess)
        {
            _centerDb.Database.ExecuteSqlCommand("update WeixinQrcodeLogin set loginHid = @loginHid, loginUserid = @loginUserid, loginDate = @loginDate, [status] = @newStatus where id = @id and keyid = @keyid and [status]=@status and substring(sys.fn_sqlvarbasetostr(HashBytes('MD5',loginOpenid)),3,32)=@loginOpenid"
                , new SqlParameter("@loginHid", hid)
                , new SqlParameter("@loginUserid", userid)
                , new SqlParameter("@loginDate", DateTime.Now)
                , new SqlParameter("@newStatus", isSuccess ? (int)WeixinQrcodeLoginStatus.QRCODE_LOGIN_SUCCESS : (int)WeixinQrcodeLoginStatus.QRCODE_REFUSE_LOGIN)

                , new SqlParameter("@id", id)
                , new SqlParameter("@keyid", keyid)
                , new SqlParameter("@status", (int)WeixinQrcodeLoginStatus.QRCODE_SCANED)
                , new SqlParameter("@loginOpenid", openid)
            );
        }
        private void QrcodeWxLoginSuccess(Guid id, Guid keyid, string hid, Guid userid, string openid, bool isSuccess)
        {
            _centerDb.Database.ExecuteSqlCommand("update WeixinQrcodeLogin set loginHid = @loginHid, loginUserid = @loginUserid, loginDate = @loginDate, [status] = @newStatus where id = @id and keyid = @keyid and [status]=@status and loginOpenid=@loginOpenid"
                , new SqlParameter("@loginHid", hid)
                , new SqlParameter("@loginUserid", userid)
                , new SqlParameter("@loginDate", DateTime.Now)
                , new SqlParameter("@newStatus", isSuccess ? (int)WeixinQrcodeLoginStatus.QRCODE_LOGIN_SUCCESS : (int)WeixinQrcodeLoginStatus.QRCODE_REFUSE_LOGIN)

                , new SqlParameter("@id", id)
                , new SqlParameter("@keyid", keyid)
                , new SqlParameter("@status", (int)WeixinQrcodeLoginStatus.QRCODE_SCANED)
                , new SqlParameter("@loginOpenid", openid)
            );
        }
        #endregion

        #region 操作员二维码确认
        /// <summary>
        /// 微信页面-确认二维码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        public JsonResultData GetConfirmQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity, out List<KeyValuePairModel<string, string>> hids)
        {
            entity = null; hids = null;
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }

            Guid _keyid = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(keyid, out _keyid))
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            string data = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }
            if (!(entity != null && entity.Id == _id && entity.Keyid == _keyid && entity.LoginDate == null && entity.LoginHid == null && entity.LoginOpenid == null && entity.LoginUserid == null))
            {
                return result;
            }

            var hidList = _centerDb.WeixinOperatorHotelMappings.Where(c => c.OperatorWxOpenId == openid).Select(c => c.Hid).Distinct().ToList();
            if (hidList == null || hidList.Count <= 0)
            {
                QrcodeEmptyAccount(entity.Id);
                result = JsonResultData.Failure("该微信号尚未绑定任何操作员<br>请先登录系统绑定操作员");
                return result;
            }

            hids = _centerDb.Hotels.Where(c => hidList.Contains(c.Hid)).Select(c => new KeyValuePairModel<string, string> { Key = c.Hid, Value = c.Name }).ToList();
            if (hids == null || hids.Count <= 0)
            {
                QrcodeEmptyAccount(entity.Id);
                result = JsonResultData.Failure("该微信号尚未绑定任何操作员<br>请先登录系统绑定操作员");
                return result;
            }

            SetOpenId(entity.Id, openid);
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId（md5加密后的openid）</param>
        /// <param name="hid">选择要登录的酒店ID</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        public JsonResultData SubmitConfirmQrcode(string id, string keyid, string openid, string hid, bool isSuccess)
        {
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }

            Guid _keyid = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(keyid, out _keyid))
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(hid))
            {
                return result;
            }
            var _hotelInfoService = DependencyResolver.Current.GetService<IHotelInfoService>();
            var hotelInfo = _hotelInfoService.GetHotelInfo(hid);
            var sendService = DependencyResolver.Current.GetService<ISysParaService>();
            Dictionary<string, string> ExpirePara = sendService.GetExpiredPara();
            string ExpiredRemindContent = ExpirePara["expiredremindcontent"];//到期提醒内容
            int ExpiredLoginDayCount = int.Parse(ExpirePara["expiredlogindaycount"]);//到期后仍能登录使用天数
            DateTime dt = DateTime.Now.AddDays(-ExpiredLoginDayCount).AddHours(-14);
            if (!hotelInfo.ExpiryDate.HasValue || dt >= hotelInfo.ExpiryDate)
            {
                return JsonResultData.Failure(ExpiredRemindContent);
            }
            string data = null; WeixinQrcodeLogin entity = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }
            if (!(entity != null && entity.Id == _id && entity.Keyid == _keyid && entity.LoginDate == null && entity.LoginHid == null && CryptHelper.EncryptMd5(entity.LoginOpenid) == openid && entity.LoginUserid == null))
            {
                return result;
            }

            var mapping = _centerDb.WeixinOperatorHotelMappings.Where(c => c.OperatorWxOpenId == entity.LoginOpenid && c.Hid == hid).SingleOrDefault();
            if (mapping == null)
            {
                QrcodeEmptyAccount(entity.Id);
                result = JsonResultData.Failure("该微信号尚未绑定任何操作员<br>请先登录系统绑定操作员");
                return result;
            }

            QrcodeLoginSuccess(entity.Id, entity.Keyid, mapping.Hid, mapping.OperatorId, openid, isSuccess);
            return JsonResultData.Successed();
        }
        #endregion

        #region 售后工程师二维码确认
        /// <summary>
        /// 微信页面-确认二维码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        public JsonResultData GetConfirmAuthQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity)
        {
            entity = null;
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }

            Guid _keyid = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(keyid, out _keyid))
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            string data = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }
            if (!(entity != null && entity.Id == _id && entity.Keyid == _keyid && entity.LoginDate == null && entity.LoginOpenid == null && entity.LoginUserid == null))
            {
                return result;
            }

            var serviceOperators = _centerDb.ServicesOperators.Where(c => c.LoginOpenid == openid).ToList();
            if (serviceOperators == null || serviceOperators.Count <= 0)
            {
                result = JsonResultData.Failure("该微信号尚未绑定任何售后工程师");
                result.ErrorCode = 99;
                return result;
            }
            SetOpenId(entity.Id, openid);

            return JsonResultData.Successed();
        }

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        public JsonResultData SubmitConfirmAuthQrcode(string id, string openid, bool isSuccess, AccountInfo accountInfo)
        {
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }


            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            string data = null; WeixinQrcodeLogin entity = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }

            QrcodeWxLoginSuccess(entity.Id, entity.Keyid, accountInfo.Hid, Guid.Parse(accountInfo.UserId), openid, isSuccess);
            return JsonResultData.Successed();
        }
        #endregion

        #region 运营后台二维码确认
        /// <summary>
        /// 微信页面-确认二维码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        public JsonResultData GetConfirmAdminQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity)
        {
            entity = null;
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }

            Guid _keyid = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(keyid, out _keyid))
            {
                return result;
            }

            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            string data = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }
            if (!(entity != null && entity.Id == _id && entity.Keyid == _keyid && entity.LoginDate == null && entity.LoginOpenid == null && entity.LoginUserid == null))
            {
                return result;
            }

            var sysUserId = _centerDb.Database.SqlQuery<string>("SELECT CONVERT(VARCHAR(60),id) AS id FROM sysUser WHERE WxOpenId = {0}", openid).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(sysUserId))
            {
                result = JsonResultData.Failure("该微信号尚未绑定任何运营人员");
                result.ErrorCode = 99;
                return result;
            }
            SetOpenId(entity.Id, openid);

            return JsonResultData.Successed();
        }

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        public JsonResultData SubmitConfirmAdminQrcode(string id, string openid, bool isSuccess, AccountInfo accountInfo)
        {
            var result = JsonResultData.Failure("参数错误");

            Guid _id = Guid.Empty;
            if (!Gemstar.BSPMS.Common.Tools.CommonHelper.ToGuid(id, out _id))
            {
                return result;
            }


            if (string.IsNullOrWhiteSpace(openid))
            {
                return result;
            }

            string data = null; WeixinQrcodeLogin entity = null;
            var status = GetQrcodeStatus(id, out data, out entity);
            if (status == WeixinQrcodeLoginStatus.QRCODE_INVALID)
            {
                result = JsonResultData.Failure("请求已过期");
                return result;
            }
            if (status != WeixinQrcodeLoginStatus.QRCODE_SCANED)
            {
                return result;
            }

            QrcodeWxLoginSuccess(entity.Id, entity.Keyid, accountInfo.Hid, Guid.Parse(accountInfo.UserId), openid, isSuccess);
            return JsonResultData.Successed();
        }
        #endregion


        #region 获取酒店集合
        /// <summary>
        /// 获取绑定酒店集合
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<BSPMS.Common.Tools.KeyValuePairModel<string, string>> GetHidlist(string openid)
        {
            if (string.IsNullOrWhiteSpace(openid)) { return null; }

            var list = from wx in _centerDb.WeixinOperatorHotelMappings
                       join hotel in _centerDb.Hotels on wx.Hid equals hotel.Hid
                       where wx.OperatorWxOpenId == openid
                       select new BSPMS.Common.Tools.KeyValuePairModel<string, string>
                       {
                           Key = wx.Hid,
                           Value = hotel.Name,
                           Data = wx.OperatorId.ToString(),
                       };

            return list.ToList();
        }
        #endregion
    }
}
