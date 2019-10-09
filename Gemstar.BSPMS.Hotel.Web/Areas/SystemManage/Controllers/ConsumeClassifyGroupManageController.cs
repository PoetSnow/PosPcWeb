using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CodelistGroupManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{   
    /// <summary>
    /// 消费分类管理
    /// </summary>
    [AuthPage("99080")]
    [AuthPage(ProductType.Member, "m99020")]
    [BusinessType("消费类型")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeConsume)]
    public class ConsumeClassifyGroupManageController : BaseEditInWindowController<CodeList, ICodeListService>
    {

        public string _dcFlag = "D";
        private string typeCode = "02";//消费类别代码
        private string typeCodeName = "消费类别";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_pmsCodeList_GrpDistrib", "@h99typeCode=" + typeCode + "&@s23分店=" + CurrentInfo.HotelId, "gridConsumeClassify");
            return View();
        }
        #endregion
        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string typeCode)
        {
            return _AddGroup(new CodeListGroupAddModel() { TypeCode = typeCode }, M_V_BasicDataType.BasicDataCodeConsume, "_Add");
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
                throw new Exception(string.Format("消费类型[{0}]已存在，请修改。", CodeListAddViewModel.Code));
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

            }, OpLogType.消费类别增加);
        }
        #endregion

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(int id)
        {
            var originEntity = GetService<ICodeListService>().Get(id);
            var editResult = _CanEdit(originEntity, M_V_BasicDataType.BasicDataCodeConsume);
            if (editResult != null)
            {
                return editResult;
            }
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;
            return _EditGroup(id, new CodeListGroupEditModel(), M_V_BasicDataType.BasicDataCodeConsume, "_Edit");
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
                return Json(JsonResultData.Failure("消费类型代码已存在，请重新输入。"));
            }
            if (origi.Id == upd.Id && origi.Code != upd.Code)
            {
                List<Item> lists = itemserv.GetCodeListbyitemTypeid(hid, origi.Id, _dcFlag);
                if (lists.Count > 0)
                {
                    return Json(JsonResultData.Failure("有子项不可修改消费代码"));
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
                    return Json(JsonResultData.Failure("系统保留消费类型的代码不允许修改！"));
                }
            } 
         
            upd.Id = upd.Hid + typeCode + upd.Code;
            return _EditGroup(upd, origi, OpLogType.消费类别修改);
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDeleteGroup(id, EntityKeyDataType.Int, GetService<ICodeListService>(), OpLogType.消费类别删除);
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