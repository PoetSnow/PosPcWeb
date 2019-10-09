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
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Common.Services.EF;
using System.Transactions;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.PayManage;
using System.Web.Script.Serialization;
using System.Text;
using Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Controllers
{
    /// <summary>
    /// 会员列表
    /// </summary>
    [AuthPage("30001")]
    [AuthPage(ProductType.Member, "m30001")]
    public class MbrCardManageController : BaseEditInWindowController<MbrCard, IMbrCardService>
    {
        #region 查询
        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            //检查权限
            var select = IsHasAuth("30001", 1);//综合查询-此权限包含精确查询的
            var fastSelect = IsHasAuth("30001", 4503599627370496);//精确查询
            var IsFast = select ? 1 : 0;
            ViewBag.IsFast = IsFast;
            SetCommonQueryValues("up_list_mbrCardList", "@h99IsFast=" + IsFast);
            bool isAllowOwner=GetService< IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            var _sysParaService = GetService<ISysParaService>();
            var para = _sysParaService.GetSMSSendPara();
            var sendService = GetService<ISMSSendService>();
            var balance = sendService.QueryBalance(CurrentInfo.HotelId, para);
            if (balance >= 0)
            {
                ViewBag.balance = string.Format("短信剩余{0}条", balance);
            }
            else
            {
                ViewBag.balance = "";
            }
            GetLockInfo();
            ViewBag.isallowOwner = isAllowOwner;
            ViewBag.isGroup = CurrentInfo.IsGroup;
            var pmsHotelService = GetService<IPmsHotelService>();
            if (CurrentInfo.IsGroup)
            {
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);//当前集团的酒店列表
            }
            ViewBag.hotelid = CurrentInfo.HotelId; 
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            GetGenderSelectList();
            GetCerTypeSelectList();
            GetMbrCardTypeSelectList();
            GetLockInfo();
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;
            ViewBag.Username = CurrentInfo.UserName;
            return _Add(new MbrCardAddViewModel() { IsAdvertisementMsg = true, IsTransactionMsg = true, IsNeedPhone = 0 });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(MbrCardAddViewModel model)
        {
            var service = GetService<IMbrCardService>();
            if (string.IsNullOrWhiteSpace(model.Sales))
            {
                return Json(JsonResultData.Failure("请选择业务员"), JsonRequestBehavior.DenyGet);
            }
            if (CheckCer(model.CerType, model.CerId))
            {
                return Json(JsonResultData.Failure("请输入有效的证件号"), JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrWhiteSpace(model.MbrCardNo))
            {
                model.MbrCardNo = model.Mobile;
            }
            else if (IsExistsMbrCardNo(model.MbrCardNo))
            {
                return Json(JsonResultData.Failure("会员卡号已存在，请重新输入"), JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrWhiteSpace(model.MbrCardNo))
            {
                return Json(JsonResultData.Failure("会员卡号和手机号至少填写一个"), JsonRequestBehavior.DenyGet);
            }
            if (!string.IsNullOrWhiteSpace(model.Mobile))
            {
                if (service.CheckMobile(model.Mobile, null))
                    return Json(JsonResultData.Failure("手机号已存在，请重新输入"), JsonRequestBehavior.DenyGet);
            }
            if (service.CheckCer(model.CerType, model.CerId))
                return Json(JsonResultData.Failure("证件号已存在，请重新输入"), JsonRequestBehavior.DenyGet);
            if (string.IsNullOrWhiteSpace(model.InductionCar))
                model.InductionCar = model.MbrCardNo;
            if (service.RepeatCheckCar(model.InductionCar))
                return Json(JsonResultData.Failure(string.Format("感应卡号({0})已存在，请重新输入", model.InductionCar)), JsonRequestBehavior.DenyGet);
            if (string.IsNullOrWhiteSpace(model.Mobile) && model.IsNeedPhone == 0)
            {
                model.IsNeedPhone = 1;
                return Json(JsonResultData.Successed(model.IsNeedPhone));
            }
            else
            {
                var hotelService = GetService<IHotelStatusService>();
                var businessDate = hotelService.GetBusinessDate(CurrentInfo.HotelId);
                if(businessDate == null)
                {
                    businessDate = DateTime.Now;
                }
                var mbr = new MbrCard
                {
                    Grpid = CurrentInfo.GroupId,
                    Hid = CurrentInfo.HotelId,
                    Id = Guid.NewGuid(),
                    IsAudit = true,
                    JoinDate = businessDate,//入会日期由当前时间改成营业日时间
                    Pwd = PasswordHelper.GetEncryptedPassword(model.Mobile, PasswordHelper.GetDefaultPasswordFromMobile(model.Mobile)),
                    Status = (byte)MbrCardStatus.Nomal,
                    ValidDate = DateTime.Now,
                    CreateUser = CurrentInfo.UserName,
                    Source = "酒店",
                };
                //取出会员卡类型中的默认有效时长和卡费
                var cardTypeService = GetService<IMbrCardTypeService>();
                var cardType = cardTypeService.Get(model.MbrCardTypeId);
                if (cardType.Validdate >= 0)
                {
                    if(cardType.Validdate > 1200)
                    {
                        cardType.Validdate = 1200;
                    }
                    mbr.ValidDate = DateTime.Now.AddMonths(cardType.Validdate);
                }
                //如果需要收取卡费，则自动将会员状态设置为禁用，等收取卡费成功后再更改为正常
                if(cardType.CardFee > 0)
                {
                    mbr.Status = (byte)MbrCardStatus.Disabled;
                }
                var addJsonResult =  _Add(model,mbr , OpLogType.会员增加) as JsonResult;
                var addResultData = addJsonResult.Data as JsonResultData;
                if (addResultData.Success)
                {
                    var addData = new MbrCardAddResultModel
                    {
                        ProfileId = mbr.Id.ToString(),
                        CardFeeAmount = cardType.CardFee
                    };
                    addResultData = JsonResultData.Successed(addData);
                }
                return Json(addResultData);
            }

        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            var entity = GetService<IMbrCardService>().GetMbrCard(id).FirstOrDefault();
            if (entity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            }
            GetGenderSelectList();
            GetCerTypeSelectList();
            GetMbrCardTypeSelectList();
            GetLockInfo();
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;
            return _Edit(id, new MbrCardEditViewModel() { IsNeedPhone = 0 });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(MbrCardEditViewModel model)
        {
            var service = GetService<IMbrCardService>();
            var entity = service.GetMbrCard(model.Id).FirstOrDefault();
            if (entity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            }
            if (CheckCer(model.CerType, model.CerId))
            {
                return Json(JsonResultData.Failure("请输入有效的证件号"), JsonRequestBehavior.DenyGet);
            }
            if (!string.IsNullOrWhiteSpace(model.Mobile))
            {
                if (service.CheckMobile(model.Mobile, model.Id.ToString()))
                    return Json(JsonResultData.Failure("手机号已存在，请重新输入"), JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrWhiteSpace(model.Mobile) && model.IsNeedPhone == 0)
            {
                model.IsNeedPhone = 1;
                return Json(JsonResultData.Successed(model.IsNeedPhone));
            }
            else
            {
                return _Edit(model, entity, OpLogType.会员修改);
            }
        }
        #endregion

        #region 详情
        [AuthButton(AuthFlag.Details)]
        public ActionResult Detail(Guid id, bool viewType = true)
        {
            IMbrCardService iMbrCardService = GetService<IMbrCardService>();
            //MbrCard entity = iMbrCardService.Get(id);
            MbrCard entity = iMbrCardService.GetMbrCard(id).FirstOrDefault();
            if (entity == null)
            {
                entity = null;
                ViewBag.Message = "错误信息！";
            }
            else
            {
                GetStatusSelectList();
                GetGenderSelectList();
                GetMbrCardTypeSelectList();
                var balance = iMbrCardService.GetCardBalance(id);
                if (balance == null)
                    balance = new MbrCardBalance();
                var data = iMbrCardService.GetProfileTransRes(id);
                var amount = data.Sum(s => s.Amount);
                var time = data.FirstOrDefault();
                balance.Amounts = amount;
                balance.Nigths = data.Count();
                balance.LastIn = time == null ? DateTime.MinValue : time.TransDate;
                ViewBag.CardBalance = balance;
            }
            ViewBag.viewType = viewType;
            bool isAllowOwner = GetService<IPmsParaService>().isAllowOwner(CurrentInfo.HotelId);
            ViewBag.isallowOwner = isAllowOwner;
            if (viewType)
            {
                return View(entity);
            }
            else
            {
                return PartialView(entity);
            }
        }
        #endregion

        #region 下拉绑定
        /// <summary>
        /// 会员卡类型
        /// </summary>
        private void GetMbrCardTypeSelectList()
        {
            var dataList = GetService<IMbrCardTypeService>().List(CurrentInfo.GroupHotelId);
            ViewBag.MbrCardTypeSelectList = new SelectList(dataList, "Key", "Value");
        }
        /// <summary>
        /// 获取优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listForCoupon()
        {
            var hotelserver = GetService<ICouponService>();
            var list = hotelserver.List(CurrentInfo.GroupHotelId);//优惠券类型
            List<SelectListItem> listItems = list.Select(w => new SelectListItem { Value = w.Key, Text = w.Value }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 积分兑换项目
        /// </summary>
        private void GetItemScoreSelectList(string cardTypeid)
        {
            var data = GetService<IScoreUseRuleService>().GetListEntity(CurrentInfo.GroupHotelId, CurrentInfo.HotelId, cardTypeid); ;
            ViewBag.ItemScoreSelectList = new SelectList(data, "Key", "Value");
        }
        /// <summary>
        /// 性别
        /// </summary>
        private void GetGenderSelectList()
        {
            ViewBag.GenderSelectList = EnumExtension.ToSelectList(typeof(Gender), EnumValueType.Text, EnumValueType.Description);
        }
        /// <summary>
        /// 状态
        /// </summary>
        private void GetStatusSelectList()
        {
            ViewBag.StatusSelectList = EnumExtension.ToSelectList(typeof(MbrCardStatus), EnumValueType.Value, EnumValueType.Description);
        }
        /// <summary>
        /// 证件类型
        /// </summary>
        private void GetCerTypeSelectList()
        {
            var dataList = GetService<ICodeListService>().GetCerType().Select(c => c.name).ToList();
            ViewBag.CerTypeSelectList = new SelectList(dataList);
        }
        /// <summary>
        /// 默认有效期至
        /// </summary>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetValidDate(string id)
        {
            var data = GetService<IMbrCardTypeService>().Get(id);
            if (data != null)
                return Json(JsonResultData.Successed(data.Validdate), JsonRequestBehavior.DenyGet);
            return Json(JsonResultData.Failure("不存在此会员卡类型"), JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 付款方式
        /// </summary>
        private void GetPayWapSelectList(string type)
        {
            var data = GetService<IItemService>().GetItems(CurrentInfo.HotelId, "C", true);
            if(type != "CardFee")
            {
                data = data.Where(w => w.Action != "roomFolio");
            }
            if (type == "ScoreUse")//可充值，排除挂账
                data = data.Where(w => w.Action != "corp");
            ViewBag.PayWapSelectList = data.Select(w => new SelectListItem { Value = w.Id + "&|&" + w.Rate + "&|&" + w.Action, Text = w.Code + "-" + w.Name }).ToList();
        }
        /// <summary>
        /// 业务员，自动填充
        /// </summary>
        /// <param name="nameOrMobile">名称或手机号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetSalesSelectList(string nameOrMobile, string notName)
        {
            if (string.IsNullOrWhiteSpace(nameOrMobile))
            {
                return Json(new List<Sales>(), JsonRequestBehavior.AllowGet);
            }
            var _salesService = GetService<ISalesService>();

            return Json(_salesService.List((CurrentInfo.GroupHotelId), nameOrMobile, notName).Select(c => c.Name), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 业务员,下拉框
        /// </summary>
        /// <param name="nameOrMobile">名称或手机号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetSalesSelectLists()
        {
            var _salesService = GetService<ISalesService>();
            List<Sales> alist = _salesService.List(CurrentInfo.GroupHotelId);
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取升级卡所需积分
        /// </summary>
        /// <param name="mbrCardTypeid">卡类型ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetChangeLevelScore(string mbrCardTypeid)
        {
            if (string.IsNullOrWhiteSpace(mbrCardTypeid))
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            var entity = GetService<IMbrCardTypeService>().Get(mbrCardTypeid);
            if (entity == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(entity.Score, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据充值赠送规则 计算赠送金额
        /// </summary>
        /// <param name="money">充值金额</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetSendMoney(Guid id, decimal money)
        {
            //var entity = GetService<IMbrCardService>().Get(id);
            var entity = GetService<IMbrCardService>().GetMbrCard(id).FirstOrDefault();
            if (entity == null || string.IsNullOrWhiteSpace(entity.MbrCardTypeid) || money <= 0)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(GetService<IChargeFreeService>().GetSendMoney(CurrentInfo.GroupHotelId, entity.MbrCardTypeid, money), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取兑换项目所需积分、金额
        /// </summary>
        /// <param name="money">充值金额</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetScoreUseRule(Guid id, string itemId)
        {
            //var entity = GetService<IMbrCardService>().Get(id);
            var entity = GetService<IMbrCardService>().GetMbrCard(id).FirstOrDefault();
            if (entity == null || string.IsNullOrWhiteSpace(entity.MbrCardTypeid))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var ItemEntity = GetService<IItemScoreService>().Get(itemId);
            if (ItemEntity == null && ItemEntity.Status == EntityStatus.启用)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var result = GetService<IScoreUseRuleService>().GetEntity(CurrentInfo.GroupHotelId, entity.MbrCardTypeid, itemId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 根据证件类型验证证件号是否正确
        /// </summary>
        /// <param name="cerType">证件类型</param>
        /// <param name="cerId">证件号</param>
        /// <returns></returns>
        private bool CheckCer(string cerType, string cerId)
        {
            bool isTrue = false;
            if (!string.IsNullOrWhiteSpace(cerId))
            {
                switch (cerType)
                {
                    case "居民身份证":
                        isTrue = !RegexHelper.IsRightIdentityCard(cerId);
                        break;
                    case "护照":
                        isTrue = !RegexHelper.IsRightPassport(cerId);
                        break;
                    case "港澳台居民居民身份证":
                        isTrue = !RegexHelper.IsRightIdentityCardOthers(cerId);
                        break;
                }
            }
            return isTrue;
        }
        /// <summary>
        /// 检查会员卡号是否已存在
        /// </summary>
        /// <param name="mbrCardNo"></param>
        /// <returns></returns>
        private bool IsExistsMbrCardNo(string mbrCardNo, string id = null)
        {
            return GetService<IMbrCardService>().RepeatCheck(mbrCardNo, id);
        }
        /// <summary>
        /// 检查会员卡号和感应卡号是否同时存在
        /// </summary>
        /// <param name="mbrCardNo"></param>
        /// <param name="InductionCar"></param>
        /// <returns></returns>
        private bool IsExistsMbrCardNos(string mbrCardNo, string InductionCar)
        {
            return GetService<IMbrCardService>().RepeatChecks(mbrCardNo, InductionCar);
        }
        /// <summary>
        /// 检查卡号格式
        /// </summary>
        /// <param name="cardNo">会员卡号</param>
        /// <returns></returns>
        private bool CheckCardNo(string cardNo)
        {
            if (string.IsNullOrWhiteSpace(cardNo)) { return false; }
            //if (cardNo.Length != 12) { return false; }
            return new Regex("^[A-Za-z0-9]+$", RegexOptions.Compiled).IsMatch(cardNo);
        }
        [HttpPost]
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetMbrCardId(string mbrNo)
        {
            if (string.IsNullOrWhiteSpace(mbrNo))
                return Json(JsonResultData.Failure("会员卡号和手机号至少填写一个"), JsonRequestBehavior.DenyGet);
            var data = GetService<IMbrCardService>().GetMbrCar(mbrNo, CurrentInfo.HotelId);
            if (data == null)
                return Json(JsonResultData.Failure("没有该会员信息"), JsonRequestBehavior.DenyGet);
            return Json(JsonResultData.Successed(data.Id), JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取门锁信息
        /// </summary>
        private void GetLockInfo()
        {
            var hotelInterfaces = GetService<IHotelInfoService>().GetHotelInterface(CurrentInfo.HotelId);
            var idInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "02");//只取出设置的身份证读卡器接口信息
            ViewBag.idType = idInterface == null ? "" : idInterface.TypeCode;
            ViewBag.idCode = idInterface == null ? "" : idInterface.Code;
            ViewBag.idEditionName = idInterface == null ? "" : idInterface.EditionName;
            var mbrInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "07");//只取出设置的会员卡接口信息
            ViewBag.mbrType = mbrInterface == null ? "" : mbrInterface.TypeCode;
            ViewBag.mbrCode = mbrInterface == null ? "" : mbrInterface.Code;
            ViewBag.mbrEditionName = mbrInterface == null ? "" : mbrInterface.EditionName;
        }
        #endregion

        //操作

        #region 启用禁用
        [HttpPost]
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            string[] idarr = id.Split(',');
            for (int i = 0; i < idarr.Length; i++)
            {
                MbrCard entity = new MbrCard() { Id = Guid.Parse(idarr[i]), Status = (byte)EntityStatus.启用 };
                GetService<IMbrCardService>().UpdateSingle("Status", entity);
            }
            return Json(JsonResultData.Successed("启用成功"), JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            string[] idarr = id.Split(',');
            for (int i = 0; i < idarr.Length; i++)
            {
                MbrCard entity = new MbrCard() { Id = Guid.Parse(idarr[i]), Status = (byte)EntityStatus.禁用 };
                GetService<IMbrCardService>().UpdateSingle("Status", entity);
            }
            return Json(JsonResultData.Successed("禁用成功"), JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IMbrCardService>(), OpLogType.会员删除);
        }
        #endregion
        #region (换卡号 升级卡类型 变更卡状态 审核 更换业务员)公共方法
        [AuthButton(AuthFlag.Query)]
        public MbrCardEditSingleViewModel EditSingle(Guid id, string type)
        {
            //MbrCard entity = GetService<IMbrCardService>().Get(id);
            var entity = GetService<IMbrCardService>().GetMbrCard(id).FirstOrDefault();
            GetLockInfo();
            if (entity == null)
            {
                // return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
                return null;
            }
            MbrCardEditSingleViewModel model = new MbrCardEditSingleViewModel() { Id = entity.Id, Type = type, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName };
            switch (type)
            {
                case "MbrCardNo":
                    break;
                case "Status":
                    GetStatusSelectList();
                    model.Status = entity.Status;
                    break;
                case "IsAudit":
                    model.IsAudit = entity.IsAudit;
                    break;
                case "Sales":
                    model.Sales = entity.Sales;
                    break;
                case "ValidDate":
                    model.ValidDate = entity.ValidDate;
                    break;
            }
            return model;

            // PartialView("_EditSingle", model);
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult EditSingle(MbrCardEditSingleViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Type))
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            }
            MbrCard entity = new MbrCard();
            entity.Id = model.Id;
            entity.Remark = model.Remark;
            switch (model.Type)
            {
                case "MbrCardNo":
                    entity.MbrCardNo = model.MbrCardNo;
                    entity.InductionCar = string.IsNullOrWhiteSpace(model.InductionCar) ? model.MbrCardNo : model.InductionCar;
                    if (string.IsNullOrWhiteSpace(model.MbrCardNo)) { return Json(JsonResultData.Failure("请输入会员卡号"), JsonRequestBehavior.DenyGet); }
                    if (!CheckCardNo(model.MbrCardNo)) { return Json(JsonResultData.Failure("卡号只能为[字母、数字]组成"), JsonRequestBehavior.DenyGet); }
                    if (IsExistsMbrCardNos(model.MbrCardNo, entity.InductionCar))
                    {
                        return Json(JsonResultData.Failure("请输入新会员卡号或新感应卡号"), JsonRequestBehavior.DenyGet);
                    }
                    break;
                case "Status":
                    entity.Status = model.Status;
                    if (model.Status == null) { return Json(JsonResultData.Failure("请选择会员卡状态"), JsonRequestBehavior.DenyGet); }
                    break;
                case "IsAudit":
                    entity.IsAudit = model.IsAudit;
                    if (model.IsAudit == null) { return Json(JsonResultData.Failure("请选择审核状态"), JsonRequestBehavior.DenyGet); }
                    break;
                case "Sales":
                    entity.Sales = model.Sales;
                    if (string.IsNullOrWhiteSpace(model.Sales)) { return Json(JsonResultData.Failure("请选择业务员"), JsonRequestBehavior.DenyGet); }
                    break;
                case "ValidDate":
                    entity.ValidDate = model.ValidDate;
                    if (model.ValidDate == null) { return Json(JsonResultData.Failure("请选择有效期"), JsonRequestBehavior.DenyGet); }
                    break;
                default:
                    return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            }
            return Json(GetService<IMbrCardService>().UpdateSingle(model.Type, entity), JsonRequestBehavior.DenyGet);
        }
        #endregion
        #region 换卡号
        [AuthButton(AuthFlag.ChangeCardNum)]
        public ActionResult ChangeCardNum(Guid id, string type)
        {
            var model = EditSingle(id, type);
            if (model == null)
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            return PartialView("_EditSingle", model);
        }
        #endregion
        #region 变更卡状态
        [AuthButton(AuthFlag.UpdateCardStatus)]
        public ActionResult UpdateCardStatus(Guid id, string type)
        {
            var model = EditSingle(id, type);
            if (model == null)
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            return PartialView("_EditSingle", model);
        }
        #endregion
        #region 审核
        [AuthButton(AuthFlag.Inspect)]
        public ActionResult Inspect(Guid id, string type)
        {
            var model = EditSingle(id, type);
            if (model == null)
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            return PartialView("_EditSingle", model);
        }
        #endregion
        #region 延期
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult BatchDelay(Guid id, string type)
        {
            var model = EditSingle(id, type);
            if (model == null)
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            return PartialView("_EditSingle", model);
        }
        #endregion
        #region 变更业务员
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult ReplaceSalesman(Guid id, string type)
        {
            var model = EditSingle(id, type);
            if (model == null)
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            return PartialView("_EditSingle", model);
        }
        #endregion

        #region 升级卡类型
        [AuthButton(AuthFlag.UpgradeCard)]
        public ActionResult ChangeLevel(Guid id)
        {
            GetMbrCardTypeSelectList();
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            var balanceEntity = _mbrCardService.GetCardBalance(id);
            if (balanceEntity == null) { balanceEntity = new MbrCardBalance { Score = 0 }; }
            //检查是否有修改积分的权限
            var IsAuth = IsHasAuth("30001", 562949953421312);
            ViewBag.IsAuth = IsAuth;
            return PartialView("_ChangeLevel", new MbrCardChangeLevelViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName, MbrCardTypeid = entity.MbrCardTypeid, CurrentScore = balanceEntity.Score });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.UpgradeCard)]
        public ActionResult ChangeLevel(MbrCardChangeLevelViewModel model)
        {
            if (model == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后再重试"), JsonRequestBehavior.DenyGet);
            }
            if (string.IsNullOrWhiteSpace(model.MbrCardTypeid)) { return Json(JsonResultData.Failure("请选择会员卡类型"), JsonRequestBehavior.DenyGet); }
            return Json(GetService<IMbrCardService>().CardUpgrade(model.Id, model.MbrCardTypeid, model.Score, model.Remark), JsonRequestBehavior.DenyGet);
        }
        #endregion


        #region 会员卡费
        [AuthButton(AuthFlag.Accounting)]
        public ActionResult CardFee(Guid id,decimal? amount,int? isRecharge)
        {
            GetPayWapSelectList("CardFee");
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            if (!amount.HasValue)
            {
                //如果没有指定要收取的卡费，则根据会员的会员卡类型取会员卡上设置的卡费金额
                var cardTypeService = GetService<IMbrCardTypeService>();
                var cardType = cardTypeService.Get(entity.MbrCardTypeid);
                if(cardType != null)
                {
                    amount = cardType.CardFee;
                }
            }
            return PartialView("_CardFee", new MbrCardCardFeeViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName,OriginPayMoney = amount.Value,PayMoney = amount.Value,IsRecharge = isRecharge??0 });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Accounting)]
        public ActionResult CardFee(MbrCardCardFeeViewModel model)
        {
            try
            {
                var _mbrCardService = GetService<IMbrCardService>();

                var payServiceBuilder = GetService<IPayServiceBuilder>();
                var commonDb = GetService<DbCommonContext>();
                var pmsParaService = GetService<IPmsParaService>();

                var commonPayParas = commonDb.M_v_payParas.ToList();
                var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                JsonResultData addResult;
                OpLogType logType = OpLogType.会员卡费收取;
                var payService = payServiceBuilder.GetPayService(model.FolioItemAction, commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                using (var tc = new TransactionScope())
                {
                    //如果是付款，则获取支付服务实例，进行支付，并且将支付成功后返回的交易号保存到refno中
                    var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                    if (payService != null)
                    {
                        if (string.IsNullOrWhiteSpace(model.FolioItemActionJsonPara))
                        {
                            return Json(JsonResultData.Failure("参数不能为空"));
                        }
                        payResult = payService.DoPayBeforeSaveFolio(model.FolioItemActionJsonPara);
                    }
                    if (payResult.IsWaitPay)
                    {
                        //如果是待支付的，则不调用充值的存储过程，而是增加一待支付的记录，等到支付成功回调处理中再调用存储过程来真实增加余额
                        var waitPay = new WaitPayList
                        {
                            WaitPayId = Guid.NewGuid(),
                            CreateDate = DateTime.Now,
                            ProductType = PayProductType.MbrCardFee.ToString(),
                            Status = 0
                        };
                        var businessPara = new
                        {
                            profileId = model.Id,
                            itemid = model.PayWayId,
                            floatAmount = model.PayMoney,
                            invno = model.InvNo,
                            remark = model.Remark,
                            originFloatAmount = model.OriginPayMoney,
                            inputUser = CurrentInfo.UserName,
                            shiftId = CurrentInfo.ShiftId
                        };
                        var serializer = new JavaScriptSerializer();
                        waitPay.BusinessPara = serializer.Serialize(businessPara);
                        var waitPayService = GetService<IWaitPayListService>();
                        waitPayService.Add(waitPay);
                        waitPayService.Commit();
                        logType = OpLogType.会员卡费收取待支付;
                        addResult = JsonResultData.Successed(waitPay.WaitPayId);
                    }
                    else
                    {
                        addResult = _mbrCardService.CardFee(model.Id, model.PayWayId, model.PayMoney, model.InvNo, model.Remark, model.OriginPayMoney, payResult.RefNo);
                    }
                    tc.Complete();
                }
                //记录日志
                var items = GetService<IItemService>().Query(CurrentInfo.HotelId, "C", null);
                var item = items.Where(s => s.Id == model.PayWayId).FirstOrDefault();
                var logStr = new StringBuilder();
                logStr.Append("缴纳卡费")
                    .Append(" 金额:").Append(model.PayMoney)
                    .Append(",付款方式:").Append(item == null ? "" : item.Name)
                    .Append(",班次:").Append(CurrentInfo.ShiftName);
                if (!string.IsNullOrWhiteSpace(model.InvNo))
                {
                    logStr.Append(",单号:").Append(model.InvNo);
                }
                if (!string.IsNullOrWhiteSpace(model.Remark))
                {
                    logStr.Append(",备注:").Append(model.Remark);
                }
                if (addResult.Success)
                {
                    AddOperationLog(logType, logStr.ToString(), addResult.Data.ToString());
                    var returnResult = new ResFolioAddResult
                    {
                        FolioTransId = addResult.Data.ToString(),
                        Statu = PayStatu.Successed.ToString(),
                        Callback = "",
                        QrCodeUrl = "",
                        QueryTransId = "",
                        DCFlag = "c"
                    };
                    if (payService != null)
                    {
                        //转换一下folio的transid格式，以保证长度为32位
                        var transId = Guid.Parse(addResult.Data.ToString()).ToString("N");
                        var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.MbrCardFee, transId, model.FolioItemActionJsonPara);
                        returnResult.Statu = afterPayResult.Statu.ToString();
                        returnResult.Callback = afterPayResult.Callback;
                        returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                        returnResult.QueryTransId = afterPayResult.QueryTransId;
                    }

                    return Json(JsonResultData.Successed(returnResult));
                }
                else
                {
                    return Json(addResult);
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult CheckRoomNo(string roomNo)
        {
            var _roomService = GetService<IRoomService>();
            if (!_roomService.IsExistsRoomNo(CurrentInfo.HotelId, roomNo, null))
            {
                return Json(JsonResultData.Failure("酒店不存在此房间号"));
            }
            var _resService = GetService<IResService>();
            var resdetial = _resService.GetResDetailByRoomNo(CurrentInfo.HotelId, roomNo);
            if(resdetial == null)
            {
                return Json(JsonResultData.Failure("此房间号没有客人在住"));
            }
            return Json(JsonResultData.Successed(resdetial));
        }
        #endregion

        #region 会员充值
        [AuthButton(AuthFlag.MemberRecharge)]
        public ActionResult Recharge(Guid id)
        {
            GetPayWapSelectList("Recharge");
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            var balanceEntity = _mbrCardService.GetCardBalance(id);
            if (balanceEntity == null) { balanceEntity = new MbrCardBalance { Balance = 0, Free = 0 }; }
            var IsAuth = IsHasAuth("30001", 35184372088832);
            ViewBag.IsAuth = IsAuth;
            return PartialView("_Recharge", new MbrCardRechargeViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName, Balance = balanceEntity.Balance, SendBalance = balanceEntity.Free });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.MemberRecharge)]
        public ActionResult Recharge(MbrCardRechargeViewModel model)
        {
            try
            {
                var _mbrCardService = GetService<IMbrCardService>();

                var payServiceBuilder = GetService<IPayServiceBuilder>();
                var commonDb = GetService<DbCommonContext>();
                var pmsParaService = GetService<IPmsParaService>();

                var commonPayParas = commonDb.M_v_payParas.ToList();
                var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                JsonResultData addResult;
                var needSendSms = false;
                OpLogType logType = OpLogType.会员充值;
                var payService = payServiceBuilder.GetPayService(model.FolioItemAction, commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                using (var tc = new TransactionScope())
                {
                    //如果是付款，则获取支付服务实例，进行支付，并且将支付成功后返回的交易号保存到refno中
                    var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                    if (payService != null)
                    {
                        if (string.IsNullOrWhiteSpace(model.FolioItemActionJsonPara))
                        {
                            return Json(JsonResultData.Failure("参数不能为空"));
                        }
                        payResult = payService.DoPayBeforeSaveFolio(model.FolioItemActionJsonPara);
                    }
                    if (payResult.IsWaitPay)
                    {
                        //如果是待支付的，则不调用充值的存储过程，而是增加一待支付的记录，等到支付成功回调处理中再调用存储过程来真实增加余额
                        var waitPay = new WaitPayList
                        {
                            WaitPayId = Guid.NewGuid(),
                            CreateDate = DateTime.Now,
                            ProductType = PayProductType.MbrRecharge.ToString(),
                            Status = 0
                        };
                        var businessPara = new
                        {
                            profileId = model.Id,
                            itemid = model.PayWayId,
                            floatAmount = model.PayMoney,
                            freeAmount = model.SendMoney,
                            invno = model.InvNo,
                            remark = model.Remark,
                            originFloatAmount = model.OriginPayMoney,
                            inputUser = CurrentInfo.UserName,
                            shiftId = CurrentInfo.ShiftId
                        };
                        var serializer = new JavaScriptSerializer();
                        waitPay.BusinessPara = serializer.Serialize(businessPara);
                        var waitPayService = GetService<IWaitPayListService>();
                        waitPayService.Add(waitPay);
                        waitPayService.Commit();
                        logType = OpLogType.会员充值待支付;
                        addResult = JsonResultData.Successed(waitPay.WaitPayId);
                    }
                    else
                    {
                        addResult = _mbrCardService.Recharge(model.Id, model.PayWayId, model.PayMoney, model.SendMoney, model.InvNo, model.Remark, model.OriginPayMoney, payResult.RefNo);
                        //不是待支付的，表示已经充值成功，设置需要发送短信标记位
                        if (addResult.Success)
                        {
                            needSendSms = true;
                        }
                    }
                    tc.Complete();
                }
                //发送短信
                if (needSendSms)
                {
                    var sendService = GetService<ISMSSendService>();
                    var _sysParaService = GetService<ISysParaService>();
                    var para = _sysParaService.GetSMSSendPara();
                    var smsLogService = GetService<ISmsLogService>();
                    sendService.SendMbrRecharge(CurrentInfo.HotelId, model.Id, addResult.Data.ToString(), para, smsLogService);
                }
                //记录日志
                var items = GetService<IItemService>().Query(CurrentInfo.HotelId, "C", null);
                var item = items.Where(s => s.Id == model.PayWayId).FirstOrDefault();
                var logStr = new StringBuilder();
                logStr.Append("充值")
                    .Append(" 金额:").Append(model.PayMoney)
                    .Append(",增值金额:").Append(model.SendMoney)
                    .Append(",付款方式:").Append(item == null ? "" : item.Name)
                    .Append(",班次:").Append(CurrentInfo.ShiftName);
                if (!string.IsNullOrWhiteSpace(model.InvNo))
                {
                    logStr.Append(",单号:").Append(model.InvNo);
                }
                if (!string.IsNullOrWhiteSpace(model.Remark))
                {
                    logStr.Append(",备注:").Append(model.Remark);
                }
                if (addResult.Success)
                {
                    AddOperationLog(logType, logStr.ToString(), addResult.Data.ToString());
                    var returnResult = new ResFolioAddResult
                    {
                        FolioTransId = addResult.Data.ToString(),
                        Statu = PayStatu.Successed.ToString(),
                        Callback = "",
                        QrCodeUrl = "",
                        QueryTransId = "",
                        DCFlag = "c"
                    };
                    if (payService != null)
                    {
                        //转换一下folio的transid格式，以保证长度为32位
                        var transId = Guid.Parse(addResult.Data.ToString()).ToString("N");
                        var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.MbrRecharge, transId, model.FolioItemActionJsonPara);
                        returnResult.Statu = afterPayResult.Statu.ToString();
                        returnResult.Callback = afterPayResult.Callback;
                        returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                        returnResult.QueryTransId = afterPayResult.QueryTransId;
                    }

                    return Json(JsonResultData.Successed(returnResult));
                } else
                {
                    return Json(addResult);
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        #endregion

        #region 会员扣款
        [AuthButton(AuthFlag.MemberDebit)]
        public ActionResult SubtractMoney(Guid id)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            var balanceEntity = _mbrCardService.GetCardBalance(id);
            if (balanceEntity == null) { balanceEntity = new MbrCardBalance { Balance = 0, Free = 0 }; }
            return PartialView("_SubtractMoney", new MbrCardSubtractMoneyViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName, Balance = balanceEntity.Balance, SendBalance = balanceEntity.Free });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.MemberDebit)]
        public ActionResult SubtractMoney(MbrCardSubtractMoneyViewModel model)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var subtractResult = _mbrCardService.SubtractMoney(model.Id, model.AccountType, model.Money, model.InvNo, model.Remark);
            //发送短信
            if (subtractResult.Success)
            {
                var sendService = GetService<ISMSSendService>();
                var _sysParaService = GetService<ISysParaService>();
                var para = _sysParaService.GetSMSSendPara();
                var smsLogService = GetService<ISmsLogService>();
                sendService.SendMbrConsume(CurrentInfo.HotelId, model.Id.ToString(), subtractResult.Data.ToString(), para, smsLogService);
            }
            return Json(subtractResult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 积分调整
        [AuthButton(AuthFlag.IntegralAdjustment)]
        public ActionResult ChangeScore(Guid id)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            var balanceEntity = _mbrCardService.GetCardBalance(id);
            if (balanceEntity == null) { balanceEntity = new MbrCardBalance { Score = 0, scoreOwner = 0 }; }
            return PartialView("_ChangeScore", new MbrCardChangeScoreViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName, CurrentScore = balanceEntity.Score, CurrentOwnerScore = balanceEntity.scoreOwner });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.IntegralAdjustment)]
        public ActionResult ChangeScore(MbrCardChangeScoreViewModel model)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            return Json(_mbrCardService.ScoreAction(model.Id, model.AccountType, model.Score, model.InvNo, model.Remark), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 积分兑换
        [AuthButton(AuthFlag.IntegralExchange)]
        public ActionResult ScoreUse(Guid id)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.Get(id);
            var balanceEntity = _mbrCardService.GetCardBalance(id);
            GetItemScoreSelectList(entity.MbrCardTypeid);
            GetPayWapSelectList("ScoreUse");
            if (balanceEntity == null) { balanceEntity = new MbrCardBalance { Score = 0, scoreOwner = 0 }; }
            return PartialView("_ScoreUse", new MbrCardScoreUseViewModel() { Id = entity.Id, MbrCardNo = entity.MbrCardNo, GuestName = entity.GuestName, CurrentScore = balanceEntity.Score, CurrentOwnerScore = balanceEntity.scoreOwner });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.IntegralExchange)]
        public ActionResult ScoreUse(MbrCardScoreUseViewModel model)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            if (model.ItemType == "01")
            {
                return Json(_mbrCardService.ScoreUse(model.Id, model.ItemId, model.Score, model.InvNo, model.Remark), JsonRequestBehavior.AllowGet);
            }
            else if (model.ItemType == "02")
            {
                return Json(_mbrCardService.ScoreUse(model.Id, model.ItemId, model.PartScore, model.PartMoney, model.PayWayId, model.InvNo, model.Remark, model.OriginPartMoney), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonResultData.Failure("请选择兑换方式"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 批量延期
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult Delay(string id)
        {
            ViewBag.ids = id;
            return PartialView("_Delay");
        }
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult Delays(string id, DateTime validdate)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            return Json(_mbrCardService.DelayValidDate(id, validdate));
        }
        #endregion

        #region  批量更改业务员
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult UpdateSales(string id)
        {
            ViewBag.ids = id;
            return PartialView("_UpdateSales");
        }
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult UpdateSalees(string id, string sales, string remark)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            return Json(_mbrCardService.UpdateSales(id, sales, remark));
        }
        #endregion

        #region 批量积分调整
        [AuthButton(AuthFlag.IntegralAdjustment)]
        public ActionResult UpdateScore(string id)
        {
            ViewBag.ids = id;
            return PartialView("_BatchScore");
        }
        [AuthButton(AuthFlag.IntegralAdjustment)]
        public ActionResult UpdateScores(string id, string score, string accounttype, string invno, string remark)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            return Json(_mbrCardService.UpdateScore(id, score, accounttype, invno, remark));
        }
        #endregion

        #region 批量发放优惠券
        [AuthButton(AuthFlag.GiveCoupon)]
        public ActionResult GiveCoupon(string id)
        {
            ViewBag.ids = id;
            return PartialView("_BatchCoupon");
        }
        [AuthButton(AuthFlag.GiveCoupon)]
        public ActionResult GiveCoupons(string id, string ticketTypeid, string number, string remarks)
        {
            int result = 0;
            if(string.IsNullOrWhiteSpace(ticketTypeid))
            {
                return Json(JsonResultData.Failure("请选择优惠券类型"), JsonRequestBehavior.DenyGet);
            }
            if (int.TryParse(number, out result) && result > 0)
            {
                var _mbrCardService = GetService<IMbrCardService>();
                return Json(_mbrCardService.GiveCoupons(id, ticketTypeid, result, remarks));
            }
            else
            {
                return Json(JsonResultData.Failure("优惠券数量至少一张"), JsonRequestBehavior.DenyGet);
            }
        }
        [AuthButton(AuthFlag.None)]
        public ActionResult CheckTicketNo(string ticketNo, string money)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.GetProfileCard(ticketNo);
            if (entity == null)
            {
                return Json(JsonResultData.Failure("票券号不存在"));
            }
            var profileId = entity.Profileid;
            if (!_mbrCardService.IsHotelMbr(profileId))
            {
                return Json(JsonResultData.Failure("票券号不存在"));
            }
            if (entity.Status != 1)
            {
                return Json(JsonResultData.Failure("此票券已被使用"));
            }
            if (entity.EndDate <= DateTime.Now)
            {
                return Json(JsonResultData.Failure("此票券已过期"));
            }
            decimal? result = _mbrCardService.GetCouponMoney(ticketNo);
            if(!string.IsNullOrWhiteSpace(money))
            {
                if (decimal.Parse(money) > result)
                {
                    return Json(JsonResultData.Failure(string.Format("此代金券金额为{0}元,请修改原币金额", result)));
                }
            }
            return Json(JsonResultData.Successed(result));
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult GetTicket(string ticketTypeId)
        {
            var _mbrCardService = GetService<IMbrCardService>();
            var entity = _mbrCardService.GetTicket(ticketTypeId, CurrentInfo.GroupHotelId);
            return Json(JsonResultData.Successed(entity));
        }
        #endregion
    }
}