using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.PayManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Controllers
{
    /// <summary>
    /// 会员交易记录列表
    /// </summary>
    [AuthPage("30001")]
    public class MbrCardCaManageController : BaseEditInWindowController<MbrCard, IMbrCardService>
    {
        [AuthButton(AuthFlag.transactionRecord)]
        public ActionResult Index(string profileId,string profileType="01")
        {
            ViewBag.IsGroup = CurrentInfo.IsGroup;
            ViewBag.profileType = profileType;
            SetCommonQueryValues("up_list_profileCaList", "@h99profileid=" + profileId + "&@m14会员账户类型="+profileType);
            return View();
        }

        [AuthButton(AuthFlag.Query)]
        public JsonResult AccountTypeList()
        {
            return Json(GetService<ICodeListService>().GetAccountType().Select(c => new { Value = c.code,  Text = c.name }), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetProfileType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(JsonResultData.Failure("账务类型不正确"));
           var service=  GetService<ICodeListService>().GetAccountType();
           var data=  service.Where(w => w.name == name.Trim()).FirstOrDefault();
           if(data!=null)
               return Json(JsonResultData.Successed(data), JsonRequestBehavior.AllowGet);
           else
               return Json(JsonResultData.Failure("账务类型不正确"));
        }
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditRecharge(string itemid, string remark,string id)
        {
            var dataList = GetService<IItemService>().GetItems(CurrentInfo.HotelId, "C", true).Where(w => w.Action == "no"||w.Action== "credit");
            ViewBag.PayWapSelectList = dataList.Select(w => new SelectListItem { Value = w.Id, Text = w.Code + "-" + w.Name }).ToList();
            return PartialView("editRecharge", new MbrCardRechargeViewModel() {Id=string .IsNullOrWhiteSpace(id)?new Guid():Guid.Parse(id),PayWayId=itemid,Remark=remark });
        }
        [AuthButton(AuthFlag.Update)]
        public JsonResult EditRechargeSave(string itemid, string remark, string id)
        {
            if (string.IsNullOrWhiteSpace(itemid) && string.IsNullOrWhiteSpace(id))
                return Json( JsonResultData.Failure("付款方式不正确"),JsonRequestBehavior.DenyGet);
            var service= GetService<IMbrCardService>();
            var data = service.UpdateProfileCa(CurrentInfo.HotelId, Guid.Parse(id), itemid, remark);
            return Json(data,JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 给指定的充值记录进行退款
        /// 首先检测是否已经退款，已经退款的则直接给出提示
        /// 再检测支付方式，如果是微信支付和支付宝支付的，则发起相应的退款申请，退款申请失败则直接给出提示
        /// 如果退款成功或者不是微信支付和支付宝支付的，则直接更改充值记录的退款标志，同时插入一笔对应金额的负数来冲减余额
        /// </summary>
        /// <param name="id">要进行退款的充值记录id</param>
        /// <returns>退款结果</returns>
        [AuthButton(AuthFlag.Update)]
        [HttpPost]
        public JsonResult Refund(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请指定要退款的充值记录id"));
            }
            try
            {
                var commonDb = GetService<DbCommonContext>();
                var pmsParaService = GetService<IPmsParaService>();
                var chargeFreeService = GetService<IChargeFreeService>();
                var payLogService = GetService<IPayLogService>();

                var commonPayParas = commonDb.M_v_payParas.ToList();
                var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);
                var mbrCardService = GetService<IMbrCardService>();
                var refundPara = new RechargeRefundPara
                {
                    ChargeFreeService = chargeFreeService,
                    CommonPayParas = commonPayParas,
                    Hid = CurrentInfo.HotelId,
                    HotelName = CurrentInfo.HotelName,
                    HotelPayParas = hotelPayParas,
                    IsEnvTest = MvcApplication.IsTestEnv,
                    PayLogService = payLogService,
                    ProfileCaId = id,
                    UserName = CurrentInfo.UserName
                };
                var refundResult = mbrCardService.Refund(refundPara);
                return Json(refundResult);
            }
            catch(Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
    }
}