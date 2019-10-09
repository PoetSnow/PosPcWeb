using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 授权信息类
    /// </summary>
    public class AuthorizationInfo
    {
        /// <summary>
        /// 提交授权信息类
        /// </summary>
        public class SubmitAuthInfo
        {
            /// <summary>
            /// 授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）
            /// </summary>
            public string AuthContent { get; set; }
            /// <summary>
            /// 授权原因（调价授权，则调价原因；冲销授权，则冲销原因。等等）
            /// </summary>
            public string AuthReason { get; set; }
            /// <summary>
            /// 授权模式（1：登录授权；2：微信授权）
            /// </summary>
            public byte AuthMode { get; set; }
            /// <summary>
            /// 登录授权-登录名
            /// </summary>
            public string LoginName { get; set; }
            /// <summary>
            /// 登录授权-登录密码
            /// </summary>
            public string LoginPassword { get; set; }
            /// <summary>
            /// 微信授权-用户ID
            /// </summary>
            public Guid? Userid { get; set; }
            /// <summary>
            /// 授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）
            /// </summary>
            public byte AuthType { get; set; }
        }

        /// <summary>
        /// 授权类型
        /// </summary>
        public enum AuthType
        {
            调价授权申请 = 1,
            减免授权申请 = 2,
            冲销授权申请 = 3,
            房租加收修改授权申请 = 4,
        }
        /// <summary>
        /// 检查授权类型
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <returns></returns>
        public static bool CheckAuthType(byte authType)
        {
            if(authType == 1 || authType == 2 || authType == 3 || authType == 4)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取授权类型名称
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <returns></returns>
        public static string GetAuthTypeName(byte authType)
        {
            switch (authType)
            {
                case 1:
                    return "调价授权申请";
                case 2:
                    return "减免授权申请";
                case 3:
                    return "冲销授权申请";
                case 4:
                    return "房租加收修改授权申请";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取授权内容-调价
        /// </summary>
        /// <param name="authContent">授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）</param>
        /// <returns></returns>
        public static List<Gemstar.BSPMS.Hotel.Services.ResManage.ResAdjustPriceInfo.AdjustPriceResultModel> GetAuthContentByAdjustPrice(string authContent)
        {
            try
            {
                var contentEntity = JsonHelper.SerializeObject<List<Gemstar.BSPMS.Hotel.Services.ResManage.ResAdjustPriceInfo.AdjustPriceResultModel>>(authContent);
                if (contentEntity != null && contentEntity.Count > 0)
                {
                    foreach (var entity in contentEntity)
                    {
                        if (string.IsNullOrWhiteSpace(entity.RateCodeId) || string.IsNullOrWhiteSpace(entity.RoomTypeId) || string.IsNullOrWhiteSpace(entity.RegId))
                        {
                            return null;
                        }
                        if (entity.AdjustPriceList != null && entity.AdjustPriceList.Count > 0)
                        {
                            foreach (var item in entity.AdjustPriceList)
                            {
                                if (item.Price == null || item.Price.HasValue == false)
                                {
                                    return null;
                                }
                            }
                        }
                    }
                    return contentEntity;
                }
            }
            catch { }
            return null; 
        }
        /// <summary>
        /// 获取授权内容-减免
        /// </summary>
        /// <param name="authContent">授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）</param>
        /// <returns></returns>
        public static Gemstar.BSPMS.Hotel.Services.ResFolioManage.ResFolioAbatementInfo GetAuthContentByAbatement(string authContent)
        {
            try
            {
                var contentEntity = JsonHelper.SerializeObject<Gemstar.BSPMS.Hotel.Services.ResFolioManage.ResFolioAbatementInfo>(authContent);
                if (contentEntity != null)
                {
                    if (string.IsNullOrWhiteSpace(contentEntity.RegId) || string.IsNullOrWhiteSpace(contentEntity.ItemId))
                    {
                        return null;
                    }
                    return contentEntity;
                }
            }
            catch { }
            return null;
        }
        /// <summary>
        /// 获取授权内容-房租加收修改
        /// </summary>
        /// <param name="authContent">授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）</param>
        /// <returns></returns>
        public static List<Gemstar.BSPMS.Hotel.Services.ResFolioManage.ResFolioDayChargeInfo> GetAuthContentByDayCharge(string authContent)
        {
            try
            {
                var contentEntity = JsonHelper.SerializeObject<List<Gemstar.BSPMS.Hotel.Services.ResFolioManage.ResFolioDayChargeInfo>>(authContent);
                if (contentEntity != null && contentEntity.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(contentEntity[0].RegId))
                    {
                        return null;
                    }
                    return contentEntity;
                }
            }
            catch { }
            return null;
        }
        /// <summary>
        /// 获取授权内容-通用
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <param name="authContent">授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）</param>
        /// <returns></returns>
        public static object GetAuthContent(byte authType, string authContent)
        {
            if(authType == 1)
            {
                return GetAuthContentByAdjustPrice(authContent);
            }
            else if(authType == 2)
            {
                return GetAuthContentByAbatement(authContent);
            }
            else if (authType == 4)
            {
                return GetAuthContentByDayCharge(authContent);
            }
            return null;
        }
        /// <summary>
        /// 获取授权内容-微信
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <param name="authContent">授权内容（此内容是从服务器获取到然后保存在页面上的（序列化的字符串），不允许修改。）</param>
        /// <param name="authReason">授权原因</param>
        public static string GetAuthContentByWeixin(byte authType, string authContent, string authReason)
        {
            string msg = "单击详情确认授权!";
            decimal? price = 999999999;
            string reason = Gemstar.BSPMS.Common.Tools.CommonHelper.ToDBC(authReason, true);
            if (authType == 1)
            {
                var contentEntity = GetAuthContentByAdjustPrice(authContent);
                if(contentEntity != null)
                {
                    foreach (var item in contentEntity)
                    {
                        if(item.AdjustPriceList != null && item.AdjustPriceList.Count > 0)
                        {
                            var tempValue = item.AdjustPriceList.Min(c => c.Price);
                            if(tempValue < price)
                            {
                                price = tempValue;
                            }
                        }
                    }
                    if(price == 999999999) { price = 0; }
                    return string.Format("{3}调价-价格:{0},原因:{1},{2}", price, reason, msg, Gemstar.BSPMS.Common.Extensions.EnumExtension.GetDescription(contentEntity[0].Source));
                }
            }
            else if(authType == 2)
            {
                var contentEntity = GetAuthContentByAbatement(authContent);
                if(contentEntity != null)
                {
                    price = contentEntity.Amount;
                    return string.Format("客人账务减免-价格{0},原因:{1},{2}", price, reason, msg);
                }
            }
            else if (authType == 4)
            {
                var contentEntity = GetAuthContentByDayCharge(authContent);
                if (contentEntity != null)
                {
                    price = contentEntity.Select(c => c.Amount).Min();
                    return string.Format("客人房租加收修改-价格{0},原因:{1},{2}", price, reason, msg);
                }
            }
            return null;
        }
        /// <summary>
        /// 根据授权类型获取权限
        /// </summary>
        /// <param name="authType">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）</param>
        /// <param name="authCode">权限码</param>
        /// <param name="authButtonValue">权限值</param>
        /// <returns></returns>
        public static bool GetAuthority(byte authType,out string authCode, out long authButtonValue)
        {
            authCode = "";
            authButtonValue = -1;
            if (authType == 1)//客情调价授权
            {
                authCode = "2002010";
                authButtonValue = (long)Gemstar.BSPMS.Hotel.Services.AuthManages.AuthFlag.AdjustPrice;
                return true;
            }
            else if (authType == 2 || authType == 3)//客账冲销减免授权
            {
                authCode = "2002020";
                authButtonValue = (long)Gemstar.BSPMS.Hotel.Services.AuthManages.AuthFlag.AbatementFolio;
                return true;
            }
            else if (authType == 4)//房租加收修改授权
            {
                authCode = "2002020";
                authButtonValue = (long)Gemstar.BSPMS.Hotel.Services.AuthManages.AuthFlag.Accounting;
                return true;
            }
            return false;
        }


    }
}
