using Gemstar.BSPMS.Common.Services;
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
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 角色管理 
    /// </summary>
    [AuthPage("99001")]
    [AuthPage(ProductType.Member, "m99010001")]
    [AuthPage(ProductType.Pos, "p99010001")]
    [BusinessType("角色管理")]
    public class RoleManageController : BaseEditIncellController<Role, IRoleService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_role", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Role> addVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            return _Add(request, addVersions, w => { w.Roleid = Guid.NewGuid(); w.Hid = hid; }, OpLogType.角色增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Role> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<Role> originVersions)
        {
            return _Update(request, updatedVersions, originVersions, (list, u) => list.SingleOrDefault(w => w.Roleid == u.Roleid), OpLogType.角色修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var result = _BatchDelete(id, GetService<IRoleService>(), OpLogType.角色删除);
            GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
            return result;
        }
        #endregion
        #region 角色成员
        [AuthButton(AuthFlag.Members)]
        public ActionResult Member(string roleId)
        {
            var viewName = GetViewName("_Member");
            if (CurrentInfo.IsGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                var groupService = GetService<IUserRoleGroupService>();
                var userService = GetService<IPmsUserService>();
                var viewModel = new RoleMemberViewModelGroup
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId),
                    GroupUsers = userService.UsersInGroup(CurrentInfo.GroupId),
                    RoleMembers = groupService.ListHotelMembersInRole(CurrentInfo.GroupId, roleId)
                };
                return PartialView(viewName, viewModel);
            }
            else
            {
                var singleService = GetService<IUserRoleSingleService>();
                var viewModel = new RoleMemberViewModel
                {
                    RoleId = roleId,
                    UsersNotInRole = singleService.UsersNotInRole(CurrentInfo.HotelId, roleId),
                    UsersInRole = singleService.UsersInRole(CurrentInfo.HotelId, roleId)
                };

                return PartialView(viewName, viewModel);
            }
        }
        /// <summary>
        /// 获取当前集团下，指定酒店id下的角色成员信息
        /// 主要用于操作员在页面上选择了集团下的其他酒店时显示对应酒店下的角色成员
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>当前集团下指定分店的角色成员信息</returns>
        [AuthButton(AuthFlag.Members)]
        [JsonException]
        public ActionResult MemberInGroupHotel(string hid, string roleId)
        {
            var groupService = GetService<IUserRoleGroupService>();
            var viewModel = new
            {
                UsersInRole = groupService.UsersInRole(CurrentInfo.GroupId, hid, roleId).Select(w => new { w.Id, w.Name }),
                UsersNotInRole = groupService.UsersNotInRole(CurrentInfo.GroupId, hid, roleId).Select(w => new { w.Id, w.Name })
            };
            return Json(JsonResultData.Successed(viewModel));
        }
        [AuthButton(AuthFlag.Members)]
        [JsonException]
        public ActionResult MemberSingle(string roleId, List<string> roleMembers)
        {
            string hid = CurrentInfo.HotelId;
            var userRoleSingle = GetService<IUserRoleSingleService>();
            var oldMembers = userRoleSingle.Listrole(hid, Guid.Parse(roleId));//即将删除的数据
            var result = userRoleSingle.ResetRoleMembers(hid, roleId, roleMembers);
            GetService<IAuthCheck>().ClearHotelUserRoleCache(hid);
            //日志  
            AddRoleMenberLog(oldMembers, roleMembers, roleId, hid);
            return Json(result);
        }
        [AuthButton(AuthFlag.Members)]
        [JsonException]
        public ActionResult MemberGroup(string roleId, List<string> roleMembers)
        {
            var userRoleGroup = GetService<IUserRoleGroupService>();
            var oldMembers = userRoleGroup.ListHotelMembersInRole(CurrentInfo.GroupId, roleId);//即将删除的数据 

            var result = userRoleGroup.ResetRoleHotelMembers(CurrentInfo.GroupId, Guid.Parse(roleId), roleMembers.ToArray());
            GetService<IAuthCheck>().ClearHotelUserRoleCache(CurrentInfo.HotelId);
            //日志  
            var roleUsers = new List<string>();
            foreach (var m in roleMembers)
            {
                var infos = m.Split('|');
                if (infos.Length > 1)
                {
                    roleUsers.Add(infos[1]);
                }
            }
            AddRoleMenberLog(oldMembers, roleUsers, roleId, CurrentInfo.HotelId);
            return Json(result);
        }
        #endregion

        #region 添加操作日志
        /// <summary>
        /// 添加角色成员的日志
        /// </summary>
        /// <param name="oldMembers"></param>
        /// <param name="roleMembers"></param>
        /// <param name="roleId"></param>
        /// <param name="hid"></param>
        public void AddRoleMenberLog(List<UserRole> oldMembers, List<string> roleMembers, string roleId, string hid)
        {
            var user = GetService<IPmsUserService>();
            var rolename = GetService<IRoleService>().getRoleName(Guid.Parse(roleId));
            List<string> del = new List<string>();
            string op = "　　删除用户";
            for (int i = 0; i < oldMembers.Count(); i++)
            {
                if (roleMembers == null || !roleMembers.Contains(oldMembers[i].Userid.ToString()))
                {
                    op += user.GetUserName(hid, oldMembers[i].Userid) + "、";
                }
                del.Add(oldMembers[i].Userid.ToString());
            }
            op = (op == "　　删除用户" ? "" : string.Format("{0}的{1}角色", op.Trim('、'), rolename));
            string op1 = "　　添加用户";
            if (roleMembers != null)
            {
                for (int i = 0; i < roleMembers.Count(); i++)
                {
                    if (!del.Contains(roleMembers[i]))
                    {
                        op1 += user.GetUserName(hid, Guid.Parse(roleMembers[i])) + "、";
                    }
                }
            }
            op1 = (op1 == "　　添加用户" ? "" : string.Format("{0}的{1}角色", op1.Trim('、'), rolename));
            AddOperationLog(OpLogType.角色成员修改, "角色名称：" + rolename + "   " + ((op + op1) == "" ? "无修改！" : (op + op1)).Trim(','));
        }
        //判断两个字符串数组差距
        public string pre(string[] str12, string[] str22)
        {
            string[] str1 = str12;
            string[] str2 = str22;
            string arr3 = null;

            for (int i = 0; i < str2.Count(); i++)
            {
                Boolean flag = false;
                for (int j = 0; j < str1.Count(); j++)
                {
                    if (str2[i] == (str1[j]))
                    {
                        flag = true;
                    }

                }
                if (!flag)
                {
                    //if(str2[i])
                    arr3 += str2[i] + '、';
                }
            }
            return arr3 == null ? "" : arr3.Trim('、');
        }

        /// <summary>
        /// 添加操作权限的日志
        /// </summary>
        /// <param name="oldMembers"></param>
        /// <param name="roleMembers"></param>
        /// <param name="roleId"></param>
        /// <param name="hid"></param>
        public void AddAuthLog(List<AuthInfo> oldMembers, List<AuthInfo> authinfo, string roleId, string hid)
        {
            var al = GetService<IAuthListService>();
            List<string> roleMembers = new List<string>(); string old = "", xin = "";
            string authname = ""; string oldbj = ""; string xinbj = ""; string oldcode = "";
            foreach (var auth in authinfo)
            {
                string code = auth.AuthCode; string[] value = auth.AuthValues.ToString().Split(','); string[] value1 = "".Split(',');
                if (al.isrootauth(code)) { continue; }
                authname = al.GetAuthNamebycode(code);
                foreach (var auth1 in oldMembers)
                {
                    if (auth1.AuthCode == code)
                    {
                        value1 = auth1.AuthValues.ToString().Split(',');
                    }
                }
                oldbj = getauthname(pre(value, value1));
                xinbj = getauthname(pre(value1, value));
                old += (oldbj == "" ? "" : (authname + "：" + oldbj + "，"));
                xin += (xinbj == "" ? "" : (authname + "：" + xinbj + "，"));
                oldcode += code + ",";
            }
            string[] oldcodearr = oldcode.Trim(',').Split(',');
            foreach (var auth in oldMembers)
            {
                if (al.isrootauth(auth.AuthCode)) { continue; }
                if (!oldcodearr.Contains(auth.AuthCode))
                {
                    authname = al.GetAuthNamebycode(auth.AuthCode);
                    var value1 = auth.AuthValues.ToString().Split(',');
                    oldbj = getauthname(pre("".Split(','), value1));
                    old += (oldbj == "" ? "" : (authname + "：" + oldbj + "，"));
                }
            }
            var rolename = GetService<IRoleService>().getRoleName(Guid.Parse(roleId));
            string log = "角色名称：" + rolename + "  ";
            if (xin != "")
            {
                log += "　　添加操作权限（" + xin.Trim('，') + "）";
            }
            if (old != "")
            {
                log += "　　删除操作权限（" + old.Trim('，') + "）";
            }
            AddOperationLog(OpLogType.角色操作权限修改, log.Trim(','));

        }

        #region 角色按钮名称
        /// <summary>
        /// 获取角色按钮名称
        /// </summary>
        /// <param name="authid">角色按钮编号</param>
        /// <returns></returns>
        public string getauthname(string authid)
        {
            string[] arr = authid.Trim('、').Split('、');
            string retval = "";
            foreach (var str in arr)
            {
                switch (str.Trim())
                {
                    #region 内容
                    case "Add":
                        retval += "增加、";
                        break;
                    //case "None":
                    //    retval += "无权限、";
                    //    break;
                    case "Query":
                        retval += "查询、";
                        break;
                    case "Update":
                        retval += "修改、";
                        break;
                    case "Delete":
                        retval += "删除、";
                        break;
                    case "Export":
                        retval += "导出、";
                        break;
                    case "Print":
                        retval += "打印、";
                        break;
                    case "Details":
                        retval += "明细、";
                        break;
                    case "Members":
                        retval += "成员、";
                        break;
                    case "AuthManage":
                        retval += "权限维护、";
                        break;
                    case "Reset":
                        retval += "重置、";
                        break;
                    case "Disable":
                        retval += "导出、";
                        break;
                    case "Enable":
                        retval += "启用、";
                        break;
                    case "Open":
                        retval += "打开、";
                        break;
                    case "Consume":
                        retval += "消费、";
                        break;
                    case "CheckIn":
                        retval += "入住、";
                        break;
                    case "Accounting":
                        retval += "帐务、";
                        break;
                    case "SetDirty":
                        retval += "设置为脏房、";
                        break;
                    case "SetWaitClean":
                        retval += "设置为清洁房、";
                        break;
                    case "SetClean":
                        retval += "设置为净房、";
                        break;
                    case "Service":
                        retval += "维修、";
                        break;
                    case "Stop":
                        retval += "停用、";
                        break;
                    case "ReportAuthManage":
                        retval += "报表权限维护、";
                        break;
                    case "Close":
                        retval += "关闭班次、";
                        break;
                    case "ReplaceSalesman":
                        retval += "更换业务员、";
                        break;
                    case "ChangeCardNum":
                        retval += "换卡号、";
                        break;
                    case "UpgradeCard":
                        retval += "升级卡类型、";
                        break;
                    case "UpdateCardStatus":
                        retval += "变更卡状态、";
                        break;
                    case "Inspect":
                        retval += "审核、";
                        break;
                    case "MemberRecharge":
                        retval += "会员充值、";
                        break;
                    case "MemberDebit":
                        retval += "会员扣款、";
                        break;
                    case "IntegralExchange":
                        retval += "积分兑换、";
                        break;
                    case "IntegralAdjustment":
                        retval += "积分调整、";
                        break;
                    case "UpdateRecord":
                        retval += "变更记录、";
                        break;
                    case "transactionRecord":
                        retval += "交易记录、";
                        break;
                    case "consumptionRecord":
                        retval += "消费记录、";
                        break;
                    case "IntegrarChRecord":
                        retval += "积分兑换记录、";
                        break;
                    case "BatchDelay":
                        retval += "批量延期、";
                        break;
                    case "CancelOrderDetailY":
                        retval += "取消预订、";
                        break;
                    case "CancelOrderDetailZ":
                        retval += "取消入住、";
                        break;
                    case "RecoveryOrderDetailZ":
                        retval += "恢复入住、";
                        break;
                    case "RecoveryOrderDetailY":
                        retval += "恢复预订、";
                        break;
                    case "CheckoutCheck":
                        retval += "结账、";
                        break;
                    case "Out":
                        retval += "迟付、";
                        break;
                    case "ClearCheck":
                        retval += "清账、";
                        break;
                    case "AddCardAuth":
                        retval += "预授权、";
                        break;
                    case "Transfer":
                        retval += "转账、";
                        break;
                    case "CancelCheckout":
                        retval += "取消结账、";
                        break;
                    case "CancelClear":
                        retval += "取消清账、";
                        break;
                    case "CancelOut":
                        retval += "取消离店、";
                        break;
                    default:
                        retval += "";
                        break;
                        #endregion
                }
            }
            return retval.Trim('、');
        }
        #endregion
        /// <summary>
        /// 增加报表权限操作日志
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="codearr"></param>
        public void addreportauthlog(string roleid, string codearr, List<RoleAuthReport> report)
        {
            string oldcodearr = "";
            var ar1 = "".Split(',');
            for (int i = 0; i < report.Count; i++)
            {
                oldcodearr += report[i].ReportCode + "、";
            }
            ar1 = oldcodearr.Trim('、').Split('、');
            string oldbj = pre(ar1, codearr.Trim('、').Split('、')); string xinbj = pre(codearr.Trim('、').Split('、'), ar1);
            string log = (oldbj == "" ? "" : string.Format("　　增加{0}报表权限", oldbj));
            log += (xinbj == "" ? "" : string.Format("　　删除{0}报表权限", xinbj));
            var rolename = GetService<IRoleService>().getRoleName(Guid.Parse(roleid));
            AddOperationLog(OpLogType.角色报表权限修改, "角色名称：" + rolename + " " + (log == "" ? "无修改" : log.Trim(',')));
        }
        /// <summary>
        /// 增加消费录入权限操作日志
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="codearr"></param>
        public void addItemConsumeauthlog(string roleid, string codearr, List<RoleAuthItemConsume> ItemConsume)
        {

            string oldcodearr = "";
            var ar1 = "".Split(',');
            var itemserv = GetService<IItemService>();
            for (int i = 0; i < ItemConsume.Count; i++)
            {
                oldcodearr += (itemserv.Get(ItemConsume[i].ItemId) != null ? itemserv.Get(ItemConsume[i].ItemId).Name + "、" : "");
            }
            ar1 = oldcodearr.Trim('、').Split('、');
            string oldbj = pre(ar1, codearr.Trim('、').Split('、')); string xinbj = pre(codearr.Trim('、').Split('、'), ar1);
            string log = (oldbj == "" ? "" : string.Format("　　增加【{0}】的消费录入权限", oldbj));
            log += (xinbj == "" ? "" : string.Format("　　删除【{0}】的消费录入权限", xinbj));
            var rolename = GetService<IRoleService>().getRoleName(Guid.Parse(roleid));
            AddOperationLog(OpLogType.消费录入权限修改, "角色名称：" + rolename + " " + (log == "" ? "无修改" : log.Trim(',')));
        }

        #endregion

        #region 角色权限
        [AuthButton(AuthFlag.AuthManage)]
        public ActionResult AuthManage(string roleId)
        {
            var viewName = GetViewName("_AuthManage");
            var authManageService = GetService<IAuthListService>();
            var authLists = authManageService.GetAllAuthLists(CurrentInfo.ProductType, CurrentInfo.AuthListType, CurrentInfo.HotelId, roleId);
            var rootAuth = authLists.Single(w => w.ParentCode == "auth0");
            var rootTreeNode = ChangeToTreeViewItem(rootAuth, true);
            setTreeViewItems(rootTreeNode, rootAuth.AuthCode, authLists, true);
            ViewBag.rootTreeNode = rootTreeNode;
            if (CurrentInfo.IsGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                var viewModel = new AuthManageViewModelGroup
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode,
                    Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId)
                };
                return PartialView(viewName, viewModel);
            }
            else
            {
                var viewModel = new AuthManageViewModel
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode
                };
                return PartialView(viewName, viewModel);
            }
        }
        [AuthButton(AuthFlag.AuthManage)]
        [JsonException]
        public ActionResult AuthInGroupHotel(string roleId, string hid)
        {
            var authManageService = GetService<IAuthListService>();
            AuthType authType = hid == CurrentInfo.GroupId ? AuthType.Group : AuthType.GroupHotel;
            var authLists = authManageService.GetAllAuthLists(CurrentInfo.ProductType, authType, hid, roleId);
            var rootAuth = authLists.Single(w => w.ParentCode == "auth0");
            var rootTreeNode = ChangeToTreeViewItem(rootAuth, true);
            setTreeViewItems(rootTreeNode, rootAuth.AuthCode, authLists, true);
            return Json(JsonResultData.Successed(rootTreeNode.ToJsonViewModel()));
        }
        [AuthButton(AuthFlag.AuthManage)]
        [JsonException]
        public ActionResult AuthManageSave(string roleId, string hid, List<string> auths, string applyHotelIds)
        {
            var authInfos = new List<AuthInfo>();
            //处理菜单模块，有两种情况，一是菜单模块本身就有被选中，则会传递回auth节点
            if (auths != null)
            {
                var authList = auths.Where(w => w.StartsWith("auth")).ToList();
                foreach (var auth in authList)
                {
                    var authCode = auth.Substring(4);
                    var info = authInfos.Where(w => w.AuthCode == authCode).ToList();
                    if (info.Count == 0)
                    {
                        authInfos.Add(new AuthInfo { AuthCode = authCode });
                    }
                }
                //二是只选择了部分功能按钮的情况，因为此时由于不是全部选中，所以父模块默认为未选中，不会传递回auth节点
                var btnList = auths.Where(w => w.StartsWith("btn")).ToList();
                foreach (var btn in btnList)
                {
                    var splitIndex = btn.IndexOf('_');
                    var authCode = btn.Substring(3, splitIndex - 3);
                    var info = authInfos.Where(w => w.AuthCode == authCode).ToList();
                    if (info.Count == 0)
                    {
                        authInfos.Add(new AuthInfo { AuthCode = authCode });
                    }
                }
                //处理所有有权限菜单模块的功能按钮权限
                foreach (var auth in authInfos)
                {
                    var authButtons = auths.Where(w => w.StartsWith("btn" + auth.AuthCode + "_")).ToList();
                    var authValue = AuthFlag.None;
                    foreach (var btn in authButtons)
                    {
                        var btnValue = Convert.ToInt64(btn.Split('_')[1]);
                        authValue = authValue | (AuthFlag)btnValue;
                    }
                    auth.AuthValues = authValue;
                }
            }
            var authManageService = GetService<IAuthListService>();
            var authCheckService = GetService<IAuthCheck>();
            JsonResultData result = setRoleAuthsInHotel(roleId, hid, authInfos, authManageService, authCheckService);
            if (!string.IsNullOrWhiteSpace(applyHotelIds))
            {
                //如果同时指定了应用到其他分店
                var applyHotelIdArray = applyHotelIds.Split(',');
                foreach (var applyHid in applyHotelIdArray)
                {
                    if (!string.IsNullOrWhiteSpace(applyHid) && applyHid != hid)
                    {
                        setRoleAuthsInHotel(roleId, applyHid, authInfos, authManageService, authCheckService);
                    }
                }
            }
            return Json(result);
        }

        private JsonResultData setRoleAuthsInHotel(string roleId, string hid, List<AuthInfo> authInfos, IAuthListService authManageService, IAuthCheck authCheckService)
        {
            var oldMembers = authManageService.Listrole(hid, Guid.Parse(roleId));//即将删除的数据 
            var result = authManageService.ResetRoleAuths(CurrentInfo.ProductType, roleId, hid, authInfos);
            authCheckService.ClearHotelRoleAuthCache(hid);
            AddAuthLog(oldMembers, authInfos, roleId, hid);
            return result;
        }

        private void setTreeViewItems(TreeViewItemModel parentItem, string parentAuthCode, List<UpQueryAuthListForRoleResult> allAuths, bool expanded)
        {
            var childAuths = allAuths.Where(w => w.ParentCode == parentAuthCode).ToList();
            foreach (var auth in childAuths)
            {
                var treeItem = ChangeToTreeViewItem(auth, expanded);
                var count = allAuths.Count(w => w.ParentCode == auth.AuthCode);
                treeItem.HasChildren = count > 0;
                parentItem.Items.Add(treeItem);
                setTreeViewItems(treeItem, auth.AuthCode, allAuths, false);
            }
        }
        private TreeViewItemModel ChangeToTreeViewItem(UpQueryAuthListForRoleResult auth, bool expanded)
        {
            var item = new TreeViewItemModel
            {
                Text = auth.AuthName,
                Id = auth.AuthCode,
                Checked = auth.Checked,
                Expanded = expanded
            };
            return item;
        }
        #endregion
        #region 消费项目权限

        /// <summary>
        /// 消费项目权限设置
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ItemConsumeRoleAuth)]
        public ActionResult ItemConsumeRoleAuth(string roleId)
        {
            var viewName = GetViewName("_ItemConsumeRoleAuth");
            var authReportManage = GetService<IRoleAuthItemConsumeService>();
            var authLists = authReportManage.GetV_roleItemConsumelist(CurrentInfo.HotelId, roleId);
            var rootAuth = authLists.Single(w => w.type == "-1");
            var rootTreeNode = ChangeToTreeViewItemConsume(rootAuth, true);
            setTreeViewItemConsume(rootTreeNode, rootAuth.Code, authLists, true); ViewBag.roleid = roleId;
            if (CurrentInfo.IsGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                var viewModel = new AuthManageViewModelGroup
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode,
                    Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId)
                };
                return PartialView(viewName, viewModel);
            }
            else
            {
                var viewModel = new AuthManageViewModel
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode
                };
                return PartialView(viewName, viewModel);
            }
        }
        private TreeViewItemModel ChangeToTreeViewItemConsume(V_ReportList auth, bool expanded)
        {
            var item = new TreeViewItemModel
            {
                Text = auth.Name,
                Id = auth.Code,
                Checked = bool.Parse(auth.isAllow),
                Expanded = expanded
            };
            return item;
        }
        private void setTreeViewItemConsume(TreeViewItemModel parentItem, string parentAuthCode, List<V_ReportList> allAuths, bool expanded)
        {
            var childAuths = allAuths.Where(w => w.type == parentAuthCode).ToList();
            foreach (var auth in childAuths)
            {
                var treeItem = ChangeToTreeViewItemConsume(auth, expanded);
                parentItem.Items.Add(treeItem);
                setTreeViewItemConsume(treeItem, auth.Code, allAuths, false);
            }
        }
        /// <summary>
        /// 设置角色报表权限
        /// </summary>
        /// <param name="reportCodes">需要设置为该角色可查看的报表</param>
        /// <param name="roleid">角色编号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ItemConsumeRoleAuth)]
        public ActionResult saveChangeItemConsumeroleauth(List<string> ItemConsumeCodes, string roleid, string hid, string applyHotelIds)
        {

            string codearr = "";
            var ar = ItemConsumeCodes[0].ToString().Trim(',').Split(',');
            var itemserv = GetService<IItemService>();
            for (int i = 0; i < ar.Length; i++)
            {
                if (!ar[i].Contains(".") && ar[i] != "0")
                {
                    codearr += (itemserv.Get(ar[i]) != null ? itemserv.Get(ar[i]).Name + "、" : "");
                }
            }
            string hc = "";
            var authreportserv = GetService<IRoleAuthItemConsumeService>();
            if (string.IsNullOrWhiteSpace(applyHotelIds))
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    hid = CurrentInfo.HotelId;
                }
                List<RoleAuthItemConsume> ItemConsume = authreportserv.GetItemConsumeIsAllow(hid, roleid);
                hc = authreportserv.ChangeRoleAuthItemConsume(hid, roleid, ItemConsumeCodes[0]);
                addItemConsumeauthlog(roleid, codearr, ItemConsume);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    hid = CurrentInfo.HotelId;
                }

                List<RoleAuthItemConsume> ItemConsume = authreportserv.GetItemConsumeIsAllow(hid, roleid);
                hc = authreportserv.ChangeRoleAuthItemConsume(hid, roleid, ItemConsumeCodes[0]);
                addItemConsumeauthlog(roleid, codearr, ItemConsume);

                var applyHotelIdArray = applyHotelIds.Split(',');
                foreach (var applyHid in applyHotelIdArray)
                {
                    if (!string.IsNullOrWhiteSpace(applyHid) && applyHid != hid)
                    {
                        hc = authreportserv.ChangeRoleAuthItemConsume(applyHid, roleid, ItemConsumeCodes[0]);
                    }
                }
            }
            return Json(hc, JsonRequestBehavior.AllowGet);
        }


        [AuthButton(AuthFlag.AuthManage)]
        [JsonException]
        public ActionResult AuthItemConsumeInGroupHotel(string roleId, string hid)
        {
            var authReportManage = GetService<IRoleAuthItemConsumeService>();
            AuthType authType = hid == CurrentInfo.GroupId ? AuthType.Group : AuthType.GroupHotel;
            var authLists = authReportManage.GetV_roleItemConsumelist(hid, roleId);
            var rootAuth = authLists.Single(w => w.type == "-1");
            var rootTreeNode = ChangeToTreeViewItemConsume(rootAuth, true);
            setTreeViewItemConsume(rootTreeNode, rootAuth.Code, authLists, true);
            return Json(JsonResultData.Successed(rootTreeNode.ToJsonViewModel()));
        }
        #endregion
        #region 报表角色权限
        /// <summary>
        /// 显示报表设置数
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ReportAuthManage)]
        public ActionResult ReportRoleAuth(string roleId)
        {
            var viewName = GetViewName("_ReportRoleAuth");
            var authReportManage = GetService<IRoleAuthReportService>();
            var authLists = authReportManage.GetV_ReportLists(CurrentInfo.ProductType, CurrentInfo.HotelId, roleId);
            var rootAuth = authLists.Single(w => w.type == "-1");
            var rootTreeNode = ChangeToTreeViewItemReport(rootAuth, true);
            setTreeViewItemsReport(rootTreeNode, rootAuth.Code, authLists, true); ViewBag.roleid = roleId;
            if (CurrentInfo.IsGroup)
            {
                var pmsHotelService = GetService<IPmsHotelService>();
                var viewModel = new AuthManageViewModelGroup
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode,
                    Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId)
                };
                return PartialView(viewName, viewModel);
            }
            else
            {
                var viewModel = new AuthManageViewModel
                {
                    RoleId = roleId,
                    Hid = CurrentInfo.HotelId,
                    RootAuth = rootTreeNode
                };
                return PartialView(viewName, viewModel);
            }
        }
        private TreeViewItemModel ChangeToTreeViewItemReport(V_ReportList auth, bool expanded)
        {
            var item = new TreeViewItemModel
            {
                Text = auth.Name,
                Id = auth.Code,
                Checked = bool.Parse(auth.isAllow),
                Expanded = expanded
            };
            return item;
        }
        private void setTreeViewItemsReport(TreeViewItemModel parentItem, string parentAuthCode, List<V_ReportList> allAuths, bool expanded)
        {
            var childAuths = allAuths.Where(w => w.type == parentAuthCode).ToList();
            foreach (var auth in childAuths)
            {
                var treeItem = ChangeToTreeViewItemReport(auth, expanded);
                parentItem.Items.Add(treeItem);
                setTreeViewItemsReport(treeItem, auth.Code, allAuths, false);
            }
        }
        /// <summary>
        /// 设置角色报表权限
        /// </summary>
        /// <param name="reportCodes">需要设置为该角色可查看的报表</param>
        /// <param name="roleid">角色编号</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.ReportAuthManage)]
        public ActionResult saveChangeroleauth(List<string> reportCodes, string roleid, string hid, string applyHotelIds)
        {

            string codearr = "";
            var ar = reportCodes[0].ToString().Trim(',').Split(',');
            for (int i = 0; i < ar.Length; i++)
            {
                if (!ar[i].Contains(".") && ar[i] != "0")
                {
                    codearr += ar[i] + "、";
                }
            }
            string hc = "";
            var authreportserv = GetService<IRoleAuthReportService>();
            if (string.IsNullOrWhiteSpace(applyHotelIds))
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    hid = CurrentInfo.HotelId;
                }
                List<RoleAuthReport> report = authreportserv.GetReportsIsAllow(hid, roleid);
                hc = authreportserv.ChangeRoleAuthReport(CurrentInfo.ProductType, hid, roleid, reportCodes[0]);
                addreportauthlog(roleid, codearr, report);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(hid))
                {
                    hid = CurrentInfo.HotelId;
                }

                List<RoleAuthReport> report = authreportserv.GetReportsIsAllow(hid, roleid);
                hc = authreportserv.ChangeRoleAuthReport(CurrentInfo.ProductType, hid, roleid, reportCodes[0]);
                addreportauthlog(roleid, codearr, report);

                var applyHotelIdArray = applyHotelIds.Split(',');
                foreach (var applyHid in applyHotelIdArray)
                {
                    if (!string.IsNullOrWhiteSpace(applyHid) && applyHid != hid)
                    {
                        hc = authreportserv.ChangeRoleAuthReport(CurrentInfo.ProductType, applyHid, roleid, reportCodes[0]);
                    }
                }
            }
            return Json(hc, JsonRequestBehavior.AllowGet);
        }


        [AuthButton(AuthFlag.AuthManage)]
        [JsonException]
        public ActionResult AuthReportInGroupHotel(string roleId, string hid)
        {
            var authReportManage = GetService<IRoleAuthReportService>();
            AuthType authType = hid == CurrentInfo.GroupId ? AuthType.Group : AuthType.GroupHotel;
            var authLists = authReportManage.GetV_ReportLists(CurrentInfo.ProductType, hid, roleId);
            var rootAuth = authLists.Single(w => w.type == "-1");
            var rootTreeNode = ChangeToTreeViewItemReport(rootAuth, true);
            setTreeViewItemsReport(rootTreeNode, rootAuth.Code, authLists, true);
            return Json(JsonResultData.Successed(rootTreeNode.ToJsonViewModel()));
        }

        #endregion

        #region 权限查询
        [AuthButton(AuthFlag.AuthManage)]
        public ActionResult AuthManageForQuery(string roleId, string QueryText)
        {
            var authManageService = GetService<IAuthListService>();
            var authLists = new List<UpQueryAuthListForRoleResult>();
            if (string.IsNullOrEmpty(QueryText))
            {
                authLists = authManageService.GetAllAuthLists(CurrentInfo.ProductType, CurrentInfo.AuthListType, CurrentInfo.HotelId, roleId);
            }
            else
            {
                authLists = authManageService.GetAllAuthLists(CurrentInfo.ProductType, CurrentInfo.AuthListType, CurrentInfo.HotelId, roleId, QueryText);
            }
            var rootAuth = authLists.Where(m => m.AuthCode == (authLists.Min(c => c.AuthCode))).FirstOrDefault();
            var rootTreeNode = ChangeToTreeViewItem(rootAuth, true);
            setTreeViewItems(rootTreeNode, rootAuth.AuthCode, authLists, true);
            ViewBag.rootTreeNode = rootTreeNode;
            return PartialView("AuthManageForQuery");
        }
        #endregion
    }
}
