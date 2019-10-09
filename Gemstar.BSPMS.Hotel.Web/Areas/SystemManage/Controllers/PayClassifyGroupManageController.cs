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
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CommonCodeManage;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CodelistGroupManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 付款分类管理
    /// </summary>
    [AuthPage("99060")]
    [AuthPage(ProductType.Member, "m99025")]
    [BusinessType("付款类型")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodePayway)]
    public class PayClassifyGroupManageController : BaseEditInWindowController<CodeList, ICodeListService>
    {
        public string _dcFlag = "C";
        private string typeCode = "03";//付款类别代码
        private string typeCodeName = "付款类别";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_pmsCodeList_GrpDistrib", "@h99typeCode=" + typeCode + "&@s23分店=" + CurrentInfo.HotelId, "gridPayClassify");
            return View();
        }
        #endregion
        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string typeCode)
        {
            return _AddGroup(new CodeListGroupAddModel() { TypeCode = typeCode }, M_V_BasicDataType.BasicDataCodePayway, "_Add");
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CodeListGroupAddModel CodeListAddViewModel)
        {

            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            if (GetService<ICodeListService>().IsExists(hid, typeCode, CodeListAddViewModel.Code))
            {
                throw new Exception(string.Format("付款类别[{0}]已存在，请修改。", CodeListAddViewModel.Code));
            }
            if (CodeListAddViewModel.Code.Substring(0, 1) == "0")
            {
                throw new Exception("自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！");
            }
            return _AddGroup(CodeListAddViewModel, new CodeList
            {
                Hid = hid,
                TypeCode = typeCode,
                TypeName = typeCodeName,
                Id = hid + typeCode + CodeListAddViewModel.Code,
                Status = EntityStatus.启用,
                DataSource = BasicDataDataSource.Added.Code

            }, OpLogType.付款类别增加);
        }
        #endregion

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(int id)
        {
            var originEntity = GetService<ICodeListService>().Get(id);
            var editResult = _CanEdit(originEntity, M_V_BasicDataType.BasicDataCodePayway);
            if (editResult != null)
            {
                return editResult;
            }
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;
            return _EditGroup(id, new CodeListGroupEditModel(), M_V_BasicDataType.BasicDataCodePayway, "_Edit");
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(CodeListGroupEditModel upd)
        { 
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            //没用下级时才能修改付款code
            var itemserv = GetService<IItemService>();
            var _codeListService = GetService<ICodeListService>();
            var origi = _codeListService.Get(upd.Pk);
            if (origi.Id == upd.Id && origi.Code != upd.Code && _codeListService.IsExists(CurrentInfo.HotelId, typeCode, upd.Code))
            {
                return Json(JsonResultData.Failure("付款类别代码已存在，请重新输入。"));
            }
            if (origi.Id == upd.Id && origi.Code != upd.Code)
            {
                List<Item> lists = itemserv.GetCodeListbyitemTypeid(hid, origi.Id, _dcFlag);
                if (lists.Count > 0)
                {
                    return Json(JsonResultData.Failure("有子项不可修改付款类别代码"));
                }
                if (upd.Code.Substring(0, 1) == "0")
                {
                    return Json(JsonResultData.Failure("自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！"));
                }
            }
            List<V_codeListReserve> val = itemserv.IsexistV_codeListReserve(origi.Code, typeCode);
            if (origi.Id == upd.Id && val.Count > 0)
            {
                if (origi.Code != upd.Code)
                {
                    return Json(JsonResultData.Failure("系统保留付款类型的代码不允许修改！"));
                }
            }
            upd.Id = upd.Hid + typeCode + upd.Code;
            return _EditGroup(upd, origi, OpLogType.付款类别修改);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDeleteGroup(id, EntityKeyDataType.Int, GetService<ICodeListService>(), OpLogType.付款类别删除);
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
                List<Item> list = itemserv.GetCodeListbyitemTypeid(hid, cl.Id, _dcFlag);
                List<V_codeListReserve> val = itemserv.IsexistV_codeListReserve(cl.Code, typeCode);
                if (list.Count > 0)
                {
                    return Json(JsonResultData.Failure("有子项不可删除！"));
                }
                if (val.Count > 0)
                {
                    return Json(JsonResultData.Failure("系统保留项目不可删除！"));

                }
            }
            return Json(JsonResultData.Successed("可以删除！"));
        }
        #endregion
    }
}