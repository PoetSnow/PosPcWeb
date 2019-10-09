namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryRoomStatusResult
    {
        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 房间类型id
        /// </summary>
        public string RoomTypeId { get; set; }
        /// <summary>
        /// 房类名称
        /// </summary>
        public string RoomTypeName { get; set; } 
        /// <summary>
        /// 房间名称简写
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 是否预抵
        /// </summary>
        public int IsArr { get; set; }
        /// <summary>
        /// 是否预离
        /// </summary>
        public int IsDep { get; set; }
        /// <summary>
        /// 0:净房 1:脏房
        /// </summary>
        public byte IsDirty { get; set; }
        /// <summary>
        /// 0:非维修  1: 维修
        /// </summary>
        public byte IsService { get; set; }
        /// <summary>
        /// 0：非停用  1：停用
        /// </summary>
        public byte IsStop { get; set; }
        /// <summary>
        /// 在住人包括同住人是今天生日 0：没有 1：有
        /// </summary>
        public byte IsBirth { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }
        /// <summary>
        /// 预抵的regid
        /// </summary>
        public string ArrId { get; set; }
        /// <summary>
        /// 预离的regid
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 房间特色
        /// </summary>
        public string Feature { get; set; }
        /// <summary>
        /// 客人来源
        /// </summary>
        public string SourceId { get; set; }
        /// <summary>
        /// 团体名
        /// </summary>
        public string ResName { get; set; }
        /// <summary>
        /// 在住的关联主单id , 是不是一个关系就是看这个字段的值是不是一样的
        /// </summary>
        public string ResId { get; set; }
        /// <summary>
        /// 是否空房
        /// </summary>
        public int IsEmpty { get; set; }
        /// <summary>
        /// 是否钟点房  0：不是钟点房， 1：是钟点房 2：钟点房到时提醒
        /// </summary>
        public int HouStatus { get; set; }

        /// <summary>
        /// 合约单位
        /// </summary>
        public string Cname { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string FloorName { get; set; }
        /// <summary>
        /// 是否假房
        /// </summary>
        public bool IsNotRoom { get; set; }
        /// <summary>
        /// 市场分类Id
        /// </summary>
        public string Marketid { get; set; }
        /// <summary>
        /// 是否续住
        /// </summary>
        public int IsContinue { get; set; }
        /// <summary>
        /// 预抵的客人名
        /// </summary>
        public string ArrGuestname { get; set; }
    }
}
