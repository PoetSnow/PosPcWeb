using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IScoreUseRuleService : ICRUDService<ScoreUseRule>
    {
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardTypeid">会员卡类型ID</param>
        /// <param name="itemScoreid">兑换项目ID</param>
        /// <returns></returns>
        ScoreUseRule GetEntity(string hid, string cardTypeid, string itemScoreid);

        List<KeyValuePair<string,string>> GetListEntity(string grpid,string hid, string cardTypeid);
    }
}
