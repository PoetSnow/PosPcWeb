using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PayManage
{
    /// <summary>
    /// 支付管理区域，所有的支付方式处理都在此区域下进行管理
    /// </summary>
    public class PayManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PayManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PayManage_default",
                "PayManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}