using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage
{
    /// <summary>
    /// 单体酒店角色成员视图模型
    /// </summary>
    public class AuthManageViewModelGroup : AuthManageViewModel
    {
        /// <summary>
        /// 当前集团下的所有酒店
        /// </summary>
        public List<PmsHotel> Hotels { get; set; }
        /// <summary>
        /// 同时将权限应用到的酒店id，以逗号分隔
        /// </summary>
        public string ApplyHotelIds { get; set; }
    }
}