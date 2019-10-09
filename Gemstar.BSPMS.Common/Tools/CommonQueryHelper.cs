using Gemstar.BSPMS.Common.Services.EntityProcedures;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Gemstar.BSPMS.Common.Tools
{
    public class CommonQueryHelper
    {
        #region 私有变量定义
        private readonly string _orgStr = "";
        private Dictionary<string, string> _values;
        private Dictionary<string, string> _texts;
        private string[] _paraValues;
        #endregion

        #region 构建sql语句参数值解析对象实例
        /// <summary>
        /// 构建sql语句参数值解析对象实例
        /// </summary>
        /// <param name="parameterValues">自动生成查询页面返回的存储过程参数值字符串，格式如：@name1=value1&@name2=value2</param>
        public CommonQueryHelper(string parameterValues)
        {
            _orgStr = parameterValues;
        }
        #endregion

        #region 根据参数名称来获取参数值
        /// <summary>
        /// 根据参数名称来获取参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        public string this[string parameterName]
        {
            get
            {
                if (_values == null)
                    GetValues();
                if (_values.ContainsKey(parameterName))
                {
                    return _values[parameterName] + (_texts.ContainsKey(parameterName) ? ("^" + _texts[parameterName]) : "");
                }
                return "";
            }
        }
        #endregion

        #region 根据参数位置序号来获取参数值
        /// <summary>
        /// 根据参数位置序号来获取参数值
        /// </summary>
        /// <param name="parameterIndex">参数位置序号</param>
        /// <returns>参数值。如果位置序号有误，则返回空值</returns>
        public string this[int parameterIndex]
        {
            get
            {
                if (_values == null)
                    GetValues();
                if (parameterIndex >= 0 && parameterIndex < _paraValues.Length)
                {
                    return _paraValues[parameterIndex];
                }

                return "";
            }
        }
        #endregion

        #region 将所有参数值格式化成sql语句的格式，如@name1='value1',@name2=value2        
        public Dictionary<string, string> GetParameters()
        {
            if (_values == null)
            {
                GetValues();
            }
            return _values;
        }
        #endregion

        #region 将所有参数值格式化成报表上显示的查询参数的格式，如name1：value1 name2：value2
        /// <summary>
        /// 将所有参数值格式化成报表上显示的查询参数的格式，如name1：value1 name2：value2
        /// 如果值为空，则此参数名称与值都不显示，如果用户未选择任何参数，则返回空值
        /// </summary>
        /// <returns>将所有参数值格式化成报表上显示的查询参数的格式，如name1：value1 name2：value2</returns>
        public string GetArgumentString()
        {
            if (_values == null)
            {
                GetValues();
            }
            if (_values.Count > 0)
            {
                StringBuilder sql = new StringBuilder();
                string splitChar = "";
                foreach (string key in _values.Keys)
                {
                    if (!string.IsNullOrEmpty(_values[key]))
                    {
                        sql.Append(splitChar).Append(CommonQueryParameterHelper.GetDisplayName(key)).Append(":").Append(_texts[key]);
                        splitChar = " ";
                    }
                }
                return sql.ToString();
            }
            return "";
        }
        #endregion

        #region 将所有参数值格式化成url格式
        /// <summary>
        /// 将所有参数值格式化成url格式，用于将现在的值传递给通用查询页面，以支持中文的参数名称，格式如：<![CDATA[@name1=value1&@name2=values]]>.名称中的中文会自动编码
        /// </summary>
        /// <returns>格式化后的url格式语句。在此值前面加上<![CDATA[?/&]]>符号即可直接跟在页面地址后面进行传递值，如果没有参数值，则返回空串</returns>
        public string GetUrlString()
        {
            if (_values == null)
            {
                GetValues();
            }
            if (_values.Count > 0)
            {
                StringBuilder url = new StringBuilder();
                string splitChar = "";
                HttpServerUtility server = HttpContext.Current.Server;
                foreach (string key in _values.Keys)
                {
                    url.Append(splitChar).Append(server.UrlEncode(CommonQueryParameterHelper.GetDisplayName(key))).Append("=").Append(server.UrlEncode(_values[key])).Append("^").Append(server.UrlEncode(_texts[key]));
                    splitChar = "&";
                }
                return url.ToString();
            }
            return "";
        }
        #endregion

        #region 将字符串分解到集合中
        /// <summary>
        /// 将字符串分解到集合中
        /// </summary>
        private void GetValues()
        {
            _values = new Dictionary<string, string>();
            _texts = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(_orgStr))
            {
                return;
            }
            string[] paras = _orgStr.Split('&');
            foreach (string str1 in paras)
            {
                string[] para = str1.Split('=');
                if (para.Length > 1)
                {
                    //   values.Add(para[0], para[1]);
                    string[] valueAndText = para[1].Split('^');
                    if (valueAndText.Length > 1)
                    {
                        if (_values.ContainsKey(para[0]))//判断是否存在这个键，存在则删除前面
                        {
                            _values.Remove(para[0]);
                            _texts.Remove(para[0]); 
                        }
                        _values.Add(para[0], valueAndText[0]);
                        _texts.Add(para[0], valueAndText[1]);

                    }
                    else
                    {
                        _values.Add(para[0], para[1]);
                        _texts.Add(para[0], para[1]);
                    }
                }
            }
            _paraValues = new string[_values.Values.Count];
            _values.Values.CopyTo(_paraValues, 0);
        }
        #endregion

        #region 设置固定的隐藏参数值
        /// <summary>
        /// 设置参数值
        /// 主要是针对一些固定的隐藏参数，需要根据当前程序的上下文来自动赋值
        /// </summary>
        /// <param name="paraName">参数名称</param>
        /// <param name="paraValue">参数值</param>
        public void SetParaValue(string paraName, string paraValue, List<UpQueryProcedureParametersResult> paras)
        {
            if (_values == null)
                GetValues();
            if (_values.ContainsKey(paraName))
            {
                _values[paraName] = paraValue;
            }
            else
            {
                //如果不在当前值中，则判断存储过程的参数中是否有指定名称的参数，有则直接增加
                var para = paras.Find(w => w.ParameterName == paraName);
                if (para != null)
                {
                    _values.Add(paraName, paraValue);
                }
            }
        }
        #endregion
    }
}