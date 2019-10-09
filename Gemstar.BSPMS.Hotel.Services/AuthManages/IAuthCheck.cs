using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 权限检查接口
    /// </summary>
    public interface IAuthCheck
    {
        /// <summary>
        /// 检查指定操作员是否拥有指定模块的指定按钮权限
        /// </summary>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="authCode">权限项id</param>
        /// <param name="authButtonValue">权限项按钮值</param>
        /// <returns>true:拥有权限,false:没有权限</returns>
        bool HasAuth(string userId, string authCode, long authButtonValue,string hid);
        /// <summary>
        /// 获取指定操作员在指定酒店有权限访问的第一级的菜单模块
        /// </summary>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>指定操作员在指定酒店有权限访问的第一级的菜单模块</returns>
        List<AuthList> GetFirstLevelAuths(string userId,string hid,ProductType productType,AuthType authType);
        /// <summary>
        /// 获取指定操作员在指定酒店有权限访问的指定菜单模块代码下面的子菜单模块
        /// </summary>
        /// <param name="parentAuthCode">指定的父菜单模块代码</param>
        /// <param name="userId">操作员id，唯一主键值</param>
        /// <param name="hid">酒店id</param>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">菜单类型</param>
        /// <returns>获取指定操作员在指定酒店有权限访问的指定菜单模块代码下面的子菜单模块</returns>
        List<AuthList> GetChildAuths(string parentAuthCode,string userId,string hid, ProductType productType, AuthType authType);
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<System.Web.Mvc.SelectListItem> GetBntAuths(string id);
        /// <summary>
        /// 清除缓存的角色权限数据，一般在重新设置了角色权限时调用，以便让新设置数据生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        void ClearHotelRoleAuthCache(string hid);
        /// <summary>
        /// 清除缓存的用户所属角色对应关系数据，一般在设置了用户所属角色，角色成员后调用，以便让新设置生效
        /// </summary>
        /// <param name="hid">酒店id</param>
        void ClearHotelUserRoleCache(string hid);
    }
}
