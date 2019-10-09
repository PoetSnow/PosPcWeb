using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using System.Data.SqlClient;
using Gemstar.BSPMS.Hotel.Services.ResManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResManage
{
    public class ResBillSettingService : CRUDService<ResBillSetting>, IResBillSettingService
    {
        public ResBillSettingService(DbHotelPmsContext db) : base(db, db.ResBillSettings)
        {
            _pmsContext = db;
        }
        protected override ResBillSetting GetTById(string id)
        {
            return new ResBillSetting { Id = Int32.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 主单号是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public bool ExistsResId(string hid, string resid)
        {
            if(!_pmsContext.Reses.Any(c => c.Hid == hid && c.Resid == resid))
            {
                return false;
            }
            if (!_pmsContext.ResDetails.Any(c => c.Hid == hid && c.Resid == resid))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public List<ResBillSettingInfo> ToList(string hid, string resid)
        {
            return _pmsContext.ResBillSettings.Where(c => c.Hid == hid && c.Resid == resid).Select(c => new ResBillSettingInfo { Id = c.Id, BillCode = c.BillCode,  BillName = c.BillName }).OrderBy(c => c.BillCode).ToList();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public List<ResBillSetting> ToListAll(string hid, string resid)
        {
            return _pmsContext.ResBillSettings.Where(c => c.Hid == hid && c.Resid == resid).OrderBy(c => c.BillCode).ToList();
        }

        /// <summary>
        /// 可以由子类来重写，实现一些在删除之前的业务处理逻辑，默认为空
        /// </summary>
        /// <param name="obj">需要删除的业务实体类</param>
        //protected override void BeforeDelete(ResBillSetting obj)
        //{
        //    var entity = _pmsContext.ResBillSettings.AsNoTracking().FirstOrDefault(c => c.Id == obj.Id);
        //    if (entity != null)
        //    {
        //        var regids = _pmsContext.ResDetails.Where(c => c.Hid == entity.Hid && c.Resid == entity.Resid).Select(c => c.Regid).AsNoTracking().ToList();//获取主单内的所有子单ID
        //        if (regids.Count > 0)
        //        {
        //            if (_pmsContext.ResFolios.AsNoTracking().Any(c => c.Hid == entity.Hid && c.Resid == entity.Resid && regids.Contains(c.Regid) && c.resBillCode == entity.BillCode))//是否有此使用此ID的账单
        //            {
        //                throw new Exception("不能删除，" + entity.BillName + "中有账务存在！");
        //            }
        //        }
        //    }
        //}

        #region 账单与账务
        /// <summary>
        /// 获取有账务的账单列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <returns></returns>
        public List<string> ListItemForResBillId(string hid, string resid)
        {
            return _pmsContext.ResFolios.AsNoTracking().Where(c => c.Hid == hid && c.Resid == resid).Select(c => c.resBillCode).Distinct().ToList();
        }

        /// <summary>
        /// 调账
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resId">主单ID</param>
        /// <param name="folioIds">账务ID列表</param>
        /// <param name="toResBillId">目标账单代码</param>
        /// <returns></returns>
        public JsonResultData AdjustFolio(string hid, string resId, Guid[] folioIds, string toResBillCode, string userName)
        {
            var resFolioList = _pmsContext.ResFolios.Where(c => c.Hid == hid && c.Resid == resId && folioIds.Contains(c.Transid)).Select(c => new { c.Transid, c.resBillCode, c.Regid, c.Status }).AsNoTracking().ToList();
            if(resFolioList.Count <= 0)
            {
                return JsonResultData.Failure("账务不存在！");
            }

            string cText = "";
            foreach (var resFolio in resFolioList)
            {
                if(resFolio.resBillCode != toResBillCode)
                {
                    System.Data.Entity.Infrastructure.DbEntityEntry<ResFolio> entry = _pmsContext.Entry(new ResFolio { Transid = resFolio.Transid, resBillCode = toResBillCode, Status = resFolio.Status });
                    entry.State = EntityState.Unchanged;
                    entry.Property("resBillCode").IsModified = true;

                    cText += string.Format("账务ID：{0}，账单：{1}=>{2}。", resFolio.Transid, resFolio.resBillCode, toResBillCode);
                }
            }
            if (!string.IsNullOrWhiteSpace(cText))
            {
                _pmsContext.OpLogs.Add(new OpLog
                {
                    CDate = DateTime.Now,
                    CText = (cText.Length > 4000 ? cText.Substring(0, 4000) : cText),
                    CUser = userName,
                    Hid = hid,
                    Ip = Gemstar.BSPMS.Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(),
                    Keys = resFolioList[0].Regid,
                    XType = OpLogType.调账.ToString(),
                });
            }
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 账单设置应用到账务中
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">主单ID</param>
        /// <param name="userName">用户名</param>
        public JsonResultData ResBillSettingToFolio(string hid, string resid, string userName)
        {
            //获取账单设置 升序排序
            var settingList = _pmsContext.ResBillSettings.AsNoTracking().Where(c => c.Hid == hid && c.Resid == resid).Select(c => new { c.BillCode, c.DebitTypeId }).OrderBy(c => c.BillCode).ToList();
            if(settingList == null || settingList.Count <= 0)
            {
                return JsonResultData.Failure("账单设置没有数据！");
            }

            //分析账单设置 排除重复项目ID
            List<KeyValuePairModel<string, string>> list = new List<KeyValuePairModel<string, string>>();
            foreach(var item in settingList)
            {
                if (!string.IsNullOrWhiteSpace(item.DebitTypeId))
                {
                    string[] temps  = item.DebitTypeId.Split(',');
                    foreach(var temp in temps)
                    {
                        if(!list.Select(c => c.Key).Contains(temp))
                        {
                            list.Add(new KeyValuePairModel<string, string>(temp, item.BillCode));
                        }
                    }
                }
            }
            if(list == null || list.Count <= 0)
            {
                return JsonResultData.Failure("账单设置没有数据！");
            }
            //获取账务
            var resFolioList = _pmsContext.ResFolios.AsNoTracking().Where(c => c.Hid == hid && c.Resid == resid).Select(c => new { c.Transid, c.Itemid, c.resBillCode, c.Regid, c.DepositType, c.Status }).ToList();
            if (resFolioList == null || resFolioList.Count <= 0)
            {
                return JsonResultData.Failure("此订单内暂无账务！");
            }
            var depositTypeList = _pmsContext.CodeLists.Where(c => c.Hid == hid && c.TypeCode == "20" && c.Status == EntityStatus.启用).AsNoTracking().ToList();//押金类型对应账单代码
            //更新
            string cText = "";
            foreach (var resFolio in resFolioList)
            {
                string resBillCode = "A";//默认A账单
                var entity = list.FirstOrDefault(c => c.Key == resFolio.Itemid);
                if(entity != null)
                {
                    resBillCode = entity.Value;//账单配置后，用配置的账单代码
                }

                if (!string.IsNullOrWhiteSpace(resFolio.DepositType))
                {
                    var depositTypeEntity = depositTypeList.FirstOrDefault(c => c.Id == resFolio.DepositType);
                    if(depositTypeEntity != null && !string.IsNullOrWhiteSpace(depositTypeEntity.Name2))
                    {
                        resBillCode = depositTypeEntity.Name2;
                    }
                }
                
                if(resFolio.resBillCode != resBillCode)
                {
                    System.Data.Entity.Infrastructure.DbEntityEntry<ResFolio> entry = _pmsContext.Entry(new ResFolio { Transid = resFolio.Transid, resBillCode = resBillCode, Status = resFolio.Status });
                    entry.State = EntityState.Unchanged;
                    entry.Property("resBillCode").IsModified = true;
                    cText += string.Format("账务ID：{0}，账单：{1}=>{2}。", resFolio.Transid, resFolio.resBillCode, resBillCode);
                }
            }
            if (!string.IsNullOrWhiteSpace(cText))
            {
                _pmsContext.OpLogs.Add(new OpLog
                {
                    CDate = DateTime.Now,
                    CText = (cText.Length > 4000 ? cText.Substring(0, 4000) : cText),
                    CUser = userName,
                    Hid = hid,
                    Ip = Gemstar.BSPMS.Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(),
                    Keys = resFolioList[0].Regid,
                    XType = OpLogType.调账.ToString(),
                });
            }
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        #endregion

        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<ResBillSettingTemplete> ToTempleteList(string hid)
        {
            return _pmsContext.ResBillSettingTempletes.Where(c => c.Hid == hid).ToList();
        }
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        public List<ResBillSetting> ToTempleteDetailList(string hid, Guid id)
        {
            var list =  _pmsContext.ResBillSettings.Where(c => c.Hid == hid && c.Resid == id.ToString()).ToList();
            foreach(var item in list)
            {
                item.Hid = null;
                item.Id = 0;
                item.Resid = null;
            }
            return list;
        }

        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="name">模板名称</param>
        /// <param name="addVersions">模板内容</param>
        /// <returns></returns>
        public JsonResultData AddTemplete(string hid, string name, List<ResBillSetting> addVersions)
        {
            if(_pmsContext.ResBillSettingTempletes.Where(c => c.TempleteName == name).Any())
            {
                return JsonResultData.Failure("模板名称已存在！");
            }
            Guid templeteId = Guid.NewGuid();
            _pmsContext.ResBillSettingTempletes.Add(new ResBillSettingTemplete
            {
                Hid = hid,
                TempleteId = templeteId,
                TempleteName = name,
            });
            foreach(var item in addVersions)
            {
                item.Id = 0;
                item.Hid = hid;
                item.Resid = templeteId.ToString();
            }
            _pmsContext.ResBillSettings.AddRange(addVersions);
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        public void DelTemplete(string hid, Guid id)
        {
            string sql = @"delete from resBillSettingTemplete where hid = @hid and templeteId = @templeteId;
                           delete from resBillSetting where hid = @hid and resid = @resid;
                           ";
            _pmsContext.Database.ExecuteSqlCommand(sql
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@templeteId", id)
                , new SqlParameter("@resid", id.ToString())
                );
        }
        #endregion

    }
}
