namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_itemByItemClassidResult 执行后的结果集对象
    /// </summary>
    public class up_pos_list_itemByItemClassidResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string Cname { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string Ename { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 是否是子分类
        /// </summary>
        public bool? isSubClass { get; set; }

        /// <summary>
        /// 是否是子分类
        /// </summary>
        public bool? isDiscount { get; set; }

        /// <summary>
        /// 是否是子分类
        /// </summary>
        public bool? isService { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Seqid { get; set; }

        /// <summary>
        /// 是否手写单
        /// </summary>
        public bool? isHandWrite { get; set; }

        /// <summary>
        /// 是否可手工录入手写菜名
        /// </summary>
        public bool? isInput { get; set; }

        /// <summary>
        /// 显示编码
        /// </summary>
        public bool? isshowcode { get; set; }

        /// <summary>
        /// 显示价格
        /// </summary>
        public bool? isshowprice { get; set; }

        /// <summary>
        /// 是否套餐
        /// </summary>
        public bool? isSuite { get; set; }

        /// <summary>
        /// 是否酒席
        /// </summary>
        public bool? isFeast { get; set; }

        /// <summary>
        /// 自定义套餐/酒席
        /// </summary>
        public bool? isUserDefined { get; set; }

        /// <summary>
        /// 沽清表状态
        /// </summary>
        public byte? sellStatus { get; set; }

        /// <summary>
        /// 时价菜
        /// </summary>
        public bool? isCurrent { get; set; }

        /// <summary>
        /// 特价菜日期类型
        /// </summary>
        public string itagperiod { get; set; }


        /// <summary>
        /// 是否称重
        /// </summary>
        public bool? isWeight { get; set; }

        /// <summary>
        /// 是否海鲜
        /// </summary>
        public bool? isSeaFood { get; set; }

        /// <summary>
        /// 是否输入数量
        /// </summary>
        public bool? isQuan { get; set; }
    }
}