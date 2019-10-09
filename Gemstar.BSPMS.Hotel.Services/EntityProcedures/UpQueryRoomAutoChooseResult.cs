namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryRoomAutoChooseResult
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int seqid { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string roomno { get; set; }
        /// <summary>
        /// 房间前缀
        /// </summary>
        public string preFix { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public string Roomid { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int orderBy { get; set; }
        /// <summary>
        /// 是否在住
        /// </summary>
        public byte isReg { get; set; }
        /// <summary>
        /// 是否脏房
        /// </summary>
        public byte isDirty { get; set; }
        /// <summary>
        /// 是否维修
        /// </summary>
        public byte isService { get; set; }
        /// <summary>
        /// 是否停用
        /// </summary>
        public byte isStop { get; set; }
    }
}
