using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage
{
    public class MarketingManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MarketingManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MarketingManage_default",
                "MarketingManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}