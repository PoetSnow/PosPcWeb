using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_ActionMultisubByActionid 执行后的结果集对象
    /// </summary>
    public class up_pos_list_ActionMultisubByActionidResult
    {
        public Guid Id { get; set; }

        public string Hid { get; set; }

        public string Actionid { get; set; }

        public string ActionCode { get; set; }

        public string ActionName { get; set; }

        public string Actionid2 { get; set; }

        public string ActionCode2 { get; set; }

        public string ActionName2 { get; set; }

        public string Remark { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedStr { get; set; }
    }
}