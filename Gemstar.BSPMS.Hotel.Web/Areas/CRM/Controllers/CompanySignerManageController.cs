using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.CRMManage;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 合约单位签单人
    /// </summary>
    [AuthPage("60030002002")]
    public class CompanySignerManageController : BaseEditIncellController<CompanySigner, ICompanySignerService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string companyId)
        {
            ViewBag.companyId = companyId ?? "";
            return PartialView();
        }

        #region 执行ajax数据查询
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxQuerySigner(string companyId, [DataSourceRequest]DataSourceRequest request)
        {
            Guid idValue;
            if(!Guid.TryParse(companyId,out idValue))
            {
                idValue = Guid.Empty;
            }
            var queryService = GetService<ICompanySignerService>();
            var result = queryService.GetSignerByCompany(CurrentInfo.HotelId, idValue);
            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanySigner> addVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            return _Add(request, addVersions, w => { w.Id = Guid.NewGuid(); w.Hid = hid; }, OpLogType.合约单位签单人增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CompanySigner> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<CompanySigner> originVersions)
        {
            return _Update(request, updatedVersions, originVersions, (list, u) => list.SingleOrDefault(w => w.Id == u.Id), OpLogType.合约单位签单人修改);
            
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var result = _BatchDelete(id, GetService<ICompanySignerService>(), OpLogType.合约单位签单人删除);
            return result;
        }
        #endregion
    }
}