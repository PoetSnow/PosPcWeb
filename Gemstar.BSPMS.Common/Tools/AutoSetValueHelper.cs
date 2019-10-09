using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 在对象之间自动赋值的辅助类
    /// </summary>
    public static class AutoSetValueHelper
    {
        /// <summary>
        /// 自动赋值，只有在源对象和目标对象具有相同属性，才赋值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void SetValues(object source, object target)
        {
            var sourceProperties = source.GetType().GetProperties();
            var targetProperties = target.GetType().GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.SingleOrDefault(w => w.Name.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (targetProperty != null)
                {
                    if (targetProperty.PropertyType == sourceProperty.PropertyType)
                    {
                        targetProperty.SetValue(target, sourceProperty.GetValue(source));
                    }
                    else
                    {
                        SetValueByConvert(sourceProperty, targetProperty, source, target);
                    }
                }
            }
        }
        /// <summary>
        /// 自动赋值，只有在源对象和目标对象具有相同属性，才赋值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="propertyNames">要更新的属性名称列表</param>
        public static void SetValues(object source, object target,List<string> propertyNames)
        {
            var sourceProperties = source.GetType().GetProperties();
            var targetProperties = target.GetType().GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                if (propertyNames.Contains(sourceProperty.Name))
                {
                    var targetProperty = targetProperties.SingleOrDefault(w => w.Name.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                    if (targetProperty != null)
                    {
                        if (targetProperty.PropertyType == sourceProperty.PropertyType)
                        {
                            targetProperty.SetValue(target, sourceProperty.GetValue(source));
                        }
                        else
                        {
                            SetValueByConvert(sourceProperty, targetProperty, source, target);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 自动赋值，只有在源对象和目标对象具有相同属性，并且此属性名称在values的键值中存在时，才赋值
        /// </summary>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="values">传递回来的键值对集合</param>
        public static void SetValues(object source, object target, NameValueCollection values, out List<string> settedValuesFieldNames)
        {
            var sourceProperties = source.GetType().GetProperties();
            var targetProperties = target.GetType().GetProperties();
            var keys = values.AllKeys;
            settedValuesFieldNames = new List<string>();
            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.SingleOrDefault(w => w.Name.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                var key = keys.SingleOrDefault(w => w.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (targetProperty != null && key != null)
                {
                    if (targetProperty.PropertyType == sourceProperty.PropertyType)
                    {
                        targetProperty.SetValue(target, sourceProperty.GetValue(source));
                    }
                    else
                    {
                        SetValueByConvert(sourceProperty, targetProperty, source, target);
                    }
                    settedValuesFieldNames.Add(targetProperty.Name);
                }
            }
        }
        private static void SetValueByConvert(PropertyInfo sourceProperty, PropertyInfo targetProperty, object source, object target)
        {
            var targetPropertyType = targetProperty.PropertyType;
            var sourcePropertyType = sourceProperty.PropertyType;
            if (targetPropertyType == typeof(Guid))
            {
                var sourceValue = sourceProperty.GetValue(source);
                var targetValue = Guid.Parse(sourceValue.ToString());
                targetProperty.SetValue(target, targetValue);
            }
            else if (targetPropertyType == typeof(string))
            {
                var sourceValue = sourceProperty.GetValue(source);
                var targetValue = sourceValue.ToString();
                targetProperty.SetValue(target, targetValue);
            }
            else if (targetPropertyType == typeof(byte) && sourcePropertyType == typeof(bool))
            {
                var sourceValue = sourceProperty.GetValue(source);
                bool value;
                if (bool.TryParse(sourceValue.ToString(), out value))
                {
                    targetProperty.SetValue(target, (byte)(value ? 1 : 0));
                }
                else
                {
                    targetProperty.SetValue(target, byte.Parse(sourceValue.ToString()));
                }
            }
            else if (targetPropertyType == typeof(bool) && sourcePropertyType == typeof(byte))
            {
                var sourceValue = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, sourceValue.ToString() == "1");
            }
            else if (targetPropertyType == typeof(Services.EntityStatus) && sourcePropertyType == typeof(byte))
            {
                var sourceValue = sourceProperty.GetValue(source);
                targetProperty.SetValue(target, byte.Parse(sourceValue.ToString()));
            }
            else if (targetPropertyType == typeof(byte) && sourcePropertyType == typeof(Services.EntityStatus))
            {
                var sourceValue = sourceProperty.GetValue(source);
                Array enumValues = Enum.GetValues(sourcePropertyType);
                foreach (Enum enumValue in enumValues)
                {
                    if(enumValue.ToString()== sourceValue.ToString())
                    {
                        targetProperty.SetValue(target, Convert.ToByte(enumValue));
                        break;
                    }
                }
            }
            else
            {
                throw new ArgumentException(string.Format("无法从类型{0}转换到{1}后进行赋值，请增加类型转换赋值规则", sourcePropertyType, targetPropertyType));
            }
        }
    }
}
