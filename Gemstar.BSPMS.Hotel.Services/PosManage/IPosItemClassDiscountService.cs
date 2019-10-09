using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
   public interface IPosItemClassDiscountService : ICRUDService<PosOnSale>
    {

        /// <summary>
        /// 检测是否有冲突的大类折扣记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemClassid">消费项目大类ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        bool IsExists(string hid, string module, string itemid, string itemClassid,string CustomerTypeid,string Refeid,string TabTypeid,string Unitid, string starttime, string endtime);


        /// <summary>
        /// 检测是否有冲突的大类折扣记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemClassid">消费项目大类ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        bool IsExists(string hid, string module, string itemid, string itemClassid, string CustomerTypeid, string Refeid, string TabTypeid, string Unitid, string starttime, string endtime, Guid exceptId);


        /// <summary>
        /// 获取大类折扣 
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="wherefunc">表达式</param>
        /// <returns></returns>
        List<PosOnSale> GetItemClassDisCount(string hid, Func<PosOnSale, bool> wherefunc);


        //获取会员卡类型
        /// <summary>
        /// 会员卡类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="wherefunc">lambda表达式</param>
        /// <returns></returns>
        List<MbrCardType> GetMbrCardTypes(string hid, Func<MbrCardType, bool> wherefunc);

        //获取会员信息
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="Hid">酒店ID</param>
        /// <param name="CardID">会员卡ID</param>
        /// <param name="RefeCode">营业点代码</param>
        /// <returns></returns>
        JsonResultData GetMbrCardInfoByCardID(string Hid, string CardID, string RefeCode);

        //获取会员折扣方式
        /// <summary>
        /// 获取捷云会员折扣方式
        /// </summary>
        /// <param name="typecode">会员折扣方式代码</param>
        /// <returns></returns>
        List<v_codeListPubModel> GetMemberDisCountType(string typecode = "");

        /// <summary>
        /// 会员折扣计算
        /// </summary>
        /// <param name="mbrCardInfo">会员信息</param>
        /// <param name="ItemClassDiscount">会员项目大类折扣</param>
        /// <param name="ItemDiscount">会员项目折扣</param>
        /// <param name="OldDisCount">原折扣</param>
        /// <param name="IsHasItemClassDisCount">是否有会员项目大类折扣</param>
        /// <param name="IsHasItemDisCount">是否有会员项目折扣</param>
        /// <param name="ItemIsDiscount">会员大类折扣</param>
        /// <returns>最终的折扣</returns>
        decimal CalculateMemberItemDisCount(MbrCardInfoModel mbrCardInfo, decimal ItemClassDiscount, decimal ItemDiscount, decimal OldDisCount, bool IsHasItemClassDisCount, bool IsHasItemDisCount, bool ItemIsDiscount);

        /// <summary>
        /// 会员大类折扣计算
        /// </summary>
        /// <param name="mbrCardInfo">会员信息</param>
        /// <param name="ItemClassDiscount">会员项目大类折扣</param>
        /// <param name="OldDisCount">原折扣</param>
        /// <param name="IsHasItemClassDisCount">是否有会员项目大类折扣</param>
        /// <param name="ItemIsDiscount">会员大类折扣</param>
        /// <returns>最终的折扣</returns>
        decimal CalculateMemberItemClassDisCount(MbrCardInfoModel mbrCardInfo, decimal ItemClassDiscount, decimal OldDisCount, bool IsHasItemClassDisCount);


        //会员大类折扣记录的时间筛选
        /// <summary>
        /// 待筛选的时间记录
        /// </summary>
        /// <param name="posOnSales"></param>
        /// <returns></returns>
        List<PosOnSale> MemberDisCountTimeFileter(List<PosOnSale> posOnSales);

    }
}
