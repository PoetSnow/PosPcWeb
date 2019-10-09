using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Hotel.Services.ResInvoiceManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResInvoiceManage
{
    /// <summary>
    /// 发票
    /// </summary>
    public class ResInvoiceService: IResInvoiceService
    {
        private DbHotelPmsContext _pmsContext;

        public ResInvoiceService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }

        /// <summary>
        /// 根据酒店Id和订单Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="resId">订单Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        public ResInvoiceMainInfo GetResInvoiceMainInfoByResId(string hid, string resId)
        {
            return GetResInvoiceMainInfoByRegId(hid, 0, resId);
        }

        /// <summary>
        /// 根据酒店Id和会员账务Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="profileCaId">会员账务Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        public ResInvoiceMainInfo GetResInvoiceMainInfoByProfileCaId(string hid, Guid profileCaId)
        {
            return GetResInvoiceMainInfoByRegId(hid, 1, profileCaId.ToString());
        }

        /// <summary>
        /// 根据酒店Id和合约单位账务Id返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="companyCaId">合约单位账务Id</param>
        /// <returns>对应的发票所需要的所有信息</returns>
        public ResInvoiceMainInfo GetResInvoiceMainInfoByCompanyCaId(string hid, Guid companyCaId)
        {
            return GetResInvoiceMainInfoByRegId(hid, 2, companyCaId.ToString());
        }

        /// <summary>
        /// 根据酒店Id和（订单Id 或 会员账务Id 或 合约单位账务Id）返回对应的发票所需要的所有信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="reftype">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        private ResInvoiceMainInfo GetResInvoiceMainInfoByRegId(string hid, byte reftype, string id)
        {
            ResInvoiceMainInfo resultEntity = new ResInvoiceMainInfo();
            resultEntity.InvoiceInfos = new List<ResInvoiceInfo>();
            if(string.IsNullOrWhiteSpace(hid) || (reftype != 0 && reftype != 1 && reftype != 2) || string.IsNullOrWhiteSpace(id))
            {
                return resultEntity;
            }
            //获取发票主表信息
            var InvoiceIQueryable = _pmsContext.Invoices.Where(c => c.Hid == hid && c.Reftype == reftype);
            if (reftype == 0)//和订单号相关
            {
                InvoiceIQueryable = InvoiceIQueryable.Where(c => c.Resid == id);
            }
            else if (reftype == 1)//和会员账务相关
            {
                Guid profileCaid = Guid.Parse(id);
                InvoiceIQueryable = InvoiceIQueryable.Where(c => c.ProfileCaid == profileCaid);
            }
            else if(reftype == 2)//和合约单位账务相关
            {
                Guid companycaid = Guid.Parse(id);
                InvoiceIQueryable = InvoiceIQueryable.Where(c => c.Companycaid == companycaid);
            }
            var InvoiceList = InvoiceIQueryable.ToList();
            //获取发票明细表信息
            var InvoiceIdList = InvoiceList.Select(s => s.Id).ToList();
            var InvoiceDetailList = _pmsContext.InvoiceDetails.Where(c => c.Hid == hid && InvoiceIdList.Contains(c.Invoceid)).ToList();
            //循环 为返回实体赋值
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            foreach (var item in InvoiceList)
            {
                ResInvoiceInfo itemEntity = new ResInvoiceInfo {
                    Id = item.Id,

                    Reftype = item.Reftype,
                    Resid = item.Resid,
                    Companycaid = item.Companycaid,
                    ProfileCaid = item.ProfileCaid,

                    TaxType = item.TaxType,
                    PrintType = item.PrintType,
                    IsCancel = item.IsCancel,
                    Isread = item.Isread,

                    No = item.No,
                    TaxNo = item.TaxNo,
                    TaxName = item.TaxName,
                    TaxAddTel = item.TaxAddTel,
                    TaxBankAccount = item.TaxBankAccount,

                    InvoiceCode = item.InvoiceCode,
                    InvoiceNo = item.InvoiceNo,
                    InvoiceSeq = item.InvoiceSeq,
                    InvoiceCode0 = item.InvoiceCode0,
                    InvoiceNo0 = item.InvoiceNo0,
                    RedInfo = item.RedInfo,

                    BsnsDate = item.BsnsDate,
                    CDate = item.CDate,
                    InputUser = item.InputUser,
                    Remark = item.Remark,
                };
                //源数据
                itemEntity.OriginInvoiceJsonData = javaScriptSerializer.Serialize(item);
                //业务来源名称
                if (itemEntity.Reftype == 0) {
                    itemEntity.ReftypeName = "订单开票";
                }
                else if (itemEntity.Reftype == 1)
                {
                    itemEntity.ReftypeName = "会员开票";
                }
                else if (itemEntity.Reftype == 2)
                {
                    itemEntity.ReftypeName = "合约单位开票";
                }
                //发票类型名称
                if (itemEntity.TaxType == 0)
                {
                    itemEntity.TaxTypeName = "普通发票";
                }
                else if (itemEntity.TaxType == 1)
                {
                    itemEntity.TaxTypeName = "专用发票";
                }
                //明细
                itemEntity.InvoiceDetails = new List<ResInvoiceDetailInfo>();
                var itemInvoiceDetailList = InvoiceDetailList.Where(c => c.Invoceid == item.Id).ToList();
                foreach(var itemDetail in itemInvoiceDetailList)
                {
                    itemEntity.InvoiceDetails.Add(new ResInvoiceDetailInfo {
                        ItemTaxid = itemDetail.ItemTaxid,
                        Amount = itemDetail.Amount,
                        AmountTax = itemDetail.AmountTax,
                        AmountNoTax = itemDetail.AmountNoTax,                        
                        RateTax = itemDetail.RateTax
                    });
                }
                //汇总
                itemEntity.Amount = itemEntity.InvoiceDetails.Select(c=>c.Amount).Sum();
                //加入列表
                resultEntity.InvoiceInfos.Add(itemEntity);
            }
            return resultEntity;
        }

        /// <summary>
        /// 删除发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Id">发票ID</param>
        /// <returns></returns>
        public JsonResultData DelResInvoice(string hid, Guid Id)
        {
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("酒店ID为空！");
            }
            var entity = _pmsContext.Invoices.Where(c => c.Hid == hid && c.Id == Id).SingleOrDefault();
            if(entity == null)
            {
                return JsonResultData.Failure("找不到要删除的发票！");
            }
            //删除发票
            _pmsContext.Invoices.Remove(entity);
            _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.发票删除);
            //删除明细
            var list = _pmsContext.InvoiceDetails.Where(c => c.Hid == hid && c.Invoceid == Id);
            _pmsContext.InvoiceDetails.RemoveRange(list);
            //保存
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 增加或修改发票信息
        /// </summary>
        /// <param name="resInvoiceInfo">发票信息实例</param>
        /// <param name="currentInfo">当前登录信息</param>
        /// <param name="businessDate">当前酒店营业日</param>
        /// <returns></returns>
        public JsonResultData AddOrUpdateResInvoice(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo, DateTime businessDate)
        {
            //验证
            string msg = "";
            if(!Valid(resInvoiceInfo, out msg))
            {
                return JsonResultData.Failure(msg);
            }
            Guid InvoiceId = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(resInvoiceInfo.OriginInvoiceJsonData))
            {
                //添加
                resInvoiceInfo.Id = InvoiceId;
                AddResInvoice(resInvoiceInfo, currentInfo, businessDate);
            }
            else
            {
                //修改
                InvoiceId = resInvoiceInfo.Id;
                UpdateResInvoice(resInvoiceInfo, currentInfo);
            }
            //删除并重新添加发票明细
            SetResInvoiceDetail(resInvoiceInfo.InvoiceDetails, currentInfo.HotelId, InvoiceId);
            //设置开票信息表
            SetResInvoiceInfo(resInvoiceInfo, currentInfo);
            //更新发票信息到会员表和熟客表
            UpdateInvoiceToProfileAndGuest(resInvoiceInfo, currentInfo);
            //保存
            _pmsContext.SaveChanges();
            return JsonResultData.Successed(InvoiceId.ToString());//返回主键ID
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="resInvoiceInfo"></param>
        /// <param name="currentInfo"></param>
        /// <param name="businessDate"></param>
        private void AddResInvoice(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo, DateTime businessDate)
        {            
            _pmsContext.Invoices.Add(new Entities.Invoice {
                Hid = currentInfo.HotelId,
                Id = resInvoiceInfo.Id,

                Reftype = resInvoiceInfo.Reftype,
                Resid = resInvoiceInfo.Resid,
                Companycaid = resInvoiceInfo.Companycaid,                
                ProfileCaid = resInvoiceInfo.ProfileCaid,

                TaxType = resInvoiceInfo.TaxType,
                PrintType = resInvoiceInfo.PrintType,
                IsCancel = resInvoiceInfo.IsCancel,
                Isread = resInvoiceInfo.Isread,

                No = resInvoiceInfo.No ?? "",
                TaxNo = resInvoiceInfo.TaxNo,
                TaxName = resInvoiceInfo.TaxName,
                TaxAddTel = resInvoiceInfo.TaxAddTel,
                TaxBankAccount = resInvoiceInfo.TaxBankAccount,

                InvoiceCode = resInvoiceInfo.InvoiceCode,
                InvoiceNo = resInvoiceInfo.InvoiceNo,
                InvoiceSeq = resInvoiceInfo.InvoiceSeq,
                InvoiceCode0 = resInvoiceInfo.InvoiceCode0,
                InvoiceNo0 = resInvoiceInfo.InvoiceNo0,
                RedInfo = resInvoiceInfo.RedInfo,

                BsnsDate = businessDate,
                CDate = DateTime.Now,
                InputUser = currentInfo.UserName,
                Remark = resInvoiceInfo.Remark,
            });
            _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.发票增加);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="resInvoiceInfo"></param>
        /// <param name="currentInfo"></param>
        private void UpdateResInvoice(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo)
        {
            var originInvoiceInfo = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Entities.Invoice>(resInvoiceInfo.OriginInvoiceJsonData);

            var invoiceInfo = new Entities.Invoice {
                Id = resInvoiceInfo.Id,

                Reftype = resInvoiceInfo.Reftype,
                Resid = resInvoiceInfo.Resid,
                Companycaid = resInvoiceInfo.Companycaid,
                ProfileCaid = resInvoiceInfo.ProfileCaid,

                TaxType = resInvoiceInfo.TaxType,
                PrintType = resInvoiceInfo.PrintType,
                IsCancel = resInvoiceInfo.IsCancel,
                Isread = resInvoiceInfo.Isread,

                No = resInvoiceInfo.No ?? "",
                TaxNo = resInvoiceInfo.TaxNo,
                TaxName = resInvoiceInfo.TaxName,
                TaxAddTel = resInvoiceInfo.TaxAddTel,
                TaxBankAccount = resInvoiceInfo.TaxBankAccount,

                InvoiceCode = resInvoiceInfo.InvoiceCode,
                InvoiceNo = resInvoiceInfo.InvoiceNo,
                InvoiceSeq = resInvoiceInfo.InvoiceSeq,
                InvoiceCode0 = resInvoiceInfo.InvoiceCode0,
                InvoiceNo0 = resInvoiceInfo.InvoiceNo0,
                RedInfo = resInvoiceInfo.RedInfo,
                Remark = resInvoiceInfo.Remark,
            };
            if(invoiceInfo.Id == originInvoiceInfo.Id && currentInfo.HotelId == originInvoiceInfo.Hid)
            {
                CRUDService<Entities.Invoice>.Update<Entities.Invoice>(_pmsContext, invoiceInfo, originInvoiceInfo, new List<string> { "Reftype", "Resid", "Companycaid", "ProfileCaid", "TaxType", "PrintType", "IsCancel", "Isread", "No", "TaxNo", "TaxName", "TaxAddTel", "TaxBankAccount", "InvoiceCode", "InvoiceNo", "InvoiceSeq", "InvoiceCode0", "InvoiceNo0", "RedInfo", "Remark" });
                _pmsContext.AddDataChangeLogs(BSPMS.Common.Services.Enums.OpLogType.发票更新);
            }
        }
        /// <summary>
        /// 设置发票明细
        /// </summary>
        /// <param name="InvoiceDetails">发票明细信息</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="invoiceId">发票ID</param>
        private void SetResInvoiceDetail(List<ResInvoiceDetailInfo> InvoiceDetails, string hid, Guid invoiceId)
        {
            //删除
            var list  = _pmsContext.InvoiceDetails.Where(c => c.Hid == hid && c.Invoceid == invoiceId);
            _pmsContext.InvoiceDetails.RemoveRange(list);
            //添加
            foreach (var item in InvoiceDetails)
            {
                _pmsContext.InvoiceDetails.Add(new Entities.InvoiceDetail {
                    Hid = hid,
                    Invoceid = invoiceId,
                    Id = Guid.NewGuid(),
                    ItemTaxid = item.ItemTaxid,
                    Amount = item.Amount,
                    AmountNoTax = item.AmountNoTax,
                    AmountTax = item.AmountTax,
                    RateTax = item.RateTax
                });
            }
        }
        /// <summary>
        /// 设置开票信息表
        /// </summary>
        /// <param name="resInvoiceInfo"></param>
        /// <param name="currentInfo"></param>
        private void SetResInvoiceInfo(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo)
        {
            var invoiceInfos = _pmsContext.InvoiceInfos.AsNoTracking().Where(c => c.Hid == currentInfo.HotelId);
            if (resInvoiceInfo.TaxType == 0)//0：普通发票 以发票抬头查找
            {
                invoiceInfos = invoiceInfos.Where(c => c.TaxType == 0 && c.TaxName == resInvoiceInfo.TaxName);
            }
            else if (resInvoiceInfo.TaxType == 1)//1：增值税专用发票 以税务登记号查找
            {
                invoiceInfos = invoiceInfos.Where(c => c.TaxType == 1 && c.TaxNo == resInvoiceInfo.TaxNo);
            }
            else
            {
                return;
            }
            var entity = invoiceInfos.FirstOrDefault();
            if (entity != null)
            {
                var InvoiceInfo = new Entities.InvoiceInfo{
                    Id = entity.Id,
                    Hid= entity.Hid,
                    Py = entity.Py,
                    TaxType = resInvoiceInfo.TaxType,
                    TaxNo = resInvoiceInfo.TaxNo,
                    TaxName = resInvoiceInfo.TaxName,
                    TaxAddTel = resInvoiceInfo.TaxAddTel,
                    TaxBankAccount = resInvoiceInfo.TaxBankAccount
                };
                CRUDService<Entities.InvoiceInfo>.Update<Entities.InvoiceInfo>(_pmsContext, InvoiceInfo, entity, new List<string> { "TaxType", "TaxNo", "TaxName", "TaxAddTel", "TaxBankAccount" });
            }
            else
            {
                _pmsContext.InvoiceInfos.Add(new Entities.InvoiceInfo {
                    Hid = currentInfo.HotelId,
                    Id = Guid.NewGuid(),
                    TaxType = resInvoiceInfo.TaxType,
                    TaxNo = resInvoiceInfo.TaxNo,
                    TaxName = resInvoiceInfo.TaxName,
                    TaxAddTel = resInvoiceInfo.TaxAddTel,
                    TaxBankAccount = resInvoiceInfo.TaxBankAccount
                });
            }
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="resInvoiceInfo">表单内容</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public bool Valid(ResInvoiceInfo resInvoiceInfo, out string message)
        {
            if (resInvoiceInfo == null)
            {
                message = "请填写表单！";
                return false;
            }
            if (resInvoiceInfo.Reftype != 0 && resInvoiceInfo.Reftype != 1 && resInvoiceInfo.Reftype != 2)
            {
                message = "发票业务来源错误！";
                return false;
            }
            if (resInvoiceInfo.Reftype == 0)
            {
                if (string.IsNullOrWhiteSpace(resInvoiceInfo.Resid))
                {
                    message = "参数订单ID错误！";
                    return false;
                }
                resInvoiceInfo.ProfileCaid = null;
                resInvoiceInfo.Companycaid = null;
            }
            else if (resInvoiceInfo.Reftype == 1)
            {
                if(resInvoiceInfo.ProfileCaid == null)
                {
                    message = "参数会员账务ID错误！";
                    return false;
                }
                resInvoiceInfo.Resid = null;
                resInvoiceInfo.Companycaid = null;
            }
            else if (resInvoiceInfo.Reftype == 2)
            {
                if(resInvoiceInfo.Companycaid == null)
                {
                    message = "参数合约单位账务ID错误！";
                    return false;
                }
                resInvoiceInfo.Resid = null;
                resInvoiceInfo.ProfileCaid = null;
            }
            if(resInvoiceInfo.TaxType != 0 && resInvoiceInfo.TaxType != 1)
            {
                message = "请选择发票类型！";
                return false;
            }
            if (resInvoiceInfo.PrintType != 0 && resInvoiceInfo.PrintType != 1)
            {
                message = "请选择发票打印类型！";
                return false;
            }
            //if (string.IsNullOrWhiteSpace(resInvoiceInfo.No))
            //{
            //    message = "请输入发票号码！";
            //    return false;
            //}
            if(resInvoiceInfo.InvoiceDetails == null || resInvoiceInfo.InvoiceDetails.Count <= 0)
            {
                message = "请增加发票项目！";
                return false;
            }
            if(resInvoiceInfo.IsCancel == null)
            {
                resInvoiceInfo.IsCancel = false;
            }
            if (resInvoiceInfo.Isread == null)
            {
                resInvoiceInfo.Isread = false;
            }
            foreach (var item in resInvoiceInfo.InvoiceDetails)
            {
                if (string.IsNullOrWhiteSpace(item.ItemTaxid))
                {
                    message = "请选择发票项目！";
                    return false;
                }
                if (item.Amount == null)
                {
                    message = "请输入含税金额！";
                    return false;
                }
                if (item.AmountNoTax == null)
                {
                    message = "请输入不含税金额！";
                    return false;
                }
                if (item.AmountTax == null)
                {
                    message = "请输入税金！";
                    return false;
                }
                if (item.RateTax == null)
                {
                    message = "请输入税率！";
                    return false;
                }
            }
            message = "";
            return true;
        }

        /// <summary>
        /// 获取 发票来源的 发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="reftype">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        public JsonResultData GetInvoiceSourceInfo(string hid, byte reftype, string id)
        {
            //验证
            if (string.IsNullOrWhiteSpace(hid) || (reftype != 0 && reftype != 1 && reftype != 2) || string.IsNullOrWhiteSpace(id))
            {
                return JsonResultData.Failure("参数错误！");
            }
            if (reftype == 0)//和订单号相关
            {
                #region
                //订单是否存在
                if (!_pmsContext.Reses.Where(c => c.Hid == hid && c.Resid == id).Any())
                {
                    return JsonResultData.Failure("参数错误，订单ID不存在！");
                }
                //获取计划发票金额
                decimal? planAmount = null;
                var regidList = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == id).Select(s => s.Regid).AsNoTracking().ToList();
                if(regidList != null && regidList.Count > 0)
                {
                    planAmount = _pmsContext.ResFolios.Where(c => c.Hid == hid && regidList.Contains(c.Regid) && c.Dcflag == "d" && c.Status < 50).Sum(s => s.Amount);
                }
                //获取订单发票
                var entity = _pmsContext.ResInvoiceInfos.Where(c => c.Hid == hid && c.Resid == id).SingleOrDefault();
                //返回
                if (entity == null)
                {
                    var result = new ResInvoiceSimple
                    {
                        TaxType = 0,//发票类型（0 false：普通发票，1 true：增值税专用发票）
                        TaxNo = "",//税务登记号
                        TaxName = "",//发票抬头
                        TaxAddTel = "",//发票地址和电话
                        TaxBankAccount = "",//发票银行和账号
                        PlanAmount = planAmount,//计划发票金额
                    };
                    return JsonResultData.Successed(result);
                }
                else
                {
                    var result = new ResInvoiceSimple
                    {
                        TaxType = (entity.InvoiceType == true) ? 1:0,//发票类型（0 false：普通发票，1 true：增值税专用发票）
                        TaxNo = entity.TaxNo,//税务登记号
                        TaxName = entity.TaxName,//发票抬头
                        TaxAddTel = entity.TaxAddTel,//发票地址和电话
                        TaxBankAccount = entity.TaxBankAccount,//发票银行和账号
                        PlanAmount = planAmount,//计划发票金额
                    };
                    return JsonResultData.Successed(result);
                }
                #endregion
            }
            else if (reftype == 1)//和会员账务相关
            {
                #region
                //验证ID
                Guid profileCaid = new Guid();
                if (!Guid.TryParse(id, out profileCaid))
                {
                    return JsonResultData.Failure("参数错误，会员账务ID格式错误！");
                }
                ////获取会员ID
                var profileId = _pmsContext.ProfileCas.Where(c => c.Hid == hid && c.Id == profileCaid).Select(s => s.Profileid).SingleOrDefault();
                if(profileId == null)
                {
                    return JsonResultData.Failure("参数错误，会员账务ID不存在！");
                }
                //获取会员发票
                var entity = _pmsContext.MbrCards.Where(c => c.Hid == hid && c.Id == profileId).SingleOrDefault();
                if(entity == null)
                {
                    return JsonResultData.Failure("参数错误，会员ID不存在！");
                }
                //返回
                var result = new ResInvoiceSimple
                {
                    TaxType = (entity.TaxType == 1) ? 1 : 0,//发票类型（0 false：普通发票，1 true：增值税专用发票）
                    TaxNo = entity.TaxNo,//税务登记号
                    TaxName = entity.TaxName,//发票抬头
                    TaxAddTel = entity.TaxAddTel,//发票地址和电话
                    TaxBankAccount = entity.TaxBankAccount,//发票银行和账号
                };
                return JsonResultData.Successed(result);
                #endregion
            }
            else if (reftype == 2)//和合约单位账务相关
            {
                #region
                //验证ID
                Guid companyCaid = new Guid();
                if (!Guid.TryParse(id, out companyCaid))
                {
                    return JsonResultData.Failure("参数错误，合约单位账务ID格式错误！");
                }
                ////获取合约单位ID
                var companyId = _pmsContext.CompanyCas.Where(c => c.Hid == hid && c.Id == companyCaid).Select(s => s.Companyid).SingleOrDefault();
                if (companyId == null)
                {
                    return JsonResultData.Failure("参数错误，合约单位账务ID不存在！");
                }
                //获取合约单位发票
                var entity = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id == companyId).SingleOrDefault();
                if (entity == null)
                {
                    return JsonResultData.Failure("参数错误，合约单位ID不存在！");
                }
                //返回
                var result = new ResInvoiceSimple
                {
                    TaxType = (entity.TaxType == 1) ? 1 : 0,//发票类型（0 false：普通发票，1 true：增值税专用发票）
                    TaxNo = entity.TaxNo,//税务登记号
                    TaxName = entity.TaxName,//发票抬头
                    TaxAddTel = entity.TaxAddTel,//发票地址和电话
                    TaxBankAccount = entity.TaxBankAccount,//发票银行和账号
                };
                return JsonResultData.Successed(result);
                #endregion
            }
            else
            {
                return JsonResultData.Failure("参数错误！");
            }
        }

        /// <summary>
        /// 获取开票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="taxType">0:普通发票  1：增值税专用发票</param>
        /// <param name="taxName">模糊查询字符串</param>
        /// <returns></returns>
        public List<Entities.InvoiceInfo> GetInvoicePartInfo(string hid, byte taxType, string taxName)
        {
            if(string.IsNullOrWhiteSpace(hid) || (taxType!=0 && taxType!=1) || string.IsNullOrWhiteSpace(taxName) || taxName.Length < 3)
            {
                return new List<Entities.InvoiceInfo>();
            }
            return _pmsContext.InvoiceInfos.Where(c => c.Hid == hid && c.TaxType == taxType && (c.Py.Contains(taxName) || c.TaxName.Contains(taxName))).ToList();
        }

        /// <summary>
        /// 更新发票信息到会员表和熟客表
        /// </summary>
        public void UpdateInvoiceToProfileAndGuest(ResInvoiceInfo resInvoiceInfo, ICurrentInfo currentInfo)
        {
            if(!(resInvoiceInfo != null && resInvoiceInfo.Reftype == 0 && !string.IsNullOrWhiteSpace(resInvoiceInfo.Resid)))
            {
                return;
            }
            string hid = currentInfo.HotelId;
            string resid = resInvoiceInfo.Resid;
            if (!_pmsContext.Reses.Where(c => c.Resid == resid && c.Hid == hid).AsNoTracking().Any())
            {
                return;
            }
            var resDetails = _pmsContext.ResDetails.AsNoTracking().Where(c => c.Resid == resid && c.Hid == hid).AsNoTracking().ToList();
            if(resDetails == null || resDetails.Count <= 0)
            {
                return;
            }
            var profileids = resDetails.Where(c => c.Profileid != null).Select(c => c.Profileid).Distinct().ToList();
            var guestids = resDetails.Where(c => c.Guestid != null).Select(c => c.Guestid).Distinct().ToList();
            if(profileids != null && profileids.Count > 0)
            {
                //会员
                var mbrCardList = _pmsContext.MbrCards.Where(c => profileids.Contains(c.Id) && c.Hid == hid).ToList();
                if(mbrCardList != null && mbrCardList.Count > 0)
                {
                    foreach(var item in mbrCardList)
                    {
                        item.TaxNo = resInvoiceInfo.TaxNo;
                        item.TaxName = resInvoiceInfo.TaxName;
                        item.TaxAddTel = resInvoiceInfo.TaxAddTel;
                        item.TaxBankAccount = resInvoiceInfo.TaxBankAccount;
                        item.TaxType = resInvoiceInfo.TaxType;
                        _pmsContext.Entry(item).State = EntityState.Modified;
                    }
                }
            }
            if(guestids != null && guestids.Count > 0)
            {
                //熟客
                var guestsList = _pmsContext.Guests.Where(c => guestids.Contains(c.Id) && c.Hid == hid).ToList();
                if (guestsList != null && guestsList.Count > 0)
                {
                    foreach (var item in guestsList)
                    {
                        item.TaxNo = resInvoiceInfo.TaxNo;
                        item.TaxName = resInvoiceInfo.TaxName;
                        item.TaxAddTel = resInvoiceInfo.TaxAddTel;
                        item.TaxBankAccount = resInvoiceInfo.TaxBankAccount;
                        item.TaxType = resInvoiceInfo.TaxType;
                        _pmsContext.Entry(item).State = EntityState.Modified;
                    }
                }
            }
        }

        /// <summary>
        /// 获取订单中指定合约单位的发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">订单ID</param>
        /// <returns></returns>
        public ResInvoiceSimple GetCompanyInvoiceInfo(string hid, string resid)
        {
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(resid))
            {
                return null;
            }
            var companyid = _pmsContext.Reses.Where(c => c.Hid == hid && c.Resid == resid).Select(c => c.Cttid).FirstOrDefault();
            if(companyid != null && companyid.HasValue)
            {
                var companyEntity = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id == companyid).FirstOrDefault();
                if(companyEntity != null)
                {
                    var resInvoiceSimpleEntity = new ResInvoiceSimple
                    {
                        TaxAddTel = companyEntity.TaxAddTel,
                        TaxBankAccount = companyEntity.TaxBankAccount,
                        TaxName = companyEntity.TaxName,
                        TaxNo = companyEntity.TaxNo,
                        TaxType = (companyEntity.TaxType == 1) ? 1 : 0,//发票类型（0 false：普通发票，1 true：增值税专用发票）
                    };
                    if(string.IsNullOrWhiteSpace(resInvoiceSimpleEntity.TaxAddTel) && string.IsNullOrWhiteSpace(resInvoiceSimpleEntity.TaxBankAccount) && string.IsNullOrWhiteSpace(resInvoiceSimpleEntity.TaxName) && string.IsNullOrWhiteSpace(resInvoiceSimpleEntity.TaxNo))
                    {
                        return null;
                    }
                    else
                    {
                        return resInvoiceSimpleEntity;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 生成发票明细消息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="reftype">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        public List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string,decimal?>> GenerateInvoiceDetails(string hid, byte reftype, string id)
        {
            //验证
            if (string.IsNullOrWhiteSpace(hid) || (reftype != 0 && reftype != 1 && reftype != 2) || string.IsNullOrWhiteSpace(id))
            {
                return null;
            }
            if (reftype == 0)//和订单号相关
            {
                var regidList = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.Resid == id).Select(s => s.Regid).AsNoTracking().ToList();
                if (regidList != null && regidList.Count > 0)
                {
                    var resfoliolist = _pmsContext.ResFolios.Where(c => c.Hid == hid && regidList.Contains(c.Regid) && c.Dcflag == "d" && c.Status < 50).GroupBy(c => c.Itemid).Select(c => new { ItemId = c.Key, Amount = c.Sum(s => s.Amount) }).AsNoTracking().ToList();
                    if(resfoliolist != null && resfoliolist.Count > 0)
                    {
                        var itemids = resfoliolist.Select(c => c.ItemId).ToList();
                        var itemList = _pmsContext.Items.Where(c => c.Hid == hid && itemids.Contains(c.Id)).Select(c => new { c.Id, c.InvoiceItemid }).AsNoTracking().ToList();
                        return resfoliolist.Join(itemList, c => c.ItemId, d => d.Id, (c, d) => new { c.Amount, d.InvoiceItemid }).GroupBy(c => c.InvoiceItemid).Select(c => new Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, decimal?> { Key = c.Key, Value = c.Sum(e => e.Amount) }).ToList();
                    }
                }
            }
            else if (reftype == 1)//和会员账务相关
            {
                return null;
            }
            else if (reftype == 2)//和合约单位账务相关
            {
                return null;
            }
            return null;
        }


    }
}