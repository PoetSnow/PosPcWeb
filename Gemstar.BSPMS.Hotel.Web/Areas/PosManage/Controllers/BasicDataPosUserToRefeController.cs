using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosUserToRefe;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.UserManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 操作员对应营业点
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035018")]
    public class BasicDataPosUserToRefeController : BaseEditInWindowController<PmsUser, IPmsUserService>
    {
        // GET: PosManage/BasicDataPosUserToRefe
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroup)  //仅当集团在集团里设置的时候才显示集团用户，否者查询单店用户
            {
                SetCommonQueryValues("up_pos_list_pmsGroupUser", "");
            }
            else
            {
                SetCommonQueryValues("up_pos_list_pmsUser", "");
            }
            return View();
        }

        #region 改
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(Guid.Parse(id), new UserToRefeViewModel());
        }
        /// <summary>
        /// 指定操作员对应营业点
        /// </summary>
        /// <param name="oldmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(UserToRefeViewModel oldmodel)
        {
            try
            {
                var modelService = GetService<IPmsUserService>();
                bool isexsit = modelService.IsExists(CurrentInfo.HotelId, oldmodel.Code, oldmodel.Name, oldmodel.Id);
                if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
                if (!string.IsNullOrEmpty(Request["RefeId"]))
                {
                    oldmodel.RefeId = Request["RefeId"].ToString();
                }
                if (!string.IsNullOrEmpty(Request["PosId"]))
                {
                    oldmodel.PosId = Request["PosId"].ToString();
                }
                return _Edit(oldmodel, new PmsUser(), OpLogType.操作员修改);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.ToString()));
            }
        }
        #endregion

        #region 数据

        #endregion
    }
} 