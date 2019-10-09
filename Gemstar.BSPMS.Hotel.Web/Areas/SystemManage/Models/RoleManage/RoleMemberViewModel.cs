using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage
{
    /// <summary>
    /// 单体酒店角色成员视图模型
    /// </summary>
    public class RoleMemberViewModel
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 非指定角色成员的操作员列表
        /// </summary>
        public List<PmsUser> UsersNotInRole { get; set; }
        /// <summary>
        /// 属于指定角色成员的操作员列表
        /// </summary>
        public List<PmsUser> UsersInRole { get; set; }
    }
}