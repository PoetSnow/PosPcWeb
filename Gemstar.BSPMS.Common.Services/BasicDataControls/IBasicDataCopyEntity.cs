namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 需要进行集团分发的基础资料实体接口，用于规范分发相关的属性，以便代码重用
    /// </summary>
    public interface IBasicDataCopyEntity
    {
        /// <summary>
        /// 数据来源，是自主增加还是集团分发
        /// </summary>
        string DataSource { get; set; }
        /// <summary>
        /// 酒店id
        /// </summary>
        string Hid { get; set; }
    }
}
