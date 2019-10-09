using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 操作代码
    /// 0:初始化，将没有的房间加到这个表里面来，同时删除a多余的，在修改房间资料时调用
	///	1:开台，预订   
	///	2:维修停用等
	///	9:更新所有状态(保留，暂不支持)
    /// </summary>
    public enum PosTabStatusOpType : byte
    {
        初始化 = 0,
        开台预订 = 1,
        维修停用等 = 2,
        更新所有状态 = 9,
    }
}
