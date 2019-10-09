using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    [AuthPage(ProductType.Pos, "p50021")]
    public class VoucherListController : BaseEditIncellController<WhVoucher, IPosWhVoucherService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            ViewBag.hid = CurrentInfo.HotelId;
            SetCommonQueryValues("up_pos_voucher_query", "");
            return View();
        }

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WhVoucher> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<WhVoucher> originVersions)
        {
            string hid = CurrentInfo.HotelId;
            return _Update(request, updatedVersions, originVersions, (list, u) =>
            {
                var entity = list.SingleOrDefault(w => w.Voucherid == u.Voucherid);
                if (entity == null)
                {
                    throw new Exception("错误信息，请关闭后重试");
                }
                return entity;
            }
            , OpLogType.凭证列表);
        }
        #endregion

        [AuthButton(AuthFlag.Query)]
        public ActionResult Detail(int id, string vouchertype, string varcharno, string voucherDate, string remark)
        {
            ViewBag.Id = id;
            ViewBag.vouchertype = vouchertype;
            ViewBag.varcharno = varcharno;
            ViewBag.voucherDate = voucherDate;
            ViewBag.remark = remark;
            return PartialView("_Detail");
        }

        [AuthButton(AuthFlag.Query)]
        public ActionResult List(string id, [DataSourceRequest]DataSourceRequest request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            IEnumerable<WhVoucherdetail> List = GetService<IPosWhVoucherdetailService>().GetList(CurrentInfo.HotelId, id);
            return Json(List.ToDataSourceResult(request));
        }

        [AuthButton(AuthFlag.Add)]
        public string ADD(string beginDate, string endDate)
        {
            DbHotelPmsContext db = GetHotelDb(CurrentInfo.HotelId);
            var sql = "exec up_pos_Voucher_create @hid='" + CurrentInfo.HotelId + "',@beginDate='" + beginDate + "',@endDate='" + endDate + "',@operator='" + CurrentInfo.UserName + "'";
            var reset = db.Database.SqlQuery<int>(sql).ToList();
            if (reset.Count == 1)
                return reset[0] + "";
            else
                return "-1";//添加失败
        }

        [AuthButton(AuthFlag.Delete)]
        public string Del(string ids)
        {
            var id = ids.Split(',');
            List<int> reset = new List<int>();
            for (int i = 0; i < id.Length; i++)
            {
                if (!string.IsNullOrEmpty(id[i]))
                {
                    GetService<IPosWhVoucherdetailService>().Del(CurrentInfo.HotelId, id[i]);
                }
            }
            if (reset.Count == 1)
                return reset[0] + "";
            else
                return "-1";//添加失败
        }


        #region 导出
        [AuthButton(AuthFlag.Export)]
        public ActionResult Export(string id)
        {
            var v = GetService<IPmsParaService>();
            var localfileurl = v.GetValue(CurrentInfo.HotelId, "VoucherFileAddress");
            string filePath = "";
            try
            {
                filePath = Server.MapPath(@"~/bin/凭证列表导出模板.xls");
                if (!Download(localfileurl, filePath))//下载模板
                {
                    return Json(new JsonResultData { Success = false, Data = "请先上传模板" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new JsonResultData { Success = false, Data = "请先上传模板" }, JsonRequestBehavior.AllowGet);
            }


            DbHotelPmsContext db = GetHotelDb(CurrentInfo.HotelId);
            SqlParameter para = new SqlParameter() { ParameterName = "@type", DbType = DbType.String, Size = 500 }; para.Direction = ParameterDirection.Output;//这个必须加，sql语句中的output也要加 获取输出参数的值
            SqlParameter pageIndex = new SqlParameter() { ParameterName = "@pageIndex", DbType = DbType.String, Size = 500 }; pageIndex.Direction = ParameterDirection.Output;
            SqlParameter dataStartRow = new SqlParameter() { ParameterName = "@dataStartRow", DbType = DbType.String, Size = 500 }; dataStartRow.Direction = ParameterDirection.Output;

            SqlParameter[] param = {
                new SqlParameter() {ParameterName = "@hid", Value= CurrentInfo.HotelId},
                new SqlParameter() {ParameterName = "@voucherid", Value= id},
                para,
                pageIndex,
                dataStartRow
            };
            db.Database.ExecuteSqlCommand(@"exec [up_Voucher_export] @hid ,  @voucherid ,@type output ,@pageIndex output ,@dataStartRow output", param);

            try
            {
                //去数据库里获取数据
                var queryService = GetService<ICommonQueryService>();
                var procedure = "up_voucher_export";
                Dictionary<string, string> paraValues = new Dictionary<string, string>();
                paraValues.Add("@hid", CurrentInfo.HotelId);
                paraValues.Add("@voucherid", id);
                var dt = queryService.ExecuteQuery(procedure, paraValues);
                //根据输出参数判断 导出格式
                if (para.Value.ToString() == "xml")
                {
                    var sendid = dt.Rows[0][0] + "";
                    var TransXml = db.Database.SqlQuery<string>(@"select TransXml from  crsSendInfo  where sendid='" + sendid + "'").ToList();
                    StringWriter sw = new StringWriter();
                    var txt = TransXml[0] + "";
                    sw.WriteLine(txt.Trim());
                    sw.Close();
                    var name = string.Format("凭证列表导出{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
                    Response.ContentType = "text/plain";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    Response.Write(sw);
                    Response.End();
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                int page = int.Parse(pageIndex.Value.ToString());
                int row = int.Parse(dataStartRow.Value.ToString());
                ExportToExcelByTemplate(filePath, dt, page, row);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        //下载文件
        public bool Download(string url, string localfile)
        {
            bool flag = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            writeStream = new FileStream(localfile, FileMode.Create);// 文件不保存创建一个文件
            startPosition = 0;
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);// 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int)startPosition);// 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }


                Stream readStream = myRequest.GetResponse().GetResponseStream();// 向服务器请求,获得服务器的回应数据流


                byte[] btArray = new byte[512];// 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length);// 向远程文件读第一次

                while (contentSize > 0)// 如果读取长度大于零则继续读
                {
                    writeStream.Write(btArray, 0, contentSize);// 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length);// 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                flag = true;        //返回true下载成功
            }
            catch (Exception)
            {
                writeStream.Close();
                flag = false;       //返回false下载失败
            }

            return flag;
        }


        //编辑xls
        public IWorkbook ExportToExcelByTemplate(string filePath, DataTable dt, int page, int row)
        {
            try
            {
                #region 打开Excel表格模板，并初始化到NPOI对象中
                IWorkbook wk = null;
                if (!System.IO.File.Exists(filePath))
                {
                    //Windows.MessageBox(Page, "导出失败：课程表模板不存在！", MessageType.Normal);
                    return wk;
                }
                string extension = System.IO.Path.GetExtension(filePath);
                FileStream fs = System.IO.File.OpenRead(filePath);
                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                }
                fs.Close();
                #endregion

                #region 数据处理
                //1.读取Excel表格中的第一张Sheet表
                ISheet sheet = wk.GetSheetAt(page);
                //第二行开始填充单元格中的数据
                int index = row;
                foreach (DataRow item in dt.Rows)
                {
                    int len = item.ItemArray.Length;
                    IRow rowdetails = sheet.CreateRow(index);
                    for (int i = 0; i < len; i++)
                    {
                        rowdetails.CreateCell(i).SetCellValue(item[i].ToString());
                    }
                    index++;
                }

                #endregion

                #region 表格导出
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode("凭证列表导出", System.Text.Encoding.UTF8)));
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                return wk;
                #endregion
            }
            catch (Exception ex)
            {
                IWorkbook wk = null;
                return wk;
            }
        }



        #endregion
    }
}