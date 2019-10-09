using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.MbrCardCenter
{
    public interface IMbrCardService : ICRUDService<MbrCard>
    {
        /// <summary>
        /// 检查会员卡号是否已存在
        /// </summary>
        /// <param name="cardNo">会员卡号</param>
        /// <param name="profileId">排除会员ID</param>
        /// <returns>true已存在,false不存在</returns>
        bool RepeatCheck(string cardNo, string profileId = null);
        bool RepeatCheckCar(string InductionCar, string profileId = null);
        bool CheckMobile(string mobile,string id);
        bool CheckCer(string cerType, string cerid);
        bool RepeatChecks(string cardNo, string InductionCar);
        /// <summary>
        /// 根据会员卡号获取会员信息
        /// </summary>
        /// <param name="mbrNo"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        MbrCard GetMbrCar(string mbrNo, string hid);
        /// <summary>
        /// 获取会员账务信息
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <returns></returns>
        MbrCardBalance GetCardBalance(Guid id);
        /// <summary>
        /// 获取集团ID
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        string GetGrpid(string hid);
        /// <summary>
        /// 根据券号查询优惠券
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <returns></returns>
        ProfileCard GetProfileCard(string ticketNo);
        /// <summary>
        /// 通过优惠券类型获取券信息
        /// </summary>
        /// <param name="ticketTypeId"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        Coupon GetTicket(string ticketTypeId, string hid);
        /// <summary>
        /// 通过现金券号获取金额
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <returns></returns>
        decimal? GetCouponMoney(string ticketNo);
        /// <summary>
        /// 查询本酒店是否有此会员
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        bool IsHotelMbr(Guid profileId);
        /// <summary>
        /// 获取会员账务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<ProfileTrans> GetProfileTrans(Guid id);
        /// <summary>
        /// 获取会员消费记录 排除反结
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<ProfileTrans> GetProfileTransRes(Guid id);
        /// <summary>
        /// 更新会员实体单个项的值,包括（卡号，状态，审核状态，业务员）
        /// </summary>
        /// <param name="attributeName">会员实体属性名</param>
        /// <param name="model">会员实体</param>
        /// <returns></returns>
        JsonResultData UpdateSingle(string attributeName, MbrCard model);

        /// <summary>
        /// 变更卡类型
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="cardTypeId">新的会员卡类型ID</param>
        /// <param name="score">扣减积分数额</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData CardUpgrade(Guid id, string cardTypeId, int score, string remark);

        /// <summary>
        /// 卡费收取
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="payMoney">卡费金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData CardFee(Guid id, string payWayId, decimal payMoney, string invNo, string remark, decimal? originPayAmount = null, string refno = null);
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="payMoney">充值金额</param>
        /// <param name="sendMoney">赠送金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData Recharge(Guid id, string payWayId, decimal payMoney, decimal sendMoney, string invNo, string remark, decimal? originPayAmount = null, string refno = null);

        /// <summary>
        /// 扣款
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="accountType">会员账户类型（01储值，02赠送金额）</param>
        /// <param name="money">扣款金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData SubtractMoney(Guid id, string accountType, decimal money, string invNo, string remark);

        /// <summary>
        /// 积分调整
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="accountType">会员账户类型（11积分，12业主分）</param>
        /// <param name="score">调整积分数量（正数调增，负数调减）</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData ScoreAction(Guid id, string accountType, decimal score, string invNo, string remark);

        /// <summary>
        /// 积分兑换
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="itemId">兑换项目ID</param>
        /// <param name="score">扣除积分</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData ScoreUse(Guid id, string itemId, int score, string invNo, string remark);

        /// <summary>
        /// 积分兑换
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="itemId">兑换项目ID</param>
        /// <param name="score">扣除积分</param>
        /// <param name="money">扣除金额</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        JsonResultData ScoreUse(Guid id, string itemId, int score, decimal money, string payWayId, string invNo, string remark, decimal? originMoney = null);

        /// <summary>
        /// 会员列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="nameOrMobile">会员名称或手机号码</param>
        /// <returns></returns>
        IQueryable<MbrCard> List(string hid, string nameOrMobile);
        //会员卡有效期延期
        JsonResultData DelayValidDate(string ids, DateTime time);
        //修改业务员
        JsonResultData UpdateSales(string ids, string sale, string remark);
        //积分调整  
        JsonResultData UpdateScore(string ids, string score, string accounttype, string invno, string remark);
        /// <summary>
        /// 批量发放优惠券
        /// </summary>
        /// <param name="ids">会员号</param>
        /// <param name="ticketTypeid">券类型</param>
        /// <param name="number">数量</param>
        /// <returns></returns>
        JsonResultData GiveCoupons(string id, string ticketTypeid, int number,string remarks);
        /// <summary>
        /// 根据主键ID获取会员信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">会员主键ID</param>
        /// <returns></returns>
        JsonResultData GetMbrCardInfo(string hid, Guid id);
        /// <summary>
        /// 修改会员交易记录的付款方式
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        JsonResultData UpdateProfileCa(string hid, Guid id, string itemid, string remark);
        /// <summary>
        /// 会员充值记录退款
        /// </summary>
        /// <param name="para">会员充值退款参数，具体字段含义见参数类字段说明</param>
        /// <returns>充值记录退款结果</returns>
        JsonResultData Refund(RechargeRefundPara para);
        /// <summary>
        /// 得到是业主的会员信息
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<MbrCard> ListforOwner(string hid,string text);
        /// <summary>
        /// 获取会员列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardNo"></param>
        /// <param name="inductionCar"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        List<MbrCard> GetMbrCard(Guid? id = null, string cardNo = "", string inductionCar = "", string mobile = "", string cerType = "", string cerid = "");
        /// <summary>
        /// 发送会员营销短信
        /// </summary>
        /// <param name="ids">会员号</param>
        /// <param name="mobiles">手机号</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        JsonResultData SendMarketSms(string ids, string mobiles, string content);

        /// <summary>
        /// 根据指定的会员 获取会员卡类型名称
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <returns></returns>
        string GetMbrCardTypeNameById(Guid id);
    }
}
