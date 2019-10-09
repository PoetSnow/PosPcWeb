using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages {
    public class AuthCheckViaMemoryCache : IAuthCheck
    {
        public AuthCheckViaMemoryCache(DbHotelPmsContext pmsContext, ICurrentInfo currentInfo, HttpContextBase httpContextBase)
        {
            _pmsContext = pmsContext;
            _currentInfo = currentInfo;
            _httpContext = httpContextBase;
        }
        public List<SelectListItem> GetBntAuths(string id)
        {
            var sql = string.Format("SELECT ('{0}_'+ CONVERT(VARCHAR(100),AuthButtonValue))  AS Value,AuthButtonName AS Text  FROM dbo.AuthButtons WHERE AuthButtonId NOT IN('Add','Query','Update','Enable','Disable','Delete','Details','Close')AND AuthId LIKE '{0}' + '%'", id);
            var data = _pmsContext.Database.SqlQuery<System.Web.Mvc.SelectListItem>(sql);
            return data.ToList();
        }

        /// <summary>
        /// 获取有权限的全部菜单列表
        /// </summary>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="loadTime">加载次数</param>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>有权限的全部菜单列表</returns>
        private List<AuthList> GetAllAuths(string userId, string hid,ProductType productType, AuthType authType, int loadTime)
        {
            var allAuths = GetAllAuths();
            //先取出当前菜单类型下的所有菜单项
            var allAuthsInType = new List<AuthList>();
            switch (authType)
            {
                case AuthType.Group:
                    allAuthsInType = allAuths.Where(w => w.IsGroup == 1).ToList();
                    break;
                case AuthType.GroupHotel:
                    allAuthsInType = allAuths.Where(w => w.IsGroupHotel == 1).ToList();
                    break;
                case AuthType.SingleHotel:
                    allAuthsInType = allAuths.Where(w => w.IsHotel == 1).ToList();
                    break;
            }
            int index = (int)productType;
            allAuthsInType = allAuthsInType.Where(w => w.Mask[index - 1] == '1').ToList();
            if (productType == ProductType.Pms) {
                //得到是否启用业主功能的参数值
                var PmsPara = _pmsContext.PmsParas.Where(w => w.Hid == hid && w.Code == "isAllowOwner").FirstOrDefault();
                var isAllowOwner = "";
                if (PmsPara != null) {
                    isAllowOwner = PmsPara.Value;
                }
                if (isAllowOwner != "1") {
                    allAuthsInType = allAuthsInType.Where(w => !w.AuthName.Contains("业主")).ToList();
                }
            }
            //如果是酒店注册用户，则直接返回所有对应的菜单项
            if (_currentInfo.IsRegUser)
            {
                return allAuthsInType;
            }
            //如果是普通用户，则先取出所属角色有权限的项
            var hotelUserRoles = GetHotelUserRoles(hid);
            var userIdValue = Guid.Parse(userId);
            var roleIds = hotelUserRoles.Where(w => w.Userid == userIdValue).Select(w => w.Roleid).ToList();
            var hotelRoleAuths = GetHotelRoleAuths(hid);
            var roleAuthCodes = hotelRoleAuths.Where(w => roleIds.Contains(w.RoleId)).Select(w => w.AuthCode).ToList();
            var userAuths = allAuthsInType.Where(w => roleAuthCodes.Contains(w.AuthCode)).ToList();
            foreach (var code in roleAuthCodes)
            {
                var auth = allAuthsInType.SingleOrDefault(w => w.AuthCode == code);
                if (auth == null)
                {
                    if (loadTime < 2)
                    {
                        //表示此菜单项是新增加的，而缓存的全局的菜单项中还没有此项，所以此处直接清空全局的菜单项缓存，然后重新执行此方法
                        _httpContext.Cache.Remove(AuthListKey);
                        return GetAllAuths(userId, hid,productType, authType, loadTime + 1);
                    }
                    else
                    {
                        //如果已经是第2次加载全部数据后，仍然查找不到，则表示是由于数据调整了，直接跳过此菜单项
                        continue;
                    }
                }
                var parentCode = auth.ParentCode;
                while (!string.IsNullOrWhiteSpace(parentCode))
                {
                    var parent = allAuthsInType.SingleOrDefault(w => w.AuthCode == parentCode);
                    if (parent == null)
                    {
                        parentCode = "";
                    }
                    else
                    {
                        //存在父接点，判断是否已经存在用户有权限模块中，没有则加入
                        var exists = userAuths.SingleOrDefault(w => w.AuthCode == parentCode);
                        if (exists == null)
                        {
                            userAuths.Add(parent);
                        }
                        parentCode = parent.ParentCode;
                    }
                }
            }
            //删除一级菜单有权限，但其下任何模块都没有权限的项
            var firstLevelCodes = userAuths.Where(w => w.ParentCode == "1").Select(w => w.AuthCode).ToList();
            foreach (var code in firstLevelCodes)
            {
                var count = userAuths.Count(w => w.ParentCode == code);
                if (count == 0)
                {
                    var delete = userAuths.SingleOrDefault(w => w.AuthCode == code);
                    userAuths.Remove(delete);
                }
            }
            return userAuths.OrderBy(w => new AuthCompare { ParentCode = w.ParentCode, SeqId = w.Seqid }).ToList();
        }
        public List<AuthList> GetChildAuths(string parentAuthCode, string userId, string hid, ProductType productType, AuthType authType)
        {

            var authAll = GetAllAuths(userId, hid,productType, authType, 1);
            return authAll.Where(w => w.ParentCode == parentAuthCode).OrderBy(w => w.Seqid).ToList();

        }

        public List<AuthList> GetFirstLevelAuths(string userId, string hid, ProductType productType, AuthType authType)
        {
            //所有pcode=0的认为是根菜单，根菜单下的直属菜单则是一级菜单
            var authAll = GetAllAuths(userId, hid,productType, authType, 1);
            var root = authAll.FirstOrDefault(w => w.ParentCode == "0");
            if (root != null) {
                return authAll.Where(w => w.ParentCode == root.AuthCode).OrderBy(w => w.Seqid).ToList();
            }
            return new List<AuthList>();
        }

        public bool HasAuth(string userId, string authCode, long authButtonValue, string hid)
        {
            //如果是酒店的注册用户，则直接拥有权限
            if (_currentInfo.IsRegUser)
            {
                return true;
            }
            //检查指定操作员的所有角色是否拥有指定模块的权限
            var hotelUserRoles = GetHotelUserRoles(hid);
            var userRoles = hotelUserRoles.Where(w => w.Userid == Guid.Parse(userId)).ToList();
            if (userRoles.Count == 0)
            {
                //不属于任何角色，认为是没有权限
                return false;
            }
            var userRoleIds = userRoles.Select(w => w.Roleid).ToList();
            var hotelRoleAuths = GetHotelRoleAuths(hid);
            var userAuths = hotelRoleAuths.Where(w => userRoleIds.Contains(w.RoleId) && w.AuthCode == authCode).ToList();
            foreach (var auth in userAuths)
            {
                if ((auth.AuthButtonValue & authButtonValue) == authButtonValue)
                {
                    //拥有模块和按钮权限
                    return true;
                }
            }
            return false;
        }
        private List<AuthList> GetAllAuths()
        {
            var cachedAuths = _httpContext.Cache[AuthListKey] as List<AuthList>;
            if (cachedAuths == null)
            {
                _httpContext.Cache.Remove(AuthListKey);

                var auths = _pmsContext.AuthLists.AsNoTracking().ToList();
                _httpContext.Cache.Add(AuthListKey, auths, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                cachedAuths = auths;
            }
            return cachedAuths;
        }
        private List<UserRole> GetHotelUserRoles(string hid)
        {
            var key = GetHotelCacheKey(hid, UserRoleKey, false);
            var cachedUserRoles = _httpContext.Cache[key] as List<UserRole>;
            var ticksKey = GetHotelCacheKey(hid, UserRoleKey, true);
            var cacheTick = _httpContext.Cache[ticksKey];
            var loginTick = _currentInfo.LoginTimeTicks;
            var needReload = true;
            if (cachedUserRoles != null && cacheTick != null && !string.IsNullOrWhiteSpace(loginTick))
            {
                long cacheTickValue, loginTickValue;
                if (long.TryParse(cacheTick.ToString(), out cacheTickValue))
                {
                    if (long.TryParse(loginTick, out loginTickValue))
                    {
                        //只有在有缓存，并且缓存的时间大于登录时间时，才直接使用缓存，其他情况下都重新加载
                        needReload = loginTickValue > cacheTickValue;
                    }
                }
            }
            if (needReload) {
                _httpContext.Cache.Remove(ticksKey);
                _httpContext.Cache.Remove(key);

                var hotelUserRoles = _pmsContext.UserRoles.AsNoTracking().Where(w => w.Hid == hid).ToList();
                _httpContext.Cache.Add(ticksKey, DateTime.UtcNow.Ticks.ToString(), null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                _httpContext.Cache.Add(key, hotelUserRoles, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                cachedUserRoles = hotelUserRoles;
            }
            return cachedUserRoles;
        }
        private string GetHotelCacheKey(string hid,string ContentType,bool isForTicks) {
            if (isForTicks) {
                return string.Format("{0}_ticks_{1}", ContentType, hid);
            }
            return string.Format("{0}_{1}", ContentType, hid);
        }
        private List<RoleAuth> GetHotelRoleAuths(string hid)
        {
            var key = GetHotelCacheKey(hid,RoleAuthKey,false);
            var ticksKey = GetHotelCacheKey(hid,RoleAuthKey,true);
            var cachedRoleAuths = _httpContext.Cache[key] as List<RoleAuth>;
            var cacheTick = _httpContext.Cache[ticksKey];
            var loginTick = _currentInfo.LoginTimeTicks;
            var needReload = true;
            if (cachedRoleAuths != null && cacheTick != null && !string.IsNullOrWhiteSpace(loginTick))
            {
                long cacheTickValue, loginTickValue;
                if (long.TryParse(cacheTick.ToString(), out cacheTickValue))
                {
                    if (long.TryParse(loginTick, out loginTickValue))
                    {
                        //只有在有缓存，并且缓存的时间大于登录时间时，才直接使用缓存，其他情况下都重新加载
                        needReload = loginTickValue > cacheTickValue;
                    }
                }
            }
            if (needReload)
            {
                _httpContext.Cache.Remove(ticksKey);
                _httpContext.Cache.Remove(key);

                var hotelUserRoles = _pmsContext.RoleAuths.AsNoTracking().Where(w => w.Hid == hid).ToList();
                _httpContext.Cache.Add(ticksKey, DateTime.UtcNow.Ticks.ToString(), null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                _httpContext.Cache.Add(key, hotelUserRoles, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), CacheItemPriority.Default, null);
                cachedRoleAuths = hotelUserRoles;
            }
            return cachedRoleAuths;
        }

        /// <summary>
        /// 清除缓存的角色权限数据，一般在重新设置了角色权限时调用，以便让新设置数据生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        public void ClearHotelRoleAuthCache(string hid) {
            //由于存在多台服务器，如果只是简单的清除缓存，则存在只能清除一台服务器缓存，导致有时有权限，有时没有的问题，修改为修改会话信息中的登录时间，以便让所有服务器遇到请求时，强制重新从数据库加载数据
            ResetLoginTicket();
        }
        /// <summary>
        /// 清除缓存的用户所属角色对应关系数据，一般在设置了用户所属角色，角色成员后调用，以便让新设置生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        public void ClearHotelUserRoleCache(string hid)
        {
            //由于存在多台服务器，如果只是简单的清除缓存，则存在只能清除一台服务器缓存，导致有时有权限，有时没有的问题，修改为修改会话信息中的登录时间，以便让所有服务器遇到请求时，强制重新从数据库加载数据
            ResetLoginTicket();
        }
        private void ResetLoginTicket() {
            _currentInfo.LoginTimeTicks = DateTime.UtcNow.Ticks.ToString();
            _currentInfo.SaveValues();
        }

        private DbHotelPmsContext _pmsContext;
        private HttpContextBase _httpContext;
        private ICurrentInfo _currentInfo;
        private const string AuthListKey = "AuthListKey";
        private const string UserRoleKey = "UserRoleKey";
        private const string RoleAuthKey = "RoleAuthKey";
        private class AuthCompare : IComparable
        {
            public string ParentCode { get; set; }
            public int? SeqId { get; set; }

            public int CompareTo(object obj)
            {
                var compare = obj as AuthCompare;
                if (compare != null)
                {
                    var result = ParentCode.CompareTo(compare.ParentCode);
                    if (result == 0)
                    {
                        if (SeqId.HasValue)
                        {
                            return SeqId.Value.CompareTo(compare.SeqId);
                        }
                    }
                    return result;
                }
                return 0;
            }
        }
    }
}
