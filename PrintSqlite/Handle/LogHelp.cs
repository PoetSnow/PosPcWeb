using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PrintSqlite
{
    public class LogHelp
    {
        static string path = Application.StartupPath + "\\log"; //日志文件夹
        static string fileName = $@"{path}\{DateTime.Now.ToString("yyyy-MM-dd")}.log";  //日志文件
        
        /// <summary>
        /// 写异常
        /// </summary>
        /// <param name="ex">异常类</param>
        public static void CreateLog(Exception ex)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);    //创建日志文件夹
                }
                WriteLogInfo(ex, fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"异常({DateTime.Now})：{e.Message}");
                
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logContent">日志内容</param>
        public static void WriteLog(string logContent)
        {
            try
            {
                if (!Directory.Exists(path))//验证路径是否存在
                {
                    Directory.CreateDirectory(path);    //不存在则创建
                }

                FileStream fs;
                if (File.Exists(fileName))  //验证文件是否存在，有则追加，无则创建
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                }

                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.WriteLine($"{DateTime.Now.ToLongTimeString()}：{logContent}");

                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                
            }
        }

        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="ex">异常类</param>
        /// <param name="path">日志文件存放路径</param>
        private static void WriteLogInfo(Exception ex, string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                {
                    sw.WriteLine("**********【" + DateTime.Now.ToLongTimeString() + "】**********");
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
