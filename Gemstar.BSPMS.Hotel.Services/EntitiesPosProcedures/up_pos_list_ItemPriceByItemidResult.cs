using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_ItemPriceByItemid 执行后的结果集对象
    /// </summary>
    public class up_pos_list_ItemPriceByItemidResult
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>

        public string itemId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string itemName { get; set; }
        /// <summary>
        /// 单位id
        /// </summary>
        public string Unitid { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 单位代码
        /// </summary>
        public string UnitCode { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public decimal? Multiple { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? Grossrate { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// 油味
        /// </summary>
        public decimal? OilAmount { get; set; }
        /// <summary>
        /// 提成
        /// </summary>
        public decimal? Percent { get; set; }
        /// <summary>
        /// 会员价
        /// </summary>
        public decimal? MemberPrice { get; set; }
        /// <summary>
        /// 是否缺省单位
        /// </summary>
        public string IsDefault { get; set; }
        /// <summary>
        /// 所属餐台类型id
        /// </summary>
        public string tabTypeName { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        public int? Seqid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string Modified { get; set; }

        /// <summary>
        /// 沽清表状态
        /// </summary>
        public string SelloutStatus { get; set; }
    }
}
