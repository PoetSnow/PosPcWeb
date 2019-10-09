using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter
{
    public class MbrCardCenterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MbrCardCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MbrCardCenter_default",
                "MbrCardCenter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}