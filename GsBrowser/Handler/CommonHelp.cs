using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GsBrowser
{
    public static class CommonHelp
    {
        #region 键盘参数 常量定义

        public const byte KeyLButton = 0x1;    // 鼠标左键
        public const byte KeyRButton = 0x2;    // 鼠标右键
        public const byte KeyCancel = 0x3;     // CANCEL 键
        public const byte KeyMButton = 0x4;    // 鼠标中键
        public const byte KeyBack = 0x8;       // BACKSPACE 键
        public const byte KeyTab = 0x9;        // TAB 键
        public const byte KeyClear = 0xC;      // CLEAR 键
        public const byte KeyReturn = 0xD;     // ENTER 键
        public const byte KeyShift = 0x10;     // SHIFT 键
        public const byte KeyControl = 0x11;   // CTRL 键
        public const byte KeyAlt = 18;         // Alt 键  (键码18)
        public const byte KeyMenu = 0x12;      // MENU 键
        public const byte KeyPause = 0x13;     // PAUSE 键
        public const byte KeyCapital = 0x14;   // CAPS LOCK 键
        public const byte KeyEscape = 0x1B;    // ESC 键
        public const byte KeySpace = 0x20;     // SPACEBAR 键
        public const byte KeyPageUp = 0x21;    // PAGE UP 键
        public const byte KeyEnd = 0x23;       // End 键
        public const byte KeyHome = 0x24;      // HOME 键
        public const byte KeyLeft = 0x25;      // LEFT ARROW 键
        public const byte KeyUp = 0x26;        // UP ARROW 键
        public const byte KeyRight = 0x27;     // RIGHT ARROW 键
        public const byte KeyDown = 0x28;      // DOWN ARROW 键
        public const byte KeySelect = 0x29;    // Select 键
        public const byte KeyPrint = 0x2A;     // PRINT SCREEN 键
        public const byte KeyExecute = 0x2B;   // EXECUTE 键
        public const byte KeySnapshot = 0x2C;  // SNAPSHOT 键
        public const byte KeyDelete = 0x2E;    // Delete 键
        public const byte KeyHelp = 0x2F;      // HELP 键
        public const byte KeyNumlock = 0x90;   // NUM LOCK 键

        //常用键 字母键A到Z
        public const byte KeyA = 65;
        public const byte KeyB = 66;
        public const byte KeyC = 67;
        public const byte KeyD = 68;
        public const byte KeyE = 69;
        public const byte KeyF = 70;
        public const byte KeyG = 71;
        public const byte KeyH = 72;
        public const byte KeyI = 73;
        public const byte KeyJ = 74;
        public const byte KeyK = 75;
        public const byte KeyL = 76;
        public const byte KeyM = 77;
        public const byte KeyN = 78;
        public const byte KeyO = 79;
        public const byte KeyP = 80;
        public const byte KeyQ = 81;
        public const byte KeyR = 82;
        public const byte KeyS = 83;
        public const byte KeyT = 84;
        public const byte KeyU = 85;
        public const byte KeyV = 86;
        public const byte KeyW = 87;
        public const byte KeyX = 88;
        public const byte KeyY = 89;
        public const byte KeyZ = 90;

        //数字键盘0到9
        public const byte Key0 = 48;    // 0 键
        public const byte Key1 = 49;    // 1 键
        public const byte Key2 = 50;    // 2 键
        public const byte Key3 = 51;    // 3 键
        public const byte Key4 = 52;    // 4 键
        public const byte Key5 = 53;    // 5 键
        public const byte Key6 = 54;    // 6 键
        public const byte Key7 = 55;    // 7 键
        public const byte Key8 = 56;    // 8 键
        public const byte Key9 = 57;    // 9 键

        public const byte KeyNumpad0 = 0x60;    //0 键
        public const byte KeyNumpad1 = 0x61;    //1 键
        public const byte KeyNumpad2 = 0x62;    //2 键
        public const byte KeyNumpad3 = 0x63;    //3 键
        public const byte KeyNumpad4 = 0x64;    //4 键
        public const byte KeyNumpad5 = 0x65;    //5 键
        public const byte KeyNumpad6 = 0x66;    //6 键
        public const byte KeyNumpad7 = 0x67;    //7 键
        public const byte KeyNumpad8 = 0x68;    //8 键
        public const byte KeyNumpad9 = 0x69;    //9 键
        public const byte KeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)键
        public const byte KeyAdd = 0x6B;        // PLUS SIGN(+) 键
        public const byte KeySeparator = 0x6C;  // ENTER 键
        public const byte KeySubtract = 0x6D;   // MINUS SIGN(-) 键
        public const byte KeyDecimal = 0x6E;    // DECIMAL POINT(.) 键
        public const byte KeyDivide = 0x6F;     // DIVISION SIGN(/) 键

        //F1到F12按键
        public const byte KeyF1 = 0x70;   //F1 键
        public const byte KeyF2 = 0x71;   //F2 键
        public const byte KeyF3 = 0x72;   //F3 键
        public const byte KeyF4 = 0x73;   //F4 键
        public const byte KeyF5 = 0x74;   //F5 键
        public const byte KeyF6 = 0x75;   //F6 键
        public const byte KeyF7 = 0x76;   //F7 键
        public const byte KeyF8 = 0x77;   //F8 键
        public const byte KeyF9 = 0x78;   //F9 键
        public const byte KeyF10 = 0x79;  //F10 键
        public const byte KeyF11 = 0x7A;  //F11 键
        public const byte KeyF12 = 0x7B;  //F12 键

        #endregion

        #region 引用Win32Api方法 模拟按键

        /// <summary>
        /// 导入模拟键盘的方法
        /// </summary>
        /// <param name="bVk" >按键的虚拟键值</param>
        /// <param name= "bScan" >扫描码，一般不用设置，用0代替就行</param>
        /// <param name= "dwFlags" >选项标志：0：表示按下，2：表示松开</param>
        /// <param name= "dwExtraInfo">一般设置为0</param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        #endregion

        /// <summary>  
        /// 获取枚举的描述  
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述</returns>  
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];   //获取描述特性  

                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述  
                }
            }
            return en.ToString();
        }

        ///<summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d,true);
                }
            }
        }

        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static void DeleteFolderAndContent(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DeleteFolder(d);////递归删除子文件夹
                }

                if(Directory.Exists(d))
                {
                    Directory.Delete(d,true);
                }
            }
        }


        /// <summary>
        /// 获取本地打印机列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPrintDataTable()
        {
            DataTable dt = new DataTable();
            string data = Application.StartupPath + "\\report";
            string dataPath = data + "\\PrinterList.json";

            if (!Directory.Exists(data))
            {
                Directory.CreateDirectory(data);  //创建文件夹
                File.CreateText(dataPath).Dispose();  //创建文件
            }
            else
            {
                if (!File.Exists(dataPath))
                {
                    File.Create(dataPath).Dispose();  //创建文件
                }
                else
                {
                    var dataBase = File.ReadAllText(dataPath);  //读取文件
                    if (dataBase.IndexOf(":") > 0)
                    {
                        dt = JsonConvert.DeserializeObject<DataTable>(dataBase);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取打印机任务队列
        /// </summary>
        /// <param name="printerName">打印机名称</param>
        /// <returns></returns>
        public static ManagementObjectCollection GetPrintJobsCollection()
        {
            try
            {
                string searchQuery = "SELECT * FROM Win32_PrintJob";
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(searchQuery);
                var jobs = searchPrintJobs.Get();

                //foreach (var job in jobs)     //循环打印队列
                //{
                //    string message = "";
                //    foreach (var property in job.Properties)    //循环队列属性
                //    {
                //        message += "\r\n" + property.Name + "（" + property.Type + "）：" + property.Value + "；";
                //    }
                //    LogHelp.CreateLog(new Exception() { Source = message });
                //}

                return jobs;
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
                return null;
            }
        }

        /// <summary>
        /// 获取打印机状态
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static PropertyDataCollection GetPrintsProperties(string printerName)
        {
            try
            {
                string path = @"Win32_Printer.DeviceId='" + printerName + "'";
                ManagementObject printer = new ManagementObject(path);
                printer.Get();
                //string message = "打印机：" + printerName;
                //foreach (var property in printer.Properties)    //循环打印机属性
                //{
                //    message += property.Name + "（" + property.Type + "）：" + property.Value + "；";
                //}
                //LogHelp.CreateLog(new Exception() { Source = message });

                return printer.Properties;
            }
            catch (Exception ex)
            {
                LogHelp.CreateLog(ex);
                Console.WriteLine($"异常({DateTime.Now})：{ex.Message}");
                //throw;
                return null;
            }
        }
    }
}
