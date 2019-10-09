using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{

    [Table("PosHoliday")]
    [LogCName("节假日设置")]
    public class PosHoliday
    {
        [LogIgnore]
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("日期")]
        public string VDate { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("节日名")]
        public string DaysName { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }


        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }

    }

}
