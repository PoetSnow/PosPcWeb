using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Controllers
{
    /// <summary>
    /// 报表列表
    /// </summary>
    [AuthPage("40010")]
    [AuthPage(ProductType.Member, "m40010")]
    [AuthPage(ProductType.Pos, "p50010")]
    public class ReportListController : BaseEditInWindowController<ReportFormat, IReportService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            Dictionary<string, string> ExpirePara = GetService<ISysParaService>().GetExpiredPara();
            string ExpiredRemindContent = ExpirePara["expiredremindcontent"]; 
            UpQueryHotelInfoByIdResult hotel = GetService<IHotelInfoService>().GetHotelInfo(CurrentInfo.HotelId);
            if (DateTime.Now >= hotel.ExpiryDate)
            {
                return Content(ExpiredRemindContent);
            }
            if (!GetService<IReportService>().IsReportauth(CurrentInfo.UserId, CurrentInfo.HotelId))
            {
                return Content("你没有可以查看的报表！");
            }
            var rservice = GetService<IReportService>();
            List<string> reporttype = rservice.GetReportType(CurrentInfo.ProductType);
            string typestr = "<span name='txtbtn' onclick='queryData(\"收藏\",this)' class='k-button' style='background-color:rgb(0,71,147);color:white;margin-right:6px;'/>我的收藏</span><span id='typeAll' name='txtbtn' onclick='queryData(\"\",this)' class='k-button' />全部</span> ";//spanbt  
            foreach (var item in reporttype)
            {
                typestr += "<span name='txtbtn' onclick='queryData(\"" + item + "\",this)' class='k-button'/>" + item + "</span> ";
            }
            ViewBag.tpstring = typestr;
            ViewBag.Userid = CurrentInfo.UserId;
            var cur = GetService<ICurrentInfo>();
            ViewBag.IsHotelInGroup = CurrentInfo.IsHotelInGroup;
            //IsHotelInGroup是否集团酒店             
            SetCommonQueryValues("up_list_reportlist", "@h99用户编号=" + CurrentInfo.UserId + "&@IsHotelInGroup=" + CurrentInfo.IsHotelInGroup + "&@t00报表类型=收藏");
            return View();
        }
        #endregion

        #region 报表收藏

       [AuthButton(AuthFlag.Query)]
        public ActionResult reportCollect(string reportCode, bool isCollect)
        {
            GetService<IReportService>().setUserReportCollect(reportCode, isCollect, Guid.Parse(CurrentInfo.UserId));
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}