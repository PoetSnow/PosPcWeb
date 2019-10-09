using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage
{
    /// <summary>
    /// 单体酒店角色成员视图模型
    /// </summary>
    public class RoleMemberViewModelGroup:RoleMemberViewModel
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 当前集团下的所有酒店
        /// </summary>
        public List<PmsHotel> Hotels { get; set; }
        /// <summary>
        /// 集团下指定角色的所有成员
        /// </summary>
        public List<UserRole> RoleMembers { get; set; }
        /// <summary>
        /// 集团下的所有操作员列表
        /// </summary>
        public List<PmsUser> GroupUsers { get; set; }
    }
}