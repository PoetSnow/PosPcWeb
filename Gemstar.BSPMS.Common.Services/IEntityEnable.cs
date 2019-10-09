namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 实体可禁用启用接口
    /// </summary>
    public interface IEntityEnable
    {
        /// <summary>
        /// 实体状态
        /// </summary>
        EntityStatus Status { get; set; }
    }
}
