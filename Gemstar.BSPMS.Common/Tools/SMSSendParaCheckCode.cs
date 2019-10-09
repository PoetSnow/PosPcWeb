using Gemstar.BSPMS.Common.Enumerator;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 发送验证码参数
    /// </summary>
    public class SMSSendParaCheckCode : SMSSendPara
    {
        /// <summary>
        /// 验证码对应的功能名称
        /// </summary>
        public CheckFunc Func { get; set; }

        protected override string SendContent
        {
            get
            {
                string smsContent = "您的注册验证码为：{0}。【捷信达】";
                if (Func == CheckFunc.ResetPassword)
                {
                    smsContent = "您重置密码的手机验证码为：{0}。切勿告诉他人，验证码将在90秒后失效。【捷信达】";
                }
                if (Func == CheckFunc.TryUsePms)
                {
                    smsContent = "您的体验验证码为：{0}。【捷信达】";
                }
                if (Func == CheckFunc.InitSystem)
                {
                    smsContent = "您的系统初始化操作验证码为：{0}。【捷信达】";
                }
                if(Func == CheckFunc.OwnerBind)
                {
                    smsContent = "您的身份绑定验证码为：{0}。【捷信达】";
                }
                return smsContent;
            }
        }
    }
}
