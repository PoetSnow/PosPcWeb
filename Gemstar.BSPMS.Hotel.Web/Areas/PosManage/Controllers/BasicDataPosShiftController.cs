using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosShift;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos班次维护
    /// </summary>
    [AuthPage(ProductType.Pos, "p99021003")]
    public class BasicDataPosShiftController : BaseEditInWindowController<PosShift, IPosShiftService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_shift", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ShiftAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ShiftAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosShiftService>();

            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Name);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var dateBool = true;

                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addSTime = System.DateTime.Parse(date + " " + addViewModel.Stime);//添加的开始时间
                var addETime = System.DateTime.Parse(date + " " + addViewModel.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShiftList(CurrentInfo.HotelId, addViewModel.PosId, CurrentInfo.ModuleCode);
                foreach (var row in data)
                {
                    var sTime = System.DateTime.Parse(date + " " + row.Stime);//开始时间 
                    var eTime = System.DateTime.Parse(date + " " + row.Etime);//结束时间 
                    if ((addSTime >= sTime && addSTime <= eTime) || (addETime >= sTime && addETime <= eTime))
                    {
                        dateBool = false;
                    }
                }
                if (!dateBool)
                {
                    return Json(JsonResultData.Failure("添加的时间有交叉！！"));
                }
            }
            catch (System.Exception ex)
            {

                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            ActionResult result = _Add(addViewModel, new PosShift { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos班次增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new ShiftEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ShiftEditViewModel model)
        {
            var modelService = GetService<IPosShiftService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Name, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {

                var dateBool = true;
                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addSTime = System.DateTime.Parse(date + " " + model.Stime);//添加的开始时间
                var addETime = System.DateTime.Parse(date + " " + model.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShiftList(CurrentInfo.HotelId, model.PosId, CurrentInfo.ModuleCode);
                foreach (var row in data)
                {
                    if (row.Id == model.Id)//如果当前修改的ID等于数据集合里面的ID 直接跳出本次循环
                    {
                        continue;
                    }
                    var sTime = System.DateTime.Parse(date + " " + row.Stime);//开始时间 
                    var eTime = System.DateTime.Parse(date + " " + row.Etime);//结束时间 
                    if ((addSTime >= sTime && addSTime <= eTime) || (addETime >= sTime && addETime <= eTime))
                    {
                        dateBool = false;
                    }
                }
                if (!dateBool)
                {
                    return Json(JsonResultData.Failure("添加的时间有交叉！！"));
                }
            }
            catch (System.Exception ex)
            {

                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            var newModle = new PosShift();
            model.Hid = CurrentInfo.HotelId;
            ActionResult result = _Edit(model, newModle, OpLogType.Pos班次修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosShiftService>(), OpLogType.Pos班次删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店、收银点、模块下的班次列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosShift()
        {
            var service = GetService<IPosShiftService>();
            var datas = service.GetPosShiftList(CurrentInfo.HotelId, CurrentInfo.PosId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
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

            var service = GetService<IPosShiftService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
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

            var service = GetService<IPosShiftService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion

    }
}