using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosTab;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos餐台资料
    /// </summary>
    [AuthPage(ProductType.Pos, "p99070003")]
    public class BasicDataPosTabController : BaseEditInWindowController<PosTab, IPosTabService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_tab", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new TabAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(TabAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.TabNo;
            var modelService = GetService<IPosTabService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.TabNo, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            var tabtypeService = GetService<IPosTabtypeService>();
            var tabtype = tabtypeService.Get(addViewModel.TabTypeid);
            addViewModel.TabTypeCode = tabtype.Code;
            addViewModel.TabTypeName = tabtype.Cname;

            if (Request["ProdPrinter"] != null)
            {
                addViewModel.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",", "");
            }

            ActionResult result = _Add(addViewModel, new PosTab { Id = id, Hid = CurrentInfo.HotelId, TabTypeCode = tabtype.Code, MaxSeat = tabtype.MaxSeat, TabTypeName = tabtype.Cname, ModifiedDate = DateTime.Now }, OpLogType.Pos餐台资料增加);

            //初始化餐台状态
            var tabStatusService = GetService<IPosTabStatusService>();
            tabStatusService.SetTabStatus(CurrentInfo.HotelId, addViewModel.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new TabEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(TabEditViewModel model)
        {
            var modelService = GetService<IPosTabService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.TabNo, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            var tabtypeService = GetService<IPosTabtypeService>();
            var tabtype = tabtypeService.Get(model.TabTypeid);

            //出品打印机为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["ProdPrinter"] != null)
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",", "");
            }
            else
            {
                Request.Form.Add("ProdPrinter", "");
            }

            ActionResult result = _Edit(model, new PosTab { TabTypeCode = tabtype.Code, MaxSeat = tabtype.MaxSeat, TabTypeName = tabtype.Cname, ModifiedDate = DateTime.Now }, OpLogType.Pos餐台资料修改);

            //初始化餐台状态
            var tabStatusService = GetService<IPosTabStatusService>();
            tabStatusService.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");

            //更新餐台状态的台号和名称
            var tabStatus = tabStatusService.Get(model.Id);
            if (tabStatus != null)
            {
                tabStatus.TabNo = model.TabNo;
                tabStatus.TabName = model.Cname;
                tabStatus.Refeid = model.Refeid;
                tabStatusService.Update(tabStatus, new PosTabStatus());
                tabStatusService.Commit();
            }

            return result;
        }
        #endregion

        #region 餐台复制
        [AuthButton(AuthFlag.Export)]
        public ActionResult Copy(string tabid)
        {
            //获取tab
            var tabservice = GetService<IPosTabService>();
            var tab = tabservice.GetEntity(CurrentInfo.HotelId, tabid);
            if (tab == null)
            {
                return Json(JsonResultData.Failure("要复制餐台未找到"));
            }
            var model = new TabAddViewModel();
            //值复制
            AutoSetValueHelper.SetValues(tab, model);
            model.Cname = "";
            model.Ename = "";
            model.TabNo = "";

            ArrayList arrlist = new ArrayList();
            if (!string.IsNullOrEmpty(model.ProdPrinter))
            {

                string tempStr = model.ProdPrinter;
                var charNumber = 3;
                for (int i = 0; i < tempStr.Length; i += charNumber)    //首先判断字符串的长度，循环截取，进去循环后首先判断字符串是否大于每段的长度
                {
                    if ((tempStr.Length - i) > charNumber)  //如果是，就截取
                    {
                        arrlist.Add(tempStr.Substring(i, charNumber));
                    }
                    else
                    {
                        arrlist.Add(tempStr.Substring(i));  //如果不是，就截取最后剩下的那部分
                    }
                }
            }
            model.ProdPrinters = (string[])arrlist.ToArray(typeof(string));
            return _Add(model);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var service = GetService<IPosTabService>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的数据！"));
            }
            if (id == "0")
            {
                return Json(JsonResultData.Failure("要删除的数据不存在！"));
            }
            var idArray = id.Split(',');
            foreach (var tabid in idArray)
            {
                var postab = service.GetEntity(CurrentInfo.HotelId, tabid);
                //初始化餐台状态

                service.Delete(postab);
                if (postab != null)
                {
                    var tabStatusService = GetService<IPosTabStatusService>();
                    tabStatusService.SetTabStatus(CurrentInfo.HotelId, postab.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");
                }
                service.AddDataChangeLog(OpLogType.Pos餐台资料删除);
                service.Commit();
            }

            return Json(JsonResultData.Successed(""));
            //   var postab = service.GetEntity(CurrentInfo.HotelId, id);
            // var result = _BatchDelete(id, GetService<IPosTabService>(), OpLogType.Pos餐台资料删除);
            //return result;
        }
        #endregion

        #region 下拉列表
        /// <summary>
        /// 根据餐台类型ID得到餐台类型信息
        /// </summary>
        /// <param name="TabTypeId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult GetPosTabType(string TabTypeId)
        {
            var tabtypeService = GetService<IPosTabtypeService>();
            var tabtype = tabtypeService.Get(TabTypeId);
            var model = new
            {
                TabTypeCode = tabtype.Code,
                MaxSeat = tabtype.MaxSeat,
                TabTypeName = tabtype.Cname
            };

            return Json(JsonResultData.Successed(model));
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult GetOpenType()
        {
            var itemlist = EnumHelper.GetSelectList<PosTabOpenType>();
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {

            var service = GetService<IPosTabService>();

            var refe = GetService<IPosRefeService>();

            var model = service.Get(id);


            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
            if (model != null)
            {
                var refeModel = refe.Get(model.Refeid);
                //初始化餐台状态


                var tabStatusService = GetService<IPosTabStatusService>();
                tabStatusService.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");
            }

            return reval;
        }
        #endregion

        #region 禁用
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {

            var service = GetService<IPosTabService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));
            var refe = GetService<IPosRefeService>();
            var model = service.Get(id);
            if (model != null)
            {
                var refeModel = refe.Get(model.Refeid);
                //初始化餐台状态


                var tabStatusService = GetService<IPosTabStatusService>();
                tabStatusService.SetTabStatus(CurrentInfo.HotelId, model.Refeid, (byte)PosTabStatusOpType.初始化, "", "", "");
            }
            return reval;

        }
        #endregion

        #region 餐台选择出品部门界面
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _DeptDepartList(string id = "")
        {
            var service = GetService<IPosDepartService>();

            var list = service.GetDepartByProc(CurrentInfo.HotelId).Where(w => w.Module == CurrentInfo.ModuleCode).ToList();

            return PartialView("_DeptDepartList", list);
        }
        #endregion
    }
}