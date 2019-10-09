using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder
{
    public class ScanOrderAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ScanOrder";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ScanOrder_default",
                "ScanOrder/{controller}/{action}/{id}",
                new { controller = "Order", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}