namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_scan_list_ItemListBySubClassid 执行结果
    /// </summary>
    public class up_pos_scan_list_ItemListBySubClassidResult
    {
        /// <summary>
        /// 消费项目id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消费项目代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 消费项目中文名
        /// </summary>
        public string Cname { get; set; }

        /// <summary>
        /// 消费项目分类ID
        /// </summary>
        public string SubClassid { get; set; }

        /// <summary>
        /// 消费项目分类名称
        /// </summary>
        public string SubClassName { get; set; }

        /// <summary>
        /// 消费项目价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 显示作法
        /// </summary>
        public bool? IsAutoAction { get; set; }

        /// <summary>
        /// 是否有多个单位
        /// </summary>
        public bool? IsMultiUnit { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Bmp { get; set; }

        /// <summary>
        /// 消费项目备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否打折
        /// </summary>
        public bool IsDiscount { get; set; }

        /// <summary>
        /// 是否收服务费
        /// </summary>
        public bool IsService { get; set; }

        /// <summary>
        /// 消费项目默认单位ID
        /// </summary>
        public string UnitId { get; set; }

        /// <summary>
        /// 消费项目默认单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 消费项目默认单位代码
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// 消费项目默认单位价格
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}