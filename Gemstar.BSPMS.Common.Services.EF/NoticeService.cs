using System;
using System.Collections.Generic;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services.Entities;
using System.Data;
using System.Linq;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class NoticeService : CRUDService<Notice>, INoticeService
    {
        private DbCommonContext _db;
        public NoticeService(DbCommonContext db) : base(db, db.Notice)
        {
            _db = db;
        }
        protected override Notice GetTById(string id)
        {
            var ad = _db.Notice.Find(Guid.Parse(id));
            return ad;
        }
        public Notice GetNoticeFirst(DateTime time, string versionId = null)
        {
            var data = _db.Notice.Where(w => w.BeginDate <= time && time <= w.EndDate && (string.IsNullOrEmpty(w.VersionId) || w.VersionId == versionId)).OrderByDescending(o => o.Level).ThenByDescending(d => d.SDate).FirstOrDefault();
            return data;
        }
        public void UpdateNotice(Notice data)
        {
            var entity = _db.Notice.Find(data.Id);
            entity.Title = data.Title;
            entity.Content = data.Content;
            entity.BeginDate = data.BeginDate;
            entity.EndDate = data.EndDate;
            entity.SDate = data.SDate;
            entity.InputUser = data.InputUser;
            entity.Level = data.Level;
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }
      
    }
}
