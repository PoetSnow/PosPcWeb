using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom
{
    public class PermanentRoomAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PermanentRoom";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PermanentRoom_default",
                "PermanentRoom/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}