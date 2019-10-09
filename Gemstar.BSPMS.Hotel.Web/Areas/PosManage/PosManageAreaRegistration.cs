using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage
{
    public class PosManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PosManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PosManage_default",
                "PosManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}