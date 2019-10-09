using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("UserReportCollect")]
    [LogCName("用户报表收藏表")]
    public class UserReportCollect
    {
        [Key]
        [Column(Order = 1)]
        [LogCName("用户id")]
        public Guid UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        [LogCName("报表代码")]
        public string ReportCode { get; set; }
    }
}
