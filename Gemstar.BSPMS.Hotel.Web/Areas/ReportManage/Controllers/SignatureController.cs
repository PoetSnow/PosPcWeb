using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Text;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.ReportManage;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Common.Services;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using iTextSharp.text.pdf;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Controllers
{
    [NotAuth]
    public class SignatureController : Controller
    {
        // GET: ReportManage/Signature
        [AuthButton(AuthFlag.None)]
        public ActionResult WriteSignature(string par)
        {
            var data = par.Split('=');
            if (data.Length > 7)
            {
                var url = data[0];
                var sType = data[1];
                var regid = data[2];
                var roomNo = data[3];
                var hid = data[4];
                var userName = data[5];
                var width = data[6];
                ViewBag.pdfUrl = url;
                ViewBag.IsText = MvcApplication.IsTestEnv;
                ViewBag.sType = sType;
                ViewBag.regid = regid;
                ViewBag.roomNo = roomNo;
                ViewBag.hid = hid;
                userName = Server.UrlDecode(userName);
                ViewBag.userName = userName;
                ViewBag.width = width;
                ViewBag.signature = data[7];
            }
            return View();
        }
        [AuthButton(AuthFlag.None)]
        public ActionResult SvaeOcxFile(string sType, string pdfKey, string regid, string roomNo, string hid, string userName)
        {
            try
            {
                var file = Request.Files.Get(0).InputStream;
                Web.Controllers.QiniuController qin = new Web.Controllers.QiniuController();
                string key = "SignaturePdf" + hid + Guid.NewGuid();
                qin.UploadFile(file, key);
                string url = "http://res.gshis.com/" + key;

                var rep = GetDbEntity(hid, userName);
                var result = rep.InsertSignature(new SignatureLog
                {
                    Id = Guid.NewGuid(),
                    Hid = hid,
                    SDate = DateTime.Now,
                    SType = Byte.Parse(sType),
                    InputUser = userName,
                    Url = url,
                    Remark = "",
                    Regid = string.IsNullOrWhiteSpace(regid) ? "" : regid.Replace(hid, ""),
                    RoomNo = roomNo
                });
                if (result)
                {
                    var pdfUrl = "http://res.gshis.com/" + pdfKey;
                    qin.QiniuDelete(pdfUrl, true);
                }
                return Json(0);

            }
            catch (Exception ex)
            {

                return Json(ex);
            }

        }
        [AuthButton(AuthFlag.None)]
        public ActionResult LookSignature(string par, string code)
        {
            if (string.IsNullOrWhiteSpace(par))
            {
                var service = GetService<ICurrentInfo>();
                ViewBag.hid = service.HotelId;
                ViewBag.userName = service.UserName;
                ViewBag.signature = code;
            }
            else
            {
                var data = par.Split('=');
                if (data.Length > 3)
                {
                    ViewBag.sType = data[0];
                    ViewBag.hid = data[1];
                    var userName = Server.UrlDecode(data[2]);
                    ViewBag.userName = userName;
                    ViewBag.signature = data[3];
                }
            }
            return View();
        }
        public ActionResult PdfSignature(string url, int isPrint = 0)
        {

            var urls = new Uri(url);
            PdfReader reader = new PdfReader(urls);
            MemoryStream fs = new MemoryStream();
            PdfStamper stamper = new PdfStamper(reader, fs);
            stamper.Writer.ViewerPreferences = PdfWriter.HideWindowUI;
            stamper.Writer.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting | PdfWriter.AllowFillIn);
            stamper.Writer.CloseStream = false;
            //直接弹出打印不用点击打印按钮
            if (isPrint == 1)
            {
                PdfAction.JavaScript("myOnMessage();", stamper.Writer);
                stamper.Writer.AddJavaScript("this.print(true);function myOnMessage(aMessage) {app.alert('Test',2);} var msgHandlerObject = new Object();doc.onWillPrint = myOnMessage;this.hostContainer.messageHandler = msgHandlerObject;");
                StringBuilder script = new StringBuilder();
                script.Append("this.print({bUI: false,bSilent: true,bShrinkToFit: true});").Append("\r\nthis.closeDoc();");
                script.Append("var pp = this.getPrintParams();pp.printerName = '\\\\fpserver\\hp LaserJet 1010'; this.print(pp);");
                script.Append("this.print(flase);");
                stamper.Writer.AddJavaScript(script.ToString(), false);
            }
            stamper.Close();
            byte[] buffer = new byte[fs.Length];
            fs.Position = 0;
            fs.Read(buffer, 0, (int)fs.Length);
            Response.Clear();
            Response.AddHeader("Content-Length", fs.Length.ToString());
            Response.ContentType = "application/pdf";
            fs.Close();
            Response.BinaryWrite(buffer);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            return View();
        }
        private void EditPDF(string fpath)
        {

            //string path = fpath.Replace("\\", "/");
            var url = new Uri(fpath);
            PdfReader reader = new PdfReader(url);
            MemoryStream ms = new MemoryStream();
            PdfStamper stamper = new PdfStamper(reader, ms);
            stamper.Writer.ViewerPreferences = PdfWriter.HideWindowUI;
            stamper.Writer.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting | PdfWriter.AllowFillIn);
            stamper.Writer.CloseStream = false;
            //直接弹出打印不用点击打印按钮
            //PdfAction.JavaScript("myOnMessage();", stamper.Writer);
            //stamper.Writer.AddJavaScript("this.print(true);function myOnMessage(aMessage) {app.alert('Test',2);} var msgHandlerObject = new Object();doc.onWillPrint = myOnMessage;this.hostContainer.messageHandler = msgHandlerObject;");

            //StringBuilder script = new StringBuilder();
            //script.Append("this.print({bUI: false,bSilent: true,bShrinkToFit: true});").Append("\r\nthis.closeDoc();");
            //script.Append("var pp = this.getPrintParams();pp.printerName = '\\\\fpserver\\hp LaserJet 1010'; this.print(pp);");
            //script.Append("this.print(flase);");
            //stamper.Writer.AddJavaScript(script.ToString(),false);

            //PdfContentByte cb = stamper.GetOverContent(1);
            //cb.Circle(250, 250, 50);
            //cb.SetColorFill(iTextSharp.text.Color.RED);
            //cb.SetColorStroke(iTextSharp.text.Color.WHITE);
            //cb.FillStroke();
            stamper.Close();

            //ViewPdf(ms);


        }
        [AuthButton(AuthFlag.None)]
        public ActionResult GetLookSignature(string sType, string hid, string userName, string regid, string roomid, DateTime? timeBegin, DateTime? timeEnd, [DataSourceRequest] DataSourceRequest request)
        {
            var service = GetDbEntity(hid, userName);
            var data = service.GetSignatureLogList(sType, hid);
            if (!string.IsNullOrWhiteSpace(regid))
                data = data.Where(w => w.Regid == regid);
            if (!string.IsNullOrWhiteSpace(roomid))
                data = data.Where(w => w.RoomNo == roomid);
            if (timeBegin != null)
                data = data.Where(w => w.SDate >= timeBegin);
            if (timeEnd != null)
                data = data.Where(w => w.SDate <= timeEnd);
            return Json(data.ToList().ToDataSourceResult(request));
        }
        [AuthButton(AuthFlag.None)]
        public ActionResult DeletePdf(string id, string hid, string userName, string url)
        {
            Web.Controllers.QiniuController qin = new Web.Controllers.QiniuController();
            qin.QiniuDelete(url, true);
            var service = GetDbEntity(hid, userName);
            var data = service.DeleteSignature(id);
            return Json(new JsonResultData { Success = data });


        }
        public ReportService GetDbEntity(string hid, string userName)
        {
            var centerDb = new DbCommonContext(MvcApplication.GetCenterDBConnStr());
            var dbNotify = new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(centerDb, hid), hid, userName, Request);
            var rep = new ReportService(dbNotify);
            return rep;
        }
        public ActionResult GetIsSelectList()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "1", Text = "RC单" },
                   new SelectListItem() { Value = "2", Text = "结账单" },
                   new SelectListItem() { Value = "3", Text = "押金单" }
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        protected T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

    }
}