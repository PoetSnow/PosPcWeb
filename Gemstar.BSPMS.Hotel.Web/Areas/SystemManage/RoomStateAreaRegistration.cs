using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState
{
    public class RoomStateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RoomState";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RoomState_default",
                "RoomState/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}