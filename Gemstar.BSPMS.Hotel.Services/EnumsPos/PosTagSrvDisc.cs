using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 服务费是否可折
    /// </summary>
    public enum PosTagSrvDisc:byte
    {
        //0：不可折；1：可折；2：按折扣金额收

        [Description("不可折")]
        不可折 =0,
        [Description("可折")]
        可折 =1,
        [Description("按折扣金额收")]
        按折扣金额收 =2
    }
}
