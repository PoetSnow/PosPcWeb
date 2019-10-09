using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services.EF.Common;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class BookingNotesService : CRUDService<BookingNotes>, IBookingNotesService, IBasicDataCopyService<BookingNotes>
    {
        private DbHotelPmsContext _db;
        public BookingNotesService(DbHotelPmsContext db) : base(db, db.BookingNotess)
        {
            _db = db;
        }

        public List<BookingNotes> GetBookingNotes(string hid)
        {
            return _db.BookingNotess.Where(w => w.Hid == hid).ToList();
        }
        /// <summary>
        /// 增加集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        public override List<BookingNotes> AddAndCopy(BookingNotes groupModel, string groupId, string dataControlCode, List<string> selectedResortHids)
        {
            var addPara = new BasicDataAddAndCopyModel<BookingNotes>
            {
                GroupId = groupId,
                BasicDataCode = M_V_BasicDataType.BasicDataCodeBookingNotes,
                BasicDataService = this,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _db,
                GroupModel = groupModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.AddAndCopy(addPara);
        }
        /// <summary>
        /// 修改集团记录并且分发
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <param name="originModel">原始集团记录</param>
        /// <param name="fieldNames">原始集团记录修改字段列表</param>
        /// <param name="groupId">集团id</param>
        /// <param name="dataControlCode">数据分发方式</param>
        /// <param name="selectedResortHids">选中的要分发的酒店id</param>
        /// <param name="updateProperties">分发属性列表</param>
        public override List<BookingNotes> EditAndCopy(BookingNotes groupModel, BookingNotes originModel, List<string> fieldNames, string groupId, string dataControlCode, List<string> selectedResortHids, List<string> updateProperties)
        {
            var editPara = new BasicDataEditAndCopyModel<BookingNotes>
            {
                GroupId = groupId,
                BasicDataCode = M_V_BasicDataType.BasicDataCodeBookingNotes,
                BasicDataService = this,
                CopyedUpdateProperties = updateProperties,
                CRUDService = this,
                DataControlCode = dataControlCode,
                DB = _db,
                GroupModel = groupModel,
                GroupModelUpdateFieldNames = fieldNames,
                OriginGroupModel = originModel,
                SelectedResortHids = selectedResortHids
            };
            return BasicDataServiceHelper.EditAndCopy(editPara);
        }
        /// <summary>
        /// 删除集团和分发到集团分店的记录
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        public override List<BookingNotes> DeleteGroupAndHotelCopied(BookingNotes groupModel)
        {
            var deletePara = new BasicDataDeleteGroupAndHotelCopiedModel<BookingNotes>
            {
                BasicDataService = this,
                CRUDService = this,
                GroupModel = groupModel
            };
            return BasicDataServiceHelper.DeleteGroupAndHotelCopied(deletePara);
        }

        protected override BookingNotes GetTById(string id)
        {
            var bnId = Guid.Parse(id);
            var bknotes = _db.BookingNotess.Find(bnId);
            return bknotes;
        }

        /// <summary>
        /// 获取集团记录在指定分店的分发实例
        /// </summary>
        /// <param name="hid">分店id</param>
        /// <param name="groupModel">集团记录</param>
        public BookingNotes GetNewHotelBasicData(string hid, BookingNotes groupModel)
        {
            var hotelModel = new BookingNotes { Hid = hid, Id = Guid.NewGuid(), DataSource = BasicDataDataSource.Copyed.Code, DataCopyId = groupModel.Id, Name = groupModel.Name, Remark = groupModel.Remark };
            return hotelModel;
        }
        /// <summary>
        /// 获取指定集团记录分发到酒店后的酒店记录实例
        /// </summary>
        /// <param name="hid">分店id</param>
        /// <param name="groupModel">集团记录</param>
        /// <returns>null：分店没有此集团记录的分发记录，分发实例：集团记录分发到此分店后的记录实例</returns>
        public BookingNotes GetCopyedHotelBasicData(string hid, BookingNotes groupModel, bool iscopyed)
        {
            if (iscopyed)
            {
                return _db.BookingNotess.FirstOrDefault(w => w.DataCopyId == groupModel.Id && w.Hid == hid && w.DataSource == BasicDataDataSource.Copyed.Code);
            }
            else
            {
                return _db.BookingNotess.FirstOrDefault(w => w.Name == groupModel.Name && w.Hid == hid);
            }
        }
        /// <summary>
        /// 获取指定集团记录分发到酒店后的酒店记录列表
        /// </summary>
        /// <param name="groupModel">集团记录</param>
        /// <returns>指定集团记录分发到酒店后的酒店记录列表</returns>
        public List<BookingNotes> GetCopyedHotelBasicDatas(BookingNotes groupModel)
        {
            return _db.BookingNotess.Where(w => w.DataCopyId == groupModel.Id && w.DataSource == BasicDataDataSource.Copyed.Code).ToList();
        }
    }
}
