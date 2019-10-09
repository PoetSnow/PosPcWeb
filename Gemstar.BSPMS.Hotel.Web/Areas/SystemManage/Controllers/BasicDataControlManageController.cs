using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 分发型基础资料分发属性设置 
    /// </summary>
    [AuthPage("99130")]
    [BusinessType("资料分发设置")]
    public class BasicDataControlManageController : BaseEditIncellController<BasicDataResortControl, IBasicDataResortControlService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            //同步分发型基础资料到集团设置中
            var hotelInfoService = GetService<IHotelInfoService>();
            var basicDatasForCopy = hotelInfoService.GetBasicDataForCopy();
            var basicDataService = GetService<IBasicDataResortControlService>();
            basicDataService.SyncBasicDatasForCopy(basicDatasForCopy, CurrentInfo.GroupId);
            //设置分发类型下拉数据项
            var dataControlTypes = basicDataService.GetDataControlTypes();
            var dataCopyTypeList = new SelectList(dataControlTypes, "Code","Name");
            ViewData["DataCopyType_Data"] = dataCopyTypeList;
            //设置查询存储过程
            SetCommonQueryValues("up_list_basicDataResortControl", "");
            return View();
        }
        #endregion


        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BasicDataResortControl> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<BasicDataResortControl> originVersions)
        {
            return _Update(request, updatedVersions, originVersions, (list, u) => list.SingleOrDefault(w => w.Id == u.Id), OpLogType.资料分发设置);
        }
        #endregion

    }
}
