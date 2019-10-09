using System;
using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 授权他人登录服务接口
    /// </summary>
    public interface IServiceAuthorizeService
    {
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
        JsonResultData AddAuthorizeService(string grpId, string hid, DateTime beginDate, DateTime endDate,string userId, string userCode, string userName,out string authCode);
        /// <summary>
        /// 售后工程师使用授权码登录
        /// </summary>
        /// <param name="authCode">授权码</param>
        /// <param name="userCode">售后工程师登录代码</param>
        /// <param name="password">售后工程师登录明文密码</param>
        /// <returns>登录结果</returns>
        AccountInfo AuthLogin(string authCode, string userCode, string password);
        /// <summary>
        /// 修改指定售后服务工程师的登录密码
        /// </summary>
        /// <param name="userCode">售后工程师登录代码</param>
        /// <param name="originPassword">原始登录明文密码</param>
        /// <param name="change2Password">新的登录明文密码</param>
        /// <returns>更改结果</returns>
        JsonResultData ChangePassword(string userCode, string originPassword, string change2Password);

        /// <summary>
        /// 售后工程师使用授权微信扫码登录
        /// </summary>
        /// <param name="qrcodeId">二维码id</param>
        /// <param name="qrCodeKey">二维码key</param>
        /// <param name="openId">售后工程师openid</param>
        /// <returns>登录结果</returns>
        AccountInfo AuthWxLogin(string qrcodeId, string qrCodeKey, string openId);
    }
}
