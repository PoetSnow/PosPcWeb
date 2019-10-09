using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemAction;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPrice;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemRefe;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos消费项目
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020003")]
    public class BasicDataPosItemController : BaseEditInWindowController<PosItem, IPosItemService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Item", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return _Add(new PosItemAddViewModel());
            }
            else
            {
                var result = new PosItemAddViewModel();

                var posItemService = GetService<IPosItemService>();
                var posItem = posItemService.Get(id);

                //复制项目
                AutoSetValueHelper.SetValues(posItem, result);
                if (!string.IsNullOrEmpty(id))
                {
                    result.Cname = "";
                    var maxCode = posItemService.GetNewItemCodeByClassid(CurrentInfo.HotelId, posItem.ItemClassid, posItem.SubClassid, PosItemDcFlag.D.ToString());
                    //result.Id = CurrentInfo.HotelId + maxCode;
                    result.Code = maxCode;
                    result.Ename = "";
                    result.Cname = "";
                    result.Oname = "";
                    result.Barcode = "";
                }

                return _Add(result);
            }

        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname, PosItemDcFlag.D.ToString());
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (string.IsNullOrWhiteSpace(addViewModel.Unitid) && addViewModel.IsSubClass == false)
            {
                return Json(JsonResultData.Failure("操作错误,请选择单位！"));
            }
            if (addViewModel.IsSuite == true && addViewModel.IsFeast == true)
            {
                return Json(JsonResultData.Failure("操作错误,套餐，酒席只能选择一种！"));
            }

            if (!string.IsNullOrEmpty(Request["ShowSet"]))
            {
                addViewModel.ShowSet = Request["ShowSet"].ToString();
            }

            ActionResult result = _Add(addViewModel, new PosItem
            {
                Id = id,
                Hid = CurrentInfo.HotelId,
                DcFlag = PosItemDcFlag.D.ToString(),
                OperName = CurrentInfo.UserName,
                ModifiedDate = DateTime.Now,
                Status = (byte)PosItemStatus.提交,
                IndexNo = !string.IsNullOrEmpty(addViewModel.Cname) ? modelService.PosItemHz2other(addViewModel.Cname, 1) : null,//默认添加检索码
                PYCode = !string.IsNullOrEmpty(addViewModel.Cname) ? modelService.PosItemHz2other(addViewModel.Cname, 3) : null//默认添加拼音码
            }, OpLogType.Pos消费项目增加);

            #region 增加对应大类

            var multiClassService = GetService<IPosItemMultiClassService>();
            if (string.IsNullOrWhiteSpace(addViewModel.SubClassid))
            {
                PosItemMultiClass multiClass = new PosItemMultiClass
                {
                    Itemid = id,
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    ItemClassid = addViewModel.ItemClassid,
                    IsSubClass = false,
                    Modified = DateTime.Now,
                    Remark = ""
                };
                multiClassService.Add(multiClass);
                multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
                multiClassService.Commit();
            }
            else
            {
                var item = modelService.Get(addViewModel.SubClassid);
                PosItemMultiClass multiClass = new PosItemMultiClass
                {
                    Itemid = id,
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    ItemClassid = addViewModel.SubClassid,
                    IsSubClass = item == null ? null : item.IsSubClass,
                    Modified = DateTime.Now,
                    Remark = ""
                };
                multiClassService.Add(multiClass);
                multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
                multiClassService.Commit();
            }

            #endregion 增加对应大类

            #region 增加对应价格

            //判断是否是分类
            if (addViewModel.IsSubClass != true)
            {
                var unitService = GetService<IPosUnitService>();
                var itemPriceService = GetService<IPosItemPriceService>();
                var unit = unitService.Get(addViewModel.Unitid);
                PosItemPrice itemPrice = new PosItemPrice
                {
                    Id = Guid.NewGuid(),
                    Hid = CurrentInfo.HotelId,
                    Itemid = id,
                    Unitid = unit.Id,
                    UnitCode = unit.Code,
                    Unit = unit.Cname,
                    IsDefault = true,
                    Price = addViewModel.Price,
                    Multiple = 1,
                    Modified = DateTime.Now
                };
                itemPriceService.Add(itemPrice);
                itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格增加);
                itemPriceService.Commit();
            }

            #endregion 增加对应价格

            #region 增加对应营业点

            var classService = GetService<IPosItemClassService>();
            var itemRefeService = GetService<IPosItemRefeService>();
            var itemClass = classService.Get(addViewModel.ItemClassid);
            if (itemClass != null && !string.IsNullOrWhiteSpace(itemClass.Refeid))
            {
                string[] refes = itemClass.Refeid.Split(',');
                var refeService = GetService<IPosRefeService>();
                var refeList = refeService.GetRefe(CurrentInfo.HotelId).Where(m => m.IStatus == (byte)EntityStatus.启用 || m.IStatus == null);

                foreach (var temp in refes)
                {
                    var refe = refeList.Where(w => w.Id == temp).FirstOrDefault();
                    if (refe == null)
                    {
                        continue;
                    }
                    PosItemRefe itemRefe = new PosItemRefe
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        Itemid = id,
                        Refeid = temp,
                        RefeName = refe.Cname,
                        Shuffleid = refe.ShuffleId,
                        Modified = DateTime.Now,
                        IsDepartPrint = false,
                        IsTabPrint = false,
                        Seqid = 1
                    };

                    itemRefeService.Add(itemRefe);
                    itemRefeService.AddDataChangeLog(OpLogType.Pos消费项目对应营业点增加);
                    itemRefeService.Commit();
                }
            }

            #endregion 增加对应营业点

            //return _Edit(id, new PosItemEditViewModel());
            return Json(JsonResultData.Successed(id));
            // return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            var posItemService = GetService<IPosItemService>();
            var posItem = posItemService.Get(id);

            var service = GetService<IPosItemClassService>();
            var datas = service.GetPosItemClassAndSubClass(CurrentInfo.HotelId);
            var ItemEditMuiltClass = datas.Select(c => new PosItemEditMuiltClass
            {
                Id = c.Id,
                Cname = c.Cname
            });
            var result = new PosItemEditViewModel()
            {
                MuiltClass = ItemEditMuiltClass
            };
            ViewBag.ItemName = posItem.Cname;


            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(posItem, result);
            result.OriginJsonData = serializer.Serialize(posItem);

            return PartialView("_Edit", result);

            //return _Edit(id, result);
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemEditViewModel model)
        {


            var modelService = GetService<IPosItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, PosItemDcFlag.D.ToString(), model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (string.IsNullOrWhiteSpace(model.Unitid) && model.IsSubClass == false)
            {
                return Json(JsonResultData.Failure("操作错误,请选择单位！"));
            }
            if (model.IsSuite == true && model.IsFeast == true)
            {
                return Json(JsonResultData.Failure("操作错误,套餐，酒席只能选择一种！"));
            }
            model.OperName = CurrentInfo.UserName;
            model.ModifiedDate = DateTime.Now;
            //开台信息、楼面台号显示内容为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["ShowSet"] == null)
            {
                Request.Form.Add("ShowSet", "");
            }


            if (!string.IsNullOrEmpty(Request["ShowSet"]))
            {
                model.ShowSet = Request["ShowSet"].ToString();
            }


            #region 处理消费项目对应价格 
            //判断选择的单位是否存在 消费项目单位表中
            var itemPriceService = GetService<IPosItemPriceService>();
            var itemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Id, model.Unitid);
            if (itemPrice == null && model.IsSubClass == false)
            {

                var unitService = GetService<IPosUnitService>();
                var unit = unitService.Get(model.Unitid);
                //Add(不存在数据并且不是分类，取修改前的单位ID 进行修改
                var oldItemPrice = itemPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Id, model.OldUnitId);
                if (oldItemPrice != null)
                {
                    var newItemPrice = new PosItemPrice();
                    AutoSetValueHelper.SetValues(oldItemPrice, newItemPrice);

                    newItemPrice.Price = model.Price;
                    newItemPrice.Modified = DateTime.Now;
                    newItemPrice.IsDefault = true;
                    newItemPrice.Unitid = model.Unitid;
                    newItemPrice.UnitCode = unit.Code;
                    newItemPrice.Unit = unit.Cname;

                    itemPriceService.Update(newItemPrice, oldItemPrice);
                    itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                    itemPriceService.Commit();

                    var itemList = itemPriceService.GetPosItemPriceForCopy(CurrentInfo.HotelId, model.Id).Where(w => w.Unitid != newItemPrice.Unitid).ToList();
                    if (itemList != null && itemList.Count > 0)
                    {
                        foreach (var item in itemList)
                        {
                            var newItem = new PosItemPrice();
                            AutoSetValueHelper.SetValues(item, newItem);
                            newItem.IsDefault = false;

                            itemPriceService.Update(newItem, item);
                            itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                            itemPriceService.Commit();
                        }
                    }

                }
                else
                {
                    itemPrice = new PosItemPrice
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        Itemid = model.Id,
                        Unitid = unit.Id,
                        UnitCode = unit.Code,
                        Unit = unit.Cname,
                        IsDefault = true,
                        Price = model.Price,
                        Multiple = 1,
                        Modified = DateTime.Now
                    };
                    itemPriceService.Add(itemPrice);
                    itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格增加);
                    itemPriceService.Commit();
                }
            }
            else
            {
                if (itemPrice != null)
                {
                    //Update
                    var newEntity = new PosItemPrice();
                    AutoSetValueHelper.SetValues(itemPrice, newEntity);
                    newEntity.Price = model.Price;
                    newEntity.Modified = DateTime.Now;
                    newEntity.IsDefault = true;
                    itemPriceService.Update(newEntity, itemPrice);
                    itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                    itemPriceService.Commit();

                    var itemList = itemPriceService.GetPosItemPriceForCopy(CurrentInfo.HotelId, model.Id).Where(w => w.Unitid != newEntity.Unitid).ToList();
                    if (itemList != null && itemList.Count > 0)
                    {
                        foreach (var item in itemList)
                        {
                            var newItem = new PosItemPrice();
                            AutoSetValueHelper.SetValues(item, newItem);
                            newItem.IsDefault = false;

                            itemPriceService.Update(newItem, item);
                            itemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                            itemPriceService.Commit();
                        }
                    }
                }
            }

            #endregion

            #region 处理消费项目对应分类
            var multiClassService = GetService<IPosItemMultiClassService>();
            if (string.IsNullOrEmpty(model.SubClassid))
            {
                //分类为空的话.用大类处理
                var MultiClass = multiClassService.GetPosItemMultiClassByItemEditAll(CurrentInfo.HotelId, model.Id, model.ItemClassid);
                if (MultiClass == null)
                {
                    //往消费项目对应大类表中插入一条数据
                    PosItemMultiClass multiClass = new PosItemMultiClass
                    {
                        Itemid = model.Id,
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        ItemClassid = model.ItemClassid,
                        IsSubClass = false,
                        Modified = DateTime.Now,
                        Remark = ""
                    };
                    multiClassService.Add(multiClass);
                    multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
                    multiClassService.Commit();
                }
            }
            else
            {
                var MultiClass = multiClassService.GetPosItemMultiClassByItemEditAll(CurrentInfo.HotelId, model.Id, model.SubClassid);
                if (MultiClass == null)
                {
                    //往消费项目对应大类表中插入一条数据
                    PosItemMultiClass multiClass = new PosItemMultiClass
                    {
                        Itemid = model.Id,
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        ItemClassid = model.SubClassid,
                        IsSubClass = false,
                        Modified = DateTime.Now,
                        Remark = ""
                    };
                    multiClassService.Add(multiClass);
                    multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
                    multiClassService.Commit();
                }



                //var item = modelService.Get(model.SubClassid);
                //PosItemMultiClass multiClass = new PosItemMultiClass
                //{
                //    Itemid = model.Id,
                //    Id = Guid.NewGuid(),
                //    Hid = CurrentInfo.HotelId,
                //    ItemClassid = model.SubClassid,
                //    IsSubClass = item == null ? null : item.IsSubClass,
                //    Modified = DateTime.Now,
                //    Remark = ""
                //};
                //multiClassService.Add(multiClass);
                //multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
                //multiClassService.Commit();
            }


            #endregion

            //计算套餐金额

            var itemSuitService = GetService<IPosItemSuitService>();
            itemSuitService.CalculationItemSuitAmount(CurrentInfo.HotelId, model.Id);

            var oldPosItem = new JavaScriptSerializer().Deserialize<PosItem>(Request["OriginJsonData"]);//对比消费项目名称是否一样

            //判断消费项目名称是否为空
            if (!string.IsNullOrEmpty(model.Cname))
            {
                //判断检索码是否为空,是否重新生成检索码
                if (string.IsNullOrEmpty(model.IndexNo) || oldPosItem.Cname != model.Cname)
                {
                    model.IndexNo = modelService.PosItemHz2other(model.Cname, 1);//默认添加检索码
                }

                //判断拼音码是否为空,是否重新生成检索码
                if (string.IsNullOrEmpty(model.PYCode) || oldPosItem.Cname != model.Cname)
                {
                    model.PYCode = modelService.PosItemHz2other(model.Cname, 3);
                }
            }

            ActionResult result = _Edit(model, new PosItem(), OpLogType.Pos消费项目修改);
            return result;
        }

        #endregion 修改

        #region 消费项目对应大类操作

        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult MultiClass_Update(PosItemMultiClassViewModel model)
        {
            if (model != null)
            {
                var service = GetService<IPosItemMultiClassService>();
                bool isexsit = service.IsExists(CurrentInfo.HotelId, model.ItemId, model.ItemClassidForEdit);
                if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

                var itemService = GetService<IPosItemService>();
                var item = itemService.Get(model.ItemClassidForEdit);
                //新增
                if (model.Id.ToString() == "-1")
                {
                    var entity = new PosItemMultiClass
                    {
                        Id = Guid.NewGuid(),
                        Hid = CurrentInfo.HotelId,
                        Itemid = model.ItemId,
                        ItemClassid = model.ItemClassidForEdit,
                        Remark = model.Remark,
                        Modified = DateTime.Now
                    };
                    entity.IsSubClass = item == null ? null : item.IsSubClass;
                    service.Add(entity);
                    service.Commit();
                }
                else
                {
                    //修改
                    var entity = service.Get(model.Id);
                    entity.ItemClassid = model.ItemClassidForEdit;
                    entity.Remark = model.Remark;
                    entity.Modified = DateTime.Now;
                    entity.IsSubClass = item == null ? null : item.IsSubClass;
                    service.Update(entity, new PosItemMultiClass());
                    service.Commit();
                }
            }
            return Json(JsonResultData.Successed());
        }

        [HttpPost]
        [AuthButton(AuthFlag.Delete)]
        public ActionResult MultiClass_Delete(string id)
        {
            var service = GetService<IPosItemMultiClassService>();
            _BatchDelete(id, GetService<IPosItemMultiClassService>(), OpLogType.角色删除);
            return Json(JsonResultData.Successed());
        }

        #endregion 消费项目对应大类操作

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            var service = GetService<IPosItemService>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的数据！"));
            }
            if (id == "0")
            {
                return Json(JsonResultData.Failure("要删除的数据不存在！"));
            }

            var itemNames = "";
            int itemCount = 0;
            foreach (var item in id.Split(','))
            {
                var result = service.IsExistsBillByItemId(CurrentInfo.HotelId, item);
                var model = service.Get(item);
                if (result)
                {
                    itemNames += model.Cname + ",";
                    /*model.Status = 51;//禁用状态
                    service.Update(model, new PosItem());
                    service.AddDataChangeLog(OpLogType.Pos消费项目启用禁用);
                    service.Commit();*/
                }
                else
                {
                    service.DeletePosItemOther(CurrentInfo.HotelId, model.Id);

                    service.Delete(model);
                    service.AddDataChangeLog(OpLogType.Pos消费项目删除);
                    service.Commit();
                    itemCount += 1;
                }
            }

            var retult = JsonResultData.Successed();
            if (!string.IsNullOrWhiteSpace(itemNames))
            {
                if (itemCount > 0)
                {
                    retult = JsonResultData.Successed($"删除成功,消费项目[{itemNames.TrimEnd(',')}]不能删除,只能禁用");
                }
                else
                {
                    retult = JsonResultData.Failure($"消费项目[{itemNames.TrimEnd(',')}]不能删除,只能禁用");
                }
            }

            return Json(retult);
            //return _BatchDelete(id, GetService<IPosItemService>(), OpLogType.Pos消费项目删除);
        }

        #endregion 批量删除

        #region 增加其他属性

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosItemOther(string id)
        {
            var itemService = GetService<IPosItemService>();
            PosItem item = itemService.Get(id);
            PosItemOtherViewModel model = new PosItemOtherViewModel();
            model.Id = item.Id;
            model.Code = item.Code;
            model.Cname = item.Cname;
            model.PosItemPrice.Itemid = model.Id;
            model.PosItemMultiClass.Itemid = model.Id;
            model.PosItemRefe.Itemid = model.Id;

            return PartialView("_AddPosItemOther", model);
        }

        #endregion 增加其他属性

        #region 消费项目对应单位价格

        /// <summary>
        /// 根据项目ID获取对应的单位价格列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemPriceByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemPriceService>();
            var list = service.GetPosItemPriceByItemId(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据项目ID返回单位价格增加单位价格视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult AddItemPrice(string id)
        {
            PosItemPriceAddViewModel viewModel = new PosItemPriceAddViewModel();
            viewModel.Itemid = id;
            return PartialView("_AddPosItemPrice", viewModel);
        }

        /// <summary>
        /// 根据单位价格id返回编辑单位价格视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public PartialViewResult EditItemPrice(Guid id)
        {
            PosItemPriceEditViewModel viewModel = new PosItemPriceEditViewModel();
            var service = GetService<IPosItemPriceService>();
            var entity = service.Get(id);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditPosItemPrice", viewModel);
        }

        #endregion 消费项目对应单位价格

        #region 消费项目对应项目大类

        /// <summary>
        /// 根据项目ID获取对应的单位价格列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemMultiClassByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemMultiClassService>();
            var list = service.GetPosItemMultiClassByItemId(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据项目ID返回对应项目大类增加对应项目大类视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult AddItemMultiClass(string id)
        {
            PosItemMultiClassAddViewModel viewModel = new PosItemMultiClassAddViewModel();
            viewModel.Itemid = id;
            return PartialView("_AddPosItemMultiClass", viewModel);
        }

        /// <summary>
        /// 根据对应项目大类id返回编辑对应项目大类视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public PartialViewResult EditItemMultiClass(Guid id)
        {
            PosItemMultiClassEditViewModel viewModel = new PosItemMultiClassEditViewModel();
            var service = GetService<IPosItemMultiClassService>();
            var entity = service.Get(id);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditPosItemMultiClass", viewModel);
        }

        #endregion 消费项目对应项目大类

        #region 消费项目对应营业点

        /// <summary>
        /// 根据项目ID获取对应的单位价格列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListItemRefeByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemRefeService>();
            var list = service.GetPosItemRefeByItemId(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 根据项目ID返回对应营业点增加对应营业点视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public PartialViewResult AddItemRefe(string id)
        {
            PosItemRefeAddViewModel viewModel = new PosItemRefeAddViewModel();
            viewModel.Itemid = id;
            return PartialView("_AddPosItemRefe", viewModel);
        }

        /// <summary>
        /// 根据对应营业点id返回编辑对应营业点视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public PartialViewResult EditItemRefe(Guid id)
        {
            PosItemRefeEditViewModel viewModel = new PosItemRefeEditViewModel();
            var service = GetService<IPosItemRefeService>();
            var entity = service.Get(id);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditPosItemRefe", viewModel);
        }

        #endregion 消费项目对应营业点

        #region 下拉数据绑定

        /// <summary>
        /// 获取指定酒店下的消费项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItem()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.D.ToString()).Where(m => m.Status == (int)PosItemStatus.提交).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定模块下的消费项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemByModules()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItemByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, PosItemDcFlag.D.ToString());
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定模块下的开台项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosOpenItemByModules()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosOpenItemByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, PosItemDcFlag.D.ToString(), true);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定模块下是否分类的消费项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemByIsSubClass(string itemClassid, bool isSubClass)
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItemByItemAndIsSubClass(CurrentInfo.HotelId, itemClassid, isSubClass, PosItemDcFlag.D.ToString()).Where(m => m.Status == 1).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定项目大类、分类下的项目代码(自增1)
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ContentResult ItemCodeForPosItemByClassid(string itemClassid, string subClassid)
        {
            var service = GetService<IPosItemService>();
            var itemCode = service.GetNewItemCodeByClassid(CurrentInfo.HotelId, itemClassid, subClassid, PosItemDcFlag.D.ToString());
            return Content(itemCode);
        }

        #endregion 下拉数据绑定

        #region 复制数据

        [HttpPost]
        [AuthButton(AuthFlag.None)]
        [JsonException]
        public ActionResult CopyPostItemOther(string id)
        {
            var itemService = GetService<IPosItemService>();
            PosItem item = itemService.Get(id);//old数据
            var model = new PosItem();//复制的数据

            AutoSetValueHelper.SetValues(item, model);
            //获取指定项目大类、分类下的项目代码(自增1)
            var maxCode = itemService.GetNewItemCodeByClassid(CurrentInfo.HotelId, item.ItemClassid, item.SubClassid, PosItemDcFlag.D.ToString());
            model.Id = CurrentInfo.HotelId + maxCode;
            model.Code = maxCode;
            model.Ename = "";
            model.Cname = "";
            model.Oname = "";
            ActionResult result = _Add(model, new PosItem { Id = CurrentInfo.HotelId + maxCode, Hid = CurrentInfo.HotelId, DcFlag = PosItemDcFlag.D.ToString(), OperName = CurrentInfo.UserName, ModifiedDate = DateTime.Now, Status = (byte)PosItemStatus.提交 }, OpLogType.Pos消费项目增加);

            #region 复制消费项目对应的大类

            var multiClassService = GetService<IPosItemMultiClassService>();
            var multiList = multiClassService.GetPosItemMultiClassByItemIdForCopy(CurrentInfo.HotelId, item.Id);
            //foreach (var multi in multiList)
            //{
            //    PosItemMultiClass ItemMultiModel = new PosItemMultiClass()
            //    {
            //        Id = Guid.NewGuid(),
            //        Hid = CurrentInfo.HotelId,
            //        Itemid = model.Id,
            //        ItemClassid = multi.ItemClassid,
            //        IsSubClass = multi.IsSubClass,
            //        Modified = multi.Modified,
            //        Remark = multi.Remark
            //    };
            //    multiClassService.Add(ItemMultiModel);
            //    multiClassService.AddDataChangeLog(OpLogType.Pos消费项目对应大类增加);
            //    multiClassService.Commit();
            //}

            #endregion 复制消费项目对应的大类

            #region 复制消费项目单位价格

            var posItemPriceService = GetService<IPosItemPriceService>();
            var posItemPriceList = posItemPriceService.GetPosItemPriceForCopy(CurrentInfo.HotelId, item.Id);
            foreach (var posItemPrice in posItemPriceList)
            {
                PosItemPrice posItemPriceModel = new PosItemPrice();

                AutoSetValueHelper.SetValues(posItemPrice, posItemPriceModel);
                posItemPriceModel.Id = Guid.NewGuid();
                posItemPriceModel.Itemid = model.Id;
                posItemPriceModel.Modified = DateTime.Now;
                posItemPriceService.Add(posItemPriceModel);
                posItemPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格增加);
                posItemPriceService.Commit();
            }

            #endregion 复制消费项目单位价格

            #region 复制消费项目对应的营业点

            var itemRefeService = GetService<IPosItemRefeService>();
            var itemRefeList = itemRefeService.GetPosItemRefeForCopy(CurrentInfo.HotelId, item.Id);
            foreach (var itemRefe in itemRefeList)
            {
                PosItemRefe posItemRefeModel = new PosItemRefe();
                AutoSetValueHelper.SetValues(itemRefe, posItemRefeModel);
                posItemRefeModel.Id = Guid.NewGuid();
                posItemRefeModel.Itemid = model.Id;
                posItemRefeModel.Modified = DateTime.Now;
                itemRefeService.Add(posItemRefeModel);
                itemRefeService.AddDataChangeLog(OpLogType.Pos消费项目对应营业点增加);
                itemRefeService.Commit();
            }

            #endregion 复制消费项目对应的营业点

            return Json(JsonResultData.Successed(model));
        }

        #endregion 复制数据

        #region 套餐酒席

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosItemSuitByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemSuitService>();
            var list = service.GetPosItemSuitListByItemId(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 添加套餐明细视图
        /// </summary>
        /// <param name="id">消费项目ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddPosItemSuit(string id)
        {
            PosItemSuitViewModel viewModel = new PosItemSuitViewModel();
            viewModel.ItemId = id;
            return PartialView("_AddPosItemSuit", viewModel);
        }

        /// <summary>
        /// 添加套餐明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosItemSuit(PosItemSuitViewModel model)
        {
            var id = Guid.NewGuid();
            var service = GetService<IPosItemSuitService>();

            if (string.IsNullOrEmpty(model.ItemId2))
            {
                return Json(JsonResultData.Failure("请选择套餐明细"));
            }
            if (string.IsNullOrEmpty(model.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }
            if (model.IGrade == null)
            {
                model.IGrade = 0;
            }

            var boolResult = service.IsExists(CurrentInfo.HotelId, model.ItemId, model.ItemId2, model.IGrade, model.Unitid);
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的套餐"));
            }
            if (model.IsAuto == true)
            {
                //如果勾选了是否自选，判断当前级别是否有其他的自选项目
                var list = service.GetPosItemSuitListByItemId(CurrentInfo.HotelId, model.ItemId).Where(m => m.IsAuto == true && m.IGrade == model.IGrade).ToList();
                if (list != null && list.Count > 0)
                {
                    return Json(JsonResultData.Failure("该级别下面已经有自选的套餐明细了"));
                }
            }
            var newModel = new PosItemSuit();
            AutoSetValueHelper.SetValues(model, newModel);
            //消费项目服务
            var itemService = GetService<IPosItemService>();

            //消费项目赋值
            var item = itemService.Get(model.ItemId);
            if (item != null)
            {
                newModel.ItemCode = item.Code;
            }

            var item2 = itemService.Get(model.ItemId2);
            if (item2 != null)
            {
                newModel.ItemCode2 = item2.Code;
                newModel.ItemName = item2.Cname;
            }

            //单位
            var unitService = GetService<IPosUnitService>();
            var unit = unitService.Get(model.Unitid);
            //单位赋值
            if (unit != null)
            {
                newModel.UnitCode = unit.Code;
            }
            newModel.Modifieddate = DateTime.Now;
            newModel.Id = id;
            newModel.Hid = CurrentInfo.HotelId;
            newModel.Amount = newModel.Quantity * newModel.Price;
            service.Add(newModel);
            service.AddDataChangeLog(OpLogType.Pos套餐明细添加);
            service.Commit();
            //重新计算金额、

            var itemSuitService = GetService<IPosItemSuitService>();
            itemSuitService.CalculationItemSuitAmount(CurrentInfo.HotelId, item.Id);

            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 修改套餐明细视图
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _EditPosItemSuit(string Id)
        {
            PosItemSuitViewModel viewModel = new PosItemSuitViewModel();
            var service = GetService<IPosItemSuitService>();
            var entity = service.Get(new Guid(Id));
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditPosItemSuit", viewModel);
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult EditPosItemSuit(PosItemSuitViewModel model)
        {
            var service = GetService<IPosItemSuitService>();

            var newModel = service.Get(model.Id);

            PosItemSuit originobj = model.GetOriginObject<PosItemSuit>();

            if (string.IsNullOrEmpty(model.ItemId2))
            {
                return Json(JsonResultData.Failure("请选择套餐明细"));
            }
            if (string.IsNullOrEmpty(model.Unitid))
            {
                return Json(JsonResultData.Failure("请选择单位"));
            }
            if (model.IGrade == null)
            {
                model.IGrade = 0;
            }

            var boolResult = service.IsExists(CurrentInfo.HotelId, model.ItemId, model.ItemId2, model.IGrade, model.Unitid, model.Id.ToString());
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的套餐"));
            }
            if (model.IsAuto == true)
            {
                //如果勾选了是否自选，判断当前级别是否有其他的自选项目
                var list = service.GetPosItemSuitListByItemId(CurrentInfo.HotelId, newModel.ItemId).Where(m => m.IsAuto == true && m.Id != model.Id && m.IGrade == model.IGrade).ToList();
                if (list != null && list.Count > 0)
                {
                    return Json(JsonResultData.Failure("该级别下面已经有自选的套餐明细了"));
                }
            }

            AutoSetValueHelper.SetValues(model, newModel);
            try
            {
                //消费项目服务
                var itemService = GetService<IPosItemService>();
                //消费项目赋值
                var item = itemService.Get(model.ItemId);
                if (item != null)
                {
                    newModel.ItemCode = item.Code;
                }

                var item2 = itemService.Get(model.ItemId2);
                if (item2 != null)
                {
                    newModel.ItemCode2 = item2.Code;
                    newModel.ItemName = item2.Cname;
                }

                //单位
                var unitService = GetService<IPosUnitService>();
                var unit = unitService.Get(model.Unitid);
                //单位赋值
                if (unit != null)
                {
                    newModel.UnitCode = unit.Code;
                }
                newModel.Amount = newModel.Price * newModel.Quantity;
                //ViewModel取出的originobj缺少Amount
                originobj.Amount = newModel.Price * newModel.Quantity;
                //newModel缺少Hid
                newModel.Hid = originobj.Hid;
                newModel.Modifieddate = DateTime.Now;
                service.Update(newModel, originobj);
                service.AddDataChangeLog(OpLogType.Pos套餐明细修改);
                service.Commit();

                //重新计算金额、

                var itemSuitService = GetService<IPosItemSuitService>();
                itemSuitService.CalculationItemSuitAmount(CurrentInfo.HotelId, item.Id);

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult BatchDeleteItemSuit(string id)
        {
            var service = GetService<IPosItemSuitService>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的数据！"));
            }
            if (id == "0")
            {
                return Json(JsonResultData.Failure("要删除的数据不存在！"));
            }
            foreach (var item in id.Split(','))
            {
                var model = service.Get(new Guid(item));

                service.Delete(model);
                service.AddDataChangeLog(OpLogType.Pos套餐明细删除);
                service.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 下拉框 套餐明细数据
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsBySuit()
        {
            var service = GetService<IPosItemService>();
            //不是分类，并且不是套餐跟酒席的
            var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.D.ToString())
                .Where(m => m.Status == 1 && m.IsSubClass == false && (m.IsSuite == false || m.IsSuite == null) && (m.IsFeast == false || m.IsFeast == null)).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListUnitByItem(string ItemId)
        {
            var service = GetService<IPosItemPriceService>();
            var datas = service.GetPosItemPriceByItemId(CurrentInfo.HotelId, ItemId).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Unitid, Text = w.Unit }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public ContentResult GetPriceByItemAndUnit(string ItemId, string unitId)
        {
            var service = GetService<IPosItemPriceService>();
            var datas = service.GetPosItemPriceByUnitid(CurrentInfo.HotelId, ItemId, unitId);
            if (datas != null)
            {
                return Content(datas.Price.ToString());
            }
            return Content("0");
        }

        #endregion 套餐酒席

        #region 消费项目对应作法

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosItemActionByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemActionService>();

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(id);
            byte type = item.IsSubClass == true ? (byte)PosItemActionType.分类 : (byte)PosItemActionType.消费项目;
            // PosItemActionType.大类
            var list = service.GetPosItemActionListByItemId(CurrentInfo.HotelId, id).Where(w => w.iType == type);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 添加消费项目对应作法视图
        /// </summary>
        /// <param name="id">消费项目ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddPosItemAction(string id, string iType)
        {
            PosItemActionAddViewModel viewModel = new PosItemActionAddViewModel();
            if (string.IsNullOrEmpty(iType))
            {
                var service = GetService<IPosItemService>();
                var item = service.Get(id);
                viewModel.iType = item.IsSubClass == true ? (byte)PosItemActionType.分类 : (byte)PosItemActionType.消费项目;
            }
            else
            {
                viewModel.iType = (byte)PosItemActionType.大类;
            }
            viewModel.Itemid = id;
            return PartialView("_AddPosItemAction", viewModel);
        }

        /// <summary>
        /// 添加消费项目对应作法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosItemAction(PosItemActionViewModel model)
        {
            var id = Guid.NewGuid();
            var service = GetService<IPosItemActionService>();

            if (string.IsNullOrEmpty(model.Actionid))
            {
                return Json(JsonResultData.Failure("请选择消费项目对应作法"));
            }

            var boolResult = service.IsExists(CurrentInfo.HotelId, model.Itemid, model.Actionid);
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的消费项目对应作法"));
            }

            if (!string.IsNullOrEmpty(Request["ProdPrinter"]))
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString();
            }

            var newModel = new PosItemAction();
            AutoSetValueHelper.SetValues(model, newModel);

            newModel.Modified = DateTime.Now;
            newModel.Id = id;
            newModel.Hid = CurrentInfo.HotelId;
            service.Add(newModel);
            service.AddDataChangeLog(OpLogType.Pos消费项目对应作法增加);
            service.Commit();

            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 修改消费项目对应作法视图
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _EditPosItemAction(string Id)
        {
            PosItemActionEditViewModel viewModel = new PosItemActionEditViewModel();
            var service = GetService<IPosItemActionService>();
            var entity = service.Get(new Guid(Id));
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditPosItemAction", viewModel);
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult EditPosItemAction(PosItemActionViewModel model)
        {
            var service = GetService<IPosItemActionService>();

            if (string.IsNullOrEmpty(model.Actionid))
            {
                return Json(JsonResultData.Failure("请选择消费项目对应作法"));
            }

            var newModel = service.Get(model.Id);
            var boolResult = service.IsExists(CurrentInfo.HotelId, model.Itemid, model.Actionid, model.Id);
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的消费项目对应作法"));
            }

            if (!string.IsNullOrEmpty(Request["ProdPrinter"]))
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString();
            }

            AutoSetValueHelper.SetValues(model, newModel);
            try
            {
                newModel.Modified = DateTime.Now;
                service.Update(newModel, new PosItemAction());
                service.AddDataChangeLog(OpLogType.Pos消费项目对应作法修改);
                service.Commit();

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult BatchDeleteItemAction(string id)
        {
            var service = GetService<IPosItemActionService>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的数据！"));
            }
            if (id == "0")
            {
                return Json(JsonResultData.Failure("要删除的数据不存在！"));
            }
            foreach (var item in id.Split(','))
            {
                var model = service.Get(new Guid(item));

                service.Delete(model);
                service.AddDataChangeLog(OpLogType.Pos消费项目对应作法删除);
                service.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        #endregion 消费项目对应作法

        #region 批量修改


        /// <summary>
        /// 批量修改
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult _EditAll()
        {
            return PartialView("_EditAll");
        }


        /// <summary>
        /// 查询消费项目所有数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosItemA([DataSourceRequest]DataSourceRequest request, string DeptId = "", string ItemClassid = "", string SubClassid = "", string CodeAndName = "")
        {
            var service = GetService<IPosItemService>();
            var list = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.D.ToString());
            list = list.Where(m => (m.DeptClassid == DeptId || string.IsNullOrEmpty(DeptId))
            && (m.ItemClassid == ItemClassid || string.IsNullOrEmpty(ItemClassid))
            && (m.SubClassid == SubClassid || string.IsNullOrEmpty(SubClassid))
            && ((m.Code.Contains(CodeAndName) || m.Cname.Contains(CodeAndName)) || string.IsNullOrEmpty(CodeAndName))).ToList();


            return new System.Web.Mvc.JsonResult()
            {
                Data = list.ToDataSourceResult(request),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }


        [AuthButton(AuthFlag.None)]
        public ActionResult EditItemAll(PosItemEditAllView model)
        {
            if (string.IsNullOrEmpty(model.itemIds))
            {
                return Json(JsonResultData.Failure("请选择需要修改的消费项目"));
            }
            var itemIdList = model.itemIds.Split('|');  //选中的消费项目ID

            //消费项目服务
            var itemService = GetService<IPosItemService>();

            //单位价格服务
            var itenPriceService = GetService<IPosItemPriceService>();

            //单位
            var unitService = GetService<IPosUnitService>();

            //消费项目对应大类
            var ItemMultiClassService = GetService<IPosItemMultiClassService>();

            //消费项目对应营业点
            var itemRefeService = GetService<IPosItemRefeService>();

            //营业点
            var posRefeService = GetService<IPosRefeService>();

            #region 处理其他设置
            //修改的对象
            PosItemOtherEdit itemOtherModel = new PosItemOtherEdit();
            PropertyInfo[] pis = itemOtherModel.GetType().GetProperties(); //获取所有公共属性(Public)


            // model.otherSelects = A:1|B:0|C:0
            if (!string.IsNullOrEmpty(model.otherSelects))
            {
                var otherSelectList = model.otherSelects.Split('|');


                for (int i = 0; i < otherSelectList.Length; i++)    //循环需要修改的字段
                {
                    if (!string.IsNullOrEmpty(otherSelectList[i]))
                    {
                        var otherval = otherSelectList[i].Split(':');
                        for (int j = 0; j < pis.Length; j++)
                        {
                            if (pis[j].Name == otherval[0])
                            {
                                pis[j] = itemOtherModel.GetType().GetProperty(otherval[0]);
                                if (otherval[1] == "1")
                                {
                                    pis[j].SetValue(itemOtherModel, true, null);
                                }
                                else
                                {
                                    pis[j].SetValue(itemOtherModel, false, null);
                                }

                            }

                        }
                    }

                }
            }

            #endregion

            #region 处理单位价格

            List<PosItemPriceEditAll> ItemPriceList = new List<PosItemPriceEditAll>();
            if (!string.IsNullOrEmpty(model.unitVal))
            {
                var unitStringArr = model.unitVal.Split('|');
                foreach (var itemPrice in unitStringArr)
                {

                    if (!string.IsNullOrEmpty(itemPrice))
                    {
                        var ItemPriceEditAllModel = new PosItemPriceEditAll();
                        /*默认数组 索引0：价格，1:差价，2:单位ID */
                        var priceA = itemPrice.Split('_');
                        if (!string.IsNullOrEmpty(priceA[2]))
                        {
                            var unitModel = unitService.Get(priceA[2]);
                            if (unitModel != null)
                            {
                                ItemPriceEditAllModel.unitCode = unitModel.Code;
                                ItemPriceEditAllModel.unitId = unitModel.Id;
                                ItemPriceEditAllModel.unitName = unitModel.Cname;
                            }
                        }
                        ItemPriceEditAllModel.differencePrice = string.IsNullOrEmpty(priceA[1]) ? 0 : Convert.ToDecimal(priceA[1]);
                        ItemPriceEditAllModel.price = string.IsNullOrEmpty(priceA[0]) ? 0 : Convert.ToDecimal(priceA[0]);

                        ItemPriceList.Add(ItemPriceEditAllModel);
                    }
                }
            }


            #endregion

            #region 处理项目大类

            List<PosItemClassEditAll> itemClassList = new List<PosItemClassEditAll>();
            if (!string.IsNullOrEmpty(model.itemClassVal))
            {
                var itemClassArr = model.itemClassVal.Split('|');
                foreach (var itemClass in itemClassArr)
                {
                    if (!string.IsNullOrEmpty(itemClass))
                    {
                        var PosItemClassEditAllModel = new PosItemClassEditAll();
                        var itemClassA = itemClass.Split('_');
                        if (itemClassA.Length > 1)
                        {
                            //0:原项目大类ID，1：现项目大类ID
                            if (!string.IsNullOrEmpty(itemClassA[1]))
                            {
                                var itemModel = itemService.Get(itemClassA[1]);
                                if (itemModel != null)
                                {
                                    PosItemClassEditAllModel.IsSubClass = itemModel.IsSubClass;
                                }
                                PosItemClassEditAllModel.itemClassId = itemClassA[0];
                                PosItemClassEditAllModel.newItemClassId = itemClassA[1];
                            }
                        }

                        itemClassList.Add(PosItemClassEditAllModel);
                    }
                }
            }
            #endregion

            #region 处理对应营业点
            List<PosItemRefeEditAll> refeList = new List<PosItemRefeEditAll>();
            if (!string.IsNullOrEmpty(model.refeVal))
            {
                var refeArr = model.refeVal.Split('|');
                foreach (var itemRefe in refeArr)
                {
                    if (!string.IsNullOrEmpty(itemRefe))
                    {
                        var refeModel = new PosItemRefeEditAll();
                        var itemRefeA = itemRefe.Split('_');
                        if (itemRefeA.Length > 0)
                        {

                            refeModel.refeId = itemRefeA[0];
                            refeModel.Shuffleid = itemRefeA[1];
                            refeModel.ProdPrinter = itemRefeA[2].Replace(",", "");
                            refeModel.SentPrtNo = itemRefeA[3] == "null" ? "" : itemRefeA[3].Replace(",", "");
                        }

                        refeList.Add(refeModel);
                    }
                }
            }
            #endregion

            #region 处理消费项目信息
            foreach (var itemId in itemIdList)
            {
                var itemModel = itemService.Get(itemId);
                if (itemModel != null)
                {
                    var targrtItemModel = new PosItem();
                    itemModel.ShowSet = "";
                    if (!string.IsNullOrEmpty(model.otherSelects))
                    {
                        AutoSetValueHelper.SetValues(itemModel, targrtItemModel);

                        //需要修改的字段赋值给新对象
                        TypeValue(targrtItemModel, itemOtherModel);

                        //微信显示
                        if (targrtItemModel.IsWxShow == true)
                        {
                            if (!targrtItemModel.ShowSet.Contains("PC"))
                            {
                                targrtItemModel.ShowSet += "PC";
                            }

                            if (!targrtItemModel.ShowSet.Contains("Mobile"))
                            {
                                targrtItemModel.ShowSet += ",Mobile";
                            }
                        }

                        //IPad显示
                        if (targrtItemModel.IsPaidShow == true)
                        {
                            if (!targrtItemModel.ShowSet.Contains("PC"))
                            {
                                targrtItemModel.ShowSet += "PC";
                            }

                            if (!targrtItemModel.ShowSet.Contains("Ipad"))
                            {
                                targrtItemModel.ShowSet += ",Ipad";
                            }
                        }

                        itemService.Update(targrtItemModel, itemModel);

                        itemService.AddDataChangeLog(OpLogType.Pos消费项目修改);
                        itemService.Commit();

                    }


                    #region 单位价格
                    foreach (var ItemPrice in ItemPriceList)
                    {
                        var oldItemPriceModel = itenPriceService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, itemModel.Id, ItemPrice.unitId);

                        if (model.isUnitStatus == "add") //新增单位价格
                        {

                            if (oldItemPriceModel == null)
                            {
                                var newItemPriceModel = new PosItemPrice()
                                {
                                    Hid = CurrentInfo.HotelId,
                                    Itemid = itemModel.Id,
                                    Unitid = ItemPrice.unitId,
                                    UnitCode = ItemPrice.unitCode,
                                    Unit = ItemPrice.unitName,
                                    Price = ItemPrice.price,
                                    Modified = DateTime.Now,
                                    Id = Guid.NewGuid()

                                };
                                itenPriceService.Add(newItemPriceModel);
                                itenPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格增加);
                                itenPriceService.Commit();
                            }

                        }
                        else
                        {
                            //修改单价（判断该项目是否存在单位价格数据），不存在不做任何修改，存在修改对应的数据
                            if (oldItemPriceModel != null)
                            {
                                var newItemPriceModel = new PosItemPrice();
                                AutoSetValueHelper.SetValues(oldItemPriceModel, newItemPriceModel);
                                if (ItemPrice.differencePrice == 0)
                                {
                                    newItemPriceModel.Price = ItemPrice.price;
                                }
                                else
                                {
                                    newItemPriceModel.Price += ItemPrice.differencePrice;
                                }

                                newItemPriceModel.Modified = DateTime.Now;
                                itenPriceService.Update(newItemPriceModel, oldItemPriceModel);
                                itenPriceService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                                itenPriceService.Commit();
                            }
                        }
                    }
                    #endregion

                    #region 对应大类
                    foreach (var itemClass in itemClassList)
                    {

                        if (model.isItemClassStatus == "add") //新增
                        {

                            var itemMultiClassModel = ItemMultiClassService.GetPosItemMultiClassByItemEditAll(CurrentInfo.HotelId, itemModel.Id, itemClass.newItemClassId);
                            if (itemMultiClassModel == null)
                            {
                                var newItemMultiClassModel = new PosItemMultiClass()
                                {
                                    Itemid = itemModel.Id,
                                    ItemClassid = itemClass.newItemClassId,
                                    Id = Guid.NewGuid(),
                                    Hid = CurrentInfo.HotelId,
                                    IsSubClass = itemClass.IsSubClass
                                };

                                ItemMultiClassService.Add(newItemMultiClassModel);
                                ItemMultiClassService.AddDataChangeLog(OpLogType.Pos消费项目大类增加);
                                ItemMultiClassService.Commit();
                            }
                        }
                        else
                        {
                            var itemMultiClassModel = ItemMultiClassService.GetPosItemMultiClassByItemEditAll(CurrentInfo.HotelId, itemModel.Id, itemClass.itemClassId);
                            var newItemMultiClassModel = new PosItemMultiClass();
                            if (itemMultiClassModel != null)
                            {
                                AutoSetValueHelper.SetValues(itemMultiClassModel, newItemMultiClassModel);
                                newItemMultiClassModel.ItemClassid = itemClass.newItemClassId;
                                newItemMultiClassModel.IsSubClass = itemClass.IsSubClass;
                                ItemMultiClassService.Update(newItemMultiClassModel, itemMultiClassModel);
                                ItemMultiClassService.AddDataChangeLog(OpLogType.Pos消费项目大类修改);
                                ItemMultiClassService.Commit();
                            }
                        }
                    }
                    #endregion

                    #region 对应营业点

                    foreach (var refe in refeList)
                    {
                        var oldItemRefeModel = itemRefeService.GetPosItemRefeByEditAll(CurrentInfo.HotelId, itemModel.Id, refe.refeId);
                        var posRefe = posRefeService.Get(refe.refeId);
                        if (model.isRefeStatus == "add")  //新增
                        {
                            if (oldItemRefeModel == null)
                            {

                                var newItemrefeModel = new PosItemRefe()
                                {
                                    Id = Guid.NewGuid(),
                                    Hid = CurrentInfo.HotelId,
                                    Itemid = itemModel.Id,
                                    Refeid = refe.refeId,
                                    ProdPrinter = refe.ProdPrinter,
                                    SentPrtNo = refe.SentPrtNo,
                                    Shuffleid = refe.Shuffleid,
                                    Modified = DateTime.Now
                                };
                                if (posRefe != null)
                                {
                                    newItemrefeModel.RefeName = posRefe.Cname;
                                }
                                itemRefeService.Add(newItemrefeModel);
                                itemRefeService.AddDataChangeLog(OpLogType.Pos消费项目对应营业点增加);
                                itemRefeService.Commit();
                            }

                        }
                        else
                        {
                            if (oldItemRefeModel != null)
                            {
                                var newItemrefeModel = new PosItemRefe();
                                AutoSetValueHelper.SetValues(oldItemRefeModel, newItemrefeModel);

                                newItemrefeModel.Refeid = refe.refeId;
                                newItemrefeModel.Shuffleid = refe.Shuffleid;
                                newItemrefeModel.SentPrtNo = refe.SentPrtNo;
                                newItemrefeModel.ProdPrinter = refe.ProdPrinter;
                                newItemrefeModel.Modified = DateTime.Now;

                                itemRefeService.Update(newItemrefeModel, oldItemRefeModel);
                                itemRefeService.AddDataChangeLog(OpLogType.Pos消费项目对应营业点修改);
                                itemRefeService.Commit();
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion


            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 赋值，对象2赋值给对象1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oneT">对象1</param>
        /// <param name="twoT">对象2</param>
        private void TypeValue<T, T1>(T oneT, T1 twoT)
        {
            Type typeOne = oneT.GetType();
            Type typeTwo = twoT.GetType();

            PropertyInfo[] pisOne = typeOne.GetProperties(); //获取所有公共属性(Public)
            PropertyInfo[] pisTwo = typeTwo.GetProperties();

            for (int i = 0; i < pisOne.Length; i++)
            {
                //获取属性名
                string oneName = pisOne[i].Name;
                //获取属性的值
                object oneValue = pisOne[i].GetValue(oneT, null);
                for (int j = 0; j < pisTwo.Length; j++)
                {
                    string twoName = pisTwo[j].Name;
                    object twoValue = pisTwo[j].GetValue(twoT, null);

                    if (oneName == twoName && twoValue != null)
                    {
                        pisOne[i].SetValue(oneT, twoValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// 查询条件视图
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _QueryEditAll()
        {
            return PartialView("_QueryEditAll");
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult QueryEditAll()
        {
            return Json(JsonResultData.Successed());
        }
        #endregion

        #region 消费项目选择分类查询出分类的


        [AuthButton(AuthFlag.None)]
        public ActionResult GetItemOtherList(string itemId)
        {
            var itemService = GetService<IPosItemService>();

            List<string> list = new List<string>();
            var itemModel = itemService.Get(itemId);
            if (itemModel != null)
            {
                var otherModel = new PosItemOtherEdit();

                TypeValueA(otherModel, itemModel);


                PropertyInfo[] pisOne = otherModel.GetType().GetProperties();
                for (int i = 0; i < pisOne.Length; i++)
                {
                    //获取属性名
                    string oneName = pisOne[i].Name;
                    //获取属性的值
                    object oneValue = pisOne[i].GetValue(otherModel, null);
                    if (oneValue != null)
                    {
                        if (!oneValue.Equals(false))
                        {
                            list.Add(oneName);
                        }
                    }
                }

            }
            if (list.Count > 0)
            {
                return Json(JsonResultData.Successed(list));
            }
            else
            {
                return Json(JsonResultData.Failure(""));
            }
        }

        private void TypeValueA<T, T1>(T oneT, T1 twoT)
        {
            Type typeOne = oneT.GetType();
            Type typeTwo = twoT.GetType();

            PropertyInfo[] pisOne = typeOne.GetProperties(); //获取所有公共属性(Public)
            PropertyInfo[] pisTwo = typeTwo.GetProperties();

            for (int i = 0; i < pisOne.Length; i++)
            {
                //获取属性名
                string oneName = pisOne[i].Name;
                //获取属性的值
                object oneValue = pisOne[i].GetValue(oneT, null);
                for (int j = 0; j < pisTwo.Length; j++)
                {
                    string twoName = pisTwo[j].Name;
                    object twoValue = pisTwo[j].GetValue(twoT, null);

                    if (oneName == twoName)
                    {
                        pisOne[i].SetValue(oneT, twoValue, null);
                    }
                }
            }
        }
        #endregion

        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {

            var service = GetService<IPosItemService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用, OpLogType.Pos消费项目启用禁用));
            return reval;
        }
        #endregion

        #region 禁用
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {

            var service = GetService<IPosItemService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用, OpLogType.Pos消费项目启用禁用));

            return reval;

        }
        #endregion

        #region 消费项目显示
        [AuthButton(AuthFlag.None)]
        public JsonResult ListShowSet()
        {
            var service = GetService<ICodeListService>();
            var datas = service.GetShowSetList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取发票项目列表
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsPosInvoiceItem()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List(CurrentInfo.HotelId, "66");
            // var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.D.ToString()).Where(m => m.Status == (int)PosItemStatus.提交).ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}