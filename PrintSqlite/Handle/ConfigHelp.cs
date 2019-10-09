﻿using PrintSqlite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PrintSqlite
{
    /// <summary>
    /// 配置文件帮助类，对App.config进行配置项的增、删、改、查等操作
    /// </summary>
    public class ConfigHelp
    {
        /// <summary>
        /// 判断键为keyName的项是否存在
        /// </summary>
        /// <param name="keyName">键</param>
        /// <returns></returns>
        public static bool IsExist(string keyName)
        {
            try
            {
                //判断配置文件中是否存在键为keyName的项  
                foreach (string key in ConfigurationManager.AppSettings)
                {
                    if (key == keyName)
                    {
                        //存在  
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
            }
            return false;
        }

        /// <summary>
        /// 添加配置项
        /// </summary>
        /// <param name="keyName">键</param>
        /// <param name="keyValue">值</param>
        public static void AddItem(string keyName, string keyValue)
        {
            try
            {
                //添加配置文件的项，键为keyName，值为keyValue  
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add(keyName, keyValue);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
            }
        }

        /// <summary>
        /// 修改键为keyName的项的值
        /// </summary>
        /// <param name="keyName">键</param>
        /// <param name="newKeyValue">值</param>
        public static void UpdateItem(string keyName, string newKeyValue)
        {
            try
            {
                //修改配置文件中键为keyName的项的值  
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[keyName].Value = newKeyValue;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
            }
        }

        /// <summary>
        /// 删除键为keyName的项
        /// </summary>
        /// <param name="keyName">键</param>
        public static void RemoveItem(string keyName)
        {
            try
            {
                //删除配置文件键为keyName的项  
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove(keyName);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
            }
        }

        /// <summary>
        /// 获取键为keyName的项的值
        /// </summary>
        /// <param name="keyName">键</param>
        /// <returns></returns>
        public static string GetItem(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName];
        }

        /// <summary>
        /// 获取所有的AppSetting
        /// </summary>
        public static List<AppParaModel> GetAppsetting()
        {
            var apppara = new List<AppParaModel>();
            foreach (string item in ConfigurationManager.AppSettings.Keys)
            {
                apppara.Add(new AppParaModel() { Name = item, Value= ConfigurationManager.AppSettings[item]});
            }
            return apppara;
        }

    }
}