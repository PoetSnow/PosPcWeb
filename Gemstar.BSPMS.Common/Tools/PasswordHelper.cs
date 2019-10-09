namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 密码辅助类，用于检查密码是否满足规则
    /// 现在的规则是长度必须在6位以上，必须至少有大写字母，小写字母，数字
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// 获取加密后的密码
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="originPassword">原始未加密密码</param>
        /// <returns>加密后的密码</returns>
        public static string GetEncryptedPassword(string code, string originPassword)
        {
            return CryptHelper.EncryptMd5(string.Format("{0}{1}", (string.IsNullOrWhiteSpace(code) ? "" : code.ToLower()), originPassword));
        }
        /// <summary>
        /// 指定的密码是否有效，只有在满足所定义的全部规则后才有效，返回true，否则返回false
        /// </summary>
        /// <param name="password"></param>
        /// <param name="invalidMessage">无效的原因</param>
        /// <returns>true:满足所有的规则，有效，false:不满足至少一条规则，无效</returns>
        public static bool IsPasswordValid(string password,out string invalidMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                invalidMessage = "密码必须设置，不能为空";
                return false;
            }
            if(password.Trim().Length < Length)
            {
                invalidMessage = string.Format("密码必须至少{0}个字符，并且不包含空格",Length);
                return false;
            }
            bool hasUpper = false,hasLower = false,hasNumber = false;
            foreach(char c in password)
            {
                if (c >= 'A' && c <= 'Z') {
                    hasUpper = true;
                }else if(c >= 'a' && c <= 'z')
                {
                    hasLower = true;
                }else if(c >= '0' && c <= '9')
                {
                    hasNumber = true;
                }
            }
            if (HaveUpperLetter && !hasUpper)
            {
                invalidMessage = "必须至少拥有一个大写字母";
                return false;
            }
            if (HaveLowerLetter && !hasLower)
            {
                invalidMessage = "必须至少拥有一个小写字母";
                return false;
            }
            if(HaveNumber && !hasNumber)
            {
                invalidMessage = "必须至少拥有一个数字";
                return false;
            }
            invalidMessage = "";
            return true;
        }
        /// <summary>
        /// 从手机号中提取默认密码
        /// 规则是提取后六位，如果手机号为空或者非法，则提取有效位数后，不足六位的在左侧补1
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static string GetDefaultPasswordFromMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return "".PadLeft(Length, '1');
            }
            if(mobile.Length >= 6)
            {
                //更改为直接使用全部的手机号
                return mobile;
            }else
            {
                return mobile.PadLeft(Length, '1');
            }
        }
        /// <summary>
        /// 密码长度
        /// </summary>
        private const int Length = 6;
        /// <summary>
        /// 是否必须拥有大写字母
        /// </summary>
        private const bool HaveUpperLetter = true;
        /// <summary>
        /// 是否必须拥有小写字母
        /// </summary>
        private const bool HaveLowerLetter = true;
        /// <summary>
        /// 是否必须拥有数字
        /// </summary>
        private const bool HaveNumber = true;

    }
}
