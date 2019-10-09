using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 基础资料可更新属性标签，只有使用此标签标记了的属性才允许在修改保存分发时进行更新
    /// </summary>
    public class BasicDataUpdateAttribute:Attribute
    {
        /// <summary>
        /// 修改保存时，给操作员选择的属性名称
        /// </summary>
        public string UpdateName { get; set; }
        /// <summary>
        /// 获取指定类别的具有基础资料可更新属性标签的属性列表，用于让操作员选择更新哪些属性
        /// </summary>
        /// <param name="basicDataType">基础数据类型</param>
        /// <returns>指定类型中具有基础资料可更新属性标签的属性列表</returns>
        public static List<SelectListItem> GetBasicDataUpdateAttributeProperties(Type basicDataType)
        {
            var properties = basicDataType.GetProperties();
            var propertyItems = new List<SelectListItem>();
            foreach (var p in properties)
            {
                var updateAttribute = p.GetCustomAttribute(typeof(BasicDataUpdateAttribute));
                if (updateAttribute != null)
                {
                    var item = new SelectListItem
                    {
                        Value = p.Name,
                        Text = ((BasicDataUpdateAttribute)updateAttribute).UpdateName,
                        Selected = true
                    };
                    propertyItems.Add(item);
                }
            }
            return propertyItems;
        }
        /// <summary>
        /// 获取指定类别的具有基础资料可更新属性标签的属性列表，用于当分店不允许修改时，直接获取所有可更新的属性列表
        /// </summary>
        /// <param name="basicDataType">基础数据类型</param>
        /// <returns>指定类型中具有基础资料可更新属性标签的属性列表</returns>
        public static List<string> GetBasicDataUpdateAttributePropertyNames(Type basicDataType)
        {
            var properties = basicDataType.GetProperties();
            var propertyItems = new List<string>();
            foreach (var p in properties)
            {
                var updateAttribute = p.GetCustomAttribute(typeof(BasicDataUpdateAttribute));
                if (updateAttribute != null)
                {
                    propertyItems.Add(p.Name);
                }
            }
            return propertyItems;
        }

    }
}
