using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerFee;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Tools;
using NPOI.SS.UserModel;
using Gemstar.BSPMS.Common.Extensions;
using static Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers.PermanentRoomImportController;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    /// 业主费用记录
    /// </summary>
    [AuthPage("300022")]
    [BusinessType("业主费用记录")]
    public class RoomOwnerFeeController : BaseEditInWindowController<RoomOwnerFee, IRoomOwnerFeeService>
    {
        // GET: MarketingManage/RoomOwnerFee
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_RoomOwnerFee", "");
            return View();
        }
        #region 增加
        /// <summary>
        /// 增加业主费用记录
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            RoomOwnerRoomInfos rof = GetService<IRoomOwnerRoomInfosService>().List(CurrentInfo.HotelId).FirstOrDefault();
            ViewBag.ownername = getOwnerNamebyRoomid(rof.RoomId);
            return _Add(new RoomOwnerFeeAddViewModel()
            {

            });
        }

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult Add(RoomOwnerFeeAddViewModel addmodel)
        {
            var service = GetService<IRoomService>();
            if (service.Get(addmodel.roomId) == null)
            {
                return Json(JsonResultData.Failure("此房号不存在或已修改，请在业主房间委托中更换房号！"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(addmodel.itemId))
            {
                return Json(JsonResultData.Failure("费用项目不能为空！"), JsonRequestBehavior.AllowGet);
            }
            addmodel.roomNo = service.Get(addmodel.roomId).RoomNo;

            addmodel.preReadQty = GetService<IRoomOwnerFeeService>().getlastpreReadQty(CurrentInfo.HotelId, addmodel.roomNo, addmodel.itemId);
        
            decimal? preReadQty = GetService<IRoomOwnerFeeService>().getlastpreReadQty(CurrentInfo.HotelId, addmodel.roomNo, addmodel.itemId);//上次抄表数
            if (addmodel.amount == null)
            {
                if (addmodel.qty == null && addmodel.currentReadQty == null)
                {
                    return Json(JsonResultData.Failure("本次抄表数、数量、金额其中一个不能为空！"), JsonRequestBehavior.AllowGet);
                }
                decimal sl = decimal.Parse((addmodel.qty == null ? (addmodel.currentReadQty - (preReadQty == null ? 0 : preReadQty)) : addmodel.qty).ToString());
                decimal? jg = (addmodel.price == null ? GetService<IItemService>().Get(addmodel.itemId).Price : addmodel.price);
                if (jg == null)
                {
                    return Json(JsonResultData.Failure("单价不能为空！"), JsonRequestBehavior.AllowGet);
                }
                addmodel.amount = sl * jg;
                addmodel.qty = sl;
                addmodel.price = jg;
            }
            addmodel.Profileid = GetService<IRoomOwnerRoomInfosService>().getprofileidByRoomno(CurrentInfo.HotelId, addmodel.roomNo);
            return _Add(addmodel, new RoomOwnerFee
            {
                FeeId = Guid.NewGuid(),
                hid = CurrentInfo.HotelId,
                inputDate = DateTime.Parse(DateTime.Now.ToLongTimeString()),
                inputUser = CurrentInfo.UserName,
                isImport = false

            }, OpLogType.业主费用记录增加);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改业主房间委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns> 
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            RoomOwnerFee rof = GetService<IRoomOwnerFeeService>().Get(Guid.Parse(id));
            ViewBag.ownername = getOwnerNamebyRoomid(rof.roomId);
            return _Edit(Guid.Parse(id), new RoomOwnerFeeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(RoomOwnerFeeEditViewModel model)
        {
            return _Edit(model, new RoomOwnerFee() { }, OpLogType.业主费用记录修改);
        }
        #endregion


        #region 获取业主消费项目列表
        [AuthButton(AuthFlag.None)]
        public JsonResult getOwnerItem()
        {
            List<Item> OwnerfeeItem = GetService<IItemService>().getOwnerfeeItem(CurrentInfo.HotelId);//房间费用
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            for (int i = 0; i < OwnerfeeItem.Count(); i++)
            {
                list.Add(new SelectListItem() { Value = OwnerfeeItem[i].Id.ToString(), Text = OwnerfeeItem[i].Name });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 获取为业主房间的房号列表
        [AuthButton(AuthFlag.None)]
        public JsonResult getOwnerRoomNO()
        {
            List<RoomOwnerRoomInfos> RoomOwnerRoomInfo = GetService<IRoomOwnerRoomInfosService>().List(CurrentInfo.HotelId).OrderBy(w => w.RoomNo).ToList();//房间费用
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            for (int i = 0; i < RoomOwnerRoomInfo.Count(); i++)
            {
                list.Add(new SelectListItem() { Value = RoomOwnerRoomInfo[i].RoomId.ToString(), Text = RoomOwnerRoomInfo[i].RoomNo });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [AuthButton(AuthFlag.None)]
        public JsonResult getOwnername(string roomId)
        {
            string ownername = getOwnerNamebyRoomid(roomId);
            return Json(JsonResultData.Successed(ownername), JsonRequestBehavior.AllowGet);
        }
        #region 根据房间编号得到业主名称
        /// <summary>
        /// 根据房间编号得到业主名称
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public string getOwnerNamebyRoomid(string roomId)
        {
            return GetService<IMbrCardService>().Get(GetService<IRoomOwnerRoomInfosService>().getOwnernamebyRoomId(CurrentInfo.HotelId, roomId).ProfileId).GuestName;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IRoomOwnerFeeService>(), OpLogType.业主费用记录删除);
        }
        #endregion
        #region 上传文件 

        [AuthButton(AuthFlag.Add)]
        public ActionResult Import(string id)
        {
            //return PartialView("_Import");
            return View("_Import");
        }

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Import(IEnumerable<HttpPostedFileBase> files, string itemid, string feedate)
        {
            string message = "";
            try
            {
                bool isExcel = CheckFile(files, out message);
                if (isExcel)
                {
                    DataTable dt = HandleFile(files, out message);
                    ExcelToDS(dt, itemid, feedate, out message);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            //if (message.IndexOf("成功")>-1)
            //{
            //    return Json(JsonResultData.Successed("导入成功"));
            //}
            //else
            //{
            //    return Json(JsonResultData.Failure(message));
            //}
            ViewBag.message = message;
            return View("_Import");
        }

        /// <summary>
        /// 验证文件格式
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        private bool CheckFile(IEnumerable<HttpPostedFileBase> files, out string message)
        {
            message = "";
            if (files != null)
            {
                int count = files.Count();
                if (count <= 0)
                {
                    message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                    return false;
                }
                if (count != 1)
                {
                    //控制只能选一个EXCEL文件
                    message = EnumExtension.GetDescription(ImportStatus.OnlySelectOneFile);
                    return false;
                }

                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var extension = FileHelper.GetFileExtension(file.FileName, file.InputStream);
                        if (extension != FileExtension.XLS && extension != FileExtension.XLSX)
                        {
                            message = EnumExtension.GetDescription(ImportStatus.NotExcelFile);
                            break;
                        }
                    }
                    else
                    {
                        message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                    }
                }
            }
            else
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理文件
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="message">消息</param>
        private DataTable HandleFile(IEnumerable<HttpPostedFileBase> files, out string message)
        {
            message = "hahha";
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        IWorkbook workbook = null;
                        try
                        {
                            workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(file.InputStream);
                        }
                        catch
                        {
                            workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(file.InputStream);
                        }
                        if (workbook != null && workbook.NumberOfSheets > 0)
                        {
                            if (workbook.NumberOfSheets != 1)
                            {

                                workbook.Close();

                            }
                            else
                            {
                                var dt = HandleSheet(workbook.GetSheetAt(0), out message);
                                return dt;
                            }
                        }
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// 处理单个文件
        /// </summary>
        /// <param name="sheet">表格文件</param>
        /// <param name="importTableList">可导入列表</param>
        private DataTable HandleSheet(ISheet sheet, out string message)
        {

            DataTable dt = new DataTable();
            message = "";
            if (sheet == null)
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                return dt;
            }

            if (string.IsNullOrWhiteSpace(sheet.SheetName))
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNameIsNull);
                return dt;
            }

            if (sheet == null || sheet.LastRowNum < 1)
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNotValue);
                return dt;
            }
            //excel标题行
            int titlecolumns = sheet.GetRow(0).Cells.Count;
            int rowCount = sheet.LastRowNum;
            if (titlecolumns == 6)
            {
                if (sheet.GetRow(0).Cells[0].StringCellValue == "房号" && sheet.GetRow(0).Cells[1].StringCellValue == "本次抄表数" && sheet.GetRow(0).Cells[2].StringCellValue == "数量" && sheet.GetRow(0).Cells[3].StringCellValue == "单价" && sheet.GetRow(0).Cells[4].StringCellValue == "金额" && sheet.GetRow(0).Cells[5].StringCellValue == "备注")
                {
                    DataColumn Columnname = new DataColumn("房号", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    Columnname = new DataColumn("本次抄表数", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    Columnname = new DataColumn("数量", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    Columnname = new DataColumn("单价", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    Columnname = new DataColumn("金额", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    Columnname = new DataColumn("备注", System.Type.GetType("System.String"));
                    dt.Columns.Add(Columnname);
                    for (int i = 1; i < rowCount + 1; i++)
                    {
                        DataRow row = dt.NewRow();
                        for (int j = 0; j < titlecolumns; j++)
                        {
                            var aa = sheet.GetRow(i).GetCell(j);
                            try
                            {
                                row[j] = aa.StringCellValue;
                            }
                            catch
                            {
                                try
                                {
                                    row[j] = aa.NumericCellValue;
                                }
                                catch
                                {
                                    row[j] = "";
                                }
                            }
                            // = aa.StringCellValue;

                        }
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    message = "模板格式不正确！";
                }

            }
            else
            {
                message = "模板格式不正确！";
            }
            //for (int i = 0; i < rowCount; i++)
            //{

            //    for (int j = 0; j < titlecolumns.Count; j++)
            //    {

            //    }
            //}


            return dt;
        }

        #endregion
        /// <summary>
        /// 导入到数据库
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns> 
        public void ExcelToDS(DataTable dt, string itemid, string feedate, out string message)
        {
            decimal? jg = GetService<IItemService>().Get(itemid).Price;
            if (dt == null || dt.Columns.Count != 6 || !(dt.Columns.Contains("房号") && dt.Columns.Contains("金额") && dt.Columns.Contains("数量") && dt.Columns.Contains("本次抄表数") && dt.Columns.Contains("单价") && dt.Columns.Contains("备注")))
            {
                message = "导入失败，模板格式不正确！";
                return;
            }
            RoomOwnerFeeAddViewModel addmodel = new RoomOwnerFeeAddViewModel();
            string roomno = "";
            var roomserv = GetService<IRoomOwnerRoomInfosService>(); int a = 0; int b = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string roomid = roomserv.getRoomIdbyRoomno(CurrentInfo.HotelId, dt.Rows[i]["房号"].ToString());
                if (!string.IsNullOrEmpty(roomid))
                {
                    //如果有填写金额则优先按此金额导入；如果没有填写金额有填写数量（单价：优先按填入的，无填入时根据消费项目设置中的）则按表达式计算：金额=数量×单价； 如果没有填写数量单价金额有填写抄表数（未填写上次抄表数或单价时则根据消费项目设置生成）则按表达式计算：金额=（本次抄表数-上次抄表数）×单价。
                    addmodel = new RoomOwnerFeeAddViewModel();
                    decimal amounts = decimal.Parse(dt.Rows[i]["金额"].ToString() == "" ? "0" : dt.Rows[i]["金额"].ToString());
                    decimal? preReadQty = GetService<IRoomOwnerFeeService>().getlastpreReadQty(CurrentInfo.HotelId, addmodel.roomNo, addmodel.itemId);
                    decimal? currentReadQtys = decimal.Parse(dt.Rows[i]["本次抄表数"].ToString() == "" ? "0" : dt.Rows[i]["本次抄表数"].ToString());
                    decimal sl = decimal.Parse(dt.Rows[i]["数量"].ToString() == "" ? (currentReadQtys - preReadQty).ToString() : dt.Rows[i]["数量"].ToString());
                    if (dt.Rows[i]["金额"].ToString() == "")
                    {
                        if (dt.Rows[i]["单价"].ToString() != "")
                        {
                            amounts = sl * decimal.Parse(dt.Rows[i]["单价"].ToString());
                        }
                        else
                        {
                            if (jg != null)
                            {
                                amounts = sl * decimal.Parse(jg.ToString());
                            }
                        }
                    }
                    addmodel.amount = amounts;
                    addmodel.qty = sl;
                    addmodel.price = decimal.Parse(dt.Rows[i]["单价"].ToString() == "" ? "0" : dt.Rows[i]["单价"].ToString());
                    addmodel.currentReadQty = currentReadQtys;
                    addmodel.roomNo = dt.Rows[i]["房号"].ToString();
                    addmodel.Remark = dt.Rows[i]["备注"].ToString();
                    addmodel.itemId = itemid;
                    addmodel.FeeDate = DateTime.Parse(feedate);
                    addmodel.roomId = roomid;
                    addmodel.preReadQty = preReadQty;
                    addmodel.Profileid = GetService<IRoomOwnerRoomInfosService>().getprofileidByRoomno(CurrentInfo.HotelId, addmodel.roomNo);
                    _Add(addmodel, new RoomOwnerFee
                    {
                        FeeId = Guid.NewGuid(),
                        hid = CurrentInfo.HotelId,
                        inputDate = DateTime.Parse(DateTime.Now.ToLongTimeString()),
                        inputUser = CurrentInfo.UserName,
                        isImport = true
                    }, OpLogType.业主费用记录增加);
                    a++;
                }
                else
                {
                    roomno += dt.Rows[i]["房号"].ToString() + "、";
                    b++;
                }
            }
            if (roomno == "")
            {
                message = "导入成功！成功导入" + a + "条数据！";
                return;
            }
            else
            {
                message = "成功导入" + a + "条数据！导入失败" + b + "条数据。失败原因【" + roomno.Trim('、') + "】房号不存在业主房间委托！";
                return;
            }

        }

        /// <summary>
        /// 删除当天指定项目导入数据
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult DelCurImport(string[] owneritemid)
        {
            bool isok = false;
            if (owneritemid == null) { isok = GetService<IRoomOwnerFeeService>().delCurdayImport(CurrentInfo.HotelId, ""); }
            else
            {
                for (int i = 0; i < owneritemid.Length; i++)
                {
                    isok = GetService<IRoomOwnerFeeService>().delCurdayImport(CurrentInfo.HotelId, owneritemid[i]);
                }
            }
            if (isok)
            {
                return Json(JsonResultData.Successed("删除成功！"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonResultData.Failure("删除失败！"), JsonRequestBehavior.AllowGet);
            }
        }
    }
}