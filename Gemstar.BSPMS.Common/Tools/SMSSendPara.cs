using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 短信发送参数父类，具体业务参数从此类继承
    /// </summary>
    public abstract class SMSSendPara
    {
        /// <summary>
        /// 发送用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 发送密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 接收短信的手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 获取发送内容
        /// 此方法会检测参数是否有效，有效则返回发送内容，无效则抛出异常
        /// </summary>
        /// <returns>发送内容</returns>
        public string GetSendContent()
        {
            string invalidMsg;
            var isValid = IsValid(out invalidMsg);
            if (isValid)
            {
                return SendContent;
            }
            throw new ArgumentException(invalidMsg);
        }
        /// <summary>
        /// 当前参数是否有效
        /// </summary>
        /// <param name="invalidMsg">无效的原因</param>
        /// <returns>true：有效，false：无效，无效时根据输出参数取原因</returns>
        protected virtual bool IsValid(out string invalidMsg)
        {
            invalidMsg = "";
            if(Mobile.Length>11)
            {
                return true;
            }
            if (!RegexHelper.IsRightMobile(Mobile))
            {
                invalidMsg = "手机格式不正确";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 由子类实现的，返回具体发送内容
        /// </summary>
        protected abstract string SendContent { get; }
    }
}
