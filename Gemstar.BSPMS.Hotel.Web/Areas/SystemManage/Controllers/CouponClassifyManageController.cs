using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 优惠券设置
    /// </summary>
    [AuthPage("99025005")]
    [AuthPage(ProductType.Member, "m61050005")]
    public class CouponClassifyManageController : BaseEditIncellController<CodeList, ICodeListService>
    {
        private string typeCode = "28";
        private string typeCodeName = "优惠券类型";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_pmsCodeList", "@h99typeCode=" + typeCode, "gridCouponClassify");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> addVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            return _Add(request, addVersions, w =>
            {
                w.Hid = hid;
                w.TypeCode = typeCode;
                w.TypeName = typeCodeName;
                w.Id = hid + typeCode + w.Code;
                w.Status = EntityStatus.启用;
            }, OpLogType.优惠券添加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<CodeList> originVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            //没有下级时才能修改优惠券code
            var itemserv = GetService<ICouponService>();
            foreach (var origi in originVersions)
            {
                foreach (var upd in updatedVersions)
                {
                    if (origi.Id == upd.Id && origi.Code != upd.Code)
                    {
                        List<Coupon> lists = itemserv.GetCouponbyitemTypeid(hid, origi.Pk);
                        if (lists.Count > 0)
                        {
                            ModelState.AddModelError("Code", "有子项不可修改优惠券代码！");
 
                            return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }
            return _Update(request, updatedVersions, originVersions, (list, u) =>
            {

                var entity = list.SingleOrDefault(w => w.Id == u.Id);
                return entity;
            },
            OpLogType.优惠券修改
            , w => { w.Id = string.Format("{0}{1}{2}", w.Hid, w.TypeCode, w.Code); }
            );
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ICodeListService>(), OpLogType.优惠券删除);
        }
        #endregion
        #region 删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        { 
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId; 
            var itemserv = GetService<ICouponService>();
            var arr = id[0].Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                List<Coupon> list = itemserv.GetCouponbyitemTypeid(hid, int.Parse(arr[i]));
                if (list.Count > 0)
                { 
                    return Json(JsonResultData.Failure("有子项不可删除！")); 
                }
            }
            return Json(JsonResultData.Successed("可以删除！"));
        }
        #endregion
    }
}