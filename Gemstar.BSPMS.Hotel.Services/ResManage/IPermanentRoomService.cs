using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    public interface IPermanentRoomService
    {
        /// <summary>
        /// 根据账号获取订单是否属于长包房
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号ID</param>
        /// <returns></returns>
        bool IsPermanentRoom(string hid, string regid);

        /// <summary>
        /// 根据账号获取订单的长包房设置
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">账号ID</param>
        /// <returns></returns>
        PermanentRoomInfo.PermanentRoomSet Get(string hid, string regid);

        /// <summary>
        /// 保存长包房设置
        /// </summary>
        /// <param name="model">数据</param>
        /// <returns></returns>
        JsonResultData Save(ICurrentInfo currentInfo, PermanentRoomInfo.PermanentRoomSetPara model);

        /// <summary>
        /// 获取所有长包房在住订单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, string>> GetAllPermanentRoomIOrder(string hid);

        /// <summary>
        /// 验证导入长包房账务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        JsonResultData CheckImportPermanentRoomFolio(ICurrentInfo currentInfo, string itemId, List<PermanentRoomInfo.PermanentRoomImportFolioPara> list);

        /// <summary>
        /// 获取最后抄表数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regidAndRoomNos">账号ID，房号</param>
        /// <param name="action">51水费，52电费，53燃气</param>
        List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, string>> GetLastTimeMeterReading(string hid, List<Gemstar.BSPMS.Common.Tools.KeyValuePairModel<string, string>> regidAndRoomNos, string action);

        /// <summary>
        /// 删除当天导入
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="shiftid">班次ID</param>
        /// <returns></returns>
        JsonResultData DeleteCurrentDayImport(string hid, string shiftid, string itemId);

        /// <summary>
        /// 获取长包房订单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="regid">订单ID</param>
        /// <returns></returns>
        Gemstar.BSPMS.Hotel.Services.Entities.ResDetail GetPermanentRoomOrder(string hid, string regid);

        /// <summary>
        /// 获取长包房 房号最后读数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="roomNo">房号</param>
        /// <returns></returns>
        PermanentRoomInfo.LastMeter UpGetPermanentRoomLastWaterAndElectricity(string hid, string roomNo);
    }
}
