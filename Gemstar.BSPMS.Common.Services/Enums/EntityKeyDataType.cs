using System;

namespace Gemstar.BSPMS.Common.Services.Enums
{
    /// <summary>
    /// 实体主键类型
    /// </summary>
    public enum EntityKeyDataType
    {
        /// <summary>
        /// GUID类型
        /// </summary>
        GUID,
        /// <summary>
        /// 字符串类型
        /// </summary>
        String,
        /// <summary>
        /// int类型
        /// </summary>
        Int
    }
    public static class EntityKeyHelper
    {
        /// <summary>
        /// 获取主键类型值
        /// </summary>
        /// <param name="keyStr">主键字符串</param>
        /// <param name="dataType">主键数据类型</param>
        /// <returns>转换为对应类型后的值</returns>
        public static object GetKeyValue(string keyStr, EntityKeyDataType dataType)
        {
            object result = keyStr;
            switch (dataType){
                case EntityKeyDataType.GUID:
                    result = Guid.Parse(keyStr);
                    break;
                case EntityKeyDataType.Int:
                    result = Convert.ToInt32(keyStr);
                    break;
            }
            return result;
        }
    }
}
