using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.EF.Common
{
    /// <summary>
    /// 基础数据状态启用禁用并且分发模型
    /// </summary>
    public class BasicDataStatusChangeAndCopyModel<T> where T:class, IEntityEnable
    {
        /// <summary>
        /// 集团基础数据实例
        /// </summary>
        public T GroupModel { get; set; }
        /// <summary>
        /// 增删改服务实例
        /// </summary>
        public ICRUDService<T> CRUDService { get; set; }
        /// <summary>
        /// 基础数据分发接口实例，一般与增删改服务实例是同一实例
        /// </summary>
        public IBasicDataCopyService<T> BasicDataService { get; set; }
        /// <summary>
        /// 基础数据实体新状态
        /// </summary>
        public EntityStatus Status { get; set; }
    }
}
