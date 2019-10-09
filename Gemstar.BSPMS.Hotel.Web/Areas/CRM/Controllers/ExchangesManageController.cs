using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.ExchangesManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Common.Services.EF;
using System.Transactions;
using Gemstar.BSPMS.Common.Services.Entities;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using System.Text;
using Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 合约单位往来管理
    /// </summary>
    [AuthPage("60030003")]
    public class ExchangesManageController : BaseEditInWindowController<CompanyCa, ICompanyCaService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_companyForCa", "");
            return View();
        }
        #endregion

        #region 详情
        [AuthButton(AuthFlag.Details)]
        public ActionResult Edit(string id)
        {
            JsonResultData result = Check(id);
            if (!result.Success) { return Json(result, JsonRequestBehavior.AllowGet); }
            ViewBag.CompanyId = id; 
            Company entity = GetService<ICompanyService>().Get(Guid.Parse(id));
            ViewBag.CompanyCode = entity.Code;
            ViewBag.CompanyName = entity.Name;
            ViewBag.IsGroup = CurrentInfo.IsGroup;
       ///     ViewBag.JoinDate=entity.joinda
            return PartialView("_Detail");
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string id, string type)
        {
            JsonResultData result = Check(id, type);
            if (!result.Success) { return Json(result, JsonRequestBehavior.AllowGet); }

            Company entity = GetService<ICompanyService>().Get(Guid.Parse(id));
            ViewBag.Type = type;
            return _Add(new ExchangesAddViewModel()
            {
                CompanyId = entity.Id,
                CompanyName = entity.Name,
                Payable = entity.Balance,
                Type = type,
                Outletcode="01"
            });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ExchangesAddViewModel exchangesViewModel)
        {
            if (exchangesViewModel == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            JsonResultData result = Check(exchangesViewModel.CompanyId.ToString(), exchangesViewModel.Type, exchangesViewModel.ItemId);
            if (!result.Success) { return Json(result); }

            string Name = exchangesViewModel.Type == "D" ? "调账" : "收款";
            var _itemService = GetService<IItemService>().GetCodeListPub("16");
            var entity = _itemService.Where(c => c.name == Name).FirstOrDefault();
            if (entity == null)
            {
                return Json(JsonResultData.Failure("交易类型错误。"));
            }
            if (string.IsNullOrWhiteSpace(exchangesViewModel.Remark))
            {
                exchangesViewModel.Remark = "";
            }

            OpLogType logType = exchangesViewModel.Type == "D" ? OpLogType.合约往来入账 : OpLogType.合约往来收款;
            JsonResultData addResult;
            ResFolioAddResult returnResult = null;
            if (exchangesViewModel.Type == "D")
            {
                //入账
                addResult = GetService<ICompanyCaService>().AddProc(exchangesViewModel.CompanyId, entity.code, exchangesViewModel.Amount, exchangesViewModel.ItemId, exchangesViewModel.Invno, exchangesViewModel.Remark, exchangesViewModel.Outletcode);
            }
            else
            {
                //付款
                #region
                try
                {
                    var payServiceBuilder = GetService<IPayServiceBuilder>();
                    var commonDb = GetService<DbCommonContext>();
                    var pmsParaService = GetService<IPmsParaService>();

                    var commonPayParas = commonDb.M_v_payParas.ToList();
                    var hotelPayParas = pmsParaService.GetPmsParas(CurrentInfo.HotelId);

                    var payService = payServiceBuilder.GetPayService(exchangesViewModel.FolioItemAction, commonPayParas, hotelPayParas, MvcApplication.IsTestEnv);
                    using (var tc = new TransactionScope())
                    {
                        //如果是付款，则获取支付服务实例，进行支付，并且将支付成功后返回的交易号保存到refno中
                        var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                        if (payService != null)
                        {
                            if (string.IsNullOrWhiteSpace(exchangesViewModel.FolioItemActionJsonPara))
                            {
                                return Json(JsonResultData.Failure("参数不能为空"));
                            }
                            payResult = payService.DoPayBeforeSaveFolio(exchangesViewModel.FolioItemActionJsonPara);
                        }
                        if (payResult.IsWaitPay)
                        {
                            //如果是待支付的，则不调用充值的存储过程，而是增加一待支付的记录，等到支付成功回调处理中再调用存储过程来真实增加余额
                            var waitPay = new WaitPayList
                            {
                                WaitPayId = Guid.NewGuid(),
                                CreateDate = DateTime.Now,
                                ProductType = PayProductType.CorpReceive.ToString(),
                                Status = 0
                            };
                            var businessPara = new
                            {
                                hotelId = CurrentInfo.HotelId,
                                userName = CurrentInfo.UserName,
                                companyid = exchangesViewModel.CompanyId,
                                type = entity.code,
                                amount = exchangesViewModel.Amount,
                                itemid = exchangesViewModel.ItemId,
                                invno = exchangesViewModel.Invno,
                                remark = exchangesViewModel.Remark,
                                outletcode = exchangesViewModel.Outletcode,
                                refno = payResult.RefNo,
                            };
                            var serializer = new JavaScriptSerializer();
                            waitPay.BusinessPara = serializer.Serialize(businessPara);
                            var waitPayService = GetService<IWaitPayListService>();
                            waitPayService.Add(waitPay);
                            waitPayService.Commit();
                            logType = OpLogType.合约往来收款待支付;
                            addResult = JsonResultData.Successed(waitPay.WaitPayId);
                        }
                        else
                        {
                            addResult = GetService<ICompanyCaService>().AddProc(exchangesViewModel.CompanyId, entity.code, exchangesViewModel.Amount, exchangesViewModel.ItemId, exchangesViewModel.Invno, exchangesViewModel.Remark, exchangesViewModel.Outletcode, payResult.RefNo);
                        }
                        tc.Complete();
                    }
                    //记录日志
                    if (addResult.Success)
                    {
                        returnResult = new ResFolioAddResult
                        {
                            FolioTransId = addResult.Data.ToString(),
                            Statu = PayStatu.Successed.ToString(),
                            Callback = "",
                            QrCodeUrl = "",
                            QueryTransId = "",
                            DCFlag = "c"
                        };
                        if (payService != null)
                        {
                            //转换一下folio的transid格式，以保证长度为32位
                            var transId = Guid.Parse(addResult.Data.ToString()).ToString("N");
                            var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.CorpReceive, transId, exchangesViewModel.FolioItemActionJsonPara);
                            returnResult.Statu = afterPayResult.Statu.ToString();
                            returnResult.Callback = afterPayResult.Callback;
                            returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                            returnResult.QueryTransId = afterPayResult.QueryTransId;
                        }

                        //return Json(JsonResultData.Successed(returnResult));
                    }
                    //else
                    //{
                    //    return Json(addResult);
                    //}
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
                #endregion
            }

            #region 入账收款日志
            if(addResult != null && addResult.Success)
            {
                var items = GetService<IItemService>().GetItem(CurrentInfo.HotelId, exchangesViewModel.Type);
                var item = items.Where(w => w.Id == exchangesViewModel.ItemId).FirstOrDefault();
                var company = GetService<ICompanyService>().Get(exchangesViewModel.CompanyId);
                AddOperationLog(logType, string.Format("合约单位代码：{0}，名称：{1}，{6}：{2}，金额：{3}，单号：{4}，备注{5}",
                     company==null?"": company.Code,
                     company==null?"": company.Name,
                     item == null ? "" : item.Name,
                     exchangesViewModel.Amount,
                     exchangesViewModel.Invno,
                     exchangesViewModel.Remark,
                     exchangesViewModel.Type == "D" ?"消费项目":"付款方式"
                    ));
                return Json(JsonResultData.Successed(returnResult));
            }
            #endregion

            return Json(addResult);
        }
        #endregion

        #region 转账
        [AuthButton(AuthFlag.Add)]
        public ActionResult TransferAccounts(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Content("转账ID不存在");
            }
            #region 多个转账
            var list= id.Split(',').ToList();
            if (list.Count() > 1)
            {
                var cList = GetService<ICompanyCaService>().GetCompanCaList(list);
                if (cList.Count <= 0 || cList == null)
                {
                    return Content("转账ID不存在");
                }
                var companCa = cList.FirstOrDefault();
                var compan = GetService<ICompanyService>().Get(companCa.Companyid);
                return PartialView("_TransferAccounts", new TransferAccountsViewModel()
                {
                    CompanyId = compan.Id,
                    CompanyName = compan.Name,
                    Id =string.Join("," ,cList.Select(s=>s.Id.ToString())),
                    Amount = cList.Select(s => s.Amount).Sum(),
                    Type = companCa.Dcflag,
                    IsBatch=true
                });
            }
            #endregion
            Guid ID = new Guid();
            if (string.IsNullOrWhiteSpace(id) || (!Guid.TryParse(id, out ID)))
            {
                return Content("转账ID不存在");
            }
            var entityCompanyCa = GetService<ICompanyCaService>().Get(ID);
            if(entityCompanyCa == null || entityCompanyCa.Hid != CurrentInfo.HotelId)
            {
                return Content("转账信息不存在");
            }
            var entityCompany = GetService<ICompanyService>().Get(entityCompanyCa.Companyid);
            if (entityCompany == null || entityCompany.Hid != CurrentInfo.HotelId)
            {
                return Content("转账信息不存在");
            }
            var entityItem = GetService<IItemService>().Get(entityCompanyCa.Itemid);
            return PartialView("_TransferAccounts", new TransferAccountsViewModel()
            {
                ItemName =entityItem==null?"": entityItem.Name,
                Payable = entityCompany.Balance,
                CompanyId = entityCompany.Id,
                CompanyName = entityCompany.Name,
                Id = entityCompanyCa.Id.ToString(),
                Amount = entityCompanyCa.Amount,
                Invno = entityCompanyCa.Invno,
                Remark = entityCompanyCa.Remark,
                Type = entityCompanyCa.Dcflag,
                IsBatch=false
            });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult TransferAccounts(TransferAccountsViewModel transferAccountsViewModel)
        {
            string ErrorMessage = "错误信息，请关闭后重试。";
            if (string.IsNullOrWhiteSpace(transferAccountsViewModel.Id))
                return Json(JsonResultData.Failure(ErrorMessage));
            var id = transferAccountsViewModel.Id.Split(',').ToList();
            var companCaid = Guid.Parse(id[0]);
            var entityCompanyCa = GetService<ICompanyCaService>().Get(companCaid);
            if (entityCompanyCa == null || entityCompanyCa.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure(ErrorMessage));
            }
            Guid ToCompanyId = new Guid();
            if (string.IsNullOrWhiteSpace(transferAccountsViewModel.ToCompanyId) || (!Guid.TryParse(transferAccountsViewModel.ToCompanyId, out ToCompanyId)))
            {
                return Json(JsonResultData.Failure("请选择合约单位。"));
            }
            var entityCompany = GetService<ICompanyService>().Get(ToCompanyId);
            if (entityCompany == null || entityCompany.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure(ErrorMessage));
            }
            if(entityCompanyCa.Companyid == entityCompany.Id)
            {
                return Json(JsonResultData.Failure(ErrorMessage));
            }
            var company = GetService<ICompanyService>().Get(entityCompanyCa.Companyid);
            GetService<ICompanyCaService>().TransferAccounts(id, ToCompanyId, transferAccountsViewModel.ToRemark,company.Name);
            #region 转账日志
           
            AddOperationLog(OpLogType.合约往来转账, string.Format("合约单位代码：{0}，名称：{1}，{9}：{2}，{10}：{3}，单号：{4}，备注：{5}，转到的合约单位代码：{6}，名称：{7}，转账备注：{8}",
                company==null?"":company.Code,
                company==null?"":company.Name,
                transferAccountsViewModel.ItemName,
                transferAccountsViewModel.Amount,
                transferAccountsViewModel.Invno,
                transferAccountsViewModel.Remark,
                entityCompany==null?"": entityCompany.Code,
                entityCompany==null?"": entityCompany.Name,
                transferAccountsViewModel.ToRemark,
                transferAccountsViewModel.Type=="D" ? "消费项目" : "付款方式",
                transferAccountsViewModel.Type=="D" ? "消费金额" : "付款金额"
                ));
            #endregion
            return Json(JsonResultData.Successed(""));
        }
        #endregion

        #region 拆账
        [AuthButton(AuthFlag.Add)]
        public ActionResult SplitAccounts(string id)
        {
            Guid ID = new Guid();
            if (string.IsNullOrWhiteSpace(id) || (!Guid.TryParse(id, out ID)))
            {
                return Content("拆账ID不存在");
            }
            var entityCompanyCa = GetService<ICompanyCaService>().Get(ID);
            if (entityCompanyCa == null || entityCompanyCa.Hid != CurrentInfo.HotelId)
            {
                return Content("拆账信息不存在");
            }
            var entityCompany = GetService<ICompanyService>().Get(entityCompanyCa.Companyid);
            if (entityCompany == null || entityCompany.Hid != CurrentInfo.HotelId)
            {
                return Content("拆账信息不存在");
            }
            var entityItem = GetService<IItemService>().Get(entityCompanyCa.Itemid);
            return PartialView("_SplitAccounts", new SplitAccountsViewModel()
            {
                ItemName =entityItem==null?"": entityItem.Name,
                Payable = entityCompany.Balance,
                CompanyId = entityCompany.Id,
                CompanyName = entityCompany.Name,
                Id = entityCompanyCa.Id,
                Amount = entityCompanyCa.Amount,
                Invno = entityCompanyCa.Invno,
                Remark = entityCompanyCa.Remark,
                Type = entityCompanyCa.Dcflag
            });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult SplitAccounts(SplitAccountsViewModel splitAccountsViewModel)
        {
            string ErrorMessage = "错误信息，请关闭后重试。";
            var entityCompanyCa = GetService<ICompanyCaService>().Get(splitAccountsViewModel.Id);
            if (entityCompanyCa == null || entityCompanyCa.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure(ErrorMessage));
            }
            GetService<ICompanyCaService>().SplitAccounts(splitAccountsViewModel.Id, splitAccountsViewModel.SplitAmount, splitAccountsViewModel.SplitRemark);
            #region 拆账日志
            var type = splitAccountsViewModel.Type == "D";
            var company = GetService<ICompanyService>().Get(splitAccountsViewModel.CompanyId);
            AddOperationLog(OpLogType.合约往来拆账,string.Format("{0}，合约单位代码：{1}，名称{2}，{3}：{4}，{5}：{6}，拆账金额：{7}，单号：{8}，备注：{9}，拆账备注：{10}",
                 type?"挂账拆账":"付款拆账",
                 company==null?"":company.Code,
                 company==null?"":company.Name,
                 type?"消费项目":"付款方式",
                 splitAccountsViewModel.ItemName,
                 type?"消费金额":"付款金额",
                 splitAccountsViewModel.Amount,
                 splitAccountsViewModel.SplitAmount,
                 splitAccountsViewModel.Invno,
                 splitAccountsViewModel.Remark,
                 splitAccountsViewModel.SplitRemark
                ));
            #endregion
            return Json(JsonResultData.Successed(""));
        }
        #endregion

        #region 核销
        [AuthButton(AuthFlag.Add)]
        [HttpPost]
        [JsonException]
        public JsonResult CancelAfterVerification(string ids)
        {
            var data = GetService<ICompanyCaService>().CancelAfterVerification(ids);
            if (data.Success) {
                //记录核销日志
                var result=data.Data as Dictionary<string,string>;
                AddOperationLog(OpLogType.合约往来核销, string.Format("合约单位代码：{0}，名称：{1}，挂账金额：{2}，付款金额：{3}",
                      result["code"],
                      result["name"],
                      result["amout"],
                      result["amout"]
                    ));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        
        #region 下拉绑定
        [AuthButton(AuthFlag.Details)]
        public JsonResult GetItemSelectList(string type)
        {
            if (type != "C" && type != "D")
            {
                return Json(JsonResultData.Failure("类型错误"), JsonRequestBehavior.AllowGet);
            }
            var _itemService = GetService<IItemService>();
            var data = _itemService.GetItem(CurrentInfo.HotelId, type).Where(w=>w.Action!= "corp" && w.IsInput==true).ToList();
            if (data != null && data.Count > 0)
            {
                //消费入账权限控制
                var itemids = GetService<IRoleAuthItemConsumeService>().GetItemConsumeAuth(CurrentInfo.HotelId, CurrentInfo.UserId);
                if (itemids != null && itemids.Count > 0)
                {
                    var removeItemids = itemids.Where(c => c.Value == false).Select(c => c.Key).ToList();
                    if (removeItemids != null && removeItemids.Count > 0)
                    {
                        data.RemoveAll(c => removeItemids.Contains(c.Id));
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        public JsonResult GetCodeList()
        {
            var codeList = GetService<ICodeListService>().List("22");
            return Json(codeList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 其他
        public JsonResultData Check(string companyId)
        {
            string ErrorMessage = "错误信息，请关闭后重试。";
            Guid ID = new Guid();
            if (string.IsNullOrWhiteSpace(companyId) || (!Guid.TryParse(companyId, out ID)))
            {
                return JsonResultData.Failure(ErrorMessage);
            }
            Company entity = GetService<ICompanyService>().Get(ID);
            if (entity == null || entity.Hid != CurrentInfo.HotelId)
            {
                return JsonResultData.Failure(ErrorMessage);
            }
            return JsonResultData.Successed();
        }
        public JsonResultData Check(string companyId, string type)
        {
            JsonResultData result = Check(companyId);
            if (!result.Success) { return result; }

            if (type != "C" && type != "D")
            {
                return JsonResultData.Failure("交易类型错误。");
            }
            return JsonResultData.Successed();
        }
        public JsonResultData Check(string companyId, string type, string itemId)
        {
            JsonResultData result = Check(companyId, type);
            if (!result.Success) { return result; }

            if (string.IsNullOrWhiteSpace(itemId))
            {
                return JsonResultData.Failure(string.Format("请选择{0}", (type == "C" ? "付款方式" : "项目")));
            }
            var itemEntity = GetService<IItemService>().Get(itemId);
            if (itemEntity == null || itemEntity.Hid != CurrentInfo.HotelId || itemEntity.DcFlag != type && itemEntity.Status != EntityStatus.启用)
            {
                return JsonResultData.Failure((type == "C" ? "付款方式" : "项目") + "错误。");
            }
            return JsonResultData.Successed();
        }
        #endregion

        #region 查询核销记录
        [AuthButton(AuthFlag.Query)]
        public ActionResult CancelRecord(string id)
        {
            ViewBag.companyId = id;
            return PartialView("_CancelRecord");
        }
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetCancelRecord([DataSourceRequest] DataSourceRequest request,string companyId,string dateBegin,string dateEnd)
        {
            var service = GetService<ICompanyCaService>();
            var data = service.CancelRecord(CurrentInfo.HotelId, companyId, dateBegin, dateEnd);
            return Json(data.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  取消核销
        [AuthButton(AuthFlag.Add)]
        public ActionResult CancelRecordLog(string checkNo)
        {            
            try
            {
                var ids = GetService<ICompanyCaService>().GetCancelRecordByCheckNo(Guid.Parse(checkNo));
                if (string.IsNullOrWhiteSpace(ids))
                    return Json(JsonResultData.Failure("核销id不存在"));
                var data = GetService<ICompanyCaService>().CancelRecordLog(ids);
                //记录取消核销日志
                var result = data.Data as Dictionary<string, string>;
                AddOperationLog(OpLogType.合约往来取消核销, string.Format("合约单位代码：{0}，名称：{1}，挂账金额：{2}，付款金额:{3}",
                      result["code"],
                      result["name"],
                      result["amount"],
                      result["amount"]
                    ));
                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure("取消核销失败"));
            }
        }
        #endregion
    }
}