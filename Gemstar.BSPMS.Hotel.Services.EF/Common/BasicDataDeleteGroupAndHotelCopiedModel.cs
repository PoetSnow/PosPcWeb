using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.EF.Common
{
    /// <summary>
    /// 同时删除集团记录和对应的分发记录
    /// </summary>
    /// <typeparam name="T">基础数据类型</typeparam>
    public class BasicDataDeleteGroupAndHotelCopiedModel<T> where T : class
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
    }
}
