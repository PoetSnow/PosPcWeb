using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosBillOrderService : CRUDService<PosBillOrder>, IPosBillOrderService
    {
        private DbHotelPmsContext _db;

        public PosBillOrderService(DbHotelPmsContext db) : base(db, db.PosBillOrders)
        {
            _db = db;
        }

        protected override PosBillOrder GetTById(string id)
        {
            return new PosBillOrder { Id = new Guid(id) };
        }


        /// <summary>
        /// 获取预定信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单Id</param>
        /// <param name="columnName">列名</param>
        /// <param name="tabName">表名</param>
        /// <returns></returns>
        public PosBillOrder GetBillOrder(string hid, string billId, string columnName, string tabName)
        {
            return _db.PosBillOrders.Where(w => w.Hid == hid && w.Billid == billId && w.ColumnName == columnName && w.TableName == tabName).FirstOrDefault();
        }
    }
}
