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
using Gemstar.BSPMS.Common.Services;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 消费分类管理
    /// </summary>
    [AuthPage("99080")]
    [AuthPage(ProductType.Member, "m99020")]
    public class ConsumeClassifyManageController : BaseEditIncellController<CodeList, ICodeListService>
    {
        private string typeCode = "02";//消费类别代码
        private string typeCodeName = "消费类别";
        public string comfirmtext = "集团分发型基础资料，被集团设置为分店不允许此操作。";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_pmsCodeList", "@h99typeCode=" + typeCode, "gridConsumeClassify");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> addVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;

            return _Add(request, addVersions, w =>
            {
                w.Hid = hid;
                w.TypeCode = typeCode;
                w.TypeName = typeCodeName;
                w.Id = hid + typeCode + w.Code;
                w.Status = EntityStatus.启用;
                w.DataSource = BasicDataDataSource.Added.Code;
                if (GetService<ICodeListService>().IsExists(hid, typeCode, w.Code))
                {
                    throw new Exception(string.Format("消费类型[{0}]已存在，请修改。", w.Code));
                }
                if (w.Code.Substring(0, 1) == "0")
                {
                    throw new Exception("自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！");
                }
            }, OpLogType.消费类别增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<CodeList> originVersions)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            //没用下级时才能修改付款code
            var itemserv = GetService<IItemService>();
            var _codeListService = GetService<ICodeListService>();
            foreach (var origi in originVersions)
            {
                foreach (var upd in updatedVersions)
                {
                    if (origi.Id == upd.Id && !ishotelCanDo("update", "") && origi.DataSource == BasicDataDataSource.Copyed.Code)
                    {
                        ModelState.AddModelError("code", "【消费类别名称：" + origi.Name + "】为" + comfirmtext);
                        return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                    }
                    if (origi.Id == upd.Id && origi.Code != upd.Code && _codeListService.IsExists(CurrentInfo.HotelId, typeCode, upd.Code))
                    {
                        ModelState.AddModelError("Code", "消费类型代码已存在，请重新输入。");
                        return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                    }

                    if (origi.Id == upd.Id && origi.Code != upd.Code)
                    {
                        List<Item> lists = itemserv.GetCodeListbyitemTypeid(hid, origi.Id, "D");
                        if (lists.Count > 0)
                        {
                            ModelState.AddModelError("Code", "有子项不可修改消费代码！");
                            return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                        }
                        if (upd.Code.Substring(0, 1) == "0")
                        {
                            ModelState.AddModelError("Code", "自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！");
                            return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                        }
                    }
                    List<V_codeListReserve> val = itemserv.IsexistV_codeListReserve(origi.Code, typeCode);
                    if (origi.Id == upd.Id && val.Count > 0)
                    {
                        if (origi.Code != upd.Code)
                        {
                            ModelState.AddModelError("Code", "系统保留消费类型的代码不允许修改！");
                            return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }
            return _Update(request, updatedVersions, originVersions, (list, u) =>
        {
            var entity = list.SingleOrDefault(w => w.Id == u.Id);
            return entity;
        }
        , OpLogType.消费类别修改
        , w => { w.Id = string.Format("{0}{1}{2}", w.Hid, w.TypeCode, w.Code); }
        );
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ICodeListService>(), OpLogType.消费类别删除);
        }
        #endregion   
        #region 删除
        /// <summary>
        /// 删除付款类型的时候判断有没有存在他下面的付款方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var itemserv = GetService<IItemService>();
            var arr = id[0].Split(',');
            var co = GetService<ICodeListService>();
            CodeList cl = new CodeList();
            for (int i = 0; i < arr.Length; i++)
            {
                cl = co.Get(int.Parse(arr[i]));
                List<Item> list = itemserv.GetCodeListbyitemTypeid(hid, cl.Id, "D");
                List<V_codeListReserve> val = itemserv.IsexistV_codeListReserve(cl.Code, typeCode);
                if (list.Count > 0)
                {
                    return Json(JsonResultData.Failure("有子项不可删除！"));
                }
                if (val.Count > 0)
                {
                    return Json(JsonResultData.Failure("系统保留项目不可删除！"));
                }
                if (!ishotelCanDo("del", id[i]))
                {
                    return Json(JsonResultData.Failure(comfirmtext));
                }
            }
            return Json(JsonResultData.Successed("可以删除！"));
        }
        #endregion
        #region 分店的操作判断
        /// <summary>
        /// 判断分店是否可进行操作
        /// </summary>
        /// <param name="opType">操作类型</param>
        /// <param name="id">要操作的数据id</param>
        /// <returns></returns>
        public bool ishotelCanDo(string opType, string id)
        {
            bool retval = true;
            if (CurrentInfo.IsHotelInGroup)
            {
                var resortControl = GetService<IBasicDataResortControlService>().GetResortControl(M_V_BasicDataType.BasicDataCodeConsume, CurrentInfo.GroupId);
                if (resortControl != null)
                {
                    if (opType == "add" || opType == "del")
                    {
                        retval = resortControl.ResortCanAdd;
                    }
                    if (opType == "update")
                    {
                        retval = resortControl.ResortCanUpdate;
                    }
                    if (opType == "disable")
                    {
                        retval = resortControl.ResortCanDisable;
                    }
                }
                if (id == "") { return retval; }
                var CodeListSer = GetService<ICodeListService>();
                CodeList cur = CodeListSer.Get(int.Parse(id));
                if (retval == false && cur.DataSource == BasicDataDataSource.Copyed.Code)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}