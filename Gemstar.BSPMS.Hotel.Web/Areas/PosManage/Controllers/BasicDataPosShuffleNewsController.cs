using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosShuffleNews;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// pos公用市别
    /// </summary>
    [AuthPage(ProductType.Pos, "p99021005")]
    public class BasicDataPosShuffleNewsController : BaseEditInWindowController<PosShuffleNews, IPosShuffleNewsService>
    {
        [AuthButton(AuthFlag.Query)]
        // GET: PosManage/BasicDataPosShuffleNews
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_shufflenews", "");
            return View();
        }

        #region 新增
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ShuffleNewsAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ShuffleNewsAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosShuffleNewsService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var dateBool = true;

                var date = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
                var addSTime = DateTime.Parse(date + " " + addViewModel.Stime);//添加的开始时间
                var addETime = DateTime.Parse(date + " " + addViewModel.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShuffleNewsList(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
                foreach (var row in data)
                {
                    var sTime = DateTime.Parse(date + " " + row.Stime);//开始时间 
                    var eTime = DateTime.Parse(date + " " + row.Etime);//结束时间 
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
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            ActionResult result = _Add(addViewModel, new PosShuffleNews { Id = id, Hid = CurrentInfo.HotelId }, OpLogType.Pos公用市别添加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new ShuffleNewsEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ShuffleNewsEditViewModel model)
        {
            var modelService = GetService<IPosShuffleNewsService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            try
            {
                var dateBool = true;

                var date = DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
                var addSTime = DateTime.Parse(date + " " + model.Stime);//添加的开始时间
                var addETime = DateTime.Parse(date + " " + model.Etime);//添加的结束时间
                //获取已有的班次列表
                var data = modelService.GetPosShuffleNewsList(CurrentInfo.HotelId,CurrentInfo.ModuleCode);
                foreach (var row in data)
                {
                    if (row.Id == model.Id)//如果当前修改的ID等于数据集合里面的ID 直接跳出本次循环
                    {
                        continue;
                    }
                    var sTime = DateTime.Parse(date + " " + row.Stime);//开始时间 
                    var eTime = DateTime.Parse(date + " " + row.Etime);//结束时间 
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
            catch (Exception ex)
            {

                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
            ActionResult result = _Edit(model, new PosShuffleNews(), OpLogType.Pos公用市别修改);
            return result;
        }
        #endregion

        #region 下拉框
        /// <summary>
        /// 公用市别列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ShuffleNewsList()
        {
            var service = GetService<IPosShuffleNewsService>();
            var datas = service.GetPosShuffleNewsList(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}