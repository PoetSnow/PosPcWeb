using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage
{
    /// <summary>
    /// 预订管理
    /// </summary>
    public class ResManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ResManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ResManage_default",
                "ResManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}