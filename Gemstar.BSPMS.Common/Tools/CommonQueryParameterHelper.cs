namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 通用查询参数辅助类
    /// </summary>
    public static class CommonQueryParameterHelper
    {
        #region 从存储过程名称中获取规则码值
        /// <summary>
        /// 从存储过程名称中获取规则码值
        /// </summary>
        /// <param name="parameterName">存储过程参数名称</param>
        /// <param name="codeValue">存储过程参数名称中的规则码值，如果参数中没有正确的规则码则返回空值</param>
        /// <returns>获取规则码值是否成功.true:获取成功，值保存在out参数<paramref name="codeValue"/>中；false:获取不成功，out参数<paramref name="codeValue"/>值为空</returns>
        public static bool GetCodeValue(string parameterName, out string codeValue)
        {
            if (!string.IsNullOrWhiteSpace(parameterName))
            {
                string code = parameterName.Substring(2, 2);
                int temp = 0;
                if (int.TryParse(code, out temp))
                {
                    codeValue = parameterName.Substring(1,3).ToLower();
                    return true;
                }
            }
            codeValue = "";
            return false;
        }
        #endregion

        #region 获取显示给用户的存储过程名称
        /// <summary>
        /// 获取显示给用户的存储过程名称，因为现在使用了规则码，所以在显示时需要去掉规则码。如果是以前的存储过程，参数名称中未使用规则码时，则只去掉@符号即可。
        /// </summary>
        /// <param name="parameterName">存储过程参数名称</param>
        /// <returns>显示给用户的友好的存储过程名称</returns>
        public static string GetDisplayName(string parameterName)
        {
            string temp;
            if (GetCodeValue(parameterName, out temp))
            {
                return parameterName.Substring(4);
            }
            return parameterName.Substring(1);
        }
        #endregion

    }
}