using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF.Common;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class RateService : CRUDService<Rate>, IRateService, IBasicDataCopyService<Rate>
    {
        public RateService(DbHotelPmsContext db) : base(db, db.Rates)
        {
            _pmsContext = db;
        }

        protected override Rate GetTById(string id)
        {
            return new Rate { Id = id };
        }
        private DbHotelPmsContext _pmsContext;
        protected override void AfterUpdate(Rate obj, Rate originObj, List<string> needUpdateFieldNames = null) 
        {
            var hotel = _pmsContext.PmsHotels.Where(w => w.Hid == obj.Hid && w.Grpid != "").FirstOrDefault();
            if (hotel != null)
            {
                if (obj.BookingNotesid != null)
                {
                    var bookingnote = _pmsContext.BookingNotess.Where(d => d.DataCopyId == obj.BookingNotesid && d.Hid == obj.Hid).FirstOrDefault();
                    if (bookingnote != null)
                    {
                        obj.BookingNotesid = bookingnote.Id;
                    }
                }
                obj.Channelids = string.IsNullOrEmpty(obj.Channelids) ? "" : obj.Channelids.Replace(hotel.Grpid, obj.Hid);
                obj.Marketid = string.IsNullOrEmpty(obj.Marketid) ? "" : obj.Marketid.Replace(hotel.Grpid, obj.Hid);
                obj.RefRateid = string.IsNullOrEmpty(obj.RefRateid) ? "" : obj.RefRateid.Replace(hotel.Grpid, obj.Hid);
                obj.RoomTypeids = string.IsNullOrEmpty(obj.RoomTypeids) ? "" : obj.RoomTypeids.Replace(hotel.Grpid, obj.Hid);
                obj.Sourceid = string.IsNullOrEmpty(obj.Sourceid) ? "" : obj.Sourceid.Replace(hotel.Grpid, obj.Hid);
            }
            base.AfterUpdate(obj, originObj, needUpdateFieldNames);
        }
        /// <summary>
        /// 获取价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> List(string hid, string selectId = null)
        {
            DateTime nowDate = DateTime.Now;
            var list = _pmsContext.Rates.Where(c => c.Hid == hid && ((c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate) || c.Id == selectId)).OrderBy(w => w.Seqid).Select(c => new { c.Id, c.Name }).ToList();
            List<KeyValuePair<string, string>> returnList = new List<KeyValuePair<string, string>>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    returnList.Add(new KeyValuePair<string, string>(item.Id, item.Name));
                }
            }
            return returnList;
        }
        /// <summary>
        /// 获取所有价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<System.Web.Mvc.SelectListItem> ListAll(string hid)
        {
            DateTime nowDate = DateTime.Now;
            return _pmsContext.Rates.Where(c => c.Hid == hid && c.isMonth != true).OrderBy(w => w.Seqid).Select(c => new System.Web.Mvc.SelectListItem { Value = c.Id, Text = c.Name, Disabled = (c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate).Equals(true) }).ToList();
        }
        /// <summary>
        /// 获取所有价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<System.Web.Mvc.SelectListItem> PermanentRoomListAll(string hid)
        {
            DateTime nowDate = DateTime.Now;
            return _pmsContext.Rates.Where(c => c.Hid == hid && c.isMonth == true).OrderBy(w => w.Seqid).Select(c => new System.Web.Mvc.SelectListItem { Value = c.Id, Text = c.Name, Disabled = (c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate).Equals(true) }).ToList();
        }

        /// <summary>
        /// 除本身外的其他价格代码
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<Rate> GetRefRateid(string hid)
        {
            return _pmsContext.Rates.Where(c => c.Hid == hid && c.Status == EntityStatus.启用).ToList();
        }

        public List<Rate> GetRate(string hid)
        {
            return _pmsContext.Rates.Where(c => c.Hid == hid).OrderBy(w => w.Seqid).ToList();
        }
        public List<Rate> GetRateref(string hid)
        {
            DateTime nowDate = DateTime.Now;
            return _pmsContext.Rates.Where(c => c.Hid == hid && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate).OrderBy(w => w.Seqid).ToList();
        }
        /// <summary>
        /// 启用禁用价格代码
        /// </summary>
        /// <param name="rateid">价格代码</param>
        /// <param name="hid"></param>
        /// <param name="status">启用还是禁用</param>
        /// <returns></returns>
        public int DisableRates(string rateid, string hid, EntityStatus status)
        {
            try
            {
                Rate update = _pmsContext.Rates.Find(rateid);
                if (update.Status != status)
                {
                    update.Status = status;
                    _pmsContext.Entry(update).State = EntityState.Modified;
                }
                _pmsContext.AddDataChangeLogs(OpLogType.价格码启用禁用);
                _pmsContext.SaveChanges();
                return 1;
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 保存修改价格码时判断价格码相关联表是否已使用此价格码，存在则不允许修改code
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="rateid"></param>
        /// <returns></returns>
        public string checkExistOthertb(string hid, string rateid, bool isdel)
        {
            string tbname = "";
            List<Res> list = _pmsContext.Reses.Where(c => c.Hid == hid && c.RateCode == rateid).ToList();
            if (list.Count > 0)
            {
                tbname += "订单主表、";
            }
            List<ResDetail> list1 = _pmsContext.ResDetails.Where(c => c.Hid == hid && c.RateCode == rateid).ToList();
            if (list1.Count > 0)
            {
                tbname += "订单明细、";
            }
            List<Company> list2 = _pmsContext.Companys.Where(c => c.Hid == hid && c.RateCode == rateid).ToList();
            if (list2.Count > 0)
            {
                tbname += "合约单位、";
            }
            List<MbrCardType> list3 = _pmsContext.MbrCardTypes.Where(c => c.Hid == hid && c.RateCodeid == rateid).ToList();
            if (list3.Count > 0)
            {
                tbname += "会员卡类型、";
            }
            List<RateDetail> list4 = _pmsContext.RateDetails.Where(c => c.Hid == hid && c.Rateid == rateid).ToList();
            if (list4.Count > 0 && tbname == "" && isdel)
            {
                _pmsContext.RateDetails.RemoveRange(list4);
                _pmsContext.SaveChanges();
            }
            return tbname.Trim('、');
        }

        public void updateRateToRefcode(string hid, string ratecode, string refratecode, bool? addmode, decimal? addamount)
        {
            _pmsContext.Database.ExecuteSqlCommand("exec up_updateRateToRefcode @hid={0},@ratecode={1},@refratecode={2},@addMode={3},@addamount={4}", hid, ratecode, refratecode, addmode, addamount == null ? 0 : addamount);
        }
        public override List<Rate> AddAndCopy(Rate groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var addPara = new BasicDataAddAndCopyModel<Rate>
            {
                BasicDataCode = M_V_BasicDataType.BasicDataCodeRate,
                BasicDataService = this,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _pmsContext,
                GroupId = groupId,
                GroupModel = groupModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.AddAndCopy(addPara);
        }
        public override List<Rate> EditAndCopy(Rate groupModel, Rate originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            groupModel.Status = originModel.Status;
            var editPara = new BasicDataEditAndCopyModel<Rate>
            {
                BasicDataCode = M_V_BasicDataType.BasicDataCodeRate,
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
        public override List<Rate> ChangeStatusAndCopy(Rate model, EntityStatus status)
        {
            var changePara = new BasicDataStatusChangeAndCopyModel<Rate>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = model,
                Status = status
            };
            return BasicDataServiceHelper.ChangeGroupAndHotelCopiedStatus(changePara);
        }
        public override List<Rate> DeleteGroupAndHotelCopied(Rate groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<Rate>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }

        public Rate GetNewHotelBasicData(string hid, Rate groupModel)
        {
            var hotelModel = new Rate();
            AutoSetValueHelper.SetValues(groupModel, hotelModel);
            hotelModel.Hid = hid;
            hotelModel.Id = hid + groupModel.Code;
            hotelModel.DataCopyId = groupModel.Id;
            if (groupModel.BookingNotesid != null)
            {
                var bookingnote = _pmsContext.BookingNotess.Where(d => d.DataCopyId == groupModel.BookingNotesid && d.Hid == hid).FirstOrDefault();
                if (bookingnote != null)
                {
                    hotelModel.BookingNotesid = bookingnote.Id;
                }
            }
            hotelModel.Channelids = string.IsNullOrEmpty(groupModel.Channelids) ? "" : groupModel.Channelids.Replace(groupModel.Hid, hid);
            hotelModel.Marketid = string.IsNullOrEmpty(groupModel.Marketid) ? "" : groupModel.Marketid.Replace(groupModel.Hid, hid);
            hotelModel.RefRateid = string.IsNullOrEmpty(groupModel.RefRateid) ? "" : groupModel.RefRateid.Replace(groupModel.Hid, hid);
            hotelModel.RoomTypeids = string.IsNullOrEmpty(groupModel.RoomTypeids) ? "" : groupModel.RoomTypeids.Replace(groupModel.Hid, hid);
            hotelModel.Sourceid = string.IsNullOrEmpty(groupModel.Sourceid) ? "" : groupModel.Sourceid.Replace(groupModel.Hid, hid);
            hotelModel.DataSource = BasicDataDataSource.Copyed.Code;
            Rate cl = _pmsContext.Rates.FirstOrDefault(w => w.Id == hotelModel.Id && w.Hid == hid);
            if (cl != null) { return null; }
            return hotelModel;
        }

        public Rate GetCopyedHotelBasicData(string hid, Rate groupModel, bool iscopyed)
        {
            if (iscopyed)
            {
                return _pmsContext.Rates.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
            }
            else
            {
                return _pmsContext.Rates.FirstOrDefault(w => w.Id == groupModel.Id.Replace(groupModel.Hid, hid) && w.Hid == hid);
            }
        }

        public List<Rate> GetCopyedHotelBasicDatas(Rate groupModel)
        {
            return _pmsContext.Rates.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }
    }
}
