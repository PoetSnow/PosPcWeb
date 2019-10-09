using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Picture
{
    /// <summary>
    /// 房态过滤查询条件
    /// </summary>
    public class RoomStatusQueryModel
    {
        /// <summary>
        /// 关键字，房号或客人姓名,合约单位
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 客人来源id
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 房间类型id
        /// </summary>
        public string[] RoomType { get; set; }
        /// <summary>
        /// 是否空房
        /// </summary>
        public int IsEmpty { get; set; }
        /// <summary>
        /// 是否在用
        /// </summary>
        public int InUsing { get; set; }
        /// <summary>
        /// 是否脏房
        /// </summary>
        public int IsDirty { get; set; }
        /// <summary>
        /// 是否清洁房
        /// </summary>
        public int IsClean { get; set; }
        /// <summary>
        /// 是否预抵房
        /// </summary>
        public int IsArr { get; set; }
        /// <summary>
        /// 是否预离房
        /// </summary>
        public int IsDep { get; set; }
        /// <summary>
        /// 是否维修房
        /// </summary>
        public int IsService { get; set; }
        /// <summary>
        /// 是否停用
        /// </summary>
        public int IsStop { get; set; }
        /// <summary>
        /// 房间特色
        /// </summary>
        public string[] Features { get; set; }
        /// <summary>
        /// 是否钟点房
        /// </summary>
        public int IsHour { get; set; }
        /// <summary>
        /// 市场分类
        /// </summary>
        public string MarketType { get; set; }
        /// <summary>
        /// 是否净房
        /// </summary>
        public int IsEan { get; set; }
        /// <summary>
        /// 是否续住
        /// </summary>
        public int IsContinue { get; set; }

        /// <summary>
        /// 是否是客房
        /// </summary>
        //public int IsRoom { get; set; }

        /// <summary>
        /// 是否含假房
        /// </summary>
        public int IsNotRoom { get; set; }
    }
}