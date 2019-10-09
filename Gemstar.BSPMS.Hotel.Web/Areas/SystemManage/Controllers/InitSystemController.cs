using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.InitSystem;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 初始化系统
    /// </summary>
    [AuthPage("99096")]
    [AuthPage(ProductType.Member, "m99065")]
    [AuthPage(ProductType.Pos, "p99065")]
    public class InitSystemController : BaseController
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index()
        {
            var model = new InitSystemViewModel
            {
                DeleteBusinessData = false,
                DeleteMarketingPolicy = false,
                DeleteItemBaseData = false,
                DeleteMarketingBasicData = false
            };


            var userService = GetService<IPmsUserService>();
            var regMobile = userService.GetRegUserMobile(CurrentInfo.GroupHotelId);
            if (string.IsNullOrWhiteSpace(regMobile))
            {
                model.MixedMobile = "未正确设置手机";
            }else
            {
                model.MixedMobile = regMobile.Substring(0, 3) + "****" + regMobile.Substring(7);
            }
            return View(model);
        }

        /// <summary>
        /// 手机验证
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public JsonResult SendCheckCode()
        {
            try
            {
                var userService = GetService<IPmsUserService>();
                var regMobile = userService.GetRegUserMobile(CurrentInfo.GroupHotelId);
                if (string.IsNullOrWhiteSpace(regMobile))
                {
                    return Json(JsonResultData.Failure("注册用户没有设置正确的手机号"));
                }
                string username, password;
                var paraService = GetService<IPmsParaService>();
                paraService.IsSmsEnable(CurrentInfo.HotelId, out username, out password);
                var sendPara = new SMSSendParaCheckCode
                {
                    Mobile = regMobile,
                    Func = CheckFunc.InitSystem,
                    UserName = username,
                    Password = password
                };
                return Json(SMSSendHelper.SendCheckCode(sendPara), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index(InitSystemViewModel model)
        {
            try
            {
                //验证酒店编号及酒店名称
                if (model.HotelId != CurrentInfo.HotelId || model.HotelName != CurrentInfo.HotelName)
                {
                    return Json(JsonResultData.Failure("请输入正确的酒店编号及酒店名称"));
                }
                //验证手机验证码
                if (string.IsNullOrWhiteSpace(model.CheckCode))
                {
                    return Json(JsonResultData.Failure("请输入验证码"));
                }
                var userService = GetService<IPmsUserService>();
                var regMobile = userService.GetRegUserMobile(CurrentInfo.GroupHotelId);
                if (string.IsNullOrWhiteSpace(regMobile))
                {
                    return Json(JsonResultData.Failure("注册用户没有设置正确的手机号"));
                }
                var result = SMSSendHelper.MatchCheckCode(regMobile, model.CheckCode, CheckFunc.InitSystem);
                if (!result.Success)
                {
                    return Json(result);
                }

                //执行初始化存储过程
                var hotelDb = GetService<DbHotelPmsContext>();
                hotelDb.Database.ExecuteSqlCommand("exec up_pos_oriSysOfHotel @hid=@hid, @isBusinessData=@isBusinessData, @isMarketingPolicy=@isMarketingPolicy, @isItemBaseData=@isItemBaseData, @isMarketingBasicData=@isMarketingBasicData"
                    , new SqlParameter("@hid", CurrentInfo.HotelId)
                    , new SqlParameter("@isBusinessData", model.DeleteBusinessData ? 1 : 0)
                    , new SqlParameter("@isMarketingPolicy", model.DeleteMarketingPolicy ? 1 : 0)
                    , new SqlParameter("@isItemBaseData", model.DeleteItemBaseData ? 1 : 0)
                    , new SqlParameter("@isMarketingBasicData", model.DeleteMarketingBasicData ? 1 : 0)
                );
                //日志
                System.Collections.Generic.List<string> logs = new System.Collections.Generic.List<string>();
                logs.Add(string.Format("酒店：{0} {1}", CurrentInfo.HotelId, CurrentInfo.HotelName));
                logs.Add(string.Format("营业日：{0}", GetService<Gemstar.BSPMS.Hotel.Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd")));
                logs.Add(string.Format("班次：{0}", CurrentInfo.ShiftName));

                if (model.DeleteBusinessData)
                {
                    logs.Add("删除营业数据");
                }
                if (model.DeleteMarketingPolicy)
                {
                    logs.Add("删除营销政策");
                }
                if (model.DeleteItemBaseData)
                {
                    logs.Add("删除消费项目基础数据");
                }
                if (model.DeleteMarketingBasicData)
                {
                    logs.Add("删除营销基础数据");
                }
                
                AddOperationLog(Common.Services.Enums.OpLogType.系统初始化, string.Join("，", logs));

                return Json(JsonResultData.Successed(""));
            }
            catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
    }
}