using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 模块权限服务接口
    /// 用于读取模块权限和维护角色的模块权限
    /// </summary>
    public interface IAuthListService
    {
        /// <summary>
        /// 获取用于角色权限维护时，指定角色对指定酒店的权限列表
        /// </summary>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">权限类型</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        List<UpQueryAuthListForRoleResult> GetAllAuthLists(ProductType productType, AuthType authType, string hid, string roleId);

        /// <summary>
        /// 获取云POS高级功能权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        List<UpQueryAuthListForRoleResult> GetAdvanceFuncLists(string hid, string roleId);
        /// <summary>
        /// 重设指定角色在指定酒店内的所有权限
        /// </summary>
        /// <param name="productType">产品类型</param>
        /// <param name="roleId">角色id</param>
        /// <param name="hid">酒店id</param>
        /// <param name="auths">角色拥有的当前所有权限</param>
        /// <returns>重设权限结果</returns>
        JsonResultData ResetRoleAuths(ProductType productType, string roleId, string hid, List<AuthInfo> auths);
        /// <summary>
        /// 获取指定角色的操作权限
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="roleid">角色id</param>
        /// <returns> </returns>
        List<AuthInfo> Listrole(string hid, Guid roleid);
        string GetAuthNamebycode(string code);
        /// <summary>
        /// 父项编号是否为1，根部权限项
        /// </summary>
        /// <param name="authcode"></param>
        /// <returns></returns>
        bool isrootauth(string authcode);

        /// <summary>
        /// 获取用于角色权限维护时，指定角色对指定酒店的权限列表
        /// </summary>
        /// <param name="productType">产品类型</param>
        /// <param name="authType">权限类型</param>
        /// <param name="hid">酒店id</param>
        /// <param name="roleId">角色id</param>
        /// <param name="QueryText">模糊查询关键字</param>
        /// <returns></returns>
        List<UpQueryAuthListForRoleResult> GetAllAuthLists(ProductType productType, AuthType authType, string hid, string roleId,string QueryText);

        /// <summary>
        /// 根据用户获取用户对应的权限列表
        /// </summary>
        /// <param name="gid">集团ID</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        List<up_pos_list_roleAuthResult> GetUserAuthLists(string gid, string hid, string uid);
    }
    /// <summary>
    /// 角色授权时的权限信息
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// 模块权限id
        /// </summary>
        public string AuthCode { get;set;}
        /// <summary>
        /// 此模块权限下的所有有权限功能按钮的权限和
        /// </summary>
        public AuthFlag AuthValues { get; set; }
    }
}
