using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.EF.Common
{
    /// <summary>
    /// 基础数据修改并且分发参数
    /// </summary>
    /// <typeparam name="T">基础数据实体类型</typeparam>
    public class BasicDataEditAndCopyModel<T>: BasicDataAddAndCopyModel<T> where T:class
    {
        /// <summary>
        /// 原始集团记录
        /// </summary>
        public T OriginGroupModel { get; set; }
        /// <summary>
        /// 集团记录修改字段列表
        /// </summary>
        public List<string> GroupModelUpdateFieldNames { get; set; }
        /// <summary>
        /// 分店分发属性列表
        /// </summary>
        public List<string> CopyedUpdateProperties { get; set; }
    }
}
