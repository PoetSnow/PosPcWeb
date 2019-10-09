using System;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 授权他人登录服务实现
    /// </summary>
    public class ServiceAuthorizeService: IServiceAuthorizeService
    {
        private DbCommonContext _db;
        public ServiceAuthorizeService(DbCommonContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 增加授权他人登录记录
        /// </summary>
        /// <param name="grpId">集团id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="beginDate">生效时间</param>
        /// <param name="endDate">失效时间</param>
        /// <param name="userId">用户id</param>
        /// <param name="userCode">用户代码</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="authCode">授权码</param>
        /// <returns>增加结果</returns>
        public JsonResultData AddAuthorizeService(string grpId, string hid, DateTime beginDate, DateTime endDate,string userId, string userCode, string userName, out string authCode)
        {
            try
            {
                authCode = _db.Database.SqlQuery<string>("exec up_add_servicesAuthorize @grpId=@grpId,@hid=@hid,@beginDate=@beginDate,@endDate=@endDate,@userId=@userId,@userCode=@userCode,@userName=@userName"
                    , new SqlParameter("@grpId",grpId??"")
                    , new SqlParameter("@hid",hid??"")
                    , new SqlParameter("@beginDate",beginDate)
                    , new SqlParameter("@endDate",endDate)
                    , new SqlParameter("@userId",userId??"")
                    , new SqlParameter("@userCode",userCode??"")
                    , new SqlParameter("@userName",userName??"")
                    ).SingleOrDefault();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                authCode = "";
                return JsonResultData.Failure(ex);
                throw;
            }
        }
        /// <summary>
        /// 售后工程师使用授权码登录
        /// </summary>
        /// <param name="authCode">授权码</param>
        /// <param name="userCode">售后工程师登录代码</param>
        /// <param name="password">售后工程师登录明文密码</param>
        /// <returns>登录结果</returns>
        public AccountInfo AuthLogin(string authCode, string userCode, string password)
        {
            try
            {
                var info = _db.Database.SqlQuery<AccountInfo>("exec up_add_servicesAuthorizeLogin @authCode=@authCode,@userCode=@userCode,@password=@password"
                    , new SqlParameter("@authCode",authCode??"")
                    , new SqlParameter("@userCode",userCode??"")
                    , new SqlParameter("@password",PasswordHelper.GetEncryptedPassword(userCode,password))
                    ).SingleOrDefault();
                if(info == null)
                {
                    return new AccountInfo{LoginSuccess = false,ErrorMessage = "授权码没有对应到任意有效的酒店信息"};
                }

                //处理服务器地址前面的http://
                var serverAddress = info.WebServerUrl;
                if (!serverAddress.StartsWith("http://"))
                {
                    info.WebServerUrl = string.Format("http://{0}", serverAddress);
                }
                return info;
            }
            catch (Exception ex)
            {
                return new AccountInfo{LoginSuccess = false,ErrorMessage = ex.Message};
            }
        }
        /// <summary>
        /// 修改指定售后服务工程师的登录密码
        /// </summary>
        /// <param name="userCode">售后工程师登录代码</param>
        /// <param name="originPassword">原始登录明文密码</param>
        /// <param name="change2Password">新的登录明文密码</param>
        /// <returns>更改结果</returns>
        public JsonResultData ChangePassword(string userCode, string originPassword, string change2Password)
        {
            try
            {
                var serviceOperator = _db.ServicesOperators.SingleOrDefault(w => w.Code == userCode);
                if(serviceOperator == null)
                {
                    return JsonResultData.Failure(string.Format("指定的售后工程师代码{0}不存在",userCode));
                }
                var originEncryptedPassword = PasswordHelper.GetEncryptedPassword(userCode, originPassword);
                if (!originEncryptedPassword.Equals(serviceOperator.Pwd))
                {
                    return JsonResultData.Failure("旧密码错误");
                }
                var change2EncryptedPassword = PasswordHelper.GetEncryptedPassword(userCode, change2Password);
                serviceOperator.Pwd = change2EncryptedPassword;
                _db.Entry(serviceOperator).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                return JsonResultData.Successed();
            }catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 售后工程师使用授权微信扫码登录，需要注意将userid从固定值currentHotelAdminUserId转换为实际的值，在登录成功后，有当前酒店id后，需要转去取一次系统管理员的值
        /// </summary>
        /// <param name="qrcodeId">二维码id</param>
        /// <param name="qrCodeKey">二维码key</param>
        /// <param name="openId">售后工程师openid</param>
        /// <returns>登录结果</returns>
        public AccountInfo AuthWxLogin(string qrcodeId, string qrCodeKey, string openId)
        {
            try
            {
                var info = _db.Database.SqlQuery<AccountInfo>("exec up_add_servicesAuthorizeWxLogin @qrcodeId=@qrcodeId,@qrcodeKey=@qrcodeKey,@openId=@openId"
                    , new SqlParameter("@qrcodeId", qrcodeId ?? "")
                    , new SqlParameter("@qrcodeKey", qrCodeKey ?? "")
                    , new SqlParameter("@openId", openId)
                    ).SingleOrDefault();
                if (info == null)
                {
                    return new AccountInfo { LoginSuccess = false, ErrorMessage = "授权码没有对应到任意有效的酒店信息" };
                }

                //处理服务器地址前面的http://
                var serverAddress = info.WebServerUrl;
                if (!serverAddress.StartsWith("http://"))
                {
                    info.WebServerUrl = string.Format("http://{0}", serverAddress);
                }
                return info;
            }
            catch (Exception ex)
            {
                return new AccountInfo { LoginSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
