using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos早餐券
    /// </summary>
    [AuthPage(ProductType.Pos, "p200014")]
    public class PosBreakfastCouponController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        // GET: PosManage/PosBreakfastCoupon
        public ActionResult Index()
        {
            //当前用户信息
            PmsUser user = GetService<IPmsUserService>().Get(Guid.Parse(CurrentInfo.UserId));

            //产品
            var productService = GetService<IProductService>();
            var product = productService.GetHotelProducts(CurrentInfo.HotelId).ToList();
            string domain = "pms.gshis.com";
            if (product.Contains("pms"))
            {
                domain = GetService<IProductService>().GetProductByCode("pms").Domain;
            }

            if (product.Contains("HYHIS"))
            {
                domain = GetService<IProductService>().GetProductByCode("HYHIS").Domain;
            }

            //如果包含长租的则使用长租的域名
            if (product.Contains("Permanent"))
            {
                domain = GetService<IProductService>().GetProductByCode("Permanent").Domain;
            }

            string ServerAddresss = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.HotelId).ServerAddress;
            ServerAddresss = ServerAddresss == "" ? "" : ServerAddresss + ".";

            var url = $"http://{ServerAddresss + domain}/Account/PosLogin?hid={CurrentInfo.GroupHotelId}&hotelname={CurrentInfo.HotelName}&usercode={user.Code}&pwd={user.Pwd}&shiftid={CurrentInfo.ShiftId}&shiftname={CurrentInfo.ShiftName}";
            Response.Write("<script src = 'http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.0.js' ></script>");
            Response.Write("<script>var ahref=$(\"<a id='aaa' target='_blank' href='" + url + "'></a> \");$(ahref)[0].click();</script>");
            return null;
        }
    }
}