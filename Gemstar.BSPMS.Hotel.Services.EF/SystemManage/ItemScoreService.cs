using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class ItemScoreService : CRUDService<ItemScore>, IItemScoreService
    {
        public ItemScoreService(DbHotelPmsContext db, ISysParaService sysParaService) : base(db, db.ItemScores)
        {
            _pmsContext = db;
            _sysParaService = sysParaService;
        }
        protected override ItemScore GetTById(string id)
        {
            return new ItemScore { Id = id };
        }
        private DbHotelPmsContext _pmsContext;
        private ISysParaService _sysParaService;

        /// <summary>
        /// 批量更改状态（启用，禁用）
        /// </summary>
        /// <param name="ids">要更改的id，多项之间以逗号分隔</param>
        /// <param name="status">更新为当前状态</param>
        /// <returns>更改结果</returns>
        public JsonResultData BatchUpdateStatus(string ids, EntityStatus status)
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
                    ItemScore update = _pmsContext.ItemScores.Find(id);
                    if (update.Status != status)
                    {
                        update.Status = status;
                        _pmsContext.Entry(update).State = EntityState.Modified;
                    }
                }
                _pmsContext.AddDataChangeLogs(OpLogType.积分项目启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (System.Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        protected override void BeforeDelete(ItemScore obj)
        {
            var entity = _pmsContext.Database.SqlQuery<ItemScore>("select * from ItemScore where id={0}", obj.Id).FirstOrDefault();
            if (entity == null)
            {
                return;
            }
            var filename = entity.PicAdd.Split('/').Last();

            var qiniuPara = _sysParaService.GetQiniuPara();
            var domain = qiniuPara.ContainsKey("domain") ? qiniuPara["domain"] : "http://res.gshis.com/";
            string bucket = qiniuPara.ContainsKey("bucket") ? qiniuPara["bucket"] : "jxd-res";
            string access_key = qiniuPara.ContainsKey("access_key") ? qiniuPara["access_key"] : "7TVp7dAC9xHLtd8VHPnHjAJOy9YLh7rhwbzZe7s2";
            string secret_key = qiniuPara.ContainsKey("secret_key") ? qiniuPara["secret_key"] : "Ic96Wia-MQ4T2ma1wQfeqG_zlj1aRMhnZTeIsMGg";

            QiniuHelper.ImgDelete(bucket, filename, access_key, secret_key);
        }

        /// <summary>
        /// 获取积分项目键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> List(string hid)
        {
            var list = _pmsContext.ItemScores.Where(c => c.Hid == hid && c.Status == EntityStatus.启用).OrderBy(w => w.Seqid).Select(c => new { c.Id, c.Name }).ToList();
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

    }
}
