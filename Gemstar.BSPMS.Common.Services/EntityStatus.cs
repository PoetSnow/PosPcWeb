using System.ComponentModel;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 实体类型的状态
    /// 判断实体是否可用是根据状态值小于禁用值51来判断
    /// </summary>
    public enum EntityStatus:byte
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        启用 = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        禁用 = 51,
        /// <summary>
        /// 销户
        /// </summary>
        [Description("销户")]
        销户 = 60
    }
}
