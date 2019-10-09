using Newtonsoft.Json;
using System;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    /// <summary>
    /// 用于生成json格式字符串
    /// </summary>
    public abstract class JsonBuilder
    {

        // 验证bizContent对象
        public abstract bool Validate();

        // 将bizContent对象转换为json字符串
        public string BuildJson()
        {

            if (Validate())
            {
                try
                {
                    return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                } catch (Exception ex)
                {

                    throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
                }
            }
            throw new Exception("缺少必须的参数");
        }
    }
}