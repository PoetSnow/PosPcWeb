using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages
{
    public class PercentagesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Percentages";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Percentages_default",
                "Percentages/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}