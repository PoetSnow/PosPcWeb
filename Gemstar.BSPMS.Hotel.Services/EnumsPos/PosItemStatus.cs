using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 1:提交；2：审核；51：禁用
    /// </summary>
    public enum PosItemStatus : byte
    {
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        提交 = 1,
        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        审核 = 2,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        禁用 = 51,
    }
}
