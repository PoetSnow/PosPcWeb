using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using System;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.CRMManage;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Hotel.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class CompanyService : CRUDService<Company>, ICompanyService
    {
        public CompanyService(DbHotelPmsContext db, ICompanyLogService companyLogService) : base(db, db.Companys)
        {
            _pmsContext = db;
            _companyLogService = companyLogService;
        }
        protected override Company GetTById(string id)
        {
            return new Company { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
        private ICompanyLogService _companyLogService;

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status,OpLogType logType, ICurrentInfo currentInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return JsonResultData.Failure("请指定要修改的记录id，多项之间以逗号分隔");
                }
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    Guid ID = Guid.Parse(id);
                    Company update = _pmsContext.Companys.Find(ID);
                    //往来中有未核销账务不能销户
                    if (status == EntityStatus.销户)
                    {
                        var companyCa = _pmsContext.CompanyCas.FirstOrDefault(f => f.Companyid == ID && f.Ischeck == true);
                        if (companyCa != null)
                        {
                            throw new System.Exception(string.Format("不能销户[{0}]合约单位，因为有未核销的账务。", update.Name));
                        }
                    }
                    if (update.Status != status)
                    {
                        AddOperationLog(currentInfo, logType, string.Format("状态：{0}=>{1}", update.Status.ToString(), status), id);
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                        //AddDataChangeLog(logType);
                        //这里不用记录合约单位变更记录（companyLog）的日志，company已经记录了这些了
                        _companyLogService.AddSimple(ID, "状态", update.Status.ToString(), status.ToString());
                    }
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="date">延期时间</param>
        public JsonResultData DelayValidDate(Guid id, DateTime date, OpLogType logType)
        {
            try
            {
                var update = _pmsContext.Companys.Find(id);
                update.ValidDate = date;
                _pmsContext.Entry(update).State = EntityState.Modified;
                AddDataChangeLog(logType);
                _companyLogService.AddSimple(id, "延期", (update.ValidDate == null ? "" : update.ValidDate.ToString()), date.ToString());
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="ids">以逗号分隔的主键ID</param>
        /// <param name="date">延期时间</param>
        public JsonResultData BatchDelayValidDate(string ids, DateTime date, OpLogType logType,ICurrentInfo currentInfo)
        {
            try
            {
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    var idValue = Guid.Parse(id);
                    var update = _pmsContext.Companys.Find(idValue);
                    AddOperationLog(currentInfo, logType, string.Format("延期时间：{0}=>{1}", update.ValidDate, date), id);
                    update.ValidDate = date;
                    _pmsContext.Entry(update).State = EntityState.Modified;
                    // AddDataChangeLog(logType);
                    _companyLogService.AddSimple(idValue, "延期", (update.ValidDate == null ? "" : update.ValidDate.ToString()), date.ToString());
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 批量更换业务员
        /// </summary>
        /// <param name="ids">以逗号分隔的主键ID</param>
        /// <param name="saleMan">新业务员</param>
        public JsonResultData BatchUpdateSales(string ids, string saleMan, OpLogType logType,ICurrentInfo currentInfo)
        {
            try
            {
                var idArray = ids.Split(',');
                foreach (var id in idArray)
                {
                    var idValue = Guid.Parse(id);
                    var update = _pmsContext.Companys.Find(idValue);
                    AddOperationLog(currentInfo, logType, string.Format("业务员：{0}=>{1}", update.Sales, saleMan), id);
                    update.Sales = saleMan;
                    _pmsContext.Entry(update).State = EntityState.Modified;
                    //AddDataChangeLog(logType);
                    _companyLogService.AddSimple(idValue, "更换业务员", update.Sales ?? "", saleMan ?? "");
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 可以由子类来重写，实现一些在更新之后的业务处理逻辑，默认为空
        /// </summary>
        /// <param name="obj">要更新的对象，要求所有字段都有值</param>
        /// <param name="originObj">要更新的对象的原始对象，一般是从客户端传递回来的，传给这边以便只update更改了的列而不是全部列进行update</param>
        /// <param name="needUpdateFieldNames">要更新的字段名称列表，一般是根据从客户端传递回来的值自动计算出来的一个字段名称列表</param>
        protected override void AfterUpdate(Company obj, Company originObj, List<string> needUpdateFieldNames = null)
        {
            var needUpdate = needUpdateFieldNames == null ? true : needUpdateFieldNames.Contains("RateCode");
            if (needUpdate && !object.Equals(obj.RateCode, originObj.RateCode))
            {
                var oldrate= _pmsContext.Rates.Where(w => w.Id == originObj.RateCode).FirstOrDefault();
                var newrate= _pmsContext.Rates.Where(w => w.Id == obj.RateCode).FirstOrDefault();
                _companyLogService.AddSimple(obj.Id, "价格代码", (oldrate==null ? "" : oldrate.Name), (newrate==null ? "" : newrate.Name));
            }

            var needUpdate1 = needUpdateFieldNames == null ? true : needUpdateFieldNames.Contains("LimitAmount");
            if (needUpdate1 && !object.Equals(obj.LimitAmount, originObj.LimitAmount))
            {
                _companyLogService.AddSimple(obj.Id, "挂账限额", (originObj.LimitAmount == null ? "" : originObj.LimitAmount.ToString()), (obj.LimitAmount == null ? "" : obj.LimitAmount.ToString()));
            }
        }

        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">合约单位名称</param>
        /// <returns></returns>
        public List<KeyValuePair<string,string>> List(string hid, string name)
        {
            List<Company> list = new List<Company>();
            DateTime nowDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(name))
            {
                list = _pmsContext.Companys.Where(c => c.Hid == hid && c.Name.Contains(name) && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.ValidDate > nowDate).OrderBy(c => c.Code).ToList();
            }
            else
            {
                list = _pmsContext.Companys.Where(c => c.Hid == hid && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.ValidDate > nowDate).OrderBy(c => c.Code).ToList();
            }
            List<KeyValuePair<string, string>> resturnList = new List<KeyValuePair<string, string>>();
            if(list!=null && list.Count > 0)
            {
                foreach(var item in list)
                {
                    resturnList.Add(new KeyValuePair<string, string>(item.Id.ToString(),item.Name));
                }
            }
            return resturnList;
        }
        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">合约单位名称</param>
        /// <param name="notId">不包含合约单位ID</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> List(string hid, string name, Guid notId)
        {
            List<Company> list = new List<Company>();
            DateTime nowDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(name))
            {
                list = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id != notId && c.Name.Contains(name) && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.ValidDate > nowDate).OrderBy(c => c.Code).ToList();
            }
            else
            {
                list = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id != notId && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.ValidDate > nowDate).OrderBy(c => c.Code).ToList();
            }
            List<KeyValuePair<string, string>> resturnList = new List<KeyValuePair<string, string>>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    resturnList.Add(new KeyValuePair<string, string>(item.Id.ToString(), item.Name));
                }
            }
            return resturnList;
        }
        /// <summary>
        /// 获取合约单位键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public List<KeyValuePairModel<string,string>> GetCompanyInfoList(string hid, string keyword)
        {
            DateTime nowDate = DateTime.Now;
            var query = _pmsContext.Companys.Where(w => w.Hid == hid && w.Status == EntityStatus.启用 && w.BeginDate < nowDate && w.ValidDate > nowDate);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(w => (w.Code.Contains(keyword) || w.Name.Contains(keyword) || w.Py.Contains(keyword)));    
            }
            return query.OrderBy(c => c.Code).Select(w=>new KeyValuePairModel<string, string> { Key = w.Name, Value = w.Id.ToString(), Data = w.Code }).ToList();
        }

        /// <summary>
        /// 删除之前检查是否已有操作记录
        /// </summary>
        /// <param name="obj"></param>
        protected override void BeforeDelete(Company obj)
        {
            var isTrue1 = _pmsContext.CompanyCas.Where(c => c.Companyid == obj.Id).Any();
            var isTrue2 = _pmsContext.CompanyTranses.Where(c => c.Companyid == obj.Id).Any();
            if(isTrue1 || isTrue2)
            {
                string msg = (isTrue1 ? "[账务信息]" : "") + (isTrue2 ? "[消费记录]" : "");
                throw new Exception(string.Format("此合约单位已有{0}，不能删除。", msg));
            }
            else
            {
                var list = _pmsContext.CompanyLogs.Where(c => c.Companyid == obj.Id).ToList();
                foreach(var entity in list)
                {
                    if (entity != null)
                    {
                        _pmsContext.CompanyLogs.Remove(entity);
                        _pmsContext.Entry(entity).State = EntityState.Deleted;
                    }
                }
            }
        }

        /// <summary>
        /// 指定酒店的合约单位中是否已使用此类别
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="companyTypeId">合约单位类别ID</param>
        /// <returns></returns>
        public bool IsExistsCompanyType(string hid, string companyTypeId)
        {
            return _pmsContext.Companys.Where(c => c.Hid == hid && c.CompanyTypeid == companyTypeId).Any();
        }
        /// <summary>
        /// 查询满足指定关键字的合约单位列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="keyword">关键字</param>
        /// <returns>包含指定关键字的合约单位列表</returns>
        public List<CompanyPayListItem> Query(string hid, string keyword)
        {
            var list = _pmsContext.Database.SqlQuery<Company>("SELECT * FROM dbo.company WHERE (hid = @hid OR CHARINDEX(',' + @hid + ',', ',' + amountUse + ',') > 0) AND status < 51 AND GETDATE() >= beginDate AND GETDATE() <= validDate and (CHARINDEX(@keyword,code)>0 OR CHARINDEX(@keyword,name)>0 OR CHARINDEX(@keyword,py)>0)"
                , new SqlParameter("@hid", hid ?? ""), new SqlParameter("@keyword", keyword ?? "")).ToList();
            var returnList = new List<CompanyPayListItem>();
            if (list != null && list.Count > 0)
            {
                var companyids = list.Select(c => c.Id);
                var hotelSigners = _pmsContext.CompanySigners.Where(w => companyids.Contains(w.CompanyId) && w.Signtype == CompanySignType.签单人).ToList();
                foreach (var item in list)
                {
                    var companyPay = new CompanyPayListItem
                    {
                        CompanyId = item.Id.ToString(),
                        Name = item.Name
                    };
                    companyPay.Signers = hotelSigners.Where(w => w.CompanyId == item.Id).ToList();
                    returnList.Add(companyPay);
                }
            }
            return returnList;
        }

        /// <summary>
        /// 根据主键ID获取合约单位信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">合约单位主键ID</param>
        /// <returns></returns>
        public JsonResultData GetCompanyInfo(string hid, Guid id)
        {
            DateTime nowDate = DateTime.Now;
            var entity = _pmsContext.Companys.Where(c => c.Hid == hid && c.Id == id && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.ValidDate > nowDate).Select(c => new { c.RateCode, c.Contact, c.ContactMobile }).AsNoTracking().FirstOrDefault();
            if(entity != null)
            {
                var rateCode = entity.RateCode;
                if (!string.IsNullOrWhiteSpace(rateCode))
                {
                    if (_pmsContext.Rates.Where(c => c.Hid == hid && c.Id == rateCode && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate).AsNoTracking().Any())
                    {
                        return JsonResultData.Successed(new { RateCode = rateCode, Contact = entity.Contact, ContactMobile = entity.ContactMobile });
                    }
                }
                return JsonResultData.Successed(new { RateCode = "", Contact = entity.Contact, ContactMobile = entity.ContactMobile });
            }
            return JsonResultData.Successed(null);
        }

        /// <summary>
        /// 检查改合约单位代码是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCompany(string hid, string code)
        {
            return _pmsContext.Companys.Any(a => a.Hid == hid && a.Code == code);
        }
        /// <summary>
        /// 检查改合约单位名称是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsCompanyName(string hid, string name)
        {
            return _pmsContext.Companys.Any(a => a.Hid == hid && a.Name == name);
        }
        #region 操作日志
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="currentInfo">登录信息</param>
        /// <param name="type">日志类型</param>
        /// <param name="text">内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(ICurrentInfo currentInfo, Gemstar.BSPMS.Common.Services.Enums.OpLogType type, string text, string keys)
        {
            _pmsContext.OpLogs.Add(new OpLog
            {
                CDate = DateTime.Now,
                Hid = currentInfo.HotelId,
                CUser = currentInfo.UserName,
                Ip = UrlHelperExtension.GetRemoteClientIPAddress(),
                XType = type.ToString(),
                CText = (text.Length > 4000 ? text.Substring(0, 4000) : text),
                Keys = keys,
            });
        }
        #endregion

        /// <summary>
        /// 获取合约单位金额信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="companyId">合约单位ID</param>
        /// <returns></returns>
        public CommpanyBlanceInfo GetCommpanyBlance(string hid, Guid companyId)
        {
            var companyEntity = _pmsContext.Companys.AsNoTracking().Where(c => c.Hid == hid && c.Id == companyId).Select(s => new { s.RateCode, s.LimitAmount, s.Balance, s.ValidDate }).FirstOrDefault();
            if(companyEntity == null)
            {
                return null;
            }

            string rateName =_pmsContext.Rates.AsNoTracking().Where(c => c.Hid == hid && c.Id == companyEntity.RateCode).Select(c => c.Name).FirstOrDefault();

            return new CommpanyBlanceInfo
            {
                RateCode = companyEntity.RateCode,//价格代码
                RateName = rateName,//价格代码名称
                LimitAmount = companyEntity.LimitAmount,//挂账限额
                Balance = companyEntity.Balance,//应收金额
                ValidDate = companyEntity.ValidDate,//有效期
            };
        }
        public List<CompanySinImg> getCompanySignImg(string hid,Guid id)
        {
           return _pmsContext.CompanySinImg.AsNoTracking().Where(c => c.Hid == hid && c.Companyid == id).ToList();
        }
        public JsonResultData AddCompanyImage(string hid, Guid company, string name, string url)
        {
            try
            {
                var img = new CompanySinImg
                {
                    Hid = hid,
                    Companyid = company,
                    Title = name,
                    ImgAddress = url
                };
                _pmsContext.CompanySinImg.Add(img);
                _pmsContext.SaveChanges();

                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        public JsonResultData DelCompanyImage(int id)
        {
            try
            {
                var data = _pmsContext.CompanySinImg.Where(w => w.Id == id).FirstOrDefault();
                _pmsContext.CompanySinImg.Remove(data);
                _pmsContext.SaveChanges();
                return new JsonResultData() { Success = true };
            }
            catch (Exception)
            {
                return new JsonResultData() { Success = false, Data = "删除失败" };
            }
        }
        /// <summary>
        /// 发送合约单位营销短信
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="mobiles"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResultData SendMarketSms(string hid,string ids, string mobiles, string content)
        {
            try
            {
                //检测酒店是否有启用短信模块，没有则直接返回
                var smsInfo = _pmsContext.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", hid ?? "")).SingleOrDefault();
                if (smsInfo == null || smsInfo.Enable != "1")
                {
                    return JsonResultData.Failure("酒店没有启用短信模块");
                }
                var idArr = ids.Split(',');
                for (int i = 0; i < idArr.Length; i++)
                {
                    var id = Guid.Parse(idArr[i]);
                    var entity = _pmsContext.Companys.Where(m => m.Id == id).FirstOrDefault();
                    entity.MarketSmsDate = DateTime.Now;
                    _pmsContext.Entry(entity).State = EntityState.Modified;
                }
                _pmsContext.SaveChanges();
                var sendService = DependencyResolver.Current.GetService<ISMSSendService>();
                var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                var para = _sysParaService.GetSMSSendPara();
                var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                var hotelenty = _pmsContext.PmsHotels.Where(c => c.Hid == hid).SingleOrDefault();
                string hotelName = "";
                if (hotelenty != null && !string.IsNullOrEmpty(hotelenty.Hotelshortname))
                {
                    hotelName = hotelenty.Hotelshortname;
                }
                else
                {
                    var _currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
                    hotelName = _currentInfo.HotelName;
                }
                SMSSendParaCommonSms sms = new SMSSendParaCommonSms
                {
                    Mobile = mobiles,
                    UserName = smsInfo.UserName,
                    Password = smsInfo.Password,
                    Content = content,
                    HotelName = hotelName
                };
                sendService.CommonSendSms(sms, para, smsLogService);
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("发送营销短信失败");
                throw;
            }
        }
    }
}
