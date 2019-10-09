using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using System;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF.Common;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class ShiftService : CRUDService<Shift>, IShiftService,   IBasicDataCopyService<Shift>
    {
        public ShiftService(DbHotelPmsContext db) : base(db, db.Shifts)
        {
            _pmsContext = db;
        }

        public List<Shift> GetShifts(string hid)
        {
            return _pmsContext.Shifts.AsNoTracking().Where(w => w.Hid == hid).ToList();
        }

        /// <summary>
        /// 获取指定酒店下的状态为可用的班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的状态为可用的班次列表</returns>
        public List<Shift> GetShiftsAvailable(string hid)
        {
            return _pmsContext.Shifts.AsNoTracking().Where(w => w.Hid == hid && w.Status < EntityStatus.禁用).OrderBy(o => o.Seqid).ToList();
        }
        protected override Shift GetTById(string id)
        {
            var shift = new Shift { Id = id };
            return shift;
        }
        /// <summary>
        /// 启用班次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResultData Enable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Shifts.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.启用;
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }
        /// <summary>
        /// 禁用班次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResultData Disable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Shifts.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.禁用;
                }
                _pmsContext.AddDataChangeLogs(OpLogType.班次启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }
        /// <summary>
        /// 打开指定酒店下的班次
        /// 要求是已经通过权限验证后才调用此方法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="shiftId">班次id</param>
        /// <returns>打开结果</returns>
        public JsonResultData OpenShift(string hid, string shiftId)
        {
            try
            {
                var shift = _pmsContext.Shifts.SingleOrDefault(w => w.Hid == hid && w.Id == shiftId);
                if (shift == null)
                {
                    return JsonResultData.Failure("指定酒店下没有此班次");
                }
                shift.LoginStatus = Enums.ShiftLoginStatus.已开;
                _pmsContext.Entry(shift).State = EntityState.Modified;
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 关闭指定酒店下的班次
        /// 只能关闭打开状态下的班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="shiftId">班次id</param>
        /// <returns>关闭结果</returns>
        public JsonResultData CloseShift(string hid, string shiftId)
        {
            try
            {
                var shift = _pmsContext.Shifts.SingleOrDefault(w => w.Hid == hid && w.Id == shiftId);
                if (shift == null)
                {
                    return JsonResultData.Failure("指定酒店下没有此班次");
                }
                if (shift.LoginStatus == Enums.ShiftLoginStatus.未开)
                    return JsonResultData.Failure("当前班次未开不能关闭");
                shift.LoginStatus = Enums.ShiftLoginStatus.已关闭;
                _pmsContext.Entry(shift).State = EntityState.Modified;
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        public List<KeyValuePair<string, string>> List(string hid)
        {
            var list = _pmsContext.Shifts.AsNoTracking().Where(c => c.Hid == hid && c.Status == EntityStatus.启用).Select(c => new { c.Id, c.ShiftName }).ToList();
            List<KeyValuePair<string, string>> returnList = new List<KeyValuePair<string, string>>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    returnList.Add(new KeyValuePair<string, string>(item.Id, item.ShiftName));
                }
            }
            return returnList;
        }

        /// <summary>
        /// 判断班次在ResFolios账务明细表中是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExsitrResFolio(string hid, string id)
        {
            //  string shiftcode = _pmsContext.Shifts.Where(w => w.Id == id).ToList().FirstOrDefault().Code;
            var list = _pmsContext.ResFolios.AsNoTracking().Where(c => c.Hid == hid && (c.SettleShift == id || c.TransShift == id)).ToList();
            return list.Count() > 0;
        }

        public Shift GetNewHotelBasicData(string hid, Shift groupModel)
        {
            var hotelModel = new Shift();
            AutoSetValueHelper.SetValues(groupModel, hotelModel);
            hotelModel.Hid = hid;
            hotelModel.Id = hid +  hotelModel.Code;
            hotelModel.DataCopyId = groupModel.Id;
            hotelModel.DataSource = BasicDataDataSource.Copyed.Code;
            Shift cl = _pmsContext.Shifts.FirstOrDefault(w => w.Id == hotelModel.Id && w.Hid == hid);
            if (cl != null) { return null; }
            return hotelModel;
        }

        public Shift GetCopyedHotelBasicData(string hid, Shift groupModel,bool iscopyed)
        {
            if (iscopyed)
            {
                return _pmsContext.Shifts.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
            }
            else
            {
                return _pmsContext.Shifts.FirstOrDefault(w => w.Id == groupModel.Id.Replace(groupModel.Hid, hid) && w.Hid == hid);
            } 
        }

        public List<Shift> GetCopyedHotelBasicDatas(Shift groupModel)
        {
            return _pmsContext.Shifts.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }

        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        public override List<Shift> AddAndCopy(Shift groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var addPara = new BasicDataAddAndCopyModel<Shift>
            {
                GroupId = groupId,
                BasicDataCode = M_V_BasicDataType.BasicDataCodeShift,
                BasicDataService = this,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupModel = groupModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.AddAndCopy(addPara);
        }
        /// <summary>
        /// 修改并分发
        /// </summary>
        /// <param name="groupModel"></param>
        /// <param name="originModel"></param>
        /// <param name="fieldNames"></param>
        /// <param name="groupId"></param>
        /// <param name="dataControlCode"></param>
        /// <param name="selectedResortHids"></param>
        /// <param name="updateProperties"></param>
        /// <returns></returns>
        public override List<Shift> EditAndCopy(Shift groupModel, Shift originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            var editPara = new BasicDataEditAndCopyModel<Shift>
            {
                BasicDataCode = M_V_BasicDataType.BasicDataCodeShift,
                BasicDataService = this,
                CopyedUpdateProperties = updateProperties,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupId = groupId,
                GroupModel = groupModel,
                GroupModelUpdateFieldNames = fieldNames,
                OriginGroupModel = originModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.EditAndCopy(editPara);
        }
        /// <summary>
        /// 启用禁用集团记录并且同时启用禁用分发到分店的记录
        /// </summary>
        /// <param name="model">要启用禁用的集团记录</param>
        /// <param name="status">新的状态</param>
        /// <returns>启用禁用的实体列表</returns>
        public override List<Shift> ChangeStatusAndCopy(Shift model, EntityStatus status)
        {
            var changePara = new BasicDataStatusChangeAndCopyModel<Shift>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = model,
                Status = status
            };
            return BasicDataServiceHelper.ChangeGroupAndHotelCopiedStatus(changePara);
        }
        public override List<Shift> DeleteGroupAndHotelCopied(Shift groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<Shift>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }

        private DbHotelPmsContext _pmsContext;
    }
}
