using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOnSale;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos特价菜功能
    /// </summary>
    [AuthPage(ProductType.Pos, "p30003")]
    public class BasicDataPosOnSaleController : BaseEditInWindowController<PosOnSale, IPosOnSaleService>
    {

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_PosOnSale", "");
            return View();
        }
        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosOnSaleAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosOnSaleAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosOnSaleService>();

            if (!CheckDate(addViewModel.StartTime, addViewModel.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始时间或者结束时间输入不合法！"));
            }
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, "CY", addViewModel.Refeid, addViewModel.TabTypeid, addViewModel.CustomerTypeid, addViewModel.Itemid, addViewModel.Unitid, addViewModel.ITagperiod, addViewModel.StartTime, addViewModel.EndTime);
            //   bool isexsit = modelService.IsExists(CurrentInfo.HotelId, "CY", addViewModel.Itemid, addViewModel.Unitid, id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            addViewModel.Module = "CY";

            ActionResult result = _Add(addViewModel, new PosOnSale { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now, iType = 1 }, OpLogType.Pos特价菜添加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosOnSaleEditViewModel());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosOnSaleEditViewModel model)
        {
            var modelService = GetService<IPosOnSaleService>();
            if (!CheckDate(model.StartTime, model.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始时间或者结束时间输入不合法！"));
            }

            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, "CY", model.Refeid, model.TabTypeid, model.CustomerTypeid, model.Itemid, model.Unitid, model.ITagperiod, model.StartTime, model.EndTime, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.Module = "CY";
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosOnSale(), OpLogType.Pos特价菜修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id.Trim(','), GetService<IPosOnSaleService>(), OpLogType.Pos特价菜删除);
        }

        #endregion 批量删除

        #region 批量操作

        //批量操作视图
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult BatchHandle()
        {
            var model = new PosOnSaleAddViewModel() { IsUsed = true };
            return PartialView("_BatchHandle", model);
        }

        //列表视图
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosItemGrid()
        {
            return PartialView("_PosItemGrid");
        }
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosOnSaleGrid()
        {
            return PartialView("_PosOnSaleGrid");
        }


        //查询视图
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosItemQuery(_PosItemQueryViewModel model)
        {
            if (model == null)
            {
                model = new _PosItemQueryViewModel();
            }
            return PartialView("_PosItemQuery", model);
        }

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _PosOnSaleQuery(_PosonSaleQueryViewModel model)
        {
            if (model == null)
            {
                model = new _PosonSaleQueryViewModel();
            }
            return PartialView("_PosOnSaleQuery", model);
        }

        //批量增加特价菜
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        [HttpPost]
        public JsonResult BatchAdd(string selIds, PosOnSaleAddViewModel model,decimal? diff_price,decimal? percent_price)
        {
            //获取所有消费项目
            if (string.IsNullOrEmpty(selIds))
            {
                return Json(JsonResultData.Failure("请选择消费项目"));
            }

            var itemidlist = selIds.Trim().Trim('|').Split('|').ToList();

            var itemservice = GetService<IPosItemService>();
            var itemlsit = itemservice.GetItems(CurrentInfo.HotelId, u => itemidlist.Contains(u.Id));

            //单位验证
            if (string.IsNullOrEmpty(model.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }
            //获取要设置的单位
            var itemunitservice = GetService<IPosUnitService>();
            var setitemunit = itemunitservice.Get(model.Unitid);
            if (setitemunit == null)
            {
                return Json(JsonResultData.Failure("单位不存在"));
            }

            //创建数组，保存在选择单位下的价格
            var additem = new List<PosItem>();

            var itempriceservrice = GetService<IPosItemPriceService>();
            //循环所有消费项目，查找单位是否匹配,不存在不添加该项目
            for (int i = 0; i < itemlsit.Count; i++)
            {
                //查找对应的消费单位价格记录
                var itemprice = itempriceservrice.GetPosItemPriceCountByItemID(CurrentInfo.HotelId, itemlsit[i].Id, setitemunit.Id, u => true);
                if (itemprice == null)
                {
                    itemlsit.Remove(itemlsit[i]);
                    i--;
                    continue;
                }

                //价格为空，也不添加
                if (itemprice.Price == null)
                {
                    itemlsit.Remove(itemlsit[i]);
                    i--;
                    continue;
                }
                //获取单位价格
                additem.Add(new PosItem() { Id = itemlsit[i].Id, Price = itemprice.Price});             
            }

            //价格类型 (1:固定价格；2：差价；3：百分比差价)
            var pricetype = 0;
            //验证价格(优先级 ： 价格 > 差价 > 百分比差价) ，价格结果小于0价格设置为0
            if (model.Price != null)  //价格
            {
                if (model.Price < 0)
                {
                    return Json(JsonResultData.Failure("价格不能小于0"));
                }
                pricetype = 1;
            }
            else if (diff_price != null)//差价
            {                
                pricetype = 2;
            }
            else if (percent_price != null)//百分比差价
            {
                pricetype = 3;
            }
            else {
                return Json(JsonResultData.Failure("请填写价格或者差价或者百分比差价"));             
            }

            //是否打折验证
            if (model.IsDiscount == null)
            {
                return Json(JsonResultData.Failure("是否打折项取值错误"));
            }
            //折扣率验证
            if (Convert.ToBoolean(model.IsDiscount))
            {               
                if (model.Discount == null)
                {
                    return Json(JsonResultData.Failure("请填写折扣率"));
                }
                if (model.Discount > 1 || model.Discount <= 0)
                {
                    return Json(JsonResultData.Failure("折扣率取值范围为 0-1 ."));
                }
            }
            //验证营业点
            if (!string.IsNullOrEmpty(model.Refeid))
            {
                var refeservice = GetService<IPosRefeService>();               
                if (refeservice.GetEntity(CurrentInfo.HotelId, model.Refeid) == null)
                {
                    return Json(JsonResultData.Failure("营业点不存在"));
                }
            }
            //验证餐台类型
            if (!string.IsNullOrEmpty(model.TabTypeid))
            {
                var tabservice = GetService<IPosTabtypeService>();
                if (tabservice.GetEntity(CurrentInfo.HotelId, model.TabTypeid) == null)
                {
                    return Json(JsonResultData.Failure("餐台类型不存在"));
                }
            }
            //验证客人类型
            if (!string.IsNullOrEmpty(model.CustomerTypeid))
            {
                var PosCustomerTypeservice = GetService<IPosCustomerTypeService>();
                if (PosCustomerTypeservice.Get(model.CustomerTypeid) == null)
                {
                    return Json(JsonResultData.Failure("客人类型不存在"));
                }
            }
            //验证日期类型
            var service = GetService<ICodeListService>();
            if (service.GetiTagperiodList().Count(u=>u.code == Convert.ToString(model.ITagperiod)) == 0)
            {
                return Json(JsonResultData.Failure("日期类型不存在"));
            }
            //验证计算类型
            var _camlist =new List<string>(){ "0","1"};
            if (!_camlist.Contains(model.ICmpType.ToString()))
            {
                return Json(JsonResultData.Failure("计算类型不存在"));
            }
            //验证开始时间格式 和 结束时间格式
            if (!CheckDate(model.StartTime, model.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始时间或者结束时间输入不合法！"));
            }

            //比较数据库是否存在相同记录,不存在不添加该项目
            var modelService = GetService<IPosOnSaleService>();
            for (int i = 0; i < itemlsit.Count; i++)
            {

                bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Module, model.Refeid, model.TabTypeid, model.CustomerTypeid, itemlsit[i].Id, model.Unitid, model.ITagperiod, model.StartTime, model.EndTime);
                if (isexsit)
                {
                    itemlsit.Remove(itemlsit[i]);
                    i--;
                }
            }
            //创建添加特价菜记录
            var addlist = new List<PosOnSale>();
            foreach (var item in itemlsit)
            {                
                var _posonsale = new PosOnSale()
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    Unitid = model.Unitid,
                    Refeid = model.Refeid,
                    TabTypeid = model.TabTypeid,
                    CustomerTypeid = model.CustomerTypeid,
                    ITagperiod = model.ITagperiod,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Discount = model.Discount,
                    ICmpType = model.ICmpType,
                    IsLimit = model.IsLimit,
                    IsService = model.IsService,
                    IsUsed = model.IsUsed,
                    Itemid = item.Id,
                    ModifiedDate = DateTime.Now,
                    Module = model.Module,
                    Remark = model.Remark,
                    IsDiscount = model.IsDiscount,
                    iType = 1
                };

                //价格确定，根据价格类型确定 
                //价格类型 (1:固定价格；2：差价；3：百分比差价)
                //获取价格
                var curprice = additem.Where(u => u.Id == item.Id).FirstOrDefault().Price;
                if (pricetype == 1)
                {
                    _posonsale.Price = model.Price;
                }
                else if (pricetype == 2)
                {  //减去差价小于0，价格为0
                    if (curprice + diff_price < 0)
                        _posonsale.Price = 0;
                    else
                    _posonsale.Price = curprice + diff_price;
                }
                else if (pricetype == 3)
                {    //减去百分比差价小于0，价格为0
                    if ((curprice + (curprice * (Convert.ToDecimal(percent_price) / 100))) < 0)
                        _posonsale.Price = 0;
                    else
                    _posonsale.Price = curprice + (curprice * Convert.ToDecimal(percent_price) );
                }

                addlist.Add(_posonsale);
            }
            //保存
            foreach (var item in addlist)
            {
                modelService.Add(item);
            }
            modelService.Commit();
            return Json(JsonResultData.Successed(""));
        }

        //修改特价菜
        [AuthButton(AuthFlag.Update)]
        [JsonException]
        [HttpPost]
        public JsonResult BatchUpdate(string selIds, PosOnSaleAddViewModel model, decimal? diff_price, decimal? percent_price)
        {
            //获取所有消费项目
            if (string.IsNullOrEmpty(selIds))
            {
                return Json(JsonResultData.Failure("请选择特价菜"));
            }
            var onsaleidlist = selIds.Trim().Trim('|').Split('|').ToList().Select(u=> { return Guid.Parse(u); });
            //获取选择的特价菜，
            var onsaleservice = GetService<IPosOnSaleService>();
            var posinsalelist = onsaleservice.GetPosOnSales(CurrentInfo.HotelId, u => onsaleidlist.Contains(u.Id));
            if (posinsalelist.Count == 0 )
            {
                return Json(JsonResultData.Failure("请选择特价菜"));
            }          
            
            //单位验证
            if (string.IsNullOrEmpty(model.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }

            //获取要设置的单位
            var itemunitservice = GetService<IPosUnitService>();
            var setitemunit = itemunitservice.Get(model.Unitid);
            if (setitemunit == null)
            {
                return Json(JsonResultData.Failure("单位不存在"));
            }

            //创建数组，保存在选择单位下的价格
            var onsale_priceitem = new List<PosOnSale>();
            //特价菜的单位匹配筛选（查找特价菜关联的消费项目的单位关联，有才可修改，没有过滤）
            var itempriceservrice = GetService<IPosItemPriceService>();
            //循环所有特价菜项目，查找关联的消费项目单位是否匹配,存在不添加该项目
            for (int i = 0; i < posinsalelist.Count; i++)
            {
                //查找对应的消费单位价格记录
                var itemprice = itempriceservrice.GetPosItemPriceCountByItemID(CurrentInfo.HotelId, posinsalelist[i].Itemid, setitemunit.Id, u => true);
                if (itemprice == null)
                {
                    posinsalelist.Remove(posinsalelist[i]);
                    i--;
                    continue;
                }

                //价格为空，也不添加
                if (itemprice.Price == null)
                {
                    posinsalelist.Remove(posinsalelist[i]);
                    i--;
                    continue;
                }
                //获取单位价格
                onsale_priceitem.Add(new PosOnSale() { Id = posinsalelist[i].Id, Price = itemprice.Price });
            }

            //价格类型 (1:固定价格；2：差价；3：百分比差价)
            var pricetype = 0;
            //验证价格(优先级 ： 价格 > 差价 > 百分比差价)
            if (model.Price != null)  //价格
            {
                if (model.Price < 0)
                {
                    return Json(JsonResultData.Failure("价格不能小于0"));
                }
                pricetype = 1;
            }
            else if (diff_price != null)//差价
            {
                //减去差价小于0，不设置
                for (int i = 0; i < posinsalelist.Count; i++)
                {
                    if (onsale_priceitem.First(u => u.Id == posinsalelist[i].Id).Price + diff_price < 0)
                    {
                        posinsalelist.Remove(posinsalelist[i]);
                        i--;
                    }
                }
                pricetype = 2;
            }
            else if (percent_price != null)//百分比差价
            {
                //减去百分比差价小于0，不设置
                for (int i = 0; i < posinsalelist.Count; i++)
                {
                    var curpositem = onsale_priceitem.First(u => u.Id == posinsalelist[i].Id);
                    if ((curpositem.Price + (curpositem.Price * (Convert.ToDecimal(percent_price) / 100))) < 0)
                    {
                        posinsalelist.Remove(posinsalelist[i]);
                        i--;
                    }
                }
                pricetype = 3;
            }
            else
            {
                return Json(JsonResultData.Failure("请填写价格或者差价或者百分比差价"));
            }

            //是否打折验证
            if (model.IsDiscount == null)
            {
                return Json(JsonResultData.Failure("是否打折项取值错误"));
            }
            //折扣率验证
            if (Convert.ToBoolean(model.IsDiscount))
            {
                if (model.Discount == null)
                {
                    return Json(JsonResultData.Failure("请填写折扣率"));
                }
                if (model.Discount > 1 || model.Discount <= 0)
                {
                    return Json(JsonResultData.Failure("折扣率取值范围为 0-1 ."));
                }
            }
            //验证营业点
            if (!string.IsNullOrEmpty(model.Refeid))
            {
                var refeservice = GetService<IPosRefeService>();
                if (refeservice.GetEntity(CurrentInfo.HotelId, model.Refeid) == null)
                {
                    return Json(JsonResultData.Failure("营业点不存在"));
                }
            }
            //验证餐台类型
            if (!string.IsNullOrEmpty(model.TabTypeid))
            {
                var tabservice = GetService<IPosTabtypeService>();
                if (tabservice.GetEntity(CurrentInfo.HotelId, model.TabTypeid) == null)
                {
                    return Json(JsonResultData.Failure("餐台类型不存在"));
                }
            }
            //验证客人类型
            if (!string.IsNullOrEmpty(model.CustomerTypeid))
            {
                var PosCustomerTypeservice = GetService<IPosCustomerTypeService>();
                if (PosCustomerTypeservice.Get(model.CustomerTypeid) == null)
                {
                    return Json(JsonResultData.Failure("客人类型不存在"));
                }
            }
            //验证日期类型
            var service = GetService<ICodeListService>();
            if (service.GetiTagperiodList().Count(u => u.code == Convert.ToString(model.ITagperiod)) == 0)
            {
                return Json(JsonResultData.Failure("日期类型不存在"));
            }
            //验证计算类型
            var _camlist = new List<string>() { "0", "1" };
            if (!_camlist.Contains(model.ICmpType.ToString()))
            {
                return Json(JsonResultData.Failure("计算类型不存在"));
            }
            //验证开始时间格式 和 结束时间格式
            if (!CheckDate(model.StartTime, model.EndTime))
            {
                return Json(JsonResultData.Failure("操作错误,开始时间或者结束时间输入不合法！"));
            }

            //比较数据库是否存在相同记录,存在不添加该项目
            var modelService = GetService<IPosOnSaleService>();
            for (int i = 0; i < posinsalelist.Count; i++)
            {              
                //匹配存在冲突记录
                bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Module, model.Refeid, model.TabTypeid, model.CustomerTypeid, posinsalelist[i].Itemid, model.Unitid, model.ITagperiod, model.StartTime, model.EndTime, posinsalelist[i].Id);
                if (isexsit)
                {
                    posinsalelist.Remove(posinsalelist[i]);
                    i--;
                }
            }


            //修改值
            for (int i = 0; i < posinsalelist.Count; i++)
            {
                var item = posinsalelist[i];
                item.Unitid = model.Unitid;
                item.Refeid = model.Refeid;
                item.TabTypeid = model.TabTypeid;
                item.CustomerTypeid = model.CustomerTypeid;
                item.ITagperiod = model.ITagperiod;
                item.StartTime = model.StartTime;
                item.EndTime = model.EndTime;
                item.Discount = model.Discount;
                item.ICmpType = model.ICmpType;
                item.IsLimit = model.IsLimit;
                item.IsService = model.IsService;
                item.IsUsed = model.IsUsed;
                item.ModifiedDate = DateTime.Now;
                item.Module = model.Module;
                item.Remark = model.Remark;
                item.IsDiscount = model.IsDiscount;

                //价格确定，根据价格类型确定 
                //价格类型 (1:固定价格；2：差价；3：百分比差价)
                //获取价格
                var curprice = onsale_priceitem.Where(u => u.Id == item.Id).FirstOrDefault().Price;
                if (pricetype == 1)
                {
                    item.Price = model.Price;
                }
                else if (pricetype == 2)
                {  //减去差价小于0，价格为0
                    if (curprice + diff_price < 0)
                        item.Price = 0;
                    else
                        item.Price = curprice + diff_price;
                }
                else if (pricetype == 3)
                {    //减去百分比差价小于0，价格为0
                    if ((curprice + (curprice * (Convert.ToDecimal(percent_price) / 100))) < 0)
                        item.Price = 0;
                    else
                        item.Price = curprice + (curprice * Convert.ToDecimal(percent_price));
                }
            }

            //提交修改
            foreach (var item in posinsalelist)
            {
                onsaleservice.Update(item,null);
            }
            onsaleservice.Commit();

            return Json(JsonResultData.Successed(""));
        }

        #endregion



        /// <summary>
        /// 查询消费项目所有数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosItemA([DataSourceRequest]DataSourceRequest request, string DeptId = "", string ItemClassid = "", string SubClassid = "", string CodeAndName = "",string startcode = "",string endcode = "", int isfirst= 0)
        {
            //初次加载不返回数据
            if (isfirst == 1)
            {
                return new System.Web.Mvc.JsonResult()
                {                  
                    Data = new List<PosItem>().ToDataSourceResult(request),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue
                };
            }

            var service = GetService<IPosItemService>();
            var list = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.D.ToString());
            list = list.Where(m => (m.DeptClassid == DeptId || string.IsNullOrEmpty(DeptId))
            && (m.ItemClassid == ItemClassid || string.IsNullOrEmpty(ItemClassid))
            && (m.SubClassid == SubClassid || string.IsNullOrEmpty(SubClassid))
            && ((m.Code.Contains(CodeAndName) || m.Cname.Contains(CodeAndName)) || string.IsNullOrEmpty(CodeAndName)) && m.IsSubClass == false ).ToList();

            if ( !string.IsNullOrEmpty( startcode)  &&  !string.IsNullOrEmpty(endcode))
            {
                list = list.Where( m => m.Code.CompareTo(startcode) >= 0 && m.Code.CompareTo(endcode) <= 0 ).ToList();
            }

            return new System.Web.Mvc.JsonResult()
            {
                Data = list.ToDataSourceResult(request),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// 查询特价菜
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemname"></param>
        /// <param name="unitname"></param>
        /// <param name="refename"></param>
        /// <param name="tabname"></param>
        /// <param name="customerid"></param>
        /// <param name="iTagperiod"></param>
        /// <param name="CmpType"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosSale([DataSourceRequest]DataSourceRequest request,string itemname="",string unitid="",string refeid="",string tabid="",string customerid="",string starttime="",string endtime="",string iTagperiod="",Int16? CmpType =null ,int? isUsed= null)
        {
            var service = GetService<IPosOnSaleService>();
            var list = service.GetBatchHandlePosOnSale(CurrentInfo.HotelId,itemname, unitid, refeid, tabid, customerid, iTagperiod,CmpType,isUsed).Select(u=> 
            {
                var datamodel = new PosOnSaleGridViewModel();
                AutoSetValueHelper.SetValues(u, datamodel);
                return datamodel;
            });

            //对于开始时间和结束时间的筛选

            if (!string.IsNullOrEmpty(starttime) && string.IsNullOrEmpty(endtime))
            { //开始时间                
                var stime = Convert.ToDateTime($"2018-11-11 {starttime}");
                list = list.Where(u =>
                {
                    var date = Convert.ToDateTime($"2018-11-11 {u.StartTime}");
                    if (date > stime)
                    {
                        return true;
                    }
                    return false;
                });
            }
            else if (string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
            {//结束时间
                var etime = Convert.ToDateTime($"2018-11-11 {endtime}");
                list = list.Where(u =>
                {
                    var date = Convert.ToDateTime($"2018-11-11 {u.EndTime}");
                    if (date <= etime)
                    {
                        return true;
                    }
                    return false;
                });
            }
            else if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
            {//开始时间 + 结束时间
                var stime = Convert.ToDateTime($"2018-11-11 {starttime}");
                var etime = Convert.ToDateTime($"2018-11-11 {endtime}");
                if (stime > etime)  //开始时间大于结束时间，结束时间为后一天
                    etime = etime.AddDays(1);

                list = list.Where(u =>
                {

                    var sdate = Convert.ToDateTime($"2018-11-11 {u.StartTime}");
                    var edate = Convert.ToDateTime($"2018-11-11 {u.EndTime}");
                    if (sdate >= stime && edate <= etime)
                    {
                        return true;
                    }
                    return false;
                });

            }

            return new System.Web.Mvc.JsonResult()
            {
                Data = list.ToDataSourceResult(request),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };

        }

        /// <summary>
        /// 消费项目列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItems(string text="",string itemclassid="")
        {
            var service = GetService<IPosItemService>();

             Func<PosItem, bool> wherefunc = (b) =>
             {
                 var res = b.Module == "CY" && b.DcFlag.ToLower() == PosItemDcFlag.D.ToString().ToLower()
                      && (b.Status == (byte)EntityStatus.启用 || b.Status == null);

                 if (!string.IsNullOrEmpty(text))
                 {
                     res = res &&( b.Cname.Contains(text) || b.Code.Contains(text));
                 }

                 if (!string.IsNullOrEmpty(itemclassid))
                 {
                     res = res && b.ItemClassid == itemclassid;
                 }

                 return res;
             };

            var _list = service.GetItems(CurrentInfo.HotelId, wherefunc);

            var listItems = _list.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname+ "-" + w.Code }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 消费项目单位列表
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListUnits(string itemId)
        {
            var service = GetService<IPosItemPriceService>();
            var datas = service.GetPosItemPriceForCopy(CurrentInfo.HotelId, itemId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Unitid, Text = w.Unit }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 日期类型列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListiTagperiod()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetiTagperiodList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 计算类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListICmpType()
        {
            var result = new[] {
                new { Text = "开台时间", Value = "0" },
                new { Text = "买单时间", Value = "1" }
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客人类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult CustomerTypeList()
        {
            var service = GetService<IPosCustomerTypeService>();
            var datas = service.GetPosCustomerTypeByModule(CurrentInfo.HotelId, "CY");
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 餐台类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListTab()
        {
            var service = GetService<IPosTabService>();
            var datas = service.GetPosTabByModule(CurrentInfo.HotelId, "CY");
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }


        private bool CheckDate(string startTime, string EndTime)
        {
            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(EndTime))
            {
                return false;
            }
            var date = DateTime.Now.ToShortDateString().ToString();

            try
            {
                var s = Convert.ToDateTime(date + " " + startTime);
                var e = Convert.ToDateTime(date + " " + EndTime);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }




}