using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 节假日设置接口
    /// </summary>
    public interface IPosHolidayService : ICRUDService<PosHoliday>
    {
        /// <summary>
        /// 验证节假日是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="vDate">日期</param>
        /// <param name="daysName">节日名称</param>
        /// <returns></returns>
        bool IsExists(string hid, string vDate, string daysName, Guid id);


    }

}
