using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    [AuthPage(ProductType.Pos, "p99141")]
    public class VoucherManageController : BaseEditIncellController<VoucherSet, IPosVoucherSetService>
    {
        #region  收入科目设置
        [AuthButton(AuthFlag.Query)]
        public ActionResult VoucherSettingA()
        {
            SetCommonQueryValues("up_pos_list_voucherSetA", "");
            return View();
        }
        #endregion

        #region  付款方式科目设置
        [AuthButton(AuthFlag.Query)]
        public ActionResult VoucherSettingB()
        {
            SetCommonQueryValues("up_pos_list_voucherSetB", "");
            return View();
        }

        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VoucherSet> Versions, [Bind(Prefix = "originModels")]IEnumerable<VoucherSet> originVersions)
        {
            try
            {
                IEnumerable<VoucherSet> updatedVersions = new List<VoucherSet>();
                IEnumerable<VoucherSet> addVersions = new List<VoucherSet>();
                DbHotelPmsContext db = GetHotelDb(CurrentInfo.HotelId);
                foreach (var model in Versions)
                {
                    string id = model.Typeid;
                    var v = db.VoucherSet.Where(w => w.Typeid == id).FirstOrDefault();
                    model.TypeSub = model.TypeSub ?? "";
                    if (v != null)
                    {
                        (updatedVersions as List<VoucherSet>).Add(model);
                        continue;
                    }
                    model.Type = "餐饮" + model.Type;
                    (addVersions as List<VoucherSet>).Add(model);
                }

                _Add(request, addVersions, w => { w.Id = Guid.NewGuid(); w.Hid = CurrentInfo.HotelId; }, OpLogType.凭证设置);


                if (updatedVersions.Count() > 0)
                {
                    List<VoucherSet> originvoucherSets = originVersions as List<VoucherSet>;
                    if (originvoucherSets != null)
                    {
                        if (originvoucherSets[0] == null)
                        {
                            List<VoucherSet> updatedvoucherSets = updatedVersions as List<VoucherSet>;
                            if (updatedvoucherSets != null && updatedvoucherSets[0].Type == "合约单位")
                            {
                                var s = db.Database.SqlQuery<VoucherSet>("exec up_list_voucherSetC @h99hid='" + CurrentInfo.HotelId + "'").ToList();
                                originVersions = s;
                            }
                        }
                        else if (originvoucherSets[0].Type == "消费类型")
                        {
                            List<VoucherSet> updatedvoucherSets = updatedVersions as List<VoucherSet>;
                            if (updatedvoucherSets != null && updatedvoucherSets[0].Type == "消费类型")
                            {
                                var s = db.Database.SqlQuery<VoucherSet>("select * from  VoucherSet where hid='" + CurrentInfo.HotelId + "'").ToList();
                                originVersions = s;
                            }
                        }
                    }
                }
                //修改
                int i = 0;
                return _Update(request, updatedVersions, originVersions, (list, u) =>
                {
                    List<VoucherSet> updatedvoucherSets = updatedVersions as List<VoucherSet>;
                    var s = db.Database.SqlQuery<VoucherSet>("select * from  VoucherSet where id='" + updatedvoucherSets[i].Id + "'").ToList();
                    i++;
                    if (s.Count == 0)
                        return u;
                    else
                        return s[0];
                }
                , OpLogType.凭证设置);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }            
        }

        #endregion
    }
}
