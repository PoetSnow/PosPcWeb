using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using System;
using System.Data.SqlClient;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class CompanyCaService : CRUDService<CompanyCa>, ICompanyCaService
    {
        public CompanyCaService(DbHotelPmsContext db, ICurrentInfo currentInfo) : base(db, db.CompanyCas)
        {
            _pmsContext = db;
            _currentInfo = currentInfo;
        }
        protected override CompanyCa GetTById(string id)
        {
            return new CompanyCa { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;

        /// <summary>
        /// 按多个id查找
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<CompanyCa> GetCompanCaList(List<string> ids)
        {
           return  _pmsContext.CompanyCas.Where(w => ids.Contains(w.Id.ToString())).ToList();
        }
        /// <summary>
        /// 合约单位往来录入
        /// </summary>
        /// <param name="companyid">合约单位ID</param>
        /// <param name="type">交易类型</param>
        /// <param name="amount">金额</param>
        /// <param name="itemid">付款方式</param>
        /// <param name="invno">手工单号</param>
        /// <param name="remark">备注</param>
        public JsonResultData AddProc(Guid companyid, string type, decimal amount, string itemid, string invno, string remark,string outletcode,string refno = null)
        {
            try
            {
                var list = _pmsContext.Database.SqlQuery<upCompanyCaInputResult>(@"
                        exec up_companyca_input 
                        @hid=@hid,
                        @inputUser=@inputUser,
                        @companyid=@companyid,
                        @outletcode=@outletcode,
                        @type=@type,
                        @amount=@amount,
                        @itemid=@itemid,
                        @invno=@invno,
                        @remark=@remark,
                        @refno=@refno",
                    new SqlParameter("@hid", _currentInfo.HotelId),
                    new SqlParameter("@inputUser", _currentInfo.UserName),
                    new SqlParameter("@companyid", companyid),
                    new SqlParameter("@outletcode", string.IsNullOrWhiteSpace(outletcode)?"01": outletcode),
                    new SqlParameter("@type", type),
                    new SqlParameter("@amount", amount),
                    new SqlParameter("@itemid", itemid),
                    new SqlParameter("@invno", invno),
                    new SqlParameter("@remark", remark),
                    new SqlParameter("@refno", string.IsNullOrWhiteSpace(refno) ? "": refno)
                    ).ToList();
                if (list != null && list.Count > 0)
                {
                    var result = list.FirstOrDefault();
                    if (result != null)
                    {
                        return new JsonResultData { Success = result.Success, Data = result.Data };
                    }
                }
                return JsonResultData.Failure("保存失败！没有返回信息。");
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="toCompanyId">转到合约单位ID</param>
        public void TransferAccounts(List<string> id, Guid toCompanyId, string remark,string name)
        {
            if (string.IsNullOrWhiteSpace(remark)) { remark = ""; }
            List<Guid> list = new List<Guid>();
            foreach (var item in id)
            {
                Guid newId = Guid.NewGuid();
                list.Add(newId);
                var entity = _pmsContext.CompanyCas.Find(Guid.Parse(item));
                    if (entity != null && entity.Ischeck == false)
                    {
                        _pmsContext.CompanyCas.Add(new CompanyCa
                        {
                            Amount = (-entity.Amount),
                            CheckDate = entity.CheckDate,
                            CheckNo = entity.CheckNo,
                            CheckUser = entity.CheckUser,
                            Companyid = entity.Companyid,
                            Dcflag = entity.Dcflag,
                            Hid = entity.Hid,
                            Id = newId,
                            InputUser = entity.InputUser,
                            Invno = entity.Invno,
                            Itemid = entity.Itemid,
                            OutletCode = entity.OutletCode,
                            Refno = entity.Refno,
                            Remark = entity.Remark,
                            TransBsnsdate = entity.TransBsnsdate,
                            TransDate = entity.TransDate,
                            Type = entity.Type,
                            Ischeck = entity.Ischeck,
                            Grpid=_currentInfo.GroupId
                        });
                    _pmsContext.CompanyCas.Add(new CompanyCa
                    {
                        Amount = entity.Amount,
                        CheckDate = entity.CheckDate,
                        CheckNo = entity.CheckNo,
                        CheckUser = entity.CheckUser,
                        Companyid = toCompanyId,
                        Dcflag = entity.Dcflag,
                        Hid = entity.Hid,
                        Id = Guid.NewGuid(),
                        InputUser = entity.InputUser,
                        Invno = entity.Invno,
                        Itemid = entity.Itemid,
                        OutletCode = entity.OutletCode,
                        Refno = entity.Refno,
                        Grpid=_currentInfo.GroupId,
                        Remark = string.Format("由{0}公司转来", name)+" "+remark, //entity.Remark +","+ remark,
                            TransBsnsdate = entity.TransBsnsdate,
                            TransDate = entity.TransDate,
                            Type = entity.Type,
                            Ischeck = entity.Ischeck
                        });
                }
            }
            int i = _pmsContext.SaveChanges();
            if (i > 0)
            {
                CancelAfterVerification(string.Join(",",id) + "," + string.Join(",",list));
            }
        }

        /// <summary>
        /// 拆账
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="amount">拆账金额</param>
        public void SplitAccounts(Guid id, decimal amount, string remark)
        {
            if (string.IsNullOrWhiteSpace(remark)) { remark = ""; }
            var entity = _pmsContext.CompanyCas.Find(id);
            if (entity != null && entity.Ischeck == false)
            {
                entity.Amount = entity.Amount - amount;
                _pmsContext.Entry(entity).State = EntityState.Modified;

                _pmsContext.CompanyCas.Add(new CompanyCa
                {
                    Amount = amount,
                    Id = Guid.NewGuid(),
                    CheckDate = entity.CheckDate,
                    CheckNo = entity.CheckNo,
                    CheckUser = entity.CheckUser,
                    Companyid = entity.Companyid,
                    Dcflag = entity.Dcflag,
                    Hid = entity.Hid,
                    InputUser = entity.InputUser,
                    Invno = entity.Invno,
                    Itemid = entity.Itemid,
                    OutletCode = entity.OutletCode,
                    Refno = entity.Refno,
                    Remark = entity.Remark + remark,
                    TransBsnsdate = entity.TransBsnsdate,
                    TransDate = entity.TransDate,
                    Type = entity.Type,
                    Ischeck = entity.Ischeck,
                    Grpid=_currentInfo.GroupId
                });
                _pmsContext.SaveChanges();
            }
        }

        /// <summary>
        /// 核销
        /// </summary>
        /// <param name="ids">主键ID列表（逗号隔开）</param>
        /// <returns></returns>
        public JsonResultData CancelAfterVerification(string ids)
        {
            string ErrorMessage = "错误信息，请关闭后重试。";
            //1.验证空值
            if (string.IsNullOrWhiteSpace(ids))
            {
                return JsonResultData.Failure("请选择要核销的记录");
            }
            //2.过滤ID
            List<Guid> guidList = new List<Guid>();
            string[] idsList = ids.Split(',');
            if (idsList != null && idsList.Length > 0)
            {
                foreach (var id in idsList)
                {
                    Guid ID = new Guid();
                    if(Guid.TryParse(id, out ID))
                    {
                        guidList.Add(ID);
                    }
                }
            }
            if (guidList.Count <= 0)
            {
                return JsonResultData.Failure("请选择要核销的记录");
            }
            var companyCaList = _pmsContext.CompanyCas.Where(c => guidList.Contains(c.Id) && c.Hid == _currentInfo.HotelId && (c.Dcflag == "C" || c.Dcflag == "D") && c.Ischeck == false).ToList();
            if(companyCaList == null || companyCaList.Count <= 0)
            {
                return JsonResultData.Failure("请选择要核销的记录");
            }
            //3.要核销的记录 合约单位ID必须相同
            if (companyCaList.Select(c => c.Companyid).Distinct().Count() != 1)
            {
                return JsonResultData.Failure(ErrorMessage);
            }
            //4.验证要核销的记录中的合约单位ID
            Guid companyId = companyCaList.FirstOrDefault().Companyid;
            var companyEntity = _pmsContext.Companys.Where(c => c.Id == companyId).FirstOrDefault();
            if(companyEntity == null || companyEntity.Hid != _currentInfo.HotelId)
            {
                return JsonResultData.Failure(ErrorMessage);
            }
            //5.验证要核销的记录中 （挂账D 和 付款C） 是否相等
            decimal cAmountSum = companyCaList.Where(c => c.Dcflag == "C").Select(c => c.Amount).Sum();
            decimal dAmountSum = companyCaList.Where(c => c.Dcflag == "D").Select(c => c.Amount).Sum();
            if(cAmountSum != dAmountSum)
            {
                return JsonResultData.Failure(string.Format("挂账金额（{0}）和付款金额（{1}）不相等，请修改。", dAmountSum.ToString(), cAmountSum.ToString()));
            }
            //6.核销记录
            Guid checkNo = Guid.NewGuid();
            DateTime checkDate = DateTime.Now;
            foreach (var item in companyCaList)
            {
                item.Ischeck = true;
                item.CheckNo = checkNo;
                item.CheckDate = checkDate;
                item.CheckUser = _currentInfo.UserName;
                item.Grpid = _currentInfo.GroupId;
                _pmsContext.Entry(item).State = EntityState.Modified;
            }
            _pmsContext.SaveChanges();
            var data = new Dictionary<string, string>() {
                {"code", companyEntity.Code},
                { "name",companyEntity.Name},
                {"amout",cAmountSum.ToString()}
            };
            return JsonResultData.Successed(data);
        }

        /// <summary>
        /// 核销记录
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="checkDateBegin"></param>
        /// <param name="checkDateEnd"></param>
        public List<UpQueryCancelRecord> CancelRecord(string hid,string companyid, string checkDateBegin, string checkDateEnd)
        {
          var data=  _pmsContext.Database.SqlQuery<UpQueryCancelRecord>(@"
                exec up_list_cancelRecord 
                @hid=@hid,
                @companyid=@companyid,
                @checkDateBegin=@checkDateBegin,
                @checkDateEnd=@checkDateEnd",
                new SqlParameter("@hid", hid),
                new SqlParameter("@companyid",companyid??""),
                new SqlParameter("@checkDateBegin",checkDateBegin??""),
                new SqlParameter("@checkDateEnd",checkDateEnd??"")
                ).ToList();
            return data;
        }

        public string GetCancelRecordByCheckNo(Guid checkNo)
        {
            var data = string.Join(",",_pmsContext.CompanyCas.Where(c => c.CheckNo == checkNo).Select(c => c.Id).ToList());
            return data;
        }
        /// <summary>
        /// 取消核销
        /// </summary>
        /// <param name="id"></param>
        public JsonResultData CancelRecordLog(string ids)
        {
          var index=  _pmsContext.Database.ExecuteSqlCommand("exec up_companyCa_cancelRecordLog @ids=@ids", new SqlParameter("@ids", ids));
            var list = ids.Split(',').ToList();
            var companyCaList = _pmsContext.CompanyCas.Where(c => list.Contains(c.Id.ToString()) && c.Hid == _currentInfo.HotelId && (c.Dcflag == "C" || c.Dcflag == "D") && c.Ischeck == false).ToList();
            if (companyCaList.Count > 0) {
                var amount = companyCaList.Where(c => c.Dcflag == "C").Select(c => c.Amount).Sum();
                var companyId = companyCaList.FirstOrDefault().Companyid;
                var companyEntity = _pmsContext.Companys.Where(c => c.Id == companyId).FirstOrDefault();
                var data = new Dictionary<string, string>() {
                    {"code",companyEntity.Code },
                    { "name",companyEntity.Name},
                    { "amount",amount.ToString()}
                };
                return JsonResultData.Successed(data);
            }
            return JsonResultData.Failure("取消核销失败");
        }
    }
}
