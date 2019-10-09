using Gemstar.BSPMS.Common.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Tools;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.Common
{
    /// <summary>
    /// 通用查询扩展类
    /// </summary>
    public static class CommonQueryHelperExtension
    {
        /// <summary>
        /// 给通用的隐藏参数赋值
        /// 现在处理的隐藏参数有
        /// @h99grpid:如果当前是集团，则始终传集团id，如果当前是单体酒店，则始终传酒店id
        /// @h99hid:始终传递当前的酒店id
        /// </summary>
        /// <param name="queryHelper">通用查询实例</param>
        /// <param name="currentInfo">当前程序上下文</param>
        public static void SetHiddleParaValuesFromCurrentInfo(this CommonQueryHelper queryHelper, ICurrentInfo currentInfo, List<UpQueryProcedureParametersResult> paras)
        {
            queryHelper.SetParaValue("@h99grpid", currentInfo.GroupHotelId, paras);
            queryHelper.SetParaValue("@h99hid", currentInfo.HotelId, paras);
            queryHelper.SetParaValue("@h99productmask", ((byte)currentInfo.ProductType).ToString(), paras);
        }
    }
}
