using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// v_pos_ItemClassUnionItem 视图查询结果集对象
    /// </summary>
    public class v_pos_ItemClassUnionItemResult
    {
        public string Id { get; set; }

        public string Hid { get; set; }

        public string Code { get; set; }

        public string Cname { get; set; }

        public string Ename { get; set; }

        public string Module { get; set; }

        public bool? IsSubClass { get; set; }

        public int? Seqid { get; set; }

        public string ItemClassName { get; set; }
    }
}