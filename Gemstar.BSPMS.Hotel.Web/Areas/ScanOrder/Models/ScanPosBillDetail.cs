using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    public class ScanPosBillDetail
    {
        /// <summary>
        /// 排序字段。用于本地保存数据记录行号与作法进行匹配
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 上级行号。用于套餐明细与套餐匹配
        /// </summary>
        public int TopOrderId { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public string Itemid { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 单位id
        /// </summary>
        public string Unitid { get; set; }

        /// <summary>
        /// 单位代码
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// 称重条只
        /// </summary>
        public decimal? Piece { get; set; }

        /// <summary>
        /// /扣钝倍数
        /// </summary>
        public decimal? Multiple { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 作法加价
        /// </summary>
        public decimal? AddPrice { get; set; }

        /// <summary>
        /// 折前金额
        /// </summary>
        public decimal? Dueamount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        public decimal? Service { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal? Tax { get; set; }

        /// <summary>
        /// 客位
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 作法
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 要求
        /// </summary>
        public string Request { get; set; }



        /// <summary>
        /// 自动标志
        /// </summary>
        public byte? Isauto { get; set; }

        /// <summary>
        /// 计费状态
        /// </summary>
        public byte? Status { get; set; }



        /// <summary>
        /// 求和套餐
        /// </summary>
        public bool? SP { get; set; }

        /// <summary>
        /// 求和套餐明细
        /// </summary>
        public bool? SD { get; set; }

        /// <summary>
        /// 所属套餐
        /// </summary>
        public Guid? Upid { get; set; }

        /// <summary>
        /// 套餐分摊金额
        /// </summary>
        public decimal? OverSuite { get; set; }

        /// <summary>
        /// 餐台id
        /// </summary>
        public string Tabid { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string Sale { get; set; }

        /// <summary>
        /// 修改操作员
        /// </summary>
        public string ModiUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModiDate { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal? PriceOri { get; set; }

        /// <summary>
        /// 会员价
        /// </summary>
        public decimal? PriceClub { get; set; }

        /// <summary>
        /// 会员折扣
        /// </summary>
        public decimal? DiscountClub { get; set; }

        /// <summary>
        /// 批准人
        /// </summary>
        public string Approver { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        public string CanReason { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 手工单号
        /// </summary>
        public string Acbillno { get; set; }

        /// <summary>
        /// 称重原数量
        /// </summary>

        public decimal? OriQuan { get; set; }

        /// <summary>
        /// 是否单道折扣
        /// </summary>
        public bool? IsDishDisc { get; set; }


        /// <summary>
        /// 是否收取服务费
        /// </summary>
        public bool? IsService { get; set; }

        /// <summary>
        /// 消费项目大类
        /// </summary>
        public string itemClassId { get; set; }



    }
}