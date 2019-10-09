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
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 楼层管理
    /// </summary>
    [AuthPage("99006")]
    public class FloorManageController : BaseEditIncellController<CodeList, ICodeListService>
    {
        private string typeCode = "06";
        private string typeCodeName = "房间楼层";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                //集团分发型资料，列表需要不同，集团需要显示分店名称，并且查询条件中需要有分店可以选择
                SetCommonQueryValues("up_list_pmsCodeList_GrpDistrib", "@h99typeCode=" + typeCode, "gridFloor"); 
            }
            else
            {
                SetCommonQueryValues("up_list_pmsCodeList", "@h99typeCode=" + typeCode, "gridFloor");

            }
             return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> addVersions)
        {
            var hid = CurrentInfo.HotelId;
            return _Add(request, addVersions, w =>
            {
                w.Hid = hid;
                w.TypeCode = typeCode;
                w.TypeName = typeCodeName;
                w.Id = hid + typeCode + w.Name;
                w.Code = w.Name;
                w.Status = EntityStatus.启用;

                if (string.IsNullOrWhiteSpace(w.Name))
                {
                    throw new Exception("请填写楼层名");
                }
                if (GetService<ICodeListService>().IsExists(hid, typeCode, w.Name))
                {
                    throw new Exception(string.Format("楼层名[{0}]已存在，请修改。", w.Name));
                }
            }, OpLogType.楼层增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CodeList> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<CodeList> originVersions)
        {
            //因为在房间管理里有2个kendoGridOriginModelValues ，后加载房间，会导致originVersions取得值是房间列表roomList
            originVersions = GetService<ICodeListService>().GetFloorType(CurrentInfo.HotelId);
            var hid = CurrentInfo.HotelId;
           
            return _Update(request, updatedVersions, originVersions, (list, u) =>
            {
                var entity = list.SingleOrDefault(w => w.Id == u.Id && w.Hid == CurrentInfo.HotelId && w.TypeCode == typeCode);
                u.Code = u.Name;
                u.Name2 = "";
                u.Name3 = "";
                u.Name4 = "";
                //if (GetService<ICodeListService>().IsExists(hid, typeCode, u.Name))
                //{
                //    throw new Exception(string.Format("楼层名[{0}]已存在，请修改。", name));
                //}
                if (entity == null || string.IsNullOrWhiteSpace(CurrentInfo.HotelId))
                    throw new Exception("错误信息，请关闭后重试");

                var _codeListService = GetService<ICodeListService>();
                if (entity.Code != u.Code && _codeListService.IsExists(CurrentInfo.HotelId, typeCode, u.Name))
                { 
                    throw new Exception(string.Format("楼层代码已存在，请重新输入。", u.Name));
                }
                if (entity.Code != u.Code && GetService<IRoomService>().IsExistsFloor(hid, u.Id))
                {
                    throw new Exception(string.Format("[{0}]，此楼层已有房间，不能修改楼层名。", u.Name));
                }
                return entity;
            },
            OpLogType.楼层修改
            , w => { w.Id = string.Format("{0}{1}{2}", w.Hid, w.TypeCode, w.Code); }
            );
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ICodeListService>(), OpLogType.楼层删除);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 获取楼层
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetFloorList()
        {
            return Json(GetService<ICodeListService>().List(CurrentInfo.HotelId, typeCode), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
