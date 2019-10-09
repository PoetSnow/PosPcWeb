using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 系统参数维护
    /// </summary>
    [AuthPage("99050")]
    [AuthPage(ProductType.Member, "m99030")]
    [AuthPage(ProductType.Pos, "p99035011")]
    [BusinessType("系统参数维护")]
    public class PmsParaManageController : BaseEditIncellController<PmsPara, IPmsParaService>
    {
        // GET: SystemManage/PmsPara

        #region 查询
        [AuthButton(AuthFlag.Query)]
        // GET: SystemManage/ShiftManage
        public ActionResult Index(string name)
        {
            SetCommonQueryValues("up_list_PmsPara", "@t00参数名称=" + name);
            return View();
        }
        #endregion


        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PmsPara> updateClass, [Bind(Prefix = "originModels")]IEnumerable<PmsPara> originClass)
        {
           foreach (var update in updateClass)
            {
                var df = update.DefaultValue;
                if (update.Code == "isRoomCheck" && update.Value == "0")
                {
                    foreach (var origin in originClass)
                    {
                        if (origin.Code == "isRoomCheck" && origin.Value == "1")
                        {
                            var paraserv= GetService<IPmsParaService>();
                            if (paraserv.IsExistCleanRoom(update.Hid))
                            {
                                ModelState.AddModelError("Name", "房态存在清洁房不能修改为不启用清洁房。");
                                return Json(updateClass.ToDataSourceResult(request, ModelState));
                            }
                        }
                    }
                }
                double vtdb = 0;
                if (df != "" && df != null && update.Value != null)
                {
                    if (CheckDatetimeType(df) == "")
                    {
                        if (CheckDatetimeType(update.Value) != "")
                        {
                            ModelState.AddModelError("Name", "请填写正确的时间格式");
                            return Json(updateClass.ToDataSourceResult(request, ModelState));
                        }
                    }
                    else
                    {
                        if (double.TryParse(df, out vtdb))
                        {

                            if (!double.TryParse(update.Value, out vtdb))
                            {
                                ModelState.AddModelError("Name", "请填写正确的数字格式");
                                return Json(updateClass.ToDataSourceResult(request, ModelState));
                            }
                        }
                    }
                }
                else if (update.Value == null)
                {
                    update.Value = "";
                }
            }
            return _Update(request, updateClass, originClass, (list, u) => list.SingleOrDefault(w => w.Id == u.Id), OpLogType.系统参数修改);
        }

        /// <summary>
        /// 判断时间格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public string CheckDatetimeType(string time)
        {
            if (time.Trim().Length != 5) { return "时间格式不正确！"; }
            int k = 0;
            string bt = time.Substring(0, 2);
            string et = time.Substring(3, 2);
            if (!int.TryParse(bt, out k) || !int.TryParse(et, out k))
            { return "时间格式不正确！"; }

            int a = int.Parse(bt);
            string b = time.Substring(2, 1);
            int c = int.Parse(et);
            if (a > 23 || c > 59 || b != ":")
            {
                return "时间格式不正确！";
            }
            return "";
        }
        #endregion
    }
}