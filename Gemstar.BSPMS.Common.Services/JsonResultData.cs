using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 服务器端返回给客户端的统一的json格式对象
    /// </summary>
    public class JsonResultData
    {
        /// <summary>
        /// 处理是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 如果是失败时，则此对象表示错误原因
        /// 如果是成功时，则此对象表示应返回给客户端的数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 错误代码，默认为0 区分是什么类型的错误 登录超时为1
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 返回处理成功的json实例
        /// </summary>
        /// <param name="data">要返回给客户端的数据实例</param>
        /// <returns>处理成功的json实例</returns>
        public static JsonResultData Successed(object data)
        {
            return new JsonResultData { Success = true, Data = data };
        }

        public static JsonResultData Successed()
        {
            return new JsonResultData { Success = true, Data = "" };
        }

        /// <summary>
        /// 返回处理失败的json实例
        /// </summary>
        /// <param name="msg">失败原因</param>
        /// <returns>处理失败的json实例</returns>
        public static JsonResultData Failure(string msg,int errorCode=0)
        {
            return new JsonResultData { Success = false, Data = msg,ErrorCode=errorCode };
        }
        /// <summary>
        /// 返回处理失败的json实例
        /// </summary>
        /// <param name="data">失败时需要返回的复杂对象</param>
        /// <returns>处理失败的json实例</returns>
        public static JsonResultData Failure(object data, int errorCode = 0)
        {
            return new JsonResultData { Success = false, Data = data,ErrorCode=errorCode };
        }
        /// <summary>
        /// 返回验证失败的json实例
        /// </summary>
        /// <param name="values">ModelState.values</param>
        /// <returns>验证失败的json实例</returns>
        public static JsonResultData Failure(ICollection<ModelState> values, int errorCode = 0)
        {
            var errors = new StringBuilder();
            foreach (var modelState in values)
            {
                foreach (var error in modelState.Errors)
                {
                    errors.AppendLine(error.ErrorMessage);
                }
            }
            return new JsonResultData { Success = false, Data = errors.ToString(),ErrorCode=errorCode };
        }
        /// <summary>
        /// 返回处理失败的json实例
        /// </summary>
        /// <param name="ex">错误异常实例</param>
        /// <returns>处理失败的json实例</returns>
        public static JsonResultData Failure(Exception ex, int errorCode = 0)
        {
            var message = FriendlyMessage(ex);
           
            return new JsonResultData { Success = false, Data = message,ErrorCode=errorCode };
        }
        /// <summary>
        /// 将异常信息转换为友好的出错信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>转换为的对应的友好出错信息</returns>
        public static string FriendlyMessage(Exception ex)
        {
            Exception inner = ex;
            while (inner.InnerException != null)
            {
                inner = inner.InnerException;
            }
            var sqlException = inner as SqlException;
            if(sqlException == null)
            {
                return ex.Message;
            }
            //转换主键重复的提示信息
            if(sqlException.Number == 2627)
            {
                return "此代码已存在，请改为其他代码！";
            }
            //转换唯一索引重复的提示信息
            if (sqlException.Number == 2601)
            {
                //如果索引名称中包含_code,则认为是代码重复
                if (sqlException.Message.Contains("_code"))
                {
                    return "此代码已存在，请改为其他代码！";
                }
                else if (sqlException.Message.Contains("_name"))
                {
                    return "名称重复，请修改为其他名称";
                }
            }
            if (sqlException.Number == 547)
            {
                if (sqlException.Message.Contains("表\"dbo.room\", column \'roomTypeid\'"))
                {
                    return "不能删除当前房间类型，因为有房间属于此类型。";
                }
            }
            //主外键关系在删除时的数据在从表中存在时的提示
            if (sqlException.Message.Contains("DELETE 语句与 REFERENCE 约束"))
            {
                string str = sqlException.Message.Substring(sqlException.Message.IndexOf("表"), sqlException.Message.IndexOf("column") - sqlException.Message.IndexOf("表") - 2);
                return "该记录已使用！不允许删除！";//积分兑换规则
            }
            else if (sqlException.Message.Contains("UPDATE 语句与 REFERENCE 约束"))
            {
                return "该记录已使用！不允许修改！";
            }
            else if (sqlException.Message.Contains("与另一个进程被死锁在 锁 资源上，并且已被选作死锁牺牲品。请重新运行该事务。"))
            {
                return "执行失败，请重新操作。";
            }
            else if (sqlException.Message.Contains("将截断字符串或二进制数据"))
            {
                return "字符超长，请检查输入内容！";
            }
            //其他情况，则直接返回原来的出错信息
            return inner.Message;
        }
    }
}
