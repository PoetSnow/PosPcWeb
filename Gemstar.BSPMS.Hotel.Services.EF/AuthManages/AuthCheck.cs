using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages
{
    public class AuthCheck : IAuthCheck
    {
        public AuthCheck(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }
        public bool HasAuth(string userId, string authCode, long authButtonValue,string hid)
        {
            return _pmsContext.Database.SqlQuery<int>(
                "exec up_check_operatorAuth @authCode = @authCode, @authButtonValue = @authButtonValue, @userId = @userId,@hid=@hid"
                , new SqlParameter("@authCode", authCode)
                , new SqlParameter("@authButtonValue", authButtonValue)
                , new SqlParameter("@userId", userId)
                , new SqlParameter("@hid",hid)
                ).Single() == 1;
        }        
        /// <summary>
        /// 获取指定操作员在指定酒店有权限访问的第一级的菜单模块
        /// </summary>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>指定操作员在指定酒店有权限访问的第一级的菜单模块</returns>
        public List<AuthList> GetFirstLevelAuths(string userId, string hid, ProductType productType, AuthType authType)
        {
            //所有pcode=1的认为是一级菜单
            var authAll = GetAllAuths(userId,hid,authType);
            return authAll.Where(w => w.ParentCode == "1").ToList();
        }
        /// <summary>
        /// 获取指定操作员在指定酒店有权限访问的指定菜单模块代码下面的子菜单模块
        /// </summary>
        /// <param name="parentAuthCode">指定的父菜单模块代码</param>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>获取指定操作员在指定酒店有权限访问的指定菜单模块代码下面的子菜单模块</returns>
        public List<AuthList> GetChildAuths(string parentAuthCode, string userId, string hid, ProductType productType, AuthType authType)
        {
            var authAll = GetAllAuths(userId, hid, authType);
            return authAll.Where(w => w.ParentCode == parentAuthCode).ToList();
        }
        /// <summary>
        /// 获取有权限的全部菜单列表
        /// </summary>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>有权限的全部菜单列表</returns>
        private DbRawSqlQuery<AuthList> GetAllAuths(string userId, string hid, AuthType authType)
        {
            return _pmsContext.Database.SqlQuery<AuthList>("exec up_queryAuthListForOperator @userid=@userid,@hid=@hid,@authType=@authType", new SqlParameter("@userid", userId), new SqlParameter("@hid", hid), new SqlParameter("@authType", (int)authType));
        }
        public List<System.Web.Mvc.SelectListItem> GetBntAuths(string id)
        {
            var sql = string.Format("SELECT ('{0}_'+ CONVERT(VARCHAR(100),AuthButtonValue))  AS Value,AuthButtonName AS Text  FROM dbo.AuthButtons WHERE AuthButtonId NOT IN('Add','Query','Update','Enable','Disable','Delete','Details','Close')AND AuthId LIKE '{0}' + '%'", id);
           var data= _pmsContext.Database.SqlQuery<System.Web.Mvc.SelectListItem>(sql);
            return data.ToList();
        }
        /// <summary>
        /// 清除缓存的角色权限数据，一般在重新设置了角色权限时调用，以便让新设置数据生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        public void ClearHotelRoleAuthCache(string hid)
        {

        }
        /// <summary>
        /// 清除缓存的用户所属角色对应关系数据，一般在设置了用户所属角色，角色成员后调用，以便让新设置生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        public void ClearHotelUserRoleCache(string hid)
        {

        }
        private DbHotelPmsContext _pmsContext;
    }

}
