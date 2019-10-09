using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
    public static class RegexHelper
    {
        /// <summary>
        /// 手机号的正则表达式
        /// </summary>
        public const string MobileRegexString = @"^1[3456789][0-9]{9}$";
        /// <summary>
        /// 验证邮件的正则表达式
        /// </summary>
        public const string EmailRegexString = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        /// <summary>
        /// 验证身份证号的正则表达式
        /// </summary>
        public const string IdentityRegexString = @"^\d{14}(\d|(\d{3}[\d|x|X]))$";
        #region 详细身份证验证
        /// 身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 18位身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        /// <summary>
        /// 15位身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion
        /// <summary>
        /// 护照正则表达式
        /// </summary>
        public const string PassportRegexString = @"^1[45][0-9]{7}|G[0-9]{8}|P[0-9]{7}|S[0-9]{7,8}|D[0-9]+$";
        /// <summary>
        /// 香港身份证号的正则表达式1
        /// </summary>
        public const string HongKongIdentityRegexString1 = @"^[A-Z]{1,2}[0-9]{6}[\(|\（]?[0-9A-Z][\)|\）]?$";
        /// <summary>
        /// 香港身份证号的正则表达式2
        /// </summary>
        public const string HongKongIdentityRegexString2 = @"^[A-Z][0-9]{8,12}$";
        /// <summary>
        /// 澳门的正则表达式
        /// </summary>
        public const string MacaoIdentityRegexString = @"^[1|5|7][0-9]{6}[\(|\（]?[0-9A-Z][\)|\）]?$";
        /// <summary>
        /// 台湾身份证号的正则表达式
        /// </summary>
        public const string TaiwanIdentityRegexString = @"^[a-zA-Z][0-9]{9}$";

        public static bool IsRightMobile(string mobileNo)
        {
            return new Regex(MobileRegexString, RegexOptions.Compiled).IsMatch(mobileNo);
        }
        /// <summary>
        /// 验证邮箱地址是否正确，验证规则是：必须含有@.字符。
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>是否具有正确的格式，true:正确，false:不正确</returns>
        public static bool IsRightEmail(string email)
        {
            return new Regex(EmailRegexString, RegexOptions.Compiled).IsMatch(email);
        }
        /// <summary>
        /// 验证身份证号码是否正确，验证规则是必须是15位数字或者17位数字加1位数字/X
        /// </summary>
        /// <param name="identityCardNo">身份证号</param>
        /// <returns>是否具有正确的格式，true:正确，false:不正确</returns>
        public static bool IsRightIdentityCard(string identityCardNo)
        {
            bool isTrue = new Regex(IdentityRegexString, RegexOptions.Compiled).IsMatch(identityCardNo);
            if (isTrue)
            {
                isTrue = CheckIDCard(identityCardNo);
            }
            return isTrue;
        }
        /// <summary>
        /// 验证护照是否正确
        /// </summary>
        /// <param name="passportNo"></param>
        /// <returns></returns>
        public static bool IsRightPassport(string passportNo)
        {
            return new Regex(PassportRegexString, RegexOptions.Compiled).IsMatch(passportNo);
        }
        /// <summary>
        /// 验证港澳台居民居民身份证是否正确
        /// </summary>
        /// <param name="identityCardNo"></param>
        /// <returns></returns>
        public static bool IsRightIdentityCardOthers(string identityCardNo)
        {
            return (
                new Regex(HongKongIdentityRegexString1, RegexOptions.Compiled).IsMatch(identityCardNo) ||
                new Regex(HongKongIdentityRegexString2, RegexOptions.Compiled).IsMatch(identityCardNo) || 
                new Regex(MacaoIdentityRegexString, RegexOptions.Compiled).IsMatch(identityCardNo) || 
                new Regex(TaiwanIdentityRegexString, RegexOptions.Compiled).IsMatch(identityCardNo)
                );
        }
    }
}
