namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// up_pos_scan_list_PosItemByShowSet 根据酒店查询扫码点餐的餐台列表
    /// </summary>
    public class up_pos_scan_list_PosItemByShowSetResult
    {
        /// <summary>
        /// 消费项目ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string Cname { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string Ename { get; set; }

        /// <summary>
        /// 部门类别
        /// </summary>
        public string DeptClassName { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string SubClassName { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Bmp { get; set; }

        /// <summary>
        /// 显示设置
        /// </summary>
        public string ShowSet { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}