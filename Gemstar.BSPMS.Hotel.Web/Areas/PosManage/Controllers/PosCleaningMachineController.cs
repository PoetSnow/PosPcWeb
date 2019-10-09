using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos清机
    /// </summary>
    [AuthPage(ProductType.Pos, "p200011")]
    public class PosCleaningMachineController : BaseController
    {
        [AuthButton(AuthFlag.None)]
        public PartialViewResult Index()
        {
            var server = GetService<IPosPosService>();
            var model = server.GetCleaningMachine(CurrentInfo.HotelId, CurrentInfo.PosId);
            ViewBag.Version = CurrentVersion;
            return PartialView(model);
        }

        /// <summary>
        /// 清机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult UpdateBusiness(up_pos_query_cleaningMachineResult model)
        {
            try
            {
                var posPosService = GetService<IPosPosService>();

                var pos = posPosService.Get(CurrentInfo.PosId);
                //清机
                posPosService.CleaningMachineBusiness(CurrentInfo.HotelId, CurrentInfo.PosId);
                AddOperationLog(OpLogType.Pos清机, "收银点：" + model.PosName + "，营业日：" + model.Business + " => " + model.NextBusiness + "，清机时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ExpendItemByBusiDate(pos.Business);
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        /// <summary>
        /// 调用接口生成仓库结转耗用单
        /// </summary>
        private void ExpendItemByBusiDate(DateTime? Business)
        {
            var billCostService = GetService<IPosBillCostService>();
            var list = billCostService.GetPosBillCostByProc(CurrentInfo.HotelId, CurrentInfo.PosId, Business, CurrentInfo.ModuleCode);
            var DepotService = GetService<IPosDepotService>();
            foreach (IGrouping<string, PosBillCost> group in list.GroupBy(x => x.WhCode))
            {
                var depot = DepotService.GetPosDepotList(CurrentInfo.HotelId, CurrentInfo.ModuleCode).Where(w => w.Id == group.Key).FirstOrDefault();
                if (depot != null)
                {
                    PostDataSet postData = new PostDataSet();
                    postData.ResortId = CurrentInfo.HotelId;
                    postData.DepotNo = depot.Code;
                    postData.Data = new List<PostDataBill>();
                    foreach (PosBillCost item in group)
                    {
                        if (postData.Data.Where(t => t.TransNo == item.WhBillNo).ToList().Count() > 0)
                        {
                            var oldquantity = postData.Data.Where(t => t.TransNo == item.WhBillNo).FirstOrDefault().Quantity;
                            var newquantity = item.Quantity + decimal.Parse(oldquantity);
                            postData.Data.Where(t => t.TransNo == item.WhBillNo).FirstOrDefault().Quantity = newquantity.ToString();
                        }
                        else
                        {
                            var postDataBill = new PostDataBill()
                            {
                                BusinessDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.BillBsnsDate),
                                TransNo = item.WhBillNo,
                                ItemCode = item.OutCodeNo,
                                Quantity = item.Quantity.ToString(),
                            };
                            postData.Data.Add(postDataBill);
                        }
                    }
                    if (PostDataByBusiDate(postData))
                    {
                        foreach (var item in list.Where(w => w.WhCode == group.Key))
                        {
                            item.IsBuildBill = true;
                            billCostService.Update(item, new PosBillCost());

                            billCostService.AddDataChangeLog(OpLogType.Pos消费项目仓库耗用表修改);
                            billCostService.Commit();
                        }
                    }

                }
                
            }

        }

        private bool PostDataByBusiDate(PostDataSet data)
        {
            var pmsParaService = GetService<IPmsParaService>();
            //结转产生耗用单
            var postUrl = pmsParaService.GetValue(CurrentInfo.HotelId, "PosExpendItemByBusiDateUrl");


            var jsonStr = JsonConvert.SerializeObject(data);
            var httpItem = new ScanOrder.Models.HttpItem();
            httpItem.Method = "post";
            httpItem.PostEncoding = Encoding.UTF8;
            httpItem.URL = postUrl;
            httpItem.ContentType = "application/json";
            httpItem.Postdata = jsonStr;// $"resortId={data.ResortId}&depotNo={data.DepotNo}&data={ JsonConvert.SerializeObject(data.BillList)}";

            var httpHelper = new ScanOrder.Models.HttpHelper();
            var html = httpHelper.GetHtml(httpItem);

            //记录请求数据
            AddOperationLog(OpLogType.Pos结转生成仓库耗用单请求, jsonStr);
            var result = JsonConvert.DeserializeObject<StockByItemCodeResult>(html.Html);
            //记录接口返回的数据
            AddOperationLog(OpLogType.Pos结转生成仓库耗用单结果, result.Msg);
            if (html.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.ErrorNo == "0")  //产生耗用单成功
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Pos提交云仓库数据集合
    /// </summary>
    public class PostDataSet
    {
        public string ResortId { get; set; }

        public List<PostDataBill> Data { get; set; }

        public string DepotNo { get; set; }

    }

    public class PostDataBill
    {
        public string BusinessDate { get; set; }

        public string TransNo { get; set; }

        public string ItemCode { get; set; }

        public string Quantity { get; set; }

    }
}