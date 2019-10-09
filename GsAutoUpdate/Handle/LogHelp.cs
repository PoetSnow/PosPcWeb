using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GsAutoUpdate
{
    public static class LogHelp
    {
        static string temp = AppDomain.CurrentDomain.BaseDirectory;
        public static void AddLog(String value)
        {
            Debug.WriteLine(value);
            if (Directory.Exists(Path.Combine(temp, @"log\")) == false)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(temp, @"log\"));
                directoryInfo.Create();
            }
            using (StreamWriter sw = File.AppendText(Path.Combine(temp, @"log\update.log")))
            {
                sw.WriteLine($"日志({DateTime.Now})：{value}");
            }
        }

        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="ex">异常类</param>
        /// <param name="path">日志文件存放路径</param>
        public static void AddException(Exception ex)
        {
            try
            {
                if (Directory.Exists(Path.Combine(temp, @"log\")) == false)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(temp, @"log\"));
                    directoryInfo.Create();
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(temp, @"log\update.log")))
                {
                    sw.WriteLine("**********【" + DateTime.Now + "】**********");
                    if (ex != null)
                    {
                        sw.WriteLine("【异常数据】" + ex.Data);
                        sw.WriteLine("【资源来源】" + ex.Source);
                        sw.WriteLine("【信息提示】" + ex.Message);
                        sw.WriteLine("【堆栈跟踪】" + ex.StackTrace);
                        sw.WriteLine("【目标位置】" + ex.TargetSite);
                        sw.WriteLine("【异常实例】" + ex.InnerException);
                    }
                    else
                    {
                        sw.WriteLine("异常为空");
                    }
                    sw.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"异常({DateTime.Now})：{e.Message}");
            }
        }
    }
}
