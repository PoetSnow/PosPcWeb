using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin
{
    public class WeixinAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Weixin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Weixin_default",
                "Weixin/{controller}/{action}/{id}",
                new { controller="Listen",action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}