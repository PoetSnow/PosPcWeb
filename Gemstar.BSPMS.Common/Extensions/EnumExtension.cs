using Gemstar.BSPMS.Common.Enumerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Extensions
{
    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举值对应的description属性值
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>对应的description属性值</returns>
        public static string GetDescription(this Enum value)
        {
            var enumType = value.GetType();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }

            var name = value.ToString();

            FieldInfo field = enumType.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute.Description;
        }
        /// <summary>
        /// 获取枚举值对应的description属性值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="nameOrValue">值或名称</param>
        /// <returns>对应的description属性值</returns>
        public static string GetDescription(Type enumType, string nameOrValue)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            var value = Enum.Parse(enumType, nameOrValue).ToString();

            FieldInfo field = enumType.GetField(value);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute.Description;
        }
        /// <summary>
        /// 将枚举转化为下拉列表数据
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="valueType">下拉列表项的值的取值类型</param>
        /// <param name="textType">下拉列表项的文本的取值类型</param>
        /// <returns>对应的下拉列表数据</returns>
        public static List<SelectListItem> ToSelectList(Type enumType,EnumValueType valueType = EnumValueType.Value,EnumValueType textType = EnumValueType.Text)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            var result = new List<SelectListItem>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum enumValue in enumValues)
            {
                var value = Convert.ToInt32(enumValue).ToString();
                var text = enumValue.ToString();

                FieldInfo field = enumType.GetField(text);
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                var listItem = new SelectListItem();
                switch (valueType)
                {
                    case EnumValueType.Value:
                        listItem.Value = value;
                        break;
                    case EnumValueType.Text:
                        listItem.Value = text;
                        break;
                    case EnumValueType.Description:
                        listItem.Value = attribute.Description;
                        break;
                    default:
                        listItem.Value = value;
                        break;
                }
                switch (textType)
                {
                    case EnumValueType.Value:
                        listItem.Text = value;
                        break;
                    case EnumValueType.Text:
                        listItem.Text = text;
                        break;
                    case EnumValueType.Description:
                        listItem.Text = attribute.Description;
                        break;
                    default:
                        listItem.Text = text;
                        break;
                }
                result.Add(listItem);
            }
            return result;
        }
        
    }
    /// <summary>
    /// 枚举类型的取值类型
    /// </summary>
    public enum EnumValueType
    {
        /// <summary>
        /// 取枚举类型对应的数值值
        /// </summary>
        Value,
        /// <summary>
        /// 取枚举类型对应的文本值
        /// </summary>
        Text,
        /// <summary>
        /// 取枚举类型对应的description属性值
        /// </summary>
        Description
    }
}
