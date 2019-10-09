using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 授权服务
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// 提交授权
        /// </summary>
        /// <param name="type">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="aubmitAuthInfo">授权内容</param>
        /// <param name="weixinSendFunc">发送微信</param>
        /// <returns>true提交成功，返回主键ID；false验证失败</returns>
        JsonResultData SubmitAuthorization(ICurrentInfo currentInfo, Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.SubmitAuthInfo submitAuthInfo, Func<Gemstar.BSPMS.Hotel.Services.WeixinManage.TemplateMessageInfo.SendAuthTemplateMessageModel, JsonResultData> weixinSendFunc);

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="id">主键ID</param>
        /// <returns>返回{Status,Message}；Status{-2：微信发送失败；-1：授权失败；0：继续等待；>0：授权成功，值为授权日期时间}</returns>
        JsonResultData GetAuthorization(ICurrentInfo currentInfo, Guid id);

        /// <summary>
        /// 验证并且更新授权信息
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="idAndTicks">主键ID+授权时间</param>
        /// <param name="keys">外部关联ID，例如订单regid，如果有值，则更新</param>
        /// <returns>true验证成功；false验证失败</returns>
        Gemstar.BSPMS.Hotel.Services.EntityProcedures.upJsonResultData<string> CheckAndUpdateAuthorization(ICurrentInfo currentInfo, string idAndTicks, string keys, out string reason);

        /// <summary>
        /// 获取微信授权人列表
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <returns>Key：授权人ID；Value：授权人姓名；</returns>
        List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<Guid, string>> GetWeiXinAuthorizationUsersToList(ICurrentInfo currentInfo, byte authType);

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="id">主键ID</param>
        void RevokeAuthorization(ICurrentInfo currentInfo, Guid id);
    }
}
