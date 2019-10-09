using System;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.WeixinManage
{
    public interface IQrcodeLoginService
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="loginType">登录类型，1：操作员扫码登录，2：售后工程师扫码登录</param>
        /// <param name="authCode">授权码，如果是售后工程师扫码登录则需要先输入授权码</param>
        /// <param name="keyid">返回二维码键Id[WeixinQrcodeLogin.Keyid]</param>
        /// <returns>返回二维码主键ID[WeixinQrcodeLogin.Id]</returns>
        string GenerateQrcode(byte loginType, string authCode, out string keyid);

        /// <summary>
        /// 获取二维码键ID
        /// </summary>
        /// <param name="id">主键ID[WeixinQrcodeLogin.Id]</param>
        /// <returns>返回二维码键Id[WeixinQrcodeLogin.Keyid]</returns>
        string GetQrcodeKeyIdById(string id);

        /// <summary>
        /// 获取二维码状态
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="data">其他信息</param>
        /// <returns></returns>
        WeixinQrcodeLoginStatus GetQrcodeStatus(string id, out string data, out WeixinQrcodeLogin entity);

        /// <summary>
        /// 获取二维码地址
        /// </summary>
        /// <param name="keyid">键ID</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        string GetQrcodeUrl(string keyid, out string id);

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="jump">{WeixinQrcodeLogin.Id}{WeixinQrcodeLogin.Keyid}{loginDate.LoginDate.Ticks}</param>
        /// <param name="IsCheckWxAuthLogin">是否售后工程师登录 默认False</param>
        Gemstar.BSPMS.Common.Services.EntityProcedures.AccountInfo Login(string jump, out bool IsCheckWxAuthLogin);

        #region 操作员二维码登录确认
        /// <summary>
        /// 微信页面-确认验证码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        JsonResultData GetConfirmQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity, out List<BSPMS.Common.Tools.KeyValuePairModel<string, string>> hids);

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId（md5加密后的openid）</param>
        /// <param name="hid">选择要登录的酒店ID</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        JsonResultData SubmitConfirmQrcode(string id, string keyid, string openid, string hid, bool isSuccess);
        #endregion


        #region 售后工程师二维码登录确认
        /// <summary>
        /// 微信页面-确认验证码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        JsonResultData GetConfirmAuthQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity);

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId（md5加密后的openid）</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        JsonResultData SubmitConfirmAuthQrcode(string id, string openid, bool isSuccess, AccountInfo accountInfo);

        #endregion

        #region 运营后台二维码登录确认
        /// <summary>
        /// 微信页面-确认验证码显示
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId</param>
        /// <param name="entity">返回实体</param>
        /// <param name="hids">返回OpenId绑定酒店列表</param>
        /// <returns></returns>
        JsonResultData GetConfirmAdminQrcode(string id, string keyid, string openid, out WeixinQrcodeLogin entity);

        /// <summary>
        /// 微信页面-确认二维码提交
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="keyid">键ID</param>
        /// <param name="openid">微信OpenId（md5加密后的openid）</param>
        /// <param name="isSuccess">确认登录 或者 拒绝登录</param>
        /// <returns></returns>
        JsonResultData SubmitConfirmAdminQrcode(string id, string openid, bool isSuccess, AccountInfo accountInfo);

        #endregion

        /// <summary>
        /// 执行登录流程
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="userid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        AccountInfo CheckLogin(string hid, Guid userid, string openid);


        /// <summary>
        /// 获取绑定酒店集合
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        List<BSPMS.Common.Tools.KeyValuePairModel<string, string>> GetHidlist(string openid);
    }
}
