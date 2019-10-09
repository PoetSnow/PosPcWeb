using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosShuffle;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos市别
    /// </summary>
    [AuthPage(ProductType.Pos, "p99021004")]
    public class BasicDataPosShuffleController : BaseEditInWindowController<PosShuffle, IPosShuffleService>
    {
        // GET: PosManage/BasicDataPosShuffle
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_shuffle", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ShuffleAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ShuffleAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;

            var modelService = GetService<IPosShuffleService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var dateBool = true;

                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addSTime = System.DateTime.Parse(date + " " + addViewModel.Stime);//添加的开始时间
                var addETime = System.DateTime.Parse(date + " " + addViewModel.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShuffleList(CurrentInfo.HotelId, addViewModel.Refeid, CurrentInfo.ModuleCode);
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
            ActionResult result = _Add(addViewModel, new PosShuffle { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos市别增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new ShuffleEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ShuffleEditViewModel model)
        {
            var modelService = GetService<IPosShuffleService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var dateBool = true;

                var date = System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day;
                var addSTime = System.DateTime.Parse(date + " " + model.Stime);//添加的开始时间
                var addETime = System.DateTime.Parse(date + " " + model.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShuffleList(CurrentInfo.HotelId, model.Refeid, CurrentInfo.ModuleCode);
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
            ActionResult result = _Edit(model, new PosShuffle(), OpLogType.Pos市别修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosShuffleService>(), OpLogType.Pos市别删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店下的市别
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosShuffle(string Refeid = "")
        {
            var service = GetService<IPosShuffleService>();
            var datas = new List<PosShuffle>();
            if (!string.IsNullOrEmpty(Refeid))
            {
                datas = service.GetPosShuffle(CurrentInfo.HotelId).Where(m => m.Refeid == Refeid).ToList();
            }

            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的市别
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosShuffleByModules()
        {
            var service = GetService<IPosShuffleService>();
            var datas = service.GetPosShuffleByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定营业点下的市别
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosShuffleByRefe(string refeid)
        {
            var service = GetService<IPosShuffleService>();
            var datas = service.GetPosShuffleList(CurrentInfo.HotelId, refeid, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
    
        [AuthButton(AuthFlag.None)]
        public JsonResult ListShuffle()
        {
            var service = GetService<IPosShuffleService>();
            var datas = service.GetPosShuffleByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            return Json(datas, JsonRequestBehavior.AllowGet);
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

            var service = GetService<IPosShuffleService>();
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

            var service = GetService<IPosShuffleService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion

    }
}