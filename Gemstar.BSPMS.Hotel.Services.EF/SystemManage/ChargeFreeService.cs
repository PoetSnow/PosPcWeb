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
    public class ChargeFreeService : CRUDService<ChargeFree>, IChargeFreeService
    {
        public ChargeFreeService(DbHotelPmsContext db) : base(db, db.ChargeFrees)
        {
            _pmsContext = db;
        }
        protected override ChargeFree GetTById(string id)
        {
            return new ChargeFree { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 根据充值赠送规则 计算赠送金额
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardTypeId">卡类型ID</param>
        /// <param name="money">充值金额</param>
        /// <returns></returns>
        public decimal GetSendMoney(string hid, string cardTypeId, decimal money)
        {
            var entity = _pmsContext.ChargeFrees.Where(c => c.Hid == hid && c.MbrCardTypeid == cardTypeId && c.BeginAmount <= money && c.EndAmount >= money).FirstOrDefault();
            if (entity != null)
            {
                return Math.Round((entity.Amount == null ? 0 : (decimal)entity.Amount) + (money * (entity.Rate == null ? 0 : (decimal)entity.Rate)), 2);
            }
            return 0;
        }

    }


}
