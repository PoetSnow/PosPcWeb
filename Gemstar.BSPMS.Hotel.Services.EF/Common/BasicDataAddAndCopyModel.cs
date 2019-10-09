using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.EF.Common
{
    /// <summary>
    /// 基础数据增加并且分发参数
    /// </summary>
    /// <typeparam name="T">基础数据实体类型</typeparam>
    public class BasicDataAddAndCopyModel<T> where T:class
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
        /// 数据库实例
        /// </summary>
        public DbHotelPmsContext DB { get; set; }
        /// <summary>
        /// 基础数据代码
        /// </summary>
        public string BasicDataCode { get; set; }
        /// <summary>
        /// 集团id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 分发类型
        /// </summary>
        public string DataControlCode { get; set; }
        /// <summary>
        /// 选中的要分发的酒店id
        /// </summary>
        public List<string> SelectedResortHids { get; set; }
    }
}
