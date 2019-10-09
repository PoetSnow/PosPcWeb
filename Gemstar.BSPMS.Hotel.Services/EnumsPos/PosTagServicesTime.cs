using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    public enum PosTagServicesTime : byte
    {
        //0：开台时间；1：买单时间
        [Description("开台时间")]
        开台时间 = 0,
        [Description("买单时间")]
        买单时间 = 1
    }
}
