using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosRefe;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos营业点维护
    /// </summary>
    [AuthPage(ProductType.Pos, "p99021001")]
    public class BasicDataPosRefeController : BaseEditInWindowController<PosRefe, IPosRefeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Refe", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosRefeAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosRefeAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosRefeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            //var HotelType = GetHotelServicesType();//酒店类型

            //ActionResult result;
            //if (HotelType == "B" || HotelType == "C")
            //{
            //    //开台录入信息内容
            //    addViewModel.OpenInfo = string.IsNullOrEmpty(addViewModel.OpenInfo) ? (HotelType == "B" ? "J" : "I") : addViewModel.OpenInfo.ToString();
            //    //判断营业点是否有数据（有数据正常添加，没有则默认添加一条收银点，市别，班次等信息）
            //    var posRefService = GetService<IPosRefeService>();
            //    var count = posRefService.GetRefe(CurrentInfo.HotelId).ToList().Count;
            //    if (count > 0)
            //    {
            //        result = _Add(addViewModel, new PosRefe { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos营业点增加);
            //    }
            //    else
            //    {
            //        //创建默认数据
            //        CreatePosDate(id);
            //        result = _Add(addViewModel, new PosRefe { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos营业点增加);
            //    }
            //}
            //else
            //{
            //    result = _Add(addViewModel, new PosRefe { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos营业点增加);
            //}
            //if (!string.IsNullOrEmpty(Request["OpenInfo"]))
            //{
            //    addViewModel.OpenInfo = Request["OpenInfo"].ToString();
            //    if (addViewModel.OpenInfo.IndexOf("I", System.StringComparison.CurrentCultureIgnoreCase) > -1
            //        && addViewModel.OpenInfo.IndexOf("J", System.StringComparison.CurrentCultureIgnoreCase) > -1)
            //    {
            //        return Json(JsonResultData.Failure("“自动虚拟台”和“手工指定台”只能选择一项！"));
            //    }
            //}
            if (!string.IsNullOrEmpty(Request["FloorShowData"]))
            {
                addViewModel.FloorShowData = Request["FloorShowData"].ToString();
                var floorShowDataList = addViewModel.FloorShowData.Split(',');
                if (floorShowDataList.Length > 2)
                {
                    return Json(JsonResultData.Failure("楼面台号显示内容只能选择两项！"));
                }
            }
            //该字段没有用了。为了系统不出错，赋初始值
            addViewModel.IsShowTableproperty = true;
            var result = _Add(addViewModel, new PosRefe { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos营业点增加);
            return result;
        }

        /// <summary>
        /// 创建一条默认的收银点，班次，市别信息
        /// </summary>
        private void CreatePosDate(string refId)
        {
            var posPosModel = new PosPos
            {
                Id = CurrentInfo.HotelId + "01",
                Hid = CurrentInfo.HotelId,
                Code = "01",
                Name = "收银点",
                Seqid = 1,
                Ename = "SY",
                ShiftId = CurrentInfo.HotelId + "01",
                Module = CurrentInfo.ModuleCode,
                Business = DateTime.Now
            };

            var posShift = new PosShift
            {
                Id = CurrentInfo.HotelId + "01",
                Hid = CurrentInfo.HotelId,
                Code = "01",
                Name = "班次",
                PosId = CurrentInfo.HotelId + "01",
                Module = CurrentInfo.ModuleCode,
                Stime = "00:00",
                Etime = "23:59"
            };
            var posShuffe = new PosShuffle
            {
                Id = CurrentInfo.HotelId + "01",
                Hid = CurrentInfo.HotelId,
                Code = "01",
                Cname = "市别",
                Refeid = refId,
                Stime = "00:00",
                Etime = "23:59",
                IsHideTab = 1,
                Module = CurrentInfo.ModuleCode
            };

            var posPosServiece = GetService<IPosPosService>();
            posPosServiece.Add(posPosModel);
            posPosServiece.AddDataChangeLog(OpLogType.Pos收银点增加);
            posPosServiece.Commit();

            var posShiftService = GetService<IPosShiftService>();
            posShiftService.Add(posShift);
            posShiftService.AddDataChangeLog(OpLogType.Pos班次增加);
            posShiftService.Commit();

            var posShuffeService = GetService<IPosShuffleService>();
            posShuffeService.Add(posShuffe);
            posShuffeService.AddDataChangeLog(OpLogType.Pos市别增加);
            posShuffeService.Commit();
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosRefeEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosRefeEditViewModel model)
        {
            var modelService = GetService<IPosRefeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (!string.IsNullOrEmpty(Request["OpenInfo"]))
            {
                model.OpenInfo = Request["OpenInfo"].ToString();
                if (model.OpenInfo.IndexOf("I", StringComparison.CurrentCultureIgnoreCase) > -1 && model.OpenInfo.IndexOf("J", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    return Json(JsonResultData.Failure("“自动虚拟台”和“手工指定台”只能选择一项！"));
                }
            }

            //开台信息、楼面台号显示内容为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["OpenInfo"] == null)
            {
                Request.Form.Add("OpenInfo", "");
            }
            if (Request["FloorShowData"] == null)
            {
                Request.Form.Add("FloorShowData", "");
            }

            if (!string.IsNullOrEmpty(Request["FloorShowData"]))
            {
                model.FloorShowData = Request["FloorShowData"].ToString();
                var floorShowDataList = model.FloorShowData.Split(',');
                if (floorShowDataList.Length > 2)
                {
                    return Json(JsonResultData.Failure("楼面台号显示内容只能选择两项！"));
                }
            }

            ActionResult result = _Edit(model, new PosRefe(), OpLogType.Pos营业点修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosRefeService>(), OpLogType.Pos营业点删除);
        }

        #endregion 批量删除

        #region 下拉数据绑定

        /// <summary>
        /// 获取指定模块下的营业厅列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosRefe()
        {
            var service = GetService<IPosRefeService>();
            var datas = service.GetRefe(CurrentInfo.HotelId).Where(m => m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定模块下的营业厅列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosRefeByModules()
        {
            var service = GetService<IPosRefeService>();
            var datas = service.GetRefeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定收银点、模块下的营业点列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosRefeByPos()
        {
            var service = GetService<IPosRefeService>();
            var datas = service.GetRefeByPos(CurrentInfo.HotelId, CurrentInfo.PosId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取楼面台号显示内容列表
        /// </summary>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public JsonResult ListPosRefFloorShowData()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List("71");
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListRefeByPosId(string posId)
        {
            var service = GetService<IPosRefeService>();
            var datas = service.GetRefeByPos(CurrentInfo.HotelId, string.IsNullOrEmpty(posId) ? CurrentInfo.PosId : posId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListPosBillPrint()
        {
            //获取账单自定义格式
            var service = GetService<IReportService>();
            var datas = service.GetStyleNames(CurrentInfo.HotelId, "PosBillPrint");
            var listItems = datas.Select(w => new SelectListItem { Value = w, Text = w }).ToList();
            //插入默认格式选项
            listItems.Insert(0, new SelectListItem { Value = "默认格式", Text = "默认格式" });
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉数据绑定

        #region 启用

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var refeService = GetService<IPosRefeService>();
            var reval = Json(refeService.BatchUpdateStatus(id, EntityStatus.启用));
            return reval;
        }

        #endregion 启用

        #region 禁用

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var refeService = GetService<IPosRefeService>();
            var reval = Json(refeService.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;
        }

        #endregion 禁用
    }
}