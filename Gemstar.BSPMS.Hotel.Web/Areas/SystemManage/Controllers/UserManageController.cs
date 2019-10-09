using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.UserManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using Gemstar.BSPMS.Common.Services;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using System.Web.Script.Serialization;
using System.Reflection;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    [AuthPage("99003")]
    [AuthPage(ProductType.Member, "m99010003")]
    [AuthPage(ProductType.Pos, "p99010003")]
    public class UserManageController : BaseEditInWindowController<PmsUser, IPmsUserService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroup)  //仅当集团在集团里设置的时候才显示集团用户，否者查询单店用户
            {
                SetCommonQueryValues("up_pos_list_pmsGroupUser", "");
            }
            else
            {
                SetCommonQueryValues("up_pos_list_pmsUser","");
            }
            IsOpenAnalysis();
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            IsOpenAnalysis();
            if (CurrentInfo.IsGroup)
            {
                var groupViewModel = new UserAddGroupViewModel();
                var hotelService = GetService<IPmsHotelService>();
                groupViewModel.Hotels = hotelService.GetHotelsInGroup(CurrentInfo.GroupId);
                var roleService = GetService<IRoleService>();
                groupViewModel.Roles = roleService.List(CurrentInfo.GroupId).ToList();
                return _Add(groupViewModel, "_AddGroup");
            }
            return _Add(new UserAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(UserAddViewModel userViewModel)
        {
            Guid id = Guid.NewGuid();
            var PmsUserserv = GetService<IPmsUserService>();
            //for(int i=0;i<id.)
            userViewModel.CardId = CryptHelper.EncryptDES(userViewModel.CardId);

            if (!string.IsNullOrEmpty(Request["OperatorStatus"]))
            {
                userViewModel.OperatorStatus = Request["OperatorStatus"].ToString();
            }
            bool isexsit = PmsUserserv.IsExists(CurrentInfo.GroupHotelId, userViewModel.Code, userViewModel.Name, userViewModel.CardId);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复登录名 或 重复操作员 或 重复卡号！")); }
            


            ActionResult result = _Add(userViewModel, new PmsUser { Id = id, Status = Common.Services.EntityStatus.启用, Grpid = CurrentInfo.GroupHotelId, IsReg = 0 }, OpLogType.操作员增加);
            JsonResult jsonResult = result as JsonResult;
            if (jsonResult != null)
            {
                JsonResultData jsonResultData = jsonResult.Data as JsonResultData;
                if (jsonResultData != null && jsonResultData.Success == true)
                {
                    if (userViewModel.RoleList == null)
                    {
                        userViewModel.RoleList = new List<Guid>();
                    }
                    GetService<IUserRoleSingleService>().ResetMemberRoles(CurrentInfo.HotelId, id, userViewModel.RoleList);

                    GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
                }
            }

            return result;
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult AddGroup(UserAddGroupViewModel userViewModel)
        {
            Guid id = Guid.NewGuid();
            var PmsUserserv = GetService<IPmsUserService>();
            //for(int i=0;i<id.)
            userViewModel.CardId = CryptHelper.EncryptDES(userViewModel.CardId);
            bool isexsit = PmsUserserv.IsExists(CurrentInfo.GroupHotelId, userViewModel.Code, userViewModel.Name, userViewModel.CardId);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复登录名 或 重复操作员 或 重复卡号！")); }
            
            ActionResult result = _Add(userViewModel, new PmsUser { Id = id, Status = Common.Services.EntityStatus.启用, Grpid = CurrentInfo.GroupHotelId, IsReg = 0 }, OpLogType.操作员增加);
            JsonResult jsonResult = result as JsonResult;
            if (jsonResult != null)
            {
                JsonResultData jsonResultData = jsonResult.Data as JsonResultData;
                if (jsonResultData != null && jsonResultData.Success == true)
                {
                    var hotelRoleIds = userViewModel.HotelRoleIds ?? "";
                    GetService<IUserRoleGroupService>().ResetMemberHotelRoles(CurrentInfo.GroupId, id, hotelRoleIds.Split(','));

                    GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
                }
                //增加所属分店的操作员
                if (!string.IsNullOrEmpty(userViewModel.Belonghotel))
                {
                    PmsUserserv.GroupControlAdd(userViewModel.Code, userViewModel.Belonghotel, CurrentInfo.HotelId);
                }
            }
            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            Guid idValue = Guid.Parse(id);
            IsOpenAnalysis();
            var userService = GetService<IPmsUserService>();
            var entity = userService.Get(idValue);
            entity.CardId = CryptHelper.DecryptDES(entity.CardId);
            if (CurrentInfo.IsGroup )
            {
                var groupViewModel = new UserEditGroupViewModel();
                var hotelService = GetService<IPmsHotelService>();
                groupViewModel.Hotels = hotelService.GetHotelsInGroup(CurrentInfo.GroupId);
                var roleService = GetService<IRoleService>();
                groupViewModel.Roles = roleService.List(CurrentInfo.GroupId).ToList();
                var userRoleGroupService = GetService<IUserRoleGroupService>();
                var hotelRolesForUser = userRoleGroupService.ListHotelRoles(CurrentInfo.GroupId, idValue);
                groupViewModel.UserRoles = hotelRolesForUser;
                string[] listBelonghotels = new string[] { };
                if (!string.IsNullOrWhiteSpace(entity.Belonghotel))
                {
                    listBelonghotels = entity.Belonghotel.Split(',');
                }
            
                ViewBag.listBelonghotels = listBelonghotels;
                var serializer = new JavaScriptSerializer();
                AutoSetValueHelper.SetValues(entity, groupViewModel);
                groupViewModel.OriginJsonData = serializer.Serialize(entity);
                //卡号验证
                {
                    //根据账号和酒店代码查询用户
                    var user = userService.GetUserIDByCode(CurrentInfo.HotelId, entity.Code);
                    if (user != null)
                    {
                        groupViewModel.CardId = CryptHelper.DecryptDES(user.CardId);
                    }                 
                }
                return PartialView("_EditGroup", groupViewModel);
            }
            List<Guid> roleList = GetService<IUserRoleSingleService>().List(CurrentInfo.HotelId, idValue).Select(c => c.Roleid).ToList();
            return _Edit(idValue, new UserEditViewModel() { RoleList = roleList, CardId = entity.CardId });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(UserEditViewModel model)
        {
            model.CardId = CryptHelper.EncryptDES(model.CardId);
            bool isexsit = GetService<IPmsUserService>().IsExists(CurrentInfo.GroupHotelId, model.Code, model.Name, model.CardId, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复操作员 或 重复卡号！")); }

            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["OperatorStatus"] == null)
            {
                Request.Form.Add("OperatorStatus", "");
            }


            if (!string.IsNullOrEmpty(Request["OperatorStatus"]))
            {
                model.OperatorStatus = Request["OperatorStatus"].ToString();
            }

            ActionResult result = _Edit(model, new PmsUser(), OpLogType.操作员修改);
            JsonResult jsonResult = result as JsonResult;
            if (jsonResult != null)
            {
                JsonResultData jsonResultData = jsonResult.Data as JsonResultData;
                if (jsonResultData != null && jsonResultData.Success == true)
                {
                    if (model.RoleList == null)
                    {
                        model.RoleList = new List<Guid>();
                    }
                    GetService<IUserRoleSingleService>().ResetMemberRoles(CurrentInfo.HotelId, model.Id, model.RoleList);
                    GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
                } 
            }
            return result;
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(UserEditGroupViewModel model)
        {
            var PmsUserserv = GetService<IPmsUserService>();
            bool isexsit = PmsUserserv.IsExists(CurrentInfo.GroupHotelId, model.Code, model.Name, model.CardId, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复操作员 或 重复卡号！")); }

            var updatecard = CryptHelper.EncryptDES(model.CardId);


         

            model.CardId = updatecard;

            //查询是否在单店操作，单店操作卡号修改只修改单店操作员
            if (CurrentInfo.GroupId != CurrentInfo.HotelId)
            {
                //根据账号和酒店代码查询用户
                var user = PmsUserserv.GetUserIDByCode(CurrentInfo.HotelId, model.Code);
                if (user != null)
                {
                    var olduser = model.GetOriginObject<PmsUser>();
                    model.CardId = CryptHelper.EncryptDES(olduser.CardId);
                    user.CardId = updatecard;
                    PmsUserserv.Update(user, null);
                    PmsUserserv.Commit();
                }          
            }
            ActionResult result = _Edit(model, new PmsUser(), OpLogType.操作员修改);
            JsonResult jsonResult = result as JsonResult;
            if (jsonResult != null)
            {
                JsonResultData jsonResultData = jsonResult.Data as JsonResultData;
                if (jsonResultData != null && jsonResultData.Success == true)
                {
                    var hotelRoleIds = model.HotelRoleIds ?? "";
                    GetService<IUserRoleGroupService>().ResetMemberHotelRoles(CurrentInfo.GroupId, model.Id, hotelRoleIds.Split(','));

                    GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
                }


              
                //修改所属分店的操作员信息
                if (!string.IsNullOrEmpty(model.Belonghotel))
                {
                    PmsUserserv.GroupControlEdit(model.Code, model.Belonghotel, CurrentInfo.HotelId);
                }
            }
            return result;
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<IPmsUserService>().getGrouphotelid(id);
            }
            return Json(GetService<IPmsUserService>().BatchUpdateStatus(id, Common.Services.EntityStatus.启用), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<IPmsUserService>().getGrouphotelid(id);
            }
            return Json(GetService<IPmsUserService>().BatchUpdateStatus(id, Common.Services.EntityStatus.禁用), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<IPmsUserService>().getGrouphotelid(id);
            }
            return _BatchDelete(id, GetService<IPmsUserService>(), OpLogType.操作员删除);
        }
        #endregion
        #region 下拉列表
        [AuthButton(AuthFlag.None)]
        public JsonResult RoleList()
        {
            var datas = GetService<IRoleService>().List(CurrentInfo.HotelId).OrderBy(c => c.Seqid).Select(c => new { c.Roleid, c.Authname }).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Roleid.ToString(), Text = w.Authname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        [NotAuthAttribute]
        public JsonResult GetHotelSelectList()
        {
            var hotellist = GetService<IPmsHotelService>().GetHotelsInGroup(CurrentInfo.HotelId).Where(w => w.Hid != CurrentInfo.HotelId);
            var list = hotellist.Select(s => new SelectListItem { Value = s.Hid, Text = string.IsNullOrEmpty(s.Hotelshortname) ? s.Name : s.Hotelshortname }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region 删除判断
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {

            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            //var payclass=GetService<>
            var PmsUserserv = GetService<IPmsUserService>();
            //for(int i=0;i<id.)
            bool isreg = PmsUserserv.IsRegUser(id[0].ToString());
            if (!isreg)
            {
                return Json(JsonResultData.Successed("可以删除！"));
            }
            else
            {
                return Json(JsonResultData.Failure("注册用户不可删除！"));
            }
        }
        #endregion
        #region 重置密码
        [AuthButton(AuthFlag.Update)]
        public ActionResult ResetPwd(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            //var payclass=GetService<>
            var PmsUserserv = GetService<IPmsUserService>();
            //for(int i=0;i<id.)
            JsonResultData isreg = PmsUserserv.ResetPwds(id);
            return Json(isreg);
        }
        #endregion
        #region 获取是否开启总裁驾驶舱
        /// <summary>
        /// 获取是否开启总裁驾驶舱
        /// </summary>
        private void IsOpenAnalysis()
        {
            ViewBag.isOpenAnalysis = GetService<IPmsHotelService>().IsOpenAnalysis(CurrentInfo.HotelId);
        }
        #endregion


        #region MyRegion
        /// <summary>
        /// 获取操作员身份
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForOperatorStatus()
        {
            var itemService = GetService<IItemService>();
            var datas = itemService.GetCodeListPub("90");
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}