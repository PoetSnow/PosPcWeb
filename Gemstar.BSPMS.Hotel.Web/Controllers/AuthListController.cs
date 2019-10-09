using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    /// <summary>
    /// 菜单显示，用于主界面上显示一级，二级菜单
    /// </summary>
    [NotAuth]
    public class AuthListController : BaseController
    {
        public ActionResult FirstLevelAuth(string hid)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                hid = CurrentInfo.HotelId;
            }
            var isGroup = CurrentInfo.IsGroup;
            var authType = AuthType.SingleHotel;
            if (isGroup)
            {
                authType = hid == CurrentInfo.GroupId ? AuthType.Group : AuthType.GroupHotel;
            }
            var authService = GetService<IAuthCheck>();
            var auths = authService.GetFirstLevelAuths(CurrentInfo.UserId, hid, CurrentInfo.ProductType, authType);

            #region 长包房功能开关

            if (auths != null && auths.Count > 0)
            {
                var permanentRoomMenu = auths.Where(c => c.AuthCode == "21").FirstOrDefault();
                if (permanentRoomMenu != null)//是否有长租管理功能权限
                {
                    if (!IsPermanentRoom(hid))//是否启用长租管理功能
                    {
                        auths.Remove(permanentRoomMenu);
                    }
                }
            }

            #endregion 长包房功能开关

            return PartialView("_FirstLevelAuth", auths);
        }

        public ActionResult ChildrenAuth(string parentCode)
        {
            try
            {
                if (CurrentInfo.ProductType == ProductType.Pms || CurrentInfo.ProductType == ProductType.Permanent || CurrentInfo.ProductType == ProductType.Pos)
                {
                    if (parentCode.Equals("p45", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //会员中心
                        PmsUser user = GetService<IPmsUserService>().Get(Guid.Parse(CurrentInfo.UserId));

                        //产品
                        var productService = GetService<IProductService>();
                        var product = productService.GetHotelProducts(CurrentInfo.HotelId).ToList();
                        string domain = "";
                        if (product.Contains("pms"))
                        {
                            domain = GetService<IProductService>().GetProductByCode("member").Domain;

                        }
                        else if (product.Contains("HYHIS"))
                        {
                            domain = GetService<IProductService>().GetProductByCode("hmember").Domain;
                        }
                        else
                        {
                            domain = GetService<IProductService>().GetProductByCode("member").Domain;
                        }
                        
                       
                        string ServerAddresss = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.HotelId).ServerAddress;
                        ServerAddresss = ServerAddresss == "" ? "" : ServerAddresss + ".";

                        var posServer = GetService<IPosPosService>();
                        var pos = posServer.GetPosByHid(CurrentInfo.HotelId, CurrentInfo.PosId);
                        var url = $"http://{ServerAddresss + domain}/Account/AutoLogin?hid={CurrentInfo.GroupHotelId}&usercode={user.Code}&hotelname={CurrentInfo.HotelName}&pwd={user.Pwd}&shiftid={CurrentInfo.ShiftId}&shiftname={CurrentInfo.ShiftName}&curHid={CurrentInfo.HotelId}&code={CurrentInfo.UserCode}&name={CurrentInfo.UserName}&pointcode={pos.CodeIn}&pointname={pos.Name}";
                        Response.Write("<script>var ahref=$(\"<a id='aaa' target='_blank' href='" + url + "'></a> \");$(ahref)[0].click();</script>");
                        return null;
                    }
                    else if (parentCode == "p90")
                    {
                        //合约单位                    
                        PmsUser user = GetService<IPmsUserService>().Get(Guid.Parse(CurrentInfo.UserId));
                        string domain = GetService<IProductService>().GetProductByCode("corp").Domain;
                        string ServerAddresss = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.HotelId).ServerAddress;
                        ServerAddresss = ServerAddresss == "" ? "" : ServerAddresss + ".";
                        Response.Write("<script>var aahref=$(\"<a id='aa' target='_blank' href='http://" + ServerAddresss + domain + "/Account/AutoLogin?hid=" + CurrentInfo.GroupHotelId + "&usercode=" + user.Code + "&hotelname=" + CurrentInfo.HotelName + "&pwd=" + user.Pwd + "&shiftid=" + CurrentInfo.ShiftId + "&shiftname=" + CurrentInfo.ShiftName + "&curHid=" + CurrentInfo.HotelId + "&code=" + CurrentInfo.UserCode + "&name=" + CurrentInfo.UserName + "'></a> \");$(aahref)[0].click();</script>");
                    }
                }
                var authService = GetService<IAuthCheck>();
                var auths = authService.GetChildAuths(parentCode, CurrentInfo.UserId, CurrentInfo.HotelId, CurrentInfo.ProductType, CurrentInfo.AuthListType);
                var isDirtyLogToReportForm = GetService<Services.IPmsParaService>().IsDirtyLogToReportForm(CurrentInfo.HotelId);//是否启用脏房转净房生成报表


                //是否启用云Pos扫码点餐功能
                var ScanOrderSwitch = "0";
                var scan = GetService<Services.IPmsParaService>().GetPmsParas(CurrentInfo.HotelId).Where(w => w.Code == "PosScanOrderSwitch").FirstOrDefault();
                if (scan != null)
                {
                    ScanOrderSwitch = scan.Value.ToString();
                }

                for (var i = 0; i < auths.Count; i++)
                {
                    var a = auths[i];
                    if (a.AuthCode == "20020")
                    {
                        //如果是新预订/入住菜单，则将其拆分成两个菜单项
                        a.AuthName = "新预订";
                        var b = new AuthList
                        {
                            Action = "IIndex",
                            Area = "ResManage",
                            AuthCode = "20020",
                            AuthName = "新入住",
                            Controller = "ResOrderAdd",
                            IsGroup = 1,
                            IsGroupHotel = 1,
                            IsHotel = 1,
                            ParentCode = "20",
                            Seqid = 30
                        };
                        auths.Insert(i + 1, b);
                        break;
                    }
                    if (a.Action == "LookSignature")
                    {
                        var signature = IsHasAuth("40030", 2251799813685248);
                        a.AuthCode = a.AuthCode + "|" + signature.ToString();
                    }
                    if (!isDirtyLogToReportForm && a.Area == "Percentages" && a.Controller == "CleanRoomPolicy")
                    {
                        auths.Remove(a);
                        i--;
                    }
                    if (a.AuthCode == "p200012")//扫码点餐开关
                    {
                        var posService = GetService<IPosPosService>();
                        var posModel = posService.Get(CurrentInfo.PosId);
                        if (posModel.IsBrushOrder == true)
                        {
                            a.AuthName = "关闭扫码点餐";
                        }
                        else
                        {
                            a.AuthName = "开启扫码点餐";
                        }

                    }
                    if (ScanOrderSwitch == "0" && (a.AuthCode == "p99115" || a.AuthCode == "p200012"))
                    {
                        auths.Remove(a);
                    }
                }
                return PartialView("_ChildrenAuth", auths);
            }
            catch (System.Exception e)
            {
                //说明：当退出登录，用浏览器返回时，这里会异常（因为连接数据的字符串为空）从而不能到登录页面,所以这里强制跳转到登录页面
                //浏览器返回会缓存一部分页面数据（home/main）,会加载iframe里面的数据
                return Content("<script>window.location.href='" + FormsAuthentication.LoginUrl + "'</script>");
            }
        }

        /// <summary>
        /// 通用的以tab方式显示第三级菜单的页面
        /// </summary>
        /// <param name="id">二级菜单id</param>
        /// <returns></returns>
        public ActionResult TabstripAuth(string id)
        {
            var authService = GetService<IAuthCheck>();
            var auths = authService.GetChildAuths(id, CurrentInfo.UserId, CurrentInfo.HotelId, CurrentInfo.ProductType, CurrentInfo.AuthListType);
            return View(auths);
        }

        /// <summary>
        /// 由于以tab方式加载内容后，所有的内容都是合并在一起的，这样不行，需要使用iframe来区分开，否则之前做好封装的那些通用查询等全部都使用不了
        /// </summary>
        /// <param name="url">真实页面地址</param>
        /// <returns></returns>
        public ActionResult TabstripContent(string url)
        {
            ViewBag.url = url;
            return PartialView("_TabstripContent");
        }
    }
}