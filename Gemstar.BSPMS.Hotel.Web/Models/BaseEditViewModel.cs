using System.Collections;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    /// <summary>
    /// 所有修改视图模型的基类，提供通用的原始对象json序列字符串属性
    /// </summary>
    public class BaseEditViewModel
    {
        public string OriginJsonData { get; set; }
        public T GetOriginObject<T>()
        {
            var serializer = new JavaScriptSerializer();
            var originUser = serializer.Deserialize<T>(OriginJsonData);

            return originUser;
        }

        /// <summary>
        /// 根据指定长度截取字符串
        /// </summary>
        /// <param name="txtString">字符串</param>
        /// <param name="charNumber">要截取的长度</param>
        /// <returns></returns>
        public static ArrayList GetSeparateSubString(string txtString, int charNumber) 
        {
            ArrayList arrlist = new ArrayList();
            string tempStr = txtString;
            for (int i = 0; i < tempStr.Length; i += charNumber)    //首先判断字符串的长度，循环截取，进去循环后首先判断字符串是否大于每段的长度
            {
                if ((tempStr.Length - i) > charNumber)  //如果是，就截取
                {
                    arrlist.Add(tempStr.Substring(i, charNumber));
                }
                else
                {
                    arrlist.Add(tempStr.Substring(i));  //如果不是，就截取最后剩下的那部分
                }
            }
            return arrlist;
        }
    }
}