using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Data.SqlClient;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class ScoreUseRuleService : CRUDService<ScoreUseRule>, IScoreUseRuleService
    {
        public ScoreUseRuleService(DbHotelPmsContext db) : base(db, db.ScoreUseRules)
        {
            _pmsContext = db;
        }
        protected override ScoreUseRule GetTById(string id)
        {
            return new ScoreUseRule { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardTypeid">会员卡类型ID</param>
        /// <param name="itemScoreid">兑换项目ID</param>
        /// <returns></returns>
        public ScoreUseRule GetEntity(string hid, string cardTypeid, string itemScoreid)
        {
            //return _pmsContext.ScoreUseRules.Where(c => c.Hid == hid && c.MbrCardTypeid == cardTypeid && c.ItemScoreid == itemScoreid).FirstOrDefault();
            return _pmsContext.Database.SqlQuery<ScoreUseRule>("SELECT * FROM ScoreUseRule WHERE hid=@grpid AND CHARINDEX(',' +@mbrCardTypeid+',',','+mbrCardTypeid+',')>0 AND  itemScoreid = @itemScoreid"
                                                    , new SqlParameter("@grpid", hid)
                                                    , new SqlParameter("@itemScoreid", itemScoreid)
                                                    , new SqlParameter("@mbrCardTypeid", cardTypeid)
                                                    ).FirstOrDefault();
        }
        public List<KeyValuePair<string,string>> GetListEntity(string grpid,string hid, string cardTypeid)
        {
            //var result = _pmsContext.Database.SqlQuery<System.Web.Mvc.SelectListItem>("SELECT  m.id as Text ,m.name as Value  FROM  dbo.ScoreUseRule AS s LEFT JOIN dbo.itemScore m ON s.itemScoreid=m.id WHERE s.hid=@hid AND s.mbrCardTypeid=@mbrCardTypeid order by m.seqid"
            //                                           , new SqlParameter("@hid", hid)
            //                                           , new SqlParameter("@mbrCardTypeid", cardTypeid)
            //                                           ).ToList();
            var result = _pmsContext.Database.SqlQuery<System.Web.Mvc.SelectListItem>("SELECT m.id as Text ,m.name as Value  FROM  dbo.ScoreUseRule AS s LEFT JOIN dbo.itemScore m ON s.itemScoreid=m.id WHERE s.hid = @grpid AND CHARINDEX(',' + @mbrCardTypeid + ',', ',' + s.mbrCardTypeid + ',')> 0 AND(ISNULL(m.belongHotel, '') = '' OR CHARINDEX(',' + @hid + ',', ',' + m.belongHotel + ',') > 0) ORDER by m.seqid"
                                                    , new SqlParameter("@grpid", grpid)
                                                    , new SqlParameter("@hid", hid)
                                                    , new SqlParameter("@mbrCardTypeid", cardTypeid)
                                                    ).ToList();
            return result.Select(s => new KeyValuePair<string, string>(s.Text, s.Value)).ToList();
        }
    }
}
