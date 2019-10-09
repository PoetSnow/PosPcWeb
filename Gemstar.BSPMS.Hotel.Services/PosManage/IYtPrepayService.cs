using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IYtPrepayService : ICRUDService<YtPrepay>
    {
        /// <summary>
        /// 根据收银点生成定金单号
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="Business">营业日</param>
        ///  <param name="module">模块</param>
        /// <returns></returns>
        string GetBillNo(string hid, DateTime Business, string module);

        /// <summary>
        /// 验证单号是否唯一
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        bool isExists(string hid, string billNo);

        /// <summary>
        /// 获取押金列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        List<YtPrepay> GetYtPrepayList(string hid, string module);

        /// <summary>
        /// 获取定金详细信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        up_pos_getPrePayInfoResult GetPrePayInfoById(string hid, string module, string id);

        /// <summary>
        ///修改押金状态为已结
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billNo">押金单号</param>
        /// <param name="module">模块</param>
        void UpdatePrePayStatus(string hid, string billNo, string module);

        /// <summary>
        /// 判断是否存在定金买单以及定金退款的数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        bool isExistsPay(string hid, string billNo);

        /// <summary>
        /// 根据付款单号查询押金付款的数据
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="paidNo">posbillDetail 中的主键ID </param>
        /// <returns></returns>
        List<YtPrepay> GetModelByPaidNo(string hid, string paidNo);

        /// <summary>
        /// 根据状态获取定金信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billNo"></param>
        /// <param name="module"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        YtPrepay GetPreModel(string hid, string billNo,string module, PrePayStatus status);
    }
}
