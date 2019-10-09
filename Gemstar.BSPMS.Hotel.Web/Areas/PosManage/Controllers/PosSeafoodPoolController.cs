using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosSeafoodPool;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models.Account;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 海鲜池功能模块
    /// </summary>
    [AuthPage(ProductType.Pos, "p70")]
    public class PosSeafoodPoolController : BaseEditInWindowController<PosBillDetail, IPosSeaFoodPoolService>
    {

        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            //称重是否刷卡
            var pmsParaService = GetService<IPmsParaService>();
            ViewBag.WeighIsPayCard = pmsParaService.GetValue(CurrentInfo.HotelId, "WeighIsPayCard");

            SeaFoodPoolQueryViewModel model = new SeaFoodPoolQueryViewModel();
            var service = GetService<IPosSeaFoodPoolService>();
            model.PageTotal = service.GetSeaFoodPoolListCount(CurrentInfo.HotelId, "");
            return View(model);
        }

        #region 查询数据

        /// <summary>
        /// 查询海鲜池数据
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosSeaFoodPoolList(SeaFoodPoolQueryViewModel queryModel)
        {
            var service = GetService<IPosSeaFoodPoolService>();
            var list = service.GetSeaFoodPoolList(CurrentInfo.HotelId, queryModel.TabId ?? "", queryModel.PageIndex, queryModel.PageSize);
            return PartialView("_PosSeaFoodPoolList", list);
        }

        /// <summary>
        /// 获取查询数据的总数量
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult GetSeaFoodPoolListCount(SeaFoodPoolQueryViewModel queryModel)
        {
            var service = GetService<IPosSeaFoodPoolService>();
            var PageTotal = service.GetSeaFoodPoolListCount(CurrentInfo.HotelId, queryModel.TabId ?? "");
            return Content(PageTotal.ToString());
        }
        #endregion

        #region 查询视图

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _Query()
        {
            return PartialView("_Query");
        }

        /// <summary>
        /// 称重输入界面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AuthManage)]
        public PartialViewResult _WeighInput(PosBillDetailAddViewModel model)
        {
            var service = GetService<IPosBillDetailService>();
            var itemService = GetService<IPosItemService>();

            var tabService = GetService<IPosTabService>();
            var billModel = service.Get(Convert.ToInt32(model.mId));

          
            ///从 billdetail表中，获取单价

            if (billModel != null)
            {  
                model.OriPiece = billModel.Piece;
                model.Tabid = billModel.Tabid;
                var tab = tabService.Get(billModel.Tabid);

                model.tabName = tab.Cname;
                model.tabNo = tab.TabNo;
                model.ItemName = billModel.ItemName;
                model.ItemCode = billModel.ItemCode;
                model.OriQuan = billModel.OriQuan;

                //给消费单价赋值
                model.Price = billModel.Price;

                return PartialView("_WeighInput", model);
            }
            return PartialView("_WeighInput");
        }


        #endregion

        #region 刷卡
        /// <summary>
        /// 刷卡界面
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]

        public ActionResult CheckCard(PayByCardViewModel model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.CardId))
            {
                try
                {
                    var service = GetService<IPmsUserService>();
                    var entity = service.GetEntityByCardId(CurrentInfo.GroupId, CurrentInfo.HotelId, CryptHelper.EncryptDES(model.CardId));
                    if (entity != null)
                    {
                        var product = GetProduct();
                        FormsAuthentication.SetAuthCookie(entity.Code, false);
                        CurrentInfo.UserId = entity.Id.ToString();
                        CurrentInfo.UserCode = entity.Code;
                        CurrentInfo.UserName = entity.Name;
                        CurrentInfo.IsRegUser = entity.IsReg == 1 ? true : false;
                        CurrentInfo.SaveValues();
                        CurrentInfo.LoadValues();
                        return Json(JsonResultData.Successed());
                    }
                    else
                    {
                        return Json(JsonResultData.Failure("请输入正确卡号！"));
                    }
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            else
            {
                return Json(JsonResultData.Failure("请输入卡号！"));
            }

        }

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PayByCard(PayByCardViewModel model)
        {
            return PartialView("_PayByCard", model);
        }

        #endregion

        #region 修改账单明细
        [AuthButton(AuthFlag.None)]
        public ActionResult UpdateBillDetail(PosBillDetailAddViewModel model)
        {
            //修改对应，消费商品的单价:
            //var priceNew = model.Price;
        
            
                   

            var service = GetService<IPosBillDetailService>();
            var BillDetailModel = service.Get(Convert.ToInt64(model.mId));

           


            if (BillDetailModel == null)
            {
                return Json(JsonResultData.Failure("操作的数据不存在,请刷新界面"));
            }
            var newBillDetailModel = new PosBillDetail();
            AutoSetValueHelper.SetValues(BillDetailModel, newBillDetailModel);

            var pmsParaService = GetService<IPmsParaService>();
            //称重是否出品
            var SeafoodWeighProduceVal = pmsParaService.GetValue(CurrentInfo.HotelId, "SeafoodWeighProduce");

            //海鲜第一次落单是否出品
            var SeafoodProduceVal = pmsParaService.GetValue(CurrentInfo.HotelId, "SeafoodProduce");
            if (SeafoodProduceVal == "1")//海鲜第一次落单出品
            {
                if (SeafoodWeighProduceVal == "1")//出品
                {
                    newBillDetailModel.IsProduce = (byte)PosBillDetailIsProduce.修改;
                }
            }
            else if (SeafoodProduceVal == "0")  //海鲜落单第一次不出品
            {
                if (SeafoodWeighProduceVal == "1")//出品
                {
                    newBillDetailModel.IsProduce = (byte)PosBillDetailIsProduce.未出品;
                }
            }

            if(model.Quantity!=null)
            {
                newBillDetailModel.Quantity = model.Quantity;
            }
            if(model.Piece!=null)
            {
                newBillDetailModel.Piece = model.Piece;
            }
          
            if(model.Price!=null)
            {
                newBillDetailModel.Price = model.Price;
                newBillDetailModel.PriceClub = model.Price;
                newBillDetailModel.PriceOri = model.Price;
            }


            newBillDetailModel.IsWeight = true;
            newBillDetailModel.Dueamount = newBillDetailModel.Quantity * newBillDetailModel.Price;
         
           
           

            var billService = GetService<IPosBillService>();
            var bill = billService.Get(newBillDetailModel.Billid);

            service.Update(newBillDetailModel, BillDetailModel);


            AddOperationLog(OpLogType.Pos账单消费明细修改, "称重数量修改,称重原数量:" + BillDetailModel.OriQuan + "新数量:" + newBillDetailModel.Quantity , bill.BillNo);
            service.Commit();

            //重算金额
            service.StatisticsBillDetail(CurrentInfo.HotelId, newBillDetailModel.Billid, newBillDetailModel.Billid);
            return Json(JsonResultData.Successed());
        }
        #endregion

        #region 已称重

        [AuthButton(AuthFlag.None)]
        public ActionResult _WeighedList()
        {
            return PartialView("_WeighedList");
        }

        [AuthButton(AuthFlag.None)]

        public ActionResult GetWeighedList([DataSourceRequest]DataSourceRequest request, string querytext)
        {

            var service = GetService<IPosSeaFoodPoolService>();
            var list = service.GetWeighedList(CurrentInfo.HotelId, querytext ?? "");
            return Json(list.ToDataSourceResult(request));
        }
        #endregion
    }
}