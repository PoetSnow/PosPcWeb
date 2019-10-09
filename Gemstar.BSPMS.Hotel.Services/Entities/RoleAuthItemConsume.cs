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
        [Table("RoleAuthItemConsume")]
        [LogCName("消费入账权限表")]
        public class RoleAuthItemConsume
        {
            [Column(TypeName = "varchar")]
            [LogCName("酒店代码")]
            public string Grpid { get; set; }

            [Key]
            [Column(Order = 1)]
            [LogCName("角色id")]
            [LogAnywayWhenEdit]
            public Guid RoleId { get; set; }

            [Key]
            [Column(Order = 2)]
            [LogCName("消费项目编号")]
            public string ItemId { get; set; }

            [LogCName("是否权限")]
            public bool IsAllow { get; set; }

        }
    }
