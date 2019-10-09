using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Common.Tools;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages
{
    /// <summary>
    /// 授权服务
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private DbHotelPmsContext _pmsContext;
        public AuthorizationService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }

        /// <summary>
        /// 提交授权
        /// </summary>
        /// <param name="type">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="aubmitAuthInfo">授权内容</param>
        /// <param name="weixinSendFunc">发送微信</param>
        /// <returns>true提交成功，返回主键ID；false验证失败</returns>
        public JsonResultData SubmitAuthorization(ICurrentInfo currentInfo, Gemstar.BSPMS.Hotel.Services.AuthManages.AuthorizationInfo.SubmitAuthInfo submitAuthInfo, Func<Gemstar.BSPMS.Hotel.Services.WeixinManage.TemplateMessageInfo.SendAuthTemplateMessageModel, JsonResultData> weixinSendFunc)
        {
            #region 验证参数
            if(currentInfo == null || string.IsNullOrWhiteSpace(currentInfo.HotelId) || string.IsNullOrWhiteSpace(currentInfo.UserId))
            {
                return JsonResultData.Failure("当前登录用户信息错误！");
            }
            if(submitAuthInfo == null)
            {
                return JsonResultData.Failure("授权参数错误！");
            }
            if (submitAuthInfo.AuthMode == 1)
            {
                submitAuthInfo.Userid = null;
                if (string.IsNullOrWhiteSpace(submitAuthInfo.LoginName) || string.IsNullOrWhiteSpace(submitAuthInfo.LoginPassword))
                {
                    return JsonResultData.Failure("请输入授权登录名和密码！");
                }
            }
            else if (submitAuthInfo.AuthMode == 2)
            {
                submitAuthInfo.LoginName = null;
                submitAuthInfo.LoginPassword = null;
                if (submitAuthInfo.Userid == null || submitAuthInfo.Userid.HasValue == false)
                {
                    return JsonResultData.Failure("请选择微信授权用户！");
                }
            }
            else
            {
                return JsonResultData.Failure("授权模式错误！");
            }
            if (!AuthorizationInfo.CheckAuthType(submitAuthInfo.AuthType))
            {
                return JsonResultData.Failure("授权类型错误！");
            }
            if (string.IsNullOrWhiteSpace(submitAuthInfo.AuthContent))
            {
                return JsonResultData.Failure("授权内容错误！");
            }
            if (string.IsNullOrWhiteSpace(submitAuthInfo.AuthReason))
            {
                return JsonResultData.Failure("请输入授权原因！");
            }
            string authContent = AuthorizationInfo.GetAuthContentByWeixin(submitAuthInfo.AuthType, submitAuthInfo.AuthContent, submitAuthInfo.AuthReason);
            if(string.IsNullOrWhiteSpace(authContent))
            {
                return JsonResultData.Failure("授权内容错误！");
            }
            #endregion

            #region 授权人
            Guid authUserId = Guid.Empty;
            string openId = null;
            if (submitAuthInfo.AuthMode == 1)//登录授权
            {
                var accountService = DependencyResolver.Current.GetService<Services.IAccountService>();
                var accountResult = accountService.Login(currentInfo.GroupHotelId, submitAuthInfo.LoginName, submitAuthInfo.LoginPassword);
                if (accountResult != null && accountResult.LoginSuccess == true && !string.IsNullOrWhiteSpace(accountResult.UserId))
                {
                    authUserId = Guid.Parse(accountResult.UserId);
                }
                else
                {
                    return JsonResultData.Failure(string.Format("授权账户登录失败，原因：{0}！", accountResult.ErrorMessage));
                }
            }
            else if(submitAuthInfo.AuthMode == 2)//微信授权
            {
                var pmsUserEntity = _pmsContext.PmsUsers.AsNoTracking().Where(c => c.Grpid == currentInfo.GroupHotelId && c.Id == submitAuthInfo.Userid && c.Status == EntityStatus.启用 && c.WxOpenId != null && c.WxOpenId.Trim().Length > 0).AsNoTracking().FirstOrDefault();
                if(pmsUserEntity != null)
                {
                    authUserId = pmsUserEntity.Id;
                    openId = pmsUserEntity.WxOpenId;
                }
                else
                {
                    return JsonResultData.Failure("微信授权用户不存在或没有绑定微信！");
                }
            }
            if(authUserId == null || authUserId == Guid.Empty)
            {
                return JsonResultData.Failure("获取授权人失败！");
            }
            #endregion

            #region 验证授权人权限
            bool isAuth = false;
            var userEntity = _pmsContext.PmsUsers.Where(c => c.Grpid == currentInfo.GroupHotelId && c.Id == authUserId && c.Status == EntityStatus.启用).AsNoTracking().FirstOrDefault();
            if (userEntity != null)
            {
                if (userEntity.IsReg == 1)
                {
                    isAuth = true;
                }
                else
                {
                    var roleids = _pmsContext.UserRoles.AsNoTracking().Where(c => c.Hid == currentInfo.HotelId && c.Userid == userEntity.Id).Select(c => c.Roleid).ToList();
                    if (roleids != null && roleids.Count > 0)
                    {
                        string authCode = "";
                        long authButtonValue = -1;
                        AuthorizationInfo.GetAuthority(submitAuthInfo.AuthType, out authCode, out authButtonValue);
                        isAuth = _pmsContext.RoleAuths.AsNoTracking().Where(c => c.Hid == currentInfo.HotelId && roleids.Contains(c.RoleId) && c.AuthCode == authCode && (c.AuthButtonValue & authButtonValue) == authButtonValue).Any();
                    }
                }
            }
            if(isAuth == false)
            {
                return JsonResultData.Failure("授权人没有权限！");
            }
            #endregion

            #region 保存
            AuthorizationRecord addEntity = new AuthorizationRecord();
            addEntity.Id = Guid.NewGuid();
            addEntity.Hid = currentInfo.HotelId;
            addEntity.Mode = submitAuthInfo.AuthMode;
            addEntity.Type = submitAuthInfo.AuthType;
            addEntity.Content = submitAuthInfo.AuthContent;
            addEntity.Reason = submitAuthInfo.AuthReason;
            addEntity.CreateUserId = Guid.Parse(currentInfo.UserId);
            addEntity.CreateDate = DateTime.Now;
            addEntity.AuthUserId = authUserId;
            if (addEntity.Mode == 1)
            {
                addEntity.AuthStatus = 1;
                addEntity.AuthDate = DateTime.Now;
            }
            else if(addEntity.Mode == 2)
            {
                addEntity.AuthStatus = 0;
            }
            _pmsContext.AuthorizationRecords.Add(addEntity);
            _pmsContext.SaveChanges();
            #endregion

            #region 发送微信
            if (submitAuthInfo.AuthMode == 2)
            {
                string title = AuthorizationInfo.GetAuthTypeName(addEntity.Type);
                string url = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/Weixin/Authorization/Index/";
                var sendResult = weixinSendFunc(new Services.WeixinManage.TemplateMessageInfo.SendAuthTemplateMessageModel
                {
                    AuthApplicant = currentInfo.UserName,
                    AuthApplicantDateTime = DateTime.Now,
                    Hid = currentInfo.HotelId,
                    HotelName = currentInfo.HotelName,
                    Keyid = addEntity.Id,
                    Openid = openId,
                    Type = addEntity.Type,
                    Title = title,
                    AuthContent = authContent,
                    Url = url,
                });
                if (sendResult.Success == false)
                {
                    return sendResult;
                }
            }
            #endregion

            return JsonResultData.Successed(addEntity.Id);
        }

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="id">主键ID</param>
        /// <returns>返回{Status,Message}；Status{-2：微信发送失败；-1：授权失败；0：继续等待；>0：授权成功，值为授权日期时间}</returns>
        public JsonResultData GetAuthorization(ICurrentInfo currentInfo, Guid id)
        {
            Guid createUserId = Guid.Parse(currentInfo.UserId);
            var entity = _pmsContext.AuthorizationRecords.Where(c => c.Hid == currentInfo.HotelId && c.Id == id && c.CreateUserId == createUserId).Select(c => new { c.AuthDate, c.AuthStatus, c.Mode, c.Id, c.RevokeDate }).AsNoTracking().FirstOrDefault();
            if(entity == null)
            {
                return JsonResultData.Failure("找不到授权信息！");
            }
            if (entity.RevokeDate != null && entity.RevokeDate.HasValue == true)
            {
                return JsonResultData.Failure("授权请求已取消！");
            }
            if (entity.AuthStatus == 1)
            {
                long? ticks = 0;
                if (entity.AuthDate != null && entity.AuthDate.HasValue && entity.AuthDate.Value != DateTime.MinValue)
                {
                    ticks = entity.AuthDate.Value.Ticks;
                }
                return JsonResultData.Successed(new { Status = ticks, Message = "" });//成功
            }
            else if (entity.AuthStatus == 2)
            {
                return JsonResultData.Successed(new { Status = -1, Message = "" });//失败
            }
            else
            {
                if(entity.Mode == 2)//微信授权
                {
                    var messageEntity = _pmsContext.WeixinTemplateMessages.Where(c => c.Hid == currentInfo.HotelId && c.Keyid == entity.Id).AsNoTracking().FirstOrDefault();
                    if(messageEntity == null)
                    {
                        return JsonResultData.Successed(new { Status = -2, Message = "微信发送失败！" });//失败
                    }
                    if(!string.IsNullOrWhiteSpace(messageEntity.SendStatus) && messageEntity.SendStatus != "0")
                    {
                        return JsonResultData.Successed(new { Status = -2, Message = string.Format("微信发送失败！<br />{0}", messageEntity.SendMsg) });//失败
                    }
                    else if(!string.IsNullOrWhiteSpace(messageEntity.SendFinishStatus) && messageEntity.SendFinishStatus != "success")
                    {
                        return JsonResultData.Successed(new { Status = -2, Message = string.Format("微信发送失败！<br />{0}！", messageEntity.SendFinishMsg) });//失败
                    }
                    else
                    {
                        var sendMsg = (messageEntity.SendStatus == "0") ? "微信消息已发送" : messageEntity.SendMsg;
                        var sendFinishMsg = (messageEntity.SendFinishStatus == "success") ? "微信消息已送达" : messageEntity.SendFinishMsg;
                        var split = (!string.IsNullOrWhiteSpace(sendMsg) && !string.IsNullOrWhiteSpace(sendFinishMsg)) ? "，" : "";
                        return JsonResultData.Successed(new { Status = 0, Message = sendMsg + split + sendFinishMsg + "！" });//没有结果，继续等待结果
                    }
                }
            }
            return JsonResultData.Successed(new { Status = 0, Message = "" });//没有结果，继续等待结果
        }

        /// <summary>
        /// 验证并且更新授权信息
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="idAndTicks">主键ID+授权时间</param>
        /// <param name="keys">外部关联ID，例如订单regid，如果有值，则更新</param>
        /// <returns>true验证成功；false验证失败</returns>
        public upJsonResultData<string> CheckAndUpdateAuthorization(ICurrentInfo currentInfo, string idAndTicks, string keys,out string reason)
        {
            reason = "";
            #region 验证参数
            if (currentInfo == null || string.IsNullOrWhiteSpace(currentInfo.HotelId) || string.IsNullOrWhiteSpace(currentInfo.UserId))
            {
                return new upJsonResultData<string> { Success = false };
            }
            Guid userid = Guid.Empty;
            Guid.TryParse(currentInfo.UserId, out userid);
            if(userid == null || userid == Guid.Empty)
            {
                return new upJsonResultData<string> { Success = false };
            }
            Guid authId = Guid.Empty;
            DateTime authDate = DateTime.MinValue;
            bool isCheck = CheckIdAndTicks(idAndTicks, out authId, out authDate);
            if (isCheck == false)
            {
                return new upJsonResultData<string> { Success = false };
            }
            #endregion

            string sql = @"
                            begin
                            declare @authUserId uniqueidentifier = null;
                            select @authUserId=authUserId from AuthorizationRecord where hid=@hid and createUserId=@createUserId and id=@id and authDate=@authDate and authStatus=1 and revokeDate is null;
                            if(@authUserId is not null and LEN(CAST(@authUserId as varchar(60))) = 36)
                            begin
                            update AuthorizationRecord set keys=@keys where hid=@hid and id=@id and authStatus=1 and len(isnull(@keys,''))>0;	
                            select top 1 CAST('1' as bit) as Success, Name as Data from pmsUser where id=@authUserId; return;
                            end
                            select top 1 CAST('0' as bit) as Success, '' as Data; return;
                            end
                            ";
            var result = _pmsContext.Database.SqlQuery<upJsonResultData<string>>(sql,
                new SqlParameter("@hid", currentInfo.HotelId),
                new SqlParameter("@createUserId", userid),
                new SqlParameter("@id", authId),
                new SqlParameter("@authDate", authDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                new SqlParameter("@keys", string.IsNullOrWhiteSpace(keys) ? "" : keys)
            ).FirstOrDefault();
            var entity = _pmsContext.AuthorizationRecords.Where(w => w.Id == authId).FirstOrDefault();
            if(entity!= null)
            {
                var type = gAuthTypeName(entity.Type);
                if(!string.IsNullOrWhiteSpace(type))
                {
                    reason = type + "原因：" + entity.Reason;
                }
                
            }
            if (result == null)
            {
                result = new upJsonResultData<string> { Success = false };
            }

            return result;
        }
        /// <summary>
        /// 获取授权类型名称
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <returns></returns>
        private string gAuthTypeName(byte authType)
        {
            switch (authType)
            {
                case 1:
                    return "调价授权";
                case 2:
                    return "减免授权";
                case 3:
                    return "冲销授权";
                case 4:
                    return "房租加收修改授权";
                default:
                    return "";
            }
        }
        /// <summary>
        /// 检查并获取
        /// </summary>
        /// <param name="idAndTicks">主键ID+授权时间戳</param>
        /// <param name="authId">主键ID</param>
        /// <param name="authDateTime">授权时间</param>
        /// <returns></returns>
        private bool CheckIdAndTicks(string idAndTicks, out Guid authId, out DateTime authDateTime)
        {
            authId = Guid.Empty;
            authDateTime = DateTime.MinValue;
            long ticks = -2;
            if (!string.IsNullOrWhiteSpace(idAndTicks))
            {
                if (idAndTicks.Length > 37)
                {
                    Guid.TryParse(idAndTicks.Substring(0, 36), out authId);
                    Int64.TryParse(idAndTicks.Substring(37), out ticks);
                }
            }
            if (authId != null && authId != Guid.Empty && ticks > 0)
            {
                authDateTime = new DateTime(ticks);
                if(authDateTime != null && authDateTime != DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取微信授权人列表
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <returns>Key：授权人ID；Value：授权人姓名；</returns>
        public List<KeyValuePairModel<Guid, string>> GetWeiXinAuthorizationUsersToList(ICurrentInfo currentInfo, byte authType)
        {
            List<KeyValuePairModel<Guid, string>> resultList = new List<KeyValuePairModel<Guid, string>>();
            //酒店ID
            string hid = currentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid) && !AuthorizationInfo.CheckAuthType(authType))
            {
                return resultList;
            }
            //权限
            string authCode = "";
            long authButtonValue = -1;
            AuthorizationInfo.GetAuthority(authType, out authCode, out authButtonValue);
            //获取数据
            var roleids = _pmsContext.RoleAuths.AsNoTracking().Where(c => c.Hid == hid && c.AuthCode == authCode && (c.AuthButtonValue & authButtonValue) == authButtonValue).Select(c => c.RoleId).Distinct().ToList();
            if (roleids != null && roleids.Count > 0)
            {
                var userids = _pmsContext.UserRoles.AsNoTracking().Where(c => c.Hid == hid && roleids.Contains(c.Roleid)).Select(c => c.Userid).Distinct().ToList();
                if (userids != null && userids.Count > 0)
                {
                    resultList.AddRange(_pmsContext.PmsUsers.AsNoTracking().Where(c => c.Grpid == currentInfo.GroupHotelId && userids.Contains(c.Id) && (c.WxOpenId != null && c.WxOpenId.Trim().Length > 0) && c.Status == EntityStatus.启用).Select(c => new KeyValuePairModel<Guid, string> { Key = c.Id, Value = c.Name }).Distinct().ToList());
                }
            }
            //如果是酒店的注册用户，则直接拥有权限
            resultList.AddRange(_pmsContext.PmsUsers.AsNoTracking().Where(c => c.Grpid == currentInfo.GroupHotelId && c.IsReg == 1 && (c.WxOpenId != null && c.WxOpenId.Trim().Length > 0) && c.Status == EntityStatus.启用).Select(c => new KeyValuePairModel<Guid, string> { Key = c.Id, Value = c.Name }).ToList());
            return resultList.Distinct().ToList();
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="currentInfo">当前登录用户信息</param>
        /// <param name="id">主键ID</param>
        public void RevokeAuthorization(ICurrentInfo currentInfo, Guid id)
        {
            Guid createUserId = Guid.Parse(currentInfo.UserId);
            var entity = _pmsContext.AuthorizationRecords.Where(c => c.Hid == currentInfo.HotelId && c.Id == id && c.CreateUserId == createUserId).FirstOrDefault();
            if(entity != null && entity.AuthStatus == 0)
            {
                entity.RevokeDate = DateTime.Now;
                _pmsContext.Entry(entity).State = EntityState.Unchanged;
                _pmsContext.Entry(entity).Property("RevokeDate").IsModified = true;
                _pmsContext.SaveChanges();
            }
        }
    }
}
