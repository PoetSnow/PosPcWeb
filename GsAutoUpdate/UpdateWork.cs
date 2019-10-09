using GsAutoUpdate.Models;
using GsAutoUpdate.Service;
using Ionic.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace GsAutoUpdate
{
    public class UpdateWork
    {
        public delegate void UpdateProgess(double data);

        public readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;  //程序根目录

        public UpdateProgess OnUpdateProgess;
        private string mainName;

        //临时目录（WIN7以及以上在C盘只有对于temp目录有操作权限）
        private String tempPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), @"PrintSqlite\temp\");

        private String bakPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), @"PrintSqlite\bak\");

        public static UpdateAppInfo updateAppInfo;

        private LocalInfo localInfo = new LocalInfo();
        public List<RemoteInfo> UpdateVerList { get; set; }
        public String programName { get; set; }
        public String subKey { get; set; }

        /// <summary>
        /// 初始化配置目录信息
        /// </summary>
        public UpdateWork(String _programName)
        {
            try
            {
                Process cur = Process.GetCurrentProcess();
                mainName = Path.GetFileName(cur.MainModule.FileName);
                //subKey = _subKey;
                programName = _programName;
                //创建备份目录信息
                DirectoryInfo bakinfo = new DirectoryInfo(bakPath);
                if (bakinfo.Exists == false)
                {
                    bakinfo.Create();
                }
                //创建临时目录信息
                DirectoryInfo tempinfo = new DirectoryInfo(tempPath);
                if (tempinfo.Exists == false)
                {
                    tempinfo.Create();
                }


                localInfo.LoadXml();
                UpdateVerList = GetUpdateList(localInfo.ServerUpdateUrl, updateAppInfo.Hid,localInfo.LocalVersion);
                CheckVer(localInfo.LocalVersion);
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
        }

        public Boolean Do()
        {
            //检测是否存在更新,没有更新直接返回
            if (UpdateVerList.Count == 0)
            {
                return true;
            }

            KillProcessExist();
            //更新之前先备份
            Bak();
            //备份结束开始下载东西
            DownLoad();//下载更新包文件信息
            //3、开始更新
            Update();

            Start();
            return true;
        }

        public void IgnoreThisVersion()
        {
            var item = UpdateVerList[UpdateVerList.Count - 1];
            localInfo.LastUdpate = item.ReleaseDate;
            localInfo.LocalVersion = item.ReleaseVersion;
            localInfo.SaveXml();
        }

        ///// <summary>
        ///// 获取更新的服务器端的数据信息
        ///// </summary>
        ///// <param name="url">自动更新的URL信息</param>
        ///// <returns></returns>
        //private static List<RemoteInfo> GetServer(String url)
        //{
        //    List<RemoteInfo> list = new List<RemoteInfo>();

        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();
        //        XmlReaderSettings settings = new XmlReaderSettings();
        //        settings.IgnoreComments = true;
        //        XmlReader reader = XmlReader.Create(url, settings);
        //        xmlDoc.Load(reader);
        //        reader.Close();

        //        var root = xmlDoc.DocumentElement;
        //        var listNodes = root.SelectNodes("/ServerUpdate/item");
        //        foreach (XmlNode item in listNodes)
        //        {
        //            RemoteInfo remote = new RemoteInfo();
        //            foreach (XmlNode pItem in item.ChildNodes)
        //            {
        //                remote.GetType().GetProperty(pItem.Name).SetValue(remote, pItem.InnerText, null);
        //            }
        //            list.Add(remote);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelp.AddLog("获取线上更新信息失败");
        //        LogHelp.AddException(ex);
        //    }
        //    return list;
        //}

        //获取更新列表
        private static List<RemoteInfo> GetUpdateList(string Url, string Hid,string version)
        {
            List<RemoteInfo> list = new List<RemoteInfo>();
            try
            {

                // 请求接口数据
                var httpItem = new HttpItem();
                httpItem.Method = "post";
                httpItem.PostEncoding = Encoding.UTF8;
                httpItem.URL = $"{Url}/GetAppUpdate";
                httpItem.ContentType = "application/x-www-form-urlencoded";
                httpItem.Postdata = $"hid={Hid}&version={version}&isEnvTest=true";

                var httpHelper = new HttpHelper();
                var html = httpHelper.GetHtml(httpItem);

                LogHelp.AddLog(html.Html);

                if (html.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = JObject.Parse(html.Html);
                    if (Convert.ToBoolean(json["Success"]))
                    {
                        var updatelist = JsonConvert.DeserializeObject<List<RemoteInfo>>(json["Data"].ToString());
                        return updatelist;
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelp.AddLog("获取线上更新信息失败");
                LogHelp.AddException(ex);
            }
            return list;
        }

        /// <summary>
        /// 下载方法
        /// </summary>
        private UpdateWork DownLoad()
        {
            //比如uri=http://localhost/Rabom/1.rar;iis就需要自己配置了。
            //截取文件名
            //构造文件完全限定名,准备将网络流下载为本地文件

            using (WebClient web = new WebClient())
            {
                foreach (var item in UpdateVerList)
                {
                    try
                    {
                        LogHelp.AddLog("下载更新包文件" + item.ReleaseVersion);
                        web.DownloadFile(item.ReleaseUrl, tempPath + item.ReleaseVersion + ".zip");
                        OnUpdateProgess?.Invoke(60 / UpdateVerList.Count);
                    }
                    catch (Exception ex)
                    {
                        LogHelp.AddException(ex);
                    }
                }
                return this;
            }
        }

        /// <summary>
        /// 备份当前的程序目录信息
        /// </summary>
        private UpdateWork Bak()
        {
            try
            {
                LogHelp.AddLog("准备执行备份操作");

                DirectoryInfo di = new DirectoryInfo(updateAppInfo.WatUpdateDirectory);
                foreach (var item in di.GetFiles())
                {
                    if (item.Name != mainName)//当前文件不需要备份
                    {
                        File.Copy(item.FullName, bakPath + item.Name, true);
                    }
                }
                //文件夹复制
                foreach (var item in di.GetDirectories())
                {
                    if (item.Name != "bak" && item.Name != "temp")
                    {
                        CopyDirectory(item.FullName, bakPath);
                    }
                }
                LogHelp.AddLog("备份操作执行完成，开始关闭应用程序");
                OnUpdateProgess?.Invoke(20);
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        private UpdateWork Update()
        {
            foreach (var item in UpdateVerList)
            {
                try
                {
                    //如果是覆盖安装的话，先删除原先的所有程序
                    if (item.UpdateMode == "Cover")
                    {
                        DelLocal();
                    }
                    string path = tempPath + item.ReleaseVersion + ".zip";

                    LogHelp.AddLog(path);

                    LogHelp.AddLog("解压 ZipFile zip = new ZipFile(path,Encoding.Default)");

                    using (ZipFile zip = new ZipFile(path,Encoding.Default))
                    {
                        LogHelp.AddLog("解压 " + item.ReleaseVersion + ".zip");
                        zip.IgnoreDuplicateFiles = true;  
                       
                        zip.ExtractAll(updateAppInfo.WatUpdateDirectory, ExtractExistingFileAction.OverwriteSilently);                   
                        LogHelp.AddLog(item.ReleaseVersion + ".zip" + "解压完成");
                        ExecuteINI();//执行注册表等更新以及删除文件
                        LogHelp.AddLog("执行注册表等更新以及删除文件完成 --- ");
                    }

                    localInfo.LastUdpate = item.ReleaseDate;
                    localInfo.LocalVersion = item.ReleaseVersion;
                    localInfo.SaveXml();
                }
                catch (Exception ex)
                {
                    LogHelp.AddLog("更新错误");

                    LogHelp.AddException(ex);
                    Restore();
                    break;
                }
                finally
                {
                    //删除下载的临时文件
                    LogHelp.AddLog("删除临时文件 " + item.ReleaseVersion);
                    DelTempFile(item.ReleaseVersion + ".zip");//删除更新包
                    LogHelp.AddLog("临时文件删除完成 " + item.ReleaseVersion);
                }
            }
            OnUpdateProgess?.Invoke(98);
            return this;
        }

        private UpdateWork Start()
        {
            try
            {
                String[] StartInfo = UpdateVerList[UpdateVerList.Count - 1].ApplicationStartName.Split(',');
                if (StartInfo.Length > 0)
                {
                    foreach (var item in StartInfo)
                    {
                        LogHelp.AddLog("启动" + item);
                        LogHelp.AddLog("启动: " + Path.Combine(updateAppInfo.WatUpdateDirectory, item));
                        Process.Start(Path.Combine(updateAppInfo.WatUpdateDirectory, item));
                    }
                }
                OnUpdateProgess?.Invoke(100);
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="srcdir">源目录</param>
        /// <param name="desdir">目标目录</param>
        private UpdateWork CopyDirectory(string srcdir, string desdir)
        {
            try
            {
                string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

                string desfolderdir = desdir + "\\" + folderName;

                if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
                {
                    desfolderdir = desdir + folderName;
                }
                string[] filenames = Directory.GetFileSystemEntries(srcdir);
                foreach (string file in filenames)// 遍历所有的文件和目录
                {
                    string fileName = Assembly.GetExecutingAssembly().GetName().Name;
                    if (file.IndexOf(fileName) == -1)
                    {
                        if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                        {
                            string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                            if (!Directory.Exists(currentdir))
                            {
                                Directory.CreateDirectory(currentdir);
                            }
                            CopyDirectory(file, desfolderdir);
                        }
                        else // 否则直接copy文件
                        {
                            string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);
                            srcfileName = desfolderdir + "\\" + srcfileName;
                            if (!Directory.Exists(desfolderdir))
                            {
                                Directory.CreateDirectory(desfolderdir);
                            }
                            File.Copy(file, srcfileName, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        private UpdateWork DelTempFile(String name)
        {
            try
            {
                FileInfo file = new FileInfo(tempPath + name);
                file.Delete();
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 更新失败的情况下，回滚当前更新
        /// </summary>
        private UpdateWork Restore()
        {
            try
            {
                DelLocal();
                CopyDirectory(bakPath, updateAppInfo.WatUpdateDirectory);
            }
            catch (Exception ex)
            {
                LogHelp.AddLog("回滚错误");
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 删除本地文件夹的文件
        /// </summary>
        private UpdateWork DelLocal()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(updateAppInfo.WatUpdateDirectory);
                foreach (var item in di.GetFiles())
                {
                    if (item.Name != mainName)
                    {
                        string fileName = Assembly.GetExecutingAssembly().GetName().Name;
                        if (item.Name != "Local.xml" && item.Name.IndexOf(fileName) == -1)
                        {
                            if (File.Exists(item.FullName))
                            {
                                File.Delete(item.FullName);
                            }
                        }
                    }
                }
                foreach (var item in di.GetDirectories())
                {
                    if (item.Name != "bak" && item.Name != "temp")
                    {
                        item.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 校验程序版本号
        /// </summary>
        /// <param name="LocalVer">当前本地版本信息</param>
        /// <param name="RemoteVer">服务器端版本信息</param>
        /// <returns></returns>
        private UpdateWork CheckVer(String LocalVer)
        {
            try
            {
                String[] Local = LocalVer.Split('.');
                List<RemoteInfo> list = new List<RemoteInfo>();
                foreach (var item in UpdateVerList)
                {
                    String[] Remote = item.ReleaseVersion.Split('.');
                    for (int i = 0; i < Local.Length; i++)
                    {
                        if (Int32.Parse(Local[i]) < Int32.Parse(Remote[i]))
                        {
                            list.Add(item);
                            break;
                        }
                        else if (Int32.Parse(Local[i]) == Int32.Parse(Remote[i]))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                UpdateVerList = list;
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 更新配置信息
        /// </summary>
        private UpdateWork ExecuteINI()
        {
            try
            {
                DirectoryInfo TheFolder = new DirectoryInfo(updateAppInfo.WatUpdateDirectory);



                if (File.Exists(Path.Combine(TheFolder.FullName, "config.update")))
                {
                    String[] ss = File.ReadAllLines(Path.Combine(TheFolder.FullName, "config.update"));
                    Int32 i = -1;//0[regedit_del] 表示注册表删除‘1[regedit_add]表示注册表新增 2[file_del] 表示删除文件
                    foreach (var s in ss)
                    {
                        String s1 = s.Trim();
                        if (s1 == "[regedit_del]")
                        {
                            i = 0;
                        }
                        else if (s1 == "[regedit_add]")
                        {
                            i = 1;
                        }
                        else if (s1 == "[file_del]")
                        {
                            i = 2;
                        }
                        else
                        {
                            if (i == 0)
                            {
                                String[] tempKeys = s1.Split(',');
                                DelRegistryKey(tempKeys[0], tempKeys[1]);
                            }
                            else if (i == 1)
                            {
                                String[] values = s1.Split('=');
                                String[] tempKeys = values[0].Split(',');
                                SetRegistryKey(tempKeys[0], tempKeys[1], values[1]);
                            }
                            else if (i == 2)
                            {
                                DelFile(Path.Combine(updateAppInfo.WatUpdateDirectory, s1));
                            }
                        }
                    }
                    DelFile(Path.Combine(TheFolder.FullName, "config.update"));
                }
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        private UpdateWork DelFile(String name)
        {
            try
            {
                if (File.Exists(Path.Combine(updateAppInfo.WatUpdateDirectory, name)))
                {
                    FileInfo file = new FileInfo(Path.Combine(updateAppInfo.WatUpdateDirectory, name));
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
            return this;
        }

        /// <summary>
        /// 校验当前程序是否在运行
        /// </summary>
        /// <param name="programName"></param>
        /// <returns></returns>
        public Boolean CheckProcessExist()
        {
            return Process.GetProcessesByName(programName).Length > 0 ? true : false;
        }

        /// <summary>
        /// 杀掉当前运行的程序进程
        /// </summary>
        /// <param name="programName">程序名称</param>
        public void KillProcessExist()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(programName);
                foreach (Process p in processes)
                {
                    p.Kill();
                    p.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelp.AddException(ex);
            }
        }

        #region 暂时没用，如果需要将本地版本放注册表的话 那是有用的

        /// <summary>
        /// 设置注册表值
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetRegistryKey(String subKey, String key, String value)
        {
            RegistryKey reg;
            RegistryKey reglocal = Registry.CurrentUser;

            reg = reglocal.OpenSubKey(subKey, true);
            if (reg == null)
                reg = reglocal.CreateSubKey(subKey);
            reg.SetValue(key, value, RegistryValueKind.String);
            if (reg != null)
            {
                reg.Close();
            }
        }

        private void DelRegistryKey(String subKey, String key)
        {
            RegistryKey reg;
            RegistryKey reglocal = Registry.CurrentUser;

            reg = reglocal.OpenSubKey(subKey, true);
            if (reg != null)
            {
                var res = reg.GetValue(key);
                if (res != null)
                {
                    reg.DeleteValue(key);
                }
            }
            reg.Close();
        }

        #endregion 暂时没用，如果需要将本地版本放注册表的话 那是有用的



    }
}