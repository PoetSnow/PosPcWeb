using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_ItemMultiClassByItemid 执行后的结果集对象
    /// </summary>
    public class up_pos_list_ItemMultiClassByItemidResult
    {
        public Guid Id { get; set; }

        public string itemName { get; set; }

        public string itemClassName { get; set; }

        public string isSubClassStr { get; set; }

        public string Remark { get; set; }

        public string ModifiedStr { get; set; }

        public string ItemClassidForEdit { get; set; }
    }
}
