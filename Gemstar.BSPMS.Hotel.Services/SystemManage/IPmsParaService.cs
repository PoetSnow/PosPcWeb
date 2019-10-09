using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using System.Collections.Generic;
namespace Gemstar.BSPMS.Hotel.Services
{
    /// <summary>
    /// 系统参数接口
    /// </summary>
   public interface IPmsParaService:ICRUDService<PmsPara>
    {
        /// <summary>
        /// 获取指定酒店的系统参数
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<PmsPara> GetPmsParas(string hid);
        /// <summary>
        /// 获取指定酒店的指定代码的系统参数
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="code">代码</param>
        /// <returns></returns>
        string GetValue(string hid, string code);
        /// <summary>
        /// 检查指定酒店是否启用清洁房检查功能
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>false:不启用，脏房转为净房，true:启用时，脏房先转为清洁房，</returns>
        bool IsRoomCheck(string hid);
        /// <summary>
        /// 检查指定酒店是否启用清洁房检查功能
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>false:不启用，脏房转为净房，true:启用时，脏房先转为清洁房，</returns>
        bool IsExistCleanRoom(string hid);
        /// <summary>
        /// 检查指定酒店是否启用短信模块，并且返回短信用户名和密码
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="username">短信用户名</param>
        /// <param name="password">短信密码</param>
        /// <returns>true:酒店或者酒店所属集团启用了短信模块，false:没有启用</returns>
        bool IsSmsEnable(string hid, out string username, out string password);
        /// <summary>
        /// 是否启用授权调价
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool IsAllowAuthAdjustPrice(string hid);
        /// <summary>
        /// 是否启用授权冲减账务
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool isAllowAuthAbatementFolio(string hid);

        bool isAllowOwner(string hid);

        /// <summary>
        /// 是否启用授权房租加收修改
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool isAllowAuthDayChargeFolio(string hid);

        /// <summary>
        /// 是否启用长租管理功能
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool IsPermanentRoom(string hid);

        /// <summary>
        /// 是否启用扫描身份证保存图片功能
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        bool IsScanSavePhoto(string hid);

        /// <summary>
        /// 是否启用脏房转净房生成报表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        bool IsDirtyLogToReportForm(string hid);

        /// <summary>
        /// 入帐付款后是否打印押金单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool IsPayPrintDeposit(string hid);

        /// <summary>
        /// 团体默认价格代码
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        string GroupDefaultRateCode(string hid);

        /// <summary>
        /// 入住时是否自动打印RC单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool IsCheckInAutoPrintRC(string hid);

        /// <summary>
        /// 获取酒店自定义logo和名称信息，以便前端界面根据此信息进行显示
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="hotelInfoService">用于获取酒店logo</param>
        /// <returns>酒店自定义logo和名称信息</returns>
        HotelLogoAndNameInfo GetHotelLogoAndNameInfo(string hid, IHotelInfoService hotelInfoService);


        /// <summary>
        /// 是否自动切换市别
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        bool IsSelfChangeShuffle(string hid);
        /// <summary>
        /// 是否打印会员结账单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns>true ： 是，false 否</returns>
        bool IsPrintMemberBill(string hid);


        /// <summary>
        /// 支付账单是否可以再次消费
        /// </summary>
        /// <returns></returns>
        bool IsPayOrderAgain(string hid);

    }
}
