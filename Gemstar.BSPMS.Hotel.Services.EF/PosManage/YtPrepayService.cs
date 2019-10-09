using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;


namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class YtPrepayService : CRUDService<YtPrepay>, IYtPrepayService
    {
        private DbHotelPmsContext _db;

        public YtPrepayService(DbHotelPmsContext db) : base(db, db.YtPrepays)
        {
            _db = db;
        }

        protected override YtPrepay GetTById(string id)
        {
            return new YtPrepay { Id = new Guid(id) };
        }


        /// <summary>
        /// 根据收银点生成定金单号
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="Business">营业日</param>
        ///  <param name="module">模块</param>
        /// <returns></returns>
        public string GetBillNo(string hid, DateTime Business, string module)
        {
            var month = Business.Month.ToString().Length == 1 ? "0" + Business.Month.ToString() : Business.Month.ToString();
            var day = Business.Day.ToString().Length == 1 ? "0" + Business.Day.ToString() : Business.Day.ToString();

            var startNo = module + Business.Year + month + day;
            var list = _db.YtPrepays.Where(w => w.Hid == hid && w.DBusiness == Business && w.Module == module && w.IPrepay == (byte)PrePayStatus.交押金).ToList();
            if (list == null || list.Count < 1)
            {
                return startNo + "0001";
            }
            else
            {
                //CY201907110001
                var billCode = "";
                var maxCode = list.ConvertAll(w => Convert.ToInt32(w.BillNo.Substring(10, 4))).Max() + 1;
                if (maxCode.ToString().Length == 1)
                {
                    billCode = "000" + maxCode.ToString();
                }
                else if (maxCode.ToString().Length == 2)
                {
                    billCode = "00" + maxCode.ToString();
                }
                else if (maxCode.ToString().Length == 3)
                {
                    billCode = "0" + maxCode.ToString();
                }
                else
                {
                    billCode = maxCode.ToString();
                }
                return startNo + billCode;
            }

        }

        /// <summary>
        /// 验证单号是否重复
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        public bool isExists(string hid, string billNo)
        {
            var result = _db.YtPrepays.Any(w => w.Hid == hid && w.BillNo == billNo && (w.IType == (byte)PrePayStatus.交押金 || w.IType == (byte)PrePayStatus.待支付));
            return result;
        }

        /// <summary>
        /// 获取押金列表
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        public List<YtPrepay> GetYtPrepayList(string hid, string module)
        {
            var result = _db.YtPrepays.Where(w => w.Hid == hid && w.Module == module && w.IPrepay == 0 && w.IsClear == 0).ToList();
            return result;
        }

        /// <summary>
        /// 获取定金详细信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        public up_pos_getPrePayInfoResult GetPrePayInfoById(string hid, string module, string id)

        {
            var result = _db.Database.SqlQuery<up_pos_getPrePayInfoResult>("exec up_pos_getPrePayInfo @hid=@hid,@Module=@Module,@PrePayId=@PrePayId",
                            new SqlParameter("@hid", hid),
                            new SqlParameter("@Module", module),
                            new SqlParameter("@PrePayId", id)
                            ).FirstOrDefault();
            return result;
        }


        public void UpdatePrePayStatus(string hid, string billNo, string module)
        {

            _db.Database.ExecuteSqlCommand("Update ytPrepay SET isClear=1 where hid=@hid and module=@module and billNo=@billno and IPrepay in(0,1,2)"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@module", module)
                , new SqlParameter("@billno", billNo));

        }

        /// <summary>
        /// 判断是否存在定金买单以及定金退款的数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        public bool isExistsPay(string hid, string billNo)
        {
            var list = _db.YtPrepays.Where(w => w.Hid == hid && w.BillNo == billNo && (w.IPrepay == (byte)PrePayStatus.押金付款 || w.IPrepay == (byte)PrePayStatus.退押金)).ToList();
            if (list != null && list.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据付款单号查询押金付款的数据
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="paidNo">posbillDetail 中的主键ID </param>
        /// <returns></returns>
        public List<YtPrepay> GetModelByPaidNo(string hid, string paidNo)
        {
            return _db.YtPrepays.Where(w => w.Hid == hid && w.PaidNo == paidNo && (w.IPrepay == (byte)PrePayStatus.押金付款 || w.IPrepay == (byte)PrePayStatus.退押金)).ToList();
        }

        /// <summary>
        /// 根据状态获取定金信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billNo"></param>
        /// <param name="module"></param>
        /// <param name="status"></param>
        /// <returns></returns>
      public  YtPrepay GetPreModel(string hid, string billNo, string module, PrePayStatus status)
        {
            return _db.YtPrepays.Where(w => w.Hid == hid && w.BillNo == billNo && w.Module == module && w.IPrepay == (byte)status).FirstOrDefault();
        }
    }

}
