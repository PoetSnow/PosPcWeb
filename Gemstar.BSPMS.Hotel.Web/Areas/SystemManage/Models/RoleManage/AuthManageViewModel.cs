using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage
{
    /// <summary>
    /// 角色权限视图模型
    /// </summary>
    public class AuthManageViewModel
    {
        /// <summary>
        /// 根节点模块,以及其下的所有子模块，角色有权限的则默认选中
        /// </summary>
        public TreeViewItemModel RootAuth { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
    }
}