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
using Gemstar.BSPMS.Hotel.Services.PermanentRoomManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PermanentRoomManage
{
    public class PermanentRoomGoodsService : CRUDService<PermanentRoomGoodsSet>, IPermanentRoomGoodsService
    {
        public PermanentRoomGoodsService(DbHotelPmsContext db) : base(db, db.PermanentRoomGoodsSets)
        {
            _pmsContext = db;
        }
        protected override PermanentRoomGoodsSet GetTById(string id)
        {
            return new PermanentRoomGoodsSet { Id = Int32.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 子单号是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">主单ID</param>
        /// <returns></returns>
        public bool ExistsRegId(string hid, string regid)
        {
            return _pmsContext.ResDetails.Any(c => c.Hid == hid && c.Regid == regid);
        }
        /// <summary>
        /// 长包房设置表主键ID是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">主单ID</param>
        /// <returns></returns>
        public bool ExistsPermanentRoomId(string hid, string permanentRoomSetId)
        {
            Guid id = Guid.Empty;
            if(Guid.TryParse(permanentRoomSetId, out id))
            {
                return _pmsContext.PermanentRoomSets.Any(c => c.Hid == hid && c.Id == id);
            }
            return false;
        }
        /// <summary>
        /// 获取长包房设置表 主键ID
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">子单ID</param>
        /// <returns></returns>
        public string GetPermanentRoomId(string hid, string regid)
        {
            var id = _pmsContext.PermanentRoomSets.Where(c => c.Hid == hid && c.Regid == regid).Select(c => c.Id).SingleOrDefault();
            if(id != null && id != Guid.Empty)
            {
                return id.ToString();
            }
            return null;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">长包房设置表ID</param>
        /// <returns></returns>
        public List<PermanentRoomGoodsSet> ToList(string hid, string permanentRoomSetId)
        {
            if(!string.IsNullOrWhiteSpace(permanentRoomSetId))
            {
                return _pmsContext.PermanentRoomGoodsSets.Where(c => c.Hid == hid && c.PermanentRoomSetId == permanentRoomSetId).ToList();
            }
            return null;
        }


        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<PermanentRoomGoodsSetTemplete> ToTempleteList(string hid)
        {
            return _pmsContext.PermanentRoomGoodsSetTempletes.Where(c => c.Hid == hid).ToList();
        }
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        public List<PermanentRoomGoodsSet> ToTempleteDetailList(string hid, Guid id)
        {
            var list = _pmsContext.PermanentRoomGoodsSets.Where(c => c.Hid == hid && c.PermanentRoomSetId == id.ToString()).ToList();
            foreach (var item in list)
            {
                item.Hid = null;
                item.Id = 0;
                item.PermanentRoomSetId = null;
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
        public JsonResultData AddTemplete(string hid, string name, List<PermanentRoomGoodsSet> addVersions)
        {
            if (_pmsContext.PermanentRoomGoodsSetTempletes.Where(c => c.TempleteName == name).Any())
            {
                return JsonResultData.Failure("模板名称已存在！");
            }
            Guid templeteId = Guid.NewGuid();
            _pmsContext.PermanentRoomGoodsSetTempletes.Add(new PermanentRoomGoodsSetTemplete
            {
                Hid = hid,
                TempleteId = templeteId,
                TempleteName = name,
            });
            foreach (var item in addVersions)
            {
                item.Id = 0;
                item.Hid = hid;
                item.PermanentRoomSetId = templeteId.ToString();
            }
            _pmsContext.PermanentRoomGoodsSets.AddRange(addVersions);
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
            string sql = @"delete from permanentRoomGoodsSetTemplete where hid = @hid and templeteId = @templeteId;
                           delete from permanentRoomGoodsSet where hid = @hid and permanentRoomSetId = @permanentRoomSetId;
                           ";
            _pmsContext.Database.ExecuteSqlCommand(sql
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@templeteId", id)
                , new SqlParameter("@permanentRoomSetId", id.ToString())
                );
        }
        #endregion



    }
}
