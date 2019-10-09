using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogExceptionAttribute());
        }
    }
}