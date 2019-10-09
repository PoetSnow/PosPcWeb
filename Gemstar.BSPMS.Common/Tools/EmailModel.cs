using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Tools
{
    public class EmailModel
    {
        /// <summary>
        /// 邮件发送者显示名称
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 邮件接收者地址
        /// </summary>
        public string ToAddress { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件正文参数，将模板中的占位符替换成想要的值
        /// 占位符格式为 {~Key~}，其中Key为参数名称，对应BodyPara的Key
        /// 模板中有多少占位符，则需要在此参数中添加多少个Key，Value键值对，其中Key为占位符的Key
        /// </summary>
        public Dictionary<string, string> BodyPara { get; set; }
        /// <summary>
        /// 邮件备注，一般在邮件中以红色字体出现在结尾
        /// </summary>
        public string Remark { get; set; }
    }
}
