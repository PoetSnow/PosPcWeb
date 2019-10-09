using System;
using System.Web;
using GemStarSecurity;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    /// <summary>
    /// 此项目包含多个系统，通过静态变量定义包含哪个系统，在编译时确定
    /// </summary>
    public static class ProgVersion
    {
        private static object objectForLock = new object();
        #region 版本定义
        /// <summary>
        /// 开始组号
        /// </summary>
        public static Int32 BeginGroup = 98;
        /// <summary>
        /// 结束组号
        /// </summary>
        public static Int32 EndGroup = 98;
        #endregion

        #region 是否启用狗的设置
        /// <summary>
        /// 服务器狗是否启用，如果设置false：则不检查狗，否则检查,true
        /// </summary>
        public const bool IsServerDogEnabled = false;
        #endregion

        #region 存储服务器狗的状态

        /// <summary>
        /// 存储服务器狗的状态,;true:服务器狗工作正常，false:不正常，请转到注册页面。

        /// </summary>
        public static bool IsDogOk
        {
            get
            {
                if (IsServerDogEnabled)
                {
                    //由于使用了cookie+redis，所以有可能用户在重新打开浏览器时，不会执行session_start，导致会话中的值是不正确的
                    //所以此处判断一下HttpContext.Current.Application["dogisok"]中是否有值，没有则自动执行一次
                    //同时判断一下值是否为不通过，不通过的话则自动执行一次，因为有可能原因已经解决，只是由于会话中的值还没有变更过来
                    if(DogErrorMessage == CheckingMsgString || HttpContext.Current.Session["IsDogOk"] == null || !(bool)HttpContext.Current.Session["IsDogOk"])
                    {
                        CheckServerDog();
                    }
                    return HttpContext.Current.Session["IsDogOk"] == null ? false : (bool)HttpContext.Current.Session["IsDogOk"];
                }
                else
                {
                    return true;
                }
            }
            set
            {
                HttpContext.Current.Session["IsDogOk"] = value;
            }
        }
        /// <summary>
        /// 服务器检测时出现错误的原因

        /// </summary>
        public static string DogErrorMessage
        {
            get
            {
                if (IsServerDogEnabled)
                {
                    return HttpContext.Current.Session["DogErrorMessage"] == null ? string.Empty : HttpContext.Current.Session["DogErrorMessage"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                HttpContext.Current.Session["DogErrorMessage"] = value;
            }
        }
        /// <summary>
        /// 服务器检测时可以查看其他功能，不能查看报表的固定原因
        /// </summary>
        public static string NoReportDogErrorMessage
        {
            get
            {
                return HttpContext.Current.Application["NoReportDogErrorMessage"] == null ? string.Empty : HttpContext.Current.Application["NoReportDogErrorMessage"].ToString();
            }
            set
            {
                HttpContext.Current.Application["NoReportDogErrorMessage"] = value;
            }
        }

        public static void CheckServerDog()
        {
            //验证规则，增加一全局变量，checkStatus,
            //1、先检查cehckstatus，如果不存在或者为空，则更改其值为checking(表示正在进行验证)，然后进行验证，如果验证成功则将其值更改为当天的日期值，如果验证不成功则更改回空值。

            //2、如果为日期值，则检查日期值是否当天的日期值，如果是则直接认为是验证通过，返回，否则更改其值为checking，进行验证，验证处理同上
            //3、如果为checking，表示有其他人正在进行验证，则提示用户稍后再试    
            //由于在多线程的情况下，可能会引起调用外部dll时内存错误，所以此处强制增加单线程锁
            lock (objectForLock)
            {
                var checkStatus = "";
                if (HttpContext.Current.Application["checkStatus"] != null) checkStatus = HttpContext.Current.Application["checkStatus"].ToString();
                if (checkStatus == "checking")
                {
                    IsDogOk = false;
                    DogErrorMessage = CheckingMsgString;
                }
                else if (checkStatus == DateTime.Today.ToString("yyyyMMdd") && (bool)HttpContext.Current.Application["dogisok"])
                {
                    IsDogOk = (bool)HttpContext.Current.Application["dogisok"];
                    DogErrorMessage = HttpContext.Current.Application["dogErrorMsg"].ToString();
                }
                else
                {
                        var isok = false;
                        var preErrorMessage = "";
                    try
                    {
                        HttpContext.Current.Application["checkStatus"] = "checking";

                        var begin = BeginGroup;
                        var end = EndGroup;
                        NoReportDogErrorMessage = CstRegister.NoReportErrorMessage;
                        var connStr = MvcApplication.GetCenterDBConnStr();
                        var dataAccess = new Gemstar.Data.DataAccess(connStr, Gemstar.Data.DbType.SqlServer);
                        while (begin <= end && !isok)
                        {
                            var register = new CstRegister(dataAccess, begin, GemstarSystems.Hmscy, NetDogAccessMethod.CloudCheckMacOnly);

                            isok = register.IsRegInfoValid();
                            
                            if (!register.ErrorMessage.EndsWith("130"))
                                preErrorMessage = register.ErrorMessage;
                            register.WriteLog();
                            begin++;
                        }
                    }
                    finally
                    {
                        //将结果保存到application中
                        HttpContext.Current.Application["dogisok"] = isok;
                        HttpContext.Current.Application["dogErrorMsg"] = preErrorMessage;
                        IsDogOk = isok;
                        DogErrorMessage = preErrorMessage;
                        if (isok)
                        {
                            HttpContext.Current.Application["checkStatus"] = DateTime.Today.ToString("yyyyMMdd");
                        }
                        else
                        {
                            HttpContext.Current.Application["checkStatus"] = "";
                        }
                    }
                }
            }
        }
        private const string CheckingMsgString = "有其他人正在进行验证，请关闭此窗口后稍后再试！";
        #endregion
    }
}