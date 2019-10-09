using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using Gemstar.BSPMS.Common.Tools;
using System.Data;
using NPOI.SS.UserModel;
using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 长包房费用导入
    /// </summary>
    [AuthPage("21040")]
    public class PermanentRoomImportController : BaseController
    {
        #region 导入
        /// <summary>
        /// 长包房费用导入页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthButton(AuthFlag.Query)]
        public ActionResult Import()
        {
            var resDetails = GetService<IPermanentRoomService>().GetAllPermanentRoomIOrder(CurrentInfo.HotelId);
            if (resDetails == null || resDetails.Count <= 0)
            {
                return Content("没有找到在住的长包房");
            }
            return View(resDetails);
        }
        /// <summary>
        /// 长包房费用导入excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Import(IEnumerable<HttpPostedFileBase> files)
        {
            string message = "";
            List<PermanentRoomInfo.PermanentRoomImportFolioPara> result = new List<PermanentRoomInfo.PermanentRoomImportFolioPara>();
            try
            {
                bool isExcel = CheckFile(files, out message);
                if (isExcel)
                {
                    HandleFile(files, out message, out result);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(JsonResultData.Successed(result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonResultData.Failure(message), JsonRequestBehavior.AllowGet);
            }
        }
        #region 状态消息
        public enum ImportStatus
        {
            /// <summary>
            /// 请选择要上传的EXCEL文件！
            /// </summary>
            [Description("请选择要上传的EXCEL文件！")]
            PlaseSelectFile,
            /// <summary>
            /// 只能选择一个EXCEL文件上传！
            /// </summary>
            [Description("只能选择一个EXCEL文件上传！")]
            OnlySelectOneFile,
            /// <summary>
            /// 此文件不是EXCEL格式的文件！
            /// </summary>
            [Description("此文件不是EXCEL格式的文件！")]
            NotExcelFile,
            /// <summary>
            /// 没有对应的功能来处理上传的EXCEL文件！
            /// </summary>
            [Description("没有对应的功能来处理上传的EXCEL文件！")]
            NotFunction,
            /// <summary>
            /// EXCEL文件中只能有一个工作表，请删除其他工作表！
            /// </summary>
            [Description("EXCEL文件中只能有一个工作表，请删除其他工作表！")]
            OnlyHasOneExcelTable,
            /// <summary>
            /// 工作表的名字不能为空！
            /// </summary>
            [Description("工作表的名字不能为空！")]
            SheetNameIsNull,
            /// <summary>
            /// 没有数据！
            /// </summary>
            [Description("没有数据！")]
            SheetNotValue,
            /// <summary>
            /// 没有标题行！
            /// </summary>
            [Description("没有标题行！")]
            NotTitleRow,
            /// <summary>
            /// 列数量不正确！
            /// </summary>
            [Description("列数量不正确！")]
            NotEqualColumnCount,
            /// <summary>
            /// 列名不正确！
            /// </summary>
            [Description("列名不正确！")]
            ErrorColumnName,
            /// <summary>
            /// 列的值不允许为空！
            /// </summary>
            [Description("列的值不允许为空！")]
            NotNullColumn,
        }
        #endregion
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
        private void HandleFile(IEnumerable<HttpPostedFileBase> files, out string message, out List<PermanentRoomInfo.PermanentRoomImportFolioPara> result)
        {
            message = "";
            result = new List<PermanentRoomInfo.PermanentRoomImportFolioPara>();
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
                                //控制在一个EXCEL文件中，只能有一个工作表
                                message = EnumExtension.GetDescription(ImportStatus.OnlyHasOneExcelTable);
                                if (workbook != null)
                                {
                                    workbook.Close();
                                }
                                return;
                            }
                            for (int i = 0; i < workbook.NumberOfSheets; i++)
                            {
                                HandleSheet(workbook.GetSheetAt(i), out message, out result);
                            }
                        }
                        else
                        {
                            message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                        }
                        if (workbook != null)
                        {
                            workbook.Close();
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
        }
        /// <summary>
        /// 处理单个文件
        /// </summary>
        /// <param name="sheet">表格文件</param>
        /// <param name="importTableList">可导入列表</param>
        private void HandleSheet(ISheet sheet, out string message, out List<PermanentRoomInfo.PermanentRoomImportFolioPara> result)
        {
            message = "";
            result = new List<PermanentRoomInfo.PermanentRoomImportFolioPara>();

            if (sheet == null)
            {
                message = EnumExtension.GetDescription(ImportStatus.PlaseSelectFile);
                return;
            }

            if (string.IsNullOrWhiteSpace(sheet.SheetName))
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNameIsNull);
                return;
            }

            if (sheet == null || sheet.LastRowNum < 1)
            {
                message = EnumExtension.GetDescription(ImportStatus.SheetNotValue);
                return;
            }

            //excel标题行
            var titleRow = sheet.GetRow(0);
            if (titleRow == null || titleRow.Cells == null || titleRow.Cells.Count <= 0)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotTitleRow);
                return;
            }

            List<string> columnNames = new List<string>();
            columnNames.Add("房号");
            columnNames.Add("上次抄表数");
            columnNames.Add("本次抄表数");
            columnNames.Add("数量");
            columnNames.Add("单价");
            columnNames.Add("金额");
            columnNames.Add("单号");
            columnNames.Add("备注");
            if (titleRow.Cells.Count != columnNames.Count)
            {
                message = EnumExtension.GetDescription(ImportStatus.NotEqualColumnCount) + "\n正确的列数量：" + columnNames.Count;
                return;
            }
            foreach (var item in columnNames)
            {
                if (!titleRow.Cells.Where(c => c.ToString() == item).Any())
                {
                    message = EnumExtension.GetDescription(ImportStatus.ErrorColumnName) + string.Format("\n列名[{0}]不存在！", item);
                    return;
                }
            }

            //获取excel数据
            int columCount = columnNames.Count;
            int rowCount = sheet.LastRowNum;
            for (int i = 1; i <= rowCount; i++)
            {
                var entity = new PermanentRoomInfo.PermanentRoomImportFolioPara();
                for (int j = 0; j < columCount; j++)
                {
                    #region 获取Value
                    try
                    {
                        string columnName = titleRow.GetCell(j).StringCellValue.ToLower();
                        string cellValue = sheet.GetRow(i).GetCell(j).ToString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (columnName == "房号")
                            {
                                entity.RoomNo = cellValue;
                            }
                            else if (columnName == "上次抄表数")
                            {
                                entity.LastTimeMeterReading = Convert.ToInt64(cellValue);
                            }
                            else if (columnName == "本次抄表数")
                            {
                                entity.ThisTimeMeterReading = Convert.ToInt64(cellValue);
                            }
                            else if (columnName == "数量")
                            {
                                entity.Quantity = Convert.ToInt32(cellValue);
                            }
                            else if (columnName == "单价")
                            {
                                entity.Price = Convert.ToDecimal(cellValue);
                            }
                            else if (columnName == "金额")
                            {
                                entity.AmountD = Convert.ToDecimal(cellValue);
                            }
                            else if (columnName == "单号")
                            {
                                entity.InvNo = cellValue;
                            }
                            else if (columnName == "备注")
                            {
                                entity.Remark = cellValue;
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
                result.Add(entity);
            }

            //验证excel数据
            if (result.Where(c => string.IsNullOrWhiteSpace(c.RoomNo)).Any())
            {
                message = "房号不能为空！";
                return;
            }

            var resDetails = GetService<IPermanentRoomService>().GetAllPermanentRoomIOrder(CurrentInfo.HotelId);
            if (resDetails == null || resDetails.Count <= 0)
            {
                message = "没有找到在住的长包房！";
                return;
            }

            if (result.Select(c => c.RoomNo).GroupBy(c => c.ToString()).Count() != result.Count)
            {
                message = "请去除重复的房号！";
                return;
            }

            string hid = CurrentInfo.HotelId;
            foreach (var item in result)
            {
                var resDetailEntity = resDetails.Where(c => c.Value == item.RoomNo).FirstOrDefault();
                if (resDetailEntity != null)
                {
                    item.Regid = resDetailEntity.Key;
                    item.ShortRegid = item.Regid.Substring(hid.Length);
                }
                else
                {
                    message += string.Format("房号：{0}，不是在住长包房！\n", item.RoomNo);
                }
            }

        }
        #endregion

        #region 获取上次抄表数
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult GetLastTimeMeterReading(List<KeyValuePairModel<string, string>> list, string action)
        {
            var result = GetService<IPermanentRoomService>().GetLastTimeMeterReading(CurrentInfo.HotelId, list, action);
            if (result != null && result.Count > 0)
            {
                return Json(JsonResultData.Successed(result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonResultData.Failure("没有找到上次抄表数！"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        /// 保存导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        public ActionResult SaveImport(string itemid, List<PermanentRoomInfo.PermanentRoomImportFolioPara> list)
        {
            //验证
            var itemEntity = GetService<IItemService>().Get(itemid);
            if (itemEntity == null || itemEntity.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure("请选择消费项目！"), JsonRequestBehavior.DenyGet);
            }

            var checkResult = GetService<IPermanentRoomService>().CheckImportPermanentRoomFolio(CurrentInfo, itemid, list);
            if (checkResult.Success == false)
            {
                return Json(checkResult, JsonRequestBehavior.DenyGet);
            }
            //保存
            bool isSuccess = true;
            string resultData = "";
            var service = GetService<Services.ResFolioManage.IResFolioService>();
            list = list.OrderBy(c => c.RoomNo).ToList();
            foreach (var item in list)
            {
                var addResult = service.AddFolioDebit(new Services.ResFolioManage.ResFolioDebitPara
                {
                    Hid = CurrentInfo.HotelId,
                    RegId = item.Regid,
                    ItemId = itemid,
                    Quantity = item.Quantity,
                    Amount = item.AmountD.Value,
                    InvNo = item.InvNo,
                    Remark = item.Remark + string.Format("[读数：{0}-{1}]", Convert.ToString(item.LastTimeMeterReading), Convert.ToString(item.ThisTimeMeterReading)),
                    UserName = CurrentInfo.UserName,
                    TransShift = CurrentInfo.ShiftId
                });
                if (addResult.Success)
                {
                    resultData += string.Format("账号：{0}，房号：{1}，成功\n", item.Regid.Substring(CurrentInfo.HotelId.Length), item.RoomNo);
                    service.AddResFolioLog(CurrentInfo.HotelId, CurrentInfo.UserName, Guid.Parse(addResult.Data), 2, Convert.ToString(item.LastTimeMeterReading), Convert.ToString(item.ThisTimeMeterReading), item.Regid, item.RoomNo, "", CurrentInfo.ShiftId);
                    AddOperationLog(Gemstar.BSPMS.Common.Services.Enums.OpLogType.入账,
                        string.Format("{9}　账号：{0}，房号：{8}，{7}：{1}，数量：{2}，金额：{3:F2}，单号：{4}，备注：{5}，班次：{6}",
                        item.Regid.Replace(CurrentInfo.HotelId, ""),
                        itemEntity.Name,
                        item.Quantity,
                        item.AmountD,
                        item.InvNo,
                        item.Remark,
                        CurrentInfo.ShiftName,
                        "消费项目",
                        item.RoomNo,
                        "入账-消费"), item.Regid);
                }
                else
                {
                    isSuccess = false;
                    resultData += string.Format("账号：{0}，房号：{1}，失败，原因：{2}\n", item.Regid.Substring(CurrentInfo.HotelId.Length), item.RoomNo, addResult.Data);
                }
            }

            return Json((isSuccess ? JsonResultData.Successed(resultData) : JsonResultData.Failure(resultData)), JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 删除导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Delete)]
        public ActionResult DeleteImport(string itemId)
        {
            var result = GetService<IPermanentRoomService>().DeleteCurrentDayImport(CurrentInfo.HotelId, CurrentInfo.ShiftId, itemId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 费用导入模板
        private string fileName = "长包房费用导入模板.xlsx";
        /// <summary>
        /// 获取导入文件列表
        /// </summary>
        private List<string> GetDownLoadFiles()
        {
            List<string> fileList = new List<string>();
            try
            {
                var path = Server.MapPath("~/Content/Template/");
                var list = System.IO.Directory.GetFiles(path, fileName, System.IO.SearchOption.TopDirectoryOnly);
                if (list != null && list.Length > 0)
                {
                    var count = list.Length;
                    for (int i = 0; i < count; i++)
                    {
                        list[i] = list[i].Replace(path, "");
                    }
                    fileList = list.ToList();
                }
            }
            catch { }
            return fileList;
        }
        /// <summary>
        /// 下载导入文件
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult DownLoadFile()
        {
            var list = GetDownLoadFiles();
            if (list != null && list.Contains(fileName))
            {
                var path = Server.MapPath("~/Content/Template/");
                return File((path + fileName), "application/octet-stream", Url.Encode(fileName));
            }
            return Content("文件不存在！");
        }
        #endregion
    }
}